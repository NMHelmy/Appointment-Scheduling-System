using System.ComponentModel.DataAnnotations;

namespace AppointmentScheduling.DTOs
{
    public class AppointmentToAddDto
    {
        [Required]
        public string? Title { get; set; }

        public string? Description { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required]
        public int UserId { get; set; }
    }
}
