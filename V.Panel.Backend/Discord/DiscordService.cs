using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Steamworks;
using V.Panel.Backend.Discord.TypeConverters;
using V.Panel.Backend.Discord.TypeReaders;
using IResult = Discord.Interactions.IResult;

namespace V.Panel.Backend.Discord;

public class DiscordService : BackgroundService
{
    private readonly IServiceProvider _provider;
    private readonly ILogger<DiscordService> _logger;
    private readonly IConfiguration _config;
    private readonly DiscordSocketClient _client;
    private readonly InteractionService _interactionService;
    
    public DiscordService(IServiceProvider provider, ILogger<DiscordService> logger, IConfiguration config, DiscordSocketClient client, InteractionService interactionService)
    {
        _provider = provider;
        _logger = logger;
        _config = config;
        _client = client;
        _interactionService = interactionService;
    }
    
    protected override async Task ExecuteAsync(CancellationToken token)
    {
        _client.Ready += OnReady;
        _client.InteractionCreated += OnInteractionCreated;
        _interactionService.SlashCommandExecuted += OnSlashCommandExecuted;

        var secret = _config.GetValue<string>("V.Panel:DiscordToken");
        
        await _client.LoginAsync(TokenType.Bot, secret);
        await _client.StartAsync();
        
        await Task.Delay(Timeout.Infinite, token);

        await _client.StopAsync();
        
        _client.InteractionCreated -= OnInteractionCreated;
        _interactionService.SlashCommandExecuted -= OnSlashCommandExecuted;
        
        async Task OnReady()
        {
            _interactionService.AddTypeConverter<CSteamID>(new CSteamIDConverter());
            _interactionService.AddTypeReader<CSteamID>(new CSteamIDReader());
            
            await _interactionService.AddModulesAsync(Assembly.GetExecutingAssembly(), _provider);
            
            foreach (var command in _interactionService.SlashCommands)
                _logger.LogInformation($"Loaded Command: {command.Name}");

            await _interactionService.RegisterCommandsGloballyAsync();
            
            _client.Ready -= OnReady;
        }
        
        async Task OnInteractionCreated(SocketInteraction interaction)
        {
            var ctx = new SocketInteractionContext(_client, interaction);
            await _interactionService.ExecuteCommandAsync(ctx, _provider);
        }

        Task OnSlashCommandExecuted(SlashCommandInfo info, IInteractionContext context, IResult result)
        {
            return result.IsSuccess ? Task.CompletedTask : context.Interaction.RespondAsync(result.ErrorReason);
        }
            
    }
}