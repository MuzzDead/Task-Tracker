using Microsoft.Extensions.DependencyInjection;
using TaskTracker.Application.Common.Interfaces.UnitOfWork;

namespace TaskTracker.Persistence;

public class UnitOfWorkFactory : IUnitOfWorkFactory
{
    private readonly IServiceProvider _serviceProvider;
    public UnitOfWorkFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IUnitOfWork CreateUnitOfWork()
    {
        var context = _serviceProvider.GetRequiredService<ApplicationDbContext>();
        return new UnitOfWork(context);
    }
}
