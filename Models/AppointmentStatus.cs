using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace AppointmentScheduling.Models
{
    public class AppointmentStatus
    {
        [Key]
        public int AppointmentStatusId { get; set; }

        [ForeignKey("Appointment")]
        public int? AppointmentId { get; set; }  // FK to Appointment

        [Required(ErrorMessage = "Status name is required.")]
        [StringLength(50)]
        public string? Name { get; set; }  // e.g., "Booked", "Completed", "Cancelled"

        [Required]
        public string? Description { get; set; }

        // Navigation property 
        public Appointment? Appointment { get; set; }
    }
}
