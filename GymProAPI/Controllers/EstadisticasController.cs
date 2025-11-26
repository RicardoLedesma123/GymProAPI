using GymProAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymProAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EstadisticasController : ControllerBase
    {
        private readonly GymDbContext _context;

        public EstadisticasController(GymDbContext context)
        {
            _context = context;
        }

        [HttpGet("bajas-por-mes")]
        public async Task<IActionResult> GetBajasPorMes()
        {
            var result = await _context.BajasPorMes
                .FromSqlRaw("EXEC GetBajasPorMes")
                .ToListAsync();

            return Ok(result);
        }

        [HttpGet("pagos-por-mes")]
        public async Task<IActionResult> GetPagosPorMes()
        {
            var result = await _context.PagosPorMes
                .FromSqlRaw("EXEC GetPagosPorMes")
                .ToListAsync();

            return Ok(result);
        }

        [HttpGet("socios-activos")]
        public async Task<IActionResult> GetSociosActivos()
        {
            var result = await _context.SociosActivos
                .FromSqlRaw("EXEC GetSociosActivos")
                .ToListAsync();

            return Ok(result);
        }

        [HttpGet("altas-por-mes")]
        public async Task<IActionResult> GetAltasPorMes()
        {
            var result = await _context.AltasPorMes
                .FromSqlRaw("EXEC GetAltasPorMes")
                .ToListAsync();

            return Ok(result);
        }
    }
}