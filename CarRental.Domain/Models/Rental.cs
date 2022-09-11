using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRental.Domain.Models
{
    public class Rental
    {
        public int RentalId { get; init; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public decimal Price { get; set; }
        
        public virtual Vehicle Vehicle { get; set; }
        public virtual Client Client { get; set; }
    }
}
