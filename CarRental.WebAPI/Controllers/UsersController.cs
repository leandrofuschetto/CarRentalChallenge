using CarRental.Service.Users;
using CarRental.WebAPI.DTOs.User;
using CarRental.WebAPI.Filters;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.WebAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UsersController : ControllerBase   
    {
        private readonly IUsersService _usersService;
        
        public UsersController(IUsersService userServices)
        {
            _usersService = userServices;
        }

        [HttpPost, AllowAnonymousCustom]
        public async Task<ActionResult<UserCreateDTO>> CreateUser(UserCreateDTO loginDTO)
        {
            bool result = await _usersService.CreateUser(loginDTO.UserName, loginDTO.Password);

            if (result)
                return Ok(loginDTO.UserName);
            else
                throw new Exception($"Unable to create User");
        }
    }
}
