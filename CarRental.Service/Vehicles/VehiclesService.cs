using AutoMapper;
using CarRental.Data.DAOs.Vehicles;
using CarRental.Domain.Exceptions;
using CarRental.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRental.Service.Vehicles
{
    public class VehiclesService : IVehiclesService
    {
        private IVehiclesDao _vehicleDao;

        public VehiclesService(IVehiclesDao vehicleDao)
        {
            _vehicleDao = vehicleDao;
        }

        public async Task<IEnumerable<Vehicle>> GetAllVehiclesAsync(bool active)
        {
            var vehicles = await _vehicleDao.GetAllVehiclesAsync(active);

            return vehicles;
        }

        public async Task<Vehicle> GetVehicleByIdAsync(int id)
        {
            var vehicle = await FindVehicleByIdAsync(id);

            return vehicle;
        }

        public async Task<Vehicle> CreateVehicleAsync(Vehicle vehicle)
        {
            bool modelInUse = await _vehicleDao.ModelExits(vehicle);
            if (modelInUse)
                throw new ModelVehicleInUseException($"The model: {vehicle.Model} is in Use");

            var vehicleCreated = await _vehicleDao.CreateVehicleAsync(vehicle);

            return vehicleCreated;
        }

        public async Task<bool> DeleteByIdAsync(int id)
        {
            var vehicle = await FindVehicleByIdAsync(id);
            
            if (!vehicle.Active)
                return true;

            var deletedOk = await _vehicleDao.DeleteByIdAsync(vehicle);

            return deletedOk;
        }

        private async Task<Vehicle> FindVehicleByIdAsync(int id)
        {
            var vehicle = await _vehicleDao.GetVehicleByIdAsync(id);

            if (vehicle == null)
                throw new EntityNotFoundException(
                    $"Vehicle with id: {id} not found",
                    "VEHICLE_NOT_FOUND");

            return vehicle;
        }
    }
}
