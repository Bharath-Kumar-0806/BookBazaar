namespace BookBazaar.DTOs

{
    public class CartAddRequest
    {
        public string UserName { get; set; }
        public int BookId { get; set; }
        public int Quantity { get; set; }
    }
}
