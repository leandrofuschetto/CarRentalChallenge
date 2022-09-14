using CarRental.Data.DAOs.Clients;
using CarRental.Data.DAOs.Rentals;
using CarRental.Data.DAOs.Vehicles;
using CarRental.Domain.Models;
using CarRental.Service.Rentals;
using Microsoft.Extensions.Logging;
using Moq;

namespace CarRental.Service.Tests.Fakes
{
    internal class RentalsServiceFake
    {
        public Mock<IRentalsDao> RentalsDaoMock { get; set; }
        public Mock<IVehiclesDao> VehiclesDaoMock { get; set; }
        public Mock<IClientsDao> ClientsDaoMock { get; set; }
        private Mock<ILogger<RentalsService>> _loggerMock { get; set; }

        public RentalsService RentalService { get; set; }

        public RentalsServiceFake()
        {
            RentalsDaoMock = new Mock<IRentalsDao>();
            VehiclesDaoMock = new Mock<IVehiclesDao>();
            ClientsDaoMock = new Mock<IClientsDao>();
            _loggerMock = new Mock<ILogger<RentalsService>>();

            RentalService = new RentalsService(
                RentalsDaoMock.Object,
                VehiclesDaoMock.Object,
                ClientsDaoMock.Object,
                _loggerMock.Object);
        }

        public List<Rental> Result_Dao_GetAll_WithData()
        {
            var dateTo = new DateTime(2022, 01, 05);

            return new List<Rental>()
            {
                new Rental()
                {
                    Id = 1,
                    DateFrom = DateOnly.FromDateTime(dateTo),
                    DateTo = DateOnly.FromDateTime(dateTo.AddDays(5)),
                    Price = 20,
                    Client = new Client() { Id = 1 },
                    Vehicle = new Vehicle() { Id = 1 },
                    Active = true
                },
                new Rental()
                {
                    Id = 2,
                    DateFrom = DateOnly.FromDateTime(dateTo.AddDays(5)),
                    DateTo = DateOnly.FromDateTime(dateTo.AddDays(15)),
                    Price = 20,
                    Client = new Client() { Id = 2 },
                    Vehicle = new Vehicle() { Id = 2 },
                    Active = false
                },
                new Rental()
                {
                    Id = 1,
                    DateFrom = DateOnly.FromDateTime(dateTo.AddDays(15)),
                    DateTo = DateOnly.FromDateTime(dateTo.AddDays(20)),
                    Price = 20,
                    Client = new Client() { Id = 1 },
                    Vehicle = new Vehicle() { Id = 4 },
                    Active = true
                },
            };
        }

        public Rental Result_Dao_GetById_WithData()
        {
            var dateTo = new DateTime(2022, 01, 05);

            return new Rental()
            {
                Id = 1,
                DateFrom = DateOnly.FromDateTime(dateTo),
                DateTo = DateOnly.FromDateTime(dateTo.AddDays(5)),
                Price = 20,
                Client = new Client() { Id = 1 },
                Vehicle = new Vehicle() { Id = 1 }
            };
        }

        public Client Result_ClientDao_GetById(bool active)
        {
            return new Client() 
            { 
                Id = 1, 
                Fullname = "lean", 
                Active = active 
            };
        }

        public Vehicle Result_VehicleDao_GetById(bool active)
        {
            return new Vehicle()
            {
                Id = 1,
                PricePerDay = 10,
                Model = "JustTest",
                Active = active
            };
        }

        public List<Rental> Result_Dao_GetAll_WithoutData()
            => new List<Rental>();

        public Rental FakeRentalInput(int days)
        {
            var date = new DateTime(2022, 01, 01);

            return new Rental()
            {
                DateFrom = DateOnly.FromDateTime(date),
                DateTo = DateOnly.FromDateTime(date.AddDays(days)),
                Vehicle = new Vehicle() { Id = 1 },
                Client = new Client() { Id = 1 }
            };
        }

        public Rental FakeRentalResult(Vehicle fakeVehicle, int days)
        {
            DateTime dateTo = new DateTime(2022, 02, 02);
            return new Rental()
            {
                Id = 5,
                DateFrom = DateOnly.FromDateTime(dateTo),
                DateTo = DateOnly.FromDateTime(dateTo.AddDays(days)),
                Price = fakeVehicle.PricePerDay * days,
                Vehicle = new Vehicle() { Id = 1 },
                Client = new Client() { Id = 1 }
            };
        }

        public void PrepareTestCase(
            Client fakeClient,
            Vehicle fakeVehicle,
            Rental fakeRentalInput,
            Rental fakeRentalResult)
        {
            if (fakeVehicle != null)
            {
                VehiclesDaoMock.Setup(v => v.GetVehicleByIdAsync(fakeVehicle.Id))
                    .ReturnsAsync(fakeVehicle);
            }

            if (fakeClient != null)
            {
                ClientsDaoMock.Setup(c => c.GetClientByIdAsync(fakeClient.Id))
                    .ReturnsAsync(fakeClient);
            }

            if (fakeRentalInput != null)
            {
                RentalsDaoMock.Setup(r => r.VehicleAvailable(fakeRentalInput))
                    .ReturnsAsync(true);
            }

            if (fakeRentalResult != null)
            {
                RentalsDaoMock.Setup(c => c.CreateRentalAsync(fakeRentalInput))
                    .ReturnsAsync(fakeRentalResult);
            }
        }
    }
}
