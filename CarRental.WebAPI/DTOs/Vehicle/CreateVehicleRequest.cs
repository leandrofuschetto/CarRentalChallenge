using CarRental.WebAPI.Filters;
using System.ComponentModel.DataAnnotations;

namespace CarRental.WebAPI.DTOs.Vehicle
{
    public class CreateVehicleRequest
    {
        [RequiredCustomAttribute(ErrorMessage = "Model is mandatory")]
        [StringLenghtAttribute(250, "Fullname max lenght is 250")]
        public string Model { get; set; }

        [RequiredCustomAttribute(ErrorMessage = "Price per Day is mandatory")]
        [Range(1, 100)]
        public int PricePerDay { get; set; }

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
