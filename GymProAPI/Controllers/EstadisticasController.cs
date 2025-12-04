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

        [HttpGet("ganancia-productos")]
        public async Task<IActionResult> GetGananciaProductos()
        {
            var resultado = await _context.MovimientosInventario
                .Include(m => m.Producto)
                .Where(m => m.TipoMovimiento == "Salida")
                .GroupBy(m => new { Mes = m.FechaMovimiento.Month, Año = m.FechaMovimiento.Year })
                .Select(g => new {
                    Mes = g.Key.Mes,
                    Año = g.Key.Año,
                    GananciaNeta = g.Sum(m =>
                        (m.Producto.PrecioVenta - m.Producto.PrecioCompra) * m.Cantidad
                    )
                })
                .OrderBy(r => r.Año).ThenBy(r => r.Mes)
                .ToListAsync();

            return Ok(resultado);
        }

        [HttpGet("ingresos-visitas")]
        public async Task<IActionResult> GetIngresosVisitas()
        {
            var resultado = await _context.Visitas
                .GroupBy(v => new { Mes = v.FechaVisita.Month, Año = v.FechaVisita.Year })
                .Select(g => new {
                    Mes = g.Key.Mes,
                    Año = g.Key.Año,
                    TotalRecaudado = g.Sum(v => v.TotalRecaudado)
                })
                .OrderBy(r => r.Año).ThenBy(r => r.Mes)
                .ToListAsync();

            return Ok(resultado);
        }
    }
}