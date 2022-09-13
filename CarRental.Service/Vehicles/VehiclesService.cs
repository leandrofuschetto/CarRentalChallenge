using CarRental.Data.DAOs.Vehicles;
using CarRental.Domain.Exceptions;
using CarRental.Domain.Models;
using CarRental.Service.Utils;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace CarRental.Service.Vehicles
{
    public class VehiclesService : IVehiclesService
    {
        private string GetActualAsyncMethodName([CallerMemberName] string name = "") => name;
        private IVehiclesDao _vehicleDao;
        private readonly ILogger<VehiclesService> _logger;
        private readonly MethodBase _methodBase;

        public VehiclesService(IVehiclesDao vehicleDao, ILogger<VehiclesService> logger)
        {
            _vehicleDao = vehicleDao;
            _logger = logger;
            _methodBase = MethodBase.GetCurrentMethod()!;
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
                LogHelper.Log(_logger, "Model in Use",
                        LogHelper.ERROR,
                        _methodBase.ReflectedType?.Name!,
                        GetActualAsyncMethodName());

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
                LogHelper.Log(_logger, "Vehicle alredy deleted. Returns true",
                            LogHelper.INFORMATION,
                            _methodBase.ReflectedType?.Name!,
                            GetActualAsyncMethodName());

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
                LogHelper.Log(_logger, "Vehicle not found",
                    LogHelper.ERROR,
                    _methodBase.ReflectedType?.Name!,
                    GetActualAsyncMethodName());

                throw new EntityNotFoundException(
                $"Vehicle with id: {id} not found",
                "VEHICLE_NOT_FOUND");
            }

            return vehicle;
        }
    }
}
