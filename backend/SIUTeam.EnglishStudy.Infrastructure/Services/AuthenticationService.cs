using BCrypt.Net;
using SIUTeam.EnglishStudy.Core.Entities;
using SIUTeam.EnglishStudy.Core.Interfaces.Repositories;
using SIUTeam.EnglishStudy.Core.Interfaces.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace SIUTeam.EnglishStudy.Infrastructure.Services;

/// <summary>
/// Service implementation for user authentication operations
/// </summary>
public class AuthenticationService : IAuthenticationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;
    private readonly HashSet<string> _revokedTokens = new(); // In production, use Redis or database

    public AuthenticationService(IUnitOfWork unitOfWork, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _configuration = configuration;
    }

    /// <summary>
    /// Authenticates a user and returns a JWT token
    /// </summary>
    /// <param name="email">User's email address</param>
    /// <param name="password">User's password</param>
    /// <returns>JWT token string if authentication successful, null otherwise</returns>
    public async Task<string> LoginAsync(string email, string password)
    {
        try
        {
            // Find user by email
            var user = await _unitOfWork.Users.GetByEmailAsync(email);
            if (user == null || !user.IsActive)
            {
                return string.Empty;
            }

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return string.Empty;
            }

            // Generate JWT token
            return GenerateJwtToken(user);
        }
        catch
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// Registers a new user in the system
    /// </summary>
    /// <param name="email">User's email address</param>
    /// <param name="username">Unique username</param>
    /// <param name="password">User's password</param>
    /// <param name="firstName">User's first name</param>
    /// <param name="lastName">User's last name</param>
    /// <returns>True if registration successful, false otherwise</returns>
    public async Task<bool> RegisterAsync(string email, string username, string password, string firstName, string lastName)
    {
        try
        {
            // Check if email already exists
            if (await _unitOfWork.Users.EmailExistsAsync(email))
            {
                return false;
            }

            // Check if username already exists
            if (await _unitOfWork.Users.UsernameExistsAsync(username))
            {
                return false;
            }

            // Create new user
            var user = new User
            {
                Email = email.ToLowerInvariant(),
                Username = username.ToLowerInvariant(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                FirstName = firstName,
                LastName = lastName,
                Role = UserRole.Student,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Changes a user's password
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="currentPassword">Current password for verification</param>
    /// <param name="newPassword">New password</param>
    /// <returns>True if password changed successfully, false otherwise</returns>
    public async Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null || !user.IsActive)
            {
                return false;
            }

            // Verify current password
            if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
            {
                return false;
            }

            // Update password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Initiates password reset process (placeholder implementation)
    /// </summary>
    /// <param name="email">User's email address</param>
    /// <returns>True if reset initiated successfully, false otherwise</returns>
    public async Task<bool> ResetPasswordAsync(string email)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(email);
            if (user == null || !user.IsActive)
            {
                return false;
            }

            // TODO: Implement password reset logic
            // 1. Generate reset token
            // 2. Store reset token with expiration
            // 3. Send reset email
            
            // For now, return true to indicate email was found
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Logs out a user by revoking their token
    /// </summary>
    /// <param name="token">JWT token to revoke</param>
    public async Task LogoutAsync(string token)
    {
        // Add token to revoked tokens list
        // In production, store in Redis or database with expiration
        _revokedTokens.Add(token);
        await Task.CompletedTask;
    }

    /// <summary>
    /// Validates a JWT token
    /// </summary>
    /// <param name="token">JWT token to validate</param>
    /// <returns>True if token is valid, false otherwise</returns>
    public async Task<bool> ValidateTokenAsync(string token)
    {
        try
        {
            // Check if token is revoked
            if (_revokedTokens.Contains(token))
            {
                return false;
            }

            // Validate JWT token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(GetJwtSecret());
            
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Generates a JWT token for the specified user
    /// </summary>
    /// <param name="user">User entity</param>
    /// <returns>JWT token string</returns>
    private string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(GetJwtSecret());
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            }),
            Expires = DateTime.UtcNow.AddDays(7), // Token valid for 7 days
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    /// <summary>
    /// Gets the JWT secret from configuration
    /// </summary>
    /// <returns>JWT secret string</returns>
    private string GetJwtSecret()
    {
        return _configuration["JwtSettings:Secret"] ?? "SIUTeamEnglishStudySecretKey2024!@#$%^&*()";
    }
}