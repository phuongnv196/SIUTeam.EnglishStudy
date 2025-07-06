namespace SIUTeam.EnglishStudy.Core.DTOs;

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
