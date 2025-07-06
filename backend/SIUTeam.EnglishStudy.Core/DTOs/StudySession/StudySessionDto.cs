namespace SIUTeam.EnglishStudy.Core.DTOs;

/// <summary>
/// Data Transfer Object for Study Session information
/// </summary>
/// <param name="Id">Study session unique identifier</param>
/// <param name="StartTime">When the session started</param>
/// <param name="EndTime">When the session ended (if completed)</param>
/// <param name="Score">Current score in the session</param>
/// <param name="MaxScore">Maximum possible score</param>
/// <param name="IsCompleted">Whether the session is completed</param>
/// <param name="Duration">Total duration of the session</param>
/// <param name="UserId">ID of the user taking the session</param>
/// <param name="LessonId">ID of the lesson being studied</param>
/// <param name="LessonTitle">Title of the lesson</param>
public record StudySessionDto(
    Guid Id,
    DateTime StartTime,
    DateTime? EndTime,
    int Score,
    int MaxScore,
    bool IsCompleted,
    TimeSpan Duration,
    Guid UserId,
    Guid LessonId,
    string LessonTitle
);
