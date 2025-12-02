namespace BookBazaar.DTOs
{
    public class CartUpdateRequest
    {
        public string UserName { get; set; }
        public int CartItemId { get; set; }
        public int Quantity { get; set; }
    }
}
