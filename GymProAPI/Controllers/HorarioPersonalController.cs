using GymProAPI.Data;
using GymProAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymProAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HorarioPersonalController : ControllerBase
    {
        private readonly GymDbContext _context;

        public HorarioPersonalController(GymDbContext context)
        {
            _context = context;
        }

        [HttpPost("horarios")]
        public async Task<IActionResult> CrearHorario([FromBody] HorarioPersonal horario)
        {
            _context.HorarioPersonal.Add(horario);
            await _context.SaveChangesAsync();
            return Ok(horario);
        }

        [HttpGet("horarios")]
        public async Task<IActionResult> ObtenerHorarios()
        {
            var horarios = await _context.HorarioPersonal.ToListAsync();
            return Ok(horarios);
        }
    }
}