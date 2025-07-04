namespace SIUTeam.EnglishStudy.Core.Interfaces.Repositories;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    ICourseRepository Courses { get; }
    ILessonRepository Lessons { get; }
    IExerciseRepository Exercises { get; }
    IStudySessionRepository StudySessions { get; }
    IUserProgressRepository UserProgresses { get; }
    IUserAnswerRepository UserAnswers { get; }
    
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}