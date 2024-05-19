using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order;
using Talabat.Core.Repositories.Interfaces;
using Talabat.Core.Services.Interfaces;
using Talabat.Core.Specifications.Order_Specs;

namespace Talabat.Service
{
    public class OrderService : IOrderService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentService _paymentService;

        public OrderService(IBasketRepository basketRepository, IUnitOfWork unitOfWork , IPaymentService paymentService)
        {
            _basketRepository = basketRepository;
            _unitOfWork = unitOfWork;
           _paymentService = paymentService;
        }
        public async Task<Order?> CreateOrderAsync(string BuyerEmail, Address shippingAddress, string basketId, int DeliverMethodId)
        {
            //1 Get Basket From Basket repository
            var basket = await _basketRepository.GetBasketAsync(basketId);
            //2 Get Selected Items from Basket
            var OrderItem = new List<OrderItem>();
            if (basket?.Items.Count()>0)
            {
                foreach (var item in basket.Items)
                {
                    var product = await _unitOfWork.Repository<Product>().GetAsync(item.Id); 
                    // every item is product was put in basket so we want the products
                    var productOrderItem = new ProductOrderItem(product.Id, product.Name, product.PictureUrl);
                    var orderItem = new OrderItem(productOrderItem,item.Price,item.Quantity);
                    
                    OrderItem.Add(orderItem);
                   
                }
            }
            //3 calculate subtotal 
            var subtotal = OrderItem.Sum(o => o.Price * o.Quantity);
            //4 Get deliveryMethod
            var deliveryMethod =await _unitOfWork.Repository<DeliveryMethod>().GetAsync(DeliverMethodId);
            // Get paymentintentId
            
            var Spec = new orderWithPaymentIntentSpec(basket.PaymentIntentId);
            var exOrder = await _unitOfWork.Repository<Order>().GetWithSpecAsync(Spec);
            if (exOrder is not null)
            {
                _unitOfWork.Repository<Order>().Delete(exOrder);
               await _paymentService.CreateOrUpdatePaymentIntent(basketId);
            }
            //5 create order
            var order = new Order(BuyerEmail,shippingAddress,deliveryMethod,OrderItem,subtotal,basket.PaymentIntentId);
            //6 add order locally
            await _unitOfWork.Repository<Order>().AddAsync(order);
            //7 save order to database
            var result =await _unitOfWork.CompleteAsync();
            if (result <= 0) return null;
            return order;

        }

        public Task<Order?> GetOrderByIdForSpecificUserAsync(string BuyerEmail, int OrderId)
        {
            var specs = new OrderSpecifications(BuyerEmail, OrderId);
            var order = _unitOfWork.Repository<Order>().GetWithSpecAsync(specs);
            return order;
        }

        public Task<IReadOnlyList<Order?>> GetordersForSpecificUserAsync(string BuyerEmail)
        {
            var spec = new OrderSpecifications(BuyerEmail);
            var orders = _unitOfWork.Repository<Order>().GetWithSpecAllAsync(spec);
            return orders;
        }
    }
}
