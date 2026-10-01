using SysPedido.Application.Interfaces;
using System.Text.Json;

namespace SysPedido.Infrastructure.Messaging;

public class MockServiceBus : IEventBus
{
    public Task PublishAsync<T>(T @event)
    {
        // Simulamos la publicación
        Console.WriteLine($"[Azure Service Bus MOCK] Evento publicado: {JsonSerializer.Serialize(@event)}");
        return Task.CompletedTask;
    }
}
