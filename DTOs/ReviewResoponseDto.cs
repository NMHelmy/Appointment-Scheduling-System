using System.ComponentModel.DataAnnotations;
using AppointmentScheduling.DTOs;

namespace AppointmentScheduling.DTOs
{
    public class ReviewResponseDto
    {
        public int ReviewId { get; set; }
        public int AppointmentId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime ReviewDate { get; set; }
        public UserBriefDto User { get; set; } = new();
    }
}
