using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Veenhoop.Data;
using Veenhoop.Dto;
using Veenhoop.Models;

namespace Veenhoop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KlassenController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public KlassenController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Klassen
        [HttpGet]
        public async Task<ActionResult<List<KlasDto>>> GetKlassen()
        {
            var klassen = await _context.Klassen.Include(k => k.Studenten).ToListAsync();

            var klasDtos = klassen.Select(klas => new KlasDto
            {
                KlasId = klas.Id,
                KlasNaam = klas.KlasNaam,
                Studenten = klas.Studenten.Select(s => new StudentDto
                {
                    Id = s.Id,
                    Voornaam = s.Voornaam,
                    Tussenvoegsel = s.Tussenvoegsel,
                    Achternaam = s.Achternaam
                }).ToList()
            }).ToList();

            return klasDtos;
        }

        // GET: api/Klassen/5
        [HttpGet("{id}")]
        public async Task<ActionResult<KlasDto>> GetKlassen(int id)
        {
            var klas = await _context.Klassen
                .Include(k => k.Studenten)
                .FirstOrDefaultAsync(k => k.Id == id);

            if (klas == null)
            {
                return NotFound();
            }

            var klasDto = new KlasDto
            {
                KlasId = klas.Id,
                KlasNaam = klas.KlasNaam,
                Studenten = klas.Studenten.Select(s => new StudentDto
                {
                    Id = s.Id,
                    Voornaam = s.Voornaam,
                    Tussenvoegsel = s.Tussenvoegsel,
                    Achternaam = s.Achternaam
                }).ToList()
            };

            return klasDto;
        }

        // PUT: api/Klassen/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutKlassen(int id, Klassen klassen)
        {
            if (id != klassen.Id)
            {
                return BadRequest();
            }

            _context.Entry(klassen).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!KlassenExists(id))
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

        // POST: api/Klassen
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Klassen>> PostKlassen(Klassen klassen)
        {
            klassen.Docent = await _context.Docenten.FindAsync(klassen.DocentId);

            _context.Klassen.Add(klassen);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetKlassen", new { id = klassen.Id }, klassen);
        }

        // POST api/Klassen/5/studenten
        [HttpPost("{klasId}/studenten")]
        public async Task<ActionResult<Klassen>> PostStudentKlassen(int klasId, [FromHeader] int studentId)
        {
            var klas = await _context.Klassen
                .Include(k => k.Studenten)
                .FirstOrDefaultAsync(k => k.Id == klasId);

            var student = await _context.Gebruikers.FindAsync(studentId);

            if(klas == null || student == null)
            {
                return NotFound();
            }

            if(!klas.Studenten.Contains(student))
            {
                klas.Studenten.Add(student);
            }
            
            await _context.SaveChangesAsync();

            return Ok();
        }

        // DELETE: api/Klassen/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteKlassen(int id)
        {
            var klassen = await _context.Klassen.FindAsync(id);
            if (klassen == null)
            {
                return NotFound();
            }

            _context.Klassen.Remove(klassen);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool KlassenExists(int id)
        {
            return _context.Klassen.Any(e => e.Id == id);
        }
    }
}
