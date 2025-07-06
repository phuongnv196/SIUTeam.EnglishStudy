namespace SIUTeam.EnglishStudy.Core.DTOs;

public class TextToIpaResponseDto
{
    public bool Success { get; set; }
    public string OriginalText { get; set; } = string.Empty;
    public IpaResultDto Ipa { get; set; } = new();
    public string? Error { get; set; }
}
