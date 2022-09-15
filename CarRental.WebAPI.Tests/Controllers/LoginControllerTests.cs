using CarRental.Domain.Models;
using CarRental.Service.Users;
using CarRental.WebAPI.Controllers;
using CarRental.WebAPI.DTOs.User;
using CarRental.WebAPI.Exceptions;
using CarRental.WebAPI.Helpers;
using CarRental.WebAPI.Tests.Fakes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Net;
using Xunit;

namespace CarRental.WebAPI.Tests.Controllers
{
    public class LoginControllerTests
    {
        private LoginController _loginController;
        private Mock<IUsersService> _usersServiceMock;
        private Mock<IConfiguration> _configuration;
        private Mock<JwtHelper> _jwtHelperMock;

        public LoginControllerTests()
        {
            _usersServiceMock = new Mock<IUsersService>();
            _configuration = new Mock<IConfiguration>();
            _jwtHelperMock = new Mock<JwtHelper>(_configuration.Object);

            _loginController = new LoginController
                (_usersServiceMock.Object, _jwtHelperMock.Object);
        }

        [Fact]
        public async Task Login_RequestCorrect_ReturnToken()
        {
            string tokenExpected = "token1234";
            LoginRequest loginRequest = GetLoginRequestFake();
            User userReturn = GetUserReturnDao();
            _usersServiceMock.Setup(u => u.Authenticate(
                It.IsAny<string>(),
                It.IsAny<string>()))
                .ReturnsAsync(userReturn);
            _jwtHelperMock.Setup(j => j.GenerateToken(userReturn))
                .Returns(tokenExpected);

            var token = await _loginController
                .Login(loginRequest);

            Assert.IsType<OkObjectResult>(token.Result);
            var result = token.Result as OkObjectResult;
            Assert.Equal(((int)HttpStatusCode.OK), result.StatusCode);
            Assert.Equal(tokenExpected, result.Value);
        }

        [Fact]
        public async Task Login_UserNotExist_ThrowException()
        {
            string exMsgExpected = "Wrong Credentials";
            LoginRequest loginRequest = GetLoginRequestFake();
            User userNullReturn = null;
            _usersServiceMock.Setup(u => u.Authenticate(
                It.IsAny<string>(),
                It.IsAny<string>()))
                .ReturnsAsync(userNullReturn);

            Func<Task> action = async () =>
            {
                await _loginController.Login(loginRequest);
            };
            var ex = await Assert.ThrowsAsync<WrongCredentialsException>(action);

            Assert.IsType<WrongCredentialsException>(ex);
            Assert.Equal(exMsgExpected, ex.Message);
        }

        [Fact]
        public async Task Login_TokenCreatedNull_ThrowException()
        {
            string tokennull = null;
            string exMsgExpected = "Unable to generate token";
            LoginRequest loginRequest = GetLoginRequestFake();
            User userReturn = GetUserReturnDao();
            _usersServiceMock.Setup(u => u.Authenticate(
                It.IsAny<string>(),
                It.IsAny<string>()))
                .ReturnsAsync(userReturn);
            _jwtHelperMock.Setup(j => j.GenerateToken(userReturn))
                .Returns(tokennull);

            Func<Task> action = async () =>
            {
                await _loginController.Login(loginRequest);
            };
            var ex = await Assert.ThrowsAsync<Exception>(action);

            Assert.IsType<Exception>(ex);
            Assert.Equal(exMsgExpected, ex.Message);
        }

        private User GetUserReturnDao()
        {
            return new User
            {
                Id = 1,
                Password = "leitan",
                Username = "passleitan"
            };
        }

        private LoginRequest GetLoginRequestFake()
        {
            return new LoginRequest
            {
                UserName = "isa",
                Password = "passisa"
            };
        }
    }
}
