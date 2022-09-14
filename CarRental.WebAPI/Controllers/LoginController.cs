using CarRental.Service.Users;
using CarRental.WebAPI.DTOs.User;
using CarRental.WebAPI.Exceptions;
using CarRental.WebAPI.Filters;
using CarRental.WebAPI.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.WebAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly IUsersService _usersService;
        private readonly JwtHelper _jwtHelper;
        
        public LoginController(IUsersService userServices, JwtHelper jwtHelper)
        {
            _usersService = userServices;
            _jwtHelper = jwtHelper;
        }

        [HttpPost, AllowAnonymousCustom]
        public async Task<ActionResult<string>> Login(LoginRequest loginDTO)
        {
            var user = await _usersService
                .Authenticate(loginDTO.UserName, loginDTO.Password);

            if (user == null)
                throw new WrongCredentialsException($"Wrong Credentials");

            var token = _jwtHelper.GenerateToken(user);

            if (token != null)
                return Ok(token);
            else
                throw new Exception($"Unable to generate token");
        }
    }
}
