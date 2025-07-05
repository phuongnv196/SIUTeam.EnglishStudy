using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SIUTeam.EnglishStudy.API.Models;
using SIUTeam.EnglishStudy.Core.DTOs;
using SIUTeam.EnglishStudy.Core.Interfaces;
using System.Collections.Concurrent;
using System.Security.Claims;

namespace SIUTeam.EnglishStudy.API.Hubs;

[Authorize]
public class SpeakingHub : Hub
{
    private readonly ISpeakingService _speakingService;
    private readonly ILogger<SpeakingHub> _logger;
    
    // Store active speaking sessions
    private static readonly ConcurrentDictionary<string, SpeakingSessionDto> _activeSessions = new();
    
    public SpeakingHub(ISpeakingService speakingService, ILogger<SpeakingHub> logger)
    {
        _speakingService = speakingService;
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        var userName = Context.User?.Identity?.Name;
        var isAuthenticated = Context.User?.Identity?.IsAuthenticated ?? false;
        
        // Debug: Log all claims
        var claims = Context.User?.Claims?.Select(c => $"{c.Type}: {c.Value}").ToList() ?? new List<string>();
        
        _logger.LogInformation("User connected to SpeakingHub - UserIdentifier: {UserId}, UserName: {UserName}, IsAuthenticated: {IsAuthenticated}", 
            userId, userName, isAuthenticated);
        _logger.LogInformation("User claims: {Claims}", string.Join(", ", claims));
        
        // Try to get user ID from different claim types
        var nameIdentifierClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var nameClaim = Context.User?.FindFirst(ClaimTypes.Name)?.Value;
        var subClaim = Context.User?.FindFirst("sub")?.Value;
        var nameidClaim = Context.User?.FindFirst("nameid")?.Value;
        var userIdClaim = Context.User?.FindFirst("userId")?.Value;
        var idClaim = Context.User?.FindFirst("id")?.Value;
        
        _logger.LogInformation("Claim values - NameIdentifier: {NameIdentifier}, Name: {Name}, Sub: {Sub}, NameId: {NameId}, UserId: {UserId}, Id: {Id}", 
            nameIdentifierClaim, nameClaim, subClaim, nameidClaim, userIdClaim, idClaim);
            
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.UserIdentifier;
        _logger.LogInformation("User {UserId} disconnected from SpeakingHub", userId);
        
        // Clean up any active sessions for this user
        var sessionsToRemove = _activeSessions.Where(kvp => kvp.Value.UserId == userId).ToList();
        foreach (var session in sessionsToRemove)
        {
            _activeSessions.TryRemove(session.Key, out _);
        }
        
        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Start a new speaking practice session
    /// </summary>
    /// <param name="lessonId">ID of the lesson</param>
    /// <param name="expectedText">Expected text for pronunciation practice</param>
    public async Task StartSpeakingSession(string lessonId, string expectedText)
    {
        try
        {
            var userId = Context.UserIdentifier!;
            var sessionId = Guid.NewGuid().ToString();
            
            var session = new SpeakingSessionDto
            {
                SessionId = sessionId,
                UserId = userId,
                LessonId = lessonId,
                StartTime = DateTime.UtcNow,
                IsActive = true
            };
            
            _activeSessions[sessionId] = session;
            
            // Join the user to a group for this session
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Session_{sessionId}");
            
            _logger.LogInformation("Started speaking session {SessionId} for user {UserId}", sessionId, userId);
            
            await Clients.Caller.SendAsync("SpeakingSessionStarted", new
            {
                SessionId = sessionId,
                ExpectedText = expectedText,
                Message = "Speaking session started. You can now start speaking."
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting speaking session");
            await Clients.Caller.SendAsync("SpeakingError", new { Error = "Failed to start speaking session" });
        }
    }

    /// <summary>
    /// Process real-time audio chunk for transcription
    /// </summary>
    /// <param name="sessionId">Session ID</param>
    /// <param name="audioChunk">Base64 encoded audio chunk</param>
    public async Task ProcessAudioChunk(string sessionId, string audioChunk)
    {
        try
        {
            if (!_activeSessions.TryGetValue(sessionId, out var session) || !session.IsActive)
            {
                await Clients.Caller.SendAsync("SpeakingError", new { Error = "Invalid or inactive session" });
                return;
            }

            if (session.UserId != Context.UserIdentifier)
            {
                await Clients.Caller.SendAsync("SpeakingError", new { Error = "Unauthorized access to session" });
                return;
            }

            // Convert base64 to audio file
            var audioBytes = Convert.FromBase64String(audioChunk);
            var audioFile = CreateFormFileFromBytes(audioBytes, "chunk.wav", "audio/wav");
            
            // Transcribe the chunk
            var audioFileUpload = new FormFileUpload(audioFile);
            var result = await _speakingService.TranscribeChunkAsync(audioFileUpload);
            
            if (result.Success && !string.IsNullOrEmpty(result.Text))
            {
                _logger.LogInformation("Transcribed chunk for session {SessionId}: {Text}", sessionId, result.Text);
                
                // Send real-time transcription to the client
                await Clients.Group($"Session_{sessionId}").SendAsync("ChunkTranscribed", new
                {
                    SessionId = sessionId,
                    TranscribedText = result.Text,
                    ProcessingTime = result.ProcessingTime,
                    Language = result.Language,
                    Ipa = result.Ipa
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing audio chunk for session {SessionId}", sessionId);
            await Clients.Caller.SendAsync("SpeakingError", new { Error = "Failed to process audio chunk" });
        }
    }

    /// <summary>
    /// End speaking session and get final results
    /// </summary>
    /// <param name="sessionId">Session ID</param>
    /// <param name="finalAudio">Base64 encoded final audio</param>
    /// <param name="expectedText">Expected text for comparison</param>
    public async Task EndSpeakingSession(string sessionId, string finalAudio, string expectedText)
    {
        try
        {
            if (!_activeSessions.TryGetValue(sessionId, out var session) || !session.IsActive)
            {
                await Clients.Caller.SendAsync("SpeakingError", new { Error = "Invalid or inactive session" });
                return;
            }

            if (session.UserId != Context.UserIdentifier)
            {
                await Clients.Caller.SendAsync("SpeakingError", new { Error = "Unauthorized access to session" });
                return;
            }

            // Mark session as inactive
            session.IsActive = false;

            // Process final audio
            var audioBytes = Convert.FromBase64String(finalAudio);
            var audioFile = CreateFormFileFromBytes(audioBytes, "final.wav", "audio/wav");
            
            var audioFileUpload = new FormFileUpload(audioFile);
            var transcriptionResult = await _speakingService.TranscribeAudioAsync(audioFileUpload);
            
            if (transcriptionResult.Success)
            {
                // Calculate confidence score (simple implementation)
                var confidenceScore = CalculateConfidenceScore(transcriptionResult.Text, expectedText);
                
                // Generate suggestions
                var suggestions = GenerateSuggestions(transcriptionResult.Text, expectedText);
                
                var finalResult = new SpeakingResultDto
                {
                    SessionId = sessionId,
                    TranscribedText = transcriptionResult.Text,
                    ExpectedText = expectedText,
                    ConfidenceScore = confidenceScore,
                    ProcessingTime = transcriptionResult.ProcessingTime,
                    Ipa = transcriptionResult.Ipa,
                    Suggestions = suggestions
                };

                _logger.LogInformation("Ended speaking session {SessionId} with confidence score {Score}", 
                    sessionId, confidenceScore);

                await Clients.Group($"Session_{sessionId}").SendAsync("SpeakingSessionEnded", finalResult);
            }
            else
            {
                await Clients.Caller.SendAsync("SpeakingError", new 
                { 
                    Error = "Failed to process final audio",
                    Details = transcriptionResult.Error 
                });
            }

            // Clean up
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Session_{sessionId}");
            _activeSessions.TryRemove(sessionId, out _);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ending speaking session {SessionId}", sessionId);
            await Clients.Caller.SendAsync("SpeakingError", new { Error = "Failed to end speaking session" });
        }
    }

    /// <summary>
    /// Get pronunciation feedback for text
    /// </summary>
    /// <param name="text">Text to analyze</param>
    public async Task GetPronunciationFeedback(string text)
    {
        try
        {
            var ipaResult = await _speakingService.ConvertTextToIpaAsync(text);
            
            if (ipaResult.Success)
            {
                await Clients.Caller.SendAsync("PronunciationFeedback", new
                {
                    OriginalText = text,
                    IpaNotation = ipaResult.Ipa.G2pIpa,
                    Arpabet = ipaResult.Ipa.Arpabet,
                    Success = true
                });
            }
            else
            {
                await Clients.Caller.SendAsync("SpeakingError", new 
                { 
                    Error = "Failed to generate pronunciation feedback",
                    Details = ipaResult.Error 
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pronunciation feedback");
            await Clients.Caller.SendAsync("SpeakingError", new { Error = "Failed to get pronunciation feedback" });
        }
    }

    /// <summary>
    /// Create talking avatar animation
    /// </summary>
    /// <param name="text">Text for animation</param>
    /// <param name="duration">Animation duration</param>
    /// <param name="fps">Frames per second</param>
    public async Task CreateTalkingAvatar(string text, double duration = 3.0, int fps = 30)
    {
        try
        {
            var avatarResult = await _speakingService.CreateTalkingAvatarAsync(text, duration, fps);
            
            if (avatarResult.Success)
            {
                await Clients.Caller.SendAsync("TalkingAvatarCreated", avatarResult);
            }
            else
            {
                await Clients.Caller.SendAsync("SpeakingError", new 
                { 
                    Error = "Failed to create talking avatar",
                    Details = avatarResult.Error 
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating talking avatar");
            await Clients.Caller.SendAsync("SpeakingError", new { Error = "Failed to create talking avatar" });
        }
    }

    #region Private Helper Methods

    private static IFormFile CreateFormFileFromBytes(byte[] bytes, string fileName, string contentType)
    {
        var stream = new MemoryStream(bytes);
        return new FormFile(stream, 0, bytes.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType
        };
    }

    private static double CalculateConfidenceScore(string transcribed, string expected)
    {
        // Simple Levenshtein distance-based confidence score
        if (string.IsNullOrEmpty(transcribed) || string.IsNullOrEmpty(expected))
            return 0.0;

        var distance = LevenshteinDistance(transcribed.ToLower().Trim(), expected.ToLower().Trim());
        var maxLength = Math.Max(transcribed.Length, expected.Length);
        
        if (maxLength == 0) return 1.0;
        
        var similarity = 1.0 - (double)distance / maxLength;
        return Math.Max(0.0, Math.Min(1.0, similarity));
    }

    private static int LevenshteinDistance(string s1, string s2)
    {
        var matrix = new int[s1.Length + 1, s2.Length + 1];

        for (int i = 0; i <= s1.Length; i++)
            matrix[i, 0] = i;

        for (int j = 0; j <= s2.Length; j++)
            matrix[0, j] = j;

        for (int i = 1; i <= s1.Length; i++)
        {
            for (int j = 1; j <= s2.Length; j++)
            {
                var cost = s1[i - 1] == s2[j - 1] ? 0 : 1;
                matrix[i, j] = Math.Min(Math.Min(
                    matrix[i - 1, j] + 1,     // deletion
                    matrix[i, j - 1] + 1),    // insertion
                    matrix[i - 1, j - 1] + cost); // substitution
            }
        }

        return matrix[s1.Length, s2.Length];
    }

    private static List<string> GenerateSuggestions(string transcribed, string expected)
    {
        var suggestions = new List<string>();

        if (string.IsNullOrEmpty(transcribed))
        {
            suggestions.Add("No speech detected. Please speak louder and clearer.");
            return suggestions;
        }

        var score = CalculateConfidenceScore(transcribed, expected);

        if (score >= 0.9)
        {
            suggestions.Add("Excellent pronunciation! Well done!");
        }
        else if (score >= 0.7)
        {
            suggestions.Add("Good pronunciation! Minor improvements needed.");
            suggestions.Add("Try to enunciate each word clearly.");
        }
        else if (score >= 0.5)
        {
            suggestions.Add("Practice needed. Focus on pronunciation accuracy.");
            suggestions.Add("Listen to the expected pronunciation and try again.");
        }
        else
        {
            suggestions.Add("Significant improvement needed.");
            suggestions.Add("Please practice the pronunciation slowly.");
            suggestions.Add("Consider listening to native speakers.");
        }

        return suggestions;
    }

    #endregion
}
