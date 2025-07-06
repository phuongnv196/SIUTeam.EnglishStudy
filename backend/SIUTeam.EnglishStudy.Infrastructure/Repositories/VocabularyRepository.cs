using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using SIUTeam.EnglishStudy.Core.Interfaces.Repositories;
using SIUTeam.EnglishStudy.Infrastructure.Data;

namespace SIUTeam.EnglishStudy.Infrastructure.Repositories;

public class VocabularyRepository : IVocabularyRepository
{
    private readonly IMongoCollection<VocabularyItem> _vocabularyItems;

    public VocabularyRepository(MongoDbContext context)
    {
        _vocabularyItems = context.VocabularyItems;
    }

    public async Task<IEnumerable<VocabularyItem>> GetAllAsync()
    {
        return await _vocabularyItems.Find(_ => true).ToListAsync();
    }

    public async Task<IEnumerable<VocabularyItem>> GetByUserIdAsync(Guid userId)
    {
        return await _vocabularyItems
            .Find(v => v.UserId == userId || v.UserId == null)
            .ToListAsync();
    }

    public async Task<IEnumerable<VocabularyItem>> GetByTopicAsync(string topic, Guid? userId = null)
    {
        var filter = Builders<VocabularyItem>.Filter.Eq(v => v.Topic, topic);
        
        if (userId.HasValue)
        {
            var userFilter = Builders<VocabularyItem>.Filter.Or(
                Builders<VocabularyItem>.Filter.Eq(v => v.UserId, userId.Value),
                Builders<VocabularyItem>.Filter.Eq(v => v.UserId, null)
            );
            filter = Builders<VocabularyItem>.Filter.And(filter, userFilter);
        }
        else
        {
            filter = Builders<VocabularyItem>.Filter.And(
                filter,
                Builders<VocabularyItem>.Filter.Eq(v => v.UserId, null)
            );
        }

        return await _vocabularyItems.Find(filter).ToListAsync();
    }

    public async Task<IEnumerable<VocabularyItem>> GetByLevelAsync(VocabularyLevel level, Guid? userId = null)
    {
        var filter = Builders<VocabularyItem>.Filter.Eq(v => v.Level, level);
        
        if (userId.HasValue)
        {
            var userFilter = Builders<VocabularyItem>.Filter.Or(
                Builders<VocabularyItem>.Filter.Eq(v => v.UserId, userId.Value),
                Builders<VocabularyItem>.Filter.Eq(v => v.UserId, null)
            );
            filter = Builders<VocabularyItem>.Filter.And(filter, userFilter);
        }
        else
        {
            filter = Builders<VocabularyItem>.Filter.And(
                filter,
                Builders<VocabularyItem>.Filter.Eq(v => v.UserId, null)
            );
        }

        return await _vocabularyItems.Find(filter).ToListAsync();
    }

    public async Task<VocabularyItem?> GetByIdAsync(Guid id)
    {
        return await _vocabularyItems.Find(v => v.Id == id).FirstOrDefaultAsync();
    }

    public async Task<VocabularyItem?> GetByWordAsync(string word, Guid? userId = null)
    {
        var filter = Builders<VocabularyItem>.Filter.Eq(v => v.Word, word);
        
        if (userId.HasValue)
        {
            var userFilter = Builders<VocabularyItem>.Filter.Or(
                Builders<VocabularyItem>.Filter.Eq(v => v.UserId, userId.Value),
                Builders<VocabularyItem>.Filter.Eq(v => v.UserId, null)
            );
            filter = Builders<VocabularyItem>.Filter.And(filter, userFilter);
        }

        return await _vocabularyItems.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<VocabularyItem> CreateAsync(VocabularyItem vocabularyItem)
    {
        vocabularyItem.CreatedAt = DateTime.UtcNow;
        vocabularyItem.UpdatedAt = DateTime.UtcNow;
        await _vocabularyItems.InsertOneAsync(vocabularyItem);
        return vocabularyItem;
    }

    public async Task<VocabularyItem?> UpdateAsync(Guid id, VocabularyItem vocabularyItem)
    {
        vocabularyItem.UpdatedAt = DateTime.UtcNow;
        var result = await _vocabularyItems.ReplaceOneAsync(v => v.Id == id, vocabularyItem);
        return result.ModifiedCount > 0 ? vocabularyItem : null;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var result = await _vocabularyItems.DeleteOneAsync(v => v.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<bool> MarkAsLearnedAsync(Guid id, bool learned = true)
    {
        // First, try to find the vocabulary item
        var vocabularyItem = await _vocabularyItems.Find(v => v.Id == id).FirstOrDefaultAsync();
        if (vocabularyItem == null)
            return false;

        // If this is a public vocabulary item (UserId == null), create a user-specific copy
        if (vocabularyItem.UserId == null)
        {
            // For now, just update the public item directly
            // In a real application, you might want to create user-specific progress records
            var update = Builders<VocabularyItem>.Update
                .Set(v => v.Learned, learned)
                .Set(v => v.UpdatedAt, DateTime.UtcNow);
            
            var result = await _vocabularyItems.UpdateOneAsync(v => v.Id == id, update);
            return result.ModifiedCount > 0;
        }
        else
        {
            // For user-specific items, update normally
            var update = Builders<VocabularyItem>.Update
                .Set(v => v.Learned, learned)
                .Set(v => v.UpdatedAt, DateTime.UtcNow);
            
            var result = await _vocabularyItems.UpdateOneAsync(v => v.Id == id, update);
            return result.ModifiedCount > 0;
        }
    }

    public async Task<IEnumerable<string>> GetTopicsAsync(Guid? userId = null)
    {
        FilterDefinition<VocabularyItem> filter = Builders<VocabularyItem>.Filter.Empty;
        
        if (userId.HasValue)
        {
            filter = Builders<VocabularyItem>.Filter.Or(
                Builders<VocabularyItem>.Filter.Eq(v => v.UserId, userId.Value),
                Builders<VocabularyItem>.Filter.Eq(v => v.UserId, null)
            );
        }
        else
        {
            filter = Builders<VocabularyItem>.Filter.Eq(v => v.UserId, null);
        }

        return await _vocabularyItems
            .Distinct(v => v.Topic, filter)
            .ToListAsync();
    }

    public async Task<Dictionary<string, int>> GetTopicStatsAsync(Guid? userId = null)
    {
        var aggregate = _vocabularyItems.Aggregate();
        
        // Apply user filter if needed
        if (userId.HasValue)
        {
            aggregate = aggregate.Match(v => v.UserId == userId.Value || v.UserId == null);
        }
        else
        {
            aggregate = aggregate.Match(v => v.UserId == null);
        }

        // Group by topic and calculate stats
        var results = await aggregate
            .Group(v => v.Topic, g => new
            {
                Topic = g.Key,
                Total = g.Count(),
                Learned = g.Sum(x => x.Learned ? 1 : 0)
            })
            .ToListAsync();

        var stats = new Dictionary<string, int>();
        foreach (var result in results)
        {
            stats[$"{result.Topic}_total"] = result.Total;
            stats[$"{result.Topic}_learned"] = result.Learned;
        }

        return stats;
    }
}
