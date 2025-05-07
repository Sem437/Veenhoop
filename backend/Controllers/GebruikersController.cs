using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Veenhoop.Data;
using Veenhoop.Models;

namespace Veenhoop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GebruikersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GebruikersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Gebruikers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Gebruikers>>> GetGebruikers()
        {
            return await _context.Gebruikers.ToListAsync();
        }

        // GET: api/Gebruikers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Gebruikers>> GetGebruikers(int id)
        {
            var gebruikers = await _context.Gebruikers.FindAsync(id);

            if (gebruikers == null)
            {
                return NotFound();
            }

            return gebruikers;
        }

        // PUT: api/Gebruikers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGebruikers(int id, Gebruikers gebruikers)
        {
            if (id != gebruikers.Id)
            {
                return BadRequest();
            }

            _context.Entry(gebruikers).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GebruikersExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Gebruikers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Gebruikers>> PostGebruikers([FromBody] Gebruikers gebruikers)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var EmailCheck = await _context.Gebruikers
                .FirstOrDefaultAsync(x => x.Email == gebruikers.Email);

            if (EmailCheck != null)
            {
                return BadRequest("Email bestaat al");
            }

            string hashedPassword = HashPassword(gebruikers.Wachtwoord);
            gebruikers.Wachtwoord = hashedPassword;

            _context.Gebruikers.Add(gebruikers);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGebruikers", new { id = gebruikers.Id }, gebruikers);
        }

        // DELETE: api/Gebruikers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGebruikers(int id)
        {
            var gebruikers = await _context.Gebruikers.FindAsync(id);
            if (gebruikers == null)
            {
                return NotFound();
            }

     

            _context.Gebruikers.Remove(gebruikers);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GebruikersExists(int id)
        {
            return _context.Gebruikers.Any(e => e.Id == id);
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
