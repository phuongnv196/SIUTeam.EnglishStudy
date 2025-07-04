using System.ComponentModel.DataAnnotations;

namespace SIUTeam.EnglishStudy.Core.DTOs;

// Base interface for file upload
public interface IFileUpload
{
    Stream GetStream();
    string FileName { get; }
    string ContentType { get; }
    long Length { get; }
}

// Request DTOs
public class TranscribeFileRequestDto
{
    [Required]
    public IFileUpload File { get; set; } = null!;
}

public class TranscribeChunkRequestDto
{
    [Required]
    public IFileUpload Chunk { get; set; } = null!;
}

public class TextToIpaRequestDto
{
    [Required]
    [StringLength(1000, MinimumLength = 1)]
    public string Text { get; set; } = string.Empty;
}

public class TalkingAvatarRequestDto
{
    [Required]
    [StringLength(500, MinimumLength = 1)]
    public string Text { get; set; } = string.Empty;
    
    [Range(0.5, 30.0)]
    public double Duration { get; set; } = 3.0;
    
    [Range(15, 60)]
    public int Fps { get; set; } = 30;
}

// Response DTOs
public class IpaResultDto
{
    public string G2pIpa { get; set; } = string.Empty;
    public string EpitranIpa { get; set; } = string.Empty;
    public string Arpabet { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string? Error { get; set; }
}

public class SegmentDto
{
    public int Id { get; set; }
    public double Start { get; set; }
    public double End { get; set; }
    public string Text { get; set; } = string.Empty;
}

public class TranscriptionResponseDto
{
    public bool Success { get; set; }
    public string Filename { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public double LanguageProbability { get; set; }
    public double Duration { get; set; }
    public double ProcessingTime { get; set; }
    public List<SegmentDto> Segments { get; set; } = new();
    public string Model { get; set; } = string.Empty;
    public IpaResultDto Ipa { get; set; } = new();
    public string? Error { get; set; }
}

public class ChunkTranscriptionResponseDto
{
    public bool Success { get; set; }
    public string Text { get; set; } = string.Empty;
    public double ProcessingTime { get; set; }
    public string Language { get; set; } = string.Empty;
    public IpaResultDto Ipa { get; set; } = new();
    public string? Error { get; set; }
}

public class TextToIpaResponseDto
{
    public bool Success { get; set; }
    public string OriginalText { get; set; } = string.Empty;
    public IpaResultDto Ipa { get; set; } = new();
    public string? Error { get; set; }
}

// Animation DTOs
public class VisemeDto
{
    public string Name { get; set; } = string.Empty;
    public double StartTime { get; set; }
    public double Duration { get; set; }
}

public class KeyframeDto
{
    public int Frame { get; set; }
    public double Time { get; set; }
    public string Viseme { get; set; } = string.Empty;
    public double Weight { get; set; }
}

public class AnimationDataDto
{
    public double Duration { get; set; }
    public int Fps { get; set; }
    public int TotalFrames { get; set; }
    public int VisemesCount { get; set; }
    public int PhonemesCount { get; set; }
    public List<KeyframeDto> Keyframes { get; set; } = new();
    public List<VisemeDto> Visemes { get; set; } = new();
}

public class TalkingAvatarResponseDto
{
    public bool Success { get; set; }
    public string Text { get; set; } = string.Empty;
    public string IpaText { get; set; } = string.Empty;
    public string Arpabet { get; set; } = string.Empty;
    public double ProcessingTime { get; set; }
    public AnimationDataDto Animation { get; set; } = new();
    public string? Error { get; set; }
}

// Real-time speaking DTOs
public class SpeakingSessionDto
{
    public string SessionId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string LessonId { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public bool IsActive { get; set; }
}

public class SpeakingResultDto
{
    public string SessionId { get; set; } = string.Empty;
    public string TranscribedText { get; set; } = string.Empty;
    public string ExpectedText { get; set; } = string.Empty;
    public double ConfidenceScore { get; set; }
    public double ProcessingTime { get; set; }
    public IpaResultDto Ipa { get; set; } = new();
    public List<string> Suggestions { get; set; } = new();
}
