using SIUTeam.EnglishStudy.Core.Entities;

namespace SIUTeam.EnglishStudy.Core.DTOs;

/// <summary>
/// Data Transfer Object for User information
/// </summary>
/// <param name="Id">User unique identifier</param>
/// <param name="Email">User email address</param>
/// <param name="Username">User's username</param>
/// <param name="FirstName">User's first name</param>
/// <param name="LastName">User's last name</param>
/// <param name="Avatar">User's avatar URL</param>
/// <param name="Role">User's role in the system</param>
/// <param name="IsActive">Whether the user account is active</param>
/// <param name="CreatedAt">When the user account was created</param>
public record UserDto(
    Guid Id,
    string Email,
    string Username,
    string FirstName,
    string LastName,
    string? Avatar,
    UserRole Role,
    bool IsActive,
    DateTime CreatedAt
);

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

/// <summary>
/// Data Transfer Object for Exercise information
/// </summary>
/// <param name="Id">Exercise unique identifier</param>
/// <param name="Title">Exercise title</param>
/// <param name="Question">Exercise question</param>
/// <param name="Type">Exercise type</param>
/// <param name="Options">Exercise options (for multiple choice)</param>
/// <param name="Points">Points awarded for correct answer</param>
/// <param name="Order">Exercise order within the lesson</param>
/// <param name="LessonId">ID of the lesson this exercise belongs to</param>
public record ExerciseDto(
    Guid Id,
    string Title,
    string Question,
    ExerciseType Type,
    string Options,
    int Points,
    int Order,
    Guid LessonId
);

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

/// <summary>
/// Data Transfer Object for User Answer information
/// </summary>
/// <param name="Id">User answer unique identifier</param>
/// <param name="Answer">User's submitted answer</param>
/// <param name="IsCorrect">Whether the answer was correct</param>
/// <param name="PointsEarned">Points earned for this answer</param>
/// <param name="AnsweredAt">When the answer was submitted</param>
/// <param name="ExerciseId">Exercise identifier</param>
/// <param name="ExerciseTitle">Exercise title</param>
public record UserAnswerDto(
    Guid Id,
    string Answer,
    bool IsCorrect,
    int PointsEarned,
    DateTime AnsweredAt,
    Guid ExerciseId,
    string ExerciseTitle
);