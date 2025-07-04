using MongoDB.Driver;
using SIUTeam.EnglishStudy.Core.Entities;
using SIUTeam.EnglishStudy.Core.Interfaces.Repositories;
using SIUTeam.EnglishStudy.Infrastructure.Data;

namespace SIUTeam.EnglishStudy.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Exercise entity operations with MongoDB
/// </summary>
public class ExerciseRepository : GenericRepository<Exercise>, IExerciseRepository
{
    public ExerciseRepository(MongoDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Gets all exercises for a specific lesson, ordered by their sequence
    /// </summary>
    /// <param name="lessonId">The lesson identifier</param>
    /// <returns>Collection of exercises ordered by sequence</returns>
    public async Task<IEnumerable<Exercise>> GetExercisesByLessonIdAsync(Guid lessonId)
    {
        var filter = Builders<Exercise>.Filter.And(
            Builders<Exercise>.Filter.Eq(x => x.LessonId, lessonId),
            Builders<Exercise>.Filter.Eq(x => x.IsDeleted, false)
        );
        var sort = Builders<Exercise>.Sort.Ascending(x => x.Order);
        return await _collection.Find(filter).Sort(sort).ToListAsync();
    }

    /// <summary>
    /// Gets exercises filtered by exercise type
    /// </summary>
    /// <param name="type">The exercise type to filter by</param>
    /// <returns>Collection of exercises of the specified type</returns>
    public async Task<IEnumerable<Exercise>> GetExercisesByTypeAsync(ExerciseType type)
    {
        var filter = Builders<Exercise>.Filter.And(
            Builders<Exercise>.Filter.Eq(x => x.Type, type),
            Builders<Exercise>.Filter.Eq(x => x.IsDeleted, false)
        );
        return await _collection.Find(filter).ToListAsync();
    }
}