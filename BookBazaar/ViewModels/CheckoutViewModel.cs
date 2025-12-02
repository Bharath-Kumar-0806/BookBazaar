namespace BookBazaar.ViewModels
{
    public class CheckoutViewModel
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
        //public string PaymentMethodName { get; set; }
        public List<CartItemViewModel> Items { get; set; }
        public decimal TotalAmount => Items?.Sum(i => i.Quantity * i.UnitPrice) ?? 0;
    }
}
