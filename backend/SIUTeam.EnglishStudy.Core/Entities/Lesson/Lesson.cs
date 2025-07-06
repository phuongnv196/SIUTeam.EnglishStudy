using MongoDB.Bson.Serialization.Attributes;

namespace SIUTeam.EnglishStudy.Core.Entities;

[BsonCollection("lessons")]
public class Lesson : BaseEntity
{
    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;
    
    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;
    
    [BsonElement("content")]
    public string Content { get; set; } = string.Empty;
    
    [BsonElement("level")]
    [BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public LessonLevel Level { get; set; }
    
    [BsonElement("order")]
    public int Order { get; set; }
    
    [BsonElement("isPublished")]
    public bool IsPublished { get; set; }
    
    // Navigation properties
    [BsonElement("courseId")]
    public Guid CourseId { get; set; }
    
    [BsonElement("exerciseIds")]
    public List<Guid> ExerciseIds { get; set; } = new();
    
    [BsonElement("studySessionIds")]
    public List<Guid> StudySessionIds { get; set; } = new();
    
    // These properties are not stored in MongoDB, used for loading related data
    [BsonIgnore]
    public Course Course { get; set; } = null!;
    
    [BsonIgnore]
    public ICollection<Exercise> Exercises { get; set; } = new List<Exercise>();
    
    [BsonIgnore]
    public ICollection<StudySession> StudySessions { get; set; } = new List<StudySession>();
}
