using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using servicer.API.Data;
using servicer.API.Dtos;
using servicer.API.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System;
using servicer.API.Helpers;

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
            var productSpecificationId = await _repository.GetProductSpecificationId(
                ticketToCreate.Item.ProductSpecification.Manufacturer,
                ticketToCreate.Item.ProductSpecification.Series,
                ticketToCreate.Item.ProductSpecification.Name
            );

            if (productSpecificationId == null)
            {
                return BadRequest("Wybrany produk nie istnieje, bądź nie jest aktywny.");
            }

            ticketToCreate.Item.ProductSpecificationId = (int)productSpecificationId;

            var customerId = await _repository.GetCustomerId(
                ticketToCreate.Item.Customer.Person.Email,
                ticketToCreate.Item.Customer.Person.FirstName,
                ticketToCreate.Item.Customer.Person.LastName
            );

            if (customerId != null)
            {
                ticketToCreate.Item.CustomerId = (int)customerId;

                var itemId = await _repository.GetItemId(
                    (int)productSpecificationId,
                    (int)customerId,
                    ticketToCreate.Item.ProductionYear
                );

                if (itemId != null)
                {
                    ticketToCreate.ItemId = (int)itemId;
                }
            }

            var createdTicket = await _repository.CreateTicket(ticketToCreate);
            var ticketToReturn = _mapper.Map<TicketForDetailDto>(createdTicket);

            return CreatedAtRoute("GetTicket", new { controller = "Tickets", id = createdTicket.Id }, ticketToReturn);
        }

        [HttpGet]
        public async Task<IActionResult> GetTickets([FromQuery]TicketParams ticketParams)
        {
            var tickets = await _repository.GetTickets(ticketParams);

            var ticketsToReturn = _mapper.Map<IEnumerable<TicketForListDto>>(tickets);

            Response.AddPagination(tickets.CurrentPage, tickets.PageSize, tickets.TotalCount, tickets.TotalPages);

            return Ok(ticketsToReturn);
        }

        [HttpGet("{id}", Name = "GetTicket")]
        public async Task<IActionResult> GetTicket(int id)
        {
            var ticket = await _repository.GetTicket(id);
            var ticketToReturn = _mapper.Map<TicketForDetailDto>(ticket);

            return Ok(ticketToReturn);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTicket(int id, TicketForUpdateDto ticketForUpdate)
        {
            // TODO: admin/manager/owner

            var ticketFromRepo = await _repository.GetTicket(id);

            _mapper.Map(ticketForUpdate, ticketFromRepo);

            if (await _repository.SaveAll())
                return NoContent();

            throw new Exception($"Błąd przy edytowaniu zgłoszenia o id: {id}.");
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