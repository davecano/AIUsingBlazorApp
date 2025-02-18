using Microsoft.SemanticKernel.ChatCompletion;

namespace AIForEverything.Services;

public interface IAIChatService
{
    IAsyncEnumerable<string> GetStreamingChatResponseAsync(ChatHistory chatHistory);
}
