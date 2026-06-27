using System.ComponentModel.DataAnnotations;

namespace WashNGo.Models;

public class PasswordResetToken
{
    public int TokenID { get; set; }

    [Required]
    public int UserID { get; set; }

    [Required, StringLength(200)]
    public string Token { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }
    public bool IsUsed        { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public User? User { get; set; }
}
