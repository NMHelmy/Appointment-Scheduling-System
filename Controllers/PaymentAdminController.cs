// Controllers/PaymentAdminController.cs
using AppointmentScheduling.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AppointmentScheduling.Data;
using AppointmentScheduling.Models;
using AppointmentScheduling.Contstants;

namespace AppointmentScheduling.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Staff}")]
    public class PaymentAdminController : ControllerBase
    {
        private readonly IRepository _repository;

        public PaymentAdminController(IRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        public async Task<ActionResult<PaymentResponseDto>> CreatePayment(PaymentRequestDto paymentRequestDto)
        {
            var payment = new Payment
            {
                AppointmentId = paymentRequestDto.AppointmentId,
                Amount = paymentRequestDto.Amount,
                PaymentMethod = paymentRequestDto.PaymentMethod,
                Status = "Pending",
                PaymentDate = DateTime.UtcNow,
                TransactionId = paymentRequestDto.TransactionId
            };

            _repository.AddEntity(payment);
            await _repository.SaveChangesAsync();

            var responseDto = new PaymentResponseDto
            {
                PaymentId = payment.PaymentId,
                AppointmentId = payment.AppointmentId,
                Amount = payment.Amount,
                PaymentMethod = payment.PaymentMethod,
                Status = payment.Status,
                PaymentDate = payment.PaymentDate,
                TransactionId = payment.TransactionId
            };

            return CreatedAtAction(nameof(GetPayment), new { paymentId = payment.PaymentId }, responseDto);
        }

        [HttpPut("{paymentId}")]
        public async Task<IActionResult> UpdatePayment(int paymentId, PaymentRequestDto paymentRequestDto)
        {
            var payment = await _repository.GetByIdAsync<Payment>(paymentId);
            if (payment == null) return NotFound();

            payment.Amount = paymentRequestDto.Amount;
            payment.PaymentMethod = paymentRequestDto.PaymentMethod;
            payment.TransactionId = paymentRequestDto.TransactionId;

            _repository.AddEntity(payment); // For update in non-generic repository
            await _repository.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{paymentId}")]
        public async Task<IActionResult> DeletePayment(int paymentId)
        {
            var payment = await _repository.GetByIdAsync<Payment>(paymentId);
            if (payment == null) return NotFound();

            _repository.RemoveEntity(payment);
            await _repository.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("{paymentId}")]
        public async Task<ActionResult<PaymentResponseDto>> GetPayment(int paymentId)
        {
            var payment = await _repository.GetByIdAsync<Payment>(paymentId);
            if (payment == null) return NotFound();

            return Ok(new PaymentResponseDto
            {
                PaymentId = payment.PaymentId,
                AppointmentId = payment.AppointmentId,
                Amount = payment.Amount,
                PaymentMethod = payment.PaymentMethod,
                Status = payment.Status,
                PaymentDate = payment.PaymentDate,
                TransactionId = payment.TransactionId
            });
        }
    }
}