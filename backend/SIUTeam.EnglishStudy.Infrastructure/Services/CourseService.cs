using SIUTeam.EnglishStudy.Core.Entities;
using SIUTeam.EnglishStudy.Core.Interfaces.Repositories;
using SIUTeam.EnglishStudy.Core.Interfaces.Services;

namespace SIUTeam.EnglishStudy.Infrastructure.Services;

/// <summary>
/// Service implementation for course management operations
/// </summary>
public class CourseService : ICourseService
{
    private readonly IUnitOfWork _unitOfWork;

    public CourseService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Creates a new course in the system
    /// </summary>
    /// <param name="title">Course title</param>
    /// <param name="description">Course description</param>
    /// <param name="level">Course difficulty level</param>
    /// <param name="estimatedHours">Estimated hours to complete</param>
    /// <returns>Course ID if creation successful, Guid.Empty otherwise</returns>
    public async Task<Guid> CreateCourseAsync(string title, string description, LessonLevel level, int estimatedHours)
    {
        try
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(description) || estimatedHours <= 0)
            {
                return Guid.Empty;
            }

            // Create new course
            var course = new Course
            {
                Id = Guid.NewGuid(),
                Title = title.Trim(),
                Description = description.Trim(),
                Level = level,
                EstimatedHours = estimatedHours,
                IsPublished = false, // New courses start as unpublished
                ImageUrl = string.Empty,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Courses.AddAsync(course);
            await _unitOfWork.SaveChangesAsync();

            return course.Id;
        }
        catch
        {
            return Guid.Empty;
        }
    }

    /// <summary>
    /// Updates an existing course
    /// </summary>
    /// <param name="courseId">Course identifier</param>
    /// <param name="title">Updated course title</param>
    /// <param name="description">Updated course description</param>
    /// <param name="level">Updated course difficulty level</param>
    /// <param name="estimatedHours">Updated estimated hours</param>
    /// <returns>True if update successful, false otherwise</returns>
    public async Task<bool> UpdateCourseAsync(Guid courseId, string title, string description, LessonLevel level, int estimatedHours)
    {
        try
        {
            // Validate input
            if (courseId == Guid.Empty || string.IsNullOrWhiteSpace(title) || 
                string.IsNullOrWhiteSpace(description) || estimatedHours <= 0)
            {
                return false;
            }

            var course = await _unitOfWork.Courses.GetByIdAsync(courseId);
            if (course == null)
            {
                return false;
            }

            // Update course information
            course.Title = title.Trim();
            course.Description = description.Trim();
            course.Level = level;
            course.EstimatedHours = estimatedHours;
            course.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Courses.Update(course);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Publishes a course, making it available to students
    /// </summary>
    /// <param name="courseId">Course identifier</param>
    /// <returns>True if publishing successful, false otherwise</returns>
    public async Task<bool> PublishCourseAsync(Guid courseId)
    {
        try
        {
            if (courseId == Guid.Empty)
            {
                return false;
            }

            var course = await _unitOfWork.Courses.GetByIdAsync(courseId);
            if (course == null)
            {
                return false;
            }

            // Check if course has at least one lesson before publishing
            var lessons = await _unitOfWork.Lessons.GetLessonsByCourseIdAsync(courseId);
            if (!lessons.Any())
            {
                return false; // Cannot publish course without lessons
            }

            course.IsPublished = true;
            course.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Courses.Update(course);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Unpublishes a course, making it unavailable to new students
    /// </summary>
    /// <param name="courseId">Course identifier</param>
    /// <returns>True if unpublishing successful, false otherwise</returns>
    public async Task<bool> UnpublishCourseAsync(Guid courseId)
    {
        try
        {
            if (courseId == Guid.Empty)
            {
                return false;
            }

            var course = await _unitOfWork.Courses.GetByIdAsync(courseId);
            if (course == null)
            {
                return false;
            }

            course.IsPublished = false;
            course.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Courses.Update(course);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Gets a course by its identifier
    /// </summary>
    /// <param name="courseId">Course identifier</param>
    /// <returns>Course entity or null if not found</returns>
    public async Task<Course?> GetCourseByIdAsync(Guid courseId)
    {
        try
        {
            if (courseId == Guid.Empty)
            {
                return null;
            }

            return await _unitOfWork.Courses.GetByIdAsync(courseId);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Gets all published courses
    /// </summary>
    /// <returns>Collection of published courses</returns>
    public async Task<IEnumerable<Course>> GetPublishedCoursesAsync()
    {
        try
        {
            return await _unitOfWork.Courses.GetPublishedCoursesAsync();
        }
        catch
        {
            return Enumerable.Empty<Course>();
        }
    }

    /// <summary>
    /// Gets courses filtered by difficulty level
    /// </summary>
    /// <param name="level">Course difficulty level</param>
    /// <returns>Collection of courses at the specified level</returns>
    public async Task<IEnumerable<Course>> GetCoursesByLevelAsync(LessonLevel level)
    {
        try
        {
            return await _unitOfWork.Courses.GetCoursesByLevelAsync(level);
        }
        catch
        {
            return Enumerable.Empty<Course>();
        }
    }
}