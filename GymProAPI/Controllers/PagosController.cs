using System.ComponentModel;
using GymProAPI.Data;
using GymProAPI.Models;
using GymProAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymProAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PagosController : ControllerBase
    {
        private readonly GymDbContext _context;
        private readonly EmailService _emailService;


        public PagosController(GymDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [HttpGet("{socioID}")]
        public async Task<ActionResult<IEnumerable<Pago>>> GetPagosPorSocio(int socioID)
        {
            var socio = await _context.Socios.FindAsync(socioID);
            if (socio == null)
                return NotFound($"No existe el socio con ID {socioID}");

            var pagos = await _context.Pagos
                .Where(p => p.SocioID == socioID)
                .OrderByDescending(p => p.FechaPago)
                .ToListAsync();

            return pagos;
        }

        [HttpPost("{socioID}")]
        public async Task<ActionResult<Pago>> RegistrarPago(int socioID, Pago pago)
        {
            var socio = await _context.Socios.FirstOrDefaultAsync(s => s.SocioID == socioID);
            if (socio == null)
                return NotFound($"No existe el socio con ID {socioID}");

            // Guardar el pago
            pago.SocioID = socioID;
            pago.FechaRegistro = DateTime.Now;
            _context.Pagos.Add(pago);

            // Calcular próxima fecha de pago (día de registro, mes del pago + 1, año en curso)
            socio.FechaPago = CalcularProximaDesdeRegYMesPago(socio.FechaRegistro, pago.FechaPago);
            socio.AlCorriente = true;
            _context.Entry(socio).State = EntityState.Modified;

            // Guardar TODO en un solo viaje a la base
            await _context.SaveChangesAsync();

            // Generar HTML del comprobante
            var cuerpoHtml = $@"
<!DOCTYPE html>
<html>
<head>
  <meta charset='UTF-8'>
  <style>
    body {{
      font-family: Arial, sans-serif;
      background-color: #f4f4f4;
      padding: 20px;
    }}
    .card {{
      background-color: #fff;
      border-radius: 8px;
      padding: 20px;
      box-shadow: 0 2px 6px rgba(0,0,0,0.15);
      max-width: 600px;
      margin: auto;
    }}
    h2 {{
      color: #2c3e50;
      text-align: center;
    }}
    .info p {{
      margin: 5px 0;
      font-size: 14px;
    }}
    .footer {{
      text-align: center;
      font-size: 12px;
      color: #888;
      margin-top: 20px;
    }}
  </style>
</head>
<body>
  <div class='card'>
    <h2>Comprobante de Pago</h2>
    <div class='info'>
      <p><strong>SocioID:</strong> {socio.SocioID}</p>
      <p><strong>Nombre:</strong> {socio.Nombre} {socio.ApellidoPaterno} {socio.ApellidoMaterno}</p>
      <p><strong>Email:</strong> {socio.Email}</p>
      <p><strong>Fecha de Pago:</strong> {pago.FechaPago:dd/MM/yyyy}</p>
      <p><strong>Monto:</strong> {pago.Monto:C}</p>
      <p><strong>Método:</strong> {pago.MetodoPago}</p>
      <p><strong>Próxima Fecha de Pago:</strong> {socio.FechaPago:dd/MM/yyyy}</p>
    </div>
    <hr/>
    <p>Gracias por tu pago. ¡Sigue entrenando con nosotros!</p>
    <div class='footer'>
      © {DateTime.Now.Year} GymPro - Todos los derechos reservados
    </div>
  </div>
</body>
</html>";

            // Enviar correo en segundo plano para no bloquear la respuesta
            _ = Task.Run(() =>
                _emailService.EnviarTicket(
                    socio.Email,
                    "Comprobante de Pago - GymPro",
                    cuerpoHtml
                )
            );

            return CreatedAtAction(nameof(GetPagosPorSocio), new { socioID }, pago);
        }



        private DateTime CalcularProximaDesdeRegYMesPago(DateTime fechaRegistroSocio, DateTime fechaPagoActual)
        {
            int dia = fechaRegistroSocio.Day;

            // Mes: el mes del pago + 1 (1-12), con rollover
            int mes = fechaPagoActual.Month + 1;
            int anio = DateTime.Today.Year;
            if (mes > 12)
            {
                mes = 1;
                anio += 1;
            }

            int diasEnMes = DateTime.DaysInMonth(anio, mes);
            int diaAjustado = Math.Min(dia, diasEnMes);

            return new DateTime(anio, mes, diaAjustado);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pago>>> GetTodosLosPagos()
        {
            return await _context.Pagos
                .Include(p => p.Socio)
                .OrderByDescending(p => p.FechaPago)
                .ToListAsync();
        }

        [HttpDelete("{pagoID}")]
        public async Task<IActionResult> DeletePago(int pagoID)
        {
            var pago = await _context.Pagos.FindAsync(pagoID);
            if (pago == null)
                return NotFound();

            _context.Pagos.Remove(pago);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("visitas")]
        public async Task<IActionResult> RegistrarVisita([FromBody] Visita visita)
        {
            _context.Visitas.Add(visita);
            await _context.SaveChangesAsync();
            return Ok(visita);
        }

        [HttpGet("visitas")]
        public async Task<IActionResult> ObtenerVisitas()
        {
            var visitas = await _context.Visitas
                .OrderByDescending(v => v.FechaVisita)
                .ToListAsync();

            return Ok(visitas);
        }

    }
}