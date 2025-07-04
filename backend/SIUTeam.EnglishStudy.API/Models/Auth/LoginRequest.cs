using System.ComponentModel.DataAnnotations;

namespace SIUTeam.EnglishStudy.API.Models.Auth;

/// <summary>
/// Login request model
/// </summary>
public record LoginRequest
{
    /// <summary>
    /// User email address
    /// </summary>
    /// <example>john.doe@example.com</example>
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// User password
    /// </summary>
    /// <example>SecurePassword123!</example>
    [Required(ErrorMessage = "Password is required")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
    public string Password { get; init; } = string.Empty;
}
