using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenMod.API.Ioc;
using TypedSignalR.Client;
using V.Panel.Models.SignalR;

namespace V.Panel.Unturned.Clients;

public class UnturnedClient : IUnturnedClient, IHubConnectionObserver, IServiceConfigurator, IDisposable
{
    private bool _disposed;
    
    private readonly ILogger<UnturnedClient> _logger;
    private readonly IUnturnedHub _hub;
    
    private IDisposable _subscription;

    public UnturnedClient(ILogger<UnturnedClient> logger, HubConnection connection, IUnturnedHub hub)
    {
        _logger = logger;
        _logger.LogInformation("INSTANTIATING");
        _hub = hub;
        _subscription = connection.Register<IUnturnedClient>(this);
    }
    
    public Task SendMessage(string message)
    {
        _logger.LogInformation("Message from hub: " + message);
        return Task.CompletedTask;
    }

    public Task<string> ReceiveMessage()
    {
        return Task.FromResult("Hello from the client");
    }

    public Task OnClosed(Exception? exception)
    {
        return Task.CompletedTask;
    }

    public Task OnReconnected(string? connectionId)
    {
        return Task.CompletedTask;
    }

    public Task OnReconnecting(Exception? exception)
    {
        return Task.CompletedTask;
    }
    
    public void Dispose()
    {
        _logger.LogInformation("DISPOSING");
        if (_disposed) return;
        _subscription.Dispose();
        _disposed = true;
    }
    
    public void ConfigureServices(IOpenModServiceConfigurationContext openModStartupContext, IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<HubConnection>(provider =>
        {
            var configuration = provider.GetRequiredService<IConfiguration>();
            var connection = new HubConnectionBuilder()
                .WithUrl(configuration.GetSection("SignalR:HubURL").Get<string>())
                .WithAutomaticReconnect(new [] {TimeSpan.Zero, TimeSpan.Zero, TimeSpan.FromSeconds(10) })
                .Build();

            return connection;
        });
        serviceCollection.AddScoped<IUnturnedHub>(provider =>
        {
            var connection = provider.GetRequiredService<HubConnection>();
            var proxy = connection.CreateHubProxy<IUnturnedHub>();
            return proxy;
        });
        serviceCollection.AddScoped<IUnturnedClient>(provider =>
        {
            var logger = provider.GetRequiredService<ILogger<UnturnedClient>>();
            var connection = provider.GetRequiredService<HubConnection>();
            var hub = provider.GetRequiredService<IUnturnedHub>();
            var client = new UnturnedClient(logger, connection, hub);
            return client;
        });
    }
}