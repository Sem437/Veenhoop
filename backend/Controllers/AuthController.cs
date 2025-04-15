using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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

        // POST: api/Auth/Register
        [HttpPost("Register")]
        public async Task<IActionResult> Registeren([FromBody] Gebruikers model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var EmailCheck = await _context.Gebruikers
                .FirstOrDefaultAsync(x => x.Email == model.Email);

            if (EmailCheck != null)
            {
                return BadRequest("Email bestaat al");
            }

            string hashedPassword = HashPassword(model.Wachtwoord);

            var user = new Gebruikers
            {
                StudentenNummer = 0,
                Voornaam = model.Voornaam,
                Tussenvoegsel = model.Tussenvoegsel,
                Achternaam = model.Achternaam,
                GeboorteDatum = model.GeboorteDatum,
                Stad = model.Stad,
                Adres = model.Adres,
                Postcode = model.Postcode,
                Email = model.Email,
                Wachtwoord = hashedPassword,
                KlasId = 1
            };
        
            var Result = await _context.Gebruikers.AddAsync(user);

            if(Result == null)
            {
                return BadRequest("Er is iets fout gegaan");
            }

            return Ok("Account is aangemaakt");
        }


        // Hashes a password using SHA256
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }


    }
}
