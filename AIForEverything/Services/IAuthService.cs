using AIForEverything.Models;

namespace AIForEverything.Services;

public interface IAuthService
{
    Task<(bool success, string message)> RegisterAsync(string username, string password);
    Task<(bool success, User? user, string message)> LoginAsync(string username, string password);
    Task<User?> GetUserByIdAsync(int userId);
}
