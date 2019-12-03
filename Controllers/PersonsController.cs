using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using servicer.API.Data;
using servicer.API.Dtos;
using servicer.API.Helpers;
using servicer.API.Services;

namespace servicer.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PersonsController : ControllerBase
    {
        private readonly IServicerRepository _repository;
        private readonly IMapper _mapper;
        public PersonsController(IServicerRepository repository, IEmailService emailService,
            ITokenService tokenService, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetPersons([FromQuery]PersonParams personParams)
        {
            var persons = await _repository.GetPersons(personParams);

            var personsToReturn = _mapper.Map<IEnumerable<PersonForListDto>>(persons);

            Response.AddPagination(persons.CurrentPage, persons.PageSize, persons.TotalCount, persons.TotalPages);
            return Ok(personsToReturn);
        }

        [HttpGet("{id}", Name = "GetPerson")]
        public async Task<IActionResult> GetPerson(int id)
        {
            var person = await _repository.GetPerson(id);
            var personToReturn = _mapper.Map<PersonForDetailDto>(person);

            return Ok(personToReturn);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePerson(int id, PersonForUpdateDto personForUpdate)
        {
            var personFromRepo = await _repository.GetPerson(id);

            _mapper.Map(personForUpdate, personFromRepo);

            if (await _repository.SaveAll())
                return NoContent();

            throw new Exception($"Błąd edycji persony o id: {id}.");
        }
    }
}