using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using AppointmentScheduling.Data;
using AppointmentScheduling.Models;
using AppointmentScheduling.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace AppointmentScheduling.Controllers
{
    // Write Controller (Staff/Admin only)
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AdminsOnly")] // Only Admins can modify appointments
    public class AppointmentController : ControllerBase
    {
        private readonly IRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<AppointmentController> _logger;

        public AppointmentController(
            IRepository repository,
            IMapper mapper,
            ILogger<AppointmentController> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        // Add new appointment
        [HttpPost("AddAppointment")]
        public async Task<IActionResult> AddAppointment(AppointmentToAddDto appointmentDto)
        {
            bool hasTimeConflict = await _repository.AnyAsync<Appointment>(a =>
            a.UserId == appointmentDto.UserId &&
            a.AppointmentDate < appointmentDto.AppointmentDate.AddHours(1) &&  // Adjust buffer as needed
            a.AppointmentDate.AddHours(1) > appointmentDto.AppointmentDate);   // 1-hour minimum gap

            if (hasTimeConflict)
            {
                return Conflict(new ProblemDetails
                {
                    Title = "Time Conflict",
                    Detail = "User already has an appointment within the selected time window.",
                    Status = StatusCodes.Status409Conflict
                });
            }

            // Validate user exists
            var user = await _repository.GetByIdAsync<User>(appointmentDto.UserId);
            if (user == null) return NotFound("User not found");

            var appointment = _mapper.Map<Appointment>(appointmentDto);
            appointment.CreatedAt = DateTime.UtcNow;
            appointment.UpdatedAt = DateTime.UtcNow;

            // Handle ServiceId if provided
            if (appointmentDto.ServiceId.HasValue)
            {
                // Verify service exists without loading the entire entity
                var serviceExists = await _repository.GetQueryable<Service>()
                    .AnyAsync(s => s.ServiceId == appointmentDto.ServiceId.Value);

                if (!serviceExists) return NotFound($"Service with ID {appointmentDto.ServiceId} not found");

                // Set only the foreign key, not the navigation property
                appointment.ServiceId = appointmentDto.ServiceId.Value;
            }

            _repository.AddEntity(appointment);
            return await _repository.SaveChangesAsync()
                ? Ok(user)
                : BadRequest("Failed to add user.");
        }

        // Update appointment
        [HttpPut("UpdateAppointment")]
        public async Task<IActionResult> UpdateAppointment(AppointmentToUpdateDto appointmentDto)
        {
            var appointment = await _repository.GetByIdAsync<Appointment>(appointmentDto.AppointmentId);
            if (appointment == null)
            {
                return NotFound("Appointment not found.");
            }

            _mapper.Map(appointmentDto, appointment);
            appointment.UpdatedAt = DateTime.UtcNow;

            return await _repository.SaveChangesAsync()
                ? Ok(appointment)
                : BadRequest("Failed to update appointment.");
        }

        // Delete appointment
        [HttpDelete("DeleteAppointment/{appointmentId}")]
        public async Task<IActionResult> DeleteAppointment(int appointmentId)
        {
            var appointment = await _repository.GetByIdAsync<Appointment>(appointmentId);
            if (appointment == null)
            {
                return NotFound("Appointment not found.");
            }

            _repository.RemoveEntity(appointment);
            return await _repository.SaveChangesAsync()
                ? Ok("Appointment deleted.")
                : BadRequest("Failed to delete appointment.");
        }
    }
}