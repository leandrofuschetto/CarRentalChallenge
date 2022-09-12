using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarRental.Data.Entities
{
    public class VehicleEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; init; }

        [Required]
        [Column(TypeName = "VARCHAR(250)")]
        public string Model { get; set; }

        [Required]
        [Column(TypeName = "int")]
        public int PricePerDay { get; set; }

        [Required]
        [Column(TypeName = "BIT")]
        public bool Active { get; set; }
    }
}
