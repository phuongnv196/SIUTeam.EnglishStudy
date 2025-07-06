using System.ComponentModel.DataAnnotations;

namespace SIUTeam.EnglishStudy.Core.DTOs;

public class TextToIpaRequestDto
{
    [Required]
    [StringLength(1000, MinimumLength = 1)]
    public string Text { get; set; } = string.Empty;
}
