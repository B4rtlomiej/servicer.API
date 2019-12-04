using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IMapper _mapper;
        public UsersController(IServicerRepository repository, IEmailService emailService,
            ITokenService tokenService, IMapper mapper)
        {
            _repository = repository;
            _emailService = emailService;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery]UserParams userParams)
        {
            var users = await _repository.GetUsers(userParams);

            var usersToReturn = _mapper.Map<IEnumerable<UserForListDto>>(users);

            Response.AddPagination(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);
            return Ok(usersToReturn);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _repository.GetUser(id);
            var userToReturn = _mapper.Map<UserForDetailDto>(user);

            return Ok(userToReturn);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserForUpdateDto userForUpdateDto)
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)
                && UserRole.Admin.ToString() != User.FindFirst(ClaimTypes.Role).Value.ToString())
            {
                return Unauthorized();
            }

            var userFromRepo = await _repository.GetUser(id);

            _mapper.Map(userForUpdateDto, userFromRepo);

            if (await _repository.SaveAll())
                return NoContent();

            throw new Exception($"Błąd przy edytowaniu użytkownika o id: {id}.");
        }

        [HttpPost("{id}/resetpassword")]
        public async Task<IActionResult> ResetPassword(int id, TokenDto tokenDto)
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)
                && UserRole.Admin.ToString() != User.FindFirst(ClaimTypes.Role).Value.ToString())
            {
                return Unauthorized();
            }

            var token = tokenDto.Token;

            if (_tokenService.IsTokenExpired(token))
            {
                return BadRequest("Token wygasł.");
            }

            var tokenId = _tokenService.GetTokenClaim(token, "nameid");
            var tokenRole = _tokenService.GetTokenClaim(token, "role");

            var userToResetPassword = await _repository.GetUser(id);

            if (tokenRole != UserRole.Admin.ToString() && tokenId != id.ToString())
            {
                return Unauthorized();
            }

            if (!userToResetPassword.IsActive)
            {
                return BadRequest("Użytkownik nieaktywny.");
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

        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/changeisactive")]
        public async Task<IActionResult> ChangeIsActive(int id, TokenDto tokenDto)
        {
            var token = tokenDto.Token;

            if (_tokenService.IsTokenExpired(token))
            {
                return BadRequest("Token wygasł.");
            }

            var tokenRole = _tokenService.GetTokenClaim(token, "role");

            if (tokenRole != UserRole.Admin.ToString())
            {
                return Unauthorized();
            }

            var userToUpdate = await _repository.GetUser(id);

            var wasActive = userToUpdate.IsActive;
            await _repository.ChangeIsActive(userToUpdate);

            var emailSubject = wasActive ? "Dezaktywacja" : "Aktywacja";
            emailSubject += " konta";
            var emailMessage = "Twoje konto zostało ";
            emailMessage += wasActive ? "dezaktywowane." : "aktywowane.";

            if (!wasActive)
            {
                emailMessage = GetEmailMessageWithResetPasswordToken(emailMessage, userToUpdate);
            }

            _emailService.SendEmailMessage(
                userToUpdate.Person.Email,
                emailSubject,
                "<h1>" + emailSubject + "</h1>" + emailMessage,
                emailMessage
            );

            return Ok(new
            {
                message = emailSubject
            });
        }

        private string GetEmailMessageWithResetPasswordToken(string emailMessage, User user)
        {
            string resetPasswordToken = _tokenService.CreateToken(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.UserRole.ToString())
            }, DateTime.Now.AddDays(1));

            emailMessage += "<p>Zresetuj hasło klikając w poniższy link:<br></p>"
                + "<p><a href='http://localhost:4200/activate/" + resetPasswordToken + "'>Zresetuj hasło</a></p>";

            return emailMessage;
        }
    }
}