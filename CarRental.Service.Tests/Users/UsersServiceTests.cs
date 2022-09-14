using CarRental.Data.DAOs.User;
using CarRental.Domain.Exceptions;
using CarRental.Domain.Models;
using CarRental.Service.Users;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CarRental.Service.Tests.Users
{
    public class UsersServiceTests
    {
        private Mock<IUserDao> _usersDaoMock;
        private UsersService _usersService;
        public UsersServiceTests()
        {
            _usersDaoMock = new Mock<IUserDao>();
            _usersService = new UsersService(_usersDaoMock.Object);
        }

        [Fact]
        public async Task Authenticate_CorrectData_ReturnsUser()
        {
            User userExpected = Result_Dao_User();

            _usersDaoMock.Setup(u => u.Authenticate(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(userExpected);

            var result = await _usersService.Authenticate("username", "password");

            Assert.NotNull(result);
            Assert.Equal(userExpected.Username, result.Username);
            Assert.Equal(userExpected.Password, result.Password);
        }

        [Fact]
        public async Task Authenticate_WrongCredentials_ReturnNull()
        {
            User userExpected = null;
            _usersDaoMock.Setup(u => u.Authenticate(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(userExpected);

            var result = await _usersService.Authenticate("username", "password");

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateUser_UsernameExits_ThrowException()
        {
            string username = "userFake";
            string exCodeExpected = "USERNAME_IN_USE";
            string exMsgExpected = $"Username {username} is taken";
            _usersDaoMock.Setup(u => u.UsernameExist(It.IsAny<string>()))
                .ReturnsAsync(true);

            Func<Task> action = async () =>
            {
                await _usersService.CreateUser(username, "password");
            };

            var ex = await Assert.ThrowsAsync<UsernameInUseException>(action);

            Assert.IsType<UsernameInUseException>(ex);
            Assert.Equal(exMsgExpected, ex.Message);
            Assert.Equal(exCodeExpected, ex.Code);
        }

        [Fact]
        public async Task CreateUser_UsernameAvailable_ReturnNewUser()
        {
            _usersDaoMock.Setup(u => u.UsernameExist(It.IsAny<string>()))
                .ReturnsAsync(false);
            _usersDaoMock.Setup(u => u.CreateUser(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            var result = await _usersService.CreateUser("username", "password");

            Assert.True(result);
        }

        [Fact]
        public async Task GetUserById_UserExits_ReturnUser()
        {
            int id = 1;
            User userReturn = new()
            {
                Id = 1,
                Username = "leitan",
                Password = "pass1234"
            };

            _usersDaoMock.Setup(u => u.GetUser(id))
                .ReturnsAsync(userReturn);

            var result = await _usersService.GetUserById(id);

            Assert.NotNull(result);
            Assert.Equal(userReturn.Username, result.Username);
            Assert.Equal(userReturn.Password, result.Password);
        }

        [Fact]
        public async Task GetUserById_UserNoExist_ReturnNull()
        {
            int id = 1;
            User userNull = null;
            
            _usersDaoMock.Setup(u => u.GetUser(id))
                .ReturnsAsync(userNull);

            var result = await _usersService.GetUserById(id);

            Assert.Null(result);
        }

        private User Result_Dao_User()
        {
            return new User()
            {
                Id = 1,
                Username = "Leitan",
                Password = "qaz123.com"
            };
        }
    }
}
