using Microsoft.EntityFrameworkCore;
using SysPedido.Application.Interfaces;
using SysPedido.Domain.Entities;
using SysPedido.Infrastructure.Data;

namespace SysPedido.Infrastructure.Repositories;

public class PedidoRepository : IPedidoRepository
{
    private readonly SysPedidoDbContext _context;

    public PedidoRepository(SysPedidoDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Pedido>> GetAllAsync()
    {
        return await _context.Pedidos
            .Include(p => p.Renglones)
            .Include(p => p.Cliente)
            .OrderByDescending(p => p.Fecha)
            .ToListAsync();
    }

    /// <summary>
    /// Lee cotizaciones del ERP legacy (dbo.cotizacion) filtradas por tenant.
    /// Se combinan con los pedidos nuevos en PedidoService.GetAllAsync().
    /// </summary>
    public async Task<IEnumerable<Cotizacion>> GetAllCotizacionesAsync()
    {
        return await _context.Cotizaciones
            .Include(c => c.Renglones)
            .Include(c => c.Cliente)
            .OrderByDescending(c => c.Fecha)
            .ToListAsync();
    }

    public async Task<Pedido?> GetByIdAsync(string numero)
    {
        return await _context.Pedidos
            .Include(p => p.Renglones)
            .FirstOrDefaultAsync(p => p.Numero == numero);
    }

    public async Task AddAsync(Pedido pedido)
    {
        await _context.Pedidos.AddAsync(pedido);
    }

    public void Update(Pedido pedido)
    {
        _context.Pedidos.Update(pedido);
    }

    public void Delete(Pedido pedido)
    {
        _context.Pedidos.Remove(pedido);
    }

    public async Task<string> GetNextNumeroAsync(int empresaId)
    {
        int yearTwoDigits = DateTime.Now.Year % 100;
        string prefijo = $"C{yearTwoDigits}-";

        // Consultamos el máximo global para evitar violaciones de PK (Numero es PK único)
        var maxLegacy = await _context.Cotizaciones
            .IgnoreQueryFilters()
            .Where(c => c.Numero.StartsWith(prefijo))
            .Select(c => c.Numero)
            .MaxAsync();

        var maxNew = await _context.Pedidos
            .IgnoreQueryFilters()
            .Where(p => p.Numero.StartsWith(prefijo))
            .Select(p => p.Numero)
            .MaxAsync();

        string? maxActual = null;
        if (maxLegacy != null && maxNew != null)
            maxActual = string.Compare(maxLegacy, maxNew, StringComparison.OrdinalIgnoreCase) > 0 ? maxLegacy : maxNew;
        else
            maxActual = maxLegacy ?? maxNew;

        int nextValue = 1;
        if (!string.IsNullOrEmpty(maxActual))
        {
            string numericPart = maxActual.Replace(prefijo, "");
            if (int.TryParse(numericPart, out int currentMax))
            {
                nextValue = currentMax + 1;
            }
        }

        return $"{prefijo}{nextValue.ToString().PadLeft(5, '0')}";
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
