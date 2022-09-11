using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRental.Domain.Models
{
    public class Client
    {
        public int ClientId { get; init; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public bool Active { get; set; }
    }
}
