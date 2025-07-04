using MongoDB.Driver;
using SIUTeam.EnglishStudy.Core.Entities;
using SIUTeam.EnglishStudy.Core.Interfaces.Repositories;
using SIUTeam.EnglishStudy.Infrastructure.Data;

namespace SIUTeam.EnglishStudy.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for User entity operations with MongoDB
/// </summary>
public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(MongoDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Gets a user by email address
    /// </summary>
    /// <param name="email">User's email address</param>
    /// <returns>User entity or null if not found</returns>
    public async Task<User?> GetByEmailAsync(string email)
    {
        var filter = Builders<User>.Filter.And(
            Builders<User>.Filter.Eq(x => x.Email, email.ToLowerInvariant()),
            Builders<User>.Filter.Eq(x => x.IsDeleted, false)
        );
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    /// <summary>
    /// Gets a user by username
    /// </summary>
    /// <param name="username">User's username</param>
    /// <returns>User entity or null if not found</returns>
    public async Task<User?> GetByUsernameAsync(string username)
    {
        var filter = Builders<User>.Filter.And(
            Builders<User>.Filter.Eq(x => x.Username, username.ToLowerInvariant()),
            Builders<User>.Filter.Eq(x => x.IsDeleted, false)
        );
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    /// <summary>
    /// Checks if an email address already exists in the system
    /// </summary>
    /// <param name="email">Email address to check</param>
    /// <returns>True if email exists, false otherwise</returns>
    public async Task<bool> EmailExistsAsync(string email)
    {
        var filter = Builders<User>.Filter.And(
            Builders<User>.Filter.Eq(x => x.Email, email.ToLowerInvariant()),
            Builders<User>.Filter.Eq(x => x.IsDeleted, false)
        );
        return await _collection.Find(filter).AnyAsync();
    }

    /// <summary>
    /// Checks if a username already exists in the system
    /// </summary>
    /// <param name="username">Username to check</param>
    /// <returns>True if username exists, false otherwise</returns>
    public async Task<bool> UsernameExistsAsync(string username)
    {
        var filter = Builders<User>.Filter.And(
            Builders<User>.Filter.Eq(x => x.Username, username.ToLowerInvariant()),
            Builders<User>.Filter.Eq(x => x.IsDeleted, false)
        );
        return await _collection.Find(filter).AnyAsync();
    }

    /// <summary>
    /// Gets all active users in the system
    /// </summary>
    /// <returns>Collection of active users</returns>
    public async Task<IEnumerable<User>> GetActiveUsersAsync()
    {
        var filter = Builders<User>.Filter.And(
            Builders<User>.Filter.Eq(x => x.IsActive, true),
            Builders<User>.Filter.Eq(x => x.IsDeleted, false)
        );
        return await _collection.Find(filter).ToListAsync();
    }
}