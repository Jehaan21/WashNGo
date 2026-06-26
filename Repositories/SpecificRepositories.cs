using Microsoft.EntityFrameworkCore;
using WashNGo.Data;
using WashNGo.Models;

namespace WashNGo.Repositories;

// ─────────────────────────────────────────────────────────
// USER REPOSITORY
// ─────────────────────────────────────────────────────────
public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(WashNGoDbContext context) : base(context) { }

    // Override to always include Role so user.Role is never null
    public new async Task<User?> GetByIdAsync(int id)
        => await _dbSet.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserID == id);

    public async Task<User?> GetByEmailAsync(string email)
        => await _dbSet.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == email);

    public async Task<User?> GetByUsernameAsync(string username)
        => await _dbSet.FirstOrDefaultAsync(u => u.Username == username);

    public async Task<bool> EmailExistsAsync(string email)
        => await _dbSet.AnyAsync(u => u.Email == email);

    public async Task<bool> UsernameExistsAsync(string username)
        => await _dbSet.AnyAsync(u => u.Username == username);

    public async Task<IEnumerable<User>> GetCustomersAsync()
        => await _dbSet
            .Include(u => u.Role)
            .Where(u => u.RoleID == 2)
            .OrderByDescending(u => u.RegistrationDate)
            .ToListAsync();

    public async Task<User?> GetWithRoleAsync(int userId)
        => await _dbSet.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserID == userId);
}

// ─────────────────────────────────────────────────────────
// SERVICE REPOSITORY
// ─────────────────────────────────────────────────────────
public class ServiceRepository : Repository<Service>, IServiceRepository
{
    public ServiceRepository(WashNGoDbContext context) : base(context) { }

    public async Task<IEnumerable<Service>> GetAvailableAsync()
        => await _dbSet
            .Where(s => s.IsAvailable)
            .OrderBy(s => s.ServiceID)
            .ToListAsync();

    public async Task<IEnumerable<Service>> GetAvailableByVehicleTypeAsync(string vehicleType)
        => await _dbSet
            .Where(s => s.IsAvailable &&
                       (s.VehicleType == vehicleType || s.VehicleType == "Both"))
            .ToListAsync();

    public async Task<IEnumerable<Service>> SearchAsync(string? search, string? vehicleType, decimal? maxPrice)
    {
        var query = _dbSet.Where(s => s.IsAvailable).AsQueryable();

        if (!string.IsNullOrEmpty(vehicleType) && vehicleType != "All")
            query = query.Where(s => s.VehicleType == vehicleType || s.VehicleType == "Both");

        if (!string.IsNullOrEmpty(search))
            query = query.Where(s => s.ServiceName.Contains(search) || s.Description.Contains(search));

        if (maxPrice.HasValue)
            query = query.Where(s => s.Price <= maxPrice.Value);

        return await query.ToListAsync();
    }

    public async Task<Service?> GetWithReviewsAsync(int serviceId)
        => await _dbSet
            .Include(s => s.Reviews.Where(r => r.IsVisible))
            .ThenInclude(r => r.User)
            .FirstOrDefaultAsync(s => s.ServiceID == serviceId);

    public async Task<IEnumerable<Service>> GetRelatedAsync(int serviceId, string vehicleType, int take = 3)
        => await _dbSet
            .Where(s => s.IsAvailable && s.ServiceID != serviceId &&
                       (s.VehicleType == vehicleType || s.VehicleType == "Both"))
            .Take(take)
            .ToListAsync();
}

// ─────────────────────────────────────────────────────────
// TIMESLOT REPOSITORY
// ─────────────────────────────────────────────────────────
public class TimeSlotRepository : Repository<TimeSlot>, ITimeSlotRepository
{
    public TimeSlotRepository(WashNGoDbContext context) : base(context) { }

    public async Task<IEnumerable<TimeSlot>> GetActiveAsync()
        => await _dbSet
            .Where(ts => ts.IsActive)
            .OrderBy(ts => ts.DayOfWeek)
            .ThenBy(ts => ts.StartTime)
            .ToListAsync();

    public async Task<IEnumerable<TimeSlot>> GetAllByDayAsync(int dayOfWeek)
        => await _dbSet
            .Where(ts => ts.DayOfWeek == dayOfWeek)
            .OrderBy(ts => ts.StartTime)
            .ToListAsync();

    public async Task<IEnumerable<TimeSlot>> GetMatrixSlotsAsync()
        => await _dbSet
            .OrderBy(ts => ts.DayOfWeek)
            .ThenBy(ts => ts.StartTime)
            .ToListAsync();

    public async Task<TimeSlot?> GetByDayAndHourAsync(int dayOfWeek, int startHour)
        => await _dbSet.FirstOrDefaultAsync(ts =>
            ts.DayOfWeek == dayOfWeek &&
            ts.StartTime == TimeSpan.FromHours(startHour));

    public async Task<int> GetBookingCountAsync(int slotId, DateOnly date)
        => await _context.Bookings.CountAsync(b =>
            b.SlotID      == slotId &&
            b.BookingDate == date   &&
            b.BookingStatus != "Cancelled");

    public async Task<bool> IsSlotFullAsync(int slotId, DateOnly date)
    {
        var slot = await _dbSet.FindAsync(slotId);
        if (slot == null) return true;
        int count = await GetBookingCountAsync(slotId, date);
        return count >= slot.MaxBookings;
    }
}

// ─────────────────────────────────────────────────────────
// BOOKING REPOSITORY
// ─────────────────────────────────────────────────────────
public class BookingRepository : Repository<Booking>, IBookingRepository
{
    public BookingRepository(WashNGoDbContext context) : base(context) { }

    public async Task<IEnumerable<Booking>> GetByUserAsync(int userId)
        => await _dbSet
            .Include(b => b.Service)
            .Include(b => b.Slot)
            .Include(b => b.Review)
            .Where(b => b.UserID == userId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();

    public async Task<Booking?> GetWithDetailsAsync(int bookingId)
        => await _dbSet
            .Include(b => b.User)
            .Include(b => b.Service)
            .Include(b => b.Slot)
            .Include(b => b.Payment)
            .FirstOrDefaultAsync(b => b.BookingID == bookingId);

    public async Task<Booking?> GetByIdForUserAsync(int bookingId, int userId)
        => await _dbSet
            .Include(b => b.Service)
            .Include(b => b.Slot)
            .FirstOrDefaultAsync(b => b.BookingID == bookingId && b.UserID == userId);

    public async Task<IEnumerable<Booking>> GetAllWithDetailsAsync(string? status, DateOnly? date)
    {
        var query = _dbSet
            .Include(b => b.User)
            .Include(b => b.Service)
            .Include(b => b.Slot)
            .AsQueryable();

        if (!string.IsNullOrEmpty(status) && status != "All")
            query = query.Where(b => b.BookingStatus == status);

        if (date.HasValue)
            query = query.Where(b => b.BookingDate == date.Value);

        return await query.OrderByDescending(b => b.CreatedAt).ToListAsync();
    }

    public async Task<bool> HasConflictAsync(int userId, DateOnly date, int slotId)
        => await _dbSet.AnyAsync(b =>
            b.UserID      == userId &&
            b.BookingDate == date   &&
            b.SlotID      == slotId &&
            b.BookingStatus != "Cancelled");

    public async Task<decimal> GetTotalRevenueAsync()
        => await _dbSet
            .Where(b => b.BookingStatus == "Completed")
            .SumAsync(b => (decimal?)b.TotalAmount) ?? 0;

    public async Task<int> GetTodaysCountAsync()
        => await _dbSet.CountAsync(b => b.BookingDate == DateOnly.FromDateTime(DateTime.Today));

    public async Task<List<(string ServiceName, int Count)>> GetPopularServicesAsync(int take = 5)
        => await _dbSet
            .Where(b => b.BookingStatus != "Cancelled")
            .GroupBy(b => b.Service!.ServiceName)
            .Select(g => new { Name = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(take)
            .Select(x => ValueTuple.Create(x.Name, x.Count))
            .ToListAsync();

    public async Task<List<(string Status, int Count)>> GetBookingsByStatusAsync()
        => await _dbSet
            .GroupBy(b => b.BookingStatus)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .Select(x => ValueTuple.Create(x.Status, x.Count))
            .ToListAsync();
}

// ─────────────────────────────────────────────────────────
// PAYMENT REPOSITORY
// ─────────────────────────────────────────────────────────
public class PaymentRepository : Repository<Payment>, IPaymentRepository
{
    public PaymentRepository(WashNGoDbContext context) : base(context) { }

    public async Task<Payment?> GetByBookingIdAsync(int bookingId)
        => await _dbSet.FirstOrDefaultAsync(p => p.BookingID == bookingId);
}

// ─────────────────────────────────────────────────────────
// REVIEW REPOSITORY
// ─────────────────────────────────────────────────────────
public class ReviewRepository : Repository<Review>, IReviewRepository
{
    public ReviewRepository(WashNGoDbContext context) : base(context) { }

    public async Task<bool> HasReviewedAsync(int bookingId)
        => await _dbSet.AnyAsync(r => r.BookingID == bookingId);

    public async Task<IEnumerable<Review>> GetByServiceAsync(int serviceId)
        => await _dbSet
            .Include(r => r.User)
            .Where(r => r.ServiceID == serviceId && r.IsVisible)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

    public async Task<IEnumerable<Review>> GetAllWithDetailsAsync()
        => await _dbSet
            .Include(r => r.User)
            .Include(r => r.Service)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

    public async Task<double> GetAverageRatingAsync(int serviceId)
    {
        var ratings = await _dbSet
            .Where(r => r.ServiceID == serviceId && r.IsVisible)
            .Select(r => (int)r.Rating)
            .ToListAsync();
        return ratings.Any() ? ratings.Average() : 0;
    }
}

// ─────────────────────────────────────────────────────────
// CONTACT MESSAGE REPOSITORY
// ─────────────────────────────────────────────────────────
public class ContactMessageRepository : Repository<ContactMessage>, IContactMessageRepository
{
    public ContactMessageRepository(WashNGoDbContext context) : base(context) { }

    public async Task<IEnumerable<ContactMessage>> GetAllOrderedAsync()
        => await _dbSet.OrderByDescending(m => m.SentAt).ToListAsync();

    public async Task<int> GetUnreadCountAsync()
        => await _dbSet.CountAsync(m => !m.IsRead);
}

// ─────────────────────────────────────────────────────────
// NOTIFICATION REPOSITORY
// ─────────────────────────────────────────────────────────
public class NotificationRepository : Repository<Notification>, INotificationRepository
{
    public NotificationRepository(WashNGoDbContext context) : base(context) { }

    public async Task<IEnumerable<Notification>> GetByUserAsync(int userId)
        => await _dbSet
            .Where(n => n.UserID == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
}
