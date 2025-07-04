using SIUTeam.EnglishStudy.Core.Entities;

namespace SIUTeam.EnglishStudy.Core.Interfaces.Services
{
    public interface ILessonService
    {
        Task<Guid> CreateLessonAsync(Guid courseId, string title, string description, string content, LessonLevel level, int order);
        Task<bool> UpdateLessonAsync(Guid lessonId, string title, string description, string content, LessonLevel level, int order);
        Task<bool> PublishLessonAsync(Guid lessonId);
        Task<Lesson?> GetLessonByIdAsync(Guid lessonId);
        Task<IEnumerable<Lesson>> GetLessonsByCourseIdAsync(Guid courseId);
    }

}
