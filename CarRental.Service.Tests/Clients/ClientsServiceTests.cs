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
        [InlineData(false)]
        public async Task GetAllClientsAsync_NoData_EmptyResults(bool active)
        {
            int expectedCount = 0;
            List<Client> listClient = _fakes.DaoGetAllWithoutDdata();
            _fakes.ClientsMockDao.Setup(c => c.GetAllClientsAsync(active))
                .ReturnsAsync(listClient);

            var result = await _fakes.ClientService.GetAllClientsAsync(active);

            Assert.NotNull(result);
            Assert.Equal(expectedCount, result.Count());
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task GetAllClientsAsync_WithData_ReturnClients(bool active)
        {
            List<Client> listClients = _fakes.DaoGetAllWithDdata()
                .Where(c => c.Active).ToList();
            int expectedCount = listClients.Count();
            _fakes.ClientsMockDao.Setup(c => c.GetAllClientsAsync(active))
                .ReturnsAsync(listClients);

            var result = await _fakes.ClientService.GetAllClientsAsync(active);

            Assert.NotNull(result);
            Assert.Equal(expectedCount, result.Count());
        }

        [Fact]
        public async Task GetAllClientsAsync_DaoFails_ReturnDaoException()
        {
            string exMsgException = "message";
            List<Client> listClients = _fakes.DaoGetAllWithDdata()
                .Where(v => v.Active).ToList();
            int expectedCount = listClients.Count();
            _fakes.ClientsMockDao.Setup(c => c.GetAllClientsAsync(true))
                .ThrowsAsync(new DataBaseContextException(exMsgException));

            Func<Task> action = async () =>
            {
                await _fakes.ClientService.GetAllClientsAsync(true);
            };
            var ex = await Assert.ThrowsAsync<DataBaseContextException>(action);

            Assert.NotNull(ex);
            Assert.Equal(exMsgException, ex.Message);
            Assert.IsType<DataBaseContextException>(ex);
        }

        [Fact]
        public async Task GetClientByIdAsync_ExistClient_ReturnClient()
        {
            int id = 1;
            Client client = _fakes.DaoGetAllWithDdata().First();
            _fakes.ClientsMockDao.Setup(c => c.GetClientByIdAsync(id))
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
            string exCodeExpected = "CLIENT_NOT_FOUND";
            string exMsgException = $"Client with id: {id} not found";
            _fakes.ClientsMockDao.Setup(c => c.GetClientByIdAsync(id)).ReturnsAsync(client);

            Func<Task> action = async () =>
            {
                await _fakes.ClientService.GetClientByIdAsync(id);
            };
            var ex = await Assert.ThrowsAsync<EntityNotFoundException>(action);

            Assert.IsType<EntityNotFoundException>(ex);
            Assert.Equal(exMsgException, ex.Message);
            Assert.Equal(exCodeExpected, ex.Code);
        }

        [Fact]
        public async Task CreateClientAsync_RequestOk_ReturnClientCreated()
        {
            Client newClientFake = _fakes.DaoCreateClientResult();
            _fakes.ClientsMockDao.Setup(c => c.CreateClientAsync(It.IsAny<Client>()))
                .ReturnsAsync(newClientFake);

            var result = await _fakes.ClientService.CreateClientAsync(newClientFake);

            Assert.NotNull(result);
            Assert.Equal(newClientFake.Fullname, result.Fullname);
            Assert.Equal(newClientFake.Email, result.Email);
        }

        [Fact]
        public async Task CreateClientAsync_MailInUse_ThrowException()
        {
            Client newClientFake = _fakes.DaoCreateClientResult();
            string exCodeExpected = "EMAIL_UNIQUE_ERROR";
            string exMsgExpected = $"The email: {newClientFake.Email} is in Use";
            _fakes.ClientsMockDao.Setup(c => c.MailInUse(It.IsAny<Client>()))
                .ReturnsAsync(true);

            Func<Task> action = async () =>
            {
                await _fakes.ClientService.CreateClientAsync(newClientFake);
            };
            var ex = await Assert.ThrowsAsync<EmailinUseException>(action);

            Assert.IsType<EmailinUseException>(ex);
            Assert.Equal(exMsgExpected, ex.Message);
            Assert.Equal(exCodeExpected, ex.Code);
        }

        [Fact]
        public async Task DeleteByIdAsync_ExistClient_ReturnTrue()
        {
            Client client = _fakes.DaoGetAllWithDdata().First();
            _fakes.ClientsMockDao.Setup(c => c.GetClientByIdAsync(client.Id))
                .ReturnsAsync(client);
            _fakes.ClientsMockDao.Setup(c => c.DeleteByIdAsync(client))
                .ReturnsAsync(true);

            var result = await _fakes.ClientService.DeleteByIdAsync(client.Id);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteByIdAsync_ClientalreadyDelete_ReturnsTrue()
        {
            Client client = _fakes.DaoGetAllWithDdata()
                .Where(c => c.Active == false)
                .First();
            _fakes.ClientsMockDao.Setup(c => c.GetClientByIdAsync(client.Id))
                .ReturnsAsync(client);

            var result = await _fakes.ClientService.DeleteByIdAsync(client.Id);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteByIdAsync_NonExistClient_EntityNotFoundException()
        {
            int id = 10;
            Domain.Models.Client client = null;
            string exCodeExpected = "CLIENT_NOT_FOUND";
            string exMessageExpeted = $"Client with id: {id} not found";
            _fakes.ClientsMockDao.Setup(c => c.GetClientByIdAsync(id))
                .ReturnsAsync(client);

            Func<Task> action = async () =>
            {
                await _fakes.ClientService.DeleteByIdAsync(id);
            };
            var ex = await Assert.ThrowsAsync<EntityNotFoundException>(action);

            Assert.IsType<EntityNotFoundException>(ex);
            Assert.Equal(exMessageExpeted, ex.Message);
            Assert.Equal(exCodeExpected, ex.Code);
        }
    }
}
