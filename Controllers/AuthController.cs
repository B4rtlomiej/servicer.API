using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using servicer.API.Data;
using servicer.API.Dtos;
using servicer.API.Models;
using servicer.API.Services;

namespace servicer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repository;
        private readonly IServicerRepository _servicerRepository;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        public AuthController(IAuthRepository repository, IServicerRepository servicerRepository, ITokenService tokenService,
            IEmailService emailService, IMapper mapper, IConfiguration configuration)
        {
            _repository = repository;
            _servicerRepository = servicerRepository;
            _tokenService = tokenService;
            _emailService = emailService;
            _mapper = mapper;
            _configuration = configuration;
        }

        [HttpPost("setpassword")]
        public async Task<ActionResult> Register(PasswordsForPasswordSetDto passwordsForPasswordSet)
        {
            var token = passwordsForPasswordSet.Token;

            if (_tokenService.IsTokenExpired(token))
            {
                return BadRequest("Token wygasł.");
            }

            var username = _tokenService.GetTokenClaim(token, "unique_name");

            var userToSetPassword = await _servicerRepository.GetUser(username);
            if (userToSetPassword == null)
            {
                return BadRequest("Podany użytkownik nie istnieje");
            }

            if (!userToSetPassword.IsActive)
            {
                userToSetPassword.IsActive = true;
            }

            var activatedUser = _repository.SetPassword(userToSetPassword, passwordsForPasswordSet.Password);

            return Ok(new
            {
                message = "Aktywowano użytkownika."
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("register/user")]
        public async Task<IActionResult> RegisterUser(UserForRegisterDto userForRegisterDto)
        {
            userForRegisterDto.Username = userForRegisterDto.Username.ToLower();

            if (await _repository.UserExists(userForRegisterDto.Username))
            {
                return BadRequest("Login zajęty.");
            }

            var userToInsert = _mapper.Map<User>(userForRegisterDto);
            var createdUser = _repository.Register(userToInsert);

            string confirmationToken = _tokenService.CreateToken(new[]
                {
                    new Claim(ClaimTypes.Name, userForRegisterDto.Username),
                    new Claim(ClaimTypes.Role, userForRegisterDto.UserRole.ToString())
                }, DateTime.Now.AddHours(8)
            );

            _emailService.SendEmailMessage(userForRegisterDto.Person.Email, "Witamy w servicer", "<h1>Witaj " + userForRegisterDto.Person.FirstName + "</h1>"
            + "<p>Aktywuj konto korzystająć z poniższego linka:<br/></p>"
            + "<p><a href='http://localhost:4200/activate/" + confirmationToken + "'>Aktywuj konto</a></p>",
            "Aktywuj konto korzystając z poniższego linka:\n http://localhost:4200/activate/" + confirmationToken);

            return Ok(new
            {
                tokenUrl = "http://localhost:4200/activate/" + confirmationToken
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            var userFromRepo = await _repository.Login(userForLoginDto.Username.ToLower(), userForLoginDto.Password);

            if (userFromRepo == null)
            {
                return Unauthorized("Błędny login, lub hasło.");
            }
            else if (!userFromRepo.IsActive)
            {
                return Unauthorized("Konto nieaktywne. Skontaktuj się z administratorem.");
            }

            return Ok(new
            {
                token = _tokenService.CreateToken(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.Username),
                new Claim(ClaimTypes.Role, userFromRepo.UserRole.ToString())
            }, DateTime.Now.AddDays(1))
            });
        }
    }
}
