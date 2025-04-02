using System.ComponentModel.DataAnnotations;

namespace AppointmentScheduling.DTOs
{
    public class PaymentRequestDto
    {
        [Required(ErrorMessage = "Appointment ID is required")]
        public int AppointmentId { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, 100000, ErrorMessage = "Amount must be between $0.01 and $100,000")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Payment method is required")]
        [StringLength(20, ErrorMessage = "Payment method cannot exceed 20 characters")]
        public string PaymentMethod { get; set; } = string.Empty; // "CreditCard", "PayPal", "Cash"
        public string TransactionId { get; set; } = string.Empty;
    }
}
