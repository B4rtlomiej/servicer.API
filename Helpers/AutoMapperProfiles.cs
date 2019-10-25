using AutoMapper;
using servicer.API.Dtos;
using servicer.API.Models;

namespace servicer.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserForListDto>()
                .ForMember(dest => dest.UserRole, opt =>
                {
                    opt.MapFrom(src => src.UserRole.ToString());
                });
            CreateMap<User, UserForDetailDto>()
                .ForMember(dest => dest.UserRole, opt =>
                {
                    opt.MapFrom(src => src.UserRole.ToString());
                });
            CreateMap<UserForRegisterDto, User>();
            CreateMap<Ticket, TicketForListDto>();
        }
    }
}