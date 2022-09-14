using CarRental.Data.DAOs.User;
using CarRental.Domain.Exceptions;
using CarRental.Domain.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CarRental.Service.Users
{
    public class UsersService : IUsersService
    {
        private readonly IUserDao _userDao;

        public UsersService(IUserDao userDao)
        {
            _userDao = userDao;
        }

        public async Task<User> Authenticate(string username, string password)
        {
            return await _userDao.Authenticate(username, password);
        }

        public async Task<bool> CreateUser(string username, string password)
        {
            if (await _userDao.UserExist(username))
                throw new UsernameInUseException($"Username {username} is taken");

            return await _userDao.CreateUser(username, password);
        }

        public async Task<User> GetUserById(int id)
        {
            var user = await _userDao.GetUser(id);

            return user;
        }
    }
}
