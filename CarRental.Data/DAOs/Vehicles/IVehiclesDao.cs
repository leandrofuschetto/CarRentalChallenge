using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRental.Data.DAOs.Vehicles
{
    public interface IVehiclesDao
    {
        Task<Domain.Models.Vehicle> GetVehicleByIdAsync(int id);
        Task<IEnumerable<Domain.Models.Vehicle>> GetAllVehiclesAsync(bool active);
        Task<Domain.Models.Vehicle> CreateVehicleAsync(Domain.Models.Vehicle vehicle);
        Task<bool> DeleteByIdAsync(Domain.Models.Vehicle vehicle);
        Task<bool> ModelExits(Domain.Models.Vehicle vehicle);
        Task<bool> VehicleActive(int vehicleId);
    }
}
