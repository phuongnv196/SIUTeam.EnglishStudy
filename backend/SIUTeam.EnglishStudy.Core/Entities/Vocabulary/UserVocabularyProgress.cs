using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SIUTeam.EnglishStudy.Core.Entities.Vocabulary;

public class UserVocabularyProgress
{
    [BsonId]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [BsonElement("userId")]
    public Guid UserId { get; set; }
    
    [BsonElement("vocabularyItemId")]
    public Guid VocabularyItemId { get; set; }
    
    [BsonElement("learned")]
    public bool Learned { get; set; }
    
    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
