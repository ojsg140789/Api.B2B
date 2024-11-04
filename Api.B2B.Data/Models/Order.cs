using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.B2B.Data.Models
{
    [Table("orders")]
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [Column("updatedat")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        [Column("deletedat")]
        public DateTime? DeletedAt { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [Column("status")]
        [StringLength(50)]
        public string Status { get; set; }

        [Required]
        [Column("price", TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        [Required]
        [Column("loanid")]
        public int LoanId { get; set; }

        [Required]
        public int MerchantId { get; set; }

        public string Products { get; set; }

        [Required]
        [Column("branchid")]
        public int BranchId { get; set; }

        [Required]
        public int SellsAgentId { get; set; }
    }
}
