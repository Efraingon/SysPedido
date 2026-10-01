namespace SysPedido.Application.Interfaces;

public interface IPedidoNotificationService
{
    Task NotifyPedidoCreadoAsync(string numeroPedido, decimal total, string vendedorUsername, int empresaId);
}
