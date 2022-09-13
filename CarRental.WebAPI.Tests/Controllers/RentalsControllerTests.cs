using CarRental.Domain.Exceptions;
using CarRental.Domain.Models;
using CarRental.Service.Rentals;
using CarRental.WebAPI.Controllers;
using CarRental.WebAPI.DTOs.Rental;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;
using Xunit;

namespace CarRental.WebAPI.Tests.Controllers
{
    public class RentalsControllerTests
    {
        private readonly RentalsController _controller;
        private Mock<IRentalsService> _rentalsService;
        public RentalsControllerTests()
        {
            _rentalsService = new Mock<IRentalsService>();
            _controller = new RentalsController(_rentalsService.Object);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task GetRentals_NoRentalsInDB_ReturnsEmptyList(bool active)
        {
            int expectedCount = 0;
            List<Rental> fakeRentalsResult = new();
            _rentalsService.Setup(f => f.GetAllRentalsAsync(active))
                .ReturnsAsync(fakeRentalsResult);

            var rental = await _controller.GetRentals(active);

            Assert.IsType<OkObjectResult>(rental.Result);
            var result = rental.Result as OkObjectResult;
            Assert.Equal(((int)HttpStatusCode.OK), result.StatusCode);
            Assert.NotNull(fakeRentalsResult);
            Assert.Equal(expectedCount, fakeRentalsResult.Count);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task GetRentals_WithDataInDB_ReturnListCorrectly(bool active)
        {
            var fakeRentalsResult = GetListOfRentalsFake()
                .Where(c => c.Active == active)
                .ToList();
            int expectedCount = fakeRentalsResult.Count();
            _rentalsService.Setup(f => f.GetAllRentalsAsync(active))
                .ReturnsAsync(fakeRentalsResult);

            var rentals = await _controller.GetRentals(active);

            Assert.IsType<OkObjectResult>(rentals.Result);
            var result = rentals.Result as OkObjectResult;
            var rentalsReturned = GetObjectResultContent(rentals);
            Assert.Equal(((int)HttpStatusCode.OK), result.StatusCode);
            Assert.IsType<GetRentalResponse>(rentalsReturned.First());
            Assert.Equal(fakeRentalsResult.First().Id, rentalsReturned.First().Id);
            Assert.Equal(fakeRentalsResult.First().Vehicle.Id, rentalsReturned.First().Vehicle.Id);
            Assert.Equal(fakeRentalsResult.First().Vehicle.Model, rentalsReturned.First().Vehicle.Model);
            Assert.Equal(fakeRentalsResult.First().Client.Id, rentalsReturned.First().Client.Id);
            Assert.Equal(fakeRentalsResult.First().Client.Email, rentalsReturned.First().Client.Email);
            Assert.Equal(fakeRentalsResult.First().Price, rentalsReturned.First().Price);
            Assert.Equal(fakeRentalsResult.First().Active, rentalsReturned.First().Active);
            Assert.Equal(expectedCount, rentalsReturned.Count());
        }


        [Fact]
        public async Task GetRentalById_RentalNoExist_ReturnException()
        {
            int id = 10;
            string exMsgExpected = $"Rental with id: {id} not found";
            string exCodeExpected = "RENTAL_NOT_FOUND";
            var exExpected = new EntityNotFoundException(exMsgExpected, exCodeExpected);
            _rentalsService.Setup(r => r.GetRentalByIdAsync(id)).ThrowsAsync(exExpected);

            Func<Task> action = async () => await _controller.GetRentalById(id);
            var ex = await Assert.ThrowsAsync<EntityNotFoundException>(action);

            Assert.IsType<EntityNotFoundException>(ex);
            Assert.Contains(exMsgExpected, ex.Message);
            Assert.Contains(exCodeExpected, ex.Code);
        }

        [Fact]
        public async Task GetRentalById_RentalExits_ReturnRental()
        {
            var rentalExpected = GetListOfRentalsFake().First();
            _rentalsService.Setup(f => f.GetRentalByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(rentalExpected);

            var rental = await _controller.GetRentalById(1);

            Assert.IsType<OkObjectResult>(rental.Result);
            var result = rental.Result as OkObjectResult;
            Assert.Equal(((int)HttpStatusCode.OK), result.StatusCode);
            var rentalReturned = GetObjectResultContent(rental);
            Assert.IsType<GetRentalResponse>(rentalReturned);
            Assert.Equal(rentalExpected.Id, rentalReturned.Id);
            Assert.Equal(rentalExpected.Price, rentalReturned.Price);
            Assert.Equal(rentalExpected.Active, rentalReturned.Active);
            Assert.Equal(rentalExpected.Client.Id, rentalReturned.Client.Id);
            Assert.Equal(rentalExpected.Client.Email, rentalReturned.Client.Email);
            Assert.Equal(rentalExpected.Vehicle.Id, rentalReturned.Vehicle.Id);
            Assert.Equal(rentalExpected.Vehicle.Model, rentalReturned.Vehicle.Model);
        }

        [Fact]
        public async Task CreateRental_CorrectRequest_ReturnCreatedRental()
        {
            var rentalRequestFake = GetRentalRequestFake();
            var rentalExpected = GetRentalExpectedFake();

            _rentalsService.Setup(f => f.CreateRentalAsync(It.IsAny<Rental>()))
                .ReturnsAsync(rentalExpected);

            var rentalCreate = await _controller.CreateRental(rentalRequestFake);

            Assert.IsType<CreatedAtRouteResult>(rentalCreate.Result);
            var rentalReturned = GetObjectResultContent(rentalCreate);
            Assert.IsType<GetRentalResponse>(rentalReturned);
            Assert.Equal(rentalExpected.Id, rentalReturned.Id);
            Assert.Equal(rentalExpected.Price, rentalReturned.Price);
            Assert.Equal(rentalExpected.Client.Id, rentalReturned.Client.Id);
            Assert.Equal(rentalExpected.Client.Email, rentalReturned.Client.Email);
            Assert.Equal(rentalExpected.Vehicle.Id, rentalReturned.Vehicle.Id);
            Assert.Equal(rentalExpected.Vehicle.Model, rentalReturned.Vehicle.Model);
            Assert.True(rentalReturned.Active);
        }

        [Fact]
        public async Task CreateRental_VehicleInactive_ThrowException()
        {
            var rentalRequestFake = GetRentalRequestFake();
            string exMsgExpected = $"Vehicle with id: {rentalRequestFake.VehicleId} is inactive";
            string exCodeExpected = "VEHICLE_INACTIVE_EXCEPTION";
            var exExpected = new VehicleInactiveException(exMsgExpected);

            _rentalsService.Setup(f => f.CreateRentalAsync(It.IsAny<Rental>()))
                .ThrowsAsync(exExpected);

            Func<Task> action = async () => await _controller.CreateRental(rentalRequestFake);
            var ex = await Assert.ThrowsAsync<VehicleInactiveException>(action);

            Assert.IsType<VehicleInactiveException>(ex);
            Assert.Contains(exMsgExpected, ex.Message);
            Assert.Contains(exCodeExpected, ex.Code);
        }

        [Fact]
        public async Task CreateRental_ClientInactive_ThrowException()
        {
            var rentalRequestFake = GetRentalRequestFake();
            string exMsgExpected = $"Client with id: {rentalRequestFake.ClientId} is inactive";
            string exCodeExpected = "CLIENT_INACTIVE_EXCEPTION";
            var exExpected = new ClientInactiveException(exMsgExpected);

            _rentalsService.Setup(f => f.CreateRentalAsync(It.IsAny<Rental>()))
                .ThrowsAsync(exExpected);

            Func<Task> action = async () => await _controller.CreateRental(rentalRequestFake);
            var ex = await Assert.ThrowsAsync<ClientInactiveException>(action);

            Assert.IsType<ClientInactiveException>(ex);
            Assert.Contains(exMsgExpected, ex.Message);
            Assert.Contains(exCodeExpected, ex.Code);
        }

        [Fact]
        public async Task CreateRental_VehicleUnavailable_ThrowException()
        {
            var rentalRequestFake = GetRentalRequestFake();
            string exMsgExpected = $"Vehicle with id: {rentalRequestFake.VehicleId} is unavailable";
            string exCodeExpected = "VEHICLE_UNAVAILABLE_EXCEPTION";
            var exExpected = new VehicleUnavailableException(exMsgExpected);

            _rentalsService.Setup(f => f.CreateRentalAsync(It.IsAny<Rental>()))
                .ThrowsAsync(exExpected);

            Func<Task> action = async () => await _controller.CreateRental(rentalRequestFake);
            var ex = await Assert.ThrowsAsync<VehicleUnavailableException>(action);

            Assert.IsType<VehicleUnavailableException>(ex);
            Assert.Contains(exMsgExpected, ex.Message);
            Assert.Contains(exCodeExpected, ex.Code);
        }

        [Fact]
        public async Task DeleteRental_RentalNoExist_ThrowException()
        {
            int id = 10;
            string exMsgExpected = $"Rental with id: {id} not found";
            string exCodeExpected = "RENTAL_NOT_FOUND";
            var exExpected = new EntityNotFoundException(exMsgExpected, exCodeExpected);

            _rentalsService.Setup(f => f.DeleteByIdAsync(id))
                .ThrowsAsync(exExpected);

            Func<Task> action = async () => await _controller.DeleteRental(id);
            var ex = await Assert.ThrowsAsync<EntityNotFoundException>(action);

            Assert.IsType<EntityNotFoundException>(ex);
            Assert.Contains(exMsgExpected, ex.Message);
            Assert.Contains(exCodeExpected, ex.Code);
        }

        [Fact]
        public async Task DeleteRental_RentalExist_RrturnDeleted()
        {
            int id = 10;
            _rentalsService.Setup(f => f.DeleteByIdAsync(id))
                .ReturnsAsync(true);

            var result = await _controller.DeleteRental(id);

            Assert.IsType<NoContentResult>(result);
        }

        private static T GetObjectResultContent<T>(ActionResult<T> result)
        {
            return (T)((ObjectResult)result.Result).Value;
        }

        private List<Rental> GetListOfRentalsFake()
        {
            List<Rental> listResult = new();
            listResult.Add(new Rental() 
            { 
                Id = 1, 
                Active = true, 
                Vehicle = new Vehicle() { Id = 1 },
                Client = new Client() { Id = 1 },
                DateFrom = new DateTime(2022, 01, 05),
                DateTo = new DateTime(2022, 01, 10),
                Price = 100
            });
            listResult.Add(new Rental()
            {
                Id = 2,
                Active = false,
                Vehicle = new Vehicle() { Id = 2 },
                Client = new Client() { Id = 2 },
                DateFrom = new DateTime(2022, 02, 05),
                DateTo = new DateTime(2022, 02, 10),
                Price = 200
            });
            listResult.Add(new Rental()
            {
                Id = 3,
                Active = true,
                Vehicle = new Vehicle() { Id = 3 },
                Client = new Client() { Id = 3 },
                DateFrom = new DateTime(2022, 03, 05),
                DateTo = new DateTime(2022, 03, 10),
                Price = 300
            });
            
            return listResult;
        }

        private CreateRentalRequest GetRentalRequestFake()
        {
            return new CreateRentalRequest()
            {
                VehicleId =1,
                ClientId = 1,
                DateFrom = new DateTime(2022, 03, 05),
                DateTo = new DateTime(2022, 03, 10)
            };
        }

        private Rental GetRentalExpectedFake()
        {
            return new Rental()
            {
                Id = 4,
                Active = true,
                Vehicle = new Vehicle() { Id = 4 },
                Client = new Client() { Id = 4 },
                DateFrom = new DateTime(2022, 03, 05),
                DateTo = new DateTime(2022, 03, 10),
                Price = 400
            };
        }
    }
}
