using BookBazaar.Models;

namespace BookBazaar.ViewModels
{
    public class BuyNowViewModel
    {
        public int BookId { get; set; }
        public string BookTitle { get; set; }
        public int Quantity { get; set; } = 1;
        public decimal UnitPrice { get; set; }
        public decimal Subtotal => Quantity * UnitPrice;
        public decimal ShippingCost => 0;
        public decimal Total => Subtotal + ShippingCost;
        public int? AddressId { get; set; }
        public int  PaymentTypeId { get; set; }
        public List<PaymentType> PaymentType { get; set; }
        public List<CheckOutAddressViewModel> SavedAddresses { get; set; }
    }
}
