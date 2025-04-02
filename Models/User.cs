using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AppointmentScheduling.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Ensures UserId auto-increments
        public int UserId { get; set; }
        [Required, MaxLength(50)]
        public string FirstName { get; set; } = "";
        [Required, MaxLength(50)]
        public string LastName { get; set; } = "";
        [Required, EmailAddress, MaxLength(50)]
        public string Email { get; set; } = "";
        [Required]
        public string PasswordHash { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Required]
        public string Role { get; set; } = "Client"; // Default to client

        // Navigation properties
        [JsonIgnore] // Prevent circular reference issues
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public List<Review> Reviews { get; set; } = new();
        [InverseProperty("User")]
        public List<Notification> Notifications { get; set; } = new();
    }
}
