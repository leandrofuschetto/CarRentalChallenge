﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRental.Domain.Exceptions
{
    public class VehicleUnavailableException : Exception
    {
        public string Code { get; set; } = "VEHICLE_UNAVAILABLE_EXCEPTION";
        public VehicleUnavailableException(string message) : base(message)
        { }
    }
}