namespace CarRental.Domain.Models
{
    public class Rental
    {
        public int Id { get; init; }
        public DateOnly DateFrom { get; set; }
        public DateOnly DateTo { get; set; }
        public decimal Price { get; set; }
        public bool Active { get; set; }
        public virtual Vehicle Vehicle { get; set; }
        public virtual Client Client { get; set; }
    }
}
