using CarRental.WebAPI.Filters;

namespace CarRental.WebAPI.DTOs.Rental
{
    public class CreateRentalRequest
    {
        [RequiredAttribute(ErrorMessage = "ClientId is mandatory")]
        public int ClientId { get; set; }

        [RequiredAttribute(ErrorMessage = "VehicleId is mandatory")]
        public int VehicleId { get; set; }

        [RequiredAttribute(ErrorMessage = "DateFrom is mandatory")]
        public DateTime DateFrom { get; set; }

        [RequiredAttribute(ErrorMessage = "DateTo is mandatory")]
        public DateTime DateTo { get; set; }

        public Domain.Models.Rental ToDomain()
        {
            return new Domain.Models.Rental()
            {
                Client = new Domain.Models.Client() { ClientId = this.ClientId },
                Vehicle = new Domain.Models.Vehicle() { VehicleId = this.VehicleId },
                DateFrom = this.DateFrom,
                DateTo = this.DateTo
            };
        }
    }
}
