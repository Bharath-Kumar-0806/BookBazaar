namespace BookBazaar.Models
{
    public class Address
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Full_Name { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string House_No { get; set; }
        public string Phone { get; set; }
        public bool SaveNewAddress { get; set; }

    }
}
