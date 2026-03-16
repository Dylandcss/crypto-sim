using System.ComponentModel.DataAnnotations;
using CryptoSim.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Models;

[Index(nameof(Username), IsUnique = true)]
[Index(nameof(Email), IsUnique = true)]
public class User
{
    public int Id { get; set; }

    [StringLength(50)]
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [StringLength(255)]
    [Required]
    public string Email { get; set; } = string.Empty;

    [Required]
    public Role Role { get; set; } = Role.User;

    [Precision(18, 8)]
    [Required]
    public decimal Balance { get; set; } = 10_000m;

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}