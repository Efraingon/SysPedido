using Microsoft.IdentityModel.Tokens;
using SysPedido.Application.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SysPedido.CrossCutting.Security;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    // The key should ideally be in configuration
    private readonly string _secret = "EstaEsUnaLLaveMuySecretaYDebeSerLarga12345!";

    public string GenerateToken(string username, string role, int empresaId, string empresaNombre)
    {
        var handler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_secret);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role),
                new Claim("EmpresaId", empresaId.ToString()),
                new Claim("EmpresaNombre", empresaNombre)
            }),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = handler.CreateToken(tokenDescriptor);
        return handler.WriteToken(token);
    }
}
