using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using AppointmentScheduling.Data;
using AppointmentScheduling.Models;
using AppointmentScheduling.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace AppointmentScheduling.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AdminsOnly")] // Only admins can access
    public class UserController : ControllerBase
    {
        private readonly IRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<UserController> _logger;

        public UserController(
            IRepository repository,
            IMapper mapper,
            ILogger<UserController> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        // Get all users
        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _repository.GetAllAsync<User>();
            return Ok(users);
        }

        // Get user by ID
        [HttpGet("GetUser/{userId}")]
        public async Task<IActionResult> GetUser(int userId)
        {
            var user = await _repository.GetByIdAsync<User>(userId);
            return user == null
                ? NotFound("User not found.")
                : Ok(user);
        }

        // Add new user
        [HttpPost("AddUser")]
        public async Task<IActionResult> AddUser(UserToAddDto userDto)
        {
            // Check if email already exists
            var existingUser = await _repository.GetQueryable<User>()
                .FirstOrDefaultAsync(u => u.Email == userDto.Email);

            if (existingUser != null)
            {
                _logger.LogWarning("Email {Email} already exists", userDto.Email);
                return BadRequest("Email already registered.");
            }

            var user = _mapper.Map<User>(userDto);
            user.CreatedAt = DateTime.UtcNow;
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password); // Secure password hashing

            _repository.AddEntity(user);
            return await _repository.SaveChangesAsync()
                ? Ok(user)
                : BadRequest("Failed to add user.");
        }

        // Add staff

        // Update user
        [HttpPut("UpdateUser")]
        public async Task<IActionResult> UpdateUser(UserToUpdateDto userDto)
        {
            var user = await _repository.GetByIdAsync<User>(userDto.UserId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            _mapper.Map(userDto, user);

            if (!string.IsNullOrEmpty(userDto.Password))
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password);
            }

            return await _repository.SaveChangesAsync()
                ? Ok(user)
                : BadRequest("Failed to update user.");
        }

        // Delete user
        [HttpDelete("DeleteUser/{userId}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var user = await _repository.GetByIdAsync<User>(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Check if user has appointments
            var hasAppointments = await _repository.GetQueryable<Appointment>()
                .AnyAsync(a => a.UserId == userId);

            if (hasAppointments)
            {
                return BadRequest("Cannot delete user with existing appointments.");
            }

            _repository.RemoveEntity(user);
            return await _repository.SaveChangesAsync()
                ? Ok("User deleted.")
                : BadRequest("Failed to delete user.");
        }
    }
}