namespace CarRental.Domain.Models
{
    public class Client
    {
        public int Id { get; init; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public bool Active { get; set; }
    }
}
