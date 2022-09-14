using AutoMapper;
using CarRental.Data.Entities;
using CarRental.Domain.Exceptions;
using CarRental.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace CarRental.Data.DAOs.Vehicles
{
    public class VehiclesDao : IVehiclesDao
    {
        private readonly CarRentalContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<VehiclesDao> _logger;
        private readonly string CLASS_NAME = typeof(VehiclesDao).Name;

        public VehiclesDao(
            CarRentalContext context, 
            IMapper mapper,
            ILogger<VehiclesDao> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<Vehicle>> GetAllVehiclesAsync(bool active)
        {
            try
            {
                var vehicles = await _context.Vehicles
                    .Where(v => v.Active == active)
                    .ToListAsync();

                return _mapper.Map<IEnumerable<Vehicle>>(vehicles);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    "An error ocurrs when getting vehicles. At {0}, {1}",
                    CLASS_NAME,
                    "GetAllVehiclesAsync");

                throw new DataBaseContextException(ex.Message);
            }
        }

        public async Task<Vehicle> GetVehicleByIdAsync(int id)
        {
            try
            {
                var vehicleEntity = await _context.Vehicles
                    .FirstOrDefaultAsync(v => v.Id == id);
                
                return _mapper.Map<Vehicle>(vehicleEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    "{0} - An error ocurrs when getting Vehicle with Id:{1}. At {2}, {3}",
                    DateTime.Now,
                    id,
                    CLASS_NAME,
                    "GetVehicleByIdAsync");

                throw new DataBaseContextException(ex.Message, ex.InnerException);
            }
        }

        public async Task<Vehicle> CreateVehicleAsync(Vehicle vehicle)
        {
            try
            {
                var vehicleEntity = _mapper.Map<VehicleEntity>(vehicle);
                vehicleEntity.Active = true;

                await _context.Vehicles.AddAsync(vehicleEntity);
                await _context.SaveChangesAsync();

                return _mapper.Map<Vehicle>(vehicleEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error {0} creating vehicle: {1}. At {2}, {3}",
                    DateTime.Now,
                    JsonSerializer.Serialize(vehicle),
                    CLASS_NAME,
                    "CreateVehicleAsync");

                throw new DataBaseContextException(ex.Message, ex.InnerException);
            }
        }

        public async Task<bool> DeleteByIdAsync(Vehicle vehicle)
        {
            try
            {
                var vehicleEntity = await _context.Vehicles
                    .FirstOrDefaultAsync(v => v.Id == vehicle.Id);

                _context.Vehicles.Attach(vehicleEntity);
                vehicleEntity.Active = false;

                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error deleting Vehicle with Id: {1}. At {2}, {3}",
                    vehicle.Id,
                    CLASS_NAME,
                    "DeleteByIdAsync");

                throw new DataBaseContextException(ex.Message, ex.InnerException);
            }
        }

        public async Task<bool> ModelExits(Vehicle vehicle)
        {
            try
            {
                var modelalreadyUsed = await _context.Vehicles
                    .AnyAsync(v => v.Model.ToUpper() == vehicle.Model.ToUpper()
                    && v.Active == true);

                return modelalreadyUsed;
            }
            catch
            {
                _logger.LogError("Error validating Model Vehicle with Id: {1}. At {2}, {3}",
                    vehicle.Id,
                    CLASS_NAME,
                    "VehicleActive");

                throw new DataBaseContextException();
            }

        }

        public async Task<bool> VehicleActive(int vehicleId)
        {
            try
            {
                bool active = await _context.Vehicles
                    .AnyAsync(v => v.Active && v.Id.Equals(vehicleId));

                return active;
            }
            catch
            {
                _logger.LogError("Error validating Vehicle: {1}. At {2}, {3}",
                    vehicleId,
                    CLASS_NAME,
                    "VehicleActive");

                throw new DataBaseContextException();
            }
        }
    }
}
