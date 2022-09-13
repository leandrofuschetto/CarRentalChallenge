﻿using CarRental.Data.DAOs.Clients;
using CarRental.Data.DAOs.Rentals;
using CarRental.Data.DAOs.Vehicles;
using CarRental.Domain.Exceptions;
using CarRental.Domain.Models;
using CarRental.Service.Utils;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace CarRental.Service.Rentals
{
    public class RentalsService : IRentalsService
    {
        private string GetActualAsyncMethodName([CallerMemberName] string name = "") => name;
        private readonly IRentalsDao _rentalsDao;
        private readonly IVehiclesDao _vehiclesDao;
        private readonly IClientsDao _clientDao;
        private readonly ILogger<RentalsService> _logger;
        private readonly MethodBase _methodBase;

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
            _methodBase = MethodBase.GetCurrentMethod()!;
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

            if (!rental.Active)
            {
                LogHelper.Log(_logger, "Rental alredy deleted. Returns ok",
                        LogHelper.INFORMATION,
                        _methodBase.ReflectedType?.Name!,
                        GetActualAsyncMethodName());

                return true;
            }

            var deletedOk = await _rentalsDao.DeleteByIdAsync(rental);

            return deletedOk;
        }

        private async Task<Rental> FindRentalByIdAsync(int id)
        {
            var rental = await _rentalsDao.GetRentalByIdAsync(id);

            if (rental == null)
            {
                LogHelper.Log(_logger, "Rental Not found",
                        LogHelper.ERROR,
                        _methodBase.ReflectedType?.Name!,
                        GetActualAsyncMethodName());

                throw new EntityNotFoundException(
                    $"Rental with id: {id} not found",
                    "RENTAL_NOT_FOUND");
            }

            return rental;
        }

        private void ValidateVehicle(Vehicle vehicle)
        {
            string exVehicleInactive = $"Vehicle with id: {vehicle.Id} is inactive";

            if (!vehicle.Active)
            {
                LogHelper.Log(_logger, "Vehicle Inactive",
                        LogHelper.ERROR,
                        _methodBase.ReflectedType?.Name!,
                        GetActualAsyncMethodName());

                throw new VehicleInactiveException(exVehicleInactive);
            }
        }

        private void ValidateClient(Client client)
        {
            string exClientInactive = $"Client with id: {client.Id} is inactive";

            if (!client.Active)
            {
                LogHelper.Log(_logger, "Client Inactive",
                        LogHelper.ERROR,
                        _methodBase.ReflectedType?.Name!,
                        GetActualAsyncMethodName());

                throw new ClientInactiveException(exClientInactive);
            }
                
        }

        private async Task ValidateAvailability(Rental rental)
        {
            string exVehicleUnavailabe = $"Vehicle with id: {rental.Vehicle.Id} is unavailable";

            var vehicleAvailable = await _rentalsDao.VehicleAvailable(rental);
            if (!vehicleAvailable)
            {
                LogHelper.Log(_logger, "Vehicle Unavailable",
                    LogHelper.ERROR,
                    _methodBase.ReflectedType?.Name!,
                    GetActualAsyncMethodName());

                throw new VehicleUnavailableException(exVehicleUnavailabe);
            }
        }

        public decimal CalculatePrice(Rental rental)
        {
            int minDayCount = 1;
            int daysCounts = (rental.DateTo - rental.DateFrom).Days;

            int days = daysCounts > 0 ? daysCounts : minDayCount;

            decimal price = days * rental.Vehicle.PricePerDay;

            return price;
        }
    }
}
