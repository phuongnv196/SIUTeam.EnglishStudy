using MongoDB.Bson.Serialization.Attributes;

namespace SIUTeam.EnglishStudy.Core.Entities;

[BsonCollection("courses")]
public class Course : BaseEntity
{
    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;
    
    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;
    
    [BsonElement("imageUrl")]
    public string ImageUrl { get; set; } = string.Empty;
    
    [BsonElement("level")]
    [BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public LessonLevel Level { get; set; }
    
    [BsonElement("isPublished")]
    public bool IsPublished { get; set; }
    
    [BsonElement("estimatedHours")]
    public int EstimatedHours { get; set; }
    
    // Navigation properties - store as IDs for MongoDB
    [BsonElement("lessonIds")]
    public List<Guid> LessonIds { get; set; } = new();
    
    [BsonElement("userProgressIds")]
    public List<Guid> UserProgressIds { get; set; } = new();
    
    // These properties are not stored in MongoDB, used for loading related data
    [BsonIgnore]
    public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
    
    [BsonIgnore]
    public ICollection<UserProgress> UserProgresses { get; set; } = new List<UserProgress>();
}