using System.ComponentModel.DataAnnotations;

namespace AppointmentScheduling.DTOs
{
    public class AppointmentToAddDto
    {
        [Required(ErrorMessage = "Appointment title is required.")]
        public string? Title { get; set; }

        public string? Description { get; set; }

        [Required(ErrorMessage = "Appointment date/time is required.")]
        public DateTime AppointmentDate { get; set; }

        [Required(ErrorMessage = "User ID is required.")]
        public int UserId { get; set; }
        public int? ServiceId { get; set; }
    }
}
