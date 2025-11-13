using GymProAPI.Data;
using GymProAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymProAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SociosController : ControllerBase
    {
        private readonly GymDbContext _context;

        public SociosController(GymDbContext context)
        {
            _context = context;
        }

        // ✅ Utilidad privada para evaluar si el socio va al corriente
        private void EvaluarEstadoDePago(Socio socio)
        {
            socio.AlCorriente = socio.FechaPago >= DateTime.Today;
        }

        // ✅ GET: api/socios (solo activos)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Socio>>> GetSocios()
        {
            try
            {
                var socios = await _context.Socios
                    .Where(s => s.ActivoInactivo)
                    .ToListAsync();

                socios.ForEach(EvaluarEstadoDePago);
                return socios;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"🔥 ERROR en GetSocios: {ex.Message}");
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        // ✅ GET: api/socios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Socio>> GetSocio(int id)
        {
            var socio = await _context.Socios.FindAsync(id);
            if (socio == null)
                return NotFound();

            EvaluarEstadoDePago(socio);
            return socio;
        }

        // ✅ GET: api/socios/todos (activos + inactivos)
        [HttpGet("todos")]
        public async Task<ActionResult<IEnumerable<Socio>>> GetTodosLosSocios()
        {
            try
            {
                var socios = await _context.Socios.ToListAsync();
                socios.ForEach(EvaluarEstadoDePago);
                return socios;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"🔥 ERROR en GetTodosLosSocios: {ex.Message}");
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Socio>> PostSocio([FromForm] Socio socio, IFormFile? foto)
        {
            socio.FechaRegistro = DateTime.Now;
            socio.FechaPago = socio.FechaRegistro.AddMonths(1);
            socio.ActivoInactivo = true;

            EvaluarEstadoDePago(socio);

            // Guardar la imagen si fue enviada
            if (foto != null && foto.Length > 0)
            {
                var nombreArchivo = $"{Guid.NewGuid()}_{Path.GetFileName(foto.FileName)}";
                var rutaCarpeta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "fotos");

                if (!Directory.Exists(rutaCarpeta))
                    Directory.CreateDirectory(rutaCarpeta);

                var rutaCompleta = Path.Combine(rutaCarpeta, nombreArchivo);

                using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                {
                    await foto.CopyToAsync(stream);
                }

                socio.FotoUrl = $"/fotos/{nombreArchivo}";
            }

            _context.Socios.Add(socio);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSocio), new { id = socio.SocioID }, socio);
        }

        // ✅ PUT: api/socios/5 (actualización)
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSocio(int id, Socio socio)
        {
            if (id != socio.SocioID)
                return BadRequest("ID de socio no coincide.");

            EvaluarEstadoDePago(socio);
            _context.Entry(socio).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SocioExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // ✅ DELETE: api/socios/5 (deshabilitar)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSocio(int id)
        {
            var socio = await _context.Socios.FindAsync(id);
            if (socio == null)
                return NotFound();

            socio.ActivoInactivo = false;
            socio.FechaBaja = DateTime.Now;

            EvaluarEstadoDePago(socio);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ✅ Utilidad privada para verificar existencia
        private bool SocioExists(int id)
        {
            return _context.Socios.Any(e => e.SocioID == id);
        }
    }
}