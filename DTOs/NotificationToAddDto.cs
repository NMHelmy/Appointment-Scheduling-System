using System.ComponentModel.DataAnnotations;

namespace AppointmentScheduling.DTOs
{
    public class NotificationToAddDto
    {
        [Required(ErrorMessage = "User ID is required")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Appointment ID is required")]
        public int AppointmentId { get; set; }

        [Required(ErrorMessage = "Message is required")]
        [StringLength(1000, ErrorMessage = "Message cannot exceed 1000 characters")]
        public string Message { get; set; } = string.Empty;

        [Required(ErrorMessage = "Notification type is required")]
        [StringLength(20, ErrorMessage = "Notification type cannot exceed 20 characters")]
        public string NotificationType { get; set; } = string.Empty; // "Email", "SMS", "Push"
    }
}
