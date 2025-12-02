using BookBazaarApi.Models;
using System.Reflection.PortableExecutable;

namespace BookBazaarApi.ViewModels
{
    public class OrderSummaryVM
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public int PaymentTypeId { get; set; }
        public string PaymentMethodName { get; set; }
        public int DeliveryStatusId { get; set; }
        public string DeliveryStatusName { get; set; }
        public string ShippingAddress { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Shipping { get; set; } = 0;
        public decimal Total => Subtotal + Shipping;
        public List<OrderItemVM> Items { get; set; }
    }
}
