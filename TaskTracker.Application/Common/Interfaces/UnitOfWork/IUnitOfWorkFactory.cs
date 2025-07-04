namespace TaskTracker.Application.Common.Interfaces.UnitOfWork;

public interface IUnitOfWorkFactory
{
    IUnitOfWork CreateUnitOfWork();
}