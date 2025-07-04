namespace SIUTeam.EnglishStudy.Core.Interfaces.Services
{
    public interface IAuthenticationService
    {
        Task<string> LoginAsync(string email, string password);
        Task<bool> RegisterAsync(string email, string username, string password, string firstName, string lastName);
        Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword);
        Task<bool> ResetPasswordAsync(string email);
        Task LogoutAsync(string token);
        Task<bool> ValidateTokenAsync(string token);
    }
}
