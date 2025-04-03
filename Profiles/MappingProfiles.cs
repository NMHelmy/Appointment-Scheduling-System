using AutoMapper;
using AppointmentScheduling.DTOs;
using AppointmentScheduling.Models;

namespace AppointmentScheduling.Profiles
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            // Appointment mappings
            CreateMap<AppointmentToAddDto, Appointment>();
            CreateMap<AppointmentToUpdateDto, Appointment>();

            // User mappings
            CreateMap<UserToAddDto, User>();
            CreateMap<UserToUpdateDto, User>();
            
            // Service mappings
            CreateMap<ServiceToAddDto, Service>();

            // Review mappings
            CreateMap<ReviewResponseDto, Review>();
            CreateMap<ReviewToAddDto, Review>();

            // Notification mappings
            CreateMap<NotificationDto, Notification>();
            CreateMap<NotificationResponseDto, Notification>();
            CreateMap<NotificationToAddDto, Notification>();
            CreateMap<NotificationToUpdateDto, Notification>();

            // Payment mappings
            CreateMap<PaymentRequestDto, Payment>();
            CreateMap<PaymentResponseDto, Payment>();
        }
    }
}