using ElderCare.Domain.Entities;
using ElderCare.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ElderCare.Infrastructure.Persistence.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ElderCareDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(ElderCareDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.ToListAsync(cancellationToken);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(predicate).ToListAsync(cancellationToken);
    }

    public virtual async Task<T?> FirstOrDefaultAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        return entity;
    }

    public virtual async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddRangeAsync(entities, cancellationToken);
    }

    public virtual Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }

    public virtual Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Remove(entity);
        return Task.CompletedTask;
    }

    public virtual IQueryable<T> Query()
    {
        return _dbSet.AsQueryable();
    }
}

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ElderCareDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(u => u.Customer)
            .Include(u => u.Caregiver)
            .Include(u => u.Wallet)
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<User?> GetByPhoneAsync(string phone, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(u => u.Customer)
            .Include(u => u.Caregiver)
            .Include(u => u.Wallet)
            .FirstOrDefaultAsync(u => u.PhoneNumber == phone, cancellationToken);
    }
}

public class CaregiverRepository : Repository<Caregiver>, ICaregiverRepository
{
    public CaregiverRepository(ElderCareDbContext context) : base(context)
    {
    }

    public async Task<Caregiver?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.Skills)
            .Include(c => c.Availabilities)
            .Include(c => c.PersonalityAssessment)
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);
    }

    public async Task<IEnumerable<Caregiver>> GetApprovedCaregiversAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.Skills)
            .Include(c => c.Availabilities)
            .Where(c => c.VerificationStatus == Domain.Enums.VerificationStatus.Approved)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Caregiver>> GetPendingCaregiversAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.User)
            .Where(c => c.VerificationStatus == Domain.Enums.VerificationStatus.Pending)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}

public class BookingRepository : Repository<Booking>, IBookingRepository
{
    public BookingRepository(ElderCareDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Booking>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(b => b.Caregiver)
            .Include(b => b.Beneficiary)
            .Include(b => b.Review)
            .Where(b => b.CustomerId == customerId)
            .OrderByDescending(b => b.ScheduledStartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Booking>> GetByCaregiverIdAsync(Guid caregiverId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(b => b.Customer)
            .Include(b => b.Beneficiary)
            .Include(b => b.Review)
            .Where(b => b.CaregiverId == caregiverId)
            .OrderByDescending(b => b.ScheduledStartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Booking>> GetByStatusAsync(Domain.Enums.BookingStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(b => b.Customer)
            .Include(b => b.Caregiver)
            .Include(b => b.Beneficiary)
            .Where(b => b.Status == status)
            .OrderByDescending(b => b.ScheduledStartTime)
            .ToListAsync(cancellationToken);
    }
}
