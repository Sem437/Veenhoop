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
                return BadRequest("Invalid login request.");
            }

            List<string> Rollen = new List<string>();
            var docent  = await _context.Docenten.FirstOrDefaultAsync(d => d.Email == request.Email);

            if(docent != null)
            {                
                if (!CheckPassword(request.Password, docent.Wachtwoord))
                {
                    return Unauthorized("Invalid password.");
                }

                var DbRollen = await _context.RolGebruiker.Where(d => d.userId == docent.Id)
                    .Select(r => r.rolId)
                    .ToListAsync();
                var Dbrol = await _context.Rol.Where(r => DbRollen.Contains(r.Id))
                    .Select(r => r.Naam)
                    .FirstOrDefaultAsync();

                if (Dbrol != null)
                {
                    Rollen.Add(Dbrol);
                }

                string Rol = "Docent";
                Rollen.Add(Rol);

                var token = generateJwtToken(docent.Email, docent.Id, docent.Voornaam, docent.Tussenvoegsel, docent.Achternaam, Rollen);
                return Ok(new { token });
            }

            var gebruiker = await _context.Gebruikers.FirstOrDefaultAsync(g => g.Email == request.Email);

            if (gebruiker != null)
            {
                if(!CheckPassword(request.Password, gebruiker.Wachtwoord))
                {
                    return Unauthorized("Invalid password.");
                }

                var Rol = "Student";
                Rollen.Add(Rol);

                var token = generateJwtToken(gebruiker.Email, gebruiker.Id, gebruiker.Voornaam, gebruiker.Tussenvoegsel, gebruiker.Achternaam, Rollen);
                return Ok(new { token });
            }


            return Ok();
           
        }

        // GET: api/Rol
        [HttpGet("Rol")]
        public async Task<ActionResult<IEnumerable<Rol>>> GetRol()
        {
            return await _context.Rol.ToListAsync();
        }

        // GET: api/Rol/5
        [HttpGet("Rol/{id}")]
        public async Task<ActionResult<Rol>> GetRol(int id)
        {
            var rol = await _context.Rol.FindAsync(id);

            if (rol == null)
            {
                return NotFound();
            }

            return rol;
        }

        // PUT: api/Rol/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("Rol/{id}")]
        public async Task<IActionResult> PutRol(int id, Rol rol)
        {
            if (id != rol.Id)
            {
                return BadRequest();
            }

            _context.Entry(rol).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RolExists(id))
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

        // POST: api/Rol
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("Rol")]
        public async Task<ActionResult<Rol>> PostRol(Rol rol)
        {
            _context.Rol.Add(rol);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRol", new { id = rol.Id }, rol);
        }

        // DELETE: api/Rol/5
        [HttpDelete("Rol/{id}")]
        public async Task<IActionResult> DeleteRol(int id)
        {
            var rol = await _context.Rol.FindAsync(id);
            if (rol == null)
            {
                return NotFound();
            }

            _context.Rol.Remove(rol);
            await _context.SaveChangesAsync();

            return NoContent();
        }






        // GET: api/RolGebruikers
        [HttpGet("RolGebruikers")]
        public async Task<ActionResult<IEnumerable<RolGebruiker>>> GetRolGebruiker()
        {
            return await _context.RolGebruiker.ToListAsync();
        }

        // GET: api/RolGebruikers/5
        [HttpGet("RolGebruikers{id}")]
        public async Task<ActionResult<RolGebruiker>> GetRolGebruiker(int id)
        {
            var rolGebruiker = await _context.RolGebruiker.FindAsync(id);

            if (rolGebruiker == null)
            {
                return NotFound();
            }

            return rolGebruiker;
        }

        // PUT: api/RolGebruikers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("Rolgebruikers{id}")]
        public async Task<IActionResult> PutRolGebruiker(int id, RolGebruiker rolGebruiker)
        {
            if (id != rolGebruiker.Id)
            {
                return BadRequest();
            }

            _context.Entry(rolGebruiker).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RolGebruikerExists(id))
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

        // POST: api/RolGebruikers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("RolGebruikers")]
        public async Task<ActionResult<RolGebruiker>> PostRolGebruiker(RolGebruiker rolGebruiker)
        {
            _context.RolGebruiker.Add(rolGebruiker);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRolGebruiker", new { id = rolGebruiker.Id }, rolGebruiker);
        }

        // DELETE: api/RolGebruikers/5
        [HttpDelete("Rolgebruikers/{id}")]
        public async Task<IActionResult> DeleteRolGebruiker(int id)
        {
            var rolGebruiker = await _context.RolGebruiker.FindAsync(id);
            if (rolGebruiker == null)
            {
                return NotFound();
            }

            _context.RolGebruiker.Remove(rolGebruiker);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RolGebruikerExists(int id)
        {
            return _context.RolGebruiker.Any(e => e.Id == id);
        }

        private bool RolExists(int id)
        {
            return _context.Rol.Any(e => e.Id == id);
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

        private string generateJwtToken(string email, int Id, string voorNaam, string? tv, string achterNaam, List<string> Rollen)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("B9$uR2!fZ1@vL7#xQ3^pM5&nH8*wA0dE");           

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, Id.ToString()),
                new Claim(ClaimTypes.Name, $"{voorNaam} {tv} {achterNaam}"),
                new Claim(ClaimTypes.Email, email)
            };

            foreach (var rol in Rollen)
            {
                claims.Add(new Claim(ClaimTypes.Role, rol));
            }

            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(24),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescription);
            return tokenHandler.WriteToken(token);
        }
    }
}
