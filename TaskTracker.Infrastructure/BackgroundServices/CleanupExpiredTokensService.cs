using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TaskTracker.Application.Common.Interfaces.UnitOfWork;

namespace TaskTracker.Infrastructure.BackgroundServices;

public class CleanupExpiredTokensService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly TimeSpan _period = TimeSpan.FromHours(1);

    public CleanupExpiredTokensService(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await DoWorkAsync(stoppingToken);
            await Task.Delay(_period, stoppingToken);
        }
    }

    private async Task DoWorkAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var unitOfWorkFactory = scope.ServiceProvider.GetRequiredService<IUnitOfWorkFactory>();

            using var uow = unitOfWorkFactory.CreateUnitOfWork();
            await uow.RefreshTokens.RemoveExpiredTokensAsync();
            await uow.SaveChangesAsync(cancellationToken);

            Console.Error.WriteLine("Expired refresh tokens cleaned up at: {time}", DateTimeOffset.Now);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("Error occurred executing cleanup of expired refresh tokens", ex);
        }
    }
}