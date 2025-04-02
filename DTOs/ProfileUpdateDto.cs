using System.ComponentModel.DataAnnotations;

namespace AppointmentScheduling.DTOs
{
    public class ProfileUpdateDto
    {
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
        public string? FirstName { get; set; }

        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
        public string? LastName { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(50, ErrorMessage = "Email cannot exceed 50 characters")]
        public string? Email { get; set; }
    }
}
