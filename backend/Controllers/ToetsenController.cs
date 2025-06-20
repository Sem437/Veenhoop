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
    public class ToetsenController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ToetsenController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Toetsen
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Toetsen>>> GetToetsen()
        {
            return await _context.Toetsen.ToListAsync();
        }

        // GET: api/Toetsen/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Toetsen>> GetToetsen(int id)
        {
            var toetsen = await _context.Toetsen.FindAsync(id);

            if (toetsen == null)
            {
                return NotFound();
            }

            return toetsen;
        }

        // GET: api/Toetsen/vakken/5
        [HttpGet("vakken/{vakId}")]
        public async Task<ActionResult<List<Toetsen>>> GetToetsenVakken(int vakId)
        {
            var toetsen = await _context.Toetsen
                .Where(t => t.VakId == vakId)
                .ToListAsync();

            if (toetsen == null || toetsen.Count == 0)
            {
                return NotFound();
            }

            return toetsen;
        }

        // PUT: api/Toetsen/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutToetsen(int id, Toetsen toetsen)
        {
            if (id != toetsen.Id)
            {
                return BadRequest();
            }

            _context.Entry(toetsen).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ToetsenExists(id))
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

        // POST: api/Toetsen
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Toetsen>> PostToetsen(Toetsen toetsen)
        {
            _context.Toetsen.Add(toetsen);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetToetsen", new { id = toetsen.Id }, toetsen);
        }

        // DELETE: api/Toetsen/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteToetsen(int id)
        {
            var toetsen = await _context.Toetsen.FindAsync(id);
            if (toetsen == null)
            {
                return NotFound();
            }

            _context.Toetsen.Remove(toetsen);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ToetsenExists(int id)
        {
            return _context.Toetsen.Any(e => e.Id == id);
        }
    }
}
