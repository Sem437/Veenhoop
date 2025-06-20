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
    public class DocentVakkenController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DocentVakkenController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/DocentVakken
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DocentVakken>>> GetDocentVakken()
        {
            return await _context.DocentVakken.ToListAsync();
        }

        // GET: api/DocentVakken/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DocentVakken>> GetDocentVakken(int id)
        {
            var docentVakken = await _context.DocentVakken.FindAsync(id);

            if (docentVakken == null)
            {
                return NotFound();
            }

            return docentVakken;
        }

        // GET: api/DocentVakken/Dto 
        [HttpGet("DocentenOverzicht")]
        public async Task<ActionResult<List<DocentVakkenDto>>> GetDocentVakkenDto()
        {
            var docentVakken = await _context.DocentVakken.ToListAsync();            
            var docentVakkenDtoList = new List<DocentVakkenDto>();

            foreach (var item in docentVakken)
            {
                var vak = await _context.Vakken.FindAsync(item.VakId);
                var docent = await _context.Docenten.FindAsync(item.DocentId);
                var klas = await _context.Klassen.FindAsync(item.KlasId);

                if(vak == null || docent == null || klas == null)
                {
                    return NotFound("Vak, docent of klas is null");
                }
                
                var Dto = new DocentVakkenDto
                {
                    id = item.Id,
                    vakId = item.VakId,
                    docentId = item.DocentId,
                    klasId = item.KlasId,
                    vakNaam = vak.VakNaam,
                    docentNaam = docent.Voornaam + " " + (docent.Tussenvoegsel != null ? docent.Tussenvoegsel : string.Empty) + " " + docent.Achternaam,
                    klasNaam = klas.KlasNaam
                };
                docentVakkenDtoList.Add(Dto);
            }

            return Ok(docentVakkenDtoList);
        }

        // PUT: api/DocentVakken/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDocentVakken(int id, DocentVakken docentVakken)
        {
            if (id != docentVakken.Id)
            {
                return BadRequest();
            }

            _context.Entry(docentVakken).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DocentVakkenExists(id))
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

        // POST: api/DocentVakken
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<DocentVakken>> PostDocentVakken(DocentVakken docentVakken)
        {
            _context.DocentVakken.Add(docentVakken);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDocentVakken", new { id = docentVakken.Id }, docentVakken);
        }

        // DELETE: api/DocentVakken/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDocentVakken(int id)
        {
            var docentVakken = await _context.DocentVakken.FindAsync(id);
            if (docentVakken == null)
            {
                return NotFound();
            }

            _context.DocentVakken.Remove(docentVakken);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DocentVakkenExists(int id)
        {
            return _context.DocentVakken.Any(e => e.Id == id);
        }
    }
}
