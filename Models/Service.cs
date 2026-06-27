using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WashNGo.Models;

public class Service
{
    public int ServiceID { get; set; }

    [Required, StringLength(200)]
    public string ServiceName { get; set; } = string.Empty;

    [Required, StringLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Required, Column(TypeName = "decimal(10,2)")]
    public decimal Price { get; set; }

    [Required]
    public int DurationMinutes { get; set; }

    [Required, StringLength(10)]
    public string VehicleType { get; set; } = string.Empty;   // Bike | Car | Both

    [StringLength(500)]
    public string? ImageUrl { get; set; }

    public bool IsAvailable { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Navigation
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
