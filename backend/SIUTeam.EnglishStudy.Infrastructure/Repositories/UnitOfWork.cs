using SIUTeam.EnglishStudy.Core.Interfaces.Repositories;
using SIUTeam.EnglishStudy.Infrastructure.Data;

namespace SIUTeam.EnglishStudy.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly MongoDbContext _context;
    private IUserRepository? _users;
    private ICourseRepository? _courses;
    private ILessonRepository? _lessons;
    private IExerciseRepository? _exercises;
    private IStudySessionRepository? _studySessions;
    private IUserProgressRepository? _userProgresses;
    private IUserAnswerRepository? _userAnswers;

    public UnitOfWork(MongoDbContext context)
    {
        _context = context;
    }

    public IUserRepository Users => _users ??= new UserRepository(_context);
    public ICourseRepository Courses => _courses ??= new CourseRepository(_context);
    public ILessonRepository Lessons => _lessons ??= new LessonRepository(_context);
    public IExerciseRepository Exercises => _exercises ??= new ExerciseRepository(_context);
    public IStudySessionRepository StudySessions => _studySessions ??= new StudySessionRepository(_context);
    public IUserProgressRepository UserProgresses => _userProgresses ??= new UserProgressRepository(_context);
    public IUserAnswerRepository UserAnswers => _userAnswers ??= new UserAnswerRepository(_context);

    public async Task<int> SaveChangesAsync()
    {
        // In MongoDB, operations are atomic per document
        // No explicit save is needed as operations are performed immediately
        return await Task.FromResult(1);
    }

    public async Task BeginTransactionAsync()
    {
        // MongoDB transactions are supported but not implemented in this basic setup
        // For advanced scenarios, you can implement session-based transactions
        await Task.CompletedTask;
    }

    public async Task CommitTransactionAsync()
    {
        // MongoDB transactions are supported but not implemented in this basic setup
        await Task.CompletedTask;
    }

    public async Task RollbackTransactionAsync()
    {
        // MongoDB transactions are supported but not implemented in this basic setup
        await Task.CompletedTask;
    }

    public void Dispose()
    {
        // MongoDB driver handles connection pooling and disposal automatically
        // No explicit disposal needed for MongoDbContext
    }
}