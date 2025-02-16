using AIForEverything.Components;
using AIForEverything.Services;
using AIForEverything.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// 配置是否使用模拟服务
var useMockService = builder.Configuration.GetValue<bool>("UseMockService", true);

// Configure OpenAI Settings
builder.Services.Configure<OpenAISettings>(
    builder.Configuration.GetSection("OpenAISettings"));

// 根据配置注册相应的服务实现
if (useMockService)
{
    builder.Services.AddSingleton<IAIChatService, MockAIChatService>();
}
else
{
    builder.Services.AddSingleton<IAIChatService, AIChatService>();
}

// Configure logging
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.SetMinimumLevel(LogLevel.Debug);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
