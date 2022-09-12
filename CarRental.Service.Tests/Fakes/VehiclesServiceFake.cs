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
    internal class VehiclesServiceFake
    {
        public Mock<IVehiclesDao> VehicleDao { get; set; }

        public VehiclesServiceFake()
        {
            VehicleDao = new Mock<IVehiclesDao>();
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
    }
}
