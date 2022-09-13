using CarRental.Domain.Models;
using CarRental.Service.Clients;
using CarRental.WebAPI.Controllers;
using CarRental.WebAPI.DTOs.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace CarRental.WebAPI.Tests.Fakes
{
    internal class ClientsControllerFakes
    {
        public ClientsController Controller { get; set; }
        public Mock<IClientsService> ClientsService { get; set; }

        public ClientsControllerFakes()
        {
            ClientsService = new Mock<IClientsService>();
            Controller = new ClientsController(ClientsService.Object,
                new Mock<ILogger<ClientsController>>().Object);
        }

        public List<Client> GetListOfClientsFake()
        {
            List<Client> listResult = new();
            listResult.Add(new Client() { Id = 1, Active = true, Email = "lean@test.com", Fullname = "lean" });
            listResult.Add(new Client() { Id = 2, Active = true, Email = "isaac@test.com", Fullname = "Isa" });
            listResult.Add(new Client() { Id = 3, Active = false, Email = "asdf@test.com", Fullname = "asdf" });

            return listResult;
        }

        public CreateClientRequest GetClientRequestFake()
        {
            return new CreateClientRequest()
            {
                Email = "Lean@lean.com",
                Fullname = "lean"
            };
        }

        public Client GetClientExpectedFake()
        {
            return new Client()
            {
                Id = 1,
                Email = "Lean@lean.com",
                Fullname = "lean",
                Active = true
            };
        }

        public T GetObjectResultContent<T>(ActionResult<T> result)
        {
            return (T)((ObjectResult)result.Result).Value;
        }
    }
}
