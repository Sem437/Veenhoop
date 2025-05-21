using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Veenhoop.Data;
using Veenhoop.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;   

namespace Veenhoop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        //POST: api/Auth/Login
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { message = "Ongeldig login-verzoek." });
            }

            var gebruiker = await _context.Gebruikers
                .FirstOrDefaultAsync(g => g.Email == request.Email);

            if (gebruiker == null)
            {
                return BadRequest(new { message = "E-mailadres is niet correct." });
            }

            bool wachtwoordCheck = CheckPassword(request.Password, gebruiker.Wachtwoord);

            if (!wachtwoordCheck)
            {
                return BadRequest(new { message = "Wachtwoord is niet correct." });
            }

            // JWT token aanmaken
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("B9$uR2!fZ1@vL7#xQ3^pM5&nH8*wA0dE");

            var tokenParameters = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
            new Claim(ClaimTypes.NameIdentifier, gebruiker.Id.ToString()),
            new Claim(ClaimTypes.Name, $"{gebruiker.Voornaam} {gebruiker.Tussenvoegsel} {gebruiker.Achternaam}"),
            new Claim(ClaimTypes.Email, gebruiker.Email)
        }),
                Expires = DateTime.UtcNow.AddHours(24),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenParameters);
            var jwt = tokenHandler.WriteToken(token);

            return Ok(new { token = jwt });
        }


        private bool CheckPassword(string wachtwoord, string hashedWachtwoord)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] InputByte = sha256.ComputeHash(Encoding.UTF8.GetBytes(wachtwoord));
                string hashedInput = Convert.ToBase64String(InputByte);

                return hashedInput == hashedWachtwoord;
            }
        }
    }
}
