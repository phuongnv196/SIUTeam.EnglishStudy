using Microsoft.AspNetCore.Authorization;
using SIUTeam.EnglishStudy.Core.Enums;
using SIUTeam.EnglishStudy.Core.Entities;

namespace SIUTeam.EnglishStudy.Core.Authorization;

/// <summary>
/// Attribute to specify required permissions for controller actions
/// </summary>
public class RequirePermissionAttribute : AuthorizeAttribute
{
    public RequirePermissionAttribute(Permission permission) : base($"Permission.{permission}")
    {
        Permission = permission;
    }

    public Permission Permission { get; }
}

/// <summary>
/// Attribute to specify required role for controller actions
/// </summary>
public class RequireRoleAttribute : AuthorizeAttribute
{
    public RequireRoleAttribute(UserRole role) : base($"Role.{role}")
    {
        Role = role;
    }

    public UserRole Role { get; }
}

/// <summary>
/// Authorization requirement for permission-based access
/// </summary>
public class PermissionRequirement : IAuthorizationRequirement
{
    public PermissionRequirement(Permission permission)
    {
        Permission = permission;
    }

    public Permission Permission { get; }
}

/// <summary>
/// Authorization requirement for role-based access
/// </summary>
public class RoleRequirement : IAuthorizationRequirement
{
    public RoleRequirement(UserRole role)
    {
        Role = role;
    }

    public UserRole Role { get; }
}