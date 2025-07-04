using SIUTeam.EnglishStudy.Core.Enums;
using SIUTeam.EnglishStudy.Core.Entities;

namespace SIUTeam.EnglishStudy.Core.Authorization;

/// <summary>
/// Static class that defines role-based permissions
/// </summary>
public static class RolePermissions
{
    /// <summary>
    /// Gets all permissions for a specific role
    /// </summary>
    /// <param name="role">User role</param>
    /// <returns>Set of permissions for the role</returns>
    public static HashSet<Permission> GetPermissions(UserRole role)
    {
        return role switch
        {
            UserRole.Student => GetStudentPermissions(),
            UserRole.Teacher => GetTeacherPermissions(),
            UserRole.Admin => GetAdminPermissions(),
            _ => new HashSet<Permission>()
        };
    }

    /// <summary>
    /// Gets permissions for Student role
    /// </summary>
    private static HashSet<Permission> GetStudentPermissions()
    {
        return new HashSet<Permission>
        {
            // User Management (own profile only)
            Permission.ReadUser,
            Permission.UpdateUser,
            
            // Course Access
            Permission.ReadCourse,
            
            // Lesson Access
            Permission.ReadLesson,
            
            // Exercise Access
            Permission.ReadExercise,
            
            // Study & Progress
            Permission.StartStudySession,
            Permission.SubmitAnswers,
            Permission.ViewProgress,
            
            // Content Access based on level
            Permission.AccessBeginnerlevel,
            // Intermediate and Advanced access can be granted based on progress
        };
    }

    /// <summary>
    /// Gets permissions for Teacher role
    /// </summary>
    private static HashSet<Permission> GetTeacherPermissions()
    {
        return new HashSet<Permission>
        {
            // User Management (limited)
            Permission.ReadUser,
            Permission.UpdateUser,
            
            // Course Management
            Permission.CreateCourse,
            Permission.ReadCourse,
            Permission.UpdateCourse,
            Permission.PublishCourse,
            Permission.ManageCourseContent,
            
            // Lesson Management
            Permission.CreateLesson,
            Permission.ReadLesson,
            Permission.UpdateLesson,
            Permission.PublishLesson,
            Permission.ManageLessonContent,
            
            // Exercise Management
            Permission.CreateExercise,
            Permission.ReadExercise,
            Permission.UpdateExercise,
            Permission.ManageExercises,
            
            // Study & Progress (view all students)
            Permission.StartStudySession,
            Permission.SubmitAnswers,
            Permission.ViewProgress,
            Permission.ViewAllProgress,
            Permission.ManageStudySessions,
            
            // Content Access
            Permission.AccessBeginnerlevel,
            Permission.AccessIntermediateLevel,
            Permission.AccessAdvancedLevel,
            Permission.AccessAllLevels,
            
            // Reports
            Permission.ViewReports
        };
    }

    /// <summary>
    /// Gets permissions for Admin role
    /// </summary>
    private static HashSet<Permission> GetAdminPermissions()
    {
        return new HashSet<Permission>
        {
            // User Management (full access)
            Permission.CreateUser,
            Permission.ReadUser,
            Permission.UpdateUser,
            Permission.DeleteUser,
            Permission.ManageUserRoles,
            
            // Course Management (full access)
            Permission.CreateCourse,
            Permission.ReadCourse,
            Permission.UpdateCourse,
            Permission.DeleteCourse,
            Permission.PublishCourse,
            Permission.ManageCourseContent,
            
            // Lesson Management (full access)
            Permission.CreateLesson,
            Permission.ReadLesson,
            Permission.UpdateLesson,
            Permission.DeleteLesson,
            Permission.PublishLesson,
            Permission.ManageLessonContent,
            
            // Exercise Management (full access)
            Permission.CreateExercise,
            Permission.ReadExercise,
            Permission.UpdateExercise,
            Permission.DeleteExercise,
            Permission.ManageExercises,
            
            // Study & Progress (full access)
            Permission.StartStudySession,
            Permission.SubmitAnswers,
            Permission.ViewProgress,
            Permission.ViewAllProgress,
            Permission.ManageStudySessions,
            
            // System Administration
            Permission.ManageSystem,
            Permission.ViewAuditLogs,
            Permission.ManageSettings,
            Permission.ViewReports,
            
            // Content Access (all levels)
            Permission.AccessBeginnerlevel,
            Permission.AccessIntermediateLevel,
            Permission.AccessAdvancedLevel,
            Permission.AccessAllLevels
        };
    }

    /// <summary>
    /// Checks if a role has a specific permission
    /// </summary>
    /// <param name="role">User role</param>
    /// <param name="permission">Permission to check</param>
    /// <returns>True if role has permission, false otherwise</returns>
    public static bool HasPermission(UserRole role, Permission permission)
    {
        var permissions = GetPermissions(role);
        return permissions.Contains(permission);
    }

    /// <summary>
    /// Gets all permissions as a list for display purposes
    /// </summary>
    /// <returns>List of all available permissions</returns>
    public static List<Permission> GetAllPermissions()
    {
        return Enum.GetValues<Permission>().ToList();
    }
}