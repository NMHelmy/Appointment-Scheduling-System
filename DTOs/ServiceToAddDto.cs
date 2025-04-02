using System.ComponentModel.DataAnnotations;

namespace AppointmentScheduling.DTOs
{
    public class ServiceToAddDto
    {
        [Required]
        public string? Name { get; set; }

        [Required]
        public string? Duration { get; set; } // Format: "hh:mm:ss"

        [Required]
        public decimal Price { get; set; }

        public string? Description { get; set; }
    }
}