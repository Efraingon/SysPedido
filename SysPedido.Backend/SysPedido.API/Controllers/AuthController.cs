using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SysPedido.Application.DTOs;
using SysPedido.Application.Interfaces;
using SysPedido.Infrastructure.Data;

namespace SysPedido.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IJwtTokenGenerator _jwtGenerator;
    private readonly SysPedidoDbContext _context;

    public AuthController(IJwtTokenGenerator jwtGenerator, SysPedidoDbContext context)
    {
        _jwtGenerator = jwtGenerator;
        _context = context;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDto login)
    {
        try 
        {
            Console.WriteLine($"[AUTH] Intento de login para usuario: {login.Username}");
            
            var user = _context.Usuarios
                .Include(u => u.Empresa)
                .FirstOrDefault(u => u.Username.ToLower() == login.Username.ToLower());

            if (user == null)
            {
                Console.WriteLine($"[AUTH] Usuario no encontrado en la base de datos: {login.Username}");
                // Diagnóstico: listar todos los usuarios disponibles
                var allUsers = _context.Usuarios.Select(u => u.Username).ToList();
                Console.WriteLine($"[AUTH] Usuarios disponibles: {string.Join(", ", allUsers)}");
                return Unauthorized("Usuario no encontrado");
            }

            if (BCrypt.Net.BCrypt.Verify(login.Password, user.PasswordHash))
            {
                Console.WriteLine($"[AUTH] Login exitoso: {user.Username} (Empresa: {user.EmpresaId})");
                var empresaNombre = user.Empresa?.Nombre ?? $"Empresa {user.EmpresaId}";
                var token = _jwtGenerator.GenerateToken(user.Username, user.Rol, user.EmpresaId, empresaNombre);
                return Ok(new TokenDto { Token = token });
            }

            Console.WriteLine($"[AUTH] Contraseña incorrecta para el usuario: {user.Username}");
            return Unauthorized("Contraseña incorrecta");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AUTH ERROR] {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"[AUTH INNER ERROR] {ex.InnerException.Message}");
            }
            return StatusCode(500, ex.Message + (ex.InnerException != null ? " -> " + ex.InnerException.Message : ""));
        }
    }
}
