using AutoMapper;
using EventManagement.EventService.Models;

namespace EventManagement.EventService.Services
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Map from domain model to DTO
            CreateMap<Event, EventDto>();

            // Map from CreateEventDto to domain model
            CreateMap<CreateEventDto, Event>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.Registered, opt => opt.MapFrom(src => 0));

            // Map from UpdateEventDto to domain model
            CreateMap<UpdateEventDto, Event>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Registered, opt => opt.Ignore());
        }
    }
} 