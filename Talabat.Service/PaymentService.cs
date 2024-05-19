using Microsoft.Extensions.Configuration;
using Stripe;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order;
using Talabat.Core.Repositories.Interfaces;
using Talabat.Core.Services.Interfaces;
using Talabat.Core.Specifications.Order_Specs;
using Product = Talabat.Core.Entities.Product;

namespace Talabat.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public PaymentService(IBasketRepository basketRepository, IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _basketRepository = basketRepository;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

            public async Task<CustomerBasket?> CreateOrUpdatePaymentIntent(string basketId)
        {
            // Get Basket
            var basket =await _basketRepository.GetBasketAsync(basketId);
            if (basket is null) return null;


            //Get Total price
            if (basket.Items.Count > 0 )
            {
                foreach (var item in basket.Items)
                {
                    var product = await _unitOfWork.Repository<Product>().GetAsync(item.Id);
                    if (item.Price != product.Price)
                    {
                        item.Price = product.Price;
                    }
                }

            }
            // Get Subtotal and Shipping Price
            var subtotal = basket.Items.Sum(I => I.Price * I.Quantity);

            var shippingPrice = 0m;
            if (basket.DeliveryMethodId.HasValue)
            {
               var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetAsync(basket.DeliveryMethodId.Value);
                shippingPrice = deliveryMethod.Cost;
            }
            

            // call stripe
            StripeConfiguration.ApiKey = _configuration["StripeKeys:Secretkey"];
            var service = new PaymentIntentService();
            PaymentIntent paymentIntent;

            if (string.IsNullOrEmpty(basket.PaymentIntentId))
            {
                // Create PaymentIntent
                var Options = new PaymentIntentCreateOptions()
                {
                    Amount = (long)(subtotal * 100 + shippingPrice * 100),
                    Currency = "usd",
                    PaymentMethodTypes = new List<string>() { "card" },
                };
                paymentIntent = await service.CreateAsync(Options);
                basket.PaymentIntentId = paymentIntent.Id;
                basket.ClientSecret = paymentIntent.ClientSecret;
            }
            else
            {
                //Update payment intent
                var Options = new PaymentIntentUpdateOptions()
                {
                    Amount = (long)(subtotal * 100 + shippingPrice * 100)
                };
                paymentIntent = await service.UpdateAsync(basket.PaymentIntentId, Options);
                basket.PaymentIntentId = paymentIntent.Id;
                basket.ClientSecret = paymentIntent.ClientSecret;
            }
            await _basketRepository.UpdateBasketAsync(basket);
            return basket;
        }

        public async Task<Order> UpdatepaymentIntentToSuccessedOrFailed(string paymentIntentId, bool flag)
        {
            var spec = new orderWithPaymentIntentSpec(paymentIntentId);
            var order =await _unitOfWork.Repository<Order>().GetWithSpecAsync(spec);
            if (flag)
            {
                order.Status = OrderStatus.PaymentReceived;
            }
            else
            {
                order.Status = OrderStatus.PaymentFailed;
            }

            _unitOfWork.Repository<Order>().Update(order);
            await _unitOfWork.CompleteAsync();
            return order;
        }
    }
}
