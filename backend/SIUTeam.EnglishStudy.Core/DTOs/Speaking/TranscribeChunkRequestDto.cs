using System.ComponentModel.DataAnnotations;

namespace SIUTeam.EnglishStudy.Core.DTOs;

public class TranscribeChunkRequestDto
{
    [Required]
    public IFileUpload Chunk { get; set; } = null!;
}
