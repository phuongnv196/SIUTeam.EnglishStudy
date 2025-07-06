namespace SIUTeam.EnglishStudy.Core.DTOs;

public class SegmentDto
{
    public int Id { get; set; }
    public double Start { get; set; }
    public double End { get; set; }
    public string Text { get; set; } = string.Empty;
}
