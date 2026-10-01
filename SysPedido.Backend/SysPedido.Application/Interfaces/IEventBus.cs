namespace SysPedido.Application.Interfaces;

public interface IEventBus
{
    Task PublishAsync<T>(T @event);
}

public class PedidoCreadoEvent
{
    public string NumeroPedido { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
}
