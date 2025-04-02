using AppointmentScheduling.Models;

namespace AppointmentScheduling.Data
{
    public interface INotificationService
    {
        Task SendAppointmentReminderAsync(Appointment appointment);
        Task SendAppointmentConfirmationAsync(Appointment appointment);
        Task SendAppointmentCancellationAsync(Appointment appointment);
        Task<bool> MarkNotificationAsSentAsync(int notificationId);
        Task<IEnumerable<Notification>> GetPendingNotificationsAsync();
    }
}
