namespace SIUTeam.EnglishStudy.Core.DTOs.Vocabulary;

public class UpdateVocabularyItemDto
{
    public string? Word { get; set; }
    public string? Pronunciation { get; set; }
    public string? Meaning { get; set; }
    public string? Example { get; set; }
    public string? Level { get; set; }
    public string? Topic { get; set; }
    public bool? Learned { get; set; }
}
