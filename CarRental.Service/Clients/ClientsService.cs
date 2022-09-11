using AutoMapper;
using CarRental.Data.DAOs.Clients;
using CarRental.Domain.Exceptions;
using CarRental.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRental.Service.Clients
{
    public class ClientsService : IClientsService
    {
        private readonly IClientsDao _clientDao;

        public ClientsService(IClientsDao clientDao)
        {
            _clientDao = clientDao;
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
                throw new EmailinUseException($"The email: {client.Email} is in Use");

            var clientCreated = await _clientDao.CreateClientAsync(client);

            return clientCreated;
        }

        public async Task<bool> DeleteByIdAsync(int id)
        {
            var client = await FindClientByIdAsync(id);
            var deletedOk = await _clientDao.DeleteByIdAsync(client);

            return deletedOk;
        }

        private async Task<Domain.Models.Client> FindClientByIdAsync(int id)
        {
            var client = await _clientDao.GetClientByIdAsync(id);

            if (client == null)
                throw new EntityNotFoundException(
                    $"Client with id: {id} not found",
                    "CLIENT_NOT_FOUND");

            return client;
        }
    }
}
