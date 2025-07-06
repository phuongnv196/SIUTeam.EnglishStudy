namespace SIUTeam.EnglishStudy.Core.DTOs;

/// <summary>
/// Data Transfer Object for User information
/// </summary>
/// <param name="Id">User unique identifier</param>
/// <param name="Email">User email address</param>
/// <param name="Username">User's username</param>
/// <param name="FirstName">User's first name</param>
/// <param name="LastName">User's last name</param>
/// <param name="Avatar">User's avatar URL</param>
/// <param name="Role">User's role in the system</param>
/// <param name="IsActive">Whether the user account is active</param>
/// <param name="CreatedAt">When the user account was created</param>
public record UserDto(
    Guid Id,
    string Email,
    string Username,
    string FirstName,
    string LastName,
    string? Avatar,
    UserRole Role,
    bool IsActive,
    DateTime CreatedAt
);
