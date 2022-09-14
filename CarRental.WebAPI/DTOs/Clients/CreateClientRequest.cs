using CarRental.WebAPI.Filters;
using System.ComponentModel.DataAnnotations;

namespace CarRental.WebAPI.DTOs.Client
{
    public class CreateClientRequest
    {
        [RequiredCustomAttribute(ErrorMessage = "Fullname is mandatory")]
        [StringLenghtAttribute(50, 5, "Fullname max lenght is 50, min is 5")]
        public string Fullname { get; set; }

        [RequiredCustomAttribute(ErrorMessage = "Email is mandatory")]
        [StringLenghtAttribute(100, 1, "Fullname max lenght is 100")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        public Domain.Models.Client ToDomain()
        {
            return new Domain.Models.Client()
            {
                Email = this.Email,
                Fullname = this.Fullname,
            };
        }
    }
}
