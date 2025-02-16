using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.Extensions.Logging;

namespace AIUsingBlazorApp.Services;

public class AIService
{
    private readonly Kernel _kernel;
    private readonly IChatCompletionService _chatCompletionService;

    public AIService(IConfiguration configuration)
    {
        var modelId = "deepseek-v3";
        var apiKey = configuration["AI:ApiKey"] ?? throw new ArgumentNullException("AI:ApiKey configuration is missing");
        var endpoint = "https://dashscope.aliyuncs.com/compatible-mode/v1/";

        var kernelBuilder = Kernel.CreateBuilder();
        kernelBuilder.Services.AddLogging(services => services.AddConsole().SetMinimumLevel(LogLevel.Trace));

        kernelBuilder.AddOpenAIChatCompletion(
            modelId: modelId,
            apiKey: apiKey,
            httpClient: new HttpClient
            {
                BaseAddress = new Uri(endpoint)
            }
        );

        _kernel = kernelBuilder.Build();
        _chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();
    }

    public async Task<string> GetChatCompletionAsync(string userMessage, CancellationToken cancellationToken = default)
    {
        var chatHistory = new ChatHistory();
        chatHistory.AddUserMessage(userMessage);

        var result = await _chatCompletionService.GetChatMessageContentAsync(chatHistory, cancellationToken: cancellationToken);
        return result.Content;
    }

    public async Task<string> GetChatCompletionAsync(ChatHistory chatHistory, CancellationToken cancellationToken = default)
    {
        var result = await _chatCompletionService.GetChatMessageContentAsync(chatHistory, cancellationToken: cancellationToken);
        return result.Content;
    }

    public IAsyncEnumerable<string> GetStreamingChatCompletionAsync(
        ChatHistory chatHistory,
        CancellationToken cancellationToken = default)
    {
        return StreamResponse();

        async IAsyncEnumerable<string> StreamResponse()
        {
            var responseStream = _chatCompletionService.GetStreamingChatMessageContentsAsync(
                chatHistory,
                cancellationToken: cancellationToken);

            await foreach (var content in responseStream)
            {
                yield return content.Content;
            }
        }
    }
} 