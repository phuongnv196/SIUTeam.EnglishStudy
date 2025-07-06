namespace SIUTeam.EnglishStudy.Core.DTOs;

/// <summary>
/// Data Transfer Object for User Progress information
/// </summary>
/// <param name="Id">User progress unique identifier</param>
/// <param name="UserId">User identifier</param>
/// <param name="CourseId">Course identifier</param>
/// <param name="CourseTitle">Course title</param>
/// <param name="CompletedLessons">Number of completed lessons</param>
/// <param name="TotalLessons">Total number of lessons in course</param>
/// <param name="TotalScore">Total score achieved</param>
/// <param name="TotalPossibleScore">Total possible score</param>
/// <param name="CompletionPercentage">Completion percentage</param>
/// <param name="LastAccessedAt">Last time the course was accessed</param>
public record UserProgressDto(
    Guid Id,
    Guid UserId,
    Guid CourseId,
    string CourseTitle,
    int CompletedLessons,
    int TotalLessons,
    int TotalScore,
    int TotalPossibleScore,
    double CompletionPercentage,
    DateTime LastAccessedAt
);
