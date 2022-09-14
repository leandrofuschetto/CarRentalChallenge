using CarRental.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRental.Service.Users
{
    public interface IUsersService
    {
        Task<bool> CreateUser(string username, string password);
        Task<User> Authenticate(string username, string password);
        Task<User> GetUserById(int id);
    }
}
