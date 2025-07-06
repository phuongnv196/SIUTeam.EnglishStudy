namespace SIUTeam.EnglishStudy.Core.DTOs;

/// <summary>
/// Data Transfer Object for Course information
/// </summary>
/// <param name="Id">Course unique identifier</param>
/// <param name="Title">Course title</param>
/// <param name="Description">Course description</param>
/// <param name="ImageUrl">Course image URL</param>
/// <param name="Level">Course difficulty level</param>
/// <param name="IsPublished">Whether the course is published</param>
/// <param name="EstimatedHours">Estimated hours to complete</param>
/// <param name="LessonCount">Number of lessons in the course</param>
/// <param name="CreatedAt">When the course was created</param>
public record CourseDto(
    Guid Id,
    string Title,
    string Description,
    string ImageUrl,
    LessonLevel Level,
    bool IsPublished,
    int EstimatedHours,
    int LessonCount,
    DateTime CreatedAt
);
