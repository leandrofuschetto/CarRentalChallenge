using CarRental.WebAPI.Filters;
using System.ComponentModel.DataAnnotations;

namespace CarRental.WebAPI.DTOs.Vehicle
{
    public class CreateVehicleRequest
    {
        [RequiredCustomAttribute(ErrorMessage = "Model is mandatory")]
        [StringLenghtAttribute(250, 5, "Fullname max lenght is 250, min is 5")]
        public string Model { get; set; }

        [RequiredCustomAttribute(ErrorMessage = "Price per Day is mandatory")]
        [Range(1, 999999999, ErrorMessage = "Please enter a value bigger than {1}")]
        public decimal PricePerDay { get; set; }

        public Domain.Models.Vehicle ToDomain()
        {
            return new Domain.Models.Vehicle()
            {
                Model = this.Model,
                PricePerDay = this.PricePerDay,
            };
        }
    }
}
