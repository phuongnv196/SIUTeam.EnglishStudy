namespace SIUTeam.EnglishStudy.Core.DTOs;

public class ChunkTranscriptionResponseDto
{
    public bool Success { get; set; }
    public string Text { get; set; } = string.Empty;
    public double ProcessingTime { get; set; }
    public string Language { get; set; } = string.Empty;
    public IpaResultDto Ipa { get; set; } = new();
    public string? Error { get; set; }
}
