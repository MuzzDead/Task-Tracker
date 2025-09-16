namespace TaskTracker.Application.Archice;

public interface IServiceBusService
{
    Task SendMessageAsync<T>(T message, string queueName);
}
