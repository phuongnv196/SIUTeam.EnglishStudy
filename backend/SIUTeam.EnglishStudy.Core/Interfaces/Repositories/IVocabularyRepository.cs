namespace SIUTeam.EnglishStudy.Core.Interfaces.Repositories;

public interface IVocabularyRepository
{
    Task<IEnumerable<VocabularyItem>> GetAllAsync();
    Task<IEnumerable<VocabularyItem>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<VocabularyItem>> GetByTopicAsync(string topic, Guid? userId = null);
    Task<IEnumerable<VocabularyItem>> GetByLevelAsync(VocabularyLevel level, Guid? userId = null);
    Task<VocabularyItem?> GetByIdAsync(Guid id);
    Task<VocabularyItem?> GetByWordAsync(string word, Guid? userId = null);
    Task<VocabularyItem> CreateAsync(VocabularyItem vocabularyItem);
    Task<VocabularyItem?> UpdateAsync(Guid id, VocabularyItem vocabularyItem);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> MarkAsLearnedAsync(Guid id, bool learned = true);
    Task<IEnumerable<string>> GetTopicsAsync(Guid? userId = null);
    Task<Dictionary<string, int>> GetTopicStatsAsync(Guid? userId = null);
    Task DeleteAllAsync();
}
