using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;
using System.Linq;
using AIForEverything.Data;
using AIForEverything.Models;
using AIForEverything.Constants;
using ChatHistory = Microsoft.SemanticKernel.ChatCompletion.ChatHistory;

namespace AIForEverything.Services;

public class AIChatService : IAIChatService
{
    private readonly Kernel _kernel;
    private readonly OpenAISettings _settings;
    private readonly ILogger<AIChatService> _logger;

    public AIChatService(
        ILogger<AIChatService> logger,
        IOptions<OpenAISettings> settings)
    {
        _logger = logger;
        _settings = settings.Value;
        _kernel = InitializeKernel();
    }

    private Kernel InitializeKernel()
    {
        var kernelBuilder = Kernel.CreateBuilder();
        
        kernelBuilder.AddOpenAIChatCompletion(
            modelId: _settings.ModelId,
            apiKey: _settings.ApiKey,
            httpClient: new HttpClient
            {
                BaseAddress = new Uri(_settings.Endpoint)
            }
        );
        
        return kernelBuilder.Build();
    }

    public async IAsyncEnumerable<string> GetStreamingChatResponseAsync(
        ChatHistory chatHistory,
        OpenAIPromptExecutionSettings? executionSettings = null)
    {
        var chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();

        executionSettings ??= new OpenAIPromptExecutionSettings 
        { 
            Temperature = 0.7f,
            TopP = 0.95f
        };

        var response = chatCompletionService.GetStreamingChatMessageContentsAsync(
            chatHistory,
            executionSettings: executionSettings);

        await foreach (var content in response)
        {
            yield return content.Content;
        }
    }
} 