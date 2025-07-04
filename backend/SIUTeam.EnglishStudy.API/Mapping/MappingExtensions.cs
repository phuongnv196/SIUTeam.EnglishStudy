using Mapster;
using MapsterMapper;
using System.Reflection;

namespace SIUTeam.EnglishStudy.API.Mapping;

/// <summary>
/// Extension methods for mapping configuration
/// </summary>
public static class MappingExtensions
{
    /// <summary>
    /// Configure Mapster mapping profiles
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddMappingProfiles(this IServiceCollection services)
    {
        // Get the current assembly to scan for mapping profiles
        var config = TypeAdapterConfig.GlobalSettings;
        
        // Scan and register all IRegister implementations in the current assembly
        config.Scan(Assembly.GetExecutingAssembly());

        // Register the mapper
        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();

        return services;
    }
}
