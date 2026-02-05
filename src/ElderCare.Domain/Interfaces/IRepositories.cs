using System.Linq.Expressions;

namespace ElderCare.Domain.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
    IQueryable<T> Query();
}

public interface IUserRepository : IRepository<Entities.User>
{
    Task<Entities.User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Entities.User?> GetByPhoneAsync(string phone, CancellationToken cancellationToken = default);
}

public interface ICaregiverRepository : IRepository<Entities.CaregiverProfile>
{
    Task<Entities.CaregiverProfile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Entities.CaregiverProfile>> GetApprovedCaregiversAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Entities.CaregiverProfile>> GetPendingCaregiversAsync(CancellationToken cancellationToken = default);
}

public interface IBookingRepository : IRepository<Entities.Booking>
{
    Task<IEnumerable<Entities.Booking>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Entities.Booking>> GetByCaregiverIdAsync(Guid caregiverId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Entities.Booking>> GetByStatusAsync(Enums.BookingStatus status, CancellationToken cancellationToken = default);
}

public interface IUnitOfWork : IDisposable
{
    IRepository<T> Repository<T>() where T : class;
    IUserRepository Users { get; }
    IRepository<Entities.CustomerProfile> CustomerProfiles { get; }
    IRepository<Entities.Beneficiary> Beneficiaries { get; }
    ICaregiverRepository Caregivers { get; }
    IBookingRepository Bookings { get; }
    IRepository<Entities.Review> Reviews { get; }
    IRepository<Entities.Wallet> Wallets { get; }
    IRepository<Entities.Transaction> Transactions { get; }
    IRepository<Entities.Notification> Notifications { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
