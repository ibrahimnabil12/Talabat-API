using Talabat.Core.Entities.Order;

namespace Talabat.APIs.Dtos
{
    public class OrderToReturnDto
    {
        public int Id { get; set; } // 5
        public string BuyerEmail { get; set; }
        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.Now;
        public string Status { get; set; } //1
        public Address ShippingAddress { get; set; }
        public string DeliveryMethod { get; set; } //2
        public decimal DeliveryMethodCost { get; set; } // 3
        public ICollection<OrderItemDto> Items { get; set; } = new HashSet<OrderItemDto>();
        public decimal SubTotal { get; set; }
       public decimal Total { get; set; } // 4
        public string PaymentIntentId { get; set; } = string.Empty;
    }
}
