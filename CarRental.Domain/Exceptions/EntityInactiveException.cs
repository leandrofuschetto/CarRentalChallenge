using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRental.Domain.Exceptions
{
    public class ClientInactiveException : Exception
    {
        public string Code { get; set; } = "CLIENT_INACTIVE_EXCEPTION";
        public ClientInactiveException(string message) : base(message)
        { }
    }
}
