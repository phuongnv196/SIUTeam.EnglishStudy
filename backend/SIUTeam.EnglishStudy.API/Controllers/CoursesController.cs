using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using SIUTeam.EnglishStudy.Core.DTOs;
using SIUTeam.EnglishStudy.Core.Entities;
using SIUTeam.EnglishStudy.Core.Interfaces.Services;
using SIUTeam.EnglishStudy.Core.Authorization;
using SIUTeam.EnglishStudy.Core.Enums;

namespace SIUTeam.EnglishStudy.API.Controllers;

/// <summary>
/// Controller for managing courses
/// </summary>
[ApiController]
[Route("api/[controller]")]
[SwaggerTag("Courses management endpoints")]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;

    public CoursesController(ICourseService courseService)
    {
        _courseService = courseService;
    }

    /// <summary>
    /// Get all published courses (Public access)
    /// </summary>
    /// <returns>List of published courses</returns>
    /// <response code="200">Returns the list of courses</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Get all published courses", Description = "Retrieves a list of all published courses available for students")]
    [SwaggerResponse(200, "Success", typeof(IEnumerable<CourseDto>))]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<ActionResult<IEnumerable<CourseDto>>> GetCourses()
    {
        try
        {
            var courses = await _courseService.GetPublishedCoursesAsync();
            var courseDtos = courses.Select(course => new CourseDto(
                course.Id,
                course.Title,
                course.Description,
                course.ImageUrl,
                course.Level,
                course.IsPublished,
                course.EstimatedHours,
                0, // Lesson count would need to be calculated
                course.CreatedAt
            ));

            return Ok(courseDtos);
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occurred while retrieving courses.");
        }
    }

    /// <summary>
    /// Get courses by level (Authenticated users)
    /// </summary>
    /// <param name="level">Course difficulty level</param>
    /// <returns>List of courses at specified level</returns>
    /// <response code="200">Returns the list of courses</response>
    /// <response code="401">If user is not authenticated</response>
    [HttpGet("level/{level}")]
    [Authorize]
    [RequirePermission(Permission.ReadCourse)]
    [SwaggerOperation(Summary = "Get courses by level", Description = "Retrieves courses filtered by difficulty level")]
    [SwaggerResponse(200, "Success", typeof(IEnumerable<CourseDto>))]
    [SwaggerResponse(401, "Unauthorized")]
    public async Task<ActionResult<IEnumerable<CourseDto>>> GetCoursesByLevel(LessonLevel level)
    {
        try
        {
            var courses = await _courseService.GetCoursesByLevelAsync(level);
            var courseDtos = courses.Select(course => new CourseDto(
                course.Id,
                course.Title,
                course.Description,
                course.ImageUrl,
                course.Level,
                course.IsPublished,
                course.EstimatedHours,
                0, // Lesson count would need to be calculated
                course.CreatedAt
            ));

            return Ok(courseDtos);
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occurred while retrieving courses by level.");
        }
    }

    /// <summary>
    /// Get a specific course by ID
    /// </summary>
    /// <param name="id">The course ID</param>
    /// <returns>Course details</returns>
    /// <response code="200">Returns the course</response>
    /// <response code="404">If the course is not found</response>
    /// <response code="401">If user is not authenticated</response>
    [HttpGet("{id}")]
    [Authorize]
    [RequirePermission(Permission.ReadCourse)]
    [SwaggerOperation(Summary = "Get course by ID", Description = "Retrieves detailed information about a specific course")]
    [SwaggerResponse(200, "Success", typeof(CourseDto))]
    [SwaggerResponse(404, "Course not found")]
    [SwaggerResponse(401, "Unauthorized")]
    public async Task<ActionResult<CourseDto>> GetCourse(Guid id)
    {
        try
        {
            var course = await _courseService.GetCourseByIdAsync(id);
            if (course == null)
            {
                return NotFound("Course not found.");
            }

            var courseDto = new CourseDto(
                course.Id,
                course.Title,
                course.Description,
                course.ImageUrl,
                course.Level,
                course.IsPublished,
                course.EstimatedHours,
                0, // Lesson count would need to be calculated
                course.CreatedAt
            );

            return Ok(courseDto);
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occurred while retrieving the course.");
        }
    }

    /// <summary>
    /// Create a new course (Teacher and Admin only)
    /// </summary>
    /// <param name="request">Course creation request</param>
    /// <returns>Created course</returns>
    /// <response code="201">Course created successfully</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="403">If user doesn't have permission</response>
    [HttpPost]
    [Authorize]
    [RequirePermission(Permission.CreateCourse)]
    [SwaggerOperation(Summary = "Create a new course", Description = "Creates a new course in the system (Teacher and Admin only)")]
    [SwaggerResponse(201, "Course created successfully", typeof(CourseDto))]
    [SwaggerResponse(400, "Invalid request")]
    [SwaggerResponse(403, "Forbidden")]
    public async Task<ActionResult<CourseDto>> CreateCourse([FromBody] CreateCourseRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title) || 
            string.IsNullOrWhiteSpace(request.Description) ||
            request.EstimatedHours <= 0)
        {
            return BadRequest("Title, description, and estimated hours are required.");
        }

        try
        {
            var courseId = await _courseService.CreateCourseAsync(
                request.Title,
                request.Description,
                request.Level,
                request.EstimatedHours
            );

            if (courseId == Guid.Empty)
            {
                return BadRequest("Failed to create course.");
            }

            var course = await _courseService.GetCourseByIdAsync(courseId);
            if (course == null)
            {
                return StatusCode(500, "Course was created but could not be retrieved.");
            }

            var courseDto = new CourseDto(
                course.Id,
                course.Title,
                course.Description,
                course.ImageUrl,
                course.Level,
                course.IsPublished,
                course.EstimatedHours,
                0,
                course.CreatedAt
            );

            return CreatedAtAction(nameof(GetCourse), new { id = course.Id }, courseDto);
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occurred while creating the course.");
        }
    }

    /// <summary>
    /// Update an existing course (Teacher and Admin only)
    /// </summary>
    /// <param name="id">Course ID</param>
    /// <param name="request">Course update request</param>
    /// <returns>Updated course</returns>
    /// <response code="200">Course updated successfully</response>
    /// <response code="404">Course not found</response>
    /// <response code="403">If user doesn't have permission</response>
    [HttpPut("{id}")]
    [Authorize]
    [RequirePermission(Permission.UpdateCourse)]
    [SwaggerOperation(Summary = "Update course", Description = "Updates an existing course (Teacher and Admin only)")]
    [SwaggerResponse(200, "Course updated successfully", typeof(CourseDto))]
    [SwaggerResponse(404, "Course not found")]
    [SwaggerResponse(403, "Forbidden")]
    public async Task<ActionResult<CourseDto>> UpdateCourse(Guid id, [FromBody] UpdateCourseRequest request)
    {
        try
        {
            var success = await _courseService.UpdateCourseAsync(
                id,
                request.Title,
                request.Description,
                request.Level,
                request.EstimatedHours
            );

            if (!success)
            {
                return NotFound("Course not found or update failed.");
            }

            var course = await _courseService.GetCourseByIdAsync(id);
            if (course == null)
            {
                return NotFound("Course not found after update.");
            }

            var courseDto = new CourseDto(
                course.Id,
                course.Title,
                course.Description,
                course.ImageUrl,
                course.Level,
                course.IsPublished,
                course.EstimatedHours,
                0,
                course.CreatedAt
            );

            return Ok(courseDto);
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occurred while updating the course.");
        }
    }

    /// <summary>
    /// Publish a course (Teacher and Admin only)
    /// </summary>
    /// <param name="id">Course ID</param>
    /// <returns>Success message</returns>
    /// <response code="200">Course published successfully</response>
    /// <response code="404">Course not found</response>
    /// <response code="403">If user doesn't have permission</response>
    [HttpPost("{id}/publish")]
    [Authorize]
    [RequirePermission(Permission.PublishCourse)]
    [SwaggerOperation(Summary = "Publish course", Description = "Publishes a course making it available to students")]
    [SwaggerResponse(200, "Course published successfully")]
    [SwaggerResponse(404, "Course not found")]
    [SwaggerResponse(403, "Forbidden")]
    public async Task<ActionResult> PublishCourse(Guid id)
    {
        try
        {
            var success = await _courseService.PublishCourseAsync(id);
            if (!success)
            {
                return NotFound("Course not found or cannot be published.");
            }

            return Ok(new { message = "Course published successfully" });
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occurred while publishing the course.");
        }
    }

    /// <summary>
    /// Unpublish a course (Admin only)
    /// </summary>
    /// <param name="id">Course ID</param>
    /// <returns>Success message</returns>
    /// <response code="200">Course unpublished successfully</response>
    /// <response code="404">Course not found</response>
    /// <response code="403">If user doesn't have permission</response>
    [HttpPost("{id}/unpublish")]
    [Authorize]
    [RequireRole(UserRole.Admin)]
    [SwaggerOperation(Summary = "Unpublish course", Description = "Unpublishes a course (Admin only)")]
    [SwaggerResponse(200, "Course unpublished successfully")]
    [SwaggerResponse(404, "Course not found")]
    [SwaggerResponse(403, "Forbidden")]
    public async Task<ActionResult> UnpublishCourse(Guid id)
    {
        try
        {
            var success = await _courseService.UnpublishCourseAsync(id);
            if (!success)
            {
                return NotFound("Course not found or cannot be unpublished.");
            }

            return Ok(new { message = "Course unpublished successfully" });
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occurred while unpublishing the course.");
        }
    }
}

/// <summary>
/// Request model for creating a new course
/// </summary>
public record CreateCourseRequest
{
    /// <summary>
    /// Course title
    /// </summary>
    /// <example>Basic English Grammar</example>
    public string Title { get; init; } = string.Empty;

    /// <summary>
    /// Course description
    /// </summary>
    /// <example>Learn fundamental English grammar rules and improve your writing skills</example>
    public string Description { get; init; } = string.Empty;

    /// <summary>
    /// Course image URL
    /// </summary>
    /// <example>https://example.com/course-image.jpg</example>
    public string ImageUrl { get; init; } = string.Empty;

    /// <summary>
    /// Course difficulty level
    /// </summary>
    /// <example>Beginner</example>
    public LessonLevel Level { get; init; }

    /// <summary>
    /// Estimated hours to complete the course
    /// </summary>
    /// <example>20</example>
    public int EstimatedHours { get; init; }
}

/// <summary>
/// Request model for updating a course
/// </summary>
public record UpdateCourseRequest
{
    /// <summary>
    /// Updated course title
    /// </summary>
    /// <example>Advanced English Grammar</example>
    public string Title { get; init; } = string.Empty;

    /// <summary>
    /// Updated course description
    /// </summary>
    /// <example>Master advanced English grammar concepts</example>
    public string Description { get; init; } = string.Empty;

    /// <summary>
    /// Updated course difficulty level
    /// </summary>
    /// <example>Advanced</example>
    public LessonLevel Level { get; init; }

    /// <summary>
    /// Updated estimated hours to complete the course
    /// </summary>
    /// <example>30</example>
    public int EstimatedHours { get; init; }
}