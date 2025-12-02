using GymProAPI.Data;
using GymProAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymProAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventarioController : ControllerBase
    {
        private readonly GymDbContext _context;

        public InventarioController(GymDbContext context)
        {
            _context = context;
        }

        // Listar productos
        [HttpGet("productos")]
        public async Task<IActionResult> GetProductos()
        {
            var productos = await _context.Productos.ToListAsync();
            return Ok(productos);
        }

        // Agregar producto
        [HttpPost("productos")]
        public async Task<IActionResult> CrearProducto([FromBody] Producto producto)
        {
            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();
            return Ok(producto);
        }

        // Registrar movimiento
        [HttpPost("movimientos")]
        public async Task<IActionResult> RegistrarMovimiento([FromBody] MovimientoInventario movimiento)
        {
            var producto = await _context.Productos.FindAsync(movimiento.ProductoID);
            if (producto == null) return NotFound("Producto no encontrado");

            if (movimiento.TipoMovimiento == "Salida" && producto.StockActual < movimiento.Cantidad)
                return BadRequest("Stock insuficiente");

            producto.StockActual += (movimiento.TipoMovimiento == "Entrada"
                ? movimiento.Cantidad
                : -movimiento.Cantidad);

            movimiento.FechaMovimiento = DateTime.Now;

            _context.MovimientosInventario.Add(movimiento);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Movimiento registrado correctamente",
                movimiento
            });
        }

        // Historial de movimientos
        [HttpGet("movimientos")]
        public async Task<IActionResult> ObtenerMovimientos([FromQuery] int? productoID)
        {
            IQueryable<MovimientoInventario> query = _context.MovimientosInventario
                .Include(m => m.Producto); // opcional, si quieres datos del producto

            if (productoID.HasValue)
            {
                query = query.Where(m => m.ProductoID == productoID.Value);
            }

            var movimientos = await query
                .OrderByDescending(m => m.FechaMovimiento)
                .ToListAsync();

            return Ok(movimientos);
        }
    }
}
