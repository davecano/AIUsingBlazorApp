using Microsoft.SemanticKernel.ChatCompletion;

namespace AIForEverything.Services;

public interface IAIChatService
{
    Task<ChatHistory> GetChatHistoryAsync(int userId);
    IAsyncEnumerable<string> GetStreamingChatResponseAsync(string message, int userId);
    Task ClearChatHistoryAsync(int userId);
}
