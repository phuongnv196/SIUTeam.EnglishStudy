using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SIUTeam.EnglishStudy.Core.DTOs;
using SIUTeam.EnglishStudy.Core.Interfaces;
using System.Text;
using System.Text.Json;

namespace SIUTeam.EnglishStudy.Infrastructure.Services;

public class SpeakingService : ISpeakingService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<SpeakingService> _logger;
    private readonly string _pythonApiBaseUrl;

    public SpeakingService(
        HttpClient httpClient, 
        IConfiguration configuration, 
        ILogger<SpeakingService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _pythonApiBaseUrl = configuration["PythonSpeechService:BaseUrl"] ?? "http://localhost:5000";
        
        // Configure HttpClient
        _httpClient.Timeout = TimeSpan.FromMinutes(5); // Long timeout for audio processing
    }

    public async Task<TranscriptionResponseDto> TranscribeAudioAsync(IFileUpload file)
    {
        try
        {
            _logger.LogInformation("Starting audio transcription for file: {FileName}", file.FileName);
            
            using var content = new MultipartFormDataContent();
            using var fileStream = file.GetStream();
            using var streamContent = new StreamContent(fileStream);
            
            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
            content.Add(streamContent, "file", file.FileName);

            var response = await _httpClient.PostAsync($"{_pythonApiBaseUrl}/transcribe", content);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Python API error: {StatusCode} - {Content}", response.StatusCode, errorContent);
                
                return new TranscriptionResponseDto
                {
                    Success = false,
                    Error = $"Python API error: {response.StatusCode}"
                };
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            
            var result = JsonSerializer.Deserialize<TranscriptionResponseDto>(jsonResponse, options);
            
            _logger.LogInformation("Audio transcription completed successfully");
            return result ?? new TranscriptionResponseDto { Success = false, Error = "Failed to deserialize response" };
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request failed during audio transcription");
            return new TranscriptionResponseDto
            {
                Success = false,
                Error = "Failed to connect to speech service"
            };
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Request timeout during audio transcription");
            return new TranscriptionResponseDto
            {
                Success = false,
                Error = "Request timeout"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during audio transcription");
            return new TranscriptionResponseDto
            {
                Success = false,
                Error = "Internal server error"
            };
        }
    }

    public async Task<ChunkTranscriptionResponseDto> TranscribeChunkAsync(IFileUpload chunk)
    {
        try
        {
            _logger.LogInformation("Starting chunk transcription");
            
            using var content = new MultipartFormDataContent();
            using var chunkStream = chunk.GetStream();
            using var streamContent = new StreamContent(chunkStream);
            
            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(chunk.ContentType);
            content.Add(streamContent, "chunk", chunk.FileName);

            var response = await _httpClient.PostAsync($"{_pythonApiBaseUrl}/transcribe_chunk", content);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Python API error for chunk: {StatusCode} - {Content}", response.StatusCode, errorContent);
                
                return new ChunkTranscriptionResponseDto
                {
                    Success = false,
                    Error = $"Python API error: {response.StatusCode}"
                };
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            
            var result = JsonSerializer.Deserialize<ChunkTranscriptionResponseDto>(jsonResponse, options);
            
            _logger.LogInformation("Chunk transcription completed successfully");
            return result ?? new ChunkTranscriptionResponseDto { Success = false, Error = "Failed to deserialize response" };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during chunk transcription");
            return new ChunkTranscriptionResponseDto
            {
                Success = false,
                Error = "Internal server error"
            };
        }
    }

    public async Task<TextToIpaResponseDto> ConvertTextToIpaAsync(string text)
    {
        try
        {
            _logger.LogInformation("Starting text to IPA conversion");
            
            var requestData = new { text };
            var jsonContent = JsonSerializer.Serialize(requestData);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_pythonApiBaseUrl}/text_to_ipa", content);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Python API error for IPA conversion: {StatusCode} - {Content}", response.StatusCode, errorContent);
                
                return new TextToIpaResponseDto
                {
                    Success = false,
                    Error = $"Python API error: {response.StatusCode}"
                };
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            
            var result = JsonSerializer.Deserialize<TextToIpaResponseDto>(jsonResponse, options);
            
            _logger.LogInformation("Text to IPA conversion completed successfully");
            return result ?? new TextToIpaResponseDto { Success = false, Error = "Failed to deserialize response" };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during text to IPA conversion");
            return new TextToIpaResponseDto
            {
                Success = false,
                Error = "Internal server error"
            };
        }
    }

    public async Task<TalkingAvatarResponseDto> CreateTalkingAvatarAsync(string text, double duration = 3.0, int fps = 30)
    {
        try
        {
            _logger.LogInformation("Starting talking avatar creation");
            
            var requestData = new { text, duration, fps };
            var jsonContent = JsonSerializer.Serialize(requestData);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_pythonApiBaseUrl}/create_talking_avatar", content);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Python API error for talking avatar: {StatusCode} - {Content}", response.StatusCode, errorContent);
                
                return new TalkingAvatarResponseDto
                {
                    Success = false,
                    Error = $"Python API error: {response.StatusCode}"
                };
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            
            var result = JsonSerializer.Deserialize<TalkingAvatarResponseDto>(jsonResponse, options);
            
            _logger.LogInformation("Talking avatar creation completed successfully");
            return result ?? new TalkingAvatarResponseDto { Success = false, Error = "Failed to deserialize response" };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during talking avatar creation");
            return new TalkingAvatarResponseDto
            {
                Success = false,
                Error = "Internal server error"
            };
        }
    }

    public async Task<bool> CheckHealthAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_pythonApiBaseUrl}/health");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed");
            return false;
        }
    }
}
