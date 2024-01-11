using Newtonsoft.Json;
// using Newtonsoft.Json.Converters;
using Steamworks;

namespace V.Panel.Models.Steam;

using System;

public class SteamProfile()
{
    [JsonProperty(PropertyName = "steamid")]
    public ulong SteamID { get; set; }
    [JsonProperty(PropertyName = "personaname")]
    public string Name { get; set; }
    [JsonProperty(PropertyName = "avatarmedium")]
    public string Avatar { get; set; }
    [JsonProperty(PropertyName = "communityvisibilitystate")]
    public bool VisibilityState { get; set; }
    [JsonProperty(PropertyName = "profilestate")]
    public bool IsLimitedAccount { get; set; }
    [JsonProperty(PropertyName = "timecreated")]
    public long? MemberSince { get; set; }
    // [JsonProperty(PropertyName = "timecreated")]
    // [JsonConverter(typeof(UnixDateTimeConverter))]
    // public DateTime? MemberSinceDateTime { get; set; }

    public static SteamProfile Null => new()
    {
        SteamID = (ulong)CSteamID.Nil,
        Name = "Unknown",
        Avatar =
            "https://avatars.cloudflare.steamstatic.com/fef49e7fa7e1997310d705b2a6158ff8dc1cdfeb_full.jpg",
        VisibilityState = false,
        IsLimitedAccount = true,
        MemberSince = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
    };
    
    public override bool Equals(object? obj)
    {
        if (obj is not SteamProfile profile) return false;
        return SteamID == profile.SteamID;
    }

    public override int GetHashCode()
    {
        return SteamID.GetHashCode();
    }
}