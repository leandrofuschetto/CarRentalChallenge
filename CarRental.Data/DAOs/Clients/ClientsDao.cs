using AutoMapper;
using CarRental.Data.Entities;
using CarRental.Domain.Exceptions;
using CarRental.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace CarRental.Data.DAOs.Clients
{
    public class ClientsDao : IClientsDao
    {
        private readonly CarRentalContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<ClientsDao> _logger;
        private readonly string CLASS_NAME = typeof(ClientsDao).Name;

        public ClientsDao(
            CarRentalContext context, 
            IMapper mapper, 
            ILogger<ClientsDao> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<Client>> GetAllClientsAsync(bool active)
        {
            try
            {
                var listClients = await _context.Clients
                    .Where(c => c.Active == active)
                    .ToListAsync();

                return _mapper.Map<IEnumerable<Client>>(listClients);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    "An error ocurrs when getting clients. At {0}, {1}",
                    CLASS_NAME,
                    "GetAllClientsAsync");

                throw new DataBaseContextException(ex.Message);
            }
        }

        public async Task<Client> GetClientByIdAsync(int id)
        {
            try
            {
                var clientEntity = await _context.Clients
                    .FirstOrDefaultAsync(c => c.Id == id);

                return _mapper.Map<Client>(clientEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    "{0} - An error ocurrs when getting client with Id:{1}. At {2}, {3}",
                    DateTime.Now,
                    id, 
                    CLASS_NAME,
                    "GetClientByIdAsync");

                throw new DataBaseContextException(ex.Message, ex.InnerException);
            }
        }

        public async Task<Client> CreateClientAsync(Domain.Models.Client client)
        {
            try
            {
                var clientEntity = _mapper.Map<ClientEntity>(client);
                clientEntity.Active = true;

                await _context.Clients.AddAsync(clientEntity);
                await _context.SaveChangesAsync();

                return _mapper.Map<Client>(clientEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error {0} creating client: {1}. At {2}, {3}",
                    DateTime.Now,
                    JsonSerializer.Serialize(client),
                    CLASS_NAME,
                    "CreateClientAsync");

                throw new DataBaseContextException(ex.Message, ex.InnerException);
            }
        }

        public async Task<bool> DeleteByIdAsync(Client client)
        {
            try
            {
                var clientEntity = await _context.Clients
                    .FirstOrDefaultAsync(c => c.Id == client.Id);

                _context.Clients.Attach(clientEntity);
                clientEntity.Active = false;

                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error deleting client: {1}. At {2}, {3}",
                    client.Id,
                    CLASS_NAME,
                    "CreateClientAsync");

                throw new DataBaseContextException(ex.Message, ex.InnerException);
            }
        }

        public async Task<bool> MailInUse(Client client)
        {
            try
            {
                var mailalreadyUsed = await _context.Clients.AnyAsync(
                    c => c.Email.ToUpper() == client.Email.ToUpper() 
                    && c.Active == true);
                
                return mailalreadyUsed;
            }
            catch
            {
                throw new DataBaseContextException();
            }
        }
    }
}
