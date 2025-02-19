namespace AIForEverything.Models;

public class ChatListItem
{
    public Guid Id { get; set; }
    public string Title { get; set; } = "";
    public DateTime LastMessageTime { get; set; }
} 