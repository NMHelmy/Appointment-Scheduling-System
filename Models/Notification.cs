using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AppointmentScheduling.Models
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }

        [Required(ErrorMessage = "User ID is required.")]
        [ForeignKey("User")]
        public int UserId { get; set; } 
        
        [Required(ErrorMessage = "Appointment ID is required.")]
        [ForeignKey("Appointment")]
        public int AppointmentId { get; set; }

        [Required(ErrorMessage = "Message content is required.")]
        [StringLength(1000, ErrorMessage = "Message cannot exceed 1000 characters.")]
        public string? Message { get; set; }

        [Required]
        [Column(TypeName = "varchar(20)")]
        public string? NotificationType { get; set; } // "Email", "SMS", "Push"

        [Required]
        [Column(TypeName = "varchar(20)")]
        public string? Status { get; set; } // "Pending", "Sent", "Failed"

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? SentAt { get; set; } // Null until sent

        // Navigation property
        public User? User { get; set; }
        public Appointment? Appointment { get; set; }
    }
}
