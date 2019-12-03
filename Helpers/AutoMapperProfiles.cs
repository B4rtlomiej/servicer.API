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
            CreateMap<User, UserForDetailDto>().ReverseMap();
            CreateMap<User, UserForRegisterDto>().ReverseMap();
            CreateMap<User, UserForUpdateDto>().ReverseMap();
            CreateMap<Ticket, TicketForSendDto>().ReverseMap();
            CreateMap<Ticket, TicketForListDto>().ReverseMap();
            CreateMap<Ticket, TicketForDetailDto>().ReverseMap();
            CreateMap<Ticket, TicketForUpdateDto>().ReverseMap();
            CreateMap<ProductSpecification, ProductSpecificationForListDto>().ReverseMap();
            CreateMap<ProductSpecification, ProductSpecificationForDetailDto>().ReverseMap();
            CreateMap<ProductSpecification, ProductSpecificationForSendDto>().ReverseMap();
            CreateMap<ProductSpecification, ProductSpecificationForUpdateDto>().ReverseMap();
            CreateMap<Note, NoteForListDto>().ReverseMap();
            CreateMap<Note, NoteForDetailDto>().ReverseMap();
            CreateMap<Note, NoteForSendDto>().ReverseMap();
            CreateMap<Note, NoteForUpdateDto>().ReverseMap();
            CreateMap<Person, PersonForDetailDto>().ReverseMap();
            CreateMap<Person, PersonForListDto>().ReverseMap();
            CreateMap<Person, PersonForUpdateDto>().ReverseMap();
        }
    }
}