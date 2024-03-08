using Microsoft.AspNetCore.SignalR;
using Sovereignty.Models.SignalR;

namespace Sovereignty.Realm.Hubs;

public class UnturnedHub : Hub<IUnturnedClient>, IUnturnedHub
{
    private readonly ILogger<UnturnedHub> _logger;

    public UnturnedHub(ILogger<UnturnedHub> logger)
    {
        _logger = logger;
    }
    
    public Task SendMessage(string message)
    {
        _logger.LogInformation("Message from client: " + message);
        return Task.CompletedTask;
    }

    public Task<string> ReceiveMessage()
    {
        return Task.FromResult("Hello from the hub");
    }
}