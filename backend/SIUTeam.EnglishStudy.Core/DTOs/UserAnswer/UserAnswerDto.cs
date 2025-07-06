namespace SIUTeam.EnglishStudy.Core.DTOs;

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
