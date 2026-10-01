using SysPedido.Application.DTOs;
using SysPedido.Application.Interfaces;
using SysPedido.Domain.Entities;

namespace SysPedido.Application.Services;

public class PedidoService : IPedidoService
{
    private readonly IPedidoRepository _repository;
    private readonly IEventBus _eventBus;
    private readonly IPedidoNotificationService _notificationService;

    public PedidoService(IPedidoRepository repository, IEventBus eventBus, IPedidoNotificationService notificationService)
    {
        _repository = repository;
        _eventBus = eventBus;
        _notificationService = notificationService;
    }

    public async Task<IEnumerable<PedidoDto>> GetAllAsync()
    {
        // Pedidos nuevos creados desde la app (PedidoApp.Pedido)
        var pedidos = await _repository.GetAllAsync();

        // Cotizaciones legacy del ERP (dbo.cotizacion) — fuente principal de datos históricos
        var cotizaciones = await _repository.GetAllCotizacionesAsync();

        var fromNew = pedidos.Select(MapToDto);
        var fromLegacy = cotizaciones.Select(MapCotizacionToDto);

        return fromNew
            .Concat(fromLegacy)
            .OrderByDescending(p => p.Fecha);
    }

    public async Task<PedidoDto?> GetByIdAsync(string numero)
    {
        var pedido = await _repository.GetByIdAsync(numero);
        return pedido == null ? null : MapToDto(pedido);
    }

    public async Task<PedidoDto> CreateAsync(PedidoDto dto)
    {
        var numero = string.IsNullOrEmpty(dto.Numero) 
            ? await _repository.GetNextNumeroAsync(dto.EmpresaId) 
            : dto.Numero;

        var pedido = new Pedido
        {
            Numero = numero,
            Fecha = DateTime.UtcNow,
            CodigoCliente = dto.CodigoCliente,
            CodigoVendedor = dto.CodigoVendedor,
            ConsecutivoVendedor = dto.ConsecutivoVendedor,
            Observaciones = dto.Observaciones,
            Total = dto.Total,
            Moneda = dto.Moneda,
            CodigoMoneda = dto.CodigoMoneda,
            Latitud = dto.Latitud,
            Longitud = dto.Longitud,
            Renglones = dto.Renglones.Select((r, index) => new RenglonPedido
            {
                NumeroCotizacion = numero,
                ConsecutivoRenglon = index + 1,
                CodigoArticulo = r.CodigoArticulo,
                Descripcion = r.Descripcion,
                Cantidad = r.Cantidad,
                PrecioSinIVA = r.PrecioSinIVA,
                PrecioConIVA = r.PrecioConIVA,
                TotalRenglon = r.TotalRenglon,
                EmpresaId = dto.EmpresaId
            }).ToList()
        };

        await _repository.AddAsync(pedido);
        await _repository.SaveChangesAsync();

        // Publish event
        await _eventBus.PublishAsync(new PedidoCreadoEvent { NumeroPedido = pedido.Numero, FechaCreacion = pedido.Fecha });

        // SignalR Push Notification
        await _notificationService.NotifyPedidoCreadoAsync(pedido.Numero, pedido.Total, pedido.CodigoVendedor, pedido.EmpresaId);

        return MapToDto(pedido);
    }

    public async Task<bool> UpdateAsync(string numero, PedidoDto dto)
    {
        var pedido = await _repository.GetByIdAsync(numero);
        if (pedido == null) return false;

        pedido.Observaciones = dto.Observaciones;
        pedido.Total = dto.Total;
        pedido.Moneda = dto.Moneda;
        pedido.CodigoMoneda = dto.CodigoMoneda;

        _repository.Update(pedido);
        await _repository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(string numero)
    {
        var pedido = await _repository.GetByIdAsync(numero);
        if (pedido == null) return false;

        _repository.Delete(pedido);
        await _repository.SaveChangesAsync();
        return true;
    }

    // --- Mapeo: Pedido nuevo (PedidoApp) → DTO ---
    private static PedidoDto MapToDto(Pedido p) => new()
    {
        Numero = p.Numero,
        Fecha = p.Fecha,
        CodigoCliente = p.CodigoCliente,
        NombreCliente = p.Cliente?.Nombre ?? "",
        CodigoVendedor = p.CodigoVendedor,
        ConsecutivoVendedor = p.ConsecutivoVendedor,
        Observaciones = p.Observaciones,
        Total = p.Total,
        Moneda = p.Moneda,
        CodigoMoneda = p.CodigoMoneda,
        Latitud = p.Latitud,
        Longitud = p.Longitud,
        EmpresaId = p.EmpresaId,
        CantidadItems = p.Renglones?.Count ?? 0,
        Renglones = p.Renglones?.Select(r => new RenglonPedidoDto
        {
            ConsecutivoRenglon = r.ConsecutivoRenglon,
            CodigoArticulo = r.CodigoArticulo,
            Descripcion = r.Descripcion,
            Cantidad = r.Cantidad,
            PrecioSinIVA = r.PrecioSinIVA,
            PrecioConIVA = r.PrecioConIVA,
            TotalRenglon = r.TotalRenglon
        }).ToList() ?? new()
    };

    // --- Mapeo: Cotizacion legacy (dbo.cotizacion) → DTO ---
    private static PedidoDto MapCotizacionToDto(Cotizacion c) => new()
    {
        Numero = c.Numero,
        Fecha = c.Fecha,
        CodigoCliente = c.CodigoCliente,
        NombreCliente = c.Cliente?.Nombre ?? "",
        CodigoVendedor = c.CodigoVendedor,
        ConsecutivoVendedor = c.ConsecutivoVendedor,
        Observaciones = c.Observaciones ?? "",
        Exento = c.Exento,
        Base = c.Base,
        IVA = c.IVA,
        Total = c.Total,
        Moneda = c.Moneda ?? "",
        CodigoMoneda = c.CodigoMoneda ?? "",
        EmpresaId = c.EmpresaId,
        CantidadItems = c.Renglones?.Count ?? 0,
        Renglones = c.Renglones?.Select(r => new RenglonPedidoDto
        {
            ConsecutivoRenglon = r.ConsecutivoRenglon,
            CodigoArticulo = r.CodigoArticulo,
            Descripcion = r.Descripcion,
            Cantidad = r.Cantidad,
            PrecioSinIVA = r.PrecioSinIVA,
            PrecioConIVA = r.PrecioConIVA,
            TotalRenglon = r.TotalRenglon
        }).ToList() ?? new()
    };
}

