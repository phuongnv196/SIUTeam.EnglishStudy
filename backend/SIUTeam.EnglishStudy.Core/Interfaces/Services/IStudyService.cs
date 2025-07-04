using SIUTeam.EnglishStudy.Core.Entities;

namespace SIUTeam.EnglishStudy.Core.Interfaces.Services
{
    public interface IStudyService
    {
        Task<Guid> StartStudySessionAsync(Guid userId, Guid lessonId);
        Task<bool> SubmitAnswerAsync(Guid studySessionId, Guid exerciseId, string answer);
        Task<bool> CompleteStudySessionAsync(Guid studySessionId);
        Task<StudySession?> GetActiveSessionAsync(Guid userId, Guid lessonId);
        Task<IEnumerable<StudySession>> GetUserStudyHistoryAsync(Guid userId);
        Task UpdateUserProgressAsync(Guid userId, Guid courseId);
    }
}
