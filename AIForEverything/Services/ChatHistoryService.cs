using Microsoft.SemanticKernel.ChatCompletion;
using System.Collections.Concurrent;
using AIForEverything.Data;
using Microsoft.EntityFrameworkCore;
using AIForEverything.Constants;

namespace AIForEverything.Services;

public interface IChatHistoryService
{
    Task<ChatHistory> LoadChatHistoryAsync(int userId);
    Task SaveChatHistoryAsync(int userId, ChatHistory chatHistory);
    Task ClearChatHistoryAsync(int userId);
}

public class ChatHistoryService : IChatHistoryService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ChatHistoryService> _logger;

    public ChatHistoryService(ApplicationDbContext context, ILogger<ChatHistoryService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ChatHistory> LoadChatHistoryAsync(int userId)
    {
        var dbHistory = await _context.ChatHistories
            .Where(ch => ch.UserId == userId)
            .OrderBy(ch => ch.CreatedAt)
            .ToListAsync();

        var chatHistory = new ChatHistory();
        
        // Add system message first
        chatHistory.AddSystemMessage(ChatConstants.SystemPrompt);

        // Then add user chat history
        foreach (var message in dbHistory)
        {
            switch (message.Role.ToLower())
            {
                case "user":
                    chatHistory.AddUserMessage(message.Content);
                    break;
                case "assistant":
                    chatHistory.AddAssistantMessage(message.Content);
                    break;
            }
        }

        return chatHistory;
    }

    public async Task SaveChatHistoryAsync(int userId, ChatHistory chatHistory)
    {
        try
        {
            // First, remove all existing messages for this user
            var existingMessages = await _context.ChatHistories
                .Where(ch => ch.UserId == userId)
                .ToListAsync();
            _context.ChatHistories.RemoveRange(existingMessages);

            // Then add all current messages, skipping the system message
            foreach (var message in chatHistory.Where(m => m.Role != AuthorRole.System))
            {
                var dbMessage = new Models.ChatHistory
                {
                    UserId = userId,
                    Role = message.Role.ToString().ToLower(),
                    Content = message.Content
                };

                _context.ChatHistories.Add(dbMessage);
            }

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving chat history for user {UserId}", userId);
            throw;
        }
    }

    public async Task ClearChatHistoryAsync(int userId)
    {
        try
        {
            var messages = await _context.ChatHistories
                .Where(ch => ch.UserId == userId)
                .ToListAsync();

            _context.ChatHistories.RemoveRange(messages);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing chat history for user {UserId}", userId);
            throw;
        }
    }
} 