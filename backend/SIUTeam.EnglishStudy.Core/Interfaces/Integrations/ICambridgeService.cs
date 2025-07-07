using SIUTeam.EnglishStudy.Core.DTOs;

namespace SIUTeam.EnglishStudy.Core.Interfaces.Integrations
{
    public interface ICambridgeService
    {
        Task<List<CambridgeDictionaryDto>> GetWordDetailsAsync(string word, string region = "us");
        Task<Stream> GetAudioAsync(string audioKey);
    }
}
