namespace SysPedido.Application.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(string username, string role, int empresaId, string empresaNombre);
}
