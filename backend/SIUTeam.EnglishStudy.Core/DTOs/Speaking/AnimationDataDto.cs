namespace SIUTeam.EnglishStudy.Core.DTOs;

public class AnimationDataDto
{
    public double Duration { get; set; }
    public int Fps { get; set; }
    public int TotalFrames { get; set; }
    public int VisemesCount { get; set; }
    public int PhonemesCount { get; set; }
    public List<KeyframeDto> Keyframes { get; set; } = new();
    public List<VisemeDto> Visemes { get; set; } = new();
}
