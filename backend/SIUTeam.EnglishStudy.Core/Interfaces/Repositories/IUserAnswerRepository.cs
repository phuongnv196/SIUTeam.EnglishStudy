using SIUTeam.EnglishStudy.Core.Entities;

namespace SIUTeam.EnglishStudy.Core.Interfaces.Repositories;

/// <summary>
/// Repository interface for UserAnswer entity operations
/// </summary>
public interface IUserAnswerRepository : IGenericRepository<UserAnswer>
{
    /// <summary>
    /// Gets all answers for a specific study session
    /// </summary>
    /// <param name="studySessionId">Study session identifier</param>
    /// <returns>Collection of user answers</returns>
    Task<IEnumerable<UserAnswer>> GetAnswersByStudySessionIdAsync(Guid studySessionId);

    /// <summary>
    /// Gets a specific answer for a study session and exercise combination
    /// </summary>
    /// <param name="studySessionId">Study session identifier</param>
    /// <param name="exerciseId">Exercise identifier</param>
    /// <returns>User answer or null if not found</returns>
    Task<UserAnswer?> GetAnswerAsync(Guid studySessionId, Guid exerciseId);

    /// <summary>
    /// Gets all answers for a specific exercise across all study sessions
    /// </summary>
    /// <param name="exerciseId">Exercise identifier</param>
    /// <returns>Collection of user answers</returns>
    Task<IEnumerable<UserAnswer>> GetAnswersByExerciseIdAsync(Guid exerciseId);

    /// <summary>
    /// Checks if an answer already exists for a study session and exercise combination
    /// </summary>
    /// <param name="studySessionId">Study session identifier</param>
    /// <param name="exerciseId">Exercise identifier</param>
    /// <returns>True if answer exists, false otherwise</returns>
    Task<bool> AnswerExistsAsync(Guid studySessionId, Guid exerciseId);
}