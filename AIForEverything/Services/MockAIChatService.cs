using System.Text.Json;
using Microsoft.SemanticKernel.ChatCompletion;
using AIForEverything.Data;

namespace AIForEverything.Services;

public class MockAIChatService : IAIChatService
{
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<MockAIChatService> _logger;
    private static MockResponsesData? _responses;
    private static readonly Random _random = new();

    public MockAIChatService(
        IWebHostEnvironment environment, 
        ILogger<MockAIChatService> logger)
    {
        _environment = environment;
        _logger = logger;
    }

    public async IAsyncEnumerable<string> GetStreamingChatResponseAsync(
        ChatHistory chatHistory)
    {
        var responses = await GetResponses();
        var response = responses[_random.Next(responses.Count)];
        var words = response.Content.Split(' ');

        foreach (var word in words)
        {
            yield return word + " ";
            await Task.Delay(50); // 模拟打字效果
        }
    }

    private async Task<List<MockResponse>> GetResponses()
    {
        if (_responses != null) return _responses.Responses;

        var path = Path.Combine(_environment.WebRootPath, "data", "mock-responses.json");
        var content = await File.ReadAllTextAsync(path);
        _responses = JsonSerializer.Deserialize<MockResponsesData>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return _responses?.Responses ?? new List<MockResponse>();
    }
} 