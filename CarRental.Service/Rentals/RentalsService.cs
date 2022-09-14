using CarRental.Data.DAOs.Clients;
using CarRental.Data.DAOs.Rentals;
using CarRental.Data.DAOs.Vehicles;
using CarRental.Domain.Exceptions;
using CarRental.Domain.Models;
using Microsoft.Extensions.Logging;

namespace CarRental.Service.Rentals
{
    public class RentalsService : IRentalsService
    {
        private readonly IRentalsDao _rentalsDao;
        private readonly IVehiclesDao _vehiclesDao;
        private readonly IClientsDao _clientDao;
        private readonly ILogger<RentalsService> _logger;
        private readonly string CLASS_NAME = typeof(RentalsService).Name;

        public RentalsService(
            IRentalsDao rentalsDao, 
            IVehiclesDao vehiclesDao, 
            IClientsDao clientDao,
            ILogger<RentalsService> logger)
        {
            _rentalsDao = rentalsDao;
            _vehiclesDao = vehiclesDao;
            _clientDao = clientDao;
            _logger = logger;
        }

        public async Task<IEnumerable<Rental>> GetAllRentalsAsync(bool active)
        {
            var rentals = await _rentalsDao.GetAllRentalsAsync(active);

            return rentals;
        }

        public async Task<Rental> GetRentalByIdAsync(int id)
        {
            var rental = await FindRentalByIdAsync(id);
            return rental;
        }

        public async Task<Rental> CreateRentalAsync(Rental rental)
        {
            var vehicle = await _vehiclesDao.GetVehicleByIdAsync(rental.Vehicle.Id);
            ValidateVehicle(vehicle);

            var client = await _clientDao.GetClientByIdAsync(rental.Client.Id);
            ValidateClient(client);

            await ValidateAvailability(rental);

            rental.Client = client;
            rental.Vehicle = vehicle;

            rental.Price = CalculatePrice(rental);            
            
            var rentalCreated = await _rentalsDao.CreateRentalAsync(rental);

            return rentalCreated;
        }

        public async Task<bool> DeleteByIdAsync(int id)
        {
            var rental = await FindRentalByIdAsync(id);

            bool rentalIsInEffect = Utils.IsInRange(
                rental.DateFrom, 
                rental.DateTo, 
                DateOnly.FromDateTime(DateTime.Now));

            if (rentalIsInEffect)
            {
                _logger.LogError(
                    "Rental is in Effect. Cant Delete. At {0}, {1}",
                    CLASS_NAME,
                    Utils.GetActualAsyncMethodName());

                throw new RentalInEffectException("Rental is in Effect");
            }

            if (!rental.Active)
            {
                _logger.LogInformation(
                    "Rental already deleted. Returns ok. At {0}, {1}",
                    CLASS_NAME,
                    Utils.GetActualAsyncMethodName());

                return true;
            }

            var deletedOk = await _rentalsDao.DeleteByIdAsync(rental);

            return deletedOk;
        }

        public decimal CalculatePrice(Rental rental)
        {
            int includeFirstDay = 1;
            int daysCounts = (rental.DateTo.DayNumber - rental.DateFrom.DayNumber);
            int rentalDays = daysCounts + includeFirstDay;
            
            decimal price = rentalDays * rental.Vehicle.PricePerDay;

            return price;
        }

        private async Task<Rental> FindRentalByIdAsync(int id)
        {
            var rental = await _rentalsDao.GetRentalByIdAsync(id);

            if (rental == null)
            {
                _logger.LogError(
                    "Rental Not Found. At {0}, {1}",
                    CLASS_NAME,
                    Utils.GetActualAsyncMethodName());

                throw new EntityNotFoundException(
                    $"Rental with id: {id} not found",
                    "RENTAL_NOT_FOUND");
            }

            return rental;
        }

        private void ValidateVehicle(Vehicle vehicle)
        {
            if (vehicle == null)
            {
                _logger.LogError(
                    "Vehicle not found. At {0}, {1}",
                    CLASS_NAME,
                    Utils.GetActualAsyncMethodName());

                throw new EntityNotFoundException(
                $"Vehicle not found",
                "VEHICLE_NOT_FOUND");
            }

            if (!vehicle.Active)
            {
                string exVehicleInactive = $"Vehicle with id: {vehicle.Id} is inactive";

                _logger.LogError(
                    "Vehicle Inactive. At {0}, {1}",
                    CLASS_NAME,
                    Utils.GetActualAsyncMethodName());

                throw new VehicleInactiveException(exVehicleInactive);
            }
        }

        private void ValidateClient(Client client)
        {
            if (client == null)
            {
                _logger.LogError(
                    "Client Not Found. At {0}, {1}",
                    CLASS_NAME,
                    Utils.GetActualAsyncMethodName());

                throw new EntityNotFoundException(
                    $"Client not found",
                    "CLIENT_NOT_FOUND");
            }

            if (!client.Active)
            {
                string exClientInactive = $"Client with id: {client.Id} is inactive";

                _logger.LogError(
                    "Client Inactive. At {0}, {1}",
                    CLASS_NAME,
                    Utils.GetActualAsyncMethodName());

                throw new ClientInactiveException(exClientInactive);
            }
                
        }

        private async Task ValidateAvailability(Rental rental)
        {
            string exVehicleUnavailabe = $"Vehicle with id: {rental.Vehicle.Id} is unavailable";

            var vehicleAvailable = await _rentalsDao.VehicleAvailable(rental);
            if (!vehicleAvailable)
            {
                _logger.LogError(
                    "Vehicle Unavailable. At {0}, {1}",
                    CLASS_NAME,
                    Utils.GetActualAsyncMethodName());

                throw new VehicleUnavailableException(exVehicleUnavailabe);
            }
        }
    }
}
