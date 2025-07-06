namespace SIUTeam.EnglishStudy.Core.DTOs;

public class IpaResultDto
{
    public string G2pIpa { get; set; } = string.Empty;
    public string EpitranIpa { get; set; } = string.Empty;
    public string Arpabet { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string? Error { get; set; }
}
