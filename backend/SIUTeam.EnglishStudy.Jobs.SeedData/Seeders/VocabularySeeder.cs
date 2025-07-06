using SIUTeam.EnglishStudy.Core.Entities;
using SIUTeam.EnglishStudy.Core.Interfaces.Repositories;

public static class VocabularySeeder
{
    public static async Task SeedAsync(IVocabularyRepository vocabularyRepository)
    {
        // Check if data already exists
        var existingData = await vocabularyRepository.GetAllAsync();
        if (existingData.Any())
            return;

        var vocabularyItems = new[]
        {
            new VocabularyItem
            {
                Word = "entrepreneur",
                Pronunciation = "/ˌɒntrəprəˈnɜː(r)/",
                Meaning = "Doanh nhân, người khởi nghiệp",
                Example = "She is a successful entrepreneur who started three companies.",
                Level = VocabularyLevel.Advanced,
                Topic = "business",
                Learned = true,
                UserId = null // Public vocabulary
            },
            new VocabularyItem
            {
                Word = "innovative",
                Pronunciation = "/ˈɪnəveɪtɪv/",
                Meaning = "Sáng tạo, đổi mới",
                Example = "The company is known for its innovative products.",
                Level = VocabularyLevel.Intermediate,
                Topic = "business",
                Learned = false,
                UserId = null
            },
            new VocabularyItem
            {
                Word = "collaboration",
                Pronunciation = "/kəˌlæbəˈreɪʃn/",
                Meaning = "Sự hợp tác, cộng tác",
                Example = "The project was successful due to great collaboration.",
                Level = VocabularyLevel.Intermediate,
                Topic = "business",
                Learned = true,
                UserId = null
            },
            new VocabularyItem
            {
                Word = "itinerary",
                Pronunciation = "/aɪˈtɪnərəri/",
                Meaning = "Lịch trình du lịch",
                Example = "We need to plan our itinerary for the Europe trip.",
                Level = VocabularyLevel.Intermediate,
                Topic = "travel",
                Learned = false,
                UserId = null
            },
            new VocabularyItem
            {
                Word = "algorithm",
                Pronunciation = "/ˈælɡərɪðəm/",
                Meaning = "Thuật toán",
                Example = "The new algorithm improves search efficiency by 50%.",
                Level = VocabularyLevel.Advanced,
                Topic = "technology",
                Learned = false,
                UserId = null
            },
            new VocabularyItem
            {
                Word = "artificial",
                Pronunciation = "/ˌɑːtɪˈfɪʃəl/",
                Meaning = "Nhân tạo",
                Example = "Artificial intelligence is transforming many industries.",
                Level = VocabularyLevel.Intermediate,
                Topic = "technology",
                Learned = true,
                UserId = null
            },
            new VocabularyItem
            {
                Word = "conversation",
                Pronunciation = "/ˌkɒnvəˈseɪʃən/",
                Meaning = "Cuộc trò chuyện",
                Example = "They had a long conversation about their future plans.",
                Level = VocabularyLevel.Beginner,
                Topic = "daily",
                Learned = true,
                UserId = null
            },
            new VocabularyItem
            {
                Word = "grocery",
                Pronunciation = "/ˈɡroʊsəri/",
                Meaning = "Cửa hàng tạp hóa",
                Example = "I need to stop by the grocery store on my way home.",
                Level = VocabularyLevel.Beginner,
                Topic = "daily",
                Learned = true,
                UserId = null
            },
            new VocabularyItem
            {
                Word = "academic",
                Pronunciation = "/ˌækəˈdemɪk/",
                Meaning = "Thuộc về học thuật",
                Example = "Her academic performance has been excellent this semester.",
                Level = VocabularyLevel.Intermediate,
                Topic = "academic",
                Learned = false,
                UserId = null
            },
            new VocabularyItem
            {
                Word = "thesis",
                Pronunciation = "/ˈθiːsɪs/",
                Meaning = "Luận án",
                Example = "She is working on her master's thesis about climate change.",
                Level = VocabularyLevel.Advanced,
                Topic = "academic",
                Learned = false,
                UserId = null
            }
        };

        foreach (var item in vocabularyItems)
        {
            await vocabularyRepository.CreateAsync(item);
        }
    }
}
