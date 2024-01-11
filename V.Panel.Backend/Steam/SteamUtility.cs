using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;
using Steamworks;
using V.Panel.Models.Steam;

namespace V.Panel.Backend.Steam;

public class SteamUtility
{
    private readonly IConfiguration _config;
    private readonly ILogger<SteamUtility> _logger;
    private readonly HttpClient _client;
    private readonly IMemoryCache _cache;

    public SteamUtility(IConfiguration config, ILogger<SteamUtility> logger, HttpClient client, IMemoryCache cache)
    {
        _config = config;
        _logger = logger;
        _client = client;
        _cache = cache;
    }

    public async Task<SteamProfile> GetSteamUserAsync(CSteamID steamID)
    {
        SteamProfile profile;
        if (_cache.TryGetValue(steamID, out profile)) return profile;
        
        var response = await _client.GetAsync($"https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v2/?key={_config.GetValue<string>("V.Panel:SteamAPIKey")}&steamids={steamID}");

        if (!response.IsSuccessStatusCode) return SteamProfile.Null;
        
        var json = await response.Content.ReadAsStringAsync();

        var players = JObject.Parse(json)["response"]["players"];
        
        if (!players.HasValues) return SteamProfile.Null;

        profile = players.First().ToObject<SteamProfile>();
        
        _cache.Set(steamID, profile, new MemoryCacheEntryOptions()
        {
            SlidingExpiration = TimeSpan.FromMinutes(10)
        });
        
        return profile;
    }
}