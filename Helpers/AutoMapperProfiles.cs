using AutoMapper;
using servicer.API.Dtos;
using servicer.API.Models;

namespace servicer.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserForListDto>().ReverseMap();
            CreateMap<User, UserForRegisterDto>().ReverseMap();
            CreateMap<Ticket, TicketForSendDto>().ReverseMap();
            CreateMap<Ticket, TicketForListDto>().ReverseMap();
            CreateMap<Ticket, TicketForDetailDto>().ReverseMap();
        }
    }
}