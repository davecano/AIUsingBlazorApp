using Microsoft.SemanticKernel.ChatCompletion;
using System.Collections.Concurrent;
using AIForEverything.Data;
using Microsoft.EntityFrameworkCore;
using AIForEverything.Constants;

namespace AIForEverything.Services;

public class ChatHistoryService : IChatHistoryService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ChatHistoryService> _logger;

    public ChatHistoryService(ApplicationDbContext context, ILogger<ChatHistoryService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ChatHistory> LoadChatHistoryAsync(int userId, Guid conversationId)
    {
        var dbHistory = await _context.ChatHistories
            .Where(ch => ch.UserId == userId && ch.ConversationId == conversationId)
            .OrderBy(ch => ch.CreatedAt)
            .ToListAsync();

        var chatHistory = new ChatHistory();
        chatHistory.AddSystemMessage(ChatConstants.SystemPrompt);

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

    public async Task SaveChatHistoryAsync(int userId, Guid conversationId, ChatHistory chatHistory)
    {
        try
        {
            // 首先删除该会话的所有现有消息
            var existingMessages = await _context.ChatHistories
                .Where(ch => ch.UserId == userId && ch.ConversationId == conversationId)
                .ToListAsync();
            _context.ChatHistories.RemoveRange(existingMessages);

            var title = chatHistory
                .Where(m => m.Role == AuthorRole.User)
                .Select(m => m.Content)
                .FirstOrDefault() ?? "New Chat";

            if (title.Length > 30)
            {
                title = title[..30] + "...";
            }

            // 添加新的消息
            foreach (var message in chatHistory.Where(m => m.Role != AuthorRole.System))
            {
                var dbMessage = new Models.ChatHistory
                {
                    UserId = userId,
                    ConversationId = conversationId,
                    Title = title,
                    Role = message.Role.ToString().ToLower(),
                    Content = message.Content,
                    CreatedAt = DateTime.UtcNow // 确保设置创建时间
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

    public async Task<List<ConversationInfo>> GetConversationsAsync(int userId)
    {
        return await _context.ChatHistories
            .Where(ch => ch.UserId == userId)
            .GroupBy(ch => new { ch.ConversationId, ch.Title })
            .Select(g => new ConversationInfo
            {
                ConversationId = g.Key.ConversationId,
                Title = g.Key.Title,
                CreatedAt = g.Max(ch => ch.CreatedAt)
            })
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task DeleteConversationAsync(int userId, Guid conversationId)
    {
        var messages = await _context.ChatHistories
            .Where(ch => ch.UserId == userId && ch.ConversationId == conversationId)
            .ToListAsync();

        _context.ChatHistories.RemoveRange(messages);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateConversationTitleAsync(int userId, Guid conversationId, string title)
    {
        var messages = await _context.ChatHistories
            .Where(ch => ch.UserId == userId && ch.ConversationId == conversationId)
            .ToListAsync();

        foreach (var message in messages)
        {
            message.Title = title;
        }

        await _context.SaveChangesAsync();
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