namespace SIUTeam.EnglishStudy.Core.DTOs;

public class SpeakingResultDto
{
    public string SessionId { get; set; } = string.Empty;
    public string TranscribedText { get; set; } = string.Empty;
    public string ExpectedText { get; set; } = string.Empty;
    public double ConfidenceScore { get; set; }
    public double ProcessingTime { get; set; }
    public IpaResultDto Ipa { get; set; } = new();
    public List<string> Suggestions { get; set; } = new();
}
