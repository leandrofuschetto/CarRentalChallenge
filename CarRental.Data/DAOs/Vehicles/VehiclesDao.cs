using AutoMapper;
using CarRental.Data.Entities;
using CarRental.Domain.Exceptions;
using CarRental.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Data.DAOs.Vehicles
{
    public class VehiclesDao : IVehiclesDao
    {
        private readonly CarRentalContext _context;
        private readonly IMapper _mapper;

        public VehiclesDao(CarRentalContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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
            catch
            {
                throw new DataBaseContextException();
            }
        }

        public async Task<Vehicle> GetVehicleByIdAsync(int id)
        {
            try
            {
                var vehicleEntity = await _context.Vehicles.FindAsync(id);

                return _mapper.Map<Vehicle>(vehicleEntity);
            }
            catch
            {
                throw new DataBaseContextException();
            }
        }

        public async Task<Vehicle> CreateVehicleAsync(Vehicle vehicle)
        {
            var vehicleEntity = _mapper.Map<VehicleEntity>(vehicle);
            vehicleEntity.Active = true;

            await _context.Vehicles.AddAsync(vehicleEntity);
            await _context.SaveChangesAsync();

            return _mapper.Map<Vehicle>(vehicleEntity);
        }

        public async Task<bool> DeleteByIdAsync(Vehicle vehicle)
        {
            var vehicleEntity = _mapper.Map<VehicleEntity>(vehicle);

            _context.Vehicles.Attach(vehicleEntity);
            _context.Vehicles.Remove(vehicleEntity);

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> ModelExits(Vehicle vehicle)
        {
            var modelAlredyUse = await _context.Vehicles
                    .Where(v => v.Model == vehicle.Model)
                    .FirstOrDefaultAsync() != null;

            return modelAlredyUse;
        }

        public async Task<bool> VehicleActive(int vehicleId)
        {
            var result = await _context.Vehicles
                .Where(v => v.Active && v.VehicleId == vehicleId)
                .FirstOrDefaultAsync();

            return result != null;
        }
    }
}
