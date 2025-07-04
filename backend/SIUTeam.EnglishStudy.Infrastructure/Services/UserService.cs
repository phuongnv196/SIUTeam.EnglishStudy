using BCrypt.Net;
using SIUTeam.EnglishStudy.Core.Entities;
using SIUTeam.EnglishStudy.Core.Interfaces.Repositories;
using SIUTeam.EnglishStudy.Core.Interfaces.Services;

namespace SIUTeam.EnglishStudy.Infrastructure.Services;

/// <summary>
/// Service implementation for user management operations
/// </summary>
public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;

    public UserService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Creates a new user in the system
    /// </summary>
    /// <param name="email">User's email address</param>
    /// <param name="username">Unique username</param>
    /// <param name="password">User's password</param>
    /// <param name="firstName">User's first name</param>
    /// <param name="lastName">User's last name</param>
    /// <param name="role">User's role (default: Student)</param>
    /// <returns>User ID if creation successful, Guid.Empty otherwise</returns>
    public async Task<Guid> CreateUserAsync(string email, string username, string password, string firstName, string lastName, UserRole role = UserRole.Student)
    {
        try
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(username) || 
                string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(firstName) || 
                string.IsNullOrWhiteSpace(lastName))
            {
                return Guid.Empty;
            }

            // Check if email already exists
            if (await _unitOfWork.Users.EmailExistsAsync(email))
            {
                return Guid.Empty;
            }

            // Check if username already exists
            if (await _unitOfWork.Users.UsernameExistsAsync(username))
            {
                return Guid.Empty;
            }

            // Create new user
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = email.ToLowerInvariant(),
                Username = username.ToLowerInvariant(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                FirstName = firstName.Trim(),
                LastName = lastName.Trim(),
                Role = role,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return user.Id;
        }
        catch
        {
            return Guid.Empty;
        }
    }

    /// <summary>
    /// Updates user information
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="firstName">Updated first name</param>
    /// <param name="lastName">Updated last name</param>
    /// <returns>True if update successful, false otherwise</returns>
    public async Task<bool> UpdateUserAsync(Guid userId, string firstName, string lastName)
    {
        try
        {
            // Validate input
            if (userId == Guid.Empty || string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
            {
                return false;
            }

            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            // Update user information
            user.FirstName = firstName.Trim();
            user.LastName = lastName.Trim();
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
    /// Deactivates a user account
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <returns>True if deactivation successful, false otherwise</returns>
    public async Task<bool> DeactivateUserAsync(Guid userId)
    {
        try
        {
            if (userId == Guid.Empty)
            {
                return false;
            }

            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            user.IsActive = false;
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
    /// Activates a user account
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <returns>True if activation successful, false otherwise</returns>
    public async Task<bool> ActivateUserAsync(Guid userId)
    {
        try
        {
            if (userId == Guid.Empty)
            {
                return false;
            }

            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            user.IsActive = true;
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
    /// Gets a user by their identifier
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <returns>User entity or null if not found</returns>
    public async Task<User?> GetUserByIdAsync(Guid userId)
    {
        try
        {
            if (userId == Guid.Empty)
            {
                return null;
            }

            return await _unitOfWork.Users.GetByIdAsync(userId);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Gets a user by their email address
    /// </summary>
    /// <param name="email">User's email address</param>
    /// <returns>User entity or null if not found</returns>
    public async Task<User?> GetUserByEmailAsync(string email)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return null;
            }

            return await _unitOfWork.Users.GetByEmailAsync(email);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Gets all active users in the system
    /// </summary>
    /// <returns>Collection of active users</returns>
    public async Task<IEnumerable<User>> GetActiveUsersAsync()
    {
        try
        {
            return await _unitOfWork.Users.GetActiveUsersAsync();
        }
        catch
        {
            return Enumerable.Empty<User>();
        }
    }

    /// <summary>
    /// Gets all users in the system
    /// </summary>
    /// <returns>Collection of all users</returns>
    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        try
        {
            return await _unitOfWork.Users.GetAllAsync();
        }
        catch
        {
            return Enumerable.Empty<User>();
        }
    }

    /// <summary>
    /// Updates user status (active/inactive)
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="isActive">Active status</param>
    /// <returns>True if update successful, false otherwise</returns>
    public async Task<bool> UpdateUserStatusAsync(Guid userId, bool isActive)
    {
        try
        {
            if (isActive)
            {
                return await ActivateUserAsync(userId);
            }
            else
            {
                return await DeactivateUserAsync(userId);
            }
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Deletes a user from the system (soft delete by deactivating)
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>True if deletion successful, false otherwise</returns>
    public async Task<bool> DeleteUserAsync(Guid userId)
    {
        try
        {
            // For safety, we'll implement this as a soft delete (deactivation)
            // In a real system, you might want to implement hard delete or have both options
            return await DeactivateUserAsync(userId);
        }
        catch
        {
            return false;
        }
    }
}