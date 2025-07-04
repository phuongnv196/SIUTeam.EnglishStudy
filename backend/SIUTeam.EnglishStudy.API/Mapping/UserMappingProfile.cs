using Mapster;
using SIUTeam.EnglishStudy.API.Models.Users;
using SIUTeam.EnglishStudy.Core.DTOs;
using SIUTeam.EnglishStudy.Core.Entities;

namespace SIUTeam.EnglishStudy.API.Mapping;

/// <summary>
/// Mapping profile for user models
/// </summary>
public class UserMappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // User Entity to UserDto mapping
        config.NewConfig<User, UserDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Email, src => src.Email)
            .Map(dest => dest.Username, src => src.Username)
            .Map(dest => dest.FirstName, src => src.FirstName)
            .Map(dest => dest.LastName, src => src.LastName)
            .Map(dest => dest.Avatar, src => src.Avatar)
            .Map(dest => dest.Role, src => src.Role)
            .Map(dest => dest.IsActive, src => src.IsActive)
            .Map(dest => dest.CreatedAt, src => src.CreatedAt);

        // Collection mapping for User list to UserDto list
        config.NewConfig<IEnumerable<User>, IEnumerable<UserDto>>();
    }
}
