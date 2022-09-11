using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRental.Domain.Exceptions
{
    public class VehicleInactiveException : Exception
    {
        public string Code { get; set; } = "VEHICLE_INACTIVE_EXCEPTION";
        public VehicleInactiveException(string message) : base(message)
        { }
    }
}
