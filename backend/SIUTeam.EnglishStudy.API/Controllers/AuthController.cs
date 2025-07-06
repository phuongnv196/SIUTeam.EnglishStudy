using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using SIUTeam.EnglishStudy.Core.DTOs;
using SIUTeam.EnglishStudy.Core.Interfaces.Services;
using SIUTeam.EnglishStudy.API.Models.Auth;
using MapsterMapper;
using SIUTeam.EnglishStudy.Core.DTOs;

namespace SIUTeam.EnglishStudy.API.Controllers;

/// <summary>
/// Controller for user authentication
/// </summary>
[ApiController]
[Route("api/[controller]")]
[SwaggerTag("User authentication")]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public AuthController(
        IAuthenticationService authenticationService,
        IUserService userService,
        IMapper mapper)
    {
        _authenticationService = authenticationService;
        _userService = userService;
        _mapper = mapper;
    }

    /// <summary>
    /// User login
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <returns>Authentication token</returns>
    /// <response code="200">Login successful</response>
    /// <response code="401">Invalid credentials</response>
    /// <response code="400">Invalid request data</response>
    [HttpPost("login")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "User login", Description = "Authenticate user and return JWT access token")]
    [SwaggerResponse(200, "Login successful", typeof(LoginResponse))]
    [SwaggerResponse(401, "Invalid credentials")]
    [SwaggerResponse(400, "Invalid request data")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            // Authenticate user
            var token = await _authenticationService.LoginAsync(request.Email, request.Password);
            
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized("Invalid email or password.");
            }

            // Get user details
            var user = await _userService.GetUserByEmailAsync(request.Email);
            if (user == null)
            {
                return Unauthorized("User not found.");
            }

            // Map user to DTO using Mapster
            var userDto = _mapper.Map<UserDto>(user);

            var response = new LoginResponse
            {
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddDays(7), // Should match JWT expiration
                User = userDto
            };

            return Ok(response);
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occurred during login.");
        }
    }

    /// <summary>
    /// User registration
    /// </summary>
    /// <param name="request">Registration details</param>
    /// <returns>Created user information</returns>
    /// <response code="201">User registered successfully</response>
    /// <response code="400">Invalid registration data</response>
    /// <response code="409">Email or username already exists</response>
    [HttpPost("register")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "User registration", Description = "Register a new user account")]
    [SwaggerResponse(201, "User registered successfully", typeof(UserDto))]
    [SwaggerResponse(400, "Invalid registration data")]
    [SwaggerResponse(409, "Email or username already exists")]
    public async Task<ActionResult<UserDto>> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var success = await _authenticationService.RegisterAsync(
                request.Email,
                request.Username,
                request.Password,
                request.FirstName,
                request.LastName
            );

            if (!success)
            {
                return Conflict("Email or username already exists.");
            }

            // Get the created user
            var user = await _userService.GetUserByEmailAsync(request.Email);
            if (user == null)
            {
                return StatusCode(500, "User was created but could not be retrieved.");
            }

            // Map user to DTO using Mapster
            var userDto = _mapper.Map<UserDto>(user);

            return CreatedAtAction("GetUserProfile", "Users", new { id = user.Id }, userDto);
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occurred during registration.");
        }
    }

    /// <summary>
    /// User logout
    /// </summary>
    /// <returns>Success message</returns>
    /// <response code="200">Logout successful</response>
    [HttpPost("logout")]
    [Authorize]
    [SwaggerOperation(Summary = "User logout", Description = "Logout user and invalidate token")]
    [SwaggerResponse(200, "Logout successful")]
    public async Task<ActionResult> Logout()
    {
        try
        {
            // Get token from Authorization header
            var token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
            
            if (!string.IsNullOrEmpty(token))
            {
                await _authenticationService.LogoutAsync(token);
            }

            return Ok(new { message = "Logout successful" });
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occurred during logout.");
        }
    }

    /// <summary>
    /// Refresh authentication token
    /// </summary>
    /// <param name="request">Refresh token request</param>
    /// <returns>New authentication token</returns>
    /// <response code="200">Token refreshed successfully</response>
    /// <response code="401">Invalid or expired refresh token</response>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Refresh token", Description = "Refresh JWT access token using refresh token")]
    [SwaggerResponse(200, "Token refreshed successfully", typeof(LoginResponse))]
    [SwaggerResponse(401, "Invalid or expired refresh token")]
    public async Task<ActionResult<LoginResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            // This would need to be implemented in IAuthenticationService
            // For now, return a not implemented response
            return StatusCode(501, "Refresh token functionality not yet implemented.");
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occurred during token refresh.");
        }
    }

    /// <summary>
    /// Verify authentication token
    /// </summary>
    /// <returns>Token verification result</returns>
    /// <response code="200">Token is valid</response>
    /// <response code="401">Token is invalid or expired</response>
    [HttpGet("verify")]
    [Authorize]
    [SwaggerOperation(Summary = "Verify token", Description = "Verify if the current JWT token is valid")]
    [SwaggerResponse(200, "Token is valid")]
    [SwaggerResponse(401, "Token is invalid or expired")]
    public ActionResult VerifyToken()
    {
        // If we reach here, it means the token is valid (due to [Authorize] attribute)
        return Ok(new { message = "Token is valid", isValid = true });
    }
}
