using CarRental.Domain.Exceptions;
using CarRental.Domain.Models;
using CarRental.Service.Tests.Fakes;
using Moq;
using Xunit;

namespace CarRental.Service.Tests.Clients
{
    public class ClientsServiceTests
    {
        private readonly ClientsServiceFake _fakes;

        public ClientsServiceTests()
        {
            _fakes = new ClientsServiceFake();
        }

        [Theory]
        [InlineData(true)]
        public async Task GetAllClientsAsync_NoData_EmptyResults(bool active)
        {
            int expectedCount = 0;
            List<Client> listClient = _fakes.Result_Dao_GetAll_WithoutData();
            _fakes.ClientDao.Setup(c => c.GetAllClientsAsync(active))
                .ReturnsAsync(listClient);

            var result = await _fakes.ClientService.GetAllClientsAsync(active);

            Assert.NotNull(result);
            Assert.Equal(expectedCount, result.Count());
        }

        [Fact]
        public async Task GetAllClientsAsync_ActiveWithData_ReturnRemovedClients()
        {
            bool active = true;
            int expectedCount = 2;
            List<Client> listClients = _fakes.Result_Dao_GetAll_WithData()
                .Where(c => c.Active).ToList();
            _fakes.ClientDao.Setup(c => c.GetAllClientsAsync(active))
                .ReturnsAsync(listClients);

            var result = await _fakes.ClientService.GetAllClientsAsync(active);

            Assert.NotNull(result);
            Assert.Equal(expectedCount, result.Count());
        }

        [Fact]
        public async Task GetAllClientsAsync_RemovedWithData_ReturnRemovedClients()
        {
            bool active = false;
            int expectedCount = 1;
            List<Client> listClient = _fakes.Result_Dao_GetAll_WithData()
                .Where(c => !c.Active).ToList();
            _fakes.ClientDao.Setup(c => c.GetAllClientsAsync(active))
                .ReturnsAsync(listClient);

            var result = await _fakes.ClientService.GetAllClientsAsync(active);

            Assert.NotNull(result);
            Assert.Equal(expectedCount, result.Count());
        }

        [Fact]
        public async Task GetClientByIdAsync_ExistClient_ReturnClient()
        {
            int id = 1;
            Client client = _fakes.Result_Dao_GetAll_WithData().First();
            _fakes.ClientDao.Setup(c => c.GetClientByIdAsync(id))
                .ReturnsAsync(client);

            var result = await _fakes.ClientService.GetClientByIdAsync(id);

            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal(client.Fullname, result.Fullname);
        }

        [Fact]
        public async Task GetClientByIdAsync_NonExistClient_EntityNotFoundException()
        {
            int id = 10;
            Client client = null;
            string exceptionMessage = $"Client with id: {id} not found";
            _fakes.ClientDao.Setup(c => c.GetClientByIdAsync(id))
                .ReturnsAsync(client);

            Func<Task> action = async () => await _fakes.ClientService.GetClientByIdAsync(id);
            var ex = await Assert.ThrowsAsync<EntityNotFoundException>(action);

            Assert.Contains(exceptionMessage, ex.Message);
        }

        [Fact]
        public async Task CreateClientAsync_ClientComplete_ReturnClientCreated()
        {
            Client newClientFake = _fakes.Result_Dao_CreateClient();
            _fakes.ClientDao.Setup(c => c.CreateClientAsync(It.IsAny<Domain.Models.Client>()))
                .ReturnsAsync(newClientFake);

            var result = await _fakes.ClientService.CreateClientAsync(newClientFake);

            Assert.NotNull(result);
            Assert.Equal(newClientFake.Fullname, result.Fullname);
            Assert.Equal(newClientFake.Email, result.Email);
        }

        [Fact]
        public async Task CreateClientAsync_MailInUse_ThrowException()
        {
            Client newClientFake = _fakes.Result_Dao_CreateClient();
            string exceptionMessage = $"The email: {newClientFake.Email} is in Use";
            _fakes.ClientDao.Setup(c => c.MailInUse(It.IsAny<Domain.Models.Client>()))
                .ReturnsAsync(true);

            Func<Task> action = async () => await _fakes.ClientService.CreateClientAsync(newClientFake);
            var ex = await Assert.ThrowsAsync<EmailinUseException>(action);

            Assert.Contains(exceptionMessage, ex.Message);
        }

        [Fact]
        public async Task DeleteByIdAsync_ExistClient_ReturnTrue()
        {
            Client client = _fakes.Result_Dao_GetAll_WithData().First();
            _fakes.ClientDao.Setup(c => c.GetClientByIdAsync(client.Id))
                .ReturnsAsync(client);
            _fakes.ClientDao.Setup(c => c.DeleteByIdAsync(client))
                .ReturnsAsync(true);

            var result = await _fakes.ClientService.DeleteByIdAsync(client.Id);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteByIdAsync_ClientAlredyDelete_ReturnsTrue()
        {
            Client client = _fakes.Result_Dao_GetAll_WithData()
                .Where(c => c.Active == false).First();

            _fakes.ClientDao.Setup(c => c.GetClientByIdAsync(client.Id))
                .ReturnsAsync(client);

            var result = await _fakes.ClientService.DeleteByIdAsync(client.Id);

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

            Func<Task> action = async () => await _fakes.ClientService.DeleteByIdAsync(id);
            var ex = await Assert.ThrowsAsync<EntityNotFoundException>(action);
            
            Assert.Contains(exceptionMessage, ex.Message);
        }
    }
}
