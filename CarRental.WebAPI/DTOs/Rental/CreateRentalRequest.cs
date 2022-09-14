using CarRental.WebAPI.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace CarRental.WebAPI.DTOs.Rental
{
    public class CreateRentalRequest
    {
        [RequiredAttribute(ErrorMessage = "ClientId is mandatory")]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public int ClientId { get; set; }

        [RequiredAttribute(ErrorMessage = "VehicleId is mandatory")]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public int VehicleId { get; set; }

        [RequiredAttribute(ErrorMessage = "DateFrom is mandatory")]
        public DateTime DateFrom { get; set; }

        [RequiredAttribute(ErrorMessage = "DateTo is mandatory")]
        public DateTime DateTo { get; set; }

        public Domain.Models.Rental ToDomain()
        {
            ValidDates();
            return new Domain.Models.Rental()
            {
                Client = new Domain.Models.Client() { Id = this.ClientId },
                Vehicle = new Domain.Models.Vehicle() { Id = this.VehicleId },
                DateFrom = DateOnly.FromDateTime(this.DateFrom),
                DateTo = DateOnly.FromDateTime(this.DateTo),
            };
        }

        private void ValidDates()
        {
            if (this.DateFrom < DateTime.Now.Date)
                throw new DatesInvalidException("DateFrom must be higher or equal today");
             
            if (this.DateFrom > this.DateTo)
                throw new DatesInvalidException("DateFrom can'be higher than DateTo");
        }
    }
}
