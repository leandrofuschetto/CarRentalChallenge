using CarRental.Data.DAOs.Clients;
using CarRental.Domain.Models;
using CarRental.Service.Clients;
using Moq;

namespace CarRental.Service.Tests.Clients
{
    internal class ClientsServiceFake
    {
        public Mock<IClientsDao> ClientDao { get; set; }
        public ClientsService ClientService { get; set; }

        public ClientsServiceFake()
        {
            ClientDao = new Mock<IClientsDao>();
            ClientService = new ClientsService(ClientDao.Object);
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

    }
}
