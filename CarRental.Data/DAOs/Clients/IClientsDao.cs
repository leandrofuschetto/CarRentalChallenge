using CarRental.Domain.Models;

namespace CarRental.Data.DAOs.Clients
{
    public interface IClientsDao
    {
        Task<Client> GetClientByIdAsync(int id);
        Task<IEnumerable<Client>> GetAllClientsAsync(bool active);
        Task<Client> CreateClientAsync(Client client);
        Task<bool> DeleteByIdAsync(Client client);
        Task<bool> MailInUse(Client client);
    }
}
