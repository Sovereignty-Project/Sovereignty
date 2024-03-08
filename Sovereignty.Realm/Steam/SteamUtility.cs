using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Steamworks;
using Sovereignty.Models.Steam;

namespace Sovereignty.Realm.Steam;

public class SteamUtility
{
    private readonly IConfiguration _config;
    private readonly ILogger<SteamUtility> _logger;
    private readonly HttpClient _client;
    private readonly IDistributedCache _cache;

    public SteamUtility(IConfiguration config, ILogger<SteamUtility> logger, HttpClient client, IDistributedCache cache)
    {
        _config = config;
        _logger = logger;
        _client = client;
        _cache = cache;
    }

    public async Task<SteamProfile> GetSteamUserAsync(CSteamID steamID)
    {
        var json = await _cache.GetStringAsync(steamID.m_SteamID.ToString());
        
        if (json != null) return JsonConvert.DeserializeObject<SteamProfile>(json)!;
        
        var response = await _client.GetAsync($"https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v2/?key={_config.GetValue<string>("V.Panel:SteamAPIKey")}&steamids={steamID}");

        if (!response.IsSuccessStatusCode) return SteamProfile.Null;
        
        json = await response.Content.ReadAsStringAsync();

        var players = JObject.Parse(json)["response"]?["players"];
        
        if (players is null or { HasValues: false }) return SteamProfile.Null;

        var profile = players.First().ToObject<SteamProfile>();
        
        // await _cache.SetStringAsync(steamID.m_SteamID.ToString(), profile, new MemoryCacheEntryOptions()
        // {
        //     SlidingExpiration = TimeSpan.FromMinutes(10)
        // });

        await _cache.SetStringAsync(steamID.m_SteamID.ToString(), json, new DistributedCacheEntryOptions()
        {
            SlidingExpiration = TimeSpan.FromMinutes(5)
        });
        
        return profile!;
    }
}