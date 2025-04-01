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
            // Get the existing user WITHOUT creating a new one
            var user = await _repository.GetQueryable<User>()
                .FirstOrDefaultAsync(u => u.UserId == appointmentDto.UserId);

            if (user == null)
            {
                return NotFound($"User with ID {appointmentDto.UserId} not found.");
            }

            var appointment = new Appointment
            {
                Title = appointmentDto.Title,
                Description = appointmentDto.Description,
                AppointmentDate = appointmentDto.AppointmentDate,
                UserId = user.UserId,  // Use the existing user's ID
                User = user,           // Attach the entire user object
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

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