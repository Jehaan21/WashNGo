using WashNGo.Models;

namespace WashNGo.Repositories;

// ─────────────────────────────────────────────────────────
// USER REPOSITORY
// ─────────────────────────────────────────────────────────
public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByUsernameAsync(string username);
    Task<bool>  EmailExistsAsync(string email);
    Task<bool>  UsernameExistsAsync(string username);
    Task<IEnumerable<User>> GetCustomersAsync();
    Task<User?> GetWithRoleAsync(int userId);
}

// ─────────────────────────────────────────────────────────
// SERVICE REPOSITORY
// ─────────────────────────────────────────────────────────
public interface IServiceRepository : IRepository<Service>
{
    Task<IEnumerable<Service>> GetAvailableAsync();
    Task<IEnumerable<Service>> GetAvailableByVehicleTypeAsync(string vehicleType);
    Task<IEnumerable<Service>> SearchAsync(string? search, string? vehicleType, decimal? maxPrice);
    Task<Service?> GetWithReviewsAsync(int serviceId);
    Task<IEnumerable<Service>> GetRelatedAsync(int serviceId, string vehicleType, int take = 3);
}

// ─────────────────────────────────────────────────────────
// TIMESLOT REPOSITORY
// ─────────────────────────────────────────────────────────
public interface ITimeSlotRepository : IRepository<TimeSlot>
{
    Task<IEnumerable<TimeSlot>> GetActiveAsync();
    Task<IEnumerable<TimeSlot>> GetAllByDayAsync(int dayOfWeek);
    Task<IEnumerable<TimeSlot>> GetMatrixSlotsAsync();
    Task<TimeSlot?> GetByDayAndHourAsync(int dayOfWeek, int startHour);
    Task<int> GetBookingCountAsync(int slotId, DateOnly date);
    Task<bool> IsSlotFullAsync(int slotId, DateOnly date);
}

// ─────────────────────────────────────────────────────────
// BOOKING REPOSITORY
// ─────────────────────────────────────────────────────────
public interface IBookingRepository : IRepository<Booking>
{
    Task<IEnumerable<Booking>> GetByUserAsync(int userId);
    Task<Booking?> GetWithDetailsAsync(int bookingId);
    Task<Booking?> GetByIdForUserAsync(int bookingId, int userId);
    Task<IEnumerable<Booking>> GetAllWithDetailsAsync(string? status, DateOnly? date);
    Task<bool> HasConflictAsync(int userId, DateOnly date, int slotId);
    Task<decimal> GetTotalRevenueAsync();
    Task<int> GetTodaysCountAsync();
    Task<List<(string ServiceName, int Count)>> GetPopularServicesAsync(int take = 5);
    Task<List<(string Status, int Count)>> GetBookingsByStatusAsync();
}

// ─────────────────────────────────────────────────────────
// PAYMENT REPOSITORY
// ─────────────────────────────────────────────────────────
public interface IPaymentRepository : IRepository<Payment>
{
    Task<Payment?> GetByBookingIdAsync(int bookingId);
}

// ─────────────────────────────────────────────────────────
// REVIEW REPOSITORY
// ─────────────────────────────────────────────────────────
public interface IReviewRepository : IRepository<Review>
{
    Task<bool>  HasReviewedAsync(int bookingId);
    Task<IEnumerable<Review>> GetByServiceAsync(int serviceId);
    Task<IEnumerable<Review>> GetAllWithDetailsAsync();
    Task<double> GetAverageRatingAsync(int serviceId);
}

// ─────────────────────────────────────────────────────────
// CONTACT MESSAGE REPOSITORY
// ─────────────────────────────────────────────────────────
public interface IContactMessageRepository : IRepository<ContactMessage>
{
    Task<IEnumerable<ContactMessage>> GetAllOrderedAsync();
    Task<int> GetUnreadCountAsync();
}

// ─────────────────────────────────────────────────────────
// NOTIFICATION REPOSITORY
// ─────────────────────────────────────────────────────────
public interface INotificationRepository : IRepository<Notification>
{
    Task<IEnumerable<Notification>> GetByUserAsync(int userId);
}
