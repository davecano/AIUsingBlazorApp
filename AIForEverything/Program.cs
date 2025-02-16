using AIForEverything.Components;
using AIForEverything.Services;
using AIForEverything.Models;
using AIForEverything.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add SQLite and Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Authentication Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAuthStateService, AuthStateService>();

// Configure OpenAI Settings
builder.Services.Configure<OpenAISettings>(
    builder.Configuration.GetSection("OpenAISettings"));

// 配置是否使用模拟服务
var useMockService = builder.Configuration.GetValue<bool>("UseMockService", true);

// 根据配置注册相应的服务实现
if (useMockService)
{
    builder.Services.AddScoped<IAIChatService, MockAIChatService>();
}
else
{
    builder.Services.AddScoped<IAIChatService, AIChatService>();
}

// Configure logging
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.SetMinimumLevel(LogLevel.Debug);
});

var app = builder.Build();

// Ensure database is created and migrations are applied
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.EnsureCreated();
}

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

// Set default route to chat page
app.MapGet("/", context =>
{
    context.Response.Redirect("/chat");
    return Task.CompletedTask;
});

app.Run();
