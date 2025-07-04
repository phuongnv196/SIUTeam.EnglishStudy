using MongoDB.Driver;
using SIUTeam.EnglishStudy.Core.Entities;
using SIUTeam.EnglishStudy.Core.Interfaces.Repositories;
using SIUTeam.EnglishStudy.Infrastructure.Data;

namespace SIUTeam.EnglishStudy.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Course entity operations with MongoDB
/// </summary>
public class CourseRepository : GenericRepository<Course>, ICourseRepository
{
    public CourseRepository(MongoDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Gets all published courses
    /// </summary>
    /// <returns>Collection of published courses</returns>
    public async Task<IEnumerable<Course>> GetPublishedCoursesAsync()
    {
        var filter = Builders<Course>.Filter.And(
            Builders<Course>.Filter.Eq(x => x.IsPublished, true),
            Builders<Course>.Filter.Eq(x => x.IsDeleted, false)
        );
        return await _collection.Find(filter).ToListAsync();
    }

    /// <summary>
    /// Gets courses filtered by difficulty level
    /// </summary>
    /// <param name="level">The lesson level to filter by</param>
    /// <returns>Collection of courses at the specified level</returns>
    public async Task<IEnumerable<Course>> GetCoursesByLevelAsync(LessonLevel level)
    {
        var filter = Builders<Course>.Filter.And(
            Builders<Course>.Filter.Eq(x => x.Level, level),
            Builders<Course>.Filter.Eq(x => x.IsDeleted, false)
        );
        return await _collection.Find(filter).ToListAsync();
    }

    /// <summary>
    /// Gets a course with its associated lessons
    /// Note: In MongoDB, this returns the course entity. 
    /// Lessons should be loaded separately through LessonRepository
    /// </summary>
    /// <param name="courseId">The course identifier</param>
    /// <returns>Course entity or null if not found</returns>
    public async Task<Course?> GetCourseWithLessonsAsync(Guid courseId)
    {
        // In MongoDB, we would need to perform a separate query or use aggregation pipeline
        // For now, we'll return the course and let the application layer handle loading lessons
        return await GetByIdAsync(courseId);
    }
}