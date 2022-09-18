﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarRental.Data.Entities
{
    public class RentalEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; init; }

        [Required]
        [Column(TypeName = "DATE")]
        public DateOnly DateFrom { get; set; }

        [Required]
        [Column(TypeName = "DATE")]
        public DateOnly DateTo { get; set; }

        [Required]
        [Column(TypeName = "DECIMAL")]
        public decimal Price { get; set; }

        [Required]
        [Column(TypeName = "BIT")]
        public bool Active { get; set; }

        public virtual ClientEntity Client { get; set; }
        public virtual VehicleEntity Vehicle { get; set; }
    }
}
