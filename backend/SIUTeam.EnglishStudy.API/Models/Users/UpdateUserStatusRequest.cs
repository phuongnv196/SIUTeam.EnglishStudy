using System.ComponentModel.DataAnnotations;

namespace SIUTeam.EnglishStudy.API.Models.Users;

/// <summary>
/// Update user status request model
/// </summary>
public record UpdateUserStatusRequest
{
    /// <summary>
    /// Active status
    /// </summary>
    /// <example>true</example>
    [Required(ErrorMessage = "IsActive status is required")]
    public bool IsActive { get; init; }
}
