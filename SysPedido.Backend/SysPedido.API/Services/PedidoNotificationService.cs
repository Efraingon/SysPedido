using Microsoft.AspNetCore.SignalR;
using SysPedido.Application.Interfaces;
using SysPedido.API.Hubs;

namespace SysPedido.API.Services;

public class PedidoNotificationService : IPedidoNotificationService
{
    private readonly IHubContext<PedidoHub> _hubContext;

    public PedidoNotificationService(IHubContext<PedidoHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyPedidoCreadoAsync(string numeroPedido, decimal total, string vendedorUsername, int empresaId)
    {
        var message = $"Nuevo pedido {numeroPedido} por ${total}";

        // Notify Admins of this specific company
        await _hubContext.Clients.Group($"Empresa-{empresaId}-Admin").SendAsync("RecibioPedidoNuevo", message);

        // Notify the specific seller who created it, just in case they have multiple sessions open
        // SignalR maps Users by Default via NameIdentifier. Since we use `ClaimTypes.Name` for username, 
        // we'll send it directly using the username if we wire up IUserIdProvider, or we can use generic broadcast 
        // to a group named after the seller. Let's use a group named after the seller's username for safety.
        await _hubContext.Clients.User(vendedorUsername).SendAsync("RecibioPedidoNuevo", message);
    }
}
