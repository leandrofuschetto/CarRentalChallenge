using CarRental.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
