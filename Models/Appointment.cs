using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AppointmentScheduling.Models
{
    public class Appointment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AppointmentId { get; set; }
        [Required, MaxLength(100)]
        public string? Title { get; set; }
        [MaxLength(500)]
        public string? Description { get; set; }
        [Required]
        public DateTime AppointmentDate { get; set; }
        public bool ReminderSent { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Foreign key to Users
        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        // Navigation properties
        [JsonIgnore] // Prevent serialization loops
        public User? User { get; set; }

        [ForeignKey("Service")]
        public int? ServiceId { get; set; }
        [JsonIgnore]
        public Service? Service { get; set; }
        public List<AppointmentStatus> StatusHistory { get; set; } = new();
        public Review? Review { get; set; }
        [InverseProperty("Appointment")]
        public List<Notification> Notifications { get; set; } = new();

        [InverseProperty("Appointment")]
        public List<Payment> Payments { get; set; } = new();
    }
}
