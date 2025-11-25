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
    }
}