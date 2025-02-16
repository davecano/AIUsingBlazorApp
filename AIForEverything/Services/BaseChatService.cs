using Microsoft.SemanticKernel.ChatCompletion;
using System.Collections.Concurrent;
using AIForEverything.Data;
using AIForEverything.Constants;
using Microsoft.EntityFrameworkCore;

namespace AIForEverything.Services;

public abstract class BaseChatService : IAIChatService
{
    private readonly ConcurrentDictionary<int, ChatHistory> _chatHistoryCache = new();
    protected readonly ILogger _logger;
    protected readonly ApplicationDbContext _context;

    protected BaseChatService(ILogger logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    protected abstract IAsyncEnumerable<string> GenerateResponseStreamAsync(
        string userMessage,
        ChatHistory chatHistory);
 
    public async IAsyncEnumerable<string> GetStreamingChatResponseAsync(string userMessage, int userId)
    {
        var chatHistory = await GetOrCreateChatHistoryAsync(userId);
        
        // 每次请求前添加 system prompt
        var requestHistory = new ChatHistory();
        requestHistory.AddSystemMessage(ChatConstants.SystemPrompt);
        
        // 添加历史对话
        foreach (var message in chatHistory)
        {
            AddMessageToChatHistory(requestHistory, message.Role.ToString(), message.Content);
        }
        
        // 添加当前用户消息
        requestHistory.AddUserMessage(userMessage);

        string fullResponse = "";
        await foreach (var chunk in GenerateResponseStreamAsync(userMessage, requestHistory))
        {
            fullResponse += chunk;
            yield return chunk;
        }

        // 只保存用户消息和助手回复到历史记录
        chatHistory.AddUserMessage(userMessage);
        chatHistory.AddAssistantMessage(fullResponse);
        await SaveChatHistoryAsync(userId, chatHistory);
    }

    public async Task<ChatHistory> GetChatHistoryAsync(int userId)
    {
        return await GetOrCreateChatHistoryAsync(userId);
    }

    private async Task<ChatHistory> GetOrCreateChatHistoryAsync(int userId)
    {
        if (_chatHistoryCache.TryGetValue(userId, out var cachedHistory))
        {
            return cachedHistory;
        }

        var chatHistory = await CreateNewChatHistoryFromDatabase(userId);
        _chatHistoryCache.TryAdd(userId, chatHistory);
        return chatHistory;
    }

    private async Task<ChatHistory> CreateNewChatHistoryFromDatabase(int userId)
    {
        var dbHistory = await _context.ChatHistories
            .Where(ch => ch.UserId == userId)
            .OrderBy(ch => ch.CreatedAt)
            .ToListAsync();

        var chatHistory = new ChatHistory();
        foreach (var message in dbHistory)
        {
            AddMessageToChatHistory(chatHistory, message.Role, message.Content);
        }

        return chatHistory;
    }

    private void AddMessageToChatHistory(
        ChatHistory chatHistory, 
        string role, 
        string content)
    {
        switch (role.ToLower())
        {
            case "user":
                chatHistory.AddUserMessage(content);
                break;
            case "assistant":
                chatHistory.AddAssistantMessage(content);
                break;
        }
    }

    private async Task SaveChatHistoryAsync(
        int userId,
        ChatHistory chatHistory)
    {
        try
        {
            var messages = chatHistory.ToList();
            var lastMessages = messages.Skip(Math.Max(0, messages.Count - 2));

            foreach (var message in lastMessages)
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
            _chatHistoryCache.AddOrUpdate(userId, chatHistory, (_, _) => chatHistory);
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
            await DeleteChatHistoryFromDatabase(userId);
            _chatHistoryCache.TryRemove(userId, out _);
            _chatHistoryCache.TryAdd(userId, new ChatHistory());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing chat history for user {UserId}", userId);
            throw;
        }
    }

    private async Task DeleteChatHistoryFromDatabase(int userId)
    {
        var messages = await _context.ChatHistories
            .Where(ch => ch.UserId == userId)
            .ToListAsync();

        _context.ChatHistories.RemoveRange(messages);
        await _context.SaveChangesAsync();
    }
}