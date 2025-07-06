namespace SIUTeam.EnglishStudy.Core.DTOs;

public class TalkingAvatarResponseDto
{
    public bool Success { get; set; }
    public string Text { get; set; } = string.Empty;
    public string IpaText { get; set; } = string.Empty;
    public string Arpabet { get; set; } = string.Empty;
    public double ProcessingTime { get; set; }
    public AnimationDataDto Animation { get; set; } = new();
    public string? Error { get; set; }
}
