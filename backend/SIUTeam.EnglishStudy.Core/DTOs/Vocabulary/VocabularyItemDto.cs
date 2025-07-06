namespace SIUTeam.EnglishStudy.Core.DTOs.Vocabulary;

public class VocabularyItemDto
{
    public string? Id { get; set; }
    public string Word { get; set; } = string.Empty;
    public string Pronunciation { get; set; } = string.Empty;
    public string Meaning { get; set; } = string.Empty;
    public string Example { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
    public bool Learned { get; set; }
    public string? UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
