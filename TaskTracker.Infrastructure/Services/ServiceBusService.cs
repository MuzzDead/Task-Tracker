using Azure.Messaging.ServiceBus;
using System.Text.Json;
using TaskTracker.Application.Archice;

namespace TaskTracker.Infrastructure.Services;

public class ServiceBusService : IServiceBusService
{
    private readonly ServiceBusClient _serviceBusClient;
    public ServiceBusService(string connectionString)
    {
        _serviceBusClient = new ServiceBusClient(connectionString);
    }

    public async Task SendMessageAsync<T>(T message, string queueName)
    {
        var sender = _serviceBusClient.CreateSender(queueName);
        var messageBody = JsonSerializer.Serialize(message);
        var serviceBusMessage = new ServiceBusMessage(messageBody);

        await sender.SendMessageAsync(serviceBusMessage);
        await sender.CloseAsync();
    }
}
