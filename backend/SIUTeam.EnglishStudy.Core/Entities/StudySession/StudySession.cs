using MongoDB.Bson.Serialization.Attributes;

namespace SIUTeam.EnglishStudy.Core.Entities;

[BsonCollection("studySessions")]
public class StudySession : BaseEntity
{
    [BsonElement("startTime")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime StartTime { get; set; }
    
    [BsonElement("endTime")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? EndTime { get; set; }
    
    [BsonElement("score")]
    public int Score { get; set; }
    
    [BsonElement("maxScore")]
    public int MaxScore { get; set; }
    
    [BsonElement("isCompleted")]
    public bool IsCompleted { get; set; }
    
    [BsonElement("duration")]
    [BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public TimeSpan Duration { get; set; }
    
    // Navigation properties
    [BsonElement("userId")]
    public Guid UserId { get; set; }
    
    [BsonElement("lessonId")]
    public Guid LessonId { get; set; }
    
    [BsonElement("userAnswerIds")]
    public List<Guid> UserAnswerIds { get; set; } = new();
    
    // These properties are not stored in MongoDB, used for loading related data
    [BsonIgnore]
    public User User { get; set; } = null!;
    
    [BsonIgnore]
    public Lesson Lesson { get; set; } = null!;
    
    [BsonIgnore]
    public ICollection<UserAnswer> UserAnswers { get; set; } = new List<UserAnswer>();
}
