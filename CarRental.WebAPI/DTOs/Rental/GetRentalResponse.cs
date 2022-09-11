using CarRental.WebAPI.DTOs.Client;
using CarRental.WebAPI.DTOs.Vehicle;

namespace CarRental.WebAPI.DTOs.Rental
{
    public class GetRentalResponse
    {
        public GetRentalResponse(Domain.Models.Rental rental)
        {
            this.Id = rental.RentalId;
            this.Vehicle = new GetVehicleResponse(rental.Vehicle);
            this.Client = new GetClientResponse(rental.Client);
            this.DateFrom = rental.DateFrom;
            this.DateTo = rental.DateTo;
            this.Price = rental.Price;
        }

        public int Id { get; set; }
        public GetVehicleResponse Vehicle { get; set; }
        public GetClientResponse Client { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public decimal Price { get; set; }
    }
}
