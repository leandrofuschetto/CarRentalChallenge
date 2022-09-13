using CarRental.Domain.Exceptions;
using CarRental.Domain.Models;
using CarRental.Service.Vehicles;
using CarRental.WebAPI.Controllers;
using CarRental.WebAPI.DTOs.Vehicle;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;
using Xunit;

namespace CarRental.WebAPI.Tests.Controllers
{
    public class VehiclesControllerTests
    {
        private readonly VehiclesController _controller;
        private Mock<IVehiclesService> _vehiclesService;
        public VehiclesControllerTests()
        {
            _vehiclesService = new Mock<IVehiclesService>();
            _controller = new VehiclesController(_vehiclesService.Object);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task GetVehicles_NoVehiclesInDB_ReturnsEmptyList(bool active)
        {
            int expectedCount = 0;
            List<Vehicle> fakeVehiclesResult = new();
            _vehiclesService.Setup(f => f.GetAllVehiclesAsync(active))
                .ReturnsAsync(fakeVehiclesResult);

            var vehicle = await _controller.GetVehicles(active);

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
            var fakeVehiclesResult = GetListOfVehiclesFake()
                .Where(c => c.Active == active)
                .ToList();
            int expectedCount = fakeVehiclesResult.Count();
            _vehiclesService.Setup(f => f.GetAllVehiclesAsync(active))
                .ReturnsAsync(fakeVehiclesResult);

            var vehicles = await _controller.GetVehicles(active);

            Assert.IsType<OkObjectResult>(vehicles.Result);
            var result = vehicles.Result as OkObjectResult;
            var vehiclesReturned = GetObjectResultContent(vehicles);
            Assert.Equal(((int)HttpStatusCode.OK), result.StatusCode);
            Assert.IsType<GetVehicleResponse>(vehiclesReturned.First());
            Assert.Equal(fakeVehiclesResult.First().Id, vehiclesReturned.First().Id);
            Assert.Equal(fakeVehiclesResult.First().Model, vehiclesReturned.First().Model);
            Assert.Equal(fakeVehiclesResult.First().PricePerDay, vehiclesReturned.First().PricePerDay);
            Assert.Equal(fakeVehiclesResult.First().Active, vehiclesReturned.First().Active);
            Assert.Equal(expectedCount, vehiclesReturned.Count());
        }


        [Fact]
        public async Task GetVehicleById_VehicleNoExist_ReturnException()
        {
            int id = 10;
            string exMsgExpected = $"Vehicle with id: {id} not found";
            string exCodeExpected = "VEHICLE_NOT_FOUND";
            var exExpected = new EntityNotFoundException(exMsgExpected, exCodeExpected);
            _vehiclesService.Setup(f => f.GetVehicleByIdAsync(id)).ThrowsAsync(exExpected);

            Func<Task> action = async () => await _controller.GetVehicleById(id);
            var ex = await Assert.ThrowsAsync<EntityNotFoundException>(action);

            Assert.IsType<EntityNotFoundException>(ex);
            Assert.Contains(exMsgExpected, ex.Message);
            Assert.Contains(exCodeExpected, ex.Code);
        }

        [Fact]
        public async Task GetVehicleById_VehicleExits_ReturnVehicle()
        {
            var vehicleExpected = GetListOfVehiclesFake().First();
            _vehiclesService.Setup(f => f.GetVehicleByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(vehicleExpected);

            var vehicle = await _controller.GetVehicleById(1);

            Assert.IsType<OkObjectResult>(vehicle.Result);
            var result = vehicle.Result as OkObjectResult;
            Assert.Equal(((int)HttpStatusCode.OK), result.StatusCode);
            var vehicleReturned = GetObjectResultContent(vehicle);
            Assert.IsType<GetVehicleResponse>(vehicleReturned);
            Assert.Equal(vehicleExpected.Id, vehicleReturned.Id);
            Assert.Equal(vehicleExpected.Model, vehicleReturned.Model);
            Assert.Equal(vehicleExpected.PricePerDay, vehicleReturned.PricePerDay);
            Assert.Equal(vehicleExpected.Active, vehicleReturned.Active);
        }

        [Fact]
        public async Task CreateVehicle_CorrectRequest_ReturnCreatedVehicle()
        {
            var vehicleRequestFake = GetVehicleRequestFake();
            var vehicleExpected = GetVehicleExpectedFake();

            _vehiclesService.Setup(f => f.CreateVehicleAsync(It.IsAny<Vehicle>()))
                .ReturnsAsync(vehicleExpected);

            var vehicleCreate = await _controller.CreateVehicle(vehicleRequestFake);

            Assert.IsType<CreatedAtRouteResult>(vehicleCreate.Result);
            var vehicleReturned = GetObjectResultContent(vehicleCreate);
            Assert.IsType<GetVehicleResponse>(vehicleReturned);
            Assert.Equal(vehicleExpected.Id, vehicleReturned.Id);
            Assert.Equal(vehicleExpected.Model, vehicleReturned.Model);
            Assert.Equal(vehicleExpected.PricePerDay, vehicleReturned.PricePerDay);
            Assert.True(vehicleReturned.Active);
        }

        [Fact]
        public async Task CreateVehicle_ModelInUse_ThrowException()
        {
            var vehicleRequestFake = GetVehicleRequestFake();
            string exMsgExpected = $"The model: {vehicleRequestFake.Model} is in Use";
            string exCodeExpected = "MODEL_UNIQUE_ERROR";
            var exExpected = new ModelVehicleInUseException(exMsgExpected);

            _vehiclesService.Setup(f => f.CreateVehicleAsync(It.IsAny<Vehicle>()))
                .ThrowsAsync(exExpected);

            Func<Task> action = async () => await _controller.CreateVehicle(vehicleRequestFake);
            var ex = await Assert.ThrowsAsync<ModelVehicleInUseException>(action);

            Assert.IsType<ModelVehicleInUseException>(ex);
            Assert.Contains(exMsgExpected, ex.Message);
            Assert.Contains(exCodeExpected, ex.Code);
        }

        [Fact]
        public async Task DeleteVehicle_VehicleNoExist_ThrowException()
        {
            int id = 10;
            string exMsgExpected = $"Vehicle with id: {id} not found";
            string exCodeExpected = "VEHICLE_NOT_FOUND";
            var exExpected = new EntityNotFoundException(exMsgExpected, exCodeExpected);

            _vehiclesService.Setup(f => f.DeleteByIdAsync(id))
                .ThrowsAsync(exExpected);

            Func<Task> action = async () => await _controller.DeleteVehicle(id);
            var ex = await Assert.ThrowsAsync<EntityNotFoundException>(action);

            Assert.IsType<EntityNotFoundException>(ex);
            Assert.Contains(exMsgExpected, ex.Message);
            Assert.Contains(exCodeExpected, ex.Code);
        }

        [Fact]
        public async Task DeleteVehicle_VehicleExist_RrturnDeleted()
        {
            int id = 10;
            _vehiclesService.Setup(f => f.DeleteByIdAsync(id))
                .ReturnsAsync(true);

            var result = await _controller.DeleteVehicle(id);

            Assert.IsType<NoContentResult>(result);
        }

        private static T GetObjectResultContent<T>(ActionResult<T> result)
        {
            return (T)((ObjectResult)result.Result).Value;
        }

        private List<Vehicle> GetListOfVehiclesFake()
        {
            List<Vehicle> listResult = new();
            listResult.Add(new Vehicle() { Id = 1, Active = true, Model = "Model1", PricePerDay = 1 });
            listResult.Add(new Vehicle() { Id = 2, Active = true, Model = "Model2", PricePerDay = 2 });
            listResult.Add(new Vehicle() { Id = 3, Active = false, Model = "Model3", PricePerDay = 3 });

            return listResult;
        }

        private CreateVehicleRequest GetVehicleRequestFake()
        {
            return new CreateVehicleRequest()
            {
                Model = "Model4",
                PricePerDay = 4
            };
        }

        private Vehicle GetVehicleExpectedFake()
        {
            return new Vehicle()
            {
                Id = 4,
                Model = "Model4",
                PricePerDay = 4,
                Active = true
            };
        }
    }
}
