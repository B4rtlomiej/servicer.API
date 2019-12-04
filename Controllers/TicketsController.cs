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
using servicer.API.Services;
using System.Security.Claims;

namespace servicer.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TicketsController : ControllerBase
    {
        private readonly IServicerRepository _repository;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;

        public TicketsController(IServicerRepository repository, IMapper mapper, ITokenService tokenService, IEmailService emailService)
        {
            _repository = repository;
            _mapper = mapper;
            _tokenService = tokenService;
            _emailService = emailService;
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
                return BadRequest("Wybrany produkt nie istnieje, lub nie jest aktywny.");
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
            if(!string.IsNullOrEmpty(ticketParams.Token))
                ticketParams.Token = _tokenService.GetTokenClaim(ticketParams.Token, "nameid");
               
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
    
            var ticketNotes = await _repository.GetTicketNotes(id);
            ticketToReturn.TicketNotes = _mapper.Map<ICollection<NoteForDetailDto>>(ticketNotes);

            var customerNotes = await _repository.GetCustomerNotes(ticket.Item.CustomerId);
            ticketToReturn.CustomerNotes = _mapper.Map<ICollection<NoteForDetailDto>>(customerNotes);

            var itemNotes = await _repository.GetItemNotes(ticket.ItemId);
            ticketToReturn.ItemNotes = _mapper.Map<ICollection<NoteForDetailDto>>(itemNotes);

            var productSpecificationNotes = await _repository.GetProductSpecificationNotes(ticket.Item.ProductSpecificationId);
            ticketToReturn.ProductSpecificationNotes = _mapper.Map<ICollection<NoteForDetailDto>>(productSpecificationNotes);

            return Ok(ticketToReturn);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTicket(int id, TicketForUpdateDto ticketForUpdate)
        {
            var ticketFromRepo = await _repository.GetTicket(id);

            if (ticketFromRepo.UserId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)
                && UserRole.Admin.ToString() != User.FindFirst(ClaimTypes.Role).Value.ToString())
            {
                return Unauthorized();
            }

            _mapper.Map(ticketForUpdate, ticketFromRepo);

            if (await _repository.SaveAll())
                return NoContent();

            throw new Exception($"Błąd przy edytowaniu zgłoszenia o id: {id}.");
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTicket(int id)
        {
            await _repository.DeleteTicket(id);
            await _repository.SaveAll();

            return Ok();
        }

        [HttpPost("{id}/pickup")]
        public async Task<IActionResult> PickUpTicket(int id, TokenDto tokenDto)
        {
            var token = tokenDto.Token;
            if (_tokenService.IsTokenExpired(token))
                return BadRequest("Token wygasł.");

            var tokenId = _tokenService.GetTokenClaim(token, "nameid");
            var ticket = await _repository.GetTicket(id);
            ticket.UserId = Int32.Parse(tokenId);
            ticket.Status = Status.WorkedOn;
            await _repository.SaveAll();

            var customerEmailAddress = ticket.Item.Customer.Person.Email;
            var user = await _repository.GetUser(Int32.Parse(tokenId));
            _emailService.SendEmailMessage(
                customerEmailAddress,
                "Podjęto zgłoszenie: "+ticket.Id,
                "<p> Zgłoszenie "+ ticket.Id + " zostało podjęte przez użytkownika "+ user.Person.FirstName +" "+ user.Person.LastName +
                ". Możesz skontaktować się z nim pod numerem telefonu: "+ user.Person.Phone + " lub mailowo: "+ user.Person.Email+ ".</p>",
                ""
            );

            return Ok(new
            {
                message = "Wysłano maila."
            });
        }

        [HttpPost("{id}/close")]
        public async Task<IActionResult> CloseTicket(int id, TokenDto tokenDto)
        {
            var token = tokenDto.Token;
            if (_tokenService.IsTokenExpired(token))
                return BadRequest("Token wygasł.");

            var tokenId = _tokenService.GetTokenClaim(token, "nameid");
            var ticket = await _repository.GetTicket(id);

            if (ticket.UserId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)
                && UserRole.Admin.ToString() != User.FindFirst(ClaimTypes.Role).Value.ToString())
            {
                return Unauthorized();
            }

            await _repository.CloseTicket(ticket);
            var customerEmailAddress = ticket.Item.Customer.Person.Email;
            var user = await _repository.GetUser(Int32.Parse(tokenId));
            _emailService.SendEmailMessage(
                customerEmailAddress,
                "Twoje zgłoszenie zostało rozwiązane. Zapraszamy po odbiór sprzętu.",
                "<p>Zgłoszenie o numerze id: " + ticket.Id + " zostało rozwiązane przez użytkownika " + user.Person.FirstName + " " + user.Person.LastName + 
                "." + "W razie jakichkolwiek pytań proszę skontaktować się z nim pod numerem telefonu: " + user.Person.Phone + " lub mailowo: " + user.Person.Email + ".</p>",
                ""
            );

            return Ok(new
            {
                message = "Zamknięto zgłoszenie i wysłano maila."
            });
        }

        [HttpGet("customertickets")]
        public async Task<IActionResult> CustomerTicket([FromQuery]TicketParams ticketParams)
        {              
            var tickets = await _repository.GetTickets(ticketParams);

            var ticketsToReturn = _mapper.Map<IEnumerable<TicketForListDto>>(tickets);

            Response.AddPagination(tickets.CurrentPage, tickets.PageSize, tickets.TotalCount, tickets.TotalPages);

            return Ok(ticketsToReturn);
        }
    }
}