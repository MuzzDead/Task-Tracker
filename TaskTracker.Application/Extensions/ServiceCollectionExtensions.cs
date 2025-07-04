using Microsoft.Extensions.DependencyInjection;

namespace TaskTracker.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg =>
        {
            cfg.AddMaps(typeof(IApplicationMarker).Assembly);
        });

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(IApplicationMarker).Assembly));

        return services;
    }
}
