using System.ComponentModel.DataAnnotations;

namespace AppointmentScheduling.DTOs
{
    public class NotificationToUpdateDto
    {
        [Required(ErrorMessage = "Notification ID is required")]
        public int NotificationId { get; set; }

        [StringLength(20, ErrorMessage = "Status cannot exceed 20 characters")]
        public string? Status { get; set; } // "Pending", "Sent", "Failed"

        public DateTime? SentAt { get; set; }
    }
}
