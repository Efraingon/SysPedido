using SysPedido.Domain.Entities;

namespace SysPedido.Application.Interfaces;

public interface IClienteService
{
    Task<IEnumerable<Cliente>> GetAllAsync();
}

public interface IProductoService
{
    Task<IEnumerable<Producto>> GetAllAsync();
}
