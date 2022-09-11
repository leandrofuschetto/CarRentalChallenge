using AutoMapper;
using CarRental.Data.DAOs.Rentals;
using CarRental.Data.DAOs.Vehicles;
using CarRental.Domain.Exceptions;
using CarRental.Domain.Models;

namespace CarRental.Service.Rentals
{
    public class RentalsService : IRentalsService
    {
        private readonly IRentalsDao _rentalsDao;
        private readonly IVehiclesDao _vehiclesDao;
        
        public RentalsService(IRentalsDao rentalsDao, IVehiclesDao vehiclesDao)
        {
            _rentalsDao = rentalsDao;
            _vehiclesDao = vehiclesDao;
        }

        public async Task<Rental> GetRentalByIdAsync(int id)
        {
            var rental = await FindRentalByIdAsync(id);
            return rental;
        }

        public async Task<Rental> CreateRentalAsync(Rental rental)
        {
            int vehicleId = rental.Vehicle.VehicleId;
            string exMessageInactive = $"Vehicle with id: {vehicleId} is inactive";
            string exMessageUnavailabe = $"Vehicle with id: {vehicleId} is unavailable";

            var vehicleActive = await _vehiclesDao.VehicleActive(vehicleId);
            if (!vehicleActive)
                throw new VehicleInactiveException(exMessageInactive);

            var vehicleAvailable = await _rentalsDao.VehicleAvailable(rental);
            if (!vehicleAvailable)
                throw new VehicleUnavailableException(exMessageUnavailabe);

            var vehicle = await _vehiclesDao.GetVehicleByIdAsync(vehicleId);

            int daysCount = (rental.DateTo - rental.DateFrom).Days;
            decimal price = daysCount * vehicle.PricePerDay;
            rental.Price = price;

            var rentalCreated = await _rentalsDao.CreateRentalAsync(rental);

            return rentalCreated;
        }

        public async Task<bool> DeleteByIdAsync(int id)
        {
            var rental = await FindRentalByIdAsync(id);
            var deletedOk = await _rentalsDao.DeleteByIdAsync(rental);

            return deletedOk;
        }

        private async Task<Rental> FindRentalByIdAsync(int id)
        {
            var rental = await _rentalsDao.GetRentalByIdAsync(id);

            if (rental == null)
                throw new EntityNotFoundException(
                    $"Rental with id: {id} not found",
                    "RENTAL_NOT_FOUND");

            return rental;
        }
    }
}
