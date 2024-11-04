using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.B2B.Data.Models
{
    [Table("users")]
    public class User
    {
        [Key]
        [Column("id")] // Map to lowercase "id" in PostgreSQL
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Column("username")] // Map to lowercase "username" in PostgreSQL
        public string Username { get; set; }

        [Required]
        [StringLength(100)]
        [Column("password")] // Map to lowercase "password" in PostgreSQL
        public string Password { get; set; }
    }
}
