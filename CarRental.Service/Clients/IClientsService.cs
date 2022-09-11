using CarRental.Domain.Models;

namespace CarRental.Service.Clients
{
    public interface IClientsService
    {
        Task<IEnumerable<Client>> GetAllClientsAsync(bool active);
        Task<Client> GetClientByIdAsync(int id);
        Task<Client> CreateClientAsync(Client client);
        Task<bool> DeleteByIdAsync(int id);
    }
}
