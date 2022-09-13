using CarRental.Domain.Models;

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
