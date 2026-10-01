using SysPedido.Application.DTOs;

namespace SysPedido.Application.Interfaces;

public interface IPedidoService
{
    Task<IEnumerable<PedidoDto>> GetAllAsync();
    Task<PedidoDto?> GetByIdAsync(string numero);
    Task<PedidoDto> CreateAsync(PedidoDto dto);
    Task<bool> UpdateAsync(string numero, PedidoDto dto);
    Task<bool> DeleteAsync(string numero);
}
