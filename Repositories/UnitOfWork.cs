using Microsoft.EntityFrameworkCore.Storage;
using WashNGo.Data;

namespace WashNGo.Repositories;

/// <summary>
/// Concrete Unit of Work. All repositories share the same DbContext
/// so all changes are committed atomically in CompleteAsync().
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly WashNGoDbContext _context;
    private IDbContextTransaction?   _transaction;

    // ── Lazy-initialised repositories ─────────────────────
    private IUserRepository?           _users;
    private IServiceRepository?        _services;
    private ITimeSlotRepository?       _timeSlots;
    private IBookingRepository?        _bookings;
    private IPaymentRepository?        _payments;
    private IReviewRepository?         _reviews;
    private IContactMessageRepository? _contactMessages;
    private INotificationRepository?   _notifications;

    public UnitOfWork(WashNGoDbContext context) => _context = context;

    // ── Repository accessors ──────────────────────────────
    public IUserRepository           Users
        => _users           ??= new UserRepository(_context);

    public IServiceRepository        Services
        => _services        ??= new ServiceRepository(_context);

    public ITimeSlotRepository       TimeSlots
        => _timeSlots       ??= new TimeSlotRepository(_context);

    public IBookingRepository        Bookings
        => _bookings        ??= new BookingRepository(_context);

    public IPaymentRepository        Payments
        => _payments        ??= new PaymentRepository(_context);

    public IReviewRepository         Reviews
        => _reviews         ??= new ReviewRepository(_context);

    public IContactMessageRepository ContactMessages
        => _contactMessages ??= new ContactMessageRepository(_context);

    public INotificationRepository   Notifications
        => _notifications   ??= new NotificationRepository(_context);

    // ── Transaction ───────────────────────────────────────
    public async Task BeginTransactionAsync()
        => _transaction = await _context.Database.BeginTransactionAsync();

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
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

    // ── Save ──────────────────────────────────────────────
    public async Task<int> CompleteAsync()
        => await _context.SaveChangesAsync();

    // ── Dispose ───────────────────────────────────────────
    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
