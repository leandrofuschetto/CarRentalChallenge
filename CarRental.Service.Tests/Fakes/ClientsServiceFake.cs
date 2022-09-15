using CarRental.Data.DAOs.Clients;
using CarRental.Domain.Models;
using CarRental.Service.Clients;
using Microsoft.Extensions.Logging;
using Moq;

namespace CarRental.Service.Tests.Fakes
{
    internal class ClientsServiceFake
    {
        public Mock<IClientsDao> ClientsMockDao { get; set; }
        public ClientsService ClientService { get; set; }
        private Mock<ILogger<ClientsService>> loggerMock;

        public ClientsServiceFake()
        {
            loggerMock = new Mock<ILogger<ClientsService>>();
            ClientsMockDao = new Mock<IClientsDao>();

            ClientService = new ClientsService(
                ClientsMockDao.Object,
                loggerMock.Object);
        }

        public List<Client> DaoGetAllWithDdata()
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

        public List<Client> DaoGetAllWithoutDdata()
            => new List<Client>();

        public Client DaoCreateClientResult()
        {
            return new Client()
            {
                Email = "lean@lean.com",
                Fullname = "leitan"
            };
        }
    }
}
