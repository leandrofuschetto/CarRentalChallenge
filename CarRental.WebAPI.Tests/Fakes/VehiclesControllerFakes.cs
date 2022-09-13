using CarRental.Domain.Models;
using CarRental.Service.Vehicles;
using CarRental.WebAPI.Controllers;
using CarRental.WebAPI.DTOs.Vehicle;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CarRental.WebAPI.Tests.Fakes
{
    internal class VehiclesControllerFakes
    {
        public readonly VehiclesController VehiclesController;
        public Mock<IVehiclesService> VehiclesService;

        public VehiclesControllerFakes()
        {
            VehiclesService = new Mock<IVehiclesService>();
            VehiclesController = new VehiclesController(VehiclesService.Object);
        }

        public  T GetObjectResultContent<T>(ActionResult<T> result)
        {
            return (T)((ObjectResult)result.Result).Value;
        }

        public List<Vehicle> GetListOfVehiclesFake()
        {
            List<Vehicle> listResult = new();
            listResult.Add(new Vehicle() { Id = 1, Active = true, Model = "Model1", PricePerDay = 1 });
            listResult.Add(new Vehicle() { Id = 2, Active = true, Model = "Model2", PricePerDay = 2 });
            listResult.Add(new Vehicle() { Id = 3, Active = false, Model = "Model3", PricePerDay = 3 });

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
