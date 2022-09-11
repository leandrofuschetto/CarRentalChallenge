using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRental.Domain.Exceptions
{
    public class FieldMandatoryException : Exception
    {
        public string Code { get; set; }
        public FieldMandatoryException(string message, string ErrorCode) : base(message)
        {
            this.Code = ErrorCode;
        }
    }
}
