using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace AIForEverything.Services;

public interface IAuthStateService
{
    Task<bool> IsAuthenticatedAsync();
    Task<int?> GetUserIdAsync();
    Task SetAuthenticatedAsync(int userId);
    Task ClearAuthenticationAsync();
}

public class AuthStateService : IAuthStateService
{
    private readonly ProtectedLocalStorage _localStorage;

    public AuthStateService(ProtectedLocalStorage localStorage)
    {
        _localStorage = localStorage;
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var result = await _localStorage.GetAsync<int>("userId");
        return result.Success;
    }

    public async Task<int?> GetUserIdAsync()
    {
        var result = await _localStorage.GetAsync<int>("userId");
        return result.Success ? result.Value : null;
    }

    public async Task SetAuthenticatedAsync(int userId)
    {
        await _localStorage.SetAsync("userId", userId);
    }

    public async Task ClearAuthenticationAsync()
    {
        await _localStorage.DeleteAsync("userId");
    }
} 