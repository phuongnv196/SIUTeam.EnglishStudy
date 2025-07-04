using MongoDB.Driver;
using SIUTeam.EnglishStudy.Core.Entities;
using SIUTeam.EnglishStudy.Core.Interfaces.Repositories;
using SIUTeam.EnglishStudy.Infrastructure.Data;

namespace SIUTeam.EnglishStudy.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for UserAnswer entity operations with MongoDB
/// </summary>
public class UserAnswerRepository : GenericRepository<UserAnswer>, IUserAnswerRepository
{
    public UserAnswerRepository(MongoDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Gets all answers for a specific study session
    /// </summary>
    /// <param name="studySessionId">Study session identifier</param>
    /// <returns>Collection of user answers</returns>
    public async Task<IEnumerable<UserAnswer>> GetAnswersByStudySessionIdAsync(Guid studySessionId)
    {
        var filter = Builders<UserAnswer>.Filter.And(
            Builders<UserAnswer>.Filter.Eq(x => x.StudySessionId, studySessionId),
            Builders<UserAnswer>.Filter.Eq(x => x.IsDeleted, false)
        );
        var sort = Builders<UserAnswer>.Sort.Ascending(x => x.AnsweredAt);
        return await _collection.Find(filter).Sort(sort).ToListAsync();
    }

    /// <summary>
    /// Gets a specific answer for a study session and exercise combination
    /// </summary>
    /// <param name="studySessionId">Study session identifier</param>
    /// <param name="exerciseId">Exercise identifier</param>
    /// <returns>User answer or null if not found</returns>
    public async Task<UserAnswer?> GetAnswerAsync(Guid studySessionId, Guid exerciseId)
    {
        var filter = Builders<UserAnswer>.Filter.And(
            Builders<UserAnswer>.Filter.Eq(x => x.StudySessionId, studySessionId),
            Builders<UserAnswer>.Filter.Eq(x => x.ExerciseId, exerciseId),
            Builders<UserAnswer>.Filter.Eq(x => x.IsDeleted, false)
        );
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    /// <summary>
    /// Gets all answers for a specific exercise across all study sessions
    /// </summary>
    /// <param name="exerciseId">Exercise identifier</param>
    /// <returns>Collection of user answers</returns>
    public async Task<IEnumerable<UserAnswer>> GetAnswersByExerciseIdAsync(Guid exerciseId)
    {
        var filter = Builders<UserAnswer>.Filter.And(
            Builders<UserAnswer>.Filter.Eq(x => x.ExerciseId, exerciseId),
            Builders<UserAnswer>.Filter.Eq(x => x.IsDeleted, false)
        );
        var sort = Builders<UserAnswer>.Sort.Descending(x => x.AnsweredAt);
        return await _collection.Find(filter).Sort(sort).ToListAsync();
    }

    /// <summary>
    /// Checks if an answer already exists for a study session and exercise combination
    /// </summary>
    /// <param name="studySessionId">Study session identifier</param>
    /// <param name="exerciseId">Exercise identifier</param>
    /// <returns>True if answer exists, false otherwise</returns>
    public async Task<bool> AnswerExistsAsync(Guid studySessionId, Guid exerciseId)
    {
        var filter = Builders<UserAnswer>.Filter.And(
            Builders<UserAnswer>.Filter.Eq(x => x.StudySessionId, studySessionId),
            Builders<UserAnswer>.Filter.Eq(x => x.ExerciseId, exerciseId),
            Builders<UserAnswer>.Filter.Eq(x => x.IsDeleted, false)
        );
        return await _collection.Find(filter).AnyAsync();
    }
}