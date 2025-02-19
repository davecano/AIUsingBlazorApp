namespace AIForEverything.Services;

public class ConversationInfo
{
    public Guid ConversationId { get; set; }
    public string Title { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}
