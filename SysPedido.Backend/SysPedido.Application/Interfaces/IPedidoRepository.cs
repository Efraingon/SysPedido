using SysPedido.Domain.Entities;

namespace SysPedido.Application.Interfaces;

public interface IPedidoRepository
{
    Task<IEnumerable<Pedido>> GetAllAsync();
    Task<IEnumerable<Cotizacion>> GetAllCotizacionesAsync();
    Task<Pedido?> GetByIdAsync(string numero);
    Task AddAsync(Pedido pedido);
    void Update(Pedido pedido);
    void Delete(Pedido pedido);
    Task<string> GetNextNumeroAsync(int empresaId);
    Task SaveChangesAsync();
}
