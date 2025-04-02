using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using AppointmentScheduling.Data;
using AppointmentScheduling.Models;
using AppointmentScheduling.DTOs;

namespace AppointmentScheduling.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly IRepository _repository;
        private readonly IMapper _mapper;

        public ServiceController(IRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet("GetServices")]
        public async Task<IActionResult> GetServices()
        {
            var services = await _repository.GetAllAsync<Service>();
            return Ok(services);
        }

        [HttpPost("AddService")]
        public async Task<IActionResult> AddService(ServiceToAddDto serviceDto)
        {
            if (!TimeSpan.TryParse(serviceDto.Duration, out var duration))
            {
                return BadRequest("Invalid duration format. Use hh:mm:ss");
            }

            var service = _mapper.Map<Service>(serviceDto);
            service.Duration = duration;

            _repository.AddEntity(service);
            await _repository.SaveChangesAsync();

            return Ok(service);
        }

        [HttpPut("UpdateService")]
        public async Task<IActionResult> UpdateService(ServiceToUpdateDto serviceDto)
        {
            var service = await _repository.GetByIdAsync<Service>(serviceDto.ServiceId);
            if (service == null) return NotFound();

            _mapper.Map(serviceDto, service);
            if (!string.IsNullOrEmpty(serviceDto.Duration))
                service.Duration = TimeSpan.Parse(serviceDto.Duration);

            await _repository.SaveChangesAsync();
            return Ok(service);
        }
    }
}