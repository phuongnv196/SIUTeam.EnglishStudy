using MongoDB.Bson;
using SIUTeam.EnglishStudy.Core.DTOs.Vocabulary;
using SIUTeam.EnglishStudy.Core.Interfaces.Repositories;
using SIUTeam.EnglishStudy.Core.Interfaces.Services;

namespace SIUTeam.EnglishStudy.Infrastructure.Services;

public class VocabularyService : IVocabularyService
{
    private readonly IVocabularyRepository _vocabularyRepository;

    public VocabularyService(IVocabularyRepository vocabularyRepository)
    {
        _vocabularyRepository = vocabularyRepository;
    }

    public async Task<IEnumerable<VocabularyItemDto>> GetAllAsync()
    {
        var vocabularyItems = await _vocabularyRepository.GetAllAsync();
        return vocabularyItems.Select(MapToDto);
    }

    public async Task<IEnumerable<VocabularyItemDto>> GetByUserIdAsync(string userId)
    {
        if (!Guid.TryParse(userId, out var guidId))
            return Enumerable.Empty<VocabularyItemDto>();

        var vocabularyItems = await _vocabularyRepository.GetByUserIdAsync(guidId);
        return vocabularyItems.Select(MapToDto);
    }

    public async Task<IEnumerable<VocabularyItemDto>> GetByTopicAsync(string topic, string? userId = null)
    {
        Guid? userGuid = null;
        if (userId != null && Guid.TryParse(userId, out var parsed))
            userGuid = parsed;

        var vocabularyItems = await _vocabularyRepository.GetByTopicAsync(topic, userGuid);
        return vocabularyItems.Select(MapToDto);
    }

    public async Task<IEnumerable<VocabularyItemDto>> GetByLevelAsync(string level, string? userId = null)
    {
        if (!Enum.TryParse<VocabularyLevel>(level, true, out var vocabularyLevel))
            return Enumerable.Empty<VocabularyItemDto>();

        Guid? userGuid = null;
        if (userId != null && Guid.TryParse(userId, out var parsed))
            userGuid = parsed;

        var vocabularyItems = await _vocabularyRepository.GetByLevelAsync(vocabularyLevel, userGuid);
        return vocabularyItems.Select(MapToDto);
    }

    public async Task<VocabularyItemDto?> GetByIdAsync(string id)
    {
        if (!Guid.TryParse(id, out var guidId))
            return null;

        var vocabularyItem = await _vocabularyRepository.GetByIdAsync(guidId);
        return vocabularyItem != null ? MapToDto(vocabularyItem) : null;
    }

    public async Task<VocabularyItemDto?> GetByWordAsync(string word, string? userId = null)
    {
        Guid? userGuid = null;
        if (userId != null && Guid.TryParse(userId, out var parsed))
            userGuid = parsed;

        var vocabularyItem = await _vocabularyRepository.GetByWordAsync(word, userGuid);
        return vocabularyItem != null ? MapToDto(vocabularyItem) : null;
    }

    public async Task<VocabularyItemDto> CreateAsync(CreateVocabularyItemDto createDto, string? userId = null)
    {
        var vocabularyItem = new VocabularyItem
        {
            Word = createDto.Word,
            Pronunciation = createDto.Pronunciation,
            Meaning = createDto.Meaning,
            Example = createDto.Example,
            Level = Enum.TryParse<VocabularyLevel>(createDto.Level, true, out var level) ? level : VocabularyLevel.Beginner,
            Topic = createDto.Topic,
            Learned = createDto.Learned,
            UserId = userId != null && Guid.TryParse(userId, out var userGuid) ? userGuid : null
        };

        var createdItem = await _vocabularyRepository.CreateAsync(vocabularyItem);
        return MapToDto(createdItem);
    }

    public async Task<VocabularyItemDto?> UpdateAsync(string id, UpdateVocabularyItemDto updateDto)
    {
        if (!Guid.TryParse(id, out var guidId))
            return null;

        var existingItem = await _vocabularyRepository.GetByIdAsync(guidId);
        if (existingItem == null)
            return null;

        // Update only provided fields
        if (!string.IsNullOrEmpty(updateDto.Word))
            existingItem.Word = updateDto.Word;
        if (!string.IsNullOrEmpty(updateDto.Pronunciation))
            existingItem.Pronunciation = updateDto.Pronunciation;
        if (!string.IsNullOrEmpty(updateDto.Meaning))
            existingItem.Meaning = updateDto.Meaning;
        if (!string.IsNullOrEmpty(updateDto.Example))
            existingItem.Example = updateDto.Example;
        if (!string.IsNullOrEmpty(updateDto.Level) && Enum.TryParse<VocabularyLevel>(updateDto.Level, true, out var level))
            existingItem.Level = level;
        if (!string.IsNullOrEmpty(updateDto.Topic))
            existingItem.Topic = updateDto.Topic;
        if (updateDto.Learned.HasValue)
            existingItem.Learned = updateDto.Learned.Value;

        var updatedItem = await _vocabularyRepository.UpdateAsync(guidId, existingItem);
        return updatedItem != null ? MapToDto(updatedItem) : null;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        if (!Guid.TryParse(id, out var guidId))
            return false;

        return await _vocabularyRepository.DeleteAsync(guidId);
    }

    public async Task<bool> MarkAsLearnedAsync(string id, bool learned = true)
    {
        if (!Guid.TryParse(id, out var guidId))
            return false;

        return await _vocabularyRepository.MarkAsLearnedAsync(guidId, learned);
    }

    public async Task<IEnumerable<VocabularyTopicDto>> GetTopicsWithStatsAsync(string? userId = null)
    {
        Guid? userGuid = null;
        if (userId != null && Guid.TryParse(userId, out var parsed))
            userGuid = parsed;

        var topics = await _vocabularyRepository.GetTopicsAsync(userGuid);
        var stats = await _vocabularyRepository.GetTopicStatsAsync(userGuid);

        var topicDtos = new List<VocabularyTopicDto>();

        // Default topic colors
        var topicColors = new Dictionary<string, string>
        {
            { "business", "#1890ff" },
            { "travel", "#52c41a" },
            { "technology", "#fa8c16" },
            { "daily", "#eb2f96" },
            { "academic", "#722ed1" }
        };

        foreach (var topic in topics)
        {
            var totalKey = $"{topic}_total";
            var learnedKey = $"{topic}_learned";
            
            var total = stats.GetValueOrDefault(totalKey, 0);
            var learned = stats.GetValueOrDefault(learnedKey, 0);
            var progress = total > 0 ? (int)Math.Round((double)learned / total * 100) : 0;

            topicDtos.Add(new VocabularyTopicDto
            {
                Key = topic,
                Label = CapitalizeFirstLetter(topic),
                Color = topicColors.GetValueOrDefault(topic, "#1890ff"),
                Progress = progress,
                TotalWords = total,
                LearnedWords = learned
            });
        }

        return topicDtos.OrderBy(t => t.Label);
    }

    public async Task<Dictionary<string, int>> GetTopicStatsAsync(string? userId = null)
    {
        Guid? userGuid = null;
        if (userId != null && Guid.TryParse(userId, out var parsed))
            userGuid = parsed;

        return await _vocabularyRepository.GetTopicStatsAsync(userGuid);
    }

    private static VocabularyItemDto MapToDto(VocabularyItem item)
    {
        return new VocabularyItemDto
        {
            Id = item.Id.ToString(),
            Word = item.Word,
            Pronunciation = item.Pronunciation,
            Meaning = item.Meaning,
            Example = item.Example,
            Level = item.Level.ToString(),
            Topic = item.Topic,
            Learned = item.Learned,
            UserId = item.UserId?.ToString(),
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt ?? item.CreatedAt
        };
    }

    private static string CapitalizeFirstLetter(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;
        
        return char.ToUpper(input[0]) + input.Substring(1).ToLower();
    }
}
