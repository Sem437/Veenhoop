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
    public class CijfersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CijfersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Cijfers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cijfers>>> GetCijfers()
        {
            return await _context.Cijfers.ToListAsync();
        }

        // GET: api/Cijfers/user/5
        [HttpGet("user/{gebruikersId}")]
        public async Task<ActionResult> GetUserCijfer(int gebruikersId)
        {                       
            var data = await _context.Cijfers
                .Join(_context.Toetsen,
                    cijfer => cijfer.ToetsId,
                    toest => toest.Id,
                    (cijfer, toest) => new { cijfer, toest })
                .Join(_context.Vakken,
                    ct => ct.toest.VakId,
                    vak => vak.Id,
                    (ct, vak) => new { ct.cijfer, ct.toest, vak })
                .Where(x => x.cijfer.GebruikersId == gebruikersId)
                .ToListAsync();

            var grouped = data
                .GroupBy(x => x.vak.VakNaam)
                .Select(g => new
                {
                    Vaknaam = g.Key,
                    Periode1 = g.Where(x => x.cijfer.Periode == 1).Select(x => (double?)x.cijfer.Cijfer).FirstOrDefault(),
                    GemPeriode1 = g.Where(x => x.cijfer.Periode == 1).Select(x => (double)x.cijfer.Cijfer).DefaultIfEmpty().Average(),
                    Periode2 = g.Where(x => x.cijfer.Periode == 2).Select(x => (double?)x.cijfer.Cijfer).FirstOrDefault(),
                    GemPeriode2 = g.Where(x => x.cijfer.Periode == 2).Select(x => (double)x.cijfer.Cijfer).DefaultIfEmpty().Average(),
                    Periode3 = g.Where(x => x.cijfer.Periode == 3).Select(x => (double?)x.cijfer.Cijfer).FirstOrDefault(),
                    GemPeriode3 = g.Where(x => x.cijfer.Periode == 3).Select(x => (double)x.cijfer.Cijfer).DefaultIfEmpty().Average(),
                    Periode4 = g.Where(x => x.cijfer.Periode == 4).Select(x => (double?)x.cijfer.Cijfer).FirstOrDefault(),
                    GemPeriode4 = g.Where(x => x.cijfer.Periode == 4).Select(x => (double)x.cijfer.Cijfer).DefaultIfEmpty().Average(),
                    Gem = g.Select(x => (double)x.cijfer.Cijfer).DefaultIfEmpty().Average()
                }).ToList();

            return Ok(grouped);
            
        }

        // POST: api/Cijfers/
        [HttpPost("klasCijfers/{docentId}")]
        public IActionResult CijferKlas(int docentId, [FromBody] List<Cijfers> cijfers)
        {
            if (cijfers == null || cijfers.Count == 0)
            {
                return BadRequest("No data provided.");
            }
            foreach (var cijfer in cijfers)
            {
                var docentCheck = _context.DocentVakken
                    .FirstOrDefault(c => c.DocentId == docentId)
                    .ToString();
                
                if(docentCheck == null)
                {
                    return BadRequest("Docent not found.");
                }

                _context.Cijfers.Add(cijfer);
            }
            _context.SaveChanges();
            return Ok(new { message = "Cijfers successfully added." });
        }



        // GET: api/Cijfers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Cijfers>> GetCijfers(int id)
        {
            var cijfers = await _context.Cijfers.FindAsync(id);

            if (cijfers == null)
            {
                return NotFound();
            }

            return cijfers;
        }

        // PUT: api/Cijfers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCijfers(int id, Cijfers cijfers)
        {
            if (id != cijfers.Id)
            {
                return BadRequest();
            }

            _context.Entry(cijfers).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CijfersExists(id))
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

        // POST: api/Cijfers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Cijfers>> PostCijfers(Cijfers cijfers)
        {
            _context.Cijfers.Add(cijfers);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCijfers", new { id = cijfers.Id }, cijfers);
        }

        // DELETE: api/Cijfers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCijfers(int id)
        {
            var cijfers = await _context.Cijfers.FindAsync(id);
            if (cijfers == null)
            {
                return NotFound();
            }

            _context.Cijfers.Remove(cijfers);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CijfersExists(int id)
        {
            return _context.Cijfers.Any(e => e.Id == id);
        }
    }
}
