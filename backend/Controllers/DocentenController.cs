using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Veenhoop.Data;
using Veenhoop.Models;
using Veenhoop.Dto;
using System.Text;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Veenhoop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocentenController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DocentenController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Docenten
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Docenten>>> GetDocenten()
        {
            return await _context.Docenten.ToListAsync();
        }

        // GET: api/Docenten/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Docenten>> GetDocenten(int id)
        {
            var docenten = await _context.Docenten.FindAsync(id);

            if (docenten == null)
            {
                return NotFound();
            }

            return docenten;
        }

        // GET: api/Docenten/klassen/5
        [HttpGet("klassen/{docentId}")]
        public async Task<ActionResult<List<KlasStudentDto>>> GetDocentenKlassen(int docentId)
        {
            var docentIdCheck = await _context.Docenten.FirstOrDefaultAsync(d => d.Id == docentId);

            if (docentIdCheck == null)
            {
                return NotFound();
            }

            var docentVakken = await _context.DocentVakken
                .Where(dv => dv.DocentId == docentId)
                .ToListAsync();

            var klasIds = docentVakken.Select(k => k.KlasId).ToList();
            var vakIds  = docentVakken.Select(v => v.VakId ).ToList();

            var klassen = await _context.Klassen
                .Where(k => klasIds.Contains(k.Id))
                .Include(k => k.Studenten)
                .ToListAsync();
         
            var vakken = await _context.Vakken
                .Where(v => vakIds.Contains(v.Id))
                .ToListAsync();           

            var result = docentVakken.Select(dv =>
            {
                var klas = klassen.FirstOrDefault(k => k.Id == dv.KlasId);
                var vak = vakken.FirstOrDefault(v => v.Id == dv.VakId);

                return new KlasStudentDto
                {
                    Id = dv.Id,
                    KlasId = klas.Id,
                    KlasNaam = klas.KlasNaam,
                    vakId = vak.Id,
                    VakNaam = vak.VakNaam,
                    Studenten = klas.Studenten.Select(s => new StudentDto
                    {
                        Id = s.Id,
                        Voornaam = s.Voornaam,
                        Tussenvoegsel = s.Tussenvoegsel,
                        Achternaam = s.Achternaam
                    }).ToList()
                };
            });


            return Ok(result);
        }

        // GET: api/Docenten/klassen/5/klasId/vakId
        [HttpGet("klassen/{docentId}/{docentVakkenId}")]
        public async Task<ActionResult<KlasStudentDto>> GetDocentKlas(int docentId, int docentVakkenId)
        {
            var docent = await _context.Docenten.FindAsync(docentId);
            if (docent == null)
            {
                return NotFound("Docent niet gevonden.");
            }

            var docentVak = await _context.DocentVakken
                .FirstOrDefaultAsync(dv => dv.Id == docentVakkenId && dv.DocentId == docentId);

            if (docentVak == null)
            {
                return NotFound("Koppeling tussen docent en vak niet gevonden.");
            }

            var klas = await _context.Klassen
                .Include(k => k.Studenten)
                .FirstOrDefaultAsync(k => k.Id == docentVak.KlasId);

            if (klas == null)
            {
                return NotFound("Klas niet gevonden.");
            }

            var vak = await _context.Vakken.FindAsync(docentVak.VakId);
            if (vak == null)
            {
                return NotFound("Vak niet gevonden.");
            }

            var result = new KlasStudentDto
            {
                Id = docentVak.Id,
                KlasId = klas.Id,
                KlasNaam = klas.KlasNaam,
                vakId = vak.Id,
                VakNaam = vak.VakNaam,
                Studenten = klas.Studenten.Select(s => new StudentDto
                {
                    Id = s.Id,
                    Voornaam = s.Voornaam,
                    Tussenvoegsel = s.Tussenvoegsel,
                    Achternaam = s.Achternaam
                }).ToList()
            };

            return Ok(result);
        }

        // POST: api/Docenten/klas
        [HttpPost("klas")]
        public async Task<IActionResult> PostCijfers([FromBody] CijferInvoerDto cijferInvoer)
        {
            if (cijferInvoer == null)
            {
                return BadRequest("Geen data ontvangen.");
            }

            using var transaction = _context.Database.BeginTransaction();
            try
            {
                foreach (var cijfer in cijferInvoer.Cijfers)
                {
                    var cijferEntity = new Cijfers
                    {
                        Leerjaar = cijferInvoer.Leerjaar,
                        Periode = cijferInvoer.Periode,
                        ToetsId = cijferInvoer.ToetsId,
                        GebruikersId = cijfer.StudentId,
                        Cijfer = cijfer.Cijfer
                    };
                    _context.Cijfers.Add(cijferEntity);
                }

                await _context.SaveChangesAsync();

                transaction.Commit();

                return Ok("Cijfers succesvol opgeslagen.");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return BadRequest($"Er is een fout opgetreden: {ex.Message}");
            }

        }

        // PUT: api/Docenten/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDocenten(int id, Docenten docenten)
        {
            if (id != docenten.Id)
            {
                return BadRequest();
            }

            _context.Entry(docenten).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DocentenExists(id))
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

        // POST: api/Docenten
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Docenten>> PostDocenten(Docenten docenten)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var emailCheck = await _context.Docenten
                .Join(_context.Gebruikers,
                d => d.Email,
                g => g.Email,
                (d, g) => new { Docent = d, Gebruiker = g })
                .FirstOrDefaultAsync(dg => dg.Docent.Email  == docenten.Email ||
                dg.Gebruiker.Email == docenten.Email);

            if (emailCheck != null)
            {
                return BadRequest("Email is al in gebruik.");
            }

            string hashedPassword = HashPassword(docenten.Wachtwoord);
            docenten.Wachtwoord = hashedPassword;

            _context.Docenten.Add(docenten);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDocenten", new { id = docenten.Id }, docenten);
        }

        // DELETE: api/Docenten/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDocenten(int id)
        {
            var docenten = await _context.Docenten.FindAsync(id);
            if (docenten == null)
            {
                return NotFound();
            }

            _context.Docenten.Remove(docenten);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DocentenExists(int id)
        {
            return _context.Docenten.Any(e => e.Id == id);
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
