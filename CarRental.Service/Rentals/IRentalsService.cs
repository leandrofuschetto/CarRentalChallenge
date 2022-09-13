using CarRental.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRental.Service.Rentals
{
    public interface IRentalsService
    {
        Task<IEnumerable<Rental>> GetAllRentalsAsync(bool active);
        Task<Rental> GetRentalByIdAsync(int id);
        Task<Rental> CreateRentalAsync(Rental rental);
        Task<bool> DeleteByIdAsync(int id);
        decimal CalculatePrice(Rental rental);
    }
}
