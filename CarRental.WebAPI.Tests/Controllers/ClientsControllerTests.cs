using CarRental.Domain.Exceptions;
using CarRental.Domain.Models;
using CarRental.Service.Clients;
using CarRental.WebAPI.Controllers;
using CarRental.WebAPI.DTOs.Client;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;
using Xunit;

namespace CarRental.WebAPI.Tests.Controllers
{
    public class ClientsControllerTests
    {
        private readonly ClientsController _controller;
        private Mock<IClientsService> _clientsService;
        public ClientsControllerTests()
        {
            _clientsService = new Mock<IClientsService>();
            _controller = new ClientsController(_clientsService.Object);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task GetClients_NoClientsInDB_ReturnsEmptyList(bool active)
        {
            int expectedCount = 0;
            List<Client> fakeClientsResult = new();
            _clientsService.Setup(f => f.GetAllClientsAsync(active))
                .ReturnsAsync(fakeClientsResult);

            var client = await _controller.GetClients(active);

            Assert.IsType<OkObjectResult>(client.Result);
            var result = client.Result as OkObjectResult;
            Assert.Equal(((int)HttpStatusCode.OK), result.StatusCode);
            Assert.NotNull(fakeClientsResult);
            Assert.Equal(expectedCount, fakeClientsResult.Count); 
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task GetClients_WithDataInDB_ReturnListCorrectly(bool active)
        {
            var fakeClientsResult = GetListOfClientsFake()
                .Where(c => c.Active == active)
                .ToList();
            int expectedCount = fakeClientsResult.Count();
            _clientsService.Setup(f => f.GetAllClientsAsync(active))
                .ReturnsAsync(fakeClientsResult);

            var clients = await _controller.GetClients(active);
            
            Assert.IsType<OkObjectResult>(clients.Result);
            var result = clients.Result as OkObjectResult;
            var clientsReturned = GetObjectResultContent(clients);
            Assert.Equal(((int)HttpStatusCode.OK), result.StatusCode);
            Assert.IsType<GetClientResponse>(clientsReturned.First());
            Assert.Equal(fakeClientsResult.First().Id, clientsReturned.First().Id);
            Assert.Equal(fakeClientsResult.First().Fullname, clientsReturned.First().Fullname);
            Assert.Equal(fakeClientsResult.First().Email, clientsReturned.First().Email);
            Assert.Equal(expectedCount, clientsReturned.Count());
        }


        [Fact]
        public async Task GetClientById_ClientNoExist_ReturnException()
        {
            int id = 10;
            string exMsgExpected = $"Client with id: {id} not found";
            string exCodeExpected = "CLIENT_NOT_FOUND";
            var exExpected = new EntityNotFoundException(exMsgExpected, exCodeExpected);
            _clientsService.Setup(f => f.GetClientByIdAsync(id)).ThrowsAsync(exExpected);

            Func<Task> action = async () => await _controller.GetClientById(id);
            var ex = await Assert.ThrowsAsync<EntityNotFoundException>(action);

            Assert.IsType<EntityNotFoundException>(ex);
            Assert.Contains(exMsgExpected, ex.Message);
            Assert.Contains(exCodeExpected, ex.Code);
        }

        [Fact]
        public async Task GetClientById_ClientExits_ReturnClient()
        {
            var clientExpected = GetListOfClientsFake().First();
            _clientsService.Setup(f => f.GetClientByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(clientExpected);

            var client = await _controller.GetClientById(1);

            Assert.IsType<OkObjectResult>(client.Result);
            var result = client.Result as OkObjectResult;
            Assert.Equal(((int)HttpStatusCode.OK), result.StatusCode);
            var clientReturned = GetObjectResultContent(client);
            Assert.IsType<GetClientResponse>(clientReturned);
            Assert.Equal(clientExpected.Id, clientReturned.Id);
            Assert.Equal(clientExpected.Fullname, clientReturned.Fullname);
            Assert.Equal(clientExpected.Email, clientReturned.Email);
        }

        [Fact]
        public async Task CreateClient_CorrectRequest_ReturnCreatedClient()
        {
            var clientRequestFake = GetClientRequestFake();
            var clientExpected = GetClientExpectedFake();

            _clientsService.Setup(f => f.CreateClientAsync(It.IsAny<Client>()))
                .ReturnsAsync(clientExpected);

            var clientCreate = await _controller.CreateClient(clientRequestFake);

            Assert.IsType<CreatedAtRouteResult>(clientCreate.Result);
            var clientReturned = GetObjectResultContent(clientCreate);
            Assert.IsType<GetClientResponse>(clientReturned);
            Assert.Equal(clientExpected.Id, clientReturned.Id);
            Assert.Equal(clientExpected.Fullname, clientReturned.Fullname);
            Assert.Equal(clientExpected.Email, clientReturned.Email);
            Assert.True(clientReturned.Active);
        }

        [Fact]
        public async Task CreateClient_MailInUse_ThrowException()
        {
            var clientRequestFake = GetClientRequestFake();
            string exMsgExpected = $"The email: {clientRequestFake.Email} is in Use";
            string exCodeExpected = "EMAIL_UNIQUE_ERROR";
            var exExpected = new EmailinUseException(exMsgExpected);

            _clientsService.Setup(f => f.CreateClientAsync(It.IsAny<Client>()))
                .ThrowsAsync(exExpected);

            Func<Task> action = async () => await _controller.CreateClient(clientRequestFake);
            var ex = await Assert.ThrowsAsync<EmailinUseException>(action);

            Assert.IsType<EmailinUseException>(ex);
            Assert.Contains(exMsgExpected, ex.Message);
            Assert.Contains(exCodeExpected, ex.Code);
        }

        [Fact]
        public async Task DeleteClient_ClientNoExist_ThrowException()
        {
            int id = 10;
            string exMsgExpected = $"Client with id: {id} not found";
            string exCodeExpected = "CLIENT_NOT_FOUND";
            var exExpected = new EntityNotFoundException(exMsgExpected, exCodeExpected);
            
            _clientsService.Setup(f => f.DeleteByIdAsync(id))
                .ThrowsAsync(exExpected);

            Func<Task> action = async () => await _controller.DeleteClient(id);
            var ex = await Assert.ThrowsAsync<EntityNotFoundException>(action);

            Assert.IsType<EntityNotFoundException>(ex);
            Assert.Contains(exMsgExpected, ex.Message);
            Assert.Contains(exCodeExpected, ex.Code);
        }

        [Fact]
        public async Task DeleteClient_ClientExist_RrturnDeleted()
        {
            int id = 10;
            _clientsService.Setup(f => f.DeleteByIdAsync(id))
                .ReturnsAsync(true);

            var result = await _controller.DeleteClient(id);

            Assert.IsType<NoContentResult>(result);
        }

        private static T GetObjectResultContent<T>(ActionResult<T> result)
        {
            return (T)((ObjectResult)result.Result).Value;
        }

        private List<Client> GetListOfClientsFake()
        {
            List<Client> listResult = new();
            listResult.Add(new Client() { Id = 1, Active = true, Email = "lean@test.com", Fullname = "lean" });
            listResult.Add(new Client() { Id = 2, Active = true, Email = "isaac@test.com", Fullname = "Isa" });
            listResult.Add(new Client() { Id = 3, Active = false, Email = "asdf@test.com", Fullname = "asdf" });

            return listResult;
        }

        private CreateClientRequest GetClientRequestFake()
        {
            return new CreateClientRequest()
            {
                Email = "Lean@lean.com",
                Fullname = "lean"
            };
        }

        private Client GetClientExpectedFake()
        {
            return new Client()
            {
                Id = 1,
                Email = "Lean@lean.com",
                Fullname = "lean",
                Active = true
            };
        }
    }
}
