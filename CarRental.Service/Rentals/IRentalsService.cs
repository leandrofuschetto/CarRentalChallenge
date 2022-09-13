using CarRental.Domain.Models;

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
