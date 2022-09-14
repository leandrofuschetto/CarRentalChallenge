using CarRental.Domain.Models;
using CarRental.Service.Vehicles;
using CarRental.WebAPI.Controllers;
using CarRental.WebAPI.DTOs.Vehicle;
using Microsoft.Extensions.Logging;
using Moq;

namespace CarRental.WebAPI.Tests.Fakes
{
    internal class VehiclesControllerFakes
    {
        public readonly VehiclesController VehiclesController;
        public Mock<IVehiclesService> VehiclesService;
        private Mock<ILogger<VehiclesController>> _loggerMock { get; set; }

        public VehiclesControllerFakes()
        {
            VehiclesService = new Mock<IVehiclesService>();
            _loggerMock = new Mock<ILogger<VehiclesController>>();

            VehiclesController = new VehiclesController(
                VehiclesService.Object,
                _loggerMock.Object);
        }

        public List<Vehicle> GetListOfVehiclesFake()
        {
            List<Vehicle> listResult = new();
            listResult.Add(new Vehicle()
            {
                Id = 1,
                Active = true,
                Model = "Model1",
                PricePerDay = 1
            });
            listResult.Add(new Vehicle()
            {
                Id = 2,
                Active = true,
                Model = "Model2",
                PricePerDay = 2
            });
            listResult.Add(new Vehicle()
            {
                Id = 3,
                Active = false,
                Model = "Model3",
                PricePerDay = 3
            });
            
            return listResult;
        }

        public CreateVehicleRequest GetVehicleRequestFake()
        {
            return new CreateVehicleRequest()
            {
                Model = "Model4",
                PricePerDay = 4
            };
        }

        public Vehicle GetVehicleExpectedFake()
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
