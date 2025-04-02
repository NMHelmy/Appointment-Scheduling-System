using System.ComponentModel.DataAnnotations;

namespace AppointmentScheduling.DTOs
{
    public class UserToAddDto
    {
        [Required, MaxLength(50)]
        public string? FirstName { get; set; }

        [Required, MaxLength(50)]
        public string? LastName { get; set; }

        [Required, EmailAddress, MaxLength(50)]
        public string? Email { get; set; }

        [Required, MinLength(8)]
        public string? Password { get; set; }
        public string Role { get; set; } = "Client"; // Default to client
    }
}
