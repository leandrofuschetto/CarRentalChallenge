using AutoMapper;
using CarRental.Data.Entities;
using CarRental.Domain.Exceptions;
using CarRental.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Data.DAOs.Clients
{
    public class ClientsDao : IClientsDao
    {
        private readonly CarRentalContext _context;
        private readonly IMapper _mapper;

        public ClientsDao(CarRentalContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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
            catch
            {
                throw new DataBaseContextException();
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
            catch
            {
                throw new DataBaseContextException();
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
            catch
            {
                throw new DataBaseContextException();
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
            catch (Exception)
            {
                throw new DataBaseContextException();
            }
        }

        public async Task<bool> MailInUse(Client client)
        {
            try
            {
                var mailAlredyUsed = await _context.Clients
                    .Where(c => c.Email == client.Email)
                    .FirstOrDefaultAsync() != null;

                return mailAlredyUsed;
            }
            catch
            {
                throw new DataBaseContextException();
            }
        }
    }
}
