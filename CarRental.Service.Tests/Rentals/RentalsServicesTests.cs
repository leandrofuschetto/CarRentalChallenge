using CarRental.Domain.Exceptions;
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
        [InlineData(false)]
        public async Task GetAllRentalsAsync_NoData_EmptyResults(bool active)
        {
            int expectedCount = 0;
            List<Rental> listRental = _fakes.Result_Dao_GetAll_WithoutData();
            _fakes.RentalsDaoMock.Setup(c => c.GetAllRentalsAsync(active))
                .ReturnsAsync(listRental);

            var result = await _fakes.RentalService.GetAllRentalsAsync(active);

            Assert.NotNull(result);
            Assert.Equal(expectedCount, result.Count());
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task GetAllRentalsAsync_WithData_ReturnRentals(bool active)
        {
            List<Rental> listRentals = _fakes.Result_Dao_GetAll_WithData()
                .Where(c => c.Active).ToList();
            int expectedCount = listRentals.Count();
            _fakes.RentalsDaoMock.Setup(c => c.GetAllRentalsAsync(active))
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
            _fakes.RentalsDaoMock.Setup(c => c.GetRentalByIdAsync(id))
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
            string exMsgExpected = $"Rental with id: {id} not found";
            _fakes.RentalsDaoMock.Setup(c => c.GetRentalByIdAsync(id))
                .ReturnsAsync(rental);
            
            Func<Task> action = async () => 
                await _fakes.RentalService.GetRentalByIdAsync(id);

            var ex = await Assert.ThrowsAsync<EntityNotFoundException>(action);

            Assert.IsType<EntityNotFoundException>(ex);
            Assert.Equal(exMsgExpected, ex.Message);
        }

        [Fact]
        public async Task CreateRentalAsync_RequestOk_ReturnRentalCreated()
        {
            int days = 2;
            Rental fakeRentalInput = _fakes.FakeRentalInput(days);
            Client fakeClient = _fakes.Result_ClientDao_GetById(true);
            Vehicle fakeVehicle = _fakes.Result_VehicleDao_GetById(true);
            Rental fakeRentalResult = _fakes.FakeRentalResult(fakeVehicle, days);
            _fakes.PrepareTestCase(fakeClient, fakeVehicle, fakeRentalInput, fakeRentalResult);
            
            var result = await _fakes.RentalService.CreateRentalAsync(fakeRentalInput);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task CreateRentalAsync_RequestOk_PriceCalculateCorrectly()
        {
            int days = 2;
            Rental fakeRentalInput = _fakes.FakeRentalInput(days);
            Client fakeClient = _fakes.Result_ClientDao_GetById(true);
            Vehicle fakeVehicle = _fakes.Result_VehicleDao_GetById(true);
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
            Rental fakeRentalInput = _fakes.FakeRentalInput(days);
            Client fakeClient = _fakes.Result_ClientDao_GetById(true);
            Vehicle fakeVehicle = _fakes.Result_VehicleDao_GetById(true);
            Rental fakeRentalResult = _fakes.FakeRentalResult(fakeVehicle, days);
            _fakes.PrepareTestCase(fakeClient, fakeVehicle, fakeRentalInput, fakeRentalResult);
            decimal priceExpected = days * fakeVehicle.PricePerDay;

            var result = await _fakes.RentalService.CreateRentalAsync(fakeRentalInput);

            Assert.NotNull(result);
            Assert.Equal(priceExpected, result.Price);
            Assert.Equal(fakeRentalResult.Price, result.Price);
        }

        [Fact]
        public async Task CreateRentalAsync_VehicleNull_ThrowException()
        {
            Vehicle fakeVehicle = null;
            string exCodeExpected = "VEHICLE_NOT_FOUND";
            string exMessageExpected = $"Vehicle not found";
            Rental fakeRentalInput = _fakes.FakeRentalInput(1);
            _fakes.PrepareTestCase(null, fakeVehicle, fakeRentalInput, null);
            
            Func<Task> action = async () =>
                await _fakes.RentalService.CreateRentalAsync(fakeRentalInput);

            var ex = await Assert.ThrowsAsync<EntityNotFoundException>(action);

            Assert.IsType<EntityNotFoundException>(ex);
            Assert.Equal(exMessageExpected, ex.Message);
            Assert.Equal(exCodeExpected, ex.Code);
        }

        [Fact]
        public async Task CreateRentalAsync_VehicleInactive_ThrowException()
        {
            int days = 1;
            Rental fakeRentalInput = _fakes.FakeRentalInput(days);
            Vehicle fakeVehicle = _fakes.Result_VehicleDao_GetById(false);
            _fakes.PrepareTestCase(null, fakeVehicle, fakeRentalInput, null);
            string exMessageExpected = $"Vehicle with id: {fakeVehicle.Id} is inactive";
            string exCodeExpected = "VEHICLE_INACTIVE_EXCEPTION";

            Func<Task> action = async () => 
                await _fakes.RentalService.CreateRentalAsync(fakeRentalInput);

            var ex = await Assert.ThrowsAsync<VehicleInactiveException>(action);

            Assert.IsType<VehicleInactiveException>(ex);
            Assert.Equal(exMessageExpected, ex.Message);
            Assert.Equal(exCodeExpected, ex.Code);
        }

        [Fact]
        public async Task CreateRentalAsync_ClientInactive_ThrowException()
        {
            int days = 1;
            Client fakeClient = _fakes.Result_ClientDao_GetById(false);
            Vehicle fakeVehicle = _fakes.Result_VehicleDao_GetById(true);
            Rental fakeRentalInput = _fakes.FakeRentalInput(days);
            string exCodeExpected = "CLIENT_INACTIVE_EXCEPTION";
            string exMessageExpected = $"Client with id: {fakeClient.Id} is inactive";
            _fakes.PrepareTestCase(fakeClient, fakeVehicle, fakeRentalInput, null);

            Func<Task> action = async () =>
                await _fakes.RentalService.CreateRentalAsync(fakeRentalInput);

            var ex = await Assert.ThrowsAsync<ClientInactiveException>(action);

            Assert.IsType<ClientInactiveException>(ex);
            Assert.Equal(exMessageExpected, ex.Message);
            Assert.Equal(exCodeExpected, ex.Code);
        }

        [Fact]
        public async Task CreateRentalAsync_ClientNull_ThrowException()
        {
            Client fakeClient = null;
            Vehicle fakeVehicle = _fakes.Result_VehicleDao_GetById(true);
            Rental fakeRentalInput = _fakes.FakeRentalInput(1);
            _fakes.PrepareTestCase(fakeClient, fakeVehicle, fakeRentalInput, null);
            string exMessageExpected = $"Client not found";
            string exCodeExpected = "CLIENT_NOT_FOUND";

            Func<Task> action = async () =>
                await _fakes.RentalService.CreateRentalAsync(fakeRentalInput);

            var ex = await Assert.ThrowsAsync<EntityNotFoundException>(action);

            Assert.IsType<EntityNotFoundException>(ex);
            Assert.Equal(exMessageExpected, ex.Message);
            Assert.Equal(exCodeExpected, ex.Code);
        }

        [Fact]
        public async Task CreateRentalAsync_VehicleUnavailable_ThrowException()
        {
            Client fakeClient = _fakes.Result_ClientDao_GetById(true);
            Vehicle fakeVehicle = _fakes.Result_VehicleDao_GetById(true);
            Rental fakeRentalInput = _fakes.FakeRentalInput(1);
            string exMessageExpected = $"Vehicle with id: {fakeVehicle.Id} is unavailable";
            string exCodeExpected = "VEHICLE_UNAVAILABLE_EXCEPTION";
            _fakes.PrepareTestCase(fakeClient, fakeVehicle, null, null);
            _fakes.RentalsDaoMock.Setup(r => r.VehicleAvailable(fakeRentalInput))
                .ReturnsAsync(false);

            Func<Task> action = async () => 
                await _fakes.RentalService.CreateRentalAsync(fakeRentalInput);

            var ex = await Assert.ThrowsAsync<VehicleUnavailableException>(action);

            Assert.IsType<VehicleUnavailableException>(ex);
            Assert.Equal(exMessageExpected, ex.Message);
            Assert.Equal(exCodeExpected, ex.Code);
        }

        [Fact]
        public async Task DeleteByIdAsync_RentalInEffect_ThrowException()
        {
            string exMsgExpected = "Rental is in Effect";
            string exCodeExpected = "RENTAL_INEFFECT_EXCEPTION";
            Rental rental = _fakes.Result_Dao_GetById_WithData();
            Utils.IsInRange = (x, y, z) => true;
            _fakes.RentalsDaoMock.Setup(c => c.GetRentalByIdAsync(rental.Id))
                .ReturnsAsync(rental);
            

            Func<Task> action = async () =>
                await _fakes.RentalService.DeleteByIdAsync(rental.Id);

            var ex = await Assert.ThrowsAsync<RentalInEffectException>(action);

            Assert.IsType<RentalInEffectException>(ex);
            Assert.Equal(exMsgExpected, ex.Message);
            Assert.Equal(exCodeExpected, ex.Code);
        }

        [Fact]
        public async Task DeleteByIdAsync_RentalalreadyDelete_ReturnsTrue()
        {
            Rental rental = _fakes.Result_Dao_GetById_WithData();
            Utils.IsInRange = (x, y, z) => false;
            _fakes.RentalsDaoMock.Setup(c => c.GetRentalByIdAsync(rental.Id))
                .ReturnsAsync(rental);
            
            var result = await _fakes.RentalService.DeleteByIdAsync(rental.Id);
            
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteByIdAsync_RentalActive_DeletesOk()
        {
            Rental rental = _fakes.Result_Dao_GetById_WithData();
            rental.Active = true;
            Utils.IsInRange = (x, y, z) => false;
            _fakes.RentalsDaoMock.Setup(c => c.GetRentalByIdAsync(rental.Id))
                .ReturnsAsync(rental);
            _fakes.RentalsDaoMock.Setup(r => r.DeleteByIdAsync(rental))
                .ReturnsAsync(true);   
            
            var result = await _fakes.RentalService.DeleteByIdAsync(rental.Id);

            Assert.True(result);
        }
    }
}
