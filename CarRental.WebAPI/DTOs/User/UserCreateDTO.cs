using CarRental.WebAPI.Filters;
using System.ComponentModel.DataAnnotations;

namespace CarRental.WebAPI.DTOs.User
{
    public class UserCreateDTO
    {
        [Required]
        [StringLenghtAttribute(20, 5, "Username max lenght is 20, min is 5")]
        public string UserName { get; set; }

        [Required]
        [StringLenghtAttribute(20, 5, "Password max lenght is 20, min is 5")]
        public string Password { get; set; }
    }
}
