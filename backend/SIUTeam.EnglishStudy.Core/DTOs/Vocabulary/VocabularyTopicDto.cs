namespace SIUTeam.EnglishStudy.Core.DTOs.Vocabulary;

public class VocabularyTopicDto
{
    public string Key { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public int Progress { get; set; }
    public int TotalWords { get; set; }
    public int LearnedWords { get; set; }
}
