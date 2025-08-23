using TaskTracker.API.Endpoints;

namespace TaskTracker.API.Extensions;

public static class EndpointExtensions
{
    public static void MapEndpoints(this IEndpointRouteBuilder app)
    {
        var endpointTypes = typeof(IEndpoint).Assembly
            .GetTypes()
            .Where(t => typeof(IEndpoint).IsAssignableFrom(t) && t is { IsInterface: false, IsAbstract: false });

        foreach (var type in endpointTypes)
        {
            var instance = (IEndpoint)Activator.CreateInstance(type)!;
            instance.MapEndpoints(app);
        }
    }
}
