using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using servicer.API.Data;
using servicer.API.Dtos;
using servicer.API.Models;

namespace servicer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repository;
        private readonly IServicerRepository _servicerRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        public AuthController(IAuthRepository repository, IServicerRepository servicerRepository, IMapper mapper, IConfiguration configuration)
        {
            _repository = repository;
            _servicerRepository = servicerRepository;
            _mapper = mapper;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
            userForRegisterDto.Username = userForRegisterDto.Username.ToLower();

            if (await _repository.UserExists(userForRegisterDto.Username))
            {
                return BadRequest("Login zajęty.");
            }

            var userToCreate = _mapper.Map<User>(userForRegisterDto);

            var createdUser = await _repository.Register(userToCreate);
            var userToReturn = _mapper.Map<UserForDetailDto>(createdUser);

            return CreatedAtRoute("GetUser", new { controller = "Users", id = createdUser.Id }, userToReturn);
        }


        [HttpPost("activate")]
        public async Task<ActionResult> Register(PasswordsForUserActivationDto passwordsForUserActivationDto)
        {
            var jsonToken = new JwtSecurityTokenHandler().ReadJwtToken(passwordsForUserActivationDto.Token);

            if (jsonToken.ValidTo < DateTime.Now.AddMinutes(1))
            {
                return BadRequest("Token wygasł.");
            }

            var username = jsonToken.Claims.First(claim => claim.Type == "nameid").Value;

            var userToActivate = await _servicerRepository.GetInactiveUser(username);
            if (userToActivate == null)
            {
                return BadRequest("Podany użytkownik nie istnieje. Lub konto jest już aktywne.");
            }

            var activatedUser = _repository.Activate(userToActivate, passwordsForUserActivationDto.Password);

            return Ok(new
            {
                message = "Aktywowano użytkownika."
            });
        }

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

            string confirmationToken = CreateToken(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userForRegisterDto.Username),
                    new Claim(ClaimTypes.Role, userForRegisterDto.UserRole.ToString())
                }, DateTime.Now.AddHours(8)
            );

            SendEmailMessage(userForRegisterDto.Person.Email, "Witamy w servicer", "<h1>Witaj " + userForRegisterDto.Person.FirstName + "</h1>"
            + "<p>Aktywuj konto korzystająć z poniższego linka:<br/></p>"
            + "<p><a href='http://localhost:4200/activate/" + confirmationToken + "'>Aktywuj konto</a></p>",
            "Aktywuj konto korzystająć z poniższego linka:\n localhost:5000/api/auth/register/auth/" + confirmationToken);

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
                token = CreateToken(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.Username),
                new Claim(ClaimTypes.Role, userFromRepo.UserRole.ToString())
            }, DateTime.Now.AddDays(1))
            });
        }

        private void SendEmailMessage(string toAddress, string subject, string messageHtml, string messageTxt)
        {
            MimeMessage emailMessage = new MimeMessage();

            MailboxAddress from = new MailboxAddress("servicer", "servicer.smtp@gmail.com");
            emailMessage.From.Add(from);

            MailboxAddress to = new MailboxAddress("User", toAddress);
            emailMessage.To.Add(to);

            emailMessage.Subject = subject;

            BodyBuilder bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = messageHtml;
            bodyBuilder.TextBody = messageTxt;

            emailMessage.Body = bodyBuilder.ToMessageBody();

            SmtpClient client = new SmtpClient();
            client.Connect(_configuration.GetSection("EmailConfiguration:EmailAddress").Value, 587, false);
            client.Authenticate(_configuration.GetSection("EmailConfiguration:Login").Value, _configuration.GetSection("EmailConfiguration:Password").Value);

            client.Send(emailMessage);
            client.Disconnect(true);
            client.Dispose();
        }

        private string CreateToken(Claim[] claims, DateTime expiryDate)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiryDate,
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
