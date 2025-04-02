using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AppointmentScheduling.Models
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }

        [Required(ErrorMessage = "Appointment ID is required.")]
        [ForeignKey("Appointment")]
        public int AppointmentId { get; set; }

        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, 100000, ErrorMessage = "Amount must be between $0.01 and $100,000.")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

        [Required]
        [Column(TypeName = "varchar(20)")]
        public string PaymentMethod { get; set; } = string.Empty; // "CreditCard", "PayPal", "Cash"

        [Required]
        [Column(TypeName = "varchar(20)")]
        public string Status { get; set; } = string.Empty; // "Pending", "Completed", "Refunded"

        [Required]
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        [StringLength(100)]
        public string TransactionId { get; set; } = string.Empty; // Null until payment processed

        // Navigation property
        public Appointment? Appointment { get; set; }
    }
}
