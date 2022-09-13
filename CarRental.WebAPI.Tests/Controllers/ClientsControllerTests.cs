using CarRental.Domain.Exceptions;
using CarRental.Domain.Models;
using CarRental.WebAPI.DTOs.Client;
using CarRental.WebAPI.Tests.Fakes;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;
using Xunit;

namespace CarRental.WebAPI.Tests.Controllers
{
    public class ClientsControllerTests
    {
        private ClientsControllerFakes _fakes;
        
        public ClientsControllerTests()
        {
            _fakes = new ClientsControllerFakes();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task GetClients_NoClientsInDB_ReturnsEmptyList(bool active)
        {
            int expectedCount = 0;
            List<Client> fakeClientsResult = new();
            _fakes.ClientsService.Setup(f => f.GetAllClientsAsync(active))
                .ReturnsAsync(fakeClientsResult);

            var client = await _fakes.Controller.GetClients(active);

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
            List<Client> fakeClientsResult = _fakes.GetListOfClientsFake()
                .Where(c => c.Active == active)
                .ToList();
            int expectedCount = fakeClientsResult.Count();
            _fakes.ClientsService.Setup(f => f.GetAllClientsAsync(active))
                .ReturnsAsync(fakeClientsResult);

            var clients = await _fakes.Controller.GetClients(active);
            
            Assert.IsType<OkObjectResult>(clients.Result);
            var result = clients.Result as OkObjectResult;
            var clientsReturned = _fakes.GetObjectResultContent(clients);
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
            _fakes.ClientsService.Setup(f => f.GetClientByIdAsync(id)).ThrowsAsync(exExpected);

            Func<Task> action = async () => await _fakes.Controller.GetClientById(id);
            var ex = await Assert.ThrowsAsync<EntityNotFoundException>(action);

            Assert.IsType<EntityNotFoundException>(ex);
            Assert.Contains(exMsgExpected, ex.Message);
            Assert.Contains(exCodeExpected, ex.Code);
        }

        [Fact]
        public async Task GetClientById_ClientExits_ReturnClient()
        {
            Client clientExpected = _fakes.GetListOfClientsFake().First();
            _fakes.ClientsService.Setup(f => f.GetClientByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(clientExpected);

            var client = await _fakes.Controller.GetClientById(1);

            Assert.IsType<OkObjectResult>(client.Result);
            var result = client.Result as OkObjectResult;
            Assert.Equal(((int)HttpStatusCode.OK), result.StatusCode);
            var clientReturned = _fakes.GetObjectResultContent(client);
            Assert.IsType<GetClientResponse>(clientReturned);
            Assert.Equal(clientExpected.Id, clientReturned.Id);
            Assert.Equal(clientExpected.Fullname, clientReturned.Fullname);
            Assert.Equal(clientExpected.Email, clientReturned.Email);
        }

        [Fact]
        public async Task CreateClient_CorrectRequest_ReturnCreatedClient()
        {
            CreateClientRequest clientRequestFake = _fakes.GetClientRequestFake();
            Client clientExpected = _fakes.GetClientExpectedFake();

            _fakes.ClientsService.Setup(f => f.CreateClientAsync(It.IsAny<Client>()))
                .ReturnsAsync(clientExpected);

            var clientCreate = await _fakes.Controller.CreateClient(clientRequestFake);

            Assert.IsType<CreatedAtRouteResult>(clientCreate.Result);
            var clientReturned = _fakes.GetObjectResultContent(clientCreate);
            Assert.IsType<GetClientResponse>(clientReturned);
            Assert.Equal(clientExpected.Id, clientReturned.Id);
            Assert.Equal(clientExpected.Fullname, clientReturned.Fullname);
            Assert.Equal(clientExpected.Email, clientReturned.Email);
            Assert.True(clientReturned.Active);
        }

        [Fact]
        public async Task CreateClient_MailInUse_ThrowException()
        {
            CreateClientRequest clientRequestFake = _fakes.GetClientRequestFake();
            string exMsgExpected = $"The email: {clientRequestFake.Email} is in Use";
            string exCodeExpected = "EMAIL_UNIQUE_ERROR";
            var exExpected = new EmailinUseException(exMsgExpected);

            _fakes.ClientsService.Setup(f => f.CreateClientAsync(It.IsAny<Client>()))
                .ThrowsAsync(exExpected);

            Func<Task> action = async () => await _fakes.Controller.CreateClient(clientRequestFake);
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
            
            _fakes.ClientsService.Setup(f => f.DeleteByIdAsync(id))
                .ThrowsAsync(exExpected);

            Func<Task> action = async () => await _fakes.Controller.DeleteClient(id);
            var ex = await Assert.ThrowsAsync<EntityNotFoundException>(action);

            Assert.IsType<EntityNotFoundException>(ex);
            Assert.Contains(exMsgExpected, ex.Message);
            Assert.Contains(exCodeExpected, ex.Code);
        }

        [Fact]
        public async Task DeleteClient_ClientExist_RrturnDeleted()
        {
            int id = 10;
            _fakes.ClientsService.Setup(f => f.DeleteByIdAsync(id))
                .ReturnsAsync(true);

            var result = await _fakes.Controller.DeleteClient(id);

            Assert.IsType<NoContentResult>(result);
        }
    }
}
