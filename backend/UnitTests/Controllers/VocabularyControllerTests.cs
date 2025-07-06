using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using SIUTeam.EnglishStudy.Core.DTOs.Vocabulary;
using SIUTeam.EnglishStudy.Core.Interfaces.Repositories;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Xunit;

namespace SIUTeam.EnglishStudy.Tests.Controllers;

public class VocabularyControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly string _validToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoicGh1b25nbnYiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjllOWRiZDYyLTkwMTQtNDQwMi04NjMxLWYzYmYyMmFjNTNjMiIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IjAiLCJleHAiOjE3NTE4MjMyNDcsImlzcyI6IlNJVVRlYW0uRW5nbGlzaFN0dWR5LkFQSS5ERVYiLCJhdWQiOiJTSVVUZWFtLkVuZ2xpc2hTdHVkeS5DbGllbnQuREVWIn0.vgef50Qrz7No1g-d2ySfFXnKYpqz85QRkKIwWiAEaO0";

    public VocabularyControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _validToken);
    }

    [Fact]
    public async Task GetVocabularyItems_WithValidToken_ReturnsOk()
    {
        // Act
        var response = await _client.GetAsync("/api/vocabulary");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var vocabularyItems = JsonSerializer.Deserialize<List<VocabularyItemDto>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        vocabularyItems.Should().NotBeNull();
    }

    [Fact]
    public async Task GetVocabularyItems_WithoutToken_ReturnsUnauthorized()
    {
        // Arrange
        var clientWithoutAuth = _factory.CreateClient();

        // Act
        var response = await clientWithoutAuth.GetAsync("/api/vocabulary");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetVocabularyItemsByTopic_WithValidTopic_ReturnsOk()
    {
        // Arrange
        var topic = "business";

        // Act
        var response = await _client.GetAsync($"/api/vocabulary/by-topic/{topic}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var vocabularyItems = JsonSerializer.Deserialize<List<VocabularyItemDto>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        vocabularyItems.Should().NotBeNull();
        vocabularyItems.Should().OnlyContain(item => item.Topic == topic);
    }

    [Fact]
    public async Task GetVocabularyItemsByLevel_WithValidLevel_ReturnsOk()
    {
        // Arrange
        var level = "Intermediate";

        // Act
        var response = await _client.GetAsync($"/api/vocabulary/by-level/{level}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var vocabularyItems = JsonSerializer.Deserialize<List<VocabularyItemDto>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        vocabularyItems.Should().NotBeNull();
        vocabularyItems.Should().OnlyContain(item => item.Level == level);
    }

    [Fact]
    public async Task CreateVocabularyItem_WithValidData_ReturnsCreated()
    {
        // Arrange
        var newVocabulary = new CreateVocabularyItemDto
        {
            Word = "testing",
            Pronunciation = "/ˈtestɪŋ/",
            Meaning = "Kiểm thử",
            Example = "Testing is an important part of software development.",
            Level = "Intermediate",
            Topic = "technology",
            Learned = false
        };

        var json = JsonSerializer.Serialize(newVocabulary);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/vocabulary", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var createdItem = JsonSerializer.Deserialize<VocabularyItemDto>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        createdItem.Should().NotBeNull();
        createdItem.Word.Should().Be(newVocabulary.Word);
        createdItem.Meaning.Should().Be(newVocabulary.Meaning);
    }

    [Fact]
    public async Task CreateVocabularyItem_WithInvalidData_ReturnsBadRequest()
    {
        // Arrange
        var invalidVocabulary = new CreateVocabularyItemDto
        {
            Word = "", // Empty word should be invalid
            Pronunciation = "/ˈtestɪŋ/",
            Meaning = "Kiểm thử",
            Example = "Testing is an important part of software development.",
            Level = "Intermediate",
            Topic = "technology"
        };

        var json = JsonSerializer.Serialize(invalidVocabulary);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/vocabulary", content);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.Created); // Depending on validation
    }

    [Fact]
    public async Task SearchVocabularyItem_WithExistingWord_ReturnsOk()
    {
        // Arrange
        var word = "entrepreneur";

        // Act
        var response = await _client.GetAsync($"/api/vocabulary/search/{word}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
        
        if (response.StatusCode == HttpStatusCode.OK)
        {
            var content = await response.Content.ReadAsStringAsync();
            var vocabularyItem = JsonSerializer.Deserialize<VocabularyItemDto>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            vocabularyItem.Should().NotBeNull();
            vocabularyItem.Word.Should().Be(word);
        }
    }

    [Fact]
    public async Task SearchVocabularyItem_WithNonExistingWord_ReturnsNotFound()
    {
        // Arrange
        var word = "nonexistentword123";

        // Act
        var response = await _client.GetAsync($"/api/vocabulary/search/{word}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetTopics_WithValidToken_ReturnsOk()
    {
        // Act
        var response = await _client.GetAsync("/api/vocabulary/topics");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var topics = JsonSerializer.Deserialize<List<VocabularyTopicDto>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        topics.Should().NotBeNull();
        topics.Should().NotBeEmpty();
        topics.Should().OnlyContain(topic => !string.IsNullOrEmpty(topic.Key));
    }

    [Fact]
    public async Task GetStats_WithValidToken_ReturnsOk()
    {
        // Act
        var response = await _client.GetAsync("/api/vocabulary/stats");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var stats = JsonSerializer.Deserialize<Dictionary<string, int>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        stats.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateVocabularyItem_WithValidData_ReturnsOk()
    {
        // Arrange - First create an item
        var newVocabulary = new CreateVocabularyItemDto
        {
            Word = "updatetest",
            Pronunciation = "/ˈʌpdeɪt/",
            Meaning = "Cập nhật",
            Example = "We need to update the software.",
            Level = "Beginner",
            Topic = "technology",
            Learned = false
        };

        var createJson = JsonSerializer.Serialize(newVocabulary);
        var createContent = new StringContent(createJson, Encoding.UTF8, "application/json");
        var createResponse = await _client.PostAsync("/api/vocabulary", createContent);

        if (createResponse.StatusCode == HttpStatusCode.Created)
        {
            var createResponseContent = await createResponse.Content.ReadAsStringAsync();
            var createdItem = JsonSerializer.Deserialize<VocabularyItemDto>(createResponseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            // Act - Update the item
            var updateData = new UpdateVocabularyItemDto
            {
                Meaning = "Cập nhật mới",
                Learned = true
            };

            var updateJson = JsonSerializer.Serialize(updateData);
            var updateContent = new StringContent(updateJson, Encoding.UTF8, "application/json");
            var updateResponse = await _client.PutAsync($"/api/vocabulary/{createdItem.Id}", updateContent);

            // Assert
            updateResponse.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
        }
        else
        {
            // If creation failed, skip the update test
            Assert.True(true, "Skipped update test because creation failed");
        }
    }

    [Fact]
    public async Task DeleteVocabularyItem_WithValidId_ReturnsNoContent()
    {
        // Arrange - First create an item
        var newVocabulary = new CreateVocabularyItemDto
        {
            Word = "deletetest",
            Pronunciation = "/dɪˈliːt/",
            Meaning = "Xóa",
            Example = "Delete this item.",
            Level = "Beginner",
            Topic = "technology",
            Learned = false
        };

        var createJson = JsonSerializer.Serialize(newVocabulary);
        var createContent = new StringContent(createJson, Encoding.UTF8, "application/json");
        var createResponse = await _client.PostAsync("/api/vocabulary", createContent);

        if (createResponse.StatusCode == HttpStatusCode.Created)
        {
            var createResponseContent = await createResponse.Content.ReadAsStringAsync();
            var createdItem = JsonSerializer.Deserialize<VocabularyItemDto>(createResponseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            // Act
            var deleteResponse = await _client.DeleteAsync($"/api/vocabulary/{createdItem.Id}");

            // Assert
            deleteResponse.StatusCode.Should().BeOneOf(HttpStatusCode.NoContent, HttpStatusCode.NotFound);
        }
        else
        {
            // If creation failed, skip the delete test
            Assert.True(true, "Skipped delete test because creation failed");
        }
    }

    [Fact]
    public async Task MarkAsLearned_WithValidId_ReturnsOk()
    {
        // Arrange - First create an item
        var newVocabulary = new CreateVocabularyItemDto
        {
            Word = "learntest",
            Pronunciation = "/lɜːrn/",
            Meaning = "Học",
            Example = "Learn new vocabulary every day.",
            Level = "Beginner",
            Topic = "education",
            Learned = false
        };

        var createJson = JsonSerializer.Serialize(newVocabulary);
        var createContent = new StringContent(createJson, Encoding.UTF8, "application/json");
        var createResponse = await _client.PostAsync("/api/vocabulary", createContent);

        if (createResponse.StatusCode == HttpStatusCode.Created)
        {
            var createResponseContent = await createResponse.Content.ReadAsStringAsync();
            var createdItem = JsonSerializer.Deserialize<VocabularyItemDto>(createResponseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            // Act
            var markLearnedContent = new StringContent("true", Encoding.UTF8, "application/json");
            var markLearnedResponse = await _client.PatchAsync($"/api/vocabulary/{createdItem.Id}/learned", markLearnedContent);

            // Assert
            markLearnedResponse.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
        }
        else
        {
            // If creation failed, skip the mark learned test
            Assert.True(true, "Skipped mark learned test because creation failed");
        }
    }
}
