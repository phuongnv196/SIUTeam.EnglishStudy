using MongoDB.Driver;
using SIUTeam.EnglishStudy.Core.Entities;
using SIUTeam.EnglishStudy.Core.Interfaces.Repositories;
using SIUTeam.EnglishStudy.Infrastructure.Data;

namespace SIUTeam.EnglishStudy.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Lesson entity operations with MongoDB
/// </summary>
public class LessonRepository : GenericRepository<Lesson>, ILessonRepository
{
    public LessonRepository(MongoDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Gets all lessons for a specific course, ordered by their sequence
    /// </summary>
    /// <param name="courseId">The course identifier</param>
    /// <returns>Collection of lessons ordered by sequence</returns>
    public async Task<IEnumerable<Lesson>> GetLessonsByCourseIdAsync(Guid courseId)
    {
        var filter = Builders<Lesson>.Filter.And(
            Builders<Lesson>.Filter.Eq(x => x.CourseId, courseId),
            Builders<Lesson>.Filter.Eq(x => x.IsDeleted, false)
        );
        var sort = Builders<Lesson>.Sort.Ascending(x => x.Order);
        return await _collection.Find(filter).Sort(sort).ToListAsync();
    }

    /// <summary>
    /// Gets a lesson with its associated exercises
    /// Note: In MongoDB, this returns the lesson entity.
    /// Exercises should be loaded separately through ExerciseRepository
    /// </summary>
    /// <param name="lessonId">The lesson identifier</param>
    /// <returns>Lesson entity or null if not found</returns>
    public async Task<Lesson?> GetLessonWithExercisesAsync(Guid lessonId)
    {
        return await GetByIdAsync(lessonId);
    }

    /// <summary>
    /// Gets all published lessons across all courses
    /// </summary>
    /// <returns>Collection of published lessons</returns>
    public async Task<IEnumerable<Lesson>> GetPublishedLessonsAsync()
    {
        var filter = Builders<Lesson>.Filter.And(
            Builders<Lesson>.Filter.Eq(x => x.IsPublished, true),
            Builders<Lesson>.Filter.Eq(x => x.IsDeleted, false)
        );
        return await _collection.Find(filter).ToListAsync();
    }
}