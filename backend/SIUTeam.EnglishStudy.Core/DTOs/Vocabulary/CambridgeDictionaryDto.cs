namespace SIUTeam.EnglishStudy.Core.DTOs
{
    public class CambrigeDictionaryRegionDto
    {
        public string Name { get; set; }
        public string AudioKey { get; set; }
        public string Ipa { get; set; }
    }

    public class CambridgeDictionaryDto
    {
        public string Word { get; set; }
        public string Posgram { get; set; }
        public string Description { get; set; }
        public string Level { get; set; }
        public List<CambrigeDictionaryRegionDto> Regions { get; set; } = new List<CambrigeDictionaryRegionDto>();
        public List<string> Synonyms { get; set; } = new List<string>();
        public List<string> Examples { get; set; } = new List<string>();
        public bool IsSuccess { get; set; } = true;
    }
}
