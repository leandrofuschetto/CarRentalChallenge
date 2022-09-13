namespace CarRental.Domain.Models
{
    public class Vehicle
    {
        public int Id { get; init; }
        public string Model { get; set; }
        public decimal PricePerDay { get; set; }
        public bool Active { get; set; }
    }
}
