using SIUTeam.EnglishStudy.Core.DTOs;

namespace SIUTeam.EnglishStudy.Core.Interfaces;

public interface ISpeakingService
{
    /// <summary>
    /// Transcribe audio file to text with IPA conversion
    /// </summary>
    /// <param name="file">Audio file to transcribe</param>
    /// <returns>Transcription result with IPA</returns>
    Task<TranscriptionResponseDto> TranscribeAudioAsync(IFileUpload file);
    
    /// <summary>
    /// Transcribe audio chunk for real-time processing
    /// </summary>
    /// <param name="chunk">Audio chunk to transcribe</param>
    /// <returns>Chunk transcription result</returns>
    Task<ChunkTranscriptionResponseDto> TranscribeChunkAsync(IFileUpload chunk);
    
    /// <summary>
    /// Convert text to IPA notation
    /// </summary>
    /// <param name="text">Text to convert</param>
    /// <returns>IPA conversion result</returns>
    Task<TextToIpaResponseDto> ConvertTextToIpaAsync(string text);
    
    /// <summary>
    /// Create talking avatar animation data from text
    /// </summary>
    /// <param name="text">Text for animation</param>
    /// <param name="duration">Animation duration in seconds</param>
    /// <param name="fps">Frames per second</param>
    /// <returns>Animation data for talking avatar</returns>
    Task<TalkingAvatarResponseDto> CreateTalkingAvatarAsync(string text, double duration = 3.0, int fps = 30);
    
    /// <summary>
    /// Check if Python speech service is healthy
    /// </summary>
    /// <returns>Health status</returns>
    Task<bool> CheckHealthAsync();
}
