using CarRental.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRental.Data.DAOs.Rentals
{
    public interface IRentalsDao
    {
        Task<IEnumerable<Rental>> GetAllRentalsAsync(bool active);
        Task<Rental> GetRentalByIdAsync(int id);
        Task<Rental> CreateRentalAsync(Rental rental);
        Task<bool> DeleteByIdAsync(Rental rental);
        Task<bool> VehicleAvailable(Rental rental);
    }
}
