using BookBazaar.Models;

namespace BookBazaar.ViewModels
{
    public class CartViewModel
    {
        public List<CartItemViewModel> Items { get; set; }
        public decimal Subtotal => Items.Sum(i => i.TotalPrice);
        public decimal Shipping => 0;
        public decimal Total => Subtotal + Shipping;
        public int? AddressId { get; set; }
        //public int PaymentTypeId { get; set; }
        public List<PaymentType> PaymentType { get; set; }
        public List<CheckOutAddressViewModel> SavedAddresses { get; set; }
    }
}
