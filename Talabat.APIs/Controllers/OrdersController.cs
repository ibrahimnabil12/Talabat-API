using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Talabat.APIs.Dtos;
using Talabat.APIs.Errors;
using Talabat.Core;
using Talabat.Core.Entities.Order;
using Talabat.Core.Services.Interfaces;

namespace Talabat.APIs.Controllers
{

    public class OrdersController : BaseApiController
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public OrdersController(IOrderService orderService, IMapper mapper , IUnitOfWork unitOfWork)
        {
            _orderService = orderService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }


        [HttpPost]
        [Authorize]
        public async Task<ActionResult<OrderToReturnDto>> CreateOrder(OrderDto model)
        {
            var buyerEmail = User.FindFirstValue(ClaimTypes.Email);
            var address = _mapper.Map<AddressDto, Address>(model.shipToAddress);
            var order = await _orderService.CreateOrderAsync(buyerEmail, address, model.BasketId, model.DeliveryMethodId);
            if (order is null) return BadRequest(new ApiResponse(400, "there is problem with order"));


            var result = _mapper.Map<Order, OrderToReturnDto>(order);

            return Ok(result);
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IReadOnlyList<OrderToReturnDto>>> GetOrdersForUser()
        {
            var buyerEmail = User.FindFirstValue(ClaimTypes.Email);
            var orders = await _orderService.GetordersForSpecificUserAsync(buyerEmail);
            if (orders is null) return NotFound(new ApiResponse(400, "There is no orders"));
            var result = _mapper.Map<IReadOnlyList<OrderToReturnDto>>(orders);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<OrderToReturnDto>> GetOrderByIdForUser(int id)
        {
            var buyerEmail = User.FindFirstValue(ClaimTypes.Email);
            var order = await _orderService.GetOrderByIdForSpecificUserAsync(buyerEmail, id);
            if (order is null) return NotFound(new ApiResponse(404, "There is no order"));
            var result = _mapper.Map<OrderToReturnDto>(order);
            return Ok(result);
        }


        [HttpGet("DeliveryMethods")]
        public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods()
        {
         var deliveryMethos = await  _unitOfWork.Repository<DeliveryMethod>().GetAllAsync();
            return Ok(deliveryMethos);
            
        }
    }
}
