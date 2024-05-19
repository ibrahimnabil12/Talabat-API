using AutoMapper;
using Talabat.APIs.Dtos;
using Talabat.Core.Entities;
//using UserAddress = Talabat.Core.Entities.Identity.Address;
//using OrderAddress = Talabat.Core.Entities.Order.Address;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Entities.Order;


namespace Talabat.APIs.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductToReturnDto>()
                .ForMember(d => d.ProductBrand, O => O.MapFrom(s => s.ProductBrand.Name))
                .ForMember(d => d.ProductType, O => O.MapFrom(s => s.ProductType.Name))
                .ForMember(d => d.PictureUrl, O => O.MapFrom<ProductPictureUrlResolver>());

            CreateMap<Core.Entities.Identity.Address, AddressDto>();
            CreateMap<AddressDto, Core.Entities.Order.Address>()
                .ForMember(D => D.FirstName, O => O.MapFrom(S => S.FirstName))
                .ForMember(D => D.LastName, O => O.MapFrom(S => S.LastName));


            CreateMap<Order, OrderToReturnDto>()
                .ForMember(D => D.DeliveryMethod, O => O.MapFrom(s => s.DeliveryMethod.ShortName))
                .ForMember(D => D.DeliveryMethodCost, O => O.MapFrom(s => s.DeliveryMethod.Cost));

            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(D => D.ProductId, o => o.MapFrom(s => s.Product.ProductId))
                .ForMember(D => D.ProductName, o => o.MapFrom(s => s.Product.ProductName))
                .ForMember(D => D.PictureUrl, o => o.MapFrom<OrderItemPictureUrlResolver>());

            CreateMap<CustomerBasket, CustomerBasketDto>().ReverseMap();
        }
    }
}
