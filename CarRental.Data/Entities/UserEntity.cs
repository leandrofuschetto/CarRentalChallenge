using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarRental.Data.Entities
{
    public class UserEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(20)")]
        public string Username { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(100)")]
        public string Password { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(100)")]
        public string Salt { get; set; }
    }
}
