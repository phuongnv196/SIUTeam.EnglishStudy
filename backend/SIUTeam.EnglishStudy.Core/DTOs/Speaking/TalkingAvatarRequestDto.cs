using System.ComponentModel.DataAnnotations;

namespace SIUTeam.EnglishStudy.Core.DTOs;

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
