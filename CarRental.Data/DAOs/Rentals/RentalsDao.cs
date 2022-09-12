using AutoMapper;
using CarRental.Data.Entities;
using CarRental.Domain.Exceptions;
using CarRental.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Data.DAOs.Rentals
{
    public class RentalsDao : IRentalsDao
    {
        private readonly CarRentalContext _context;
        private readonly IMapper _mapper;

        public RentalsDao(CarRentalContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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
            catch
            {
                throw new DataBaseContextException();
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
            catch
            {
                throw new DataBaseContextException();
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
            catch
            {
                throw new DataBaseContextException();
            }
        }

        public async Task<bool> VehicleAvailable(Rental rental)
        {
            try
            {
                var available = await _context.Rentals.
                    AnyAsync(r => r.VehicleId.Equals(rental.Vehicle.Id)
                    && r.DateFrom >= rental.DateFrom
                    && r.DateTo < rental.DateTo);

                return available;
            }
            catch
            {
                throw new DataBaseContextException();
            }
        }
    }
}
