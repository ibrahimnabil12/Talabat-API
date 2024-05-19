using AutoMapper;
using Talabat.APIs.Dtos;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order;

namespace Talabat.APIs.Helpers
{
    public class OrderItemPictureUrlResolver : IValueResolver<OrderItem,OrderItemDto , string>
    {
        private readonly IConfiguration _configuration;
        public OrderItemPictureUrlResolver(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string Resolve(OrderItem source, OrderItemDto destination, string destMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(source.Product.PictureUrl))
            {
                return $"{_configuration["BaseUrl"]}/{source.Product.PictureUrl}";
            }
            return string.Empty;
        }
    }
}
