using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AppointmentScheduling.Data;
using AppointmentScheduling.DTOs;
using AppointmentScheduling.Models;
using AppointmentScheduling.Contstants;
using Microsoft.AspNetCore.Authorization;

namespace AppointmentScheduling.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IRepository _repository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IRepository repository,
            IConfiguration configuration,
            ILogger<AuthController> logger)
        {
            _repository = repository;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            try
            {
                // Check if email already exists
                var existingUser = await _repository.GetQueryable<User>()
                    .FirstOrDefaultAsync(u => u.Email == registerDto.Email);

                if (existingUser != null)
                {
                    _logger.LogWarning("Registration attempt with existing email: {Email}", registerDto.Email);
                    return BadRequest(new { message = "Email already registered" });
                }

                // Create new user
                var user = new User
                {
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    Email = registerDto.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                    Role = Roles.Client, 
                    CreatedAt = DateTime.UtcNow
                };

                _repository.AddEntity(user);

                if (!await _repository.SaveChangesAsync())
                {
                    return BadRequest(new { message = "Failed to register user" });
                }

                // Generate JWT token for auto-login
                var token = GenerateJwtToken(user);
                var tokenExpiration = DateTime.UtcNow.AddHours(1); // Token valid for 1 hour

                return Ok(new AuthResponseDto
                {
                    UserId = user.UserId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Role = user.Role,
                    Token = token,
                    TokenExpiration = tokenExpiration
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user registration");
                return StatusCode(500, new { message = "An error occurred during registration" });
            }
        }

        [HttpPost("register-admin")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> RegisterAdmin(RegisterDto registerDto)
        {
            try
            {
                // Validate email exists
                var existingUser = await _repository.GetQueryable<User>()
                    .FirstOrDefaultAsync(u => u.Email == registerDto.Email);

                if (existingUser != null)
                {
                    return BadRequest(new { message = "Email already registered" });
                }

                // Create new admin user
                var user = new User
                {
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    Email = registerDto.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                    Role = Roles.Admin, // Or parameterize if needed
                    CreatedAt = DateTime.UtcNow
                };

                _repository.AddEntity(user);
                await _repository.SaveChangesAsync();

                return Ok(new AuthResponseDto
                {
                    UserId = user.UserId,
                    Email = user.Email,
                    Role = user.Role,
                    Token = GenerateJwtToken(user)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Admin registration error");
                return StatusCode(500, new { message = "Admin registration failed" });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            try
            {
                // Find user by email
                var user = await _repository.GetQueryable<User>()
                    .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

                if (user == null)
                {
                    _logger.LogWarning("Login attempt with non-existent email: {Email}", loginDto.Email);
                    return Unauthorized(new { message = "Invalid credentials" });
                }

                // Verify password
                if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                {
                    _logger.LogWarning("Failed login attempt for user: {Email}", loginDto.Email);
                    return Unauthorized(new { message = "Invalid credentials" });
                }

                // Generate JWT token
                var token = GenerateJwtToken(user);
                var tokenExpiration = DateTime.UtcNow.AddHours(1); // Token valid for 1 hour

                _logger.LogInformation("User logged in: {Email}", user.Email);

                return Ok(new AuthResponseDto
                {
                    UserId = user.UserId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Role = user.Role,
                    Token = token,
                    TokenExpiration = tokenExpiration
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user login");
                return StatusCode(500, new { message = "An error occurred during login" });
            }
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];
            var issuer = jwtSettings["ValidIssuer"];
            var audience = jwtSettings["ValidAudience"];

            if (string.IsNullOrEmpty(secretKey))
            {
                throw new InvalidOperationException("JWT secret key is missing from configuration");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1), // Token valid for 1 hour
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}