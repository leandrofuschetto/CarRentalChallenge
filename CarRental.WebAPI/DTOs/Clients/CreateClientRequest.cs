using CarRental.WebAPI.Filters;
using System.ComponentModel.DataAnnotations;

namespace CarRental.WebAPI.DTOs.Client
{
    public class CreateClientRequest
    {
        [RequiredCustomAttribute(ErrorMessage = "Fullname is mandatory")]
        [StringLenghtAttribute(50, "Fullname max lenght is 50")]
        public string Fullname { get; set; }

        [RequiredCustomAttribute(ErrorMessage = "Email is mandatory")]
        [StringLenghtAttribute(100, "Fullname max lenght is 50")]
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
