// Controllers/PaymentReadOnlyController.cs
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
    [Authorize(Roles = Roles.Client)]
    public class PaymentReadOnlyController : ControllerBase
    {
        private readonly IRepository _repository;

        public PaymentReadOnlyController(IRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaymentResponseDto>>> GetPayments()
        {
            var payments = await _repository.GetAllAsync<Payment>();
            var paymentDtos = payments.Select(p => new PaymentResponseDto
            {
                PaymentId = p.PaymentId,
                AppointmentId = p.AppointmentId,
                Amount = p.Amount,
                PaymentMethod = p.PaymentMethod,
                Status = p.Status,
                PaymentDate = p.PaymentDate,
                TransactionId = p.TransactionId
            });
            return Ok(paymentDtos);
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