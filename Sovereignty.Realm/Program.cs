using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Caching.Distributed;
// using Microsoft.Extensions.Caching.Memory;
using Sovereignty.Realm.Configuration.cs;
using Sovereignty.Realm.Discord;
using Sovereignty.Realm.Hubs;
using Sovereignty.Realm.Steam;

var builder = WebApplication.CreateBuilder(args);

// Configuration
builder.Configuration.AddEnvironmentVariables(prefix: "VPanel:");

// Add services to the container.
builder.Services.AddSignalR();
builder.Services.AddControllers();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.ConfigurationOptions = new StackExchange.Redis.ConfigurationOptions()
    {
        EndPoints =
        {
            builder.Configuration.GetSection(VPanelConfiguration.SectionName).Get<VPanelConfiguration>()?.REDIS_HOST ?? string.Empty
        }
    };
    options.InstanceName = "Sovereignty.Realm";
});
// builder.Services.AddResponseCaching();

// Steam
builder.Services.AddHttpClient();
builder.Services.AddTransient<SteamUtility>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    var logger = provider.GetRequiredService<ILogger<SteamUtility>>();
    var client = provider.GetRequiredService<HttpClient>();
    var cache = provider.GetRequiredService<IDistributedCache>();
    return new SteamUtility(config, logger, client, cache);
});

// Discord Configuration
var discordConfig = new DiscordSocketConfig()
{
    GatewayIntents = GatewayIntents.All,
};

var interactionServiceConfig = new InteractionServiceConfig()
{
    DefaultRunMode = RunMode.Async
};

builder.Services.AddSingleton(discordConfig);
builder.Services.AddSingleton(interactionServiceConfig);

// Discord Services
builder.Services.AddSingleton<DiscordSocketClient>((provider) =>
{
    var config = provider.GetService<DiscordSocketConfig>();
    var logger = provider.GetRequiredService<ILogger<DiscordSocketClient>>();
    var client = config != null ? new DiscordSocketClient(config) : new DiscordSocketClient();
    
    client.Log += Log;

    return client;
    
    Task Log(LogMessage msg)
    {
        logger.LogInformation(msg.ToString());
        return Task.CompletedTask;
    }
});
builder.Services.AddSingleton<InteractionService>((provider) =>
{
    var config = provider.GetService<InteractionServiceConfig>();
    var client = provider.GetRequiredService<DiscordSocketClient>();
    var logger = provider.GetRequiredService<ILogger<InteractionService>>();
    var interactionService = new InteractionService(client, config);
    
    interactionService.Log += Log;
    
    return interactionService;
    
    Task Log(LogMessage msg)
    {
        logger.LogInformation(msg.ToString());
        return Task.CompletedTask;
    }
});
builder.Services.AddHostedService<DiscordService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

// app.UseCors();
// app.UseResponseCaching();

app.MapControllerRoute(
    name: "default",
    pattern: "api/{controller}");

app.MapHub<UnturnedHub>("/signalr/unturned");

app.Run();
