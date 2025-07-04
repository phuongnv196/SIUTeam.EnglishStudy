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

[BsonCollection("userProgress")]
public class UserProgress : BaseEntity
{
    [BsonElement("completedLessons")]
    public int CompletedLessons { get; set; }
    
    [BsonElement("totalLessons")]
    public int TotalLessons { get; set; }
    
    [BsonElement("totalScore")]
    public int TotalScore { get; set; }
    
    [BsonElement("totalPossibleScore")]
    public int TotalPossibleScore { get; set; }
    
    [BsonElement("completionPercentage")]
    public double CompletionPercentage { get; set; }
    
    [BsonElement("lastAccessedAt")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime LastAccessedAt { get; set; }
    
    // Navigation properties
    [BsonElement("userId")]
    public Guid UserId { get; set; }
    
    [BsonElement("courseId")]
    public Guid CourseId { get; set; }
    
    // These properties are not stored in MongoDB, used for loading related data
    [BsonIgnore]
    public User User { get; set; } = null!;
    
    [BsonIgnore]
    public Course Course { get; set; } = null!;
}