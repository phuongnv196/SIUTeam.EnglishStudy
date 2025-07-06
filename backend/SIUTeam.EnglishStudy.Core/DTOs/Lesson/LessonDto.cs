namespace SIUTeam.EnglishStudy.Core.DTOs;

/// <summary>
/// Data Transfer Object for Lesson information
/// </summary>
/// <param name="Id">Lesson unique identifier</param>
/// <param name="Title">Lesson title</param>
/// <param name="Description">Lesson description</param>
/// <param name="Content">Lesson content</param>
/// <param name="Level">Lesson difficulty level</param>
/// <param name="Order">Lesson order within the course</param>
/// <param name="IsPublished">Whether the lesson is published</param>
/// <param name="CourseId">ID of the course this lesson belongs to</param>
/// <param name="ExerciseCount">Number of exercises in the lesson</param>
public record LessonDto(
    Guid Id,
    string Title,
    string Description,
    string Content,
    LessonLevel Level,
    int Order,
    bool IsPublished,
    Guid CourseId,
    int ExerciseCount
);
