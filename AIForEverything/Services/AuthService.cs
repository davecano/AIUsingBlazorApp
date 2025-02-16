using AIForEverything.Data;
using AIForEverything.Models;
using Microsoft.EntityFrameworkCore;

namespace AIForEverything.Services;

public interface IAuthService
{
    Task<(bool success, string message)> RegisterAsync(string username, string password);
    Task<(bool success, User? user, string message)> LoginAsync(string username, string password);
    Task<User?> GetUserByIdAsync(int userId);
}

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;

    public AuthService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<(bool success, string message)> RegisterAsync(string username, string password)
    {
        if (await _context.Users.AnyAsync(u => u.Username == username))
        {
            return (false, "Username already exists");
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
        var user = new User
        {
            Username = username,
            PasswordHash = passwordHash
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return (true, "Registration successful");
    }

    public async Task<(bool success, User? user, string message)> LoginAsync(string username, string password)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username);

        if (user == null)
        {
            return (false, null, "Invalid username or password");
        }

        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            return (false, null, "Invalid username or password");
        }

        return (true, user, "Login successful");
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await _context.Users
            .Include(u => u.ChatHistories)
            .FirstOrDefaultAsync(u => u.Id == userId);
    }
} 