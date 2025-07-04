using MongoDB.Driver;
using SIUTeam.EnglishStudy.Core.Entities;
using SIUTeam.EnglishStudy.Core.Interfaces.Repositories;
using SIUTeam.EnglishStudy.Infrastructure.Data;

namespace SIUTeam.EnglishStudy.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for UserProgress entity operations with MongoDB
/// </summary>
public class UserProgressRepository : GenericRepository<UserProgress>, IUserProgressRepository
{
    public UserProgressRepository(MongoDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Gets the progress record for a specific user and course combination
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="courseId">The course identifier</param>
    /// <returns>User progress record or null if not found</returns>
    public async Task<UserProgress?> GetUserProgressAsync(Guid userId, Guid courseId)
    {
        var filter = Builders<UserProgress>.Filter.And(
            Builders<UserProgress>.Filter.Eq(x => x.UserId, userId),
            Builders<UserProgress>.Filter.Eq(x => x.CourseId, courseId),
            Builders<UserProgress>.Filter.Eq(x => x.IsDeleted, false)
        );
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    /// <summary>
    /// Gets all progress records for a specific user across all courses
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <returns>Collection of user progress records</returns>
    public async Task<IEnumerable<UserProgress>> GetUserProgressByUserIdAsync(Guid userId)
    {
        var filter = Builders<UserProgress>.Filter.And(
            Builders<UserProgress>.Filter.Eq(x => x.UserId, userId),
            Builders<UserProgress>.Filter.Eq(x => x.IsDeleted, false)
        );
        return await _collection.Find(filter).ToListAsync();
    }
}