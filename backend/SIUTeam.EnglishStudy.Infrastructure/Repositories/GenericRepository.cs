using MongoDB.Driver;
using SIUTeam.EnglishStudy.Core.Entities;
using SIUTeam.EnglishStudy.Core.Interfaces.Repositories;
using SIUTeam.EnglishStudy.Infrastructure.Data;
using System.Linq.Expressions;

namespace SIUTeam.EnglishStudy.Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    protected readonly IMongoCollection<T> _collection;

    public GenericRepository(MongoDbContext context)
    {
        _collection = GetCollection(context);
    }

    protected virtual IMongoCollection<T> GetCollection(MongoDbContext context)
    {
        return typeof(T).Name switch
        {
            nameof(User) => (IMongoCollection<T>)context.Users,
            nameof(Course) => (IMongoCollection<T>)context.Courses,
            nameof(Lesson) => (IMongoCollection<T>)context.Lessons,
            nameof(Exercise) => (IMongoCollection<T>)context.Exercises,
            nameof(StudySession) => (IMongoCollection<T>)context.StudySessions,
            nameof(UserAnswer) => (IMongoCollection<T>)context.UserAnswers,
            nameof(UserProgress) => (IMongoCollection<T>)context.UserProgresses,
            _ => throw new ArgumentException($"Unknown entity type: {typeof(T).Name}")
        };
    }

    public virtual async Task<T?> GetByIdAsync(Guid id)
    {
        var filter = Builders<T>.Filter.And(
            Builders<T>.Filter.Eq(x => x.Id, id),
            Builders<T>.Filter.Eq(x => x.IsDeleted, false)
        );
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        var filter = Builders<T>.Filter.Eq(x => x.IsDeleted, false);
        return await _collection.Find(filter).ToListAsync();
    }

    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> expression)
    {
        var filter = Builders<T>.Filter.And(
            Builders<T>.Filter.Where(expression),
            Builders<T>.Filter.Eq(x => x.IsDeleted, false)
        );
        return await _collection.Find(filter).ToListAsync();
    }

    public virtual async Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> expression)
    {
        var filter = Builders<T>.Filter.And(
            Builders<T>.Filter.Where(expression),
            Builders<T>.Filter.Eq(x => x.IsDeleted, false)
        );
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public virtual async Task AddAsync(T entity)
    {
        entity.Id = Guid.NewGuid();
        entity.CreatedAt = DateTime.UtcNow;
        await _collection.InsertOneAsync(entity);
    }

    public virtual async Task AddRangeAsync(IEnumerable<T> entities)
    {
        var entitiesList = entities.ToList();
        foreach (var entity in entitiesList)
        {
            entity.Id = Guid.NewGuid();
            entity.CreatedAt = DateTime.UtcNow;
        }
        await _collection.InsertManyAsync(entitiesList);
    }

    public virtual void Update(T entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        var filter = Builders<T>.Filter.Eq(x => x.Id, entity.Id);
        _collection.ReplaceOne(filter, entity);
    }

    public virtual void Remove(T entity)
    {
        // Soft delete
        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        Update(entity);
    }

    public virtual void RemoveRange(IEnumerable<T> entities)
    {
        foreach (var entity in entities)
        {
            Remove(entity);
        }
    }

    public virtual async Task<bool> ExistsAsync(Guid id)
    {
        var filter = Builders<T>.Filter.And(
            Builders<T>.Filter.Eq(x => x.Id, id),
            Builders<T>.Filter.Eq(x => x.IsDeleted, false)
        );
        return await _collection.Find(filter).AnyAsync();
    }

    public virtual async Task<int> CountAsync()
    {
        var filter = Builders<T>.Filter.Eq(x => x.IsDeleted, false);
        return (int)await _collection.CountDocumentsAsync(filter);
    }

    public virtual async Task<int> CountAsync(Expression<Func<T, bool>> expression)
    {
        var filter = Builders<T>.Filter.And(
            Builders<T>.Filter.Where(expression),
            Builders<T>.Filter.Eq(x => x.IsDeleted, false)
        );
        return (int)await _collection.CountDocumentsAsync(filter);
    }
}