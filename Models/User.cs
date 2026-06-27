using System.ComponentModel.DataAnnotations;

namespace WashNGo.Models;

public class User
{
    public int UserID { get; set; }

    // ── Personal Info ──────────────────────────────────────
    [StringLength(150)]
    public string? FullName { get; set; }

    [Required, StringLength(100)]
    public string Username { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Required, Phone, StringLength(20)]
    public string PhoneNumber { get; set; } = string.Empty;

    public DateOnly? DateOfBirth { get; set; }

    // ── Contact / Address (structured) ─────────────────────
    [Required, StringLength(500)]
    public string Address { get; set; } = string.Empty;   // kept for backward compatibility / full address

    [StringLength(100)]
    public string? Division { get; set; }

    [StringLength(100)]
    public string? City { get; set; }

    [StringLength(100)]
    public string? Area { get; set; }

    // ── Vehicle Info ────────────────────────────────────────
    [StringLength(10)]
    public string? VehicleType { get; set; }      // Car | Bike

    [StringLength(100)]
    public string? VehicleBrand { get; set; }

    [StringLength(100)]
    public string? VehicleModel { get; set; }

    [StringLength(50)]
    public string? VehicleRegNumber { get; set; }

    // ── Preferences ─────────────────────────────────────────
    [StringLength(20)]
    public string PreferredLanguage { get; set; } = "English";

    public bool NotifyByEmail    { get; set; } = true;
    public bool NotifyBySms      { get; set; } = false;
    public bool NotifyByWhatsApp { get; set; } = false;

    // ── Consent / Terms ─────────────────────────────────────
    public bool AgreedToTerms          { get; set; } = false;
    public bool AgreedToPrivacyPolicy  { get; set; } = false;
    public bool OptInPromotions        { get; set; } = false;

    // ── Verification ────────────────────────────────────────
    public bool   IsEmailVerified { get; set; } = false;
    public string? EmailVerificationToken { get; set; }

    // ── System Fields ───────────────────────────────────────
    public int RoleID { get; set; } = 2;

    public DateTime RegistrationDate { get; set; } = DateTime.Now;
    public DateTime? LastLogin { get; set; }

    [StringLength(20)]
    public string AccountStatus { get; set; } = "Active";

    // Navigation
    public Role? Role { get; set; }
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}

