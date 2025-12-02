namespace BookBazaarApi.DTOs
{
    public class LoginResponse
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public List<string> Roles { get; set; }
    }
}
