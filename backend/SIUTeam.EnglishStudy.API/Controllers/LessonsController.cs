using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using SIUTeam.EnglishStudy.Core.Entities;
using SIUTeam.EnglishStudy.Core.DTOs;

namespace SIUTeam.EnglishStudy.API.Controllers;

/// <summary>
/// Controller for managing lessons
/// </summary>
[ApiController]
[Route("api/courses/{courseId}/[controller]")]
[SwaggerTag("Lessons management within courses")]
public class LessonsController : ControllerBase
{
    /// <summary>
    /// Get all lessons for a specific course
    /// </summary>
    /// <param name="courseId">Course ID</param>
    /// <returns>List of lessons in the course</returns>
    /// <response code="200">Returns the list of lessons</response>
    /// <response code="404">If the course is not found</response>
    [HttpGet]
    [SwaggerOperation(Summary = "Get lessons by course", Description = "Retrieves all lessons for a specific course")]
    [SwaggerResponse(200, "Success", typeof(IEnumerable<LessonDto>))]
    [SwaggerResponse(404, "Course not found")]
    public async Task<ActionResult<IEnumerable<LessonDto>>> GetLessonsByCourse(Guid courseId)
    {
        // TODO: Implement actual logic
        var lessons = new List<LessonDto>
        {
            new LessonDto(
                Guid.NewGuid(),
                "Introduction to Grammar",
                "Basic grammar concepts and rules",
                "Learn the fundamental building blocks of English grammar...",
                LessonLevel.Beginner,
                1,
                true,
                courseId,
                5
            ),
            new LessonDto(
                Guid.NewGuid(),
                "Nouns and Pronouns",
                "Understanding nouns and pronouns usage",
                "Detailed explanation of different types of nouns...",
                LessonLevel.Beginner,
                2,
                true,
                courseId,
                8
            )
        };

        return Ok(lessons);
    }

    /// <summary>
    /// Get a specific lesson
    /// </summary>
    /// <param name="courseId">Course ID</param>
    /// <param name="lessonId">Lesson ID</param>
    /// <returns>Lesson details with exercises</returns>
    /// <response code="200">Returns the lesson</response>
    /// <response code="404">If the lesson is not found</response>
    [HttpGet("{lessonId}")]
    [SwaggerOperation(Summary = "Get lesson by ID", Description = "Retrieves detailed information about a specific lesson")]
    [SwaggerResponse(200, "Success", typeof(LessonDto))]
    [SwaggerResponse(404, "Lesson not found")]
    public async Task<ActionResult<LessonDto>> GetLesson(Guid courseId, Guid lessonId)
    {
        // TODO: Implement actual logic
        var lesson = new LessonDto(
            lessonId,
            "Introduction to Grammar",
            "Basic grammar concepts and rules",
            "Learn the fundamental building blocks of English grammar...",
            LessonLevel.Beginner,
            1,
            true,
            courseId,
            5
        );

        return Ok(lesson);
    }
}

/// <summary>
/// Controller for managing study sessions
/// </summary>
[ApiController]
[Route("api/[controller]")]
[SwaggerTag("Study sessions and progress tracking")]
public class StudySessionsController : ControllerBase
{
    /// <summary>
    /// Start a new study session
    /// </summary>
    /// <param name="request">Study session start request</param>
    /// <returns>Created study session</returns>
    /// <response code="201">Study session started successfully</response>
    /// <response code="400">Invalid request</response>
    /// <response code="404">User or lesson not found</response>
    [HttpPost]
    [SwaggerOperation(Summary = "Start study session", Description = "Starts a new study session for a user and lesson")]
    [SwaggerResponse(201, "Study session started successfully", typeof(StudySessionDto))]
    [SwaggerResponse(400, "Invalid request")]
    [SwaggerResponse(404, "User or lesson not found")]
    public async Task<ActionResult<StudySessionDto>> StartSession([FromBody] StartSessionRequest request)
    {
        // TODO: Implement actual logic
        var session = new StudySessionDto(
            Guid.NewGuid(),
            DateTime.UtcNow,
            null,
            0,
            100,
            false,
            TimeSpan.Zero,
            request.UserId,
            request.LessonId,
            "Introduction to Grammar"
        );

        return CreatedAtAction(nameof(GetSession), new { id = session.Id }, session);
    }

    /// <summary>
    /// Get study session details
    /// </summary>
    /// <param name="id">Study session ID</param>
    /// <returns>Study session details</returns>
    /// <response code="200">Returns the study session</response>
    /// <response code="404">Study session not found</response>
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get study session", Description = "Retrieves details of a specific study session")]
    [SwaggerResponse(200, "Success", typeof(StudySessionDto))]
    [SwaggerResponse(404, "Study session not found")]
    public async Task<ActionResult<StudySessionDto>> GetSession(Guid id)
    {
        // TODO: Implement actual logic
        var session = new StudySessionDto(
            id,
            DateTime.UtcNow.AddHours(-1),
            DateTime.UtcNow,
            75,
            100,
            true,
            TimeSpan.FromHours(1),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Introduction to Grammar"
        );

        return Ok(session);
    }

    /// <summary>
    /// Complete a study session
    /// </summary>
    /// <param name="id">Study session ID</param>
    /// <returns>Updated study session</returns>
    /// <response code="200">Study session completed successfully</response>
    /// <response code="404">Study session not found</response>
    /// <response code="400">Study session already completed</response>
    [HttpPut("{id}/complete")]
    [SwaggerOperation(Summary = "Complete study session", Description = "Marks a study session as completed")]
    [SwaggerResponse(200, "Study session completed successfully", typeof(StudySessionDto))]
    [SwaggerResponse(400, "Study session already completed")]
    [SwaggerResponse(404, "Study session not found")]
    public async Task<ActionResult<StudySessionDto>> CompleteSession(Guid id)
    {
        // TODO: Implement actual logic
        var session = new StudySessionDto(
            id,
            DateTime.UtcNow.AddHours(-1),
            DateTime.UtcNow,
            85,
            100,
            true,
            TimeSpan.FromHours(1),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Introduction to Grammar"
        );

        return Ok(session);
    }
}

/// <summary>
/// Request model for starting a study session
/// </summary>
public record StartSessionRequest
{
    /// <summary>
    /// User ID who is starting the session
    /// </summary>
    /// <example>550e8400-e29b-41d4-a716-446655440000</example>
    public Guid UserId { get; init; }

    /// <summary>
    /// Lesson ID for the study session
    /// </summary>
    /// <example>550e8400-e29b-41d4-a716-446655440001</example>
    public Guid LessonId { get; init; }
}