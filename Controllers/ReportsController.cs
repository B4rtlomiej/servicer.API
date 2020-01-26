using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using servicer.API.Data;
using Microsoft.AspNetCore.Authorization;

namespace servicer.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IServicerRepository _repository;
        private readonly IMapper _mapper;

        public ReportsController(IServicerRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetTickets()
        {
            var agentsWithMostClosedTickets = _repository.GetAgentsWithMostClosedTickets();
            var productsWithMostTickets = _repository.GetProductsWithMostTickets();
            var ticketsByMonths = _repository.GetTicketsByMonths();
            var closedTicketsByMonths = _repository.GetClosedTicketsByMonths();

            return Ok(new {
                agentTickets = agentsWithMostClosedTickets,
                productTickets = productsWithMostTickets,
                ticketsByMonths = ticketsByMonths,
                closedTicketsByMonths = closedTicketsByMonths
            });
        }
    }
}