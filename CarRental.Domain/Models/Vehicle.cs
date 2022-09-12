using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRental.Domain.Models
{
    public class Vehicle
    {
        public int Id { get; init; }
        public string Model { get; set; }
        public int PricePerDay { get; set; }
        public bool Active { get; set; }
    }
}
