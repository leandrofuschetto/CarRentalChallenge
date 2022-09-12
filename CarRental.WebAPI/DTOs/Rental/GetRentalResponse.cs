using CarRental.WebAPI.DTOs.Client;
using CarRental.WebAPI.DTOs.Vehicle;

namespace CarRental.WebAPI.DTOs.Rental
{
    public class GetRentalResponse
    {
        public int Id { get; set; }
        public GetVehicleResponse Vehicle { get; set; }
        public GetClientResponse Client { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public decimal Price { get; set; }
        public bool Active { get; set; }

        public GetRentalResponse FromDomain(Domain.Models.Rental rental)
        {
            this.Id = rental.Id;
            this.Client = new GetClientResponse()
            {
                Id = rental.Client.Id,
                Email = rental.Client.Email,
                Fullname = rental.Client.Fullname
            };
            this.Vehicle = new GetVehicleResponse()
            {
                Id = rental.Vehicle.Id,
                Model = rental.Vehicle.Model,
                PricePerDay = rental.Vehicle.PricePerDay
            };
            this.DateFrom = rental.DateFrom;
            this.DateTo = rental.DateTo;
            this.Price = rental.Price;
            this.Active = rental.Active;

            return this;
        }
    }
}
