// Controllers/ReviewReadOnlyController.cs
using AppointmentScheduling.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AppointmentScheduling.Data;
using AppointmentScheduling.Models;

namespace AppointmentScheduling.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReviewReadOnlyController : ControllerBase
    {
        private readonly IRepository _repository;

        public ReviewReadOnlyController(IRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReviewResponseDto>>> GetReviews()
        {
            var reviews = await _repository.GetAllAsync<Review>();
            var reviewDtos = reviews.Select(r => new ReviewResponseDto
            {
                ReviewId = r.ReviewId,
                AppointmentId = r.AppointmentId,
                Rating = r.Rating,
                Comment = r.Comment,
                ReviewDate = r.ReviewDate
            });
            return Ok(reviewDtos);
        }

        [HttpGet("{reviewId}")]
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