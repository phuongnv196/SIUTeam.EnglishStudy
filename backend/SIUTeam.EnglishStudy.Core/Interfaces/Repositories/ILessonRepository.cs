using SIUTeam.EnglishStudy.Core.Entities;

namespace SIUTeam.EnglishStudy.Core.Interfaces.Repositories
{
    public interface ILessonRepository : IGenericRepository<Lesson>
    {
        Task<IEnumerable<Lesson>> GetLessonsByCourseIdAsync(Guid courseId);
        Task<Lesson?> GetLessonWithExercisesAsync(Guid lessonId);
        Task<IEnumerable<Lesson>> GetPublishedLessonsAsync();
    }
}
