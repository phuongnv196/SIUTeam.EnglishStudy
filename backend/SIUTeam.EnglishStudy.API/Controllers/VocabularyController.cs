using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIUTeam.EnglishStudy.Core.DTOs.Vocabulary;
using SIUTeam.EnglishStudy.Core.Interfaces.Services;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace SIUTeam.EnglishStudy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VocabularyController : ControllerBase
{
    private readonly IVocabularyService _vocabularyService;
    private readonly ILogger<VocabularyController> _logger;

    public VocabularyController(IVocabularyService vocabularyService, ILogger<VocabularyController> logger)
    {
        _vocabularyService = vocabularyService;
        _logger = logger;
    }

    /// <summary>
    /// Get all vocabulary items for the current user
    /// </summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Get all vocabulary items", Description = "Retrieve all vocabulary items for the current user")]
    [SwaggerResponse(200, "Vocabulary items retrieved successfully", typeof(IEnumerable<VocabularyItemDto>))]
    [SwaggerResponse(401, "Unauthorized")]
    public async Task<ActionResult<IEnumerable<VocabularyItemDto>>> GetVocabularyItems()
    {
        try
        {
            var userId = GetCurrentUserId();
            var vocabularyItems = await _vocabularyService.GetByUserIdAsync(userId);
            return Ok(vocabularyItems);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving vocabulary items");
            return StatusCode(500, new { Error = "Internal server error" });
        }
    }

    /// <summary>
    /// Get vocabulary items by topic
    /// </summary>
    [HttpGet("by-topic/{topic}")]
    [SwaggerOperation(Summary = "Get vocabulary items by topic", Description = "Retrieve vocabulary items filtered by topic")]
    [SwaggerResponse(200, "Vocabulary items retrieved successfully", typeof(IEnumerable<VocabularyItemDto>))]
    [SwaggerResponse(401, "Unauthorized")]
    public async Task<ActionResult<IEnumerable<VocabularyItemDto>>> GetVocabularyItemsByTopic(string topic)
    {
        try
        {
            var userId = GetCurrentUserId();
            var vocabularyItems = await _vocabularyService.GetByTopicAsync(topic, userId);
            return Ok(vocabularyItems);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving vocabulary items by topic: {Topic}", topic);
            return StatusCode(500, new { Error = "Internal server error" });
        }
    }

    /// <summary>
    /// Get vocabulary items by level
    /// </summary>
    [HttpGet("by-level/{level}")]
    [SwaggerOperation(Summary = "Get vocabulary items by level", Description = "Retrieve vocabulary items filtered by difficulty level")]
    [SwaggerResponse(200, "Vocabulary items retrieved successfully", typeof(IEnumerable<VocabularyItemDto>))]
    [SwaggerResponse(400, "Invalid level")]
    [SwaggerResponse(401, "Unauthorized")]
    public async Task<ActionResult<IEnumerable<VocabularyItemDto>>> GetVocabularyItemsByLevel(string level)
    {
        try
        {
            var userId = GetCurrentUserId();
            var vocabularyItems = await _vocabularyService.GetByLevelAsync(level, userId);
            return Ok(vocabularyItems);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving vocabulary items by level: {Level}", level);
            return StatusCode(500, new { Error = "Internal server error" });
        }
    }

    /// <summary>
    /// Get a specific vocabulary item by ID
    /// </summary>
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get vocabulary item by ID", Description = "Retrieve a specific vocabulary item")]
    [SwaggerResponse(200, "Vocabulary item retrieved successfully", typeof(VocabularyItemDto))]
    [SwaggerResponse(404, "Vocabulary item not found")]
    [SwaggerResponse(401, "Unauthorized")]
    public async Task<ActionResult<VocabularyItemDto>> GetVocabularyItem(string id)
    {
        try
        {
            var vocabularyItem = await _vocabularyService.GetByIdAsync(id);
            if (vocabularyItem == null)
            {
                return NotFound(new { Error = "Vocabulary item not found" });
            }

            return Ok(vocabularyItem);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving vocabulary item: {Id}", id);
            return StatusCode(500, new { Error = "Internal server error" });
        }
    }

    /// <summary>
    /// Search for a vocabulary item by word
    /// </summary>
    [HttpGet("search/{word}")]
    [SwaggerOperation(Summary = "Search vocabulary item by word", Description = "Find a vocabulary item by its word")]
    [SwaggerResponse(200, "Vocabulary item found", typeof(VocabularyItemDto))]
    [SwaggerResponse(404, "Vocabulary item not found")]
    [SwaggerResponse(401, "Unauthorized")]
    public async Task<ActionResult<VocabularyItemDto>> SearchVocabularyItem(string word)
    {
        try
        {
            var userId = GetCurrentUserId();
            var vocabularyItem = await _vocabularyService.GetByWordAsync(word, userId);
            if (vocabularyItem == null)
            {
                return NotFound(new { Error = "Vocabulary item not found" });
            }

            return Ok(vocabularyItem);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching vocabulary item: {Word}", word);
            return StatusCode(500, new { Error = "Internal server error" });
        }
    }

    /// <summary>
    /// Create a new vocabulary item
    /// </summary>
    [HttpPost]
    [SwaggerOperation(Summary = "Create vocabulary item", Description = "Add a new vocabulary item")]
    [SwaggerResponse(201, "Vocabulary item created successfully", typeof(VocabularyItemDto))]
    [SwaggerResponse(400, "Invalid request")]
    [SwaggerResponse(401, "Unauthorized")]
    public async Task<ActionResult<VocabularyItemDto>> CreateVocabularyItem([FromBody] CreateVocabularyItemDto createDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetCurrentUserId();
            var vocabularyItem = await _vocabularyService.CreateAsync(createDto, userId);
            return CreatedAtAction(nameof(GetVocabularyItem), new { id = vocabularyItem.Id }, vocabularyItem);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating vocabulary item");
            return StatusCode(500, new { Error = "Internal server error" });
        }
    }

    /// <summary>
    /// Update an existing vocabulary item
    /// </summary>
    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Update vocabulary item", Description = "Update an existing vocabulary item")]
    [SwaggerResponse(200, "Vocabulary item updated successfully", typeof(VocabularyItemDto))]
    [SwaggerResponse(404, "Vocabulary item not found")]
    [SwaggerResponse(400, "Invalid request")]
    [SwaggerResponse(401, "Unauthorized")]
    public async Task<ActionResult<VocabularyItemDto>> UpdateVocabularyItem(string id, [FromBody] UpdateVocabularyItemDto updateDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var vocabularyItem = await _vocabularyService.UpdateAsync(id, updateDto);
            if (vocabularyItem == null)
            {
                return NotFound(new { Error = "Vocabulary item not found" });
            }

            return Ok(vocabularyItem);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating vocabulary item: {Id}", id);
            return StatusCode(500, new { Error = "Internal server error" });
        }
    }

    /// <summary>
    /// Delete a vocabulary item
    /// </summary>
    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Delete vocabulary item", Description = "Remove a vocabulary item")]
    [SwaggerResponse(204, "Vocabulary item deleted successfully")]
    [SwaggerResponse(404, "Vocabulary item not found")]
    [SwaggerResponse(401, "Unauthorized")]
    public async Task<IActionResult> DeleteVocabularyItem(string id)
    {
        try
        {
            var success = await _vocabularyService.DeleteAsync(id);
            if (!success)
            {
                return NotFound(new { Error = "Vocabulary item not found" });
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting vocabulary item: {Id}", id);
            return StatusCode(500, new { Error = "Internal server error" });
        }
    }

    /// <summary>
    /// Mark a vocabulary item as learned or unlearned
    /// </summary>
    [HttpPatch("{id}/learned")]
    [SwaggerOperation(Summary = "Mark vocabulary item as learned", Description = "Update the learned status of a vocabulary item")]
    [SwaggerResponse(200, "Vocabulary item status updated successfully")]
    [SwaggerResponse(404, "Vocabulary item not found")]
    [SwaggerResponse(401, "Unauthorized")]
    public async Task<IActionResult> MarkAsLearned(string id, [FromBody] bool learned = true)
    {
        try
        {
            var success = await _vocabularyService.MarkAsLearnedAsync(id, learned);
            if (!success)
            {
                return NotFound(new { Error = "Vocabulary item not found" });
            }

            return Ok(new { Message = $"Vocabulary item marked as {(learned ? "learned" : "unlearned")}" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating vocabulary item status: {Id}", id);
            return StatusCode(500, new { Error = "Internal server error" });
        }
    }

    /// <summary>
    /// Get vocabulary topics with statistics
    /// </summary>
    [HttpGet("topics")]
    [SwaggerOperation(Summary = "Get vocabulary topics", Description = "Retrieve all vocabulary topics with learning statistics")]
    [SwaggerResponse(200, "Topics retrieved successfully", typeof(IEnumerable<VocabularyTopicDto>))]
    [SwaggerResponse(401, "Unauthorized")]
    public async Task<ActionResult<IEnumerable<VocabularyTopicDto>>> GetTopics()
    {
        try
        {
            var userId = GetCurrentUserId();
            var topics = await _vocabularyService.GetTopicsWithStatsAsync(userId);
            return Ok(topics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving vocabulary topics");
            return StatusCode(500, new { Error = "Internal server error" });
        }
    }

    /// <summary>
    /// Get vocabulary statistics
    /// </summary>
    [HttpGet("stats")]
    [SwaggerOperation(Summary = "Get vocabulary statistics", Description = "Retrieve vocabulary learning statistics")]
    [SwaggerResponse(200, "Statistics retrieved successfully", typeof(Dictionary<string, int>))]
    [SwaggerResponse(401, "Unauthorized")]
    public async Task<ActionResult<Dictionary<string, int>>> GetStats()
    {
        try
        {
            var userId = GetCurrentUserId();
            var stats = await _vocabularyService.GetTopicStatsAsync(userId);
            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving vocabulary statistics");
            return StatusCode(500, new { Error = "Internal server error" });
        }
    }

    private string GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            throw new UnauthorizedAccessException("User ID not found in token");
        }
        return userIdClaim;
    }
}
