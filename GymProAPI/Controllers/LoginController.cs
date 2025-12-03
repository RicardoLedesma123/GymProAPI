using GymProAPI.Data;
using GymProAPI.Models;
using GymProAPI.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
namespace GymProAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly GymDbContext _context;
        private readonly JwtService _jwtService;

        public AuthController(GymDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Login dto)
        {
            // Buscar el usuario por Username
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);

            // Validar existencia y contraseña
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized("Credenciales inválidas");

            // Generar token usando el objeto User
            var token = _jwtService.GenerateToken(user);

            // Devolver token al cliente
            return Ok(new { token });
        }


    }
}