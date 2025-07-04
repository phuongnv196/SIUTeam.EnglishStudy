using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using SIUTeam.EnglishStudy.Core.Authorization;
using SIUTeam.EnglishStudy.Core.Enums;
using SIUTeam.EnglishStudy.Core.Entities;

namespace SIUTeam.EnglishStudy.Infrastructure.Authorization;

/// <summary>
/// Authorization handler for permission-based access control
/// </summary>
public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        // Get user role from claims
        var roleClaim = context.User.FindFirst(ClaimTypes.Role);
        if (roleClaim == null || !Enum.TryParse<UserRole>(roleClaim.Value, out var userRole))
        {
            context.Fail();
            return Task.CompletedTask;
        }

        // Check if user role has the required permission
        if (RolePermissions.HasPermission(userRole, requirement.Permission))
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }

        return Task.CompletedTask;
    }
}

/// <summary>
/// Authorization handler for role-based access control
/// </summary>
public class RoleAuthorizationHandler : AuthorizationHandler<RoleRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        RoleRequirement requirement)
    {
        // Get user role from claims
        var roleClaim = context.User.FindFirst(ClaimTypes.Role);
        if (roleClaim == null || !Enum.TryParse<UserRole>(roleClaim.Value, out var userRole))
        {
            context.Fail();
            return Task.CompletedTask;
        }

        // Check if user has the required role or a higher role
        if (userRole == requirement.Role || IsHigherRole(userRole, requirement.Role))
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Determines if one role is higher than another in the hierarchy
    /// </summary>
    /// <param name="userRole">User's current role</param>
    /// <param name="requiredRole">Required role</param>
    /// <returns>True if user role is higher than required role</returns>
    private static bool IsHigherRole(UserRole userRole, UserRole requiredRole)
    {
        // Define role hierarchy: Admin > Teacher > Student
        var roleHierarchy = new Dictionary<UserRole, int>
        {
            { UserRole.Student, 1 },
            { UserRole.Teacher, 2 },
            { UserRole.Admin, 3 }
        };

        return roleHierarchy.GetValueOrDefault(userRole, 0) > 
               roleHierarchy.GetValueOrDefault(requiredRole, 0);
    }
}

/// <summary>
/// Authorization handler for resource-based access control
/// </summary>
public class ResourceAuthorizationHandler : AuthorizationHandler<PermissionRequirement, object>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement,
        object resource)
    {
        // Get user information from claims
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
        var roleClaim = context.User.FindFirst(ClaimTypes.Role);

        if (userIdClaim == null || roleClaim == null || 
            !Guid.TryParse(userIdClaim.Value, out var userId) ||
            !Enum.TryParse<UserRole>(roleClaim.Value, out var userRole))
        {
            context.Fail();
            return Task.CompletedTask;
        }

        // Check basic permission first
        if (!RolePermissions.HasPermission(userRole, requirement.Permission))
        {
            context.Fail();
            return Task.CompletedTask;
        }

        // For resource-specific permissions, check ownership or admin access
        if (resource != null && HasResourceAccess(userId, userRole, resource, requirement.Permission))
        {
            context.Succeed(requirement);
        }
        else if (resource == null) // No specific resource, check general permission
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Checks if user has access to a specific resource
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="userRole">User role</param>
    /// <param name="resource">Resource being accessed</param>
    /// <param name="permission">Required permission</param>
    /// <returns>True if user has access to the resource</returns>
    private static bool HasResourceAccess(Guid userId, UserRole userRole, object resource, Permission permission)
    {
        // Admins have access to everything
        if (userRole == UserRole.Admin)
        {
            return true;
        }

        // Check resource-specific access rules
        return resource switch
        {
            // For user resources, users can access their own data
            { } when resource.GetType().GetProperty("UserId")?.GetValue(resource) is Guid resourceUserId =>
                resourceUserId == userId || userRole == UserRole.Teacher,
            
            // For courses, teachers can access courses they created
            { } when resource.GetType().GetProperty("CreatedBy")?.GetValue(resource) is string createdBy =>
                createdBy == userId.ToString() || userRole == UserRole.Teacher,
            
            // Default: allow if user has the permission
            _ => true
        };
    }
}