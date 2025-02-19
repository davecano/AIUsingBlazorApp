using Microsoft.SemanticKernel.ChatCompletion;

namespace AIForEverything.Services;

public interface IChatHistoryService
{
    Task<ChatHistory> LoadChatHistoryAsync(int userId, Guid conversationId);
    Task SaveChatHistoryAsync(int userId, Guid conversationId, ChatHistory chatHistory);
    Task<List<ConversationInfo>> GetConversationsAsync(int userId);
    Task DeleteConversationAsync(int userId, Guid conversationId);
    Task UpdateConversationTitleAsync(int userId, Guid conversationId, string title);
}
