namespace BookBazaar.DTOs
{
    public class BuyNowDTO
    {
        public string UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public int PaymentTypeId { get; set; }
        public int AddressId { get; set; }
        public int BookId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
