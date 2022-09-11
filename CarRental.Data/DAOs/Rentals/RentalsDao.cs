using AutoMapper;
using CarRental.Data.Entities;
using CarRental.Domain.Exceptions;
using CarRental.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Data.DAOs.Rentals
{
    public class RentalsDao
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
            var rentalEntity = await _context.Rentals.FindAsync(id);

            return _mapper.Map<Rental>(rentalEntity);
        }

        public async Task<Rental> CreateRentalAsync(Rental rental)
        {
            var rentalEntity = _mapper.Map<RentalEntity>(rental);
            rentalEntity.Active = true;

            await _context.Rentals.AddAsync(rentalEntity);
            await _context.SaveChangesAsync();

            return _mapper.Map<Rental>(rentalEntity);
        }

        public async Task<bool> DeleteByIdAsync(Rental rental)
        {
            try
            {
                var rentalEntity = _mapper.Map<RentalEntity>(rental);

                _context.Rentals.Attach(rentalEntity);
                rentalEntity.Active = false;
                _context.Rentals.Update(rentalEntity);

                return await _context.SaveChangesAsync() > 0;
            }
            catch
            {
                throw new DataBaseContextException();
            }
        }

        public async Task<bool> VehicleAvailable(Rental rental)
        {

            var vehicle = await _context.Rentals.Where
                (r => r.VehicleId == rental.Vehicle.VehicleId
                && r.DateFrom >= rental.DateFrom
                && r.DateTo < rental.DateTo).FirstOrDefaultAsync();

            return vehicle == null;
        }
    }
}
