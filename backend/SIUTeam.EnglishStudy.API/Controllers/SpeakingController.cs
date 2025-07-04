using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIUTeam.EnglishStudy.Core.DTOs;
using SIUTeam.EnglishStudy.Core.Interfaces;
using SIUTeam.EnglishStudy.API.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace SIUTeam.EnglishStudy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous] // Temporarily allow anonymous access for testing
public class SpeakingController : ControllerBase
{
    private readonly ISpeakingService _speakingService;
    private readonly ILogger<SpeakingController> _logger;

    public SpeakingController(ISpeakingService speakingService, ILogger<SpeakingController> logger)
    {
        _speakingService = speakingService;
        _logger = logger;
    }

    /// <summary>
    /// Transcribe uploaded audio file to text with IPA notation
    /// </summary>
    /// <param name="file">Audio file to transcribe</param>
    /// <returns>Transcription result with IPA</returns>
    [HttpPost("transcribe")]
    [SwaggerOperation(Summary = "Transcribe audio file", Description = "Upload an audio file and get transcription with IPA notation")]
    [SwaggerResponse(200, "Transcription successful", typeof(TranscriptionResponseDto))]
    [SwaggerResponse(400, "Invalid request")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<ActionResult<TranscriptionResponseDto>> TranscribeAudio(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { Error = "No file provided or file is empty" });
            }

            var fileUpload = new FormFileUpload(file);
            var result = await _speakingService.TranscribeAudioAsync(fileUpload);
            
            if (result.Success)
            {
                return Ok(result);
            }
            
            return StatusCode(500, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in TranscribeAudio endpoint");
            return StatusCode(500, new { Error = "Internal server error" });
        }
    }

    /// <summary>
    /// Transcribe audio chunk for real-time processing
    /// </summary>
    /// <param name="chunk">Audio chunk to transcribe</param>
    /// <returns>Chunk transcription result</returns>
    [HttpPost("transcribe-chunk")]
    [SwaggerOperation(Summary = "Transcribe audio chunk", Description = "Process audio chunk for real-time transcription")]
    [SwaggerResponse(200, "Transcription successful", typeof(ChunkTranscriptionResponseDto))]
    [SwaggerResponse(400, "Invalid request")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<ActionResult<ChunkTranscriptionResponseDto>> TranscribeChunk(IFormFile chunk)
    {
        try
        {
            if (chunk == null || chunk.Length == 0)
            {
                return BadRequest(new { Error = "No chunk provided or chunk is empty" });
            }

            var chunkUpload = new FormFileUpload(chunk);
            var result = await _speakingService.TranscribeChunkAsync(chunkUpload);
            
            if (result.Success)
            {
                return Ok(result);
            }
            
            return StatusCode(500, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in TranscribeChunk endpoint");
            return StatusCode(500, new { Error = "Internal server error" });
        }
    }

    /// <summary>
    /// Convert text to IPA notation
    /// </summary>
    /// <param name="request">Text to convert</param>
    /// <returns>IPA conversion result</returns>
    [HttpPost("text-to-ipa")]
    [SwaggerOperation(Summary = "Convert text to IPA", Description = "Convert input text to International Phonetic Alphabet notation")]
    [SwaggerResponse(200, "Conversion successful", typeof(TextToIpaResponseDto))]
    [SwaggerResponse(400, "Invalid request")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<ActionResult<TextToIpaResponseDto>> ConvertTextToIpa([FromBody] TextToIpaRequestDto request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Text))
            {
                return BadRequest(new { Error = "Text is required" });
            }

            var result = await _speakingService.ConvertTextToIpaAsync(request.Text);
            
            if (result.Success)
            {
                return Ok(result);
            }
            
            return StatusCode(500, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ConvertTextToIpa endpoint");
            return StatusCode(500, new { Error = "Internal server error" });
        }
    }

    /// <summary>
    /// Create talking avatar animation data from text
    /// </summary>
    /// <param name="request">Talking avatar creation request</param>
    /// <returns>Animation data for talking avatar</returns>
    [HttpPost("talking-avatar")]
    [SwaggerOperation(Summary = "Create talking avatar", Description = "Generate animation data for talking avatar from text")]
    [SwaggerResponse(200, "Avatar creation successful", typeof(TalkingAvatarResponseDto))]
    [SwaggerResponse(400, "Invalid request")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<ActionResult<TalkingAvatarResponseDto>> CreateTalkingAvatar([FromBody] TalkingAvatarRequestDto request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Text))
            {
                return BadRequest(new { Error = "Text is required" });
            }

            var result = await _speakingService.CreateTalkingAvatarAsync(
                request.Text, 
                request.Duration, 
                request.Fps);
            
            if (result.Success)
            {
                return Ok(result);
            }
            
            return StatusCode(500, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in CreateTalkingAvatar endpoint");
            return StatusCode(500, new { Error = "Internal server error" });
        }
    }

    /// <summary>
    /// Check health status of the speech service
    /// </summary>
    /// <returns>Health status</returns>
    [HttpGet("health")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Health check", Description = "Check if the speech service is healthy")]
    [SwaggerResponse(200, "Service is healthy")]
    [SwaggerResponse(503, "Service is unhealthy")]
    public async Task<ActionResult> CheckHealth()
    {
        try
        {
            var isHealthy = await _speakingService.CheckHealthAsync();
            
            if (isHealthy)
            {
                return Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow });
            }
            
            return StatusCode(503, new { Status = "Unhealthy", Timestamp = DateTime.UtcNow });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in health check");
            return StatusCode(503, new { Status = "Unhealthy", Error = "Health check failed", Timestamp = DateTime.UtcNow });
        }
    }

    /// <summary>
    /// Get supported audio formats
    /// </summary>
    /// <returns>List of supported audio formats</returns>
    [HttpGet("supported-formats")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Get supported formats", Description = "Get list of supported audio formats")]
    [SwaggerResponse(200, "Supported formats retrieved")]
    public ActionResult<object> GetSupportedFormats()
    {
        return Ok(new
        {
            SupportedFormats = new[] { "wav", "mp3", "m4a", "ogg", "flac", "aac" },
            MaxFileSize = "16MB",
            RecommendedFormat = "wav",
            SampleRate = "16kHz or higher",
            Channels = "Mono or Stereo"
        });
    }
}
