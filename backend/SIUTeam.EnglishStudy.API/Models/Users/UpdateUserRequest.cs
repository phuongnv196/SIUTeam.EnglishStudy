using System.ComponentModel.DataAnnotations;

namespace SIUTeam.EnglishStudy.API.Models.Users;

/// <summary>
/// Update user request model
/// </summary>
public record UpdateUserRequest
{
    /// <summary>
    /// Updated first name
    /// </summary>
    /// <example>John</example>
    [Required(ErrorMessage = "First name is required")]
    [MaxLength(100, ErrorMessage = "First name cannot exceed 100 characters")]
    public string FirstName { get; init; } = string.Empty;

    /// <summary>
    /// Updated last name
    /// </summary>
    /// <example>Doe</example>
    [Required(ErrorMessage = "Last name is required")]
    [MaxLength(100, ErrorMessage = "Last name cannot exceed 100 characters")]
    public string LastName { get; init; } = string.Empty;
}
