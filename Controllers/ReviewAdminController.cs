// Controllers/ReviewAdminController.cs
using AppointmentScheduling.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AppointmentScheduling.Data;
using AppointmentScheduling.Models;
using System.Security.Claims;
using AppointmentScheduling.Contstants;

namespace AppointmentScheduling.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Roles.Client)]
    public class ReviewAdminController : ControllerBase
    {
        private readonly IRepository _repository;

        public ReviewAdminController(IRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        public async Task<ActionResult<ReviewResponseDto>> CreateReview(ReviewToAddDto reviewToAddDto)
        {
            // Verify the appointment exists
            var appointment = await _repository.GetByIdAsync<Appointment>(reviewToAddDto.AppointmentId);
            if (appointment == null) return BadRequest("Appointment not found");

            // Get current user ID from claims with proper null checking
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("Invalid user identification");
            }

            var review = new Review
            {
                AppointmentId = reviewToAddDto.AppointmentId,
                UserId = userId,
                Rating = reviewToAddDto.Rating,
                Comment = reviewToAddDto.Comment,
                ReviewDate = DateTime.UtcNow
            };

            _repository.AddEntity(review);
            await _repository.SaveChangesAsync();

            // Get the user's name for the response
            var user = await _repository.GetByIdAsync<User>(userId);

            var responseDto = new ReviewResponseDto
            {
                ReviewId = review.ReviewId,
                AppointmentId = review.AppointmentId,
                Rating = review.Rating,
                Comment = review.Comment,
                ReviewDate = review.ReviewDate
            };

            return CreatedAtAction(nameof(GetReview), new { reviewId = review.ReviewId }, responseDto);
        }

        [HttpDelete("{reviewId}")]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Staff}")]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            var review = await _repository.GetByIdAsync<Review>(reviewId);
            if (review == null) return NotFound();

            _repository.RemoveEntity(review);
            await _repository.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("{reviewId}")]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Staff}")]
        public async Task<ActionResult<ReviewResponseDto>> GetReview(int reviewId)
        {
            var review = await _repository.GetByIdAsync<Review>(reviewId);
            if (review == null) return NotFound();

            return Ok(new ReviewResponseDto
            {
                ReviewId = review.ReviewId,
                AppointmentId = review.AppointmentId,
                Rating = review.Rating,
                Comment = review.Comment,
                ReviewDate = review.ReviewDate
            });
        }
    }
}