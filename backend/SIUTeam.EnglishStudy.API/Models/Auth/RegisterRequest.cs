using System.ComponentModel.DataAnnotations;

namespace SIUTeam.EnglishStudy.API.Models.Auth;

/// <summary>
/// Registration request model
/// </summary>
public record RegisterRequest
{
    /// <summary>
    /// User email address
    /// </summary>
    /// <example>john.doe@example.com</example>
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// Username
    /// </summary>
    /// <example>johndoe</example>
    [Required(ErrorMessage = "Username is required")]
    [MinLength(3, ErrorMessage = "Username must be at least 3 characters long")]
    [MaxLength(50, ErrorMessage = "Username cannot exceed 50 characters")]
    public string Username { get; init; } = string.Empty;

    /// <summary>
    /// User password (minimum 8 characters)
    /// </summary>
    /// <example>SecurePassword123!</example>
    [Required(ErrorMessage = "Password is required")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
    public string Password { get; init; } = string.Empty;

    /// <summary>
    /// User first name
    /// </summary>
    /// <example>John</example>
    [Required(ErrorMessage = "First name is required")]
    [MaxLength(100, ErrorMessage = "First name cannot exceed 100 characters")]
    public string FirstName { get; init; } = string.Empty;

    /// <summary>
    /// User last name
    /// </summary>
    /// <example>Doe</example>
    [Required(ErrorMessage = "Last name is required")]
    [MaxLength(100, ErrorMessage = "Last name cannot exceed 100 characters")]
    public string LastName { get; init; } = string.Empty;
}
