using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using servicer.API.Data;
using servicer.API.Dtos;
using servicer.API.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace servicer.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TicketsController : ControllerBase
    {
        private readonly IServicerRepository _repository;
        private readonly IMapper _mapper;

        public TicketsController(IServicerRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateTicket(TicketForSendDto ticketForSendDto)
        {
            var ticketToCreate = _mapper.Map<Ticket>(ticketForSendDto);
            // TODO: fix
            ticketToCreate.ItemId = 3;
            var createdTicket = await _repository.CreateTicket(ticketToCreate);
            var ticketToReturn = _mapper.Map<TicketForDetailDto>(createdTicket);

            return CreatedAtRoute("GetTicket", new { controller = "Tickets", id = createdTicket.Id }, ticketToReturn);
        }

        [HttpGet]
        public async Task<IActionResult> GetTickets()
        {
            var tickets = await _repository.GetTickets();
            var ticketsToReturn = _mapper.Map<IEnumerable<TicketForListDto>>(tickets);

            return Ok(ticketsToReturn);
        }

        [HttpGet("{id}", Name = "GetTicket")]
        public async Task<IActionResult> GetTicket(int id)
        {
            var ticket = await _repository.GetTicket(id);
            var ticketToReturn = _mapper.Map<TicketForDetailDto>(ticket);

            return Ok(ticketToReturn);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTicket(int id)
        {
            await _repository.DeleteTicket(id);
            await _repository.SaveAll();

            return Ok();
        }
    }
}