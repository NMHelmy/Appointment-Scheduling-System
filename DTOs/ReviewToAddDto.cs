using System.ComponentModel.DataAnnotations;

namespace AppointmentScheduling.DTOs
{
    public class ReviewToAddDto
    {
        [Required]
        public int AppointmentId { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [StringLength(500)]
        public string? Comment { get; set; }
    }
}
