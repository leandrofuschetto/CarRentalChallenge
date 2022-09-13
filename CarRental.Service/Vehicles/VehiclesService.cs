using CarRental.Data.DAOs.Vehicles;
using CarRental.Domain.Exceptions;
using CarRental.Domain.Models;
using Microsoft.Extensions.Logging;

namespace CarRental.Service.Vehicles
{
    public class VehiclesService : IVehiclesService
    {
        private IVehiclesDao _vehicleDao;
        private readonly ILogger<VehiclesService> _logger;
        private readonly string CLASS_NAME = typeof(VehiclesService).Name;
        public VehiclesService(IVehiclesDao vehicleDao, ILogger<VehiclesService> logger)
        {
            _vehicleDao = vehicleDao;
            _logger = logger;
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
            {
                _logger.LogError("Model in Use. At {0}, {1}",
                    CLASS_NAME,
                    Utils.GetActualAsyncMethodName());

                throw new ModelVehicleInUseException($"The model: {vehicle.Model} is in Use");
            }

            var vehicleCreated = await _vehicleDao.CreateVehicleAsync(vehicle);

            return vehicleCreated;
        }

        public async Task<bool> DeleteByIdAsync(int id)
        {
            var vehicle = await FindVehicleByIdAsync(id);

            if (!vehicle.Active)
            {
                _logger.LogInformation("Vehicle alredy deleted. Returns true. At {0}, {1}",
                    CLASS_NAME,
                    Utils.GetActualAsyncMethodName());

                return true;
            }

            var deletedOk = await _vehicleDao.DeleteByIdAsync(vehicle);

            return deletedOk;
        }

        private async Task<Vehicle> FindVehicleByIdAsync(int id)
        {
            var vehicle = await _vehicleDao.GetVehicleByIdAsync(id);

            if (vehicle == null)
            {
                _logger.LogError("Vehicle not found. At {0}, {1}",
                    CLASS_NAME,
                    Utils.GetActualAsyncMethodName());

                throw new EntityNotFoundException(
                $"Vehicle with id: {id} not found",
                "VEHICLE_NOT_FOUND");
            }

            return vehicle;
        }
    }
}
