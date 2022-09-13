using CarRental.Domain.Models;

namespace CarRental.Service.Vehicles
{
    public interface IVehiclesService
    {
        Task<IEnumerable<Vehicle>> GetAllVehiclesAsync(bool active);
        Task<Vehicle> GetVehicleByIdAsync(int id);
        Task<Vehicle> CreateVehicleAsync(Vehicle vehicle);
        Task<bool> DeleteByIdAsync(int id);
    }
}
