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
    public class VakkenController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public VakkenController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Vakken
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Vakken>>> GetVakken()
        {
            return await _context.Vakken.ToListAsync();
        }

        // GET: api/Vakken/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Vakken>> GetVakken(int id)
        {
            var vakken = await _context.Vakken.FindAsync(id);

            if (vakken == null)
            {
                return NotFound();
            }

            return vakken;
        }

        // PUT: api/Vakken/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVakken(int id, Vakken vakken)
        {
            if (id != vakken.Id)
            {
                return BadRequest();
            }

            _context.Entry(vakken).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VakkenExists(id))
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

        // POST: api/Vakken
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Vakken>> PostVakken(Vakken vakken)
        {
            _context.Vakken.Add(vakken);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetVakken", new { id = vakken.Id }, vakken);
        }

        // DELETE: api/Vakken/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVakken(int id)
        {
            var vakken = await _context.Vakken.FindAsync(id);
            if (vakken == null)
            {
                return NotFound();
            }

            _context.Vakken.Remove(vakken);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool VakkenExists(int id)
        {
            return _context.Vakken.Any(e => e.Id == id);
        }
    }
}
