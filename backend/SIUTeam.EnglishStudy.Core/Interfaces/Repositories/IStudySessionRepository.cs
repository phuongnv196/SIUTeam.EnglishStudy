using SIUTeam.EnglishStudy.Core.Entities;

namespace SIUTeam.EnglishStudy.Core.Interfaces.Repositories
{
    public interface IStudySessionRepository : IGenericRepository<StudySession>
    {
        Task<IEnumerable<StudySession>> GetSessionsByUserIdAsync(Guid userId);
        Task<IEnumerable<StudySession>> GetCompletedSessionsAsync(Guid userId);
        Task<StudySession?> GetActiveSessionAsync(Guid userId, Guid lessonId);
    }

}
