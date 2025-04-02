using System.ComponentModel.DataAnnotations;

namespace AppointmentScheduling.DTOs
{
    public class ServiceToUpdateDto
    {
        [Required]
        public int ServiceId { get; set; }

        public string? Name { get; set; }
        public string? Duration { get; set; }
        public decimal? Price { get; set; }
        public string? Description { get; set; }
    }
}
