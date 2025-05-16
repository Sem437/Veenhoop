using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Veenhoop.Data;
using Veenhoop.Models;
using Veenhoop.Dto;

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

            //var result = klassen.Select(k => new KlasStudentDto
            //{
            //    KlasId = k.Id,
            //    KlasNaam = k.KlasNaam,
            //    vakId = 2,
            //    VakNaam = k.KlasNaam,
            //    Studenten = k.Studenten.Select(s => new StudentDto
            //    {
            //        Id = s.Id,
            //        Voornaam = s.Voornaam,
            //        Tussenvoegsel = s.Tussenvoegsel,
            //        Achternaam = s.Achternaam
            //    }).ToList()
            //}).ToList();

            var result = docentVakken.Select(dv =>
            {
                var klas = klassen.FirstOrDefault(k => k.Id == dv.KlasId);
                var vak = vakken.FirstOrDefault(v => v.Id == dv.VakId);

                return new KlasStudentDto
                {
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
    }
}
