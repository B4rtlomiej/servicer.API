using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using servicer.API.Data;
using Microsoft.AspNetCore.Authorization;
using servicer.API.Helpers;

namespace servicer.API.Controllers
{
    //[Authorize]
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

            return Ok(new {
                agentTickets = agentsWithMostClosedTickets,
                productTickets = productsWithMostTickets
            });
        }
    }
}