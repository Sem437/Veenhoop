using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Veenhoop.Data;
using Veenhoop.Models;

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
                return BadRequest("Invalid login request.");
            }

            var gebruiker = await _context.Gebruikers
                .FirstOrDefaultAsync(g => g.Email == request.Email);

            if (gebruiker == null)
            {
                return NotFound("User not found.");
            }

            bool wachtwoordCheck = CheckPassword(request.Password, gebruiker.Wachtwoord);

            if(!wachtwoordCheck)
            {
                return Unauthorized("Invalid password.");
            }

            return Ok("Inlog gelukt");
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
