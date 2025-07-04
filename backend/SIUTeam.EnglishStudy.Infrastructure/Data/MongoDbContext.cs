using MongoDB.Driver;
using Microsoft.Extensions.Options;
using SIUTeam.EnglishStudy.Core.Entities;
using SIUTeam.EnglishStudy.Infrastructure.Data.Configuration;
using System.Reflection;

namespace SIUTeam.EnglishStudy.Infrastructure.Data;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;
    
    public MongoDbContext(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        _database = client.GetDatabase(settings.Value.DatabaseName);
    }

    public IMongoCollection<User> Users => GetCollection<User>();
    public IMongoCollection<Course> Courses => GetCollection<Course>();
    public IMongoCollection<Lesson> Lessons => GetCollection<Lesson>();
    public IMongoCollection<Exercise> Exercises => GetCollection<Exercise>();
    public IMongoCollection<StudySession> StudySessions => GetCollection<StudySession>();
    public IMongoCollection<UserAnswer> UserAnswers => GetCollection<UserAnswer>();
    public IMongoCollection<UserProgress> UserProgresses => GetCollection<UserProgress>();

    private IMongoCollection<T> GetCollection<T>() where T : BaseEntity
    {
        var collectionName = GetCollectionName<T>();
        return _database.GetCollection<T>(collectionName);
    }

    private static string GetCollectionName<T>() where T : BaseEntity
    {
        var attribute = typeof(T).GetCustomAttribute<BsonCollectionAttribute>();
        return attribute?.CollectionName ?? typeof(T).Name.ToLowerInvariant();
    }
}