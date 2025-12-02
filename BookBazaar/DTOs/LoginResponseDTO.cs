namespace BookBazaar.DTOs
{
    public class LoginResponseDTO
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public List<string> Roles { get; set; }
    }
}
