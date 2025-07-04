using SIUTeam.EnglishStudy.Core.Entities;

namespace SIUTeam.EnglishStudy.Core.Interfaces.Repositories
{
    public interface IExerciseRepository : IGenericRepository<Exercise>
    {
        Task<IEnumerable<Exercise>> GetExercisesByLessonIdAsync(Guid lessonId);
        Task<IEnumerable<Exercise>> GetExercisesByTypeAsync(ExerciseType type);
    }
}
