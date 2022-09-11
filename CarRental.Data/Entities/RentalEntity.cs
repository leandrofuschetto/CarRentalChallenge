using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarRental.Data.Entities
{
    public class RentalEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RentalId { get; init; }

        public int VehicleId { get; set; }

        [ForeignKey("VehicleId")]
        public virtual VehicleEntity Vehicle { get; set; }

        public int ClientId { get; set; }

        [ForeignKey("ClientId")]
        public virtual ClientEntity Client { get; set; }

        [Required]
        [Column(TypeName = "DATETIME")]
        public DateTime DateFrom { get; set; }

        [Required]
        [Column(TypeName = "DATETIME")]
        public DateTime DateTo { get; set; }

        [Required]
        [Column(TypeName = "DECIMAL")]
        public decimal Price { get; set; }

        [Required]
        [Column(TypeName = "BIT")]
        public bool Active { get; set; }
    }
}
