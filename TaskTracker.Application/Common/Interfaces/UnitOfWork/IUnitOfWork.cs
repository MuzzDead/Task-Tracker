using TaskTracker.Application.Common.Interfaces.Auth;

namespace TaskTracker.Application.Common.Interfaces.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    IBoardRepository Boards { get; }
    IBoardRoleRepository BoardRoles { get; }
    ICardRepository Cards { get; }
    IColumnRepository Columns { get; }
    ICommentRepository Comments { get; }
    IStateRepository States { get; }
    IUserRepository Users { get; }
    IRefreshTokenRepository RefreshTokens { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
