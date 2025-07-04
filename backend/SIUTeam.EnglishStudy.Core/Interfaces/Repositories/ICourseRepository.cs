using SIUTeam.EnglishStudy.Core.Entities;

namespace SIUTeam.EnglishStudy.Core.Interfaces.Repositories
{
    public interface ICourseRepository : IGenericRepository<Course>
    {
        Task<IEnumerable<Course>> GetPublishedCoursesAsync();
        Task<IEnumerable<Course>> GetCoursesByLevelAsync(LessonLevel level);
        Task<Course?> GetCourseWithLessonsAsync(Guid courseId);
    }
}
