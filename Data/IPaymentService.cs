using AppointmentScheduling.Models;

namespace AppointmentScheduling.Data
{
    public interface IPaymentService
    {
        Task<Payment> ProcessPaymentAsync(int appointmentId, string paymentMethod, string? paymentToken);
        Task<Payment?> GetPaymentByIdAsync(int paymentId);
        Task<IEnumerable<Payment>> GetPaymentsByAppointmentAsync(int appointmentId);
        Task<IEnumerable<Payment>> GetPaymentsByUserAsync(int userId);
    }
}
