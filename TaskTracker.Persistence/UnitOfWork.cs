using Microsoft.EntityFrameworkCore.Storage;
using TaskTracker.Application.Common.Interfaces;
using TaskTracker.Application.Common.Interfaces.UnitOfWork;
using TaskTracker.Persistence.Repositories;

namespace TaskTracker.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    private IBoardRepository _boards;
    private IBoardRoleRepository _boardRoles;
    private ICardRepository _cards;
    private IColumnRepository _columns;
    private ICommentRepository _comments;
    private IStateRepository _states;
    private IUserRepository _users;
    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IBoardRepository Boards =>
        _boards ??= new BoardRepository(_context);

    public IBoardRoleRepository BoardRoles =>
        _boardRoles ??= new BoardRoleRepository(_context);

    public ICardRepository Cards =>
        _cards ??= new CardRepository(_context);

    public IColumnRepository Columns =>
        _columns ??= new ColumnRepository(_context);

    public ICommentRepository Comments =>
        _comments ??= new CommentRepository(_context);

    public IStateRepository States =>
        _states ??= new StateRepository(_context);

    public IUserRepository Users =>
        _users ??= new UserRepository(_context);

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
