using System.ComponentModel.DataAnnotations;

namespace AppointmentScheduling.DTOs
{
    public class ServiceToAddDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Duration { get; set; } = string.Empty;// Format: "hh:mm:ss"

        [Required]
        public decimal Price { get; set; }

        public string? Description { get; set; }
    }
}