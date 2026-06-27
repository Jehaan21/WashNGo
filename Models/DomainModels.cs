using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WashNGo.Models;

// ─────────────────────────────────────────
// TIME SLOT
// ─────────────────────────────────────────
public class TimeSlot
{
    public int SlotID { get; set; }

    [Required, StringLength(50)]
    public string SlotLabel { get; set; } = string.Empty;

    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime   { get; set; }

    public int MaxBookings { get; set; } = 5;
    public bool IsActive    { get; set; } = true;

    // 0=Sunday,1=Monday,...,6=Saturday. -1 means applies to ALL days (legacy)
    public int DayOfWeek { get; set; } = -1;

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}

// ─────────────────────────────────────────
// BOOKING
// ─────────────────────────────────────────
public class Booking
{
    public int BookingID { get; set; }

    // Computed in DB; read-only in app
    public string? ReservationID { get; set; }

    [Required]
    public int UserID { get; set; }

    [Required]
    public int ServiceID { get; set; }

    [Required]
    public int SlotID { get; set; }

    [Required]
    public DateOnly BookingDate { get; set; }

    [Required, StringLength(10)]
    public string VehicleType { get; set; } = string.Empty;  // Bike | Car

    [StringLength(50)]
    public string? VehicleNumber { get; set; }

    [Required, Column(TypeName = "decimal(10,2)")]
    public decimal TotalAmount { get; set; }

    [StringLength(20)]
    public string BookingStatus { get; set; } = "Pending";

    [StringLength(50)]
    public string PaymentMethod { get; set; } = "Cash on Service";

    [StringLength(20)]
    public string PaymentStatus { get; set; } = "Unpaid";

    [StringLength(500)]
    public string? Notes { get; set; }

    public DateTime CreatedAt  { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public User?      User    { get; set; }
    public Service?   Service { get; set; }
    public TimeSlot?  Slot    { get; set; }
    public Payment?   Payment { get; set; }
    public Review?    Review  { get; set; }
}

// ─────────────────────────────────────────
// PAYMENT
// ─────────────────────────────────────────
public class Payment
{
    public int PaymentID { get; set; }

    [Required]
    public int BookingID { get; set; }

    [Required, Column(TypeName = "decimal(10,2)")]
    public decimal Amount { get; set; }

    [Required, StringLength(50)]
    public string Method { get; set; } = string.Empty;

    [StringLength(200)]
    public string? TransactionID { get; set; }

    public DateTime? PaidAt { get; set; }

    [StringLength(20)]
    public string Status { get; set; } = "Pending";

    public Booking? Booking { get; set; }
}

// ─────────────────────────────────────────
// REVIEW
// ─────────────────────────────────────────
public class Review
{
    public int ReviewID { get; set; }

    [Required]
    public int UserID { get; set; }

    [Required]
    public int ServiceID { get; set; }

    [Required]
    public int BookingID { get; set; }

    [Required, Range(1, 5)]
    public byte Rating { get; set; }

    [StringLength(1000)]
    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public bool IsVisible      { get; set; } = true;

    public User?    User    { get; set; }
    public Service? Service { get; set; }
    public Booking? Booking { get; set; }
}

// ─────────────────────────────────────────
// CONTACT MESSAGE
// ─────────────────────────────────────────
public class ContactMessage
{
    public int MessageID { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(300)]
    public string Subject { get; set; } = string.Empty;

    [Required, StringLength(3000)]
    public string Message { get; set; } = string.Empty;

    public DateTime SentAt { get; set; } = DateTime.Now;
    public bool IsRead      { get; set; } = false;
}

// ─────────────────────────────────────────
// NOTIFICATION
// ─────────────────────────────────────────
public class Notification
{
    public int NotificationID { get; set; }

    [Required]
    public int UserID { get; set; }

    [Required, StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required, StringLength(1000)]
    public string Message { get; set; } = string.Empty;

    public bool IsRead    { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public User? User { get; set; }
}
