namespace CarRental.Data.DAOs.User
{
    public interface IUserDao
    {
        Task<Domain.Models.User> Authenticate(string username, string password);
        Task<Domain.Models.User> GetUser(int id);
        Task<bool> CreateUser(string username, string password);
        Task<bool> UsernameExist(string username);
    }
}
