using MongoDB.Bson;
using SIUTeam.EnglishStudy.Core.DTOs.Vocabulary;

namespace SIUTeam.EnglishStudy.Core.Interfaces.Services;

public interface IVocabularyService
{
    Task<IEnumerable<VocabularyItemDto>> GetAllAsync();
    Task<IEnumerable<VocabularyItemDto>> GetByUserIdAsync(string userId);
    Task<IEnumerable<VocabularyItemDto>> GetByTopicAsync(string topic, string? userId = null);
    Task<IEnumerable<VocabularyItemDto>> GetByLevelAsync(string level, string? userId = null);
    Task<VocabularyItemDto?> GetByIdAsync(string id);
    Task<VocabularyItemDto?> GetByWordAsync(string word, string? userId = null);
    Task<VocabularyItemDto> CreateAsync(CreateVocabularyItemDto createDto, string? userId = null);
    Task<VocabularyItemDto?> UpdateAsync(string id, UpdateVocabularyItemDto updateDto);
    Task<bool> DeleteAsync(string id);
    Task<bool> MarkAsLearnedAsync(string id, bool learned = true);
    Task<IEnumerable<VocabularyTopicDto>> GetTopicsWithStatsAsync(string? userId = null);
    Task<Dictionary<string, int>> GetTopicStatsAsync(string? userId = null);
}
