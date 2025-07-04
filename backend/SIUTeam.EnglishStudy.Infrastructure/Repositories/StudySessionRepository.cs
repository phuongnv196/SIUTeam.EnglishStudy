using MongoDB.Driver;
using SIUTeam.EnglishStudy.Core.Entities;
using SIUTeam.EnglishStudy.Core.Interfaces.Repositories;
using SIUTeam.EnglishStudy.Infrastructure.Data;

namespace SIUTeam.EnglishStudy.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for StudySession entity operations with MongoDB
/// </summary>
public class StudySessionRepository : GenericRepository<StudySession>, IStudySessionRepository
{
    public StudySessionRepository(MongoDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Gets all study sessions for a specific user, ordered by start time (most recent first)
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <returns>Collection of study sessions ordered by start time descending</returns>
    public async Task<IEnumerable<StudySession>> GetSessionsByUserIdAsync(Guid userId)
    {
        var filter = Builders<StudySession>.Filter.And(
            Builders<StudySession>.Filter.Eq(x => x.UserId, userId),
            Builders<StudySession>.Filter.Eq(x => x.IsDeleted, false)
        );
        var sort = Builders<StudySession>.Sort.Descending(x => x.StartTime);
        return await _collection.Find(filter).Sort(sort).ToListAsync();
    }

    /// <summary>
    /// Gets all completed study sessions for a specific user
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <returns>Collection of completed study sessions</returns>
    public async Task<IEnumerable<StudySession>> GetCompletedSessionsAsync(Guid userId)
    {
        var filter = Builders<StudySession>.Filter.And(
            Builders<StudySession>.Filter.Eq(x => x.UserId, userId),
            Builders<StudySession>.Filter.Eq(x => x.IsCompleted, true),
            Builders<StudySession>.Filter.Eq(x => x.IsDeleted, false)
        );
        return await _collection.Find(filter).ToListAsync();
    }

    /// <summary>
    /// Gets the active (incomplete) study session for a user and lesson
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="lessonId">The lesson identifier</param>
    /// <returns>Active study session or null if none exists</returns>
    public async Task<StudySession?> GetActiveSessionAsync(Guid userId, Guid lessonId)
    {
        var filter = Builders<StudySession>.Filter.And(
            Builders<StudySession>.Filter.Eq(x => x.UserId, userId),
            Builders<StudySession>.Filter.Eq(x => x.LessonId, lessonId),
            Builders<StudySession>.Filter.Eq(x => x.IsCompleted, false),
            Builders<StudySession>.Filter.Eq(x => x.IsDeleted, false)
        );
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }
}