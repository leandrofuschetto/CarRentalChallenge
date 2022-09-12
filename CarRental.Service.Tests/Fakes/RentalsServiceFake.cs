using CarRental.Data.DAOs.Clients;
using CarRental.Data.DAOs.Rentals;
using CarRental.Data.DAOs.Vehicles;
using CarRental.Domain.Models;
using CarRental.Service.Rentals;
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
        public Mock<IClientsDao> ClientsDao { get; set; }
        public RentalsService RentalService { get; set; }

        public RentalsServiceFake()
        {
            RentalsDao = new Mock<IRentalsDao>();
            VehiclesDao = new Mock<IVehiclesDao>();
            ClientsDao = new Mock<IClientsDao>();
            RentalService = new RentalsService(
                RentalsDao.Object,
                VehiclesDao.Object,
                ClientsDao.Object);
        }

        public Rental Result_Dao_GetById_WithData()
        {
            return new Rental()
            {
                Id = 1,
                DateFrom = new DateTime(2022, 02, 7),
                DateTo = new DateTime(2022, 02, 12),
                Price = 20,
                Client = new Client()
                {
                    Id = 1
                },
                Vehicle = new Vehicle()
                {
                    Id = 1
                }
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

        public Rental FakeRentalInput(int days)
        {
            return new Rental()
            {
                DateFrom = DateTime.Now,
                DateTo = DateTime.Now.AddDays(days),
                Vehicle = new Vehicle() { Id = 1 },
                Client = new Client() { Id = 1 }
            };
        }

        internal void PrepareTestCase(
            Client fakeClient, 
            Vehicle fakeVehicle, 
            Rental fakeRentalInput, 
            Rental fakeRentalResult)
        {
            if (fakeVehicle != null)
            { 
                VehiclesDao.Setup(v => v.GetVehicleByIdAsync(fakeVehicle.Id))
                    .ReturnsAsync(fakeVehicle);
            }

            if (fakeClient != null)
            { 
                ClientsDao.Setup(c => c.GetClientByIdAsync(fakeClient.Id))
                    .ReturnsAsync(fakeClient);
            }

            if (fakeRentalInput != null)
            {
                RentalsDao.Setup(r => r.VehicleAvailable(fakeRentalInput))
                    .ReturnsAsync(true);
            }

            if (fakeRentalResult != null)
            { 
                RentalsDao.Setup(c => c.CreateRentalAsync(fakeRentalInput))
                    .ReturnsAsync(fakeRentalResult);
            }
        }

        public Rental FakeRentalResult(Vehicle fakeVehicle, int days)
        {
            return new Rental()
            {
                Id = 5,
                DateFrom = DateTime.Now,
                DateTo = DateTime.Now.AddDays(days),
                Price = fakeVehicle.PricePerDay * days,
                Vehicle = new Vehicle() { Id = 1 },
                Client = new Client() { Id = 1 }
            };
        }
    }
}
