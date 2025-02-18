namespace AIForEverything.Services;

public interface IAuthStateService
{
    Task<bool> IsAuthenticatedAsync();
    Task<int?> GetUserIdAsync();
    Task SetAuthenticatedAsync(int userId);
    Task ClearAuthenticationAsync();
}
