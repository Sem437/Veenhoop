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
