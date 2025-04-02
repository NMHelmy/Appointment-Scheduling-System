using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using AppointmentScheduling.Data;
using AppointmentScheduling.Models;
using AppointmentScheduling.DTOs;
using Microsoft.EntityFrameworkCore;

namespace AppointmentScheduling.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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

        // Get all appointments
        [HttpGet("GetAppointments")]
        public async Task<IActionResult> GetAppointments()
        {
            var appointments = await _repository.GetAllAsync<Appointment>();
            return Ok(appointments);
        }

        // Get appointment by ID
        [HttpGet("GetAppointment/{appointmentId}")]
        public async Task<IActionResult> GetAppointment(int appointmentId)
        {
            var appointment = await _repository.GetByIdAsync<Appointment>(appointmentId);
            return appointment == null
                ? NotFound("Appointment not found.")
                : Ok(appointment);
        }

        // Add new appointment
        [HttpPost("AddAppointment")]
        public async Task<IActionResult> AddAppointment(AppointmentToAddDto appointmentDto)
        {
            // Validate user exists
            var user = await _repository.GetByIdAsync<User>(appointmentDto.UserId);
            if (user == null) return NotFound("User not found");

            // Create appointment with only scalar properties
            var appointment = new Appointment
            {
                Title = appointmentDto.Title,
                Description = appointmentDto.Description,
                AppointmentDate = appointmentDto.AppointmentDate,
                UserId = user.UserId,  // Set only the foreign key
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

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
            await _repository.SaveChangesAsync();

            return Ok(appointment);
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