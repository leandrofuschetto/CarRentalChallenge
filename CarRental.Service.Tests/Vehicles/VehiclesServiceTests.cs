using CarRental.Domain.Exceptions;
using CarRental.Domain.Models;
using CarRental.Service.Tests.Fakes;
using CarRental.Service.Vehicles;
using Moq;
using Xunit;

namespace CarRental.Service.Tests.Vehicles
{
    public class VehiclesServiceTests
    {
        private readonly VehiclesServiceFake _fakes;

        public VehiclesServiceTests()
        {
            _fakes = new VehiclesServiceFake();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task GetAllVehiclesAsync_NoData_EmptyResults(bool active)
        {
            int expectedCount = 0;
            List<Vehicle> listVehicle = _fakes.DaoGetAllWithoutData();
            _fakes.VehicleDaoMock.Setup(c => c.GetAllVehiclesAsync(active))
                .ReturnsAsync(listVehicle);

            var result = await _fakes.VehiclesService.GetAllVehiclesAsync(active);

            Assert.NotNull(result);
            Assert.Equal(expectedCount, result.Count());
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task GetAllVehiclesAsync_WithData_ReturnVehicles(bool active)
        {
            List<Vehicle> listVehicles = _fakes.DaoGetAllWithData()
                .Where(v => v.Active).ToList();
            int expectedCount = listVehicles.Count();
            _fakes.VehicleDaoMock.Setup(c => c.GetAllVehiclesAsync(active))
                .ReturnsAsync(listVehicles);

            var result = await _fakes.VehiclesService.GetAllVehiclesAsync(active);

            Assert.NotNull(result);
            Assert.Equal(expectedCount, result.Count());
        }

        [Fact]
        public async Task GetAllVehiclesAsync_DaoFails_ReturnDaoException()
        {
            string exMsgException = "message";
            var innerExExpected = new ArgumentNullException();
            List<Vehicle> listVehicles = _fakes.DaoGetAllWithData()
                .Where(v => v.Active).ToList();
            int expectedCount = listVehicles.Count();
            _fakes.VehicleDaoMock.Setup(c => c.GetAllVehiclesAsync(true))
                .ThrowsAsync(new DataBaseContextException(exMsgException, innerExExpected));
            
            Func<Task> action = async () =>
            {
                await _fakes.VehiclesService.GetAllVehiclesAsync(true);
            };
            var ex = await Assert.ThrowsAsync<DataBaseContextException>(action);

            Assert.NotNull(ex);
            Assert.Equal(exMsgException, ex.Message);
            Assert.IsType<ArgumentNullException>(ex.InnerException);
            Assert.IsType<DataBaseContextException>(ex);
        }

        [Fact]
        public async Task GetAllVehiclesAsync_RemovedWithData_ReturnRemovedVehicles()
        {
            bool active = false;
            int expectedCount = 1;
            List<Vehicle> listVehicle = _fakes.DaoGetAllWithData()
                .Where(v => !v.Active).ToList();
            _fakes.VehicleDaoMock.Setup(c => c.GetAllVehiclesAsync(active))
                .ReturnsAsync(listVehicle);

            var result = await _fakes.VehiclesService.GetAllVehiclesAsync(active);

            Assert.NotNull(result);
            Assert.Equal(expectedCount, result.Count());
        }

        [Fact]
        public async Task GetVehicleByIdAsync_ExistVehicle_ReturnVehicle()
        {
            int id = 1;
            Vehicle vehicle = _fakes.DaoGetAllWithData().First();
            _fakes.VehicleDaoMock.Setup(c => c.GetVehicleByIdAsync(id))
                .ReturnsAsync(vehicle);

            var result = await _fakes.VehiclesService.GetVehicleByIdAsync(id);

            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal(vehicle.Model, result.Model);
        }

        [Fact]
        public async Task GetVehicleByIdAsync_NonExistVehicle_EntityNotFoundException()
        {
            int id = 10;
            string exMsgExpected = $"Vehicle with id: {id} not found";
            string exCodeExpected = $"VEHICLE_NOT_FOUND";
            Vehicle vehicle = null;
            _fakes.VehicleDaoMock.Setup(c => c.GetVehicleByIdAsync(id))
                .ReturnsAsync(vehicle);

            Func<Task> action = async () =>
            {
                await _fakes.VehiclesService.GetVehicleByIdAsync(id);
            };
            var ex = await Assert.ThrowsAsync<EntityNotFoundException>(action);

            Assert.IsType<EntityNotFoundException>(ex);
            Assert.Equal(exMsgExpected, ex.Message);
            Assert.Equal(exCodeExpected, ex.Code);
        }

        [Fact]
        public async Task CreateVehicleAsync_RequestOk_ReturnVehicleCreated()
        {
            Vehicle newVehicleFake = _fakes.DaoCreateVehicleResult();
            _fakes.VehicleDaoMock.Setup(c => c.CreateVehicleAsync(It.IsAny<Vehicle>()))
                .ReturnsAsync(newVehicleFake);

            var result = await _fakes.VehiclesService.CreateVehicleAsync(newVehicleFake);

            Assert.NotNull(result);
            Assert.Equal(newVehicleFake.Model, result.Model);
            Assert.Equal(newVehicleFake.PricePerDay, result.PricePerDay);
        }

        [Fact]
        public async Task CreateVehicleAsync_ModelInUse_ThrowException()
        {
            Vehicle newVehicleFake = _fakes.DaoCreateVehicleResult();
            string exMsgExpected = $"The model: {newVehicleFake.Model} is in Use";
            string exCodeExcepted = "MODEL_UNIQUE_ERROR";
            _fakes.VehicleDaoMock.Setup(c => c.ModelExits(It.IsAny<Vehicle>()))
                .ReturnsAsync(true);

            Func<Task> action = async () =>
            {
                await _fakes.VehiclesService.CreateVehicleAsync(newVehicleFake);
            };
            var ex = await Assert.ThrowsAsync<ModelVehicleInUseException>(action);

            Assert.IsType<ModelVehicleInUseException>(ex);
            Assert.Equal(exMsgExpected, ex.Message);
            Assert.Equal(exCodeExcepted, ex.Code);
        }

        [Fact]
        public async Task DeleteByIdAsync_ExistVehicle_ReturnTrue()
        {
            Vehicle vehicle = _fakes.DaoGetAllWithData().First();
            _fakes.VehicleDaoMock.Setup(c => c.GetVehicleByIdAsync(vehicle.Id))
                .ReturnsAsync(vehicle);
            _fakes.VehicleDaoMock.Setup(c => c.DeleteByIdAsync(vehicle))
                .ReturnsAsync(true);

            var result = await _fakes.VehiclesService.DeleteByIdAsync(vehicle.Id);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteByIdAsync_VechileInactive_ReturnTrue()
        {
            Vehicle vehicle = _fakes.DaoGetAllWithData()
                .Where(v => v.Active == false).First();
            _fakes.VehicleDaoMock.Setup(c => c.GetVehicleByIdAsync(vehicle.Id))
                .ReturnsAsync(vehicle);
            
            var result = await _fakes.VehiclesService.DeleteByIdAsync(vehicle.Id);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteByIdAsync_NonExistVehicle_EntityNotFoundException()
        {
            int id = 10;
            Vehicle vehicle = null;
            string exMsgExpected = $"Vehicle with id: {id} not found";
            string exCodeExpected = "VEHICLE_NOT_FOUND";
            _fakes.VehicleDaoMock.Setup(c => c.GetVehicleByIdAsync(id))
                .ReturnsAsync(vehicle);

            Func<Task> action = async () =>
            {
                await _fakes.VehiclesService.DeleteByIdAsync(id);
            };
            var ex = await Assert.ThrowsAsync<EntityNotFoundException>(action);

            Assert.IsType<EntityNotFoundException>(ex);
            Assert.Equal(exMsgExpected, ex.Message);
            Assert.Equal(exCodeExpected, ex.Code);
        }
    }
}
