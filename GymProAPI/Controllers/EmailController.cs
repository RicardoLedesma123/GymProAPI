using GymProAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace GymProAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly EmailService _emailService;

        public EmailController(EmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpGet("Test")]
        public IActionResult Test()
        {
            try
            {
                _emailService.EnviarTicket(
                    "tavarezfaby58@gmail.com", // ← aquí pon tu correo real
                    "Prueba de correo desde GymPro",
                    "<h1>¡Funciona el envío de correos!</h1><p>Este es un test SMTP.</p>"
                );

                return Ok("Correo enviado correctamente");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al enviar correo: {ex.Message}");
            }
        }
    }
}