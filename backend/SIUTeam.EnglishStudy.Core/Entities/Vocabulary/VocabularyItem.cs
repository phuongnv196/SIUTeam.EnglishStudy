using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SIUTeam.EnglishStudy.Core.Entities;

[BsonCollection("vocabulary_items")]
public class VocabularyItem : BaseEntity
{
    [BsonElement("word")]
    public string Word { get; set; } = string.Empty;

    [BsonElement("pronunciation")]
    public string Pronunciation { get; set; } = string.Empty;

    [BsonElement("meaning")]
    public string Meaning { get; set; } = string.Empty;

    [BsonElement("example")]
    public string Example { get; set; } = string.Empty;

    [BsonElement("level")]
    public VocabularyLevel Level { get; set; }

    [BsonElement("topic")]
    public string Topic { get; set; } = string.Empty;

    [BsonElement("learned")]
    public bool Learned { get; set; }

    [BsonElement("user_id")]
    [BsonRepresentation(BsonType.String)]
    public Guid? UserId { get; set; }
}
