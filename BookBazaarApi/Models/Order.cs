namespace BookBazaarApi.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public int DeliveryStatusId { get; set; }
        public int PaymentTypeId { get; set; }
        public int AddressId { get; set; }
        public Address Address { get; set; }

        public DeliveryStatus DeliveryStatus { get; set; }
        public PaymentType PaymentType { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
