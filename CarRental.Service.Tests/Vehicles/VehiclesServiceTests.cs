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
        public async Task GetAllVehiclesAsync_NoData_EmptyResults(bool active)
        {
            int expectedCount = 0;
            List<Vehicle> listVehicle = _fakes.Result_Dao_GetAll_WithoutData();
            _fakes.VehicleDao.Setup(c => c.GetAllVehiclesAsync(active))
                .ReturnsAsync(listVehicle);

            var result = await _fakes.VehiclesService.GetAllVehiclesAsync(active);

            Assert.NotNull(result);
            Assert.Equal(expectedCount, result.Count());
        }

        [Fact]
        public async Task GetAllVehiclesAsync_ActiveWithData_ReturnActiveVehicles()
        {
            bool active = true;
            int expectedCount = 2;
            List<Vehicle> listActiveVehicle = _fakes.Result_Dao_GetAll_WithData()
                .Where(v => v.Active).ToList();
            _fakes.VehicleDao.Setup(c => c.GetAllVehiclesAsync(active))
                .ReturnsAsync(listActiveVehicle);

            var result = await _fakes.VehiclesService.GetAllVehiclesAsync(active);

            Assert.NotNull(result);
            Assert.Equal(expectedCount, result.Count());
        }

        [Fact]
        public async Task GetAllVehiclesAsync_RemovedWithData_ReturnRemovedVehicles()
        {
            bool active = false;
            int expectedCount = 1;
            List<Vehicle> listVehicle = _fakes.Result_Dao_GetAll_WithData()
                .Where(v => !v.Active).ToList();
            _fakes.VehicleDao.Setup(c => c.GetAllVehiclesAsync(active))
                .ReturnsAsync(listVehicle);

            var result = await _fakes.VehiclesService.GetAllVehiclesAsync(active);

            Assert.NotNull(result);
            Assert.Equal(expectedCount, result.Count());
        }

        [Fact]
        public async Task GetVehicleByIdAsync_ExistVehicle_ReturnVehicle()
        {
            int id = 1;
            Vehicle vehicle = _fakes.Result_Dao_GetAll_WithData().First();
            _fakes.VehicleDao.Setup(c => c.GetVehicleByIdAsync(id))
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
            string exceptionMessage = $"Vehicle with id: {id} not found";
            Vehicle vehicle = null;
            _fakes.VehicleDao.Setup(c => c.GetVehicleByIdAsync(id))
                .ReturnsAsync(vehicle);

            Func<Task> action = async () => await _fakes.VehiclesService.GetVehicleByIdAsync(id);

            var ex = await Assert.ThrowsAsync<EntityNotFoundException>(action);
            Assert.Contains(exceptionMessage, ex.Message);
        }

        [Fact]
        public async Task CreateVehicleAsync_VehicleComplete_ReturnVehicleCreated()
        {
            Vehicle newVehicleFake = _fakes.Result_Dao_CreateVehicle();
            _fakes.VehicleDao.Setup(c => c.CreateVehicleAsync(It.IsAny<Vehicle>()))
                .ReturnsAsync(newVehicleFake);

            var result = await _fakes.VehiclesService.CreateVehicleAsync(newVehicleFake);

            Assert.NotNull(result);
            Assert.Equal(newVehicleFake.Model, result.Model);
            Assert.Equal(newVehicleFake.PricePerDay, result.PricePerDay);
        }

        [Fact]
        public async Task CreateVehicleAsync_ModelInUse_ThrowException()
        {
            Vehicle newVehicleFake = _fakes.Result_Dao_CreateVehicle();
            string exceptionMessage = $"The model: {newVehicleFake.Model} is in Use";

            _fakes.VehicleDao.Setup(c => c.ModelExits(It.IsAny<Vehicle>()))
                .ReturnsAsync(true);

            Func<Task> action = async () => await _fakes.VehiclesService.CreateVehicleAsync(newVehicleFake);
            var ex = await Assert.ThrowsAsync<ModelVehicleInUseException>(action);
            Assert.Contains(exceptionMessage, ex.Message);
        }

        [Fact]
        public async Task DeleteByIdAsync_ExistVehicle_ReturnTrue()
        {
            Vehicle vehicle = _fakes.Result_Dao_GetAll_WithData().First();

            _fakes.VehicleDao.Setup(c => c.GetVehicleByIdAsync(vehicle.Id))
                .ReturnsAsync(vehicle);
            _fakes.VehicleDao.Setup(c => c.DeleteByIdAsync(vehicle))
                .ReturnsAsync(true);

            var result = await _fakes.VehiclesService.DeleteByIdAsync(vehicle.Id);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteByIdAsync_VechileInactive_ReturnTrue()
        {
            Vehicle vehicle = _fakes.Result_Dao_GetAll_WithData()
                .Where(v => v.Active == false).First();

            _fakes.VehicleDao.Setup(c => c.GetVehicleByIdAsync(vehicle.Id))
                .ReturnsAsync(vehicle);
            
            var result = await _fakes.VehiclesService.DeleteByIdAsync(vehicle.Id);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteByIdAsync_NonExistVehicle_EntityNotFoundException()
        {
            int id = 10;
            string exceptionMessage = $"Vehicle with id: {id} not found";
            Vehicle vehicle = null;

            _fakes.VehicleDao.Setup(c => c.GetVehicleByIdAsync(id))
                .ReturnsAsync(vehicle);

            Func<Task> action = async () => await _fakes.VehiclesService.DeleteByIdAsync(id);

            var ex = await Assert.ThrowsAsync<EntityNotFoundException>(action);
            Assert.Contains(exceptionMessage, ex.Message);
        }
    }
}
