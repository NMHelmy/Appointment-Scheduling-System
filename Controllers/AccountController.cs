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
    [Authorize] // Any authenticated user
    public class AccountController : ControllerBase
    {
        private readonly IRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            IRepository repository,
            IMapper mapper,
            ILogger<AccountController> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                var user = await _repository.GetByIdAsync<User>(userId.Value);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                // Don't return the password hash
                return Ok(new
                {
                    user.UserId,
                    user.FirstName,
                    user.LastName,
                    user.Email,
                    user.Role,
                    user.CreatedAt
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user profile");
                return StatusCode(500, new { message = "An error occurred while retrieving profile" });
            }
        }

        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProfile(ProfileUpdateDto profileDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                var user = await _repository.GetByIdAsync<User>(userId.Value);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                // Check if email is being changed and if it's already in use
                if (!string.IsNullOrEmpty(profileDto.Email) && profileDto.Email != user.Email)
                {
                    var emailExists = await _repository.GetQueryable<User>()
                        .AnyAsync(u => u.Email == profileDto.Email && u.UserId != userId);

                    if (emailExists)
                    {
                        return BadRequest(new { message = "Email already in use" });
                    }
                }

                // Update user properties if provided
                if (!string.IsNullOrEmpty(profileDto.FirstName))
                {
                    user.FirstName = profileDto.FirstName;
                }

                if (!string.IsNullOrEmpty(profileDto.LastName))
                {
                    user.LastName = profileDto.LastName;
                }

                if (!string.IsNullOrEmpty(profileDto.Email))
                {
                    user.Email = profileDto.Email;
                }

                if (!await _repository.SaveChangesAsync())
                {
                    return BadRequest(new { message = "Failed to update profile" });
                }

                return Ok(new
                {
                    user.UserId,
                    user.FirstName,
                    user.LastName,
                    user.Email,
                    user.Role,
                    user.CreatedAt
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user profile");
                return StatusCode(500, new { message = "An error occurred while updating profile" });
            }
        }

        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto passwordDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                var user = await _repository.GetByIdAsync<User>(userId.Value);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                // Verify current password
                if (!BCrypt.Net.BCrypt.Verify(passwordDto.CurrentPassword, user.PasswordHash))
                {
                    return BadRequest(new { message = "Current password is incorrect" });
                }

                // Update password
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(passwordDto.NewPassword);

                if (!await _repository.SaveChangesAsync())
                {
                    return BadRequest(new { message = "Failed to change password" });
                }

                return Ok(new { message = "Password changed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password");
                return StatusCode(500, new { message = "An error occurred while changing password" });
            }
        }

        [HttpGet("my-appointments")]
        public async Task<IActionResult> GetMyAppointments()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                var appointments = await _repository.GetQueryable<Appointment>()
                    .Where(a => a.UserId == userId)
                    .Include(a => a.Service)
                    .Include(a => a.StatusHistory)
                    .OrderByDescending(a => a.AppointmentDate)
                    .ToListAsync();

                return Ok(appointments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user appointments");
                return StatusCode(500, new { message = "An error occurred while retrieving appointments" });
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
