using AutoMapper;
using CarRental.Data.DAOs.Clients;
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
        private readonly IMapper _mapper;
        public ClientsService(IClientsDao clientDao, IMapper mapper)
        {
            _clientDao = clientDao;
            _mapper = mapper;
        }

        public Task<Client> CreateClientAsync(Client client)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteByIdAsync(int id)
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
    }
}
