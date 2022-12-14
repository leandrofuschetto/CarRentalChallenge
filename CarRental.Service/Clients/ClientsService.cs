using CarRental.Data.DAOs.Clients;
using CarRental.Domain.Exceptions;
using CarRental.Domain.Models;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace CarRental.Service.Clients
{
    public class ClientsService : IClientsService
    {
        
        private readonly IClientsDao _clientDao;
        private readonly ILogger<ClientsService> _logger;
        private readonly MethodBase _methodBase;
        private readonly string CLASS_NAME = typeof(ClientsService).Name;

        public ClientsService(IClientsDao clientDao, ILogger<ClientsService> logger)
        {
            _clientDao = clientDao;
            _logger = logger;
            _methodBase = MethodBase.GetCurrentMethod()!;
        }

        public async Task<IEnumerable<Client>> GetAllClientsAsync(bool active)
        { 
            var clients = await _clientDao.GetAllClientsAsync(active);

            return clients;
        }

        public async Task<Client> GetClientByIdAsync(int id)
        {
            var client = await FindClientByIdAsync(id);

            return client;
        }

        public async Task<Client> CreateClientAsync(Client client)
        {
            bool mailalreadyUsed = await _clientDao.MailInUse(client);
            if (mailalreadyUsed)
            {
                _logger.LogError(
                    "Mail already use. At {0}, {1}", 
                    CLASS_NAME,
                    Utils.GetActualAsyncMethodName());

                throw new EmailinUseException($"The email: {client.Email} is in Use");
            }
                

            var clientCreated = await _clientDao.CreateClientAsync(client);

            return clientCreated;
        }

        public async Task<bool> DeleteByIdAsync(int id)
        {
            var client = await FindClientByIdAsync(id);

            if (!client.Active)
            {
                _logger.LogInformation(
                    "User already deleted.Returns true. At {0}, {1}",
                    CLASS_NAME,
                    Utils.GetActualAsyncMethodName());

                return true;
            }
                
            var deletedOk = await _clientDao.DeleteByIdAsync(client);

            return deletedOk;
        }

        private async Task<Client> FindClientByIdAsync(int id)
        {
            var client = await _clientDao.GetClientByIdAsync(id);

            if (client == null)
            {
                _logger.LogError("Client Not Found. At {0}, {1}",
                    CLASS_NAME,
                    Utils.GetActualAsyncMethodName());

                throw new EntityNotFoundException(
                    $"Client with id: {id} not found",
                    "CLIENT_NOT_FOUND");
            }

            return client;
        }
    }
}
