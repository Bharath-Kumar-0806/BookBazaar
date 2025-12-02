using BookBazaarApi.Models;

namespace BookBazaarApi.ViewModels
{
    public class OrdersManageVM
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = "Pending";
        public int DeliveryStatusId { get; set; }
        public List<DeliveryStatus> AvailableStatuses { get; set; }
    }
}
