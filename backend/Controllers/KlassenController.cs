using System;
using System.Collections.Generic;
using System.Linq;
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
    public class KlassenController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public KlassenController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Klassen
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Klassen>>> GetKlassen()
        {
            return await _context.Klassen.ToListAsync();
        }

        // GET: api/Klassen/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Klassen>> GetKlassen(int id)
        {
            var klassen = await _context.Klassen.FindAsync(id);

            if (klassen == null)
            {
                return NotFound();
            }

            return klassen;
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
