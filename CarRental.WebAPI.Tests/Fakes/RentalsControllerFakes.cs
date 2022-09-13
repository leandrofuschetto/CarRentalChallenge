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
            var dateTo = new DateTime(2022, 01, 05);
            var dateFrom = new DateTime(2022, 01, 10);

            List<Rental> listResult = new();
            listResult.Add(new Rental()
            {
                Id = 1,
                Active = true,
                Vehicle = new Vehicle() { Id = 1 },
                Client = new Client() { Id = 1 },
                DateFrom = DateOnly.FromDateTime(dateTo),
                DateTo = DateOnly.FromDateTime(dateFrom),
                Price = 100
            });
            listResult.Add(new Rental()
            {
                Id = 2,
                Active = false,
                Vehicle = new Vehicle() { Id = 2 },
                Client = new Client() { Id = 2 },
                DateFrom = DateOnly.FromDateTime(dateTo.AddMonths(1)),
                DateTo = DateOnly.FromDateTime(dateFrom.AddMonths(1)),
                Price = 200
            });
            listResult.Add(new Rental()
            {
                Id = 3,
                Active = true,
                Vehicle = new Vehicle() { Id = 3 },
                Client = new Client() { Id = 3 },
                DateFrom = DateOnly.FromDateTime(dateTo.AddMonths(3)),
                DateTo = DateOnly.FromDateTime(dateFrom.AddMonths(3)),
                Price = 300
            });

            return listResult;
        }

        public CreateRentalRequest GetRentalRequestFake()
        {
            var dateTo = new DateTime(2022, 01, 05);
            
            return new CreateRentalRequest()
            {
                VehicleId = 1,
                ClientId = 1,
                DateFrom = dateTo,
                DateTo = dateTo.AddDays(5)
            };
        }

        public Rental GetRentalExpectedFake()
        {
            var dateTo = new DateTime(2022, 01, 05);

            return new Rental()
            {
                Id = 4,
                Active = true,
                Vehicle = new Vehicle() { Id = 4 },
                Client = new Client() { Id = 4 },
                DateFrom = DateOnly.FromDateTime(dateTo),
                DateTo = DateOnly.FromDateTime(dateTo.AddDays(5)),
                Price = 400
            };
        }
    }
}
