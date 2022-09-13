using CarRental.Domain.Models;

namespace CarRental.Data.DAOs.Vehicles
{
    public interface IVehiclesDao
    {
        Task<Vehicle> GetVehicleByIdAsync(int id);
        Task<IEnumerable<Vehicle>> GetAllVehiclesAsync(bool active);
        Task<Vehicle> CreateVehicleAsync(Vehicle vehicle);
        Task<bool> DeleteByIdAsync(Vehicle vehicle);
        Task<bool> ModelExits(Vehicle vehicle);
        Task<bool> VehicleActive(int vehicleId);
    }
}
