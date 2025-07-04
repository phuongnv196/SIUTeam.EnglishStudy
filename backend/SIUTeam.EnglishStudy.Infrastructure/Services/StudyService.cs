using SIUTeam.EnglishStudy.Core.Entities;
using SIUTeam.EnglishStudy.Core.Interfaces.Repositories;
using SIUTeam.EnglishStudy.Core.Interfaces.Services;

namespace SIUTeam.EnglishStudy.Infrastructure.Services;

/// <summary>
/// Service implementation for study session and progress tracking operations
/// </summary>
public class StudyService : IStudyService
{
    private readonly IUnitOfWork _unitOfWork;

    public StudyService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Starts a new study session for a user and lesson
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="lessonId">Lesson identifier</param>
    /// <returns>Study session ID if creation successful, Guid.Empty otherwise</returns>
    public async Task<Guid> StartStudySessionAsync(Guid userId, Guid lessonId)
    {
        try
        {
            // Validate input
            if (userId == Guid.Empty || lessonId == Guid.Empty)
            {
                return Guid.Empty;
            }

            // Verify user exists and is active
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null || !user.IsActive)
            {
                return Guid.Empty;
            }

            // Verify lesson exists and is published
            var lesson = await _unitOfWork.Lessons.GetByIdAsync(lessonId);
            if (lesson == null || !lesson.IsPublished)
            {
                return Guid.Empty;
            }

            // Check if there's already an active session for this user and lesson
            var existingSession = await _unitOfWork.StudySessions.GetActiveSessionAsync(userId, lessonId);
            if (existingSession != null)
            {
                return existingSession.Id; // Return existing session ID
            }

            // Get exercises for the lesson to calculate max score
            var exercises = await _unitOfWork.Exercises.GetExercisesByLessonIdAsync(lessonId);
            var maxScore = exercises.Sum(e => e.Points);

            // Create new study session
            var studySession = new StudySession
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                LessonId = lessonId,
                StartTime = DateTime.UtcNow,
                Score = 0,
                MaxScore = maxScore,
                IsCompleted = false,
                Duration = TimeSpan.Zero,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.StudySessions.AddAsync(studySession);
            await _unitOfWork.SaveChangesAsync();

            return studySession.Id;
        }
        catch
        {
            return Guid.Empty;
        }
    }

    /// <summary>
    /// Submits an answer for an exercise in a study session
    /// </summary>
    /// <param name="studySessionId">Study session identifier</param>
    /// <param name="exerciseId">Exercise identifier</param>
    /// <param name="answer">User's answer</param>
    /// <returns>True if answer submitted successfully, false otherwise</returns>
    public async Task<bool> SubmitAnswerAsync(Guid studySessionId, Guid exerciseId, string answer)
    {
        try
        {
            // Validate input
            if (studySessionId == Guid.Empty || exerciseId == Guid.Empty || string.IsNullOrWhiteSpace(answer))
            {
                return false;
            }

            // Get study session
            var studySession = await _unitOfWork.StudySessions.GetByIdAsync(studySessionId);
            if (studySession == null || studySession.IsCompleted)
            {
                return false;
            }

            // Get exercise
            var exercise = await _unitOfWork.Exercises.GetByIdAsync(exerciseId);
            if (exercise == null || exercise.LessonId != studySession.LessonId)
            {
                return false;
            }

            // Check if answer already exists for this exercise in this session
            var existingAnswers = await _unitOfWork.StudySessions.GetByIdAsync(studySessionId);
            // Note: In a full implementation, you'd have a UserAnswer repository to check existing answers

            // Evaluate the answer
            var isCorrect = EvaluateAnswer(exercise, answer);
            var pointsEarned = isCorrect ? exercise.Points : 0;

            // Create user answer record
            var userAnswer = new UserAnswer
            {
                Id = Guid.NewGuid(),
                StudySessionId = studySessionId,
                ExerciseId = exerciseId,
                Answer = answer.Trim(),
                IsCorrect = isCorrect,
                PointsEarned = pointsEarned,
                AnsweredAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            // Update study session score
            studySession.Score += pointsEarned;
            studySession.UpdatedAt = DateTime.UtcNow;

            // Save changes
            await _unitOfWork.UserAnswers.AddAsync(userAnswer);
            _unitOfWork.StudySessions.Update(studySession);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Completes a study session and updates user progress
    /// </summary>
    /// <param name="studySessionId">Study session identifier</param>
    /// <returns>True if completion successful, false otherwise</returns>
    public async Task<bool> CompleteStudySessionAsync(Guid studySessionId)
    {
        try
        {
            if (studySessionId == Guid.Empty)
            {
                return false;
            }

            var studySession = await _unitOfWork.StudySessions.GetByIdAsync(studySessionId);
            if (studySession == null || studySession.IsCompleted)
            {
                return false;
            }

            // Complete the session
            studySession.EndTime = DateTime.UtcNow;
            studySession.IsCompleted = true;
            studySession.Duration = studySession.EndTime.Value - studySession.StartTime;
            studySession.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.StudySessions.Update(studySession);

            // Get lesson to find course ID for progress update
            var lesson = await _unitOfWork.Lessons.GetByIdAsync(studySession.LessonId);
            if (lesson != null)
            {
                // Update user progress
                await UpdateUserProgressAsync(studySession.UserId, lesson.CourseId);
            }

            await _unitOfWork.SaveChangesAsync();

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Gets the active study session for a user and lesson
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="lessonId">Lesson identifier</param>
    /// <returns>Active study session or null if none exists</returns>
    public async Task<StudySession?> GetActiveSessionAsync(Guid userId, Guid lessonId)
    {
        try
        {
            if (userId == Guid.Empty || lessonId == Guid.Empty)
            {
                return null;
            }

            return await _unitOfWork.StudySessions.GetActiveSessionAsync(userId, lessonId);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Gets the study history for a user
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <returns>Collection of study sessions</returns>
    public async Task<IEnumerable<StudySession>> GetUserStudyHistoryAsync(Guid userId)
    {
        try
        {
            if (userId == Guid.Empty)
            {
                return Enumerable.Empty<StudySession>();
            }

            return await _unitOfWork.StudySessions.GetSessionsByUserIdAsync(userId);
        }
        catch
        {
            return Enumerable.Empty<StudySession>();
        }
    }

    /// <summary>
    /// Updates user progress for a specific course
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="courseId">Course identifier</param>
    public async Task UpdateUserProgressAsync(Guid userId, Guid courseId)
    {
        try
        {
            if (userId == Guid.Empty || courseId == Guid.Empty)
            {
                return;
            }

            // Get or create user progress record
            var userProgress = await _unitOfWork.UserProgresses.GetUserProgressAsync(userId, courseId);
            var isNewProgress = userProgress == null;

            if (isNewProgress)
            {
                userProgress = new UserProgress
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    CourseId = courseId,
                    CreatedAt = DateTime.UtcNow
                };
            }

            // Get all lessons for the course
            var courseLessons = await _unitOfWork.Lessons.GetLessonsByCourseIdAsync(courseId);
            var totalLessons = courseLessons.Count();

            // Get completed sessions for this user and course
            var completedSessions = await _unitOfWork.StudySessions.GetCompletedSessionsAsync(userId);
            var courseCompletedSessions = completedSessions.Where(s => 
                courseLessons.Any(l => l.Id == s.LessonId)).ToList();

            // Calculate progress metrics
            var completedLessonIds = courseCompletedSessions.Select(s => s.LessonId).Distinct();
            var completedLessonsCount = completedLessonIds.Count();

            var totalScore = courseCompletedSessions.Sum(s => s.Score);
            var totalPossibleScore = courseCompletedSessions.Sum(s => s.MaxScore);

            var completionPercentage = totalLessons > 0 ? 
                (double)completedLessonsCount / totalLessons * 100 : 0;

            // Update progress record
            userProgress.CompletedLessons = completedLessonsCount;
            userProgress.TotalLessons = totalLessons;
            userProgress.TotalScore = totalScore;
            userProgress.TotalPossibleScore = totalPossibleScore;
            userProgress.CompletionPercentage = Math.Round(completionPercentage, 2);
            userProgress.LastAccessedAt = DateTime.UtcNow;
            userProgress.UpdatedAt = DateTime.UtcNow;

            // Save progress
            if (isNewProgress)
            {
                await _unitOfWork.UserProgresses.AddAsync(userProgress);
            }
            else
            {
                _unitOfWork.UserProgresses.Update(userProgress);
            }

            await _unitOfWork.SaveChangesAsync();
        }
        catch
        {
            // Log error but don't throw to avoid disrupting the main flow
        }
    }

    /// <summary>
    /// Evaluates a user's answer against the correct answer for an exercise
    /// </summary>
    /// <param name="exercise">Exercise entity</param>
    /// <param name="userAnswer">User's answer</param>
    /// <returns>True if answer is correct, false otherwise</returns>
    private static bool EvaluateAnswer(Exercise exercise, string userAnswer)
    {
        try
        {
            var correctAnswer = exercise.CorrectAnswer.Trim();
            var answer = userAnswer.Trim();

            return exercise.Type switch
            {
                ExerciseType.MultipleChoice => string.Equals(answer, correctAnswer, StringComparison.OrdinalIgnoreCase),
                ExerciseType.TrueFalse => string.Equals(answer, correctAnswer, StringComparison.OrdinalIgnoreCase),
                ExerciseType.FillInTheBlank => string.Equals(answer, correctAnswer, StringComparison.OrdinalIgnoreCase),
                ExerciseType.Essay => false, // Essays require manual grading
                ExerciseType.Listening => string.Equals(answer, correctAnswer, StringComparison.OrdinalIgnoreCase),
                ExerciseType.Speaking => false, // Speaking requires manual grading or speech recognition
                _ => false
            };
        }
        catch
        {
            return false;
        }
    }
}