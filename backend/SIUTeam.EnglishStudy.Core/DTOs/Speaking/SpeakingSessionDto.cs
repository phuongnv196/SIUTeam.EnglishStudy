namespace SIUTeam.EnglishStudy.Core.DTOs;

public class SpeakingSessionDto
{
    public string SessionId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string LessonId { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public bool IsActive { get; set; }
}
