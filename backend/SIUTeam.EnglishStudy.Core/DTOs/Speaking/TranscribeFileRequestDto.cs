using System.ComponentModel.DataAnnotations;

namespace SIUTeam.EnglishStudy.Core.DTOs;

public class TranscribeFileRequestDto
{
    [Required]
    public IFileUpload File { get; set; } = null!;
}
