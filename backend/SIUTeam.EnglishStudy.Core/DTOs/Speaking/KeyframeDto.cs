namespace SIUTeam.EnglishStudy.Core.DTOs;

public class KeyframeDto
{
    public int Frame { get; set; }
    public double Time { get; set; }
    public string Viseme { get; set; } = string.Empty;
    public double Weight { get; set; }
}
