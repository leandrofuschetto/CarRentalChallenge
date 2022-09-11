using CarRental.Domain.Models;

namespace CarRental.Data.DAOs.Clients
{
    public class ClientsDao : IClientsDao
    {
        private readonly CarRentalContext _context;

        public ClientsDao(CarRentalContext context)
        {
            _context = context;
        }

        public Task<Client> CreateClientAsync(Client client)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteByIdAsync(Client client)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Client>> GetAllClientsAsync(bool active)
        {
            throw new NotImplementedException();
        }

        public Task<Client> GetClientByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> MailInUse(Client client)
        {
            throw new NotImplementedException();
        }
    }
}
