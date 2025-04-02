using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using AppointmentScheduling.Data;
using AppointmentScheduling.Models;
using AppointmentScheduling.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace AppointmentScheduling.Controllers
{
    // Read Controller (All users)
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // All logged-in users can access
    public class AppointmentsReadOnlyController : Controller
    {
        private readonly IRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<AppointmentsReadOnlyController> _logger;

        public AppointmentsReadOnlyController(
            IRepository repository,
            IMapper mapper,
            ILogger<AppointmentsReadOnlyController> logger)
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
    }
}
