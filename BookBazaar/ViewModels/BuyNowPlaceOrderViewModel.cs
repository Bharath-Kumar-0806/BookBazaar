namespace BookBazaar.ViewModels
{
    public class BuyNowPlaceOrderViewModel
    {
        public int? SelectedAddressId { get; set; }
        public string Full_Name { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string House_No { get; set; }
        public string Phone { get; set; }
        public bool SaveNewAddress { get; set; }
        public int PaymentTypeId { get; set; }
        public int BookId { get; set; }
        public int Quantity { get; set; } = 1;
        public decimal UnitPrice { get; set; }
        public decimal Subtotal => Quantity * UnitPrice;
        public decimal ShippingCost => 0;
        public decimal Total => Subtotal + ShippingCost;
    }
}
