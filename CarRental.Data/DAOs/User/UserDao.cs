using AutoMapper;
using CarRental.Data.Entities;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace CarRental.Data.DAOs.User
{
    public class UserDao : IUserDao
    {
        private readonly CarRentalContext _context;
        private readonly IMapper _mapper;

        public UserDao(CarRentalContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Domain.Models.User> Authenticate(string username, string password)
        {
            var result = await _context.Users
                    .FirstOrDefaultAsync(x => x.Username == username);

            if (result == null)
                return null;

            string passwordHash = HashPassword(password, result.Salt);

            if (result.Password == passwordHash)
                return _mapper.Map<Domain.Models.User>(result);

            return null;
        }

        public async Task<Domain.Models.User> GetUser(int id)
        {
            var result = await _context.Users
                    .FirstOrDefaultAsync(x => x.Id == id);

            return _mapper.Map<Domain.Models.User>(result);
        }

        public async Task<bool> CreateUser(string username, string password)
        {
            var saltByte = GetRandomSalt();

            string saltString = Convert.ToBase64String(saltByte);
            string passHashed = HashPassword(password, saltString);

            UserEntity user = new()
            {
                Username = username,
                Password = passHashed,
                Salt = saltString
            };

            await _context.Users.AddAsync(user);

            return (_context.SaveChanges() >0);
        }

        public async Task<bool> UsernameExist(string username)
        {
            var result = await _context.Users
                    .AnyAsync(x => x.Username == username);

            return result;
        }

        private byte[] GetRandomSalt()
        {
            return RandomNumberGenerator.GetBytes(128 / 8);
        }

        private string HashPassword(string password, string salt)
        {
            byte[] saltBytes = Convert.FromBase64String(salt);

            string pass = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password!,
                salt: saltBytes,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

            return pass;
        }
    }
}
