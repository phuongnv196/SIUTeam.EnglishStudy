using SIUTeam.EnglishStudy.Core.Entities;

namespace SIUTeam.EnglishStudy.Core.Interfaces.Services
{
    public interface ICourseService
    {
        Task<Guid> CreateCourseAsync(string title, string description, LessonLevel level, int estimatedHours);
        Task<bool> UpdateCourseAsync(Guid courseId, string title, string description, LessonLevel level, int estimatedHours);
        Task<bool> PublishCourseAsync(Guid courseId);
        Task<bool> UnpublishCourseAsync(Guid courseId);
        Task<Course?> GetCourseByIdAsync(Guid courseId);
        Task<IEnumerable<Course>> GetPublishedCoursesAsync();
        Task<IEnumerable<Course>> GetCoursesByLevelAsync(LessonLevel level);
    }
}
