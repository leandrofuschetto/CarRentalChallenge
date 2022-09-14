﻿using CarRental.Domain.Exceptions;
using CarRental.Domain.Models;
using CarRental.Service.Tests.Fakes;
using Moq;
using Xunit;

namespace CarRental.Service.Tests.Rentals
{
    public class RentalsServicesTests
    {
        private readonly RentalsServiceFake _fakes;
        
        public RentalsServicesTests()
        {
            _fakes = new RentalsServiceFake();
        }

        [Theory]
        [InlineData(true)]
        public async Task GetAllRentalsAsync_NoData_EmptyResults(bool active)
        {
            int expectedCount = 0;
            List<Rental> listRental = _fakes.Result_Dao_GetAll_WithoutData();
            _fakes.RentalsDao.Setup(c => c.GetAllRentalsAsync(active))
                .ReturnsAsync(listRental);

            var result = await _fakes.RentalService.GetAllRentalsAsync(active);

            Assert.NotNull(result);
            Assert.Equal(expectedCount, result.Count());
        }

        [Fact]
        public async Task GetAllRentalsAsync_ActiveWithData_ReturnRemovedRentals()
        {
            bool active = true;
            int expectedCount = 2;
            List<Rental> listRentals = _fakes.Result_Dao_GetAll_WithData()
                .Where(c => c.Active).ToList();
            _fakes.RentalsDao.Setup(c => c.GetAllRentalsAsync(active))
                .ReturnsAsync(listRentals);

            var result = await _fakes.RentalService.GetAllRentalsAsync(active);

            Assert.NotNull(result);
            Assert.Equal(expectedCount, result.Count());
        }

        [Fact]
        public async Task GetRentalByIdAsync_RentalExists_ReturnRental()
        {
            int id = 1;
            Rental rental = _fakes.Result_Dao_GetById_WithData();
            _fakes.RentalsDao.Setup(c => c.GetRentalByIdAsync(id))
                .ReturnsAsync(rental);

            var result = await _fakes.RentalService.GetRentalByIdAsync(id);

            Assert.NotNull(result);
            Assert.Equal(rental, result);
        }

        [Fact]
        public async Task GetRentalByIdAsync_NoExists_EntityNotFoundException()
        {
            int id = 1;
            Rental rental = null;
            _fakes.RentalsDao.Setup(c => c.GetRentalByIdAsync(id))
                .ReturnsAsync(rental);
            string exceptionMessage = $"Rental with id: {id} not found";

            Func<Task> action = async () => 
                await _fakes.RentalService.GetRentalByIdAsync(id);

            var ex = await Assert.ThrowsAsync<EntityNotFoundException>(action);

            Assert.Contains(exceptionMessage, ex.Message);
        }

        [Fact]
        public async Task CreateRentalAsync_DataCorrect_ReturnRentalCreated()
        {
            int days = 2;
            Client fakeClient = _fakes.Result_ClientDao_GetById(true);
            Vehicle fakeVehicle = _fakes.Result_VehicleDao_GetById(true);
            Rental fakeRentalInput = _fakes.FakeRentalInput(days);
            Rental fakeRentalResult = _fakes.FakeRentalResult(fakeVehicle, days);
            _fakes.PrepareTestCase(fakeClient, fakeVehicle, fakeRentalInput, fakeRentalResult);
            
            var result = await _fakes.RentalService.CreateRentalAsync(fakeRentalInput);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task CreateRentalAsync_DataCorrect_PriceCalculateCorrectly()
        {
            int days = 2;
            Client fakeClient = _fakes.Result_ClientDao_GetById(true);
            Vehicle fakeVehicle = _fakes.Result_VehicleDao_GetById(true);
            Rental fakeRentalInput = _fakes.FakeRentalInput(days);
            Rental fakeRentalResult = _fakes.FakeRentalResult(fakeVehicle, days);
            _fakes.PrepareTestCase(fakeClient, fakeVehicle, fakeRentalInput, fakeRentalResult);
            decimal priceExpected = days * fakeVehicle.PricePerDay;
            
            var result = await _fakes.RentalService.CreateRentalAsync(fakeRentalInput);

            Assert.NotNull(result);
            Assert.Equal(priceExpected, result.Price);
            Assert.Equal(fakeRentalResult.Price, result.Price);
        }

        [Fact]
        public async Task CreateRentalAsync_RentOneDay_PriceCalculateCorrectly()
        {
            int days = 0;
            Client fakeClient = _fakes.Result_ClientDao_GetById(true);
            Vehicle fakeVehicle = _fakes.Result_VehicleDao_GetById(true);
            Rental fakeRentalInput = _fakes.FakeRentalInput(days);
            Rental fakeRentalResult = _fakes.FakeRentalResult(fakeVehicle, days);
            _fakes.PrepareTestCase(fakeClient, fakeVehicle, fakeRentalInput, fakeRentalResult);
            decimal priceExpected = days * fakeVehicle.PricePerDay;

            var result = await _fakes.RentalService.CreateRentalAsync(fakeRentalInput);

            Assert.NotNull(result);
            Assert.Equal(priceExpected, result.Price);
            Assert.Equal(fakeRentalResult.Price, result.Price);
        }

        [Fact]
        public async Task CreateRentalAsync_VehicleInactive_ThrowException()
        {
            Vehicle fakeVehicle = _fakes.Result_VehicleDao_GetById(false);
            Rental fakeRentalInput = _fakes.FakeRentalInput(1);
            _fakes.PrepareTestCase(null, fakeVehicle, fakeRentalInput, null);
            string exMessageExpected = $"Vehicle with id: {fakeVehicle.Id} is inactive";

            Func<Task> action = async () => 
                await _fakes.RentalService.CreateRentalAsync(fakeRentalInput);

            var ex = await Assert.ThrowsAsync<VehicleInactiveException>(action);
            Assert.Contains(exMessageExpected, ex.Message);
        }

        [Fact]
        public async Task CreateRentalAsync_ClientInactive_ThrowException()
        {
            Client fakeClient = _fakes.Result_ClientDao_GetById(false);
            Vehicle fakeVehicle = _fakes.Result_VehicleDao_GetById(true);
            Rental fakeRentalInput = _fakes.FakeRentalInput(1);
            _fakes.PrepareTestCase(fakeClient, fakeVehicle, fakeRentalInput, null);
            string exMessageExpected = $"Client with id: {fakeClient.Id} is inactive";

            Func<Task> action = async () =>
                await _fakes.RentalService.CreateRentalAsync(fakeRentalInput);

            var ex = await Assert.ThrowsAsync<ClientInactiveException>(action);
            Assert.Contains(exMessageExpected, ex.Message);
        }

        [Fact]
        public async Task CreateRentalAsync_VehicleUnavailable_ThrowException()
        {
            Client fakeClient = _fakes.Result_ClientDao_GetById(true);
            Vehicle fakeVehicle = _fakes.Result_VehicleDao_GetById(true);
            Rental fakeRentalInput = _fakes.FakeRentalInput(1);
            _fakes.PrepareTestCase(fakeClient, fakeVehicle, null, null);
            _fakes.RentalsDao.Setup(r => r.VehicleAvailable(fakeRentalInput))
                .ReturnsAsync(false);
            string exMessageExpected = $"Vehicle with id: {fakeVehicle.Id} is unavailable";

            Func<Task> action = async () => 
                await _fakes.RentalService.CreateRentalAsync(fakeRentalInput);

            var ex = await Assert.ThrowsAsync<VehicleUnavailableException>(action);
            Assert.Contains(exMessageExpected, ex.Message);
        }

        
        [Fact]
        public async Task DeleteByIdAsync_RentalInEffect_ThrowException()
        {
            Rental rental = _fakes.Result_Dao_GetById_WithData();
            _fakes.RentalsDao.Setup(c => c.GetRentalByIdAsync(rental.Id))
                .ReturnsAsync(rental);
            Utils.IsInRange = (x, y, z) => true;

            string exMessageExpected = $"Rental is in Effect";

            Func<Task> action = async () =>
                await _fakes.RentalService.DeleteByIdAsync(rental.Id);

            var ex = await Assert.ThrowsAsync<RentalInEffectException>(action);
            Assert.Contains(exMessageExpected, ex.Message);
        }

        [Fact]
        public async Task DeleteByIdAsync_RentalAlredyDelete_ReturnsTrue()
        {
            Rental rental = _fakes.Result_Dao_GetById_WithData();
            
            _fakes.RentalsDao.Setup(c => c.GetRentalByIdAsync(rental.Id))
                .ReturnsAsync(rental);
            Utils.IsInRange = (x, y, z) => false;

            var result = await _fakes.RentalService.DeleteByIdAsync(rental.Id);
            
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteByIdAsync_RentalActive_DeletesOk()
        {
            Rental rental = _fakes.Result_Dao_GetById_WithData();
            rental.Active = true;
            _fakes.RentalsDao.Setup(c => c.GetRentalByIdAsync(rental.Id))
                .ReturnsAsync(rental);
            _fakes.RentalsDao.Setup(r => r.DeleteByIdAsync(rental))
                .ReturnsAsync(true);   
            Utils.IsInRange = (x, y, z) => false;
            
            var result = await _fakes.RentalService.DeleteByIdAsync(rental.Id);

            Assert.True(result);
        }
    }
}
