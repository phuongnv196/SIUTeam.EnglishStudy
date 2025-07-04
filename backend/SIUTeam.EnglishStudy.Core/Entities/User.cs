using MongoDB.Bson.Serialization.Attributes;

namespace SIUTeam.EnglishStudy.Core.Entities;

[BsonCollection("users")]
public class User : BaseEntity
{
    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;
    
    [BsonElement("username")]
    public string Username { get; set; } = string.Empty;
    
    [BsonElement("passwordHash")]
    public string PasswordHash { get; set; } = string.Empty;
    
    [BsonElement("firstName")]
    public string FirstName { get; set; } = string.Empty;
    
    [BsonElement("lastName")]
    public string LastName { get; set; } = string.Empty;
    
    [BsonElement("avatar")]
    public string? Avatar { get; set; }
    
    [BsonElement("isActive")]
    public bool IsActive { get; set; } = true;
    
    [BsonElement("role")]
    [BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public UserRole Role { get; set; } = UserRole.Student;
    
    // Navigation properties - store as IDs for MongoDB
    [BsonElement("studySessionIds")]
    public List<Guid> StudySessionIds { get; set; } = new();
    
    [BsonElement("userProgressIds")]
    public List<Guid> UserProgressIds { get; set; } = new();
    
    // These properties are not stored in MongoDB, used for loading related data
    [BsonIgnore]
    public ICollection<StudySession> StudySessions { get; set; } = new List<StudySession>();
    
    [BsonIgnore]
    public ICollection<UserProgress> UserProgresses { get; set; } = new List<UserProgress>();
}

public enum UserRole
{
    Student,
    Teacher,
    Admin
}

// Attribute to specify MongoDB collection name
public class BsonCollectionAttribute : Attribute
{
    public string CollectionName { get; }

    public BsonCollectionAttribute(string collectionName)
    {
        CollectionName = collectionName;
    }
}