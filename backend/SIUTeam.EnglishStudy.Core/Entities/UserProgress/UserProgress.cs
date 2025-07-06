using MongoDB.Bson.Serialization.Attributes;

namespace SIUTeam.EnglishStudy.Core.Entities;

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
