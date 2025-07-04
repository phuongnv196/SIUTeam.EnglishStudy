using SIUTeam.EnglishStudy.Core.DTOs;

namespace SIUTeam.EnglishStudy.API.Models.Auth;

/// <summary>
/// Login response model
/// </summary>
public record LoginResponse
{
    /// <summary>
    /// JWT access token
    /// </summary>
    public string Token { get; init; } = string.Empty;

    /// <summary>
    /// Token expiration time
    /// </summary>
    public DateTime ExpiresAt { get; init; }

    /// <summary>
    /// User information
    /// </summary>
    public UserDto User { get; init; } = null!;
}
