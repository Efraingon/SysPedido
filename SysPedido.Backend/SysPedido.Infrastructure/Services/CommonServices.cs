using Microsoft.EntityFrameworkCore;
using SysPedido.Application.Interfaces;
using SysPedido.Domain.Entities;
using SysPedido.Infrastructure.Data;

namespace SysPedido.Infrastructure.Services;

public class ClienteService : IClienteService
{
    private readonly SysPedidoDbContext _context;
    public ClienteService(SysPedidoDbContext context) => _context = context;
    public async Task<IEnumerable<Cliente>> GetAllAsync() => await _context.Clientes.ToListAsync();
}

public class ProductoService : IProductoService
{
    private readonly SysPedidoDbContext _context;
    public ProductoService(SysPedidoDbContext context) => _context = context;
    public async Task<IEnumerable<Producto>> GetAllAsync() => 
        await _context.Productos
            .Include(p => p.CamposMonedaExtranjera)
            .ToListAsync();
}
