using AutoMapper;
using CarRental.Data.Entities;
using CarRental.Domain.Exceptions;
using CarRental.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace CarRental.Data.DAOs.Rentals
{
    public class RentalsDao : IRentalsDao
    {
        private readonly CarRentalContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<RentalsDao> _logger;
        private readonly string CLASS_NAME = typeof(RentalsDao).Name;

        public RentalsDao(
            CarRentalContext context, 
            IMapper mapper,
            ILogger<RentalsDao> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<Rental>> GetAllRentalsAsync(bool active)
        {
            try
            {
                var listRentals = await _context.Rentals
                    .Include(r => r.Vehicle)
                    .Include(r => r.Client)
                    .Where(c => c.Active == active)
                    .ToListAsync();

                return _mapper.Map<IEnumerable<Rental>>(listRentals);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    "An error ocurrs when getting Rentals. At {0}, {1}",
                    CLASS_NAME,
                    "GetAllRentalsAsync");

                throw new DataBaseContextException(ex.Message, ex.InnerException);
            }
        }

        public async Task<Rental> GetRentalByIdAsync(int id)
        {
            try
            {
                var rentalEntity = await _context.Rentals
                    .Include(r => r.Vehicle)
                    .Include(r => r.Client)
                    .FirstOrDefaultAsync(r => r.Id == id);

                return _mapper.Map<Rental>(rentalEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    "{0} - An error ocurrs when getting rental with Id:{1}. At {2}, {3}",
                    DateTime.Now,
                    id,
                    CLASS_NAME,
                    "GetRentalByIdAsync");

                throw new DataBaseContextException(ex.Message, ex.InnerException);
            }
        }

        public async Task<Rental> CreateRentalAsync(Rental rental)
        {
            try
            {
                var rentalEntity = _mapper.Map<RentalEntity>(rental);
                rentalEntity.Active = true;

                _context.Entry(rentalEntity.Client).State = EntityState.Unchanged;
                _context.Entry(rentalEntity.Vehicle).State = EntityState.Unchanged;

                _context.Rentals.Add(rentalEntity);
                await _context.SaveChangesAsync();

                return _mapper.Map<Rental>(rentalEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error {0} creating rental: {1}. At {2}, {3}",
                    DateTime.Now,
                    JsonSerializer.Serialize(rental),
                    CLASS_NAME,
                    "CreateRentalAsync");

                throw new DataBaseContextException(ex.Message, ex.InnerException);
            }
        }

        public async Task<bool> DeleteByIdAsync(Rental rental)
        {
            try
            {
                var rentalEntity = await _context.Rentals
                    .FirstOrDefaultAsync(r => r.Id == rental.Id);

                _context.Rentals.Attach(rentalEntity);
                rentalEntity.Active = false;

                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error deleting rental: {1}. At {2}, {3}",
                    rental.Id,
                    CLASS_NAME,
                    "DeleteByIdAsync");

                throw new DataBaseContextException(ex.Message, ex.InnerException);
            }
        }

        public async Task<bool> VehicleAvailable(Rental rental)
        {
            try
            {
                var vehicleUnavailable = await _context.Rentals.
                    AnyAsync(r => r.Vehicle.Id.Equals(rental.Vehicle.Id)
                    && r.DateFrom <= rental.DateTo
                    && r.DateTo >= rental.DateFrom
                    && r.Active == true);

                return !vehicleUnavailable;
            }
            catch
            {
                throw new DataBaseContextException();
            }
        }
    }
}
