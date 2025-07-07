using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using SIUTeam.EnglishStudy.Core.Helpers;
using SIUTeam.EnglishStudy.Core.Interfaces;
using SIUTeam.EnglishStudy.Core.Interfaces.Integrations;
using SIUTeam.EnglishStudy.Core.Interfaces.Repositories;
using SIUTeam.EnglishStudy.Core.Interfaces.Services;
using SIUTeam.EnglishStudy.Infrastructure.Data;
using SIUTeam.EnglishStudy.Infrastructure.Data.Configuration;
using SIUTeam.EnglishStudy.Infrastructure.Repositories;
using SIUTeam.EnglishStudy.Infrastructure.Services;

namespace SIUTeam.EnglishStudy.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure MongoDB GUID representation
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
        
        // Configure MongoDB settings
        services.Configure<MongoDbSettings>(configuration.GetSection("MongoDbSettings"));
        
        // Register MongoDB context
        services.AddSingleton<MongoDbContext>();
        
        // Register repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<ILessonRepository, LessonRepository>();
        services.AddScoped<IExerciseRepository, ExerciseRepository>();
        services.AddScoped<IStudySessionRepository, StudySessionRepository>();
        services.AddScoped<IUserProgressRepository, UserProgressRepository>();
        services.AddScoped<IUserAnswerRepository, UserAnswerRepository>();
        services.AddScoped<IVocabularyRepository, VocabularyRepository>();
        
        // Register Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        // Register business services
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ICourseService, CourseService>();
        services.AddScoped<ILessonService, LessonService>();
        services.AddScoped<IStudyService, StudyService>();
        services.AddScoped<IVocabularyService, VocabularyService>();
        services.AddSingleton<CryptoString>();
        
        // Register HTTP client for SpeakingService
        services.AddHttpClient<ISpeakingService, SpeakingService>();
        services.AddHttpClient<ICambridgeService, CambridgeService>();

        return services;
    }
}