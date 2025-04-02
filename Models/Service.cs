using System.ComponentModel.DataAnnotations;

namespace AppointmentScheduling.Models
{
    public class Service
    {
        [Key]
        public int ServiceId { get; set; }

        [Required, MaxLength(100)]
        public string? Name { get; set; }

        [Required]
        public TimeSpan Duration { get; set; } // e.g., 00:30:00 for 30 mins

        [Required, Range(0, 1000)]
        public decimal Price { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        // Navigation property
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}