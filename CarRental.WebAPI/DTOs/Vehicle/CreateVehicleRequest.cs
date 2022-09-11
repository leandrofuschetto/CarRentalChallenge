using CarRental.WebAPI.Filters;
using System.ComponentModel.DataAnnotations;
using RequiredAttribute = CarRental.WebAPI.Filters.RequiredAttribute;

namespace CarRental.WebAPI.DTOs.Vehicle
{
    public class CreateVehicleRequest
    {
        [RequiredAttribute(ErrorMessage = "Model is mandatory")]
        [StringLenghtAttribute(250, "Fullname max lenght is 250")]
        public string Model { get; set; }

        [RequiredAttribute(ErrorMessage = "Price per Day is mandatory")]
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
