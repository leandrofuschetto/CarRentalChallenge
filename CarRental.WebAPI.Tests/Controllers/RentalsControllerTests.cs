using CarRental.Domain.Exceptions;
using CarRental.Domain.Models;
using CarRental.WebAPI.DTOs.Rental;
using CarRental.WebAPI.Exceptions;
using CarRental.WebAPI.Tests.Fakes;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;
using Xunit;

namespace CarRental.WebAPI.Tests.Controllers
{
    public class RentalsControllerTests
    {
        private RentalsControllerFakes _fakes;
        public RentalsControllerTests()
        {
            _fakes = new RentalsControllerFakes();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task GetRentals_NoRentalsInDB_ReturnsEmptyList(bool active)
        {
            int expectedCount = 0;
            List<Rental> fakeRentalsResult = new();
            _fakes.RentalsService.Setup(f => f.GetAllRentalsAsync(active))
                .ReturnsAsync(fakeRentalsResult);

            var rental = await _fakes.RentalsController.GetRentals(active);

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
            var fakeRentalsResult = _fakes.GetListOfRentalsFake()
                .Where(c => c.Active == active)
                .ToList();
            int expectedCount = fakeRentalsResult.Count();
            _fakes.RentalsService.Setup(f => f.GetAllRentalsAsync(active))
                .ReturnsAsync(fakeRentalsResult);

            var rentals = await _fakes.RentalsController.GetRentals(active);

            Assert.IsType<OkObjectResult>(rentals.Result);
            var result = rentals.Result as OkObjectResult;
            var rentalsReturned = _fakes.GetObjectResultContent(rentals);
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
            _fakes.RentalsService.Setup(r => r.GetRentalByIdAsync(id)).ThrowsAsync(exExpected);

            Func<Task> action = async () => await _fakes.RentalsController.GetRentalById(id);
            var ex = await Assert.ThrowsAsync<EntityNotFoundException>(action);

            Assert.IsType<EntityNotFoundException>(ex);
            Assert.Contains(exMsgExpected, ex.Message);
            Assert.Contains(exCodeExpected, ex.Code);
        }

        [Fact]
        public async Task GetRentalById_RentalExits_ReturnRental()
        {
            var rentalExpected = _fakes.GetListOfRentalsFake().First();
            _fakes.RentalsService.Setup(f => f.GetRentalByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(rentalExpected);

            var rental = await _fakes.RentalsController.GetRentalById(1);

            Assert.IsType<OkObjectResult>(rental.Result);
            var result = rental.Result as OkObjectResult;
            Assert.Equal(((int)HttpStatusCode.OK), result.StatusCode);
            var rentalReturned = _fakes.GetObjectResultContent(rental);
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
            var rentalRequestFake = _fakes.GetRentalRequestFake();
            var rentalExpected = _fakes.GetRentalExpectedFake();

            _fakes.RentalsService.Setup(f => f.CreateRentalAsync(It.IsAny<Rental>()))
                .ReturnsAsync(rentalExpected);

            var rentalCreate = await _fakes.RentalsController.CreateRental(rentalRequestFake);

            Assert.IsType<CreatedAtRouteResult>(rentalCreate.Result);
            var rentalReturned = _fakes.GetObjectResultContent(rentalCreate);
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
        public async Task CreateRental_InvalidDates_ThrowException()
        {
            var rentalRequestFake = _fakes.GetRentalRequestFake();
            rentalRequestFake.DateFrom = new DateTime(2022, 01, 01);
            rentalRequestFake.DateTo = new DateTime(2021, 01, 01);

            string exMsgExpected = $"DateFrom can'be higher than DateTo";
            string exCodeExpected = "DATEFROM_MAJOR_DATETO";
            var exExpected = new DatesInvalidException(exMsgExpected);

            Func<Task> action = async () => await _fakes.RentalsController.CreateRental(rentalRequestFake);
            var ex = await Assert.ThrowsAsync<DatesInvalidException>(action);

            Assert.IsType<DatesInvalidException>(ex);
            Assert.Contains(exMsgExpected, ex.Message);
            Assert.Contains(exCodeExpected, ex.Code);
        }

        [Fact]
        public async Task CreateRental_VehicleInactive_ThrowException()
        {
            var rentalRequestFake = _fakes.GetRentalRequestFake();
            string exMsgExpected = $"Vehicle with id: {rentalRequestFake.VehicleId} is inactive";
            string exCodeExpected = "VEHICLE_INACTIVE_EXCEPTION";
            var exExpected = new VehicleInactiveException(exMsgExpected);

            _fakes.RentalsService.Setup(f => f.CreateRentalAsync(It.IsAny<Rental>()))
                .ThrowsAsync(exExpected);

            Func<Task> action = async () => await _fakes.RentalsController.CreateRental(rentalRequestFake);
            var ex = await Assert.ThrowsAsync<VehicleInactiveException>(action);

            Assert.IsType<VehicleInactiveException>(ex);
            Assert.Contains(exMsgExpected, ex.Message);
            Assert.Contains(exCodeExpected, ex.Code);
        }

        [Fact]
        public async Task CreateRental_ClientInactive_ThrowException()
        {
            var rentalRequestFake = _fakes.GetRentalRequestFake();
            string exMsgExpected = $"Client with id: {rentalRequestFake.ClientId} is inactive";
            string exCodeExpected = "CLIENT_INACTIVE_EXCEPTION";
            var exExpected = new ClientInactiveException(exMsgExpected);

            _fakes.RentalsService.Setup(f => f.CreateRentalAsync(It.IsAny<Rental>()))
                .ThrowsAsync(exExpected);

            Func<Task> action = async () => await _fakes.RentalsController.CreateRental(rentalRequestFake);
            var ex = await Assert.ThrowsAsync<ClientInactiveException>(action);

            Assert.IsType<ClientInactiveException>(ex);
            Assert.Contains(exMsgExpected, ex.Message);
            Assert.Contains(exCodeExpected, ex.Code);
        }

        [Fact]
        public async Task CreateRental_VehicleUnavailable_ThrowException()
        {
            var rentalRequestFake = _fakes.GetRentalRequestFake();
            string exMsgExpected = $"Vehicle with id: {rentalRequestFake.VehicleId} is unavailable";
            string exCodeExpected = "VEHICLE_UNAVAILABLE_EXCEPTION";
            var exExpected = new VehicleUnavailableException(exMsgExpected);

            _fakes.RentalsService.Setup(f => f.CreateRentalAsync(It.IsAny<Rental>()))
                .ThrowsAsync(exExpected);

            Func<Task> action = async () => await _fakes.RentalsController.CreateRental(rentalRequestFake);
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

            _fakes.RentalsService.Setup(f => f.DeleteByIdAsync(id))
                .ThrowsAsync(exExpected);

            Func<Task> action = async () => await _fakes.RentalsController.DeleteRental(id);
            var ex = await Assert.ThrowsAsync<EntityNotFoundException>(action);

            Assert.IsType<EntityNotFoundException>(ex);
            Assert.Contains(exMsgExpected, ex.Message);
            Assert.Contains(exCodeExpected, ex.Code);
        }

        [Fact]
        public async Task DeleteRental_RentalExist_RrturnDeleted()
        {
            int id = 10;
            _fakes.RentalsService.Setup(f => f.DeleteByIdAsync(id))
                .ReturnsAsync(true);

            var result = await _fakes.RentalsController.DeleteRental(id);

            Assert.IsType<NoContentResult>(result);
        }
    }
}
