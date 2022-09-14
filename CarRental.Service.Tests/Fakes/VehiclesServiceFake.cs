using CarRental.Data.DAOs.Vehicles;
using CarRental.Domain.Models;
using CarRental.Service.Vehicles;
using Microsoft.Extensions.Logging;
using Moq;

namespace CarRental.Service.Tests.Fakes
{
    internal class VehiclesServiceFake
    {
        public Mock<IVehiclesDao> VehicleDaoMock { get; set; }
        public VehiclesService VehiclesService { get; set; }
        private Mock<ILogger<VehiclesService>> _loggerMock { get; set; }

        public VehiclesServiceFake()
        {
            VehicleDaoMock = new Mock<IVehiclesDao>();
            _loggerMock = new Mock<ILogger<VehiclesService>>();

            VehiclesService = new VehiclesService(
                VehicleDaoMock.Object,
                _loggerMock.Object);
        }

        public List<Vehicle> Result_Dao_GetAll_WithData()
        {
            return new List<Vehicle>()
            {
                new Vehicle()
                {
                    Id = 1,
                    Model = "ModelTest",
                    PricePerDay = 10,
                    Active = true
                },
                new Vehicle()
                {
                    Id = 2,
                    Model = "AhotherModelTest",
                    PricePerDay = 20,
                    Active = false
                },
                new Vehicle()
                {
                    Id = 3,
                    Model = "LastModelTest",
                    PricePerDay = 30,
                    Active = true
                }
            };
        }

        public List<Vehicle> Result_Dao_GetAll_WithoutData()
            => new List<Vehicle>();

        public Vehicle Result_Dao_CreateVehicle()
        {
            return new Vehicle()
            {
                Model = "AnotherModel",
                PricePerDay = 11
            };
        }
    }
}
