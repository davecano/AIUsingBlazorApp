using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace AIForEverything.Services;

public interface IAIChatService
{
    IAsyncEnumerable<string> GetStreamingChatResponseAsync(
        ChatHistory chatHistory,
        OpenAIPromptExecutionSettings? executionSettings = null);
}
