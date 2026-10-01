using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace SysPedido.API.Hubs;

[Authorize]
public class PedidoHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var role = Context.User?.FindFirst(ClaimTypes.Role)?.Value;
        var empresaId = Context.User?.FindFirst("EmpresaId")?.Value;

        if (role == "Admin" && !string.IsNullOrEmpty(empresaId))
        {
            // Join Admin Group for their specific company
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Empresa-{empresaId}-Admin");
        }

        await base.OnConnectedAsync();
    }
}
