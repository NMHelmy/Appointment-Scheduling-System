using System.ComponentModel.DataAnnotations;

namespace AppointmentScheduling.DTOs
{
    public class AppointmentToUpdateDto
    {
        [Required]
        public int AppointmentId { get; set; }

        [Required]
        public string? Title { get; set; }

        public string? Description { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }
    }
}
