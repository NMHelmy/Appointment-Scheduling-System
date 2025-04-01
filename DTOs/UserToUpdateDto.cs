using System.ComponentModel.DataAnnotations;

namespace AppointmentScheduling.DTOs
{
    public class UserToUpdateDto
    {
        [Required]
        public int UserId { get; set; }

        [MaxLength(50)]
        public string? FirstName { get; set; }

        [MaxLength(50)]
        public string? LastName { get; set; }

        [EmailAddress, MaxLength(50)]
        public string? Email { get; set; }

        [MinLength(8)]
        public string? Password { get; set; } // Optional password update
    }
}
