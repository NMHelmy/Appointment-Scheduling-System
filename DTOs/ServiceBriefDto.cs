namespace AppointmentScheduling.DTOs
{
    public class ServiceBriefDto
    {
        public int ServiceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
