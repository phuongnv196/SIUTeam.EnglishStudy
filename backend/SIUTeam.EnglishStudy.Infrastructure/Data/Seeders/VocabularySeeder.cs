using SIUTeam.EnglishStudy.Core.Interfaces.Repositories;
using SIUTeam.EnglishStudy.Core.Entities.Vocabulary;
using SIUTeam.EnglishStudy.Infrastructure.Data;
using MongoDB.Driver;

namespace SIUTeam.EnglishStudy.Infrastructure.Data.Seeders;

public static class VocabularySeeder
{
    public static async Task SeedAsync(IVocabularyRepository vocabularyRepository, MongoDbContext context)
    {
        // Clear existing data using direct MongoDB access to avoid serialization issues
        try
        {
            await context.VocabularyItems.DeleteManyAsync(_ => true);
            Console.WriteLine("Cleared existing vocabulary data.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error clearing existing data: {ex.Message}");
            Console.WriteLine("This is normal if the collection doesn't exist.");
        }

        var vocabularyItems = new[]
        {
            // Business vocabulary (15 words)
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
                Word = "revenue",
                Pronunciation = "/ˈrevənjuː/",
                Meaning = "Doanh thu",
                Example = "The company's revenue increased by 20% this quarter.",
                Level = VocabularyLevel.Intermediate,
                Topic = "business",
                Learned = false,
                UserId = null
            },
            new VocabularyItem
            {
                Word = "strategy",
                Pronunciation = "/ˈstrætədʒi/",
                Meaning = "Chiến lược",
                Example = "We need a new marketing strategy to reach younger customers.",
                Level = VocabularyLevel.Intermediate,
                Topic = "business",
                Learned = false,
                UserId = null
            },
            new VocabularyItem
            {
                Word = "investment",
                Pronunciation = "/ɪnˈvestmənt/",
                Meaning = "Đầu tư",
                Example = "The startup is looking for investment to expand their business.",
                Level = VocabularyLevel.Intermediate,
                Topic = "business",
                Learned = false,
                UserId = null
            },
            new VocabularyItem
            {
                Word = "portfolio",
                Pronunciation = "/pɔːtˈfoʊlioʊ/",
                Meaning = "Danh mục đầu tư",
                Example = "His investment portfolio includes stocks and bonds.",
                Level = VocabularyLevel.Advanced,
                Topic = "business",
                Learned = false,
                UserId = null
            },
            new VocabularyItem
            {
                Word = "stakeholder",
                Pronunciation = "/ˈsteɪkhoʊldər/",
                Meaning = "Bên liên quan",
                Example = "All stakeholders must be informed about the company changes.",
                Level = VocabularyLevel.Advanced,
                Topic = "business",
                Learned = false,
                UserId = null
            },
            new VocabularyItem
            {
                Word = "merger",
                Pronunciation = "/ˈmɜːrdʒər/",
                Meaning = "Sáp nhập",
                Example = "The merger between the two companies was announced yesterday.",
                Level = VocabularyLevel.Advanced,
                Topic = "business",
                Learned = false,
                UserId = null
            },
            new VocabularyItem
            {
                Word = "competitive",
                Pronunciation = "/kəmˈpetətɪv/",
                Meaning = "Cạnh tranh",
                Example = "The smartphone market is very competitive.",
                Level = VocabularyLevel.Intermediate,
                Topic = "business",
                Learned = false,
                UserId = null
            },
            new VocabularyItem
            {
                Word = "profit",
                Pronunciation = "/ˈprɑːfɪt/",
                Meaning = "Lợi nhuận",
                Example = "The company made a significant profit last year.",
                Level = VocabularyLevel.Beginner,
                Topic = "business",
                Learned = true,
                UserId = null
            },
            new VocabularyItem
            {
                Word = "budget",
                Pronunciation = "/ˈbʌdʒɪt/",
                Meaning = "Ngân sách",
                Example = "We need to stay within the budget for this project.",
                Level = VocabularyLevel.Beginner,
                Topic = "business",
                Learned = false,
                UserId = null
            },
            new VocabularyItem
            {
                Word = "networking",
                Pronunciation = "/ˈnetˌwɜːrkɪŋ/",
                Meaning = "Xây dựng mạng lưới",
                Example = "Networking is essential for career development.",
                Level = VocabularyLevel.Intermediate,
                Topic = "business",
                Learned = false,
                UserId = null
            },
            new VocabularyItem
            {
                Word = "franchise",
                Pronunciation = "/ˈfrænˌtʃaɪz/",
                Meaning = "Nhượng quyền thương mại",
                Example = "McDonald's operates through a franchise system.",
                Level = VocabularyLevel.Advanced,
                Topic = "business",
                Learned = false,
                UserId = null
            },
            new VocabularyItem
            {
                Word = "efficiency",
                Pronunciation = "/ɪˈfɪʃənsi/",
                Meaning = "Hiệu quả",
                Example = "The new system improved workplace efficiency by 30%.",
                Level = VocabularyLevel.Intermediate,
                Topic = "business",
                Learned = false,
                UserId = null
            },

            // Technology vocabulary (15 words)
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
                Word = "database",
                Pronunciation = "/ˈdeɪtəbeɪs/",
                Meaning = "Cơ sở dữ liệu",
                Example = "The customer information is stored in our database.",
                Level = VocabularyLevel.Intermediate,
                Topic = "technology",
                Learned = false,
                UserId = null
            },
            new VocabularyItem
            {
                Word = "cybersecurity",
                Pronunciation = "/ˈsaɪbərsɪˌkjʊrəti/",
                Meaning = "An ninh mạng",
                Example = "Cybersecurity is a top priority for our IT department.",
                Level = VocabularyLevel.Advanced,
                Topic = "technology",
                Learned = false,
                UserId = null
            },
            new VocabularyItem
            {
                Word = "software",
                Pronunciation = "/ˈsɔːftwer/",
                Meaning = "Phần mềm",
                Example = "We need to update the software to fix security issues.",
                Level = VocabularyLevel.Beginner,
                Topic = "technology",
                Learned = true,
                UserId = null
            },
            new VocabularyItem
            {
                Word = "innovation",
                Pronunciation = "/ˌɪnəˈveɪʃən/",
                Meaning = "Sự đổi mới",
                Example = "Apple is known for its technological innovation.",
                Level = VocabularyLevel.Intermediate,
                Topic = "technology",
                Learned = false,
                UserId = null
            },
            new VocabularyItem
            {
                Word = "programming",
                Pronunciation = "/ˈproʊɡræmɪŋ/",
                Meaning = "Lập trình",
                Example = "He learned programming languages like Python and Java.",
                Level = VocabularyLevel.Intermediate,
                Topic = "technology",
                Learned = true,
                UserId = null
            },
            new VocabularyItem
            {
                Word = "blockchain",
                Pronunciation = "/ˈblɑːktʃeɪn/",
                Meaning = "Chuỗi khối",
                Example = "Blockchain technology ensures secure transactions.",
                Level = VocabularyLevel.Advanced,
                Topic = "technology",
                Learned = false,
                UserId = null
            },
            new VocabularyItem
            {
                Word = "automation",
                Pronunciation = "/ˌɔːtəˈmeɪʃən/",
                Meaning = "Tự động hóa",
                Example = "Factory automation has increased production efficiency.",
                Level = VocabularyLevel.Advanced,
                Topic = "technology",
                Learned = false,
                UserId = null
            },
            new VocabularyItem
            {
                Word = "interface",
                Pronunciation = "/ˈɪntərfeɪs/",
                Meaning = "Giao diện",
                Example = "The user interface of this app is very intuitive.",
                Level = VocabularyLevel.Intermediate,
                Topic = "technology",
                Learned = false,
                UserId = null
            },
            new VocabularyItem
            {
                Word = "cloud",
                Pronunciation = "/klaʊd/",
                Meaning = "Điện toán đám mây",
                Example = "We store all our files in the cloud for easy access.",
                Level = VocabularyLevel.Intermediate,
                Topic = "technology",
                Learned = true,
                UserId = null
            },
            new VocabularyItem
            {
                Word = "virtual",
                Pronunciation = "/ˈvɜːrtʃuəl/",
                Meaning = "Ảo",
                Example = "Virtual reality games are becoming more popular.",
                Level = VocabularyLevel.Intermediate,
                Topic = "technology",
                Learned = false,
                UserId = null
            },
            new VocabularyItem
            {
                Word = "encryption",
                Pronunciation = "/ɪnˈkrɪpʃən/",
                Meaning = "Mã hóa",
                Example = "End-to-end encryption protects your messages from hackers.",
                Level = VocabularyLevel.Advanced,
                Topic = "technology",
                Learned = false,
                UserId = null
            },
            new VocabularyItem
            {
                Word = "bandwidth",
                Pronunciation = "/ˈbændwɪdθ/",
                Meaning = "Băng thông",
                Example = "High bandwidth is needed for streaming 4K videos.",
                Level = VocabularyLevel.Advanced,
                Topic = "technology",
                Learned = false,
                UserId = null
            },
            new VocabularyItem
            {
                Word = "optimization",
                Pronunciation = "/ˌɑːptəməˈzeɪʃən/",
                Meaning = "Tối ưu hóa",
                Example = "Code optimization can significantly improve performance.",
                Level = VocabularyLevel.Advanced,
                Topic = "technology",
                Learned = false,
                UserId = null
            },

            // Academic vocabulary (10 words)
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
            },
            new VocabularyItem
            {
                Word = "research",
                Pronunciation = "/ˈriːsɜːrtʃ/",
                Meaning = "Nghiên cứu",
                Example = "His research focuses on renewable energy sources.",
                Level = VocabularyLevel.Intermediate,
                Topic = "academic",
                Learned = true,
                UserId = null
            },
            new VocabularyItem
            {
                Word = "hypothesis",
                Pronunciation = "/haɪˈpɑːθəsɪs/",
                Meaning = "Giả thuyết",
                Example = "The scientist proposed a new hypothesis about global warming.",
                Level = VocabularyLevel.Advanced,
                Topic = "academic",
                Learned = false,
                UserId = null
            },
            new VocabularyItem
            {
                Word = "methodology",
                Pronunciation = "/ˌmeθəˈdɑːlədʒi/",
                Meaning = "Phương pháp luận",
                Example = "The research methodology must be clearly explained.",
                Level = VocabularyLevel.Advanced,
                Topic = "academic",
                Learned = false,
                UserId = null
            },
            new VocabularyItem
            {
                Word = "analysis",
                Pronunciation = "/əˈnæləsɪs/",
                Meaning = "Phân tích",
                Example = "The data analysis revealed interesting patterns.",
                Level = VocabularyLevel.Intermediate,
                Topic = "academic",
                Learned = true,
                UserId = null
            },
            new VocabularyItem
            {
                Word = "curriculum",
                Pronunciation = "/kəˈrɪkjələm/",
                Meaning = "Chương trình học",
                Example = "The new curriculum includes more practical courses.",
                Level = VocabularyLevel.Intermediate,
                Topic = "academic",
                Learned = false,
                UserId = null
            },
            new VocabularyItem
            {
                Word = "scholarship",
                Pronunciation = "/ˈskɑːlərʃɪp/",
                Meaning = "Học bổng",
                Example = "She received a full scholarship to study at Harvard.",
                Level = VocabularyLevel.Intermediate,
                Topic = "academic",
                Learned = false,
                UserId = null
            },
            new VocabularyItem
            {
                Word = "dissertation",
                Pronunciation = "/ˌdɪsərˈteɪʃən/",
                Meaning = "Luận văn tiến sĩ",
                Example = "He defended his dissertation on artificial intelligence.",
                Level = VocabularyLevel.Advanced,
                Topic = "academic",
                Learned = false,
                UserId = null
            },
            new VocabularyItem
            {
                Word = "seminar",
                Pronunciation = "/ˈsemɪnɑːr/",
                Meaning = "Hội thảo",
                Example = "The university organizes weekly seminars on various topics.",
                Level = VocabularyLevel.Intermediate,
                Topic = "academic",
                Learned = false,
                UserId = null
            },

            // Education vocabulary (10 words)
            new VocabularyItem
            {
                Word = "pedagogy",
                Pronunciation = "/ˈpedəɡɑːdʒi/",
                Meaning = "Phương pháp giảng dạy",
                Example = "Modern pedagogy emphasizes student-centered learning.",
                Level = VocabularyLevel.Advanced,
                Topic = "education",
                Learned = false,
                UserId = null
            },
            new VocabularyItem
            {
                Word = "assessment",
                Pronunciation = "/əˈsesmənt/",
                Meaning = "Đánh giá",
                Example = "Regular assessment helps track student progress.",
                Level = VocabularyLevel.Intermediate,
                Topic = "education",
                Learned = true,
                UserId = null
            },
            new VocabularyItem
            {
                Word = "literacy",
                Pronunciation = "/ˈlɪtərəsi/",
                Meaning = "Khả năng đọc viết",
                Example = "Digital literacy is essential in the modern workplace.",
                Level = VocabularyLevel.Intermediate,
                Topic = "education",
                Learned = false,
                UserId = null
            },
            new VocabularyItem
            {
                Word = "enrollment",
                Pronunciation = "/ɪnˈroʊlmənt/",
                Meaning = "Đăng ký nhập học",
                Example = "University enrollment has increased by 15% this year.",
                Level = VocabularyLevel.Intermediate,
                Topic = "education",
                Learned = false,
                UserId = null
            },
            new VocabularyItem
            {
                Word = "tuition",
                Pronunciation = "/tuˈɪʃən/",
                Meaning = "Học phí",
                Example = "College tuition has risen significantly over the past decade.",
                Level = VocabularyLevel.Intermediate,
                Topic = "education",
                Learned = true,
                UserId = null
            },
            new VocabularyItem
            {
                Word = "graduation",
                Pronunciation = "/ˌɡrædʒuˈeɪʃən/",
                Meaning = "Tốt nghiệp",
                Example = "The graduation ceremony will be held next month.",
                Level = VocabularyLevel.Beginner,
                Topic = "education",
                Learned = false,
                UserId = null
            },
            new VocabularyItem
            {
                Word = "comprehension",
                Pronunciation = "/ˌkɑːmprɪˈhenʃən/",
                Meaning = "Sự hiểu biết",
                Example = "Reading comprehension skills are crucial for academic success.",
                Level = VocabularyLevel.Advanced,
                Topic = "education",
                Learned = false,
                UserId = null
            },
            new VocabularyItem
            {
                Word = "inclusive",
                Pronunciation = "/ɪnˈkluːsɪv/",
                Meaning = "Bao gồm, hòa nhập",
                Example = "The school promotes inclusive education for all students.",
                Level = VocabularyLevel.Advanced,
                Topic = "education",
                Learned = false,
                UserId = null
            },
            new VocabularyItem
            {
                Word = "extracurricular",
                Pronunciation = "/ˌekstrəkəˈrɪkjələr/",
                Meaning = "Ngoại khóa",
                Example = "Extracurricular activities help develop social skills.",
                Level = VocabularyLevel.Advanced,
                Topic = "education",
                Learned = false,
                UserId = null
            },
            new VocabularyItem
            {
                Word = "accreditation",
                Pronunciation = "/əˌkredɪˈteɪʃən/",
                Meaning = "Sự công nhận, chứng nhận",
                Example = "The university received full accreditation for its programs.",
                Level = VocabularyLevel.Advanced,
                Topic = "education",
                Learned = false,
                UserId = null
            },

            // Daily life vocabulary (10 words)
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
                Word = "appointment",
                Pronunciation = "/əˈpɔɪntmənt/",
                Meaning = "Cuộc hẹn",
                Example = "I have a doctor's appointment at 3 PM today.",
                Level = VocabularyLevel.Beginner,
                Topic = "daily",
                Learned = false,
                UserId = null
            },
            new VocabularyItem
            {
                Word = "commute",
                Pronunciation = "/kəˈmjuːt/",
                Meaning = "Đi lại hàng ngày",
                Example = "My daily commute to work takes about 45 minutes.",
                Level = VocabularyLevel.Intermediate,
                Topic = "daily",
                Learned = false,
                UserId = null
            },
            new VocabularyItem
            {
                Word = "routine",
                Pronunciation = "/ruːˈtiːn/",
                Meaning = "Thói quen hàng ngày",
                Example = "Exercise is an important part of my daily routine.",
                Level = VocabularyLevel.Intermediate,
                Topic = "daily",
                Learned = false,
                UserId = null
            },
            new VocabularyItem
            {
                Word = "household",
                Pronunciation = "/ˈhaʊshoʊld/",
                Meaning = "Hộ gia đình",
                Example = "Household chores need to be shared among family members.",
                Level = VocabularyLevel.Intermediate,
                Topic = "daily",
                Learned = false,
                UserId = null
            },
            new VocabularyItem
            {
                Word = "leisure",
                Pronunciation = "/ˈliːʒər/",
                Meaning = "Thời gian rảnh",
                Example = "Reading is my favorite leisure activity.",
                Level = VocabularyLevel.Intermediate,
                Topic = "daily",
                Learned = false,
                UserId = null
            },
            new VocabularyItem
            {
                Word = "neighborhood",
                Pronunciation = "/ˈneɪbərˌhʊd/",
                Meaning = "Khu phố",
                Example = "Our neighborhood has many parks and cafes.",
                Level = VocabularyLevel.Beginner,
                Topic = "daily",
                Learned = false,
                UserId = null
            },
            new VocabularyItem
            {
                Word = "schedule",
                Pronunciation = "/ˈskedʒuːl/",
                Meaning = "Lịch trình",
                Example = "My schedule is very busy this week.",
                Level = VocabularyLevel.Beginner,
                Topic = "daily",
                Learned = false,
                UserId = null
            },
            new VocabularyItem
            {
                Word = "budget",
                Pronunciation = "/ˈbʌdʒɪt/",
                Meaning = "Ngân sách cá nhân",
                Example = "I need to create a monthly budget to manage my expenses.",
                Level = VocabularyLevel.Intermediate,
                Topic = "daily",
                Learned = false,
                UserId = null
            },

            // Travel vocabulary (10 words)
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
                Word = "destination",
                Pronunciation = "/ˌdestɪˈneɪʃən/",
                Meaning = "Điểm đến",
                Example = "Paris is a popular tourist destination.",
                Level = VocabularyLevel.Intermediate,
                Topic = "travel",
                Learned = true,
                UserId = null
            },
            new VocabularyItem
            {
                Word = "accommodation",
                Pronunciation = "/əˌkɑːməˈdeɪʃən/",
                Meaning = "Chỗ ở",
                Example = "We need to book accommodation for our vacation.",
                Level = VocabularyLevel.Advanced,
                Topic = "travel",
                Learned = false,
                UserId = null
            },
            new VocabularyItem
            {
                Word = "luggage",
                Pronunciation = "/ˈlʌɡɪdʒ/",
                Meaning = "Hành lý",
                Example = "Please keep your luggage with you at all times.",
                Level = VocabularyLevel.Beginner,
                Topic = "travel",
                Learned = false,
                UserId = null
            },
            new VocabularyItem
            {
                Word = "passport",
                Pronunciation = "/ˈpæspɔːrt/",
                Meaning = "Hộ chiếu",
                Example = "Don't forget to bring your passport for international travel.",
                Level = VocabularyLevel.Beginner,
                Topic = "travel",
                Learned = false,
                UserId = null
            },
            new VocabularyItem
            {
                Word = "reservation",
                Pronunciation = "/ˌrezərˈveɪʃən/",
                Meaning = "Đặt chỗ",
                Example = "I made a reservation at the restaurant for dinner.",
                Level = VocabularyLevel.Intermediate,
                Topic = "travel",
                Learned = false,
                UserId = null
            },
            new VocabularyItem
            {
                Word = "excursion",
                Pronunciation = "/ɪkˈskɜːrʒən/",
                Meaning = "Chuyến tham quan",
                Example = "We went on a day excursion to the nearby mountains.",
                Level = VocabularyLevel.Advanced,
                Topic = "travel",
                Learned = false,
                UserId = null
            },
            new VocabularyItem
            {
                Word = "souvenir",
                Pronunciation = "/ˌsuːvəˈnɪr/",
                Meaning = "Quà lưu niệm",
                Example = "I bought some souvenirs for my friends back home.",
                Level = VocabularyLevel.Intermediate,
                Topic = "travel",
                Learned = false,
                UserId = null
            },
            new VocabularyItem
            {
                Word = "backpacking",
                Pronunciation = "/ˈbækˌpækɪŋ/",
                Meaning = "Du lịch balo",
                Example = "Backpacking through Europe is a popular travel style.",
                Level = VocabularyLevel.Intermediate,
                Topic = "travel",
                Learned = false,
                UserId = null
            },
            new VocabularyItem
            {
                Word = "jetlag",
                Pronunciation = "/ˈdʒetlæɡ/",
                Meaning = "Mệt mỏi do bay xa",
                Example = "I'm still recovering from jetlag after my flight from Asia.",
                Level = VocabularyLevel.Intermediate,
                Topic = "travel",
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
