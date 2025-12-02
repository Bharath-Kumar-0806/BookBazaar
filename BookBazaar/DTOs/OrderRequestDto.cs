namespace BookBazaar.DTOs
{
    public class OrderRequestDto
    {
        public string UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public int PaymentTypeId { get; set; }
        public int AddressId { get; set; }
        public List<OrderItemDto> Items { get; set; }
    }
}
