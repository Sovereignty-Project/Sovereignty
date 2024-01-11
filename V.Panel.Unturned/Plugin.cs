using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Cysharp.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using OpenMod.Unturned.Plugins;
using OpenMod.API.Plugins;

[assembly: PluginMetadata("V.Panel.Unturned", DisplayName = "V.Panel.Unturned", Author = "Sun Beam")]

namespace V.Panel.Unturned
{
    public class Plugin : OpenModUnturnedPlugin
    {
        private readonly ILogger<Plugin> _logger;
        private readonly IConfiguration _configuration;
        private readonly IStringLocalizer _stringLocalizer;
        private readonly HubConnection _connection;
        
        private readonly IServiceProvider _serviceProvider;

        public Plugin(
            ILogger<Plugin> logger,
            IConfiguration configuration,
            IStringLocalizer stringLocalizer,
            HubConnection connection,
            IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _logger = logger;
            _configuration = configuration;
            _stringLocalizer = stringLocalizer;
            _connection = connection;
            _serviceProvider = serviceProvider;
        }

        protected override async UniTask OnLoadAsync()
        {
            // await UniTask.SwitchToMainThread(); uncomment if you have to access Unturned or UnityEngine APIs
            // _mLogger.LogInformation("Hello World!");
            _logger.LogWarning($"Loaded V.Panel.Unturned v{Version}.");
            
            await _connection.StartAsync();
            // await UniTask.SwitchToThreadPool(); // you can switch back to a different thread
        }

        protected override async UniTask OnUnloadAsync()
        {
            // await UniTask.SwitchToMainThread(); uncomment if you have to access Unturned or UnityEngine APIs

            await _connection.StopAsync();
             _logger.LogWarning($"Unloaded V.Panel.Unturned v{Version}.");
        }
    }
}
