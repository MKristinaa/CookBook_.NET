using Backend.Dto;
using Backend;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Backend.Models;
using Backend.Interfaces;

namespace MySecrets.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KorisnikController : ControllerBase
    {
        private readonly IUnitOfWork uow;
        private readonly IConfiguration configuration;

        public KorisnikController(IUnitOfWork uow, IConfiguration configuration)
        {
            this.uow = uow;
            this.configuration = configuration;
        }


        [HttpPost("/login")]
        public async Task<IActionResult> Login(UserDto loginReq)
        {

            Console.WriteLine(loginReq.Username);

            var user = await uow.UserRepository.Authenticate(loginReq.Username!, loginReq.Password!);

            if (user == null)
            {
                return Ok(null);

            }

            var login = new LoginDto();
            login.Username = user.Username;
            login.Token = CreateJWT(user);
            return Ok(login);
        }

        private string CreateJWT(User user)
        {
            var secretKey = configuration.GetSection("AppSettings:Key").Value;
            var key = new SymmetricSecurityKey(Encoding.UTF8
                     .GetBytes(secretKey!));

            var claims = new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            };

            var signingCredentials = new SigningCredentials(
               key, SecurityAlgorithms.HmacSha256Signature);

            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(10),
                SigningCredentials = signingCredentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescription);
            return tokenHandler.WriteToken(token);
        }
        /*[HttpPost("post")]
        public async Task<IActionResult> PostAsync(KorisnikDto korisnik)
        {
            uow.KorisnikRepository.Register(korisnik);
            await uow.SaveAsync();
            return Ok(korisnik);
        }
        */

        [HttpPost("/register")]
        public async Task<IActionResult> Register(UserDto loginReq)
        {
            if (await uow.UserRepository.UserAlreadyExists(loginReq.Username!))
                return BadRequest("User vec postoji");

            uow.UserRepository.Register(loginReq);
            await uow.SaveAsync();
            return Ok(loginReq);
        }
    }
}
