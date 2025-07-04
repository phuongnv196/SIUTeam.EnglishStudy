using SIUTeam.EnglishStudy.Core.Entities;
using SIUTeam.EnglishStudy.Core.Interfaces.Repositories;
using SIUTeam.EnglishStudy.Core.Interfaces.Services;

namespace SIUTeam.EnglishStudy.Infrastructure.Services;

/// <summary>
/// Service implementation for lesson management operations
/// </summary>
public class LessonService : ILessonService
{
    private readonly IUnitOfWork _unitOfWork;

    public LessonService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Creates a new lesson in the specified course
    /// </summary>
    /// <param name="courseId">Course identifier</param>
    /// <param name="title">Lesson title</param>
    /// <param name="description">Lesson description</param>
    /// <param name="content">Lesson content</param>
    /// <param name="level">Lesson difficulty level</param>
    /// <param name="order">Lesson order within the course</param>
    /// <returns>Lesson ID if creation successful, Guid.Empty otherwise</returns>
    public async Task<Guid> CreateLessonAsync(Guid courseId, string title, string description, string content, LessonLevel level, int order)
    {
        try
        {
            // Validate input
            if (courseId == Guid.Empty || string.IsNullOrWhiteSpace(title) || 
                string.IsNullOrWhiteSpace(description) || string.IsNullOrWhiteSpace(content) || order <= 0)
            {
                return Guid.Empty;
            }

            // Verify that the course exists
            var course = await _unitOfWork.Courses.GetByIdAsync(courseId);
            if (course == null)
            {
                return Guid.Empty;
            }

            // Check if a lesson with the same order already exists in this course
            var existingLessons = await _unitOfWork.Lessons.GetLessonsByCourseIdAsync(courseId);
            if (existingLessons.Any(l => l.Order == order))
            {
                return Guid.Empty; // Order must be unique within a course
            }

            // Create new lesson
            var lesson = new Lesson
            {
                Id = Guid.NewGuid(),
                CourseId = courseId,
                Title = title.Trim(),
                Description = description.Trim(),
                Content = content.Trim(),
                Level = level,
                Order = order,
                IsPublished = false, // New lessons start as unpublished
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Lessons.AddAsync(lesson);
            await _unitOfWork.SaveChangesAsync();

            return lesson.Id;
        }
        catch
        {
            return Guid.Empty;
        }
    }

    /// <summary>
    /// Updates an existing lesson
    /// </summary>
    /// <param name="lessonId">Lesson identifier</param>
    /// <param name="title">Updated lesson title</param>
    /// <param name="description">Updated lesson description</param>
    /// <param name="content">Updated lesson content</param>
    /// <param name="level">Updated lesson difficulty level</param>
    /// <param name="order">Updated lesson order</param>
    /// <returns>True if update successful, false otherwise</returns>
    public async Task<bool> UpdateLessonAsync(Guid lessonId, string title, string description, string content, LessonLevel level, int order)
    {
        try
        {
            // Validate input
            if (lessonId == Guid.Empty || string.IsNullOrWhiteSpace(title) || 
                string.IsNullOrWhiteSpace(description) || string.IsNullOrWhiteSpace(content) || order <= 0)
            {
                return false;
            }

            var lesson = await _unitOfWork.Lessons.GetByIdAsync(lessonId);
            if (lesson == null)
            {
                return false;
            }

            // If order is being changed, check for conflicts
            if (lesson.Order != order)
            {
                var existingLessons = await _unitOfWork.Lessons.GetLessonsByCourseIdAsync(lesson.CourseId);
                if (existingLessons.Any(l => l.Id != lessonId && l.Order == order))
                {
                    return false; // Order must be unique within a course
                }
            }

            // Update lesson information
            lesson.Title = title.Trim();
            lesson.Description = description.Trim();
            lesson.Content = content.Trim();
            lesson.Level = level;
            lesson.Order = order;
            lesson.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Lessons.Update(lesson);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Publishes a lesson, making it available to students
    /// </summary>
    /// <param name="lessonId">Lesson identifier</param>
    /// <returns>True if publishing successful, false otherwise</returns>
    public async Task<bool> PublishLessonAsync(Guid lessonId)
    {
        try
        {
            if (lessonId == Guid.Empty)
            {
                return false;
            }

            var lesson = await _unitOfWork.Lessons.GetByIdAsync(lessonId);
            if (lesson == null)
            {
                return false;
            }

            // Verify that the parent course exists and is published
            var course = await _unitOfWork.Courses.GetByIdAsync(lesson.CourseId);
            if (course == null || !course.IsPublished)
            {
                return false; // Cannot publish lesson if course is not published
            }

            lesson.IsPublished = true;
            lesson.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Lessons.Update(lesson);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Gets a lesson by its identifier
    /// </summary>
    /// <param name="lessonId">Lesson identifier</param>
    /// <returns>Lesson entity or null if not found</returns>
    public async Task<Lesson?> GetLessonByIdAsync(Guid lessonId)
    {
        try
        {
            if (lessonId == Guid.Empty)
            {
                return null;
            }

            return await _unitOfWork.Lessons.GetByIdAsync(lessonId);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Gets all lessons for a specific course, ordered by their sequence
    /// </summary>
    /// <param name="courseId">Course identifier</param>
    /// <returns>Collection of lessons ordered by sequence</returns>
    public async Task<IEnumerable<Lesson>> GetLessonsByCourseIdAsync(Guid courseId)
    {
        try
        {
            if (courseId == Guid.Empty)
            {
                return Enumerable.Empty<Lesson>();
            }

            return await _unitOfWork.Lessons.GetLessonsByCourseIdAsync(courseId);
        }
        catch
        {
            return Enumerable.Empty<Lesson>();
        }
    }
}