namespace SIUTeam.EnglishStudy.Core.Enums;

/// <summary>
/// Defines the available permissions in the system
/// </summary>
public enum Permission
{
    // User Management
    CreateUser,
    ReadUser,
    UpdateUser,
    DeleteUser,
    ManageUserRoles,
    
    // Course Management
    CreateCourse,
    ReadCourse,
    UpdateCourse,
    DeleteCourse,
    PublishCourse,
    ManageCourseContent,
    
    // Lesson Management
    CreateLesson,
    ReadLesson,
    UpdateLesson,
    DeleteLesson,
    PublishLesson,
    ManageLessonContent,
    
    // Exercise Management
    CreateExercise,
    ReadExercise,
    UpdateExercise,
    DeleteExercise,
    ManageExercises,
    
    // Study & Progress
    StartStudySession,
    SubmitAnswers,
    ViewProgress,
    ViewAllProgress,
    ManageStudySessions,
    
    // System Administration
    ManageSystem,
    ViewAuditLogs,
    ManageSettings,
    ViewReports,
    
    // Content Access
    AccessBeginnerlevel,
    AccessIntermediateLevel,
    AccessAdvancedLevel,
    AccessAllLevels
}

/// <summary>
/// Defines resource types for permission checking
/// </summary>
public enum ResourceType
{
    User,
    Course,
    Lesson,
    Exercise,
    StudySession,
    UserProgress,
    System
}