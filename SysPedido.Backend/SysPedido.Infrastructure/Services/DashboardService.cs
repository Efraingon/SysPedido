using Microsoft.EntityFrameworkCore;
using SysPedido.Application.DTOs;
using SysPedido.Application.Interfaces;
using SysPedido.Infrastructure.Data;

namespace SysPedido.Infrastructure.Services;

public class DashboardService : IDashboardService
{
    private readonly SysPedidoDbContext _context;

    public DashboardService(SysPedidoDbContext context)
    {
        _context = context;
    }

    public async Task<ResumenDashboardDto> GetResumenAsync(DateTime? startDate, DateTime? endDate)
    {
        var query = _context.Pedidos.AsQueryable();

        if (startDate.HasValue)
            query = query.Where(p => p.Fecha >= startDate.Value);
        if (endDate.HasValue)
            query = query.Where(p => p.Fecha <= endDate.Value);

        var pedidos = await query
            .Include(p => p.Renglones)
                .ThenInclude(r => r.Producto)
                    .ThenInclude(p => p!.CamposMonedaExtranjera)
            .ToListAsync();

        var totalPedidos = pedidos.Count;
        var totalVentas = pedidos.Sum(p => p.Total);

        var ventasPorProducto = pedidos
            .SelectMany(p => p.Renglones)
            .GroupBy(r => r.Descripcion)
            .Select(g => new VentasPorProductoDto
            {
                NombreProducto = g.Key,
                TotalVendido = g.Sum(r => r.TotalRenglon)
            })
            .OrderByDescending(x => x.TotalVendido)
            .Take(5)
            .ToList();

        var pedidosPorDia = pedidos
            .GroupBy(p => p.Fecha.Date)
            .Select(g => new PedidosPorDiaDto
            {
                Fecha = g.Key.ToString("yyyy-MM-dd"),
                CantidadPedidos = g.Count()
            })
            .OrderBy(x => x.Fecha)
            .ToList();

        var distribucionCategoria = pedidos
            .SelectMany(p => p.Renglones)
            .Where(r => r.Producto != null)
            .GroupBy(r => string.IsNullOrEmpty(r.Producto!.Categoria) ? "Sin Categoría" : r.Producto.Categoria)
            .Select(g => new DistribucionCategoriaDto
            {
                Categoria = g.Key,
                Cantidad = g.Count()
            })
            .ToList();

        return new ResumenDashboardDto
        {
            TotalPedidos = totalPedidos,
            TotalVentas = totalVentas,
            VentasPorProducto = ventasPorProducto,
            PedidosPorDia = pedidosPorDia,
            DistribucionCategoria = distribucionCategoria
        };
    }
}
