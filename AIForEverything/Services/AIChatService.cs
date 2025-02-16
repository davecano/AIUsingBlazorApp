using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;
using System.Linq;
using AIForEverything.Data;
using AIForEverything.Models;

namespace AIForEverything.Services;

public class AIChatService : BaseChatService
{
    private readonly Kernel _kernel;
    private readonly OpenAISettings _settings;

    public AIChatService(
        ILogger<AIChatService> logger, 
        IOptions<OpenAISettings> settings,
        ApplicationDbContext context) : base(logger, context)
    {
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

    protected override async IAsyncEnumerable<string> GenerateResponseStreamAsync(
        string userMessage, 
        Microsoft.SemanticKernel.ChatCompletion.ChatHistory chatHistory)
    {
        var chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();
        var response = chatCompletionService.GetStreamingChatMessageContentsAsync(
            chatHistory,
            executionSettings: new OpenAIPromptExecutionSettings 
            { 
                Temperature = 0.7f,
                TopP = 0.95f
            });

        await foreach (var content in response)
        {
            yield return content.Content;
        }
    }
} 