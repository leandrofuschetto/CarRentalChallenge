using CarRental.Domain.Models;
using CarRental.Service.Rentals;
using CarRental.WebAPI.Controllers;
using CarRental.WebAPI.DTOs.Rental;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CarRental.WebAPI.Tests.Fakes
{
    internal class RentalsControllerFakes
    {
        public RentalsController RentalsController { get; set; }
        public Mock<IRentalsService> RentalsService { get; set; }

        public RentalsControllerFakes()
        {
            RentalsService = new Mock<IRentalsService>();
            RentalsController = new RentalsController(RentalsService.Object);
        }

        public T GetObjectResultContent<T>(ActionResult<T> result)
        {
            return (T)((ObjectResult)result.Result).Value;
        }

        public List<Rental> GetListOfRentalsFake()
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

        public CreateRentalRequest GetRentalRequestFake()
        {
            return new CreateRentalRequest()
            {
                VehicleId = 1,
                ClientId = 1,
                DateFrom = new DateTime(2022, 03, 05),
                DateTo = new DateTime(2022, 03, 10)
            };
        }

        public Rental GetRentalExpectedFake()
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
