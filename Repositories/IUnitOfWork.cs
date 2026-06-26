namespace WashNGo.Repositories;

/// <summary>
/// Unit of Work — coordinates all repositories and commits
/// all changes in a single transaction via CompleteAsync().
/// </summary>
public interface IUnitOfWork : IDisposable
{
    IUserRepository            Users            { get; }
    IServiceRepository         Services         { get; }
    ITimeSlotRepository        TimeSlots        { get; }
    IBookingRepository         Bookings         { get; }
    IPaymentRepository         Payments         { get; }
    IReviewRepository          Reviews          { get; }
    IContactMessageRepository  ContactMessages  { get; }
    INotificationRepository    Notifications    { get; }

    /// <summary>Commits all pending changes to the database.</summary>
    Task<int> CompleteAsync();

    /// <summary>Begins a database transaction for multi-step atomic operations.</summary>
    Task BeginTransactionAsync();

    /// <summary>Commits the current transaction.</summary>
    Task CommitTransactionAsync();

    /// <summary>Rolls back the current transaction.</summary>
    Task RollbackTransactionAsync();
}
