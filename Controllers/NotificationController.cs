using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using AppointmentScheduling.Data;
using AppointmentScheduling.DTOs;
using AppointmentScheduling.Models;
using AutoMapper;

namespace AppointmentScheduling.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly IRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<NotificationController> _logger;
        // Add email service interface when implemented
        // private readonly IEmailService _emailService;

        public NotificationController(
            IRepository repository,
            IMapper mapper,
            ILogger<NotificationController> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet("my-notifications")]
        public async Task<IActionResult> GetMyNotifications()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                var notifications = await _repository.GetQueryable<Notification>()
                    .Where(n => n.UserId == userId)
                    .OrderByDescending(n => n.CreatedAt)
                    .ToListAsync();

                return Ok(notifications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user notifications");
                return StatusCode(500, new { message = "An error occurred while retrieving notifications" });
            }
        }

        [HttpGet("mark-as-read/{notificationId}")]
        public async Task<IActionResult> MarkAsRead(int notificationId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                var notification = await _repository.GetQueryable<Notification>()
                    .FirstOrDefaultAsync(n => n.NotificationId == notificationId && n.UserId == userId);

                if (notification == null)
                {
                    return NotFound(new { message = "Notification not found" });
                }

                notification.Status = "Read";

                if (!await _repository.SaveChangesAsync())
                {
                    return BadRequest(new { message = "Failed to update notification status" });
                }

                return Ok(new { message = "Notification marked as read" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking notification as read");
                return StatusCode(500, new { message = "An error occurred while updating notification" });
            }
        }

        // Admin/Staff only endpoints
        [HttpPost("send")]
        [Authorize(Policy = "AdminsOnly")]
        public async Task<IActionResult> SendNotification(NotificationToAddDto notificationDto)
        {
            try
            {
                // Verify user exists
                var user = await _repository.GetByIdAsync<User>(notificationDto.UserId);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                // Verify appointment exists
                var appointment = await _repository.GetByIdAsync<Appointment>(notificationDto.AppointmentId);
                if (appointment == null)
                {
                    return NotFound(new { message = "Appointment not found" });
                }

                var notification = new Notification
                {
                    UserId = notificationDto.UserId,
                    AppointmentId = notificationDto.AppointmentId,
                    Message = notificationDto.Message,
                    NotificationType = notificationDto.NotificationType,
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow
                };

                _repository.AddEntity(notification);

                if (!await _repository.SaveChangesAsync())
                {
                    return BadRequest(new { message = "Failed to create notification" });
                }

                // TODO: Actually send the notification via email/SMS/push
                // This would typically call a service to handle the actual sending
                // based on the NotificationType

                // For now, we'll just mark it as sent
                notification.Status = "Sent";
                notification.SentAt = DateTime.UtcNow;

                await _repository.SaveChangesAsync();

                return Ok(notification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending notification");
                return StatusCode(500, new { message = "An error occurred while sending notification" });
            }
        }

        [HttpGet("all")]
        [Authorize(Policy = "AdminsOnly")]
        public async Task<IActionResult> GetAllNotifications()
        {
            try
            {
                var notifications = await _repository.GetQueryable<Notification>()
                    .Include(n => n.User)
                    .Include(n => n.Appointment)
                    .OrderByDescending(n => n.CreatedAt)
                    .ToListAsync();

                return Ok(notifications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all notifications");
                return StatusCode(500, new { message = "An error occurred while retrieving notifications" });
            }
        }

        // Helper method to get the current user's ID from claims
        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return null;
            }
            return userId;
        }
    }
}