using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using servicer.API.Data;
using servicer.API.Dtos;
using servicer.API.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace servicer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketsController : ControllerBase
    {
        private readonly IServicerRepository _repository;
        private readonly IMapper _mapper;

        private readonly DataContext _context;

        public TicketsController(DataContext context, IServicerRepository repository, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _repository = repository;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> SendTicket(TicketForSendDto ticketForDto)
        {
            var ticketToSend = new Ticket
            {
                Origin = ticketForDto.Origin,
                Type = ticketForDto.Type,
                Status = ticketForDto.Status,
                Priority = ticketForDto.Priority,
                Created = ticketForDto.Created,
                Closed = ticketForDto.Closed,
                Subject = ticketForDto.Subject,
                Description = ticketForDto.Description,
                ItemId = ticketForDto.ItemId
            };

            _context.Tickets.Add(ticketToSend);
            await _context.SaveChangesAsync();
            return Created("ticket", ticketToSend);
        }
        [HttpGet]
        public async Task<IActionResult> GetTickets()
        {
            var tickets = await _repository.GetTickets();
            var ticketsToReturn = _mapper.Map<IEnumerable<TicketForListDto>>(tickets);
            return Ok(ticketsToReturn);
        }
       [HttpGet("{id}")]
        public async Task<IActionResult> GetTicket(int id)
        {
            var tickets = await _repository.GetTicket(id);
            var ticketsToReturn = _mapper.Map<TicketForListDto>(tickets);

            return Ok(ticketsToReturn);
        }
    }
}