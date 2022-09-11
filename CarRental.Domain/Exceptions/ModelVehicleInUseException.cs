using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRental.Domain.Exceptions
{
    public class ModelVehicleInUseException : Exception
    {
        public string Code { get; set; } = "MODEL_UNIQUE_ERROR";
        public ModelVehicleInUseException(string message) : base(message)
        { }
    }
}
