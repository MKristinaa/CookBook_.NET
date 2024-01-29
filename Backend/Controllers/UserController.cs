using Backend.Dto;
using Backend;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Backend.Models;
using Backend.Interfaces;
using System.Net.Mail;
using System.Net;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using static System.Net.WebRequestMethods;

namespace MySecrets.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KorisnikController : ControllerBase
    {
        private readonly IUnitOfWork uow;
        private readonly IConfiguration configuration;
        private readonly DataContext dc;

        public KorisnikController(IUnitOfWork uow, IConfiguration configuration, DataContext dc)
        {
            this.uow = uow;
            this.configuration = configuration;
            this.dc = dc;
        }


        [HttpPost("/login")]
        public async Task<IActionResult> Login(loginInfoDto loginReq)
        {
            Console.WriteLine(loginReq.UsernameOrEmail);

            var user = await uow.UserRepository.Authenticate(loginReq.UsernameOrEmail!, loginReq.Password!);

            if (user == null)
            {
                return Unauthorized("Pogrešno korisničko ime, email ili lozinka.");
            }

            if (user.Verified == true)
            {
                var login = new LoginDto();
                login.Username = user.Username;
                login.Token = CreateJWT(user);
                return Ok(login);
            }
            else
            {
                return BadRequest(400);
            }
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
                new Claim("Name", user.Name),
                new Claim("Lastname", user.Lastname),
                new Claim("Image", user.Image)
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

        [HttpGet("/getInfo")]
        public async Task<IActionResult> getInfo()
        {
            var token = Request.Headers["Authorization"].ToString();

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized("No token provided");
            }

            token = token.Split(' ')[1];

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

            var claims = jwtToken?.Claims.ToDictionary(c => c.Type, c => c.Value);

            return Ok(claims);
        }


        [HttpPost("/register")]
        public async Task<IActionResult> Register(UserDto loginReq)
        {
            bool userExists = await uow.UserRepository.UserAlreadyExists(loginReq.Username!, loginReq.Email!);

            if (userExists)
            {
                if (await uow.UserRepository.UserAlreadyExistsByUsername(loginReq.Username!))
                {
                    return BadRequest("Korisnik već postoji");
                }
                else if (await uow.UserRepository.UserAlreadyExistsByEmail(loginReq.Email!))
                {
                    return BadRequest("Email već postoji");
                }
                else
                {
                    return BadRequest("Korisnik već postoji, ali nije pronađen ni po korisničkom imenu ni po emailu");
                }
            }
            else
            {
                uow.UserRepository.Register(loginReq);
                SendEmailAsync(loginReq.Email, loginReq.Username);
                await uow.SaveAsync();
                return Ok(loginReq);
            }
        }



        private async Task SendEmailAsync(string to, string username)
        {
            string fromMail = configuration["Email:FromMail"];
            string fromPassword = configuration["Email:FromPassword"];

            string c = $"http://mkristina9-001-site1.ftempurl.com/verified?username={username}";

            string body = $@"
                <h1>Verifikujte svoj nalog</h1>
                <p>Kliknite na <a href='{c}'>Potvrdi</a> za verifikaciju vašeg naloga.</p>
            ";

            using (MailMessage message = new MailMessage())
            {
                message.From = new MailAddress(fromMail);
                message.Subject = "Recepti";
                message.To.Add(new MailAddress(to));
                message.Body = body;
                message.IsBodyHtml = true;

                using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com"))
                {
                    smtpClient.Port = 587;
                    smtpClient.Credentials = new NetworkCredential(fromMail, fromPassword);
                    smtpClient.EnableSsl = true;

                    await smtpClient.SendMailAsync(message);
                }
            }
        }

        [HttpGet("/verified")]
        public async Task<IActionResult> verifeEmail(string username)
        {
            var korisnik = await dc.Users.Where(u => u.Username == username).FirstOrDefaultAsync(); 
            
            korisnik.Verified= true;
            dc.SaveChangesAsync();

            string body = $@"
                <div style='text-align: center; font-size: 20px;'>
                    <h1>Verifikacija naloga</h1>
                    <p>Uspesno ste se verifikovali!</p>
                </div>
            ";

            return Content(body, "text/html");

        }

        [HttpPost("/resetPassword")]
        public async Task<IActionResult> ResetPassword(string Email)
        {
            var user = await dc.Users.FirstOrDefaultAsync(u => u.Email == Email);

            if (user == null)
            {
                return NotFound("Korisnik sa datom email adresom nije pronađen.");
            }

            SendResetPasswordEmail(user);

            return Ok("Mail je poslat.");
        }

        private async Task SendResetPasswordEmail(User user)
        {
            string fromMail = configuration["Email:FromMail"];
            string fromPassword = configuration["Email:FromPassword"];

            string resetLink = $"http://mkristina9-001-site1.ftempurl.com/formUpdatePassword?username={user.Username}";

            string body = $@"
                <h2>Resetovanje lozinke</h2>
                <p>Kliknite na link kako biste resetovali sifru:<br> <a href='{resetLink}'>Klikni ovde</a></p>
               
            ";


            using (MailMessage message = new MailMessage())
            {
                message.From = new MailAddress(fromMail);
                message.Subject = "Resetovanje lozinke";
                message.To.Add(new MailAddress(user.Email));
                message.Body = body;
                message.IsBodyHtml = true;

                using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com"))
                {
                    smtpClient.Port = 587;
                    smtpClient.Credentials = new NetworkCredential(fromMail, fromPassword);
                    smtpClient.EnableSsl = true;

                    await smtpClient.SendMailAsync(message);
                }
            }
        }

        [HttpGet("/formUpdatePassword")]
        public async Task<IActionResult> formUpdatePassword(string username)
        {
            string resetLink = $"http://mkristina9-001-site1.ftempurl.com/updatePassword";

            string body = $@"
            <div style='text-align: center; font-size: 40px;width:100%'>
                <h2>Resetovanje lozinke</h2>
                <p>Unesite novu lozinku</p>
                <form action='{resetLink}' method='post' style='margin-top: 30px;font-size:30px;'>
                    <input type='hidden' name='username' value='{username}' />
                    <table style='margin: auto;font-size:35px;'>
                        <tr>
                            <td>Nova lozinka:</td>
                            <td>
                                <input type='password' name='newPassword' id='newPassword' style='height:60px; width:100%;font-size:30px;'>
                            </td>
                            <td>
                                <input type='checkbox' onclick=""togglePasswordVisibility('newPassword')"" style='height:40px; width:40px;'>Prikazi sifru
                            </td>
                        </tr>
                        <tr>
                            <td>Potvrda lozinke:</td>
                            <td>
                                <input type='password'  name='confirmPassword' id='confirmPassword' style='height:60px; width:100%;font-size:30px;'>
                            </td>
                            <td>
                                <input type='checkbox' onclick=""togglePasswordVisibility('confirmPassword')"" style='height:40px; width:40px;'>Prikazi sifru
                            </td>
                        </tr>
                    </table>
                    <p id='passwordMismatch' style='color: red; display: none; font-size:30px; margin-top: 10px;'>Sifra nije ista.</p>
                    <br><input type='submit' value='Potvrdi' onclick='return checkPasswordMatch()' style='height:50px; width:200px; margin-top:20px; font-size:30px; border-radius:50px; display:block; margin:auto;' />
                </form>

                <script>
                    function togglePasswordVisibility(inputId) {{
                        var input = document.getElementById(inputId);
                        if (input.type === 'password') {{
                            input.type = 'text';
                        }} else {{
                            input.type = 'password';
                        }}
                    }}

                    function checkPasswordMatch() {{
                        var newPassword = document.getElementById('newPassword').value;
                        var confirmPassword = document.getElementById('confirmPassword').value;
                        var mismatchMessage = document.getElementById('passwordMismatch');

                        if (newPassword !== confirmPassword) {{
                            mismatchMessage.style.display = 'block';
                            return false;
                        }} else {{
                            mismatchMessage.style.display = 'none';
                            return true;
                        }}
                    }}
                </script>
            </div>
            ";

            return Content(body, "text/html");
        }


        [HttpPost("/updatePassword")]
        public async Task<IActionResult> UpdatePassword([FromForm] string username, [FromForm] string newPassword)
        {
            var user = await dc.Users.FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
            {
                return NotFound("Korisnik nije pronađen");
            }

            byte[] passwordHash, passwordKey;

            using (var hmac = new HMACSHA512())
            {
                passwordKey = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(newPassword));
            }

            user.Password = passwordHash;
            user.KeyPassword = passwordKey;

            await dc.SaveChangesAsync();

            string body = $@"
                <div style='text-align: center; font-size: 40px;'>
                    <h1>Promena lozinke</h1>
                    <p>Uspesno ste promenili lozinku!</p>
                    <p>Vasa nova lozinka je: {newPassword}</p>
                </div>
            ";

            return Content(body, "text/html");
        }

        
    }
}
