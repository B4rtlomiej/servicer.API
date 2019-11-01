using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using servicer.API.Data;
using servicer.API.Dtos;
using servicer.API.Helpers;
using servicer.API.Models;
using servicer.API.Services;

namespace servicer.API.Controllers
{
    [ServiceFilter(typeof(SetLastActive))]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IServicerRepository _repository;
        private readonly IEmailService _emailService;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        public UsersController(IServicerRepository repository, IEmailService emailService,
            ITokenService tokenService, IConfiguration configuration, IMapper mapper)
        {
            _repository = repository;
            _emailService = emailService;
            _tokenService = tokenService;
            _configuration = configuration;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _repository.GetUsers();
            var usersToReturn = _mapper.Map<IEnumerable<UserForListDto>>(users);

            return Ok(usersToReturn);
        }

        [HttpGet("{id}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _repository.GetUser(id);
            var userToReturn = _mapper.Map<UserForDetailDto>(user);

            return Ok(userToReturn);
        }

        [HttpPost("{id}/resetpassword")]
        public async Task<IActionResult> ResetPassword(int id, TokenForPasswordResetDto tokenForPasswordReset)
        {
            var jsonToken = new JwtSecurityTokenHandler().ReadJwtToken(tokenForPasswordReset.Token);

            if (jsonToken.ValidTo < DateTime.Now.AddMinutes(1))
            {
                return BadRequest("Token wygasł.");
            }

            var tokenId = jsonToken.Claims.First(claim => claim.Type == "nameid").Value;
            var tokenRole = jsonToken.Claims.First(claim => claim.Type == "role").Value;

            var userToResetPassword = await _repository.GetUser(id);

            if (tokenRole != UserRole.Admin.ToString() && tokenId != id.ToString())
            {
                return Unauthorized();
            }

            string resetPasswordToken = _tokenService.CreateToken(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userToResetPassword.Id.ToString()),
                new Claim(ClaimTypes.Name, userToResetPassword.Username),
                new Claim(ClaimTypes.Role, userToResetPassword.UserRole.ToString())
            }, DateTime.Now.AddDays(1));


            _emailService.SendEmailMessage(userToResetPassword.Person.Email, "Resetowanie hasła",
                "<h1>Resetowanie hasła</h1><p>Zresetuj hasło klikając w poniższy link:<br></p>"
                + "<p><a href='http://localhost:4200/activate/" + resetPasswordToken + "'>Zresetuj hasło</a></p>",
                "Zresetuj hasło korzystając z poniższego linka:\n http://localhost:4200/activate/" + resetPasswordToken
            );

            return Ok(new
            {
                message = "Zresetowano hasło."
            });
        }
    }
}