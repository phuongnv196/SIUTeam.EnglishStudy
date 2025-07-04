using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using SIUTeam.EnglishStudy.Core.DTOs;
using SIUTeam.EnglishStudy.Core.Interfaces.Services;
using SIUTeam.EnglishStudy.Core.Authorization;
using SIUTeam.EnglishStudy.Core.Enums;
using SIUTeam.EnglishStudy.Core.Entities;
using SIUTeam.EnglishStudy.API.Models.Users;
using MapsterMapper;
using System.Security.Claims;

namespace SIUTeam.EnglishStudy.API.Controllers;

/// <summary>
/// Controller for user management
/// </summary>
[ApiController]
[Route("api/[controller]")]
[SwaggerTag("User management")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public UsersController(IUserService userService, IMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;
    }

    /// <summary>
    /// Get user profile
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>User profile information</returns>
    /// <response code="200">Profile retrieved successfully</response>
    /// <response code="404">User not found</response>
    /// <response code="403">Access denied</response>
    [HttpGet("{id}")]
    [Authorize]
    [RequirePermission(Permission.ReadUser)]
    [SwaggerOperation(Summary = "Get user profile", Description = "Retrieve user profile information")]
    [SwaggerResponse(200, "Profile retrieved successfully", typeof(UserDto))]
    [SwaggerResponse(404, "User not found")]
    [SwaggerResponse(403, "Access denied")]
    public async Task<ActionResult<UserDto>> GetUserProfile(Guid id)
    {
        try
        {
            // Get current user from token
            var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var currentUserRoleClaim = User.FindFirst(ClaimTypes.Role);

            if (currentUserIdClaim == null || !Guid.TryParse(currentUserIdClaim.Value, out var currentUserId))
            {
                return Unauthorized("Invalid token.");
            }

            // Check if user is accessing their own profile or has permission
            if (currentUserId != id && 
                currentUserRoleClaim?.Value != UserRole.Admin.ToString() && 
                currentUserRoleClaim?.Value != UserRole.Teacher.ToString())
            {
                return Forbid("You can only access your own profile.");
            }

            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Map user to DTO using Mapster
            var userDto = _mapper.Map<UserDto>(user);

            return Ok(userDto);
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occurred while retrieving the user profile.");
        }
    }

    /// <summary>
    /// Get current user profile
    /// </summary>
    /// <returns>Current user profile information</returns>
    /// <response code="200">Profile retrieved successfully</response>
    /// <response code="401">User not authenticated</response>
    [HttpGet("me")]
    [Authorize]
    [SwaggerOperation(Summary = "Get current user profile", Description = "Retrieve current authenticated user profile")]
    [SwaggerResponse(200, "Profile retrieved successfully", typeof(UserDto))]
    [SwaggerResponse(401, "User not authenticated")]
    public async Task<ActionResult<UserDto>> GetCurrentUserProfile()
    {
        try
        {
            var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (currentUserIdClaim == null || !Guid.TryParse(currentUserIdClaim.Value, out var currentUserId))
            {
                return Unauthorized("Invalid token.");
            }

            var user = await _userService.GetUserByIdAsync(currentUserId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Map user to DTO using Mapster
            var userDto = _mapper.Map<UserDto>(user);

            return Ok(userDto);
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occurred while retrieving the user profile.");
        }
    }

    /// <summary>
    /// Update user profile
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="request">Updated user information</param>
    /// <returns>Updated user profile</returns>
    /// <response code="200">Profile updated successfully</response>
    /// <response code="404">User not found</response>
    /// <response code="403">Access denied</response>
    /// <response code="400">Invalid request data</response>
    [HttpPut("{id}")]
    [Authorize]
    [RequirePermission(Permission.UpdateUser)]
    [SwaggerOperation(Summary = "Update user profile", Description = "Update user profile information")]
    [SwaggerResponse(200, "Profile updated successfully", typeof(UserDto))]
    [SwaggerResponse(404, "User not found")]
    [SwaggerResponse(403, "Access denied")]
    [SwaggerResponse(400, "Invalid request data")]
    public async Task<ActionResult<UserDto>> UpdateUserProfile(Guid id, [FromBody] UpdateUserRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            // Get current user from token
            var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var currentUserRoleClaim = User.FindFirst(ClaimTypes.Role);

            if (currentUserIdClaim == null || !Guid.TryParse(currentUserIdClaim.Value, out var currentUserId))
            {
                return Unauthorized("Invalid token.");
            }

            // Check if user is updating their own profile or has permission
            if (currentUserId != id && 
                currentUserRoleClaim?.Value != UserRole.Admin.ToString())
            {
                return Forbid("You can only update your own profile.");
            }

            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Update user
            var success = await _userService.UpdateUserAsync(id, request.FirstName, request.LastName);
            if (!success)
            {
                return StatusCode(500, "Failed to update user profile.");
            }

            // Get updated user
            var updatedUser = await _userService.GetUserByIdAsync(id);
            if (updatedUser == null)
            {
                return StatusCode(500, "User was updated but could not be retrieved.");
            }

            // Map updated user to DTO using Mapster
            var userDto = _mapper.Map<UserDto>(updatedUser);

            return Ok(userDto);
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occurred while updating the user profile.");
        }
    }

    /// <summary>
    /// Get all users (Admin only)
    /// </summary>
    /// <returns>List of all users</returns>
    /// <response code="200">Users retrieved successfully</response>
    /// <response code="403">Access denied</response>
    [HttpGet]
    [Authorize]
    [RequirePermission(Permission.ReadUser)]
    [SwaggerOperation(Summary = "Get all users", Description = "Retrieve list of all users (Admin/Teacher only)")]
    [SwaggerResponse(200, "Users retrieved successfully", typeof(IEnumerable<UserDto>))]
    [SwaggerResponse(403, "Access denied")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
    {
        try
        {
            var currentUserRoleClaim = User.FindFirst(ClaimTypes.Role);
            
            // Only Admin and Teacher can view all users
            if (currentUserRoleClaim?.Value != UserRole.Admin.ToString() && 
                currentUserRoleClaim?.Value != UserRole.Teacher.ToString())
            {
                return Forbid("Access denied. Only administrators and teachers can view all users.");
            }

            var users = await _userService.GetAllUsersAsync();
            
            // Map users to DTOs using Mapster
            var userDtos = _mapper.Map<IEnumerable<UserDto>>(users);

            return Ok(userDtos);
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occurred while retrieving users.");
        }
    }

    /// <summary>
    /// Delete user (Admin only)
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>Success message</returns>
    /// <response code="200">User deleted successfully</response>
    /// <response code="404">User not found</response>
    /// <response code="403">Access denied</response>
    [HttpDelete("{id}")]
    [Authorize]
    [RequirePermission(Permission.DeleteUser)]
    [SwaggerOperation(Summary = "Delete user", Description = "Delete a user account (Admin only)")]
    [SwaggerResponse(200, "User deleted successfully")]
    [SwaggerResponse(404, "User not found")]
    [SwaggerResponse(403, "Access denied")]
    public async Task<ActionResult> DeleteUser(Guid id)
    {
        try
        {
            var currentUserRoleClaim = User.FindFirst(ClaimTypes.Role);
            var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            // Only Admin can delete users
            if (currentUserRoleClaim?.Value != UserRole.Admin.ToString())
            {
                return Forbid("Access denied. Only administrators can delete users.");
            }

            // Prevent admin from deleting themselves
            if (currentUserIdClaim != null && Guid.TryParse(currentUserIdClaim.Value, out var currentUserId) && currentUserId == id)
            {
                return BadRequest("You cannot delete your own account.");
            }

            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var success = await _userService.DeleteUserAsync(id);
            if (!success)
            {
                return StatusCode(500, "Failed to delete user.");
            }

            return Ok(new { message = "User deleted successfully" });
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occurred while deleting the user.");
        }
    }

    /// <summary>
    /// Activate or deactivate user (Admin only)
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="request">User status update request</param>
    /// <returns>Updated user profile</returns>
    /// <response code="200">User status updated successfully</response>
    /// <response code="404">User not found</response>
    /// <response code="403">Access denied</response>
    [HttpPatch("{id}/status")]
    [Authorize]
    [RequirePermission(Permission.UpdateUser)]
    [SwaggerOperation(Summary = "Update user status", Description = "Activate or deactivate a user account (Admin only)")]
    [SwaggerResponse(200, "User status updated successfully", typeof(UserDto))]
    [SwaggerResponse(404, "User not found")]
    [SwaggerResponse(403, "Access denied")]
    public async Task<ActionResult<UserDto>> UpdateUserStatus(Guid id, [FromBody] UpdateUserStatusRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var currentUserRoleClaim = User.FindFirst(ClaimTypes.Role);
            var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            // Only Admin can update user status
            if (currentUserRoleClaim?.Value != UserRole.Admin.ToString())
            {
                return Forbid("Access denied. Only administrators can update user status.");
            }

            // Prevent admin from deactivating themselves
            if (currentUserIdClaim != null && Guid.TryParse(currentUserIdClaim.Value, out var currentUserId) && currentUserId == id && !request.IsActive)
            {
                return BadRequest("You cannot deactivate your own account.");
            }

            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var success = await _userService.UpdateUserStatusAsync(id, request.IsActive);
            if (!success)
            {
                return StatusCode(500, "Failed to update user status.");
            }

            // Get updated user
            var updatedUser = await _userService.GetUserByIdAsync(id);
            if (updatedUser == null)
            {
                return StatusCode(500, "User status was updated but user could not be retrieved.");
            }

            // Map updated user to DTO using Mapster
            var userDto = _mapper.Map<UserDto>(updatedUser);

            return Ok(userDto);
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occurred while updating user status.");
        }
    }
}
