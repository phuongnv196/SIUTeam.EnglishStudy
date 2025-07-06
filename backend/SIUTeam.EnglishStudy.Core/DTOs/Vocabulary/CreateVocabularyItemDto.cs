namespace SIUTeam.EnglishStudy.Core.DTOs.Vocabulary;

public class CreateVocabularyItemDto
{
    public string Word { get; set; } = string.Empty;
    public string Pronunciation { get; set; } = string.Empty;
    public string Meaning { get; set; } = string.Empty;
    public string Example { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
    public bool Learned { get; set; } = false;
}
