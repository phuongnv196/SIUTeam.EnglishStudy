using MongoDB.Bson.Serialization.Attributes;

namespace SIUTeam.EnglishStudy.Core.Entities;

[BsonCollection("exercises")]
public class Exercise : BaseEntity
{
    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;
    
    [BsonElement("question")]
    public string Question { get; set; } = string.Empty;
    
    [BsonElement("type")]
    [BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public ExerciseType Type { get; set; }
    
    [BsonElement("options")]
    public string Options { get; set; } = string.Empty; // JSON for multiple choice options
    
    [BsonElement("correctAnswer")]
    public string CorrectAnswer { get; set; } = string.Empty;
    
    [BsonElement("explanation")]
    public string Explanation { get; set; } = string.Empty;
    
    [BsonElement("points")]
    public int Points { get; set; }
    
    [BsonElement("order")]
    public int Order { get; set; }
    
    // Navigation properties
    [BsonElement("lessonId")]
    public Guid LessonId { get; set; }
    
    [BsonElement("userAnswerIds")]
    public List<Guid> UserAnswerIds { get; set; } = new();
    
    // These properties are not stored in MongoDB, used for loading related data
    [BsonIgnore]
    public Lesson Lesson { get; set; } = null!;
    
    [BsonIgnore]
    public ICollection<UserAnswer> UserAnswers { get; set; } = new List<UserAnswer>();
}
