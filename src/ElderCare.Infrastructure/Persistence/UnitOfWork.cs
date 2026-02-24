using ElderCare.Domain.Entities;
using ElderCare.Domain.Interfaces;
using ElderCare.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace ElderCare.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly ElderCareDbContext _context;
    private IDbContextTransaction? _transaction;
    private readonly Dictionary<Type, object> _repositories;

    public UnitOfWork(ElderCareDbContext context)
    {
        _context = context;
        _repositories = new Dictionary<Type, object>();
        
        // Initialize specialized repositories
        Users = new UserRepository(_context);
        Customers = new Repository<Customer>(_context);
        Beneficiaries = new Repository<Beneficiary>(_context);
        BeneficiaryPreferences = new Repository<BeneficiaryPreference>(_context);
        Caregivers = new CaregiverRepository(_context);
        Bookings = new BookingRepository(_context);
        Reviews = new Repository<Review>(_context);
        Wallets = new Repository<Wallet>(_context);
        Transactions = new Repository<Transaction>(_context);
        Notifications = new Repository<Notification>(_context);
    }

    public IUserRepository Users { get; }
    public IRepository<Customer> Customers { get; }
    public IRepository<Beneficiary> Beneficiaries { get; }
    public IRepository<BeneficiaryPreference> BeneficiaryPreferences { get; }
    public ICaregiverRepository Caregivers { get; }
    public IBookingRepository Bookings { get; }
    public IRepository<Review> Reviews { get; }
    public IRepository<Wallet> Wallets { get; }
    public IRepository<Transaction> Transactions { get; }
    public IRepository<Notification> Notifications { get; }

    public IRepository<T> Repository<T>() where T : class
    {
        var type = typeof(T);
        if (!_repositories.ContainsKey(type))
        {
            _repositories[type] = new Repository<T>(_context);
        }
        return (IRepository<T>)_repositories[type];
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            if (_transaction != null)
            {
                await _transaction.CommitAsync(cancellationToken);
            }
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
