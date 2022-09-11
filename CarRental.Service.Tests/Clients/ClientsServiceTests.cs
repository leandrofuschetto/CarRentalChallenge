using AutoMapper;
using CarRental.Domain.Exceptions;
using CarRental.Service.Clients;
using CarRental.Service.Tests.Fakes;
using CarRental.Tests.Helpers;
using Moq;
using Xunit;

namespace CarRental.Service.Tests.Clients
{
    public class ClientsServiceTests
    {
        private readonly ClientsServiceFake _fakes;
        private Utils _utils;
        private IMapper _mapper;

        public ClientsServiceTests()
        {
            _fakes = new ClientsServiceFake();
            _utils = new Utils();
            _mapper = _utils.GetMapper();
        }

        [Theory]
        [InlineData(true)]
        public async Task GetAllClientsAsync_NoData_EmptyResults(bool active)
        {
            int expectedCount = 0;
            var listClient = _fakes.Result_Dao_GetAll_WithoutData();

            _fakes.ClientDao.Setup(c => c.GetAllClientsAsync(active))
                .ReturnsAsync(listClient);

            var clientService = new ClientsService(_fakes.ClientDao.Object, _mapper);
            var result = await clientService.GetAllClientsAsync(active);

            Assert.NotNull(result);
            Assert.Equal(expectedCount, result.Count());
        }

        [Fact]
        public async Task GetAllClientsAsync_ActiveWithData_ReturnRemovedClients()
        {
            bool active = true;
            int expectedCount = 2;
            var listClients = _fakes.Result_Dao_GetAll_WithData()
                .Where(c => c.Active).ToList();

            _fakes.ClientDao.Setup(c => c.GetAllClientsAsync(active))
                .ReturnsAsync(listClients);

            var clientService = new ClientsService(_fakes.ClientDao.Object, _mapper);

            var result = await clientService.GetAllClientsAsync(active);

            Assert.NotNull(result);
            Assert.Equal(expectedCount, result.Count());
        }

        [Fact]
        public async Task GetAllClientsAsync_RemovedWithData_ReturnRemovedClients()
        {
            bool active = false;
            int expectedCount = 1;
            var listClient = _fakes.Result_Dao_GetAll_WithData()
                .Where(c => !c.Active).ToList();

            _fakes.ClientDao.Setup(c => c.GetAllClientsAsync(active))
                .ReturnsAsync(listClient);

            var clientService = new ClientsService(_fakes.ClientDao.Object, _mapper);

            var result = await clientService.GetAllClientsAsync(active);

            Assert.NotNull(result);
            Assert.Equal(expectedCount, result.Count());
        }

        [Fact]
        public async Task GetClientByIdAsync_ExistClient_ReturnClient()
        {
            int id = 1;
            var client = _fakes.Result_Dao_GetAll_WithData().First();

            _fakes.ClientDao.Setup(c => c.GetClientByIdAsync(id))
                .ReturnsAsync(client);

            var clientService = new ClientsService(_fakes.ClientDao.Object, _mapper);

            var result = await clientService.GetClientByIdAsync(id);

            Assert.NotNull(result);
            Assert.Equal(id, result.ClientId);
            Assert.Equal(client.Fullname, result.Fullname);
        }

        [Fact]
        public async Task GetClientByIdAsync_NonExistClient_EntityNotFoundException()
        {
            int id = 10;
            string exceptionMessage = $"Client with id: {id} not found";
            Domain.Models.Client client = null;

            _fakes.ClientDao.Setup(c => c.GetClientByIdAsync(id))
                .ReturnsAsync(client);

            var clientService = new ClientsService(_fakes.ClientDao.Object, _mapper);

            Func<Task> action = async () => await clientService.GetClientByIdAsync(id);

            var ex = await Assert.ThrowsAsync<EntityNotFoundException>(action);
            Assert.Contains(exceptionMessage, ex.Message);
        }

        [Fact]
        public async Task CreateClientAsync_ClientComplete_ReturnClientCreated()
        {
            Domain.Models.Client newClientFake = new()
            {
                Email = "lean@lean.com",
                Fullname = "leitan"
            };

            _fakes.ClientDao.Setup(c => c.CreateClientAsync(It.IsAny<Domain.Models.Client>()))
                .ReturnsAsync(newClientFake);

            //fakes.ClientDao.Setup(c => c.CreateClientAsync(newClientFake))
            //    .ReturnsAsync(newClientFake);

            var clientService = new ClientsService(_fakes.ClientDao.Object, _mapper);
            var result = await clientService.CreateClientAsync(newClientFake);

            Assert.NotNull(result);
            Assert.Equal(newClientFake.Fullname, result.Fullname);
            Assert.Equal(newClientFake.Email, result.Email);
        }

        [Fact]
        public async Task CreateClientAsync_MailInUse_ThrowException()
        {
            Domain.Models.Client newClientFake = new()
            {
                Email = "lean@lean.com",
                Fullname = "leitan"
            };
            string exceptionMessage = $"The email: {newClientFake.Email} is in Use";

            _fakes.ClientDao.Setup(c => c.MailInUse(It.IsAny<Domain.Models.Client>()))
                .ReturnsAsync(true);

            var clientService = new ClientsService(_fakes.ClientDao.Object, _mapper);

            Func<Task> action = async () => await clientService.CreateClientAsync(newClientFake);
            var ex = await Assert.ThrowsAsync<EmailinUseException>(action);
            Assert.Contains(exceptionMessage, ex.Message);
        }

        [Fact]
        public async Task DeleteByIdAsync_ExistClient_ReturnTrue()
        {
            int id = 1;
            var client = _fakes.Result_Dao_GetAll_WithData().First();

            _fakes.ClientDao.Setup(c => c.GetClientByIdAsync(id))
                .ReturnsAsync(client);
            _fakes.ClientDao.Setup(c => c.DeleteByIdAsync(client))
                .ReturnsAsync(true);

            var clientService = new ClientsService(_fakes.ClientDao.Object, _mapper);

            var result = await clientService.DeleteByIdAsync(id);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteByIdAsync_NonExistClient_EntityNotFoundException()
        {
            int id = 10;
            string exceptionMessage = $"Client with id: {id} not found";
            Domain.Models.Client client = null;

            _fakes.ClientDao.Setup(c => c.GetClientByIdAsync(id))
                .ReturnsAsync(client);

            var clientService = new ClientsService(_fakes.ClientDao.Object, _mapper);

            Func<Task> action = async () => await clientService.DeleteByIdAsync(id);

            var ex = await Assert.ThrowsAsync<EntityNotFoundException>(action);
            Assert.Contains(exceptionMessage, ex.Message);
        }
    }
}
