using AutoMapper;
using CarRental.Domain.Exceptions;
using CarRental.Domain.Models;
using CarRental.Service.Rentals;
using CarRental.Service.Tests.Fakes;
using CarRental.Tests.Helpers;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CarRental.Service.Tests.Rentals
{
    public class RentalsServicesTests
    {
        private readonly RentalsServiceFake _fakes;
        private Utils _utils;
        private IMapper _mapper;

        public RentalsServicesTests()
        {
            _fakes = new RentalsServiceFake();
            _utils = new Utils();
            _mapper = _utils.GetMapper();
        }

        [Fact]
        public async Task GetRentalByIdAsync_RentalExists_ReturnRental()
        {
            int id = 1;
            Rental rental = _fakes.Result_Dao_GetById_WithData();

            _fakes.RentalsDao.Setup(c => c.GetRentalByIdAsync(id))
                .ReturnsAsync(rental);

            var rentalService = new RentalsService(
                _fakes.RentalsDao.Object,
                _fakes.VehiclesDao.Object,
                _mapper);

            var result = await rentalService.GetRentalByIdAsync(id);

            Assert.NotNull(result);
            Assert.Equal(rental, result);
        }

        [Fact]
        public async Task GetRentalByIdAsync_NoExists_EntityNotFoundException()
        {
            int id = 1;
            string exceptionMessage = $"Rental with id: {id} not found";
            Rental rental = null;

            _fakes.RentalsDao.Setup(c => c.GetRentalByIdAsync(id))
                .ReturnsAsync(rental);

            var rentalService = new RentalsService(
                _fakes.RentalsDao.Object,
                _fakes.VehiclesDao.Object,
                _mapper);

            Func<Task> action = async () => await rentalService.GetRentalByIdAsync(id);
            var ex = await Assert.ThrowsAsync<EntityNotFoundException>(action);

            Assert.Contains(exceptionMessage, ex.Message);
        }

        [Fact]
        public async Task CreateRentalAsync_DataCorrect_ReturnRentalCreated()
        {
            Rental fakeRental = new Rental()
            {
                DateFrom = DateTime.Now,
                DateTo = DateTime.Now.AddDays(2),
                Vehicle = new Vehicle() { VehicleId = 1 },
                Client = new Client() { ClientId = 1 }
            };

            Vehicle fakeVehicle = new Vehicle()
            {
                VehicleId = 1,
                PricePerDay = 10,
                Active = true
            };

            int days = 2;
            Rental fakeRentalResult = new Rental()
            {
                RentalId = 5,
                DateFrom = DateTime.Now,
                DateTo = DateTime.Now.AddDays(days),
                Price = fakeVehicle.PricePerDay * days,
                Vehicle = new Vehicle() { VehicleId = 1 },
                Client = new Client() { ClientId = 1 }
            };

            _fakes.VehiclesDao.Setup(v => v.VehicleActive(fakeRental.Vehicle.VehicleId))
                .ReturnsAsync(true);
            _fakes.RentalsDao.Setup(r => r.VehicleAvailable(fakeRental))
                .ReturnsAsync(true);
            _fakes.VehiclesDao.Setup(v => v.GetVehicleByIdAsync(fakeVehicle.VehicleId))
                .ReturnsAsync(fakeVehicle);
            _fakes.RentalsDao.Setup(c => c.CreateRentalAsync(fakeRental))
                .ReturnsAsync(fakeRentalResult);

            var rentalService = new RentalsService(
                _fakes.RentalsDao.Object,
                _fakes.VehiclesDao.Object,
                _mapper);

            var result = await rentalService.CreateRentalAsync(fakeRental);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task CreateRentalAsync_DataCorrect_PriceCalculateCorrectly()
        {
            Rental fakeRental = new Rental()
            {
                DateFrom = DateTime.Now,
                DateTo = DateTime.Now.AddDays(2),
                Vehicle = new Vehicle() { VehicleId = 1 },
                Client = new Client() { ClientId = 1 }
            };

            Vehicle fakeVehicle = new Vehicle()
            {
                VehicleId = 1,
                PricePerDay = 10,
                Active = true
            };

            int days = 2;
            Rental fakeRentalResult = new Rental()
            {
                RentalId = 5,
                DateFrom = DateTime.Now,
                DateTo = DateTime.Now.AddDays(days),
                Price = fakeVehicle.PricePerDay * days,
                Vehicle = new Vehicle() { VehicleId = 1 },
                Client = new Client() { ClientId = 1 }
            };

            int priceExpected = days * fakeVehicle.PricePerDay;

            _fakes.VehiclesDao.Setup(v => v.VehicleActive(fakeRental.Vehicle.VehicleId))
                .ReturnsAsync(true);
            _fakes.RentalsDao.Setup(r => r.VehicleAvailable(fakeRental))
                .ReturnsAsync(true);
            _fakes.VehiclesDao.Setup(v => v.GetVehicleByIdAsync(fakeVehicle.VehicleId))
                .ReturnsAsync(fakeVehicle);
            _fakes.RentalsDao.Setup(c => c.CreateRentalAsync(fakeRental))
                .ReturnsAsync(fakeRentalResult);

            var rentalService = new RentalsService(
                _fakes.RentalsDao.Object,
                _fakes.VehiclesDao.Object,
                _mapper);

            var result = await rentalService.CreateRentalAsync(fakeRental);

            Assert.NotNull(result);
            Assert.Equal(priceExpected, result.Price);
            Assert.Equal(fakeRentalResult.Price, result.Price);
        }

        [Fact]
        public async Task CreateRentalAsync_VehicleInactive_ThrowException()
        {
            Rental fakeRental = new Rental()
            {
                DateFrom = DateTime.Now,
                DateTo = DateTime.Now.AddDays(2),
                Vehicle = new Vehicle() { VehicleId = 1 },
                Client = new Client() { ClientId = 1 }
            };

            Vehicle fakeVehicle = new Vehicle()
            {
                VehicleId = 1,
                PricePerDay = 10,
                Active = false
            };

            string exMessageExpected = $"Vehicle with id: {fakeVehicle.VehicleId} is inactive";

            _fakes.VehiclesDao.Setup(r => r.VehicleActive(fakeVehicle.VehicleId))
                .ReturnsAsync(false);

            var rentalService = new RentalsService(
                _fakes.RentalsDao.Object,
                _fakes.VehiclesDao.Object,
                _mapper);

            Func<Task> action = async () => await rentalService.CreateRentalAsync(fakeRental);

            var ex = await Assert.ThrowsAsync<VehicleInactiveException>(action);
            Assert.Contains(exMessageExpected, ex.Message);
        }

        [Fact]
        public async Task CreateRentalAsync_VehicleUnavailable_ThrowException()
        {
            Rental fakeRental = new Rental()
            {
                DateFrom = DateTime.Now,
                DateTo = DateTime.Now.AddDays(2),
                Vehicle = new Vehicle() { VehicleId = 1 },
                Client = new Client() { ClientId = 1 }
            };

            Vehicle fakeVehicle = new Vehicle()
            {
                VehicleId = 1,
                PricePerDay = 10,
                Active = true
            };
            string exMessageExpected = $"Vehicle with id: {fakeVehicle.VehicleId} is unavailable";

            _fakes.VehiclesDao.Setup(r => r.VehicleActive(fakeVehicle.VehicleId))
                .ReturnsAsync(true);
            _fakes.RentalsDao.Setup(r => r.VehicleAvailable(fakeRental))
                .ReturnsAsync(false);

            var rentalService = new RentalsService(
                _fakes.RentalsDao.Object,
                _fakes.VehiclesDao.Object,
                _mapper);

            Func<Task> action = async () => await rentalService.CreateRentalAsync(fakeRental);

            var ex = await Assert.ThrowsAsync<VehicleUnavailableException>(action);
            Assert.Contains(exMessageExpected, ex.Message);
        }
    }
}
