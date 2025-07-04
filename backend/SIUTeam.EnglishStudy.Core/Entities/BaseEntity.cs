using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SIUTeam.EnglishStudy.Core.Entities;

public abstract class BaseEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [BsonElement("createdAt")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [BsonElement("updatedAt")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? UpdatedAt { get; set; }
    
    [BsonElement("createdBy")]
    public string? CreatedBy { get; set; }
    
    [BsonElement("updatedBy")]
    public string? UpdatedBy { get; set; }
    
    [BsonElement("isDeleted")]
    public bool IsDeleted { get; set; } = false;
}