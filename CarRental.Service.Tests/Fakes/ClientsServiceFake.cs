using CarRental.Data.DAOs.Clients;
using CarRental.Domain.Models;
using Moq;

namespace CarRental.Service.Tests.Fakes
{
    internal class ClientsServiceFake
    {
        public Mock<IClientsDao> ClientDao { get; set; }

        public ClientsServiceFake()
        {
            ClientDao = new Mock<IClientsDao>();
        }

        public List<Domain.Models.Client> Result_Dao_GetAll_WithData()
        {
            return new List<Domain.Models.Client>()
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
