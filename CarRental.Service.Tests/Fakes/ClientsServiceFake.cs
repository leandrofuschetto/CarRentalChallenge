using CarRental.Data.DAOs.Clients;
using CarRental.Domain.Models;
using CarRental.Service.Clients;
using Microsoft.Extensions.Logging;
using Moq;

namespace CarRental.Service.Tests.Fakes
{
    internal class ClientsServiceFake
    {
        public Mock<IClientsDao> ClientDao { get; set; }
        public ClientsService ClientService { get; set; }

        public ClientsServiceFake()
        {
            ClientDao = new Mock<IClientsDao>();
            ClientService = new ClientsService(ClientDao.Object,
                new Mock<ILogger<ClientsService>>().Object);
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

        public Client Result_Dao_CreateClient()
        {
            return new Client()
            {
                Email = "lean@lean.com",
                Fullname = "leitan"
            };
        }

        public List<Client> Result_Dao_GetAll_WithoutData()
            => new List<Client>();

    }
}
