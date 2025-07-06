using MongoDB.Bson.Serialization.Attributes;

namespace SIUTeam.EnglishStudy.Core.Entities;

[BsonCollection("userAnswers")]
public class UserAnswer : BaseEntity
{
    [BsonElement("answer")]
    public string Answer { get; set; } = string.Empty;
    
    [BsonElement("isCorrect")]
    public bool IsCorrect { get; set; }
    
    [BsonElement("pointsEarned")]
    public int PointsEarned { get; set; }
    
    [BsonElement("answeredAt")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime AnsweredAt { get; set; }
    
    // Navigation properties
    [BsonElement("studySessionId")]
    public Guid StudySessionId { get; set; }
    
    [BsonElement("exerciseId")]
    public Guid ExerciseId { get; set; }
    
    // These properties are not stored in MongoDB, used for loading related data
    [BsonIgnore]
    public StudySession StudySession { get; set; } = null!;
    
    [BsonIgnore]
    public Exercise Exercise { get; set; } = null!;
}
