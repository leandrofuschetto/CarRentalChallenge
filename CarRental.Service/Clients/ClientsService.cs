using CarRental.Data.DAOs.Clients;
using CarRental.Domain.Exceptions;
using CarRental.Domain.Models;
using CarRental.Service.Utils;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace CarRental.Service.Clients
{
    public class ClientsService : IClientsService
    {
        static string GetActualAsyncMethodName([CallerMemberName] string name = "") => name;
        private readonly IClientsDao _clientDao;
        private readonly ILogger<ClientsService> _logger;
        private readonly MethodBase _methodBase;

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
            bool mailAlredyUsed = await _clientDao.MailInUse(client);
            if (mailAlredyUsed)
            {
                LogHelper.Log(_logger, "Mail alredy in use",
                    LogHelper.ERROR,
                    _methodBase.ReflectedType?.Name!,
                    GetActualAsyncMethodName());

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
                LogHelper.Log(_logger, "User alredy deleted. Returns true", 
                    LogHelper.INFORMATION,
                    _methodBase.ReflectedType?.Name!,
                    GetActualAsyncMethodName());

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
                LogHelper.Log(_logger, "Client Not Found",
                    LogHelper.ERROR,
                    _methodBase.ReflectedType?.Name!,
                    GetActualAsyncMethodName());

                throw new EntityNotFoundException(
                    $"Client with id: {id} not found",
                    "CLIENT_NOT_FOUND");
            }

            return client;
        }
    }
}
