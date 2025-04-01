using AutoMapper;
using AppointmentScheduling.DTOs;
using AppointmentScheduling.Models;

namespace AppointmentScheduling
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
        }
    }
}