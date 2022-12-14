using CarRental.Domain.Exceptions;
using CarRental.Domain.Models;
using CarRental.WebAPI.DTOs.Vehicle;
using CarRental.WebAPI.Tests.Fakes;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;
using Xunit;

namespace CarRental.WebAPI.Tests.Controllers
{
    public class VehiclesControllerTests
    {
        private VehiclesControllerFakes _fakes;
        public VehiclesControllerTests()
        {
            _fakes = new VehiclesControllerFakes();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task GetVehicles_NoVehiclesInDB_ReturnsEmptyList(bool active)
        {
            int expectedCount = 0;
            List<Vehicle> fakeVehiclesResult = new();
            _fakes.VehiclesService.Setup(f => f.GetAllVehiclesAsync(active))
                .ReturnsAsync(fakeVehiclesResult);

            var vehicle = await _fakes.VehiclesController.GetVehicles(active);

            Assert.IsType<OkObjectResult>(vehicle.Result);
            var result = vehicle.Result as OkObjectResult;
            Assert.Equal(((int)HttpStatusCode.OK), result.StatusCode);
            Assert.NotNull(fakeVehiclesResult);
            Assert.Equal(expectedCount, fakeVehiclesResult.Count);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task GetVehicles_WithDataInDB_ReturnListCorrectly(bool active)
        {
            var fakeVehiclesResult = _fakes.GetListOfVehiclesFake()
                .Where(c => c.Active == active)
                .ToList();
            var vehicleExpected = fakeVehiclesResult.First();
            int expectedCount = fakeVehiclesResult.Count();
            _fakes.VehiclesService.Setup(f => f.GetAllVehiclesAsync(active))
                .ReturnsAsync(fakeVehiclesResult);

            var vehicles = await _fakes.VehiclesController.GetVehicles(active);

            Assert.IsType<OkObjectResult>(vehicles.Result);
            var result = vehicles.Result as OkObjectResult;
            var vehiclesReturned = Utils.GetObjectResultContent(vehicles);
            Assert.Equal(((int)HttpStatusCode.OK), result.StatusCode);
            Assert.IsType<GetVehicleResponse>(vehiclesReturned.First());
            Assert.Equal(vehicleExpected.Id, vehiclesReturned.First().Id);
            Assert.Equal(vehicleExpected.Model, vehiclesReturned.First().Model);
            Assert.Equal(vehicleExpected.PricePerDay, vehiclesReturned.First().PricePerDay);
            Assert.Equal(vehicleExpected.Active, vehiclesReturned.First().Active);
            Assert.Equal(expectedCount, vehiclesReturned.Count());
        }


        [Fact]
        public async Task GetVehicleById_VehicleNoExist_ReturnException()
        {
            int id = 10;
            string exCodeExpected = "VEHICLE_NOT_FOUND";
            string exMsgExpected = $"Vehicle with id: {id} not found";
            var exExpected = new EntityNotFoundException(exMsgExpected, exCodeExpected);
            _fakes.VehiclesService.Setup(f => f.GetVehicleByIdAsync(id))
                .ThrowsAsync(exExpected);

            Func<Task> action = async () =>
            {
                await _fakes.VehiclesController.GetVehicleById(id);
            };
            var ex = await Assert.ThrowsAsync<EntityNotFoundException>(action);

            Assert.IsType<EntityNotFoundException>(ex);
            Assert.Equal(exMsgExpected, ex.Message);
            Assert.Equal(exCodeExpected, ex.Code);
        }

        [Fact]
        public async Task GetVehicleById_VehicleExits_ReturnVehicle()
        {
            var vehicleExpected = _fakes.GetListOfVehiclesFake().First();
            _fakes.VehiclesService.Setup(f => f.GetVehicleByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(vehicleExpected);

            var vehicle = await _fakes.VehiclesController.GetVehicleById(1);

            Assert.IsType<OkObjectResult>(vehicle.Result);
            var result = vehicle.Result as OkObjectResult;
            Assert.Equal(((int)HttpStatusCode.OK), result.StatusCode);
            var vehicleReturned = Utils.GetObjectResultContent(vehicle);
            Assert.IsType<GetVehicleResponse>(vehicleReturned);
            Assert.Equal(vehicleExpected.Id, vehicleReturned.Id);
            Assert.Equal(vehicleExpected.Model, vehicleReturned.Model);
            Assert.Equal(vehicleExpected.PricePerDay, vehicleReturned.PricePerDay);
            Assert.Equal(vehicleExpected.Active, vehicleReturned.Active);
        }

        [Fact]
        public async Task CreateVehicle_CorrectRequest_ReturnCreatedVehicle()
        {
            var vehicleRequestFake = _fakes.GetVehicleRequestFake();
            var vehicleExpected = _fakes.GetVehicleExpectedFake();

            _fakes.VehiclesService.Setup(f => f.CreateVehicleAsync(It.IsAny<Vehicle>()))
                .ReturnsAsync(vehicleExpected);

            var vehicleCreate = await _fakes.VehiclesController
                .CreateVehicle(vehicleRequestFake);

            Assert.IsType<CreatedAtRouteResult>(vehicleCreate.Result);
            var vehicleReturned = Utils.GetObjectResultContent(vehicleCreate);
            Assert.IsType<GetVehicleResponse>(vehicleReturned);
            Assert.Equal(vehicleExpected.Id, vehicleReturned.Id);
            Assert.Equal(vehicleExpected.Model, vehicleReturned.Model);
            Assert.Equal(vehicleExpected.PricePerDay, vehicleReturned.PricePerDay);
            Assert.True(vehicleReturned.Active);
        }

        [Fact]
        public async Task CreateVehicle_ModelInUse_ThrowException()
        {
            var vehicleRequestFake = _fakes.GetVehicleRequestFake();
            string exMsgExpected = $"The model: {vehicleRequestFake.Model} is in Use";
            string exCodeExpected = "MODEL_UNIQUE_ERROR";
            var exExpected = new ModelVehicleInUseException(exMsgExpected);
            _fakes.VehiclesService.Setup(f => f.CreateVehicleAsync(It.IsAny<Vehicle>()))
                .ThrowsAsync(exExpected);

            Func<Task> action = async () =>
            {
                await _fakes.VehiclesController.CreateVehicle(vehicleRequestFake);
            };
            var ex = await Assert.ThrowsAsync<ModelVehicleInUseException>(action);

            Assert.IsType<ModelVehicleInUseException>(ex);
            Assert.Equal(exMsgExpected, ex.Message);
            Assert.Equal(exCodeExpected, ex.Code);
        }

        [Fact]
        public async Task DeleteVehicle_VehicleNoExist_ThrowException()
        {
            int id = 10;
            string exCodeExpected = "VEHICLE_NOT_FOUND";
            string exMsgExpected = $"Vehicle with id: {id} not found";
            var exExpected = new EntityNotFoundException(exMsgExpected, exCodeExpected);

            _fakes.VehiclesService.Setup(f => f.DeleteByIdAsync(id))
                .ThrowsAsync(exExpected);

            Func<Task> action = async () =>
            {
                await _fakes.VehiclesController.DeleteVehicle(id);
            };
            var ex = await Assert.ThrowsAsync<EntityNotFoundException>(action);

            Assert.IsType<EntityNotFoundException>(ex);
            Assert.Equal(exMsgExpected, ex.Message);
            Assert.Equal(exCodeExpected, ex.Code);
        }

        [Fact]
        public async Task DeleteVehicle_VehicleExist_RrturnDeleted()
        {
            int id = 10;
            _fakes.VehiclesService.Setup(f => f.DeleteByIdAsync(id))
                .ReturnsAsync(true);

            var result = await _fakes.VehiclesController.DeleteVehicle(id);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteVehicle_ErrorSaving_ThrowExcpetion()
        {
            int id = 10;
            string exMsgExpected = $"An error occur while deleting vehicle with id {id}";
            _fakes.VehiclesService.Setup(f => f.DeleteByIdAsync(id))
                .ReturnsAsync(false);

            Func<Task> action = async () =>
            {
                await _fakes.VehiclesController.DeleteVehicle(id);
            };
            var ex = await Assert.ThrowsAsync<Exception>(action);

            Assert.Equal(exMsgExpected, ex.Message);
        }
    }
}
