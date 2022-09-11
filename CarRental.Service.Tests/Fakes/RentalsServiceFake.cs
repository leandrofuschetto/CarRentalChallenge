using CarRental.Data.DAOs.Rentals;
using CarRental.Data.DAOs.Vehicles;
using CarRental.Domain.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRental.Service.Tests.Fakes
{
    internal class RentalsServiceFake
    {
        public Mock<IRentalsDao> RentalsDao { get; set; }
        public Mock<IVehiclesDao> VehiclesDao { get; set; }

        public RentalsServiceFake()
        {
            RentalsDao = new Mock<IRentalsDao>();
            VehiclesDao = new Mock<IVehiclesDao>();
        }

        public Rental Result_Dao_GetById_WithData()
        {
            return new Rental()
            {
                RentalId = 1,
                DateFrom = new DateTime(2022, 02, 7),
                DateTo = new DateTime(2022, 02, 12),
                Price = 20,
                Client = new Client()
                {
                    ClientId = 1
                },
                Vehicle = new Vehicle()
                {
                    VehicleId = 1
                }
            };
        }
    }
}
