using CarRental.Service.Users;
using CarRental.WebAPI.Controllers;
using CarRental.WebAPI.DTOs.User;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CarRental.WebAPI.Tests.Controllers
{
    public class UsersControllerTests
    {
        private UsersController _usersController;
        private Mock<IUsersService> _usersService;
        
        public UsersControllerTests()
        { 
            _usersService = new Mock<IUsersService>();
            _usersController = new UsersController(_usersService.Object);   
        }

        [Fact]
        public async Task CreateUser_RequestCorrect_ReturnOk()
        {
            _usersService.Setup(u => u.CreateUser(
                It.IsAny<string>(), 
                It.IsAny<string>()))
                .ReturnsAsync(true);

            var returnController = await _usersController
                .CreateUser(new DTOs.User.CreateUserRequest());

            Assert.IsType<OkObjectResult>(returnController.Result);
            var result = returnController.Result as OkObjectResult;

            Assert.Equal(((int)HttpStatusCode.OK), result.StatusCode);
        }

        [Fact]
        public async Task CreateUser_RequestIncorrect_ThrowException()
        {
            CreateUserRequest userRequest = new()
            {
                UserName = "leitan",
                Password = " pass1234"
            };

            string exMsgExpected = "Unable to create User";
            _usersService.Setup(u => u.CreateUser(
                It.IsAny<string>(),
                It.IsAny<string>()))
                .ReturnsAsync(false);
            
            Func<Task> action = async () =>
            {
                await _usersController.CreateUser(new DTOs.User.CreateUserRequest());
            }; 

            var ex = await Assert.ThrowsAsync<Exception>(action);

            Assert.IsType<Exception>(ex);
            Assert.Equal(exMsgExpected, ex.Message);
        }
    }
}
