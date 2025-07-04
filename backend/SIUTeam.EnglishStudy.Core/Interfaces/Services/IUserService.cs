using SIUTeam.EnglishStudy.Core.Entities;

namespace SIUTeam.EnglishStudy.Core.Interfaces.Services
{
    public interface IUserService
    {
        Task<Guid> CreateUserAsync(string email, string username, string password, string firstName, string lastName, UserRole role = UserRole.Student);
        Task<bool> UpdateUserAsync(Guid userId, string firstName, string lastName);
        Task<bool> DeactivateUserAsync(Guid userId);
        Task<bool> ActivateUserAsync(Guid userId);
        Task<bool> UpdateUserStatusAsync(Guid userId, bool isActive);
        Task<bool> DeleteUserAsync(Guid userId);
        Task<User?> GetUserByIdAsync(Guid userId);
        Task<User?> GetUserByEmailAsync(string email);
        Task<IEnumerable<User>> GetActiveUsersAsync();
        Task<IEnumerable<User>> GetAllUsersAsync();
    }
}
