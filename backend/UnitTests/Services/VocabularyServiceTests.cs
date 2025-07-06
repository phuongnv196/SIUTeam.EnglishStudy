using FluentAssertions;
using Moq;
using SIUTeam.EnglishStudy.Core.DTOs.Vocabulary;
using SIUTeam.EnglishStudy.Core.Entities;
using SIUTeam.EnglishStudy.Core.Interfaces.Repositories;
using SIUTeam.EnglishStudy.Infrastructure.Services;
using Xunit;

namespace SIUTeam.EnglishStudy.Tests.Services;

public class VocabularyServiceTests
{
    private readonly Mock<IVocabularyRepository> _mockRepository;
    private readonly VocabularyService _service;

    public VocabularyServiceTests()
    {
        _mockRepository = new Mock<IVocabularyRepository>();
        _service = new VocabularyService(_mockRepository.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllVocabularyItems()
    {
        // Arrange
        var vocabularyItems = new List<VocabularyItem>
        {
            new VocabularyItem
            {
                Id = Guid.NewGuid(),
                Word = "test",
                Pronunciation = "/test/",
                Meaning = "Test meaning",
                Example = "Test example",
                Level = VocabularyLevel.Beginner,
                Topic = "test",
                Learned = false
            }
        };

        _mockRepository.Setup(x => x.GetAllAsync())
            .ReturnsAsync(vocabularyItems);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().Word.Should().Be("test");
    }

    [Fact]
    public async Task CreateAsync_WithValidData_ReturnsCreatedItem()
    {
        // Arrange
        var createDto = new CreateVocabularyItemDto
        {
            Word = "newword",
            Pronunciation = "/ˈnjuːwɜːrd/",
            Meaning = "New word meaning",
            Example = "This is a new word example.",
            Level = "Intermediate",
            Topic = "test",
            Learned = false
        };

        var vocabularyItem = new VocabularyItem
        {
            Id = Guid.NewGuid(),
            Word = createDto.Word,
            Pronunciation = createDto.Pronunciation,
            Meaning = createDto.Meaning,
            Example = createDto.Example,
            Level = VocabularyLevel.Intermediate,
            Topic = createDto.Topic,
            Learned = createDto.Learned,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(x => x.CreateAsync(It.IsAny<VocabularyItem>()))
            .ReturnsAsync(vocabularyItem);

        // Act
        var result = await _service.CreateAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.Word.Should().Be(createDto.Word);
        result.Level.Should().Be("Intermediate");
        _mockRepository.Verify(x => x.CreateAsync(It.IsAny<VocabularyItem>()), Times.Once);
    }

    [Fact]
    public async Task GetByTopicAsync_WithValidTopic_ReturnsFilteredItems()
    {
        // Arrange
        var topic = "business";
        var vocabularyItems = new List<VocabularyItem>
        {
            new VocabularyItem
            {
                Id = Guid.NewGuid(),
                Word = "entrepreneur",
                Topic = topic,
                Level = VocabularyLevel.Advanced,
                Meaning = "Business person",
                Pronunciation = "/test/",
                Example = "Example",
                Learned = false
            }
        };

        _mockRepository.Setup(x => x.GetByTopicAsync(topic, It.IsAny<Guid?>()))
            .ReturnsAsync(vocabularyItems);

        // Act
        var result = await _service.GetByTopicAsync(topic);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().Topic.Should().Be(topic);
        _mockRepository.Verify(x => x.GetByTopicAsync(topic, It.IsAny<Guid?>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsItem()
    {
        // Arrange
        var id = Guid.NewGuid();
        var vocabularyItem = new VocabularyItem
        {
            Id = id,
            Word = "test",
            Pronunciation = "/test/",
            Meaning = "Test meaning",
            Example = "Test example",
            Level = VocabularyLevel.Beginner,
            Topic = "test",
            Learned = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(x => x.GetByIdAsync(id))
            .ReturnsAsync(vocabularyItem);

        // Act
        var result = await _service.GetByIdAsync(id.ToString());

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(id.ToString());
        result.Word.Should().Be("test");
        _mockRepository.Verify(x => x.GetByIdAsync(id), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        var invalidId = "invalid-guid";

        // Act
        var result = await _service.GetByIdAsync(invalidId);

        // Assert
        result.Should().BeNull();
        _mockRepository.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_ReturnsTrue()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockRepository.Setup(x => x.DeleteAsync(id))
            .ReturnsAsync(true);

        // Act
        var result = await _service.DeleteAsync(id.ToString());

        // Assert
        result.Should().BeTrue();
        _mockRepository.Verify(x => x.DeleteAsync(id), Times.Once);
    }

    [Fact]
    public async Task MarkAsLearnedAsync_WithValidId_ReturnsTrue()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockRepository.Setup(x => x.MarkAsLearnedAsync(id, true))
            .ReturnsAsync(true);

        // Act
        var result = await _service.MarkAsLearnedAsync(id.ToString(), true);

        // Assert
        result.Should().BeTrue();
        _mockRepository.Verify(x => x.MarkAsLearnedAsync(id, true), Times.Once);
    }
}
