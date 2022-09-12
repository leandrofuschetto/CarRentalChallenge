using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarRental.Data.Entities
{
    public class ClientEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; init; }

        [Required]
        [Column(TypeName = "VARCHAR(50)")]
        public string Fullname { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(100)")]
        public string Email { get; set; }

        [Required]
        [Column(TypeName = "BIT")]
        public bool Active { get; set; }
    }
}
