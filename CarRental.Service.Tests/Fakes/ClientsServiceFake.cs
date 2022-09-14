using CarRental.Data.DAOs.Clients;
using CarRental.Domain.Models;
using CarRental.Service.Clients;
using Microsoft.Extensions.Logging;
using Moq;

namespace CarRental.Service.Tests.Fakes
{
    internal class ClientsServiceFake
    {
        public Mock<IClientsDao> ClientDaoMock { get; set; }
        public ClientsService ClientService { get; set; }
        private Mock<ILogger<ClientsService>> _logger;

        public ClientsServiceFake()
        {
            ClientDaoMock = new Mock<IClientsDao>();
            _logger = new Mock<ILogger<ClientsService>>();

            ClientService = new ClientsService(ClientDaoMock.Object, _logger.Object);
        }

        public List<Client> Result_Dao_GetAll_WithData()
        {
            return new List<Client>()
            {
                new Client()
                {
                    Id = 1,
                    Fullname = "leandro",
                    Email = "leandro@leandro.com",
                    Active = true
                },
                new Client()
                {
                    Id = 1,
                    Fullname = "pablo",
                    Email = "pablo@pablo.com",
                    Active = true
                },
                new Client()
                {
                    Id = 1,
                    Fullname = "adriana",
                    Email = "adriana@adriana.com",
                    Active = false
                }
            };
        }
        public List<Client> Result_Dao_GetAll_WithoutData()
            => new List<Client>();
        public Client Result_Dao_CreateClient()
        {
            return new Client()
            {
                Email = "lean@lean.com",
                Fullname = "leitan"
            };
        }

        

    }
}
