using Microsoft.SemanticKernel.ChatCompletion;

namespace AIForEverything.Services;

public interface IChatHistoryService
{
    Task<ChatHistory> LoadChatHistoryAsync(int userId);
    Task SaveChatHistoryAsync(int userId, ChatHistory chatHistory);
    Task ClearChatHistoryAsync(int userId);
}
