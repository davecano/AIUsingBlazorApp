using Microsoft.SemanticKernel.ChatCompletion;

namespace AIForEverything.Services;

public class MockAIChatService : BaseChatService
{
    private readonly Random _random = new();

    public MockAIChatService(ILogger<MockAIChatService> logger) : base(logger)
    {
    }
    protected override async IAsyncEnumerable<string> GenerateResponseStreamAsync(string userMessage)
    {
        // 随机选择一个响应
        string response = MockResponses.Responses[_random.Next(MockResponses.Responses.Count)];
        
        // 模拟流式响应，将响应按字符逐个返回
        foreach (char c in response)
        {
            yield return c.ToString();
            // 添加一个小延迟，使其看起来更真实
            await Task.Delay(50);
        }
    }
} 