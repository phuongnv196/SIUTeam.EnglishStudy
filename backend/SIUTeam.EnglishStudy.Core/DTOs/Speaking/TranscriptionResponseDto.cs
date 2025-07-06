namespace SIUTeam.EnglishStudy.Core.DTOs;

public class TranscriptionResponseDto
{
    public bool Success { get; set; }
    public string Filename { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public double LanguageProbability { get; set; }
    public double Duration { get; set; }
    public double ProcessingTime { get; set; }
    public List<SegmentDto> Segments { get; set; } = new();
    public string Model { get; set; } = string.Empty;
    public IpaResultDto Ipa { get; set; } = new();
    public string? Error { get; set; }
}
