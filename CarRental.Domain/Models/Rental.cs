namespace CarRental.Domain.Models
{
    public class Rental
    {
        public int Id { get; init; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public decimal Price { get; set; }
        public bool Active { get; set; }
        public virtual Vehicle Vehicle { get; set; }
        public virtual Client Client { get; set; }
    }
}
