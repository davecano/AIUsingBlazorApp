using Microsoft.SemanticKernel.ChatCompletion;

namespace AIForEverything.Services;

public abstract class BaseChatService : IAIChatService
{
    protected readonly ChatHistory _chatHistory;
    protected readonly ILogger _logger;

    protected BaseChatService(ILogger logger)
    {
        _logger = logger;
        _chatHistory = new ChatHistory();
        _chatHistory.AddSystemMessage("You are a helpful AI assistant. Respond in a friendly and professional manner.");
    }

    public async IAsyncEnumerable<string> GetStreamingChatResponseAsync(string userMessage)
    {
        // 添加用户消息
        _chatHistory.AddUserMessage(userMessage);
        
        string fullResponse = "";
        
        // 获取AI响应的流
        await foreach (var chunk in GenerateResponseStreamAsync(userMessage))
        {
            fullResponse += chunk;
            yield return chunk;
        }
        
        // 添加AI响应到历史记录
        _chatHistory.AddAssistantMessage(fullResponse);
    }

    // 派生类只需要实现这个方法，专注于响应的生成
    protected abstract IAsyncEnumerable<string> GenerateResponseStreamAsync(string userMessage);

    public ChatHistory GetChatHistory()
    {
        return _chatHistory;
    }
} 