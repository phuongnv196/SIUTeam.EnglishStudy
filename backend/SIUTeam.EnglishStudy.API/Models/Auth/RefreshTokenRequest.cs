using System.ComponentModel.DataAnnotations;

namespace SIUTeam.EnglishStudy.API.Models.Auth;

/// <summary>
/// Refresh token request model
/// </summary>
public record RefreshTokenRequest
{
    /// <summary>
    /// Refresh token
    /// </summary>
    [Required(ErrorMessage = "Refresh token is required")]
    public string RefreshToken { get; init; } = string.Empty;
}
