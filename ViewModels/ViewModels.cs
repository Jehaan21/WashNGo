using System.ComponentModel.DataAnnotations;

namespace WashNGo.ViewModels;

// ─────────────────────────────────────────
// AUTH
// ─────────────────────────────────────────
public class RegisterViewModel
{
    // ── Personal Information ───────────────────────────────
    [Required, StringLength(150)]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [Required, StringLength(100)]
    [Display(Name = "Username")]
    public string Username { get; set; } = string.Empty;

    [Required, EmailAddress]
    [Display(Name = "Email Address")]
    public string Email { get; set; } = string.Empty;

    [Required, Phone]
    [Display(Name = "Phone Number")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Display(Name = "Date of Birth")]
    [DataType(DataType.Date)]
    public DateOnly? DateOfBirth { get; set; }

    // ── Security ────────────────────────────────────────────
    [Required, MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*]).+$",
        ErrorMessage = "Password must include uppercase, lowercase, a number, and a special character.")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    [Display(Name = "Confirm Password")]
    public string ConfirmPassword { get; set; } = string.Empty;

    // ── Contact Information ────────────────────────────────
    [Display(Name = "Division")]
    public string? Division { get; set; }

    [Display(Name = "City")]
    public string? City { get; set; }

    [Display(Name = "Area")]
    public string? Area { get; set; }

    [Required]
    [Display(Name = "Full Address")]
    public string Address { get; set; } = string.Empty;

    // ── Vehicle Information ────────────────────────────────
    [Required(ErrorMessage = "Please select a vehicle type.")]
    [Display(Name = "Vehicle Type")]
    public string VehicleType { get; set; } = string.Empty;   // Car | Bike

    [Required(ErrorMessage = "Vehicle brand is required.")]
    [Display(Name = "Vehicle Brand")]
    public string VehicleBrand { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vehicle model is required.")]
    [Display(Name = "Vehicle Model")]
    public string VehicleModel { get; set; } = string.Empty;

    [Display(Name = "Vehicle Registration Number")]
    public string? VehicleRegNumber { get; set; }

    // ── Preferences ─────────────────────────────────────────
    [Display(Name = "Preferred Language")]
    public string PreferredLanguage { get; set; } = "English";

    [Display(Name = "Email Notifications")]
    public bool NotifyByEmail { get; set; } = true;

    [Display(Name = "SMS Notifications")]
    public bool NotifyBySms { get; set; } = false;

    [Display(Name = "WhatsApp Notifications")]
    public bool NotifyByWhatsApp { get; set; } = false;

    // ── Terms & Consent ─────────────────────────────────────
    [Required]
    [Range(typeof(bool), "true", "true", ErrorMessage = "You must agree to the Terms and Conditions.")]
    public bool AgreedToTerms { get; set; }

    [Required]
    [Range(typeof(bool), "true", "true", ErrorMessage = "You must agree to the Privacy Policy.")]
    public bool AgreedToPrivacyPolicy { get; set; }

    public bool OptInPromotions { get; set; } = false;
}

public class LoginViewModel
{
    [Required, EmailAddress]
    [Display(Name = "Email Address")]
    public string Email { get; set; } = string.Empty;

    [Required, DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Remember Me")]
    public bool RememberMe { get; set; }
}

// ─────────────────────────────────────────
// BOOKING
// ─────────────────────────────────────────
public class BookingViewModel
{
    public int ServiceID { get; set; }

    public int SlotID { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Booking Date")]
    public DateOnly BookingDate { get; set; } = DateOnly.FromDateTime(DateTime.Today.AddDays(1));

    [Required]
    [Display(Name = "Vehicle Type")]
    public string VehicleType { get; set; } = string.Empty;

    [Display(Name = "Vehicle Number (Optional)")]
    public string? VehicleNumber { get; set; }

    [Display(Name = "Payment Method")]
    public string PaymentMethod { get; set; } = "Cash on Service";

    [Display(Name = "Special Notes")]
    [StringLength(500)]
    public string? Notes { get; set; }

    // Auto-filled display fields
    public string Username    { get; set; } = string.Empty;
    public string Email       { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address     { get; set; } = string.Empty;

    // For dropdowns in view
    public List<WashNGo.Models.Service>  Services  { get; set; } = new();
    public List<WashNGo.Models.TimeSlot> TimeSlots { get; set; } = new();
}

public class BookingConfirmationViewModel
{
    public int     BookingID     { get; set; }
    public string  ReservationID { get; set; } = string.Empty;
    public string  ServiceName   { get; set; } = string.Empty;
    public string  VehicleType   { get; set; } = string.Empty;
    public decimal TotalAmount   { get; set; }
    public DateOnly BookingDate  { get; set; }
    public string  SlotLabel     { get; set; } = string.Empty;
    public int     Duration      { get; set; }
    public string  BookingStatus { get; set; } = string.Empty;
    public string  PaymentMethod { get; set; } = string.Empty;
}

// ─────────────────────────────────────────
// REVIEW
// ─────────────────────────────────────────
public class ReviewViewModel
{
    [Required]
    public int BookingID { get; set; }

    [Required]
    public int ServiceID { get; set; }

    [Required, Range(1, 5)]
    [Display(Name = "Rating")]
    public byte Rating { get; set; }

    [StringLength(1000)]
    [Display(Name = "Your Review")]
    public string? Comment { get; set; }

    // display only
    public string ServiceName { get; set; } = string.Empty;
}

// ─────────────────────────────────────────
// CONTACT
// ─────────────────────────────────────────
public class ContactViewModel
{
    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(300)]
    public string Subject { get; set; } = string.Empty;

    [Required, StringLength(3000)]
    public string Message { get; set; } = string.Empty;
}

// ─────────────────────────────────────────
// ADMIN – SERVICE FORM
// ─────────────────────────────────────────
public class ServiceFormViewModel
{
    public int ServiceID { get; set; }

    [Required, StringLength(200)]
    [Display(Name = "Service Name")]
    public string ServiceName { get; set; } = string.Empty;

    [Required, StringLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Required, Range(0.01, 99999)]
    [DataType(DataType.Currency)]
    public decimal Price { get; set; }

    [Required, Range(1, 1440)]
    [Display(Name = "Duration (minutes)")]
    public int DurationMinutes { get; set; }

    [Required]
    [Display(Name = "Vehicle Type")]
    public string VehicleType { get; set; } = string.Empty;

    [Display(Name = "Service Image")]
    public IFormFile? ImageFile { get; set; }

    public string? ExistingImageUrl { get; set; }
    public bool IsAvailable { get; set; } = true;
}

public class ForgotPasswordViewModel
{
    [Required, EmailAddress]
    [Display(Name = "Email Address")]
    public string Email { get; set; } = string.Empty;
}

public class ResetPasswordViewModel
{
    [Required]
    public string Token { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(6)]
    [DataType(DataType.Password)]
    [Display(Name = "New Password")]
    public string NewPassword { get; set; } = string.Empty;

    [Required, DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
    [Display(Name = "Confirm New Password")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

// ─────────────────────────────────────────
// SLOT MATRIX
// ─────────────────────────────────────────
public class SlotMatrixViewModel
{
    // All hourly time labels (rows) e.g. "5:00 AM – 6:00 AM"
    public List<(TimeSpan Start, TimeSpan End, string Label)> TimeRows { get; set; } = new();

    // Day columns: 0=Sun,1=Mon,...,6=Sat
    public List<int> Days { get; set; } = new() { 1, 2, 3, 4, 5, 6, 0 }; // Mon→Sun

    // Key: (DayOfWeek, StartHour) → TimeSlot (null = not created)
    public Dictionary<(int Day, int Hour), WashNGo.Models.TimeSlot?> Matrix { get; set; } = new();

    public int DefaultMaxBookings { get; set; } = 5;

    public static string DayName(int day) => day switch
    {
        0 => "Sunday",    1 => "Monday",   2 => "Tuesday",
        3 => "Wednesday", 4 => "Thursday", 5 => "Friday",
        6 => "Saturday",  _ => ""
    };

    public static string DayShort(int day) => day switch
    {
        0 => "SUN", 1 => "MON", 2 => "TUE",
        3 => "WED", 4 => "THU", 5 => "FRI",
        6 => "SAT", _ => ""
    };
}
public class AnalyticsViewModel
{
    public int     TotalUsers        { get; set; }
    public int     TotalBookings     { get; set; }
    public int     PendingBookings   { get; set; }
    public int     TodaysBookings    { get; set; }
    public decimal TotalRevenue      { get; set; }
    public int     TotalMessages     { get; set; }
    public int     UnreadMessages    { get; set; }
    public List<(string ServiceName, int Count)> PopularServices  { get; set; } = new();
    public List<(string Status, int Count)>      BookingsByStatus { get; set; } = new();
}
