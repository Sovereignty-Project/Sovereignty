using Discord;
using Discord.Interactions;
using Steamworks;

namespace Sovereignty.Realm.Discord.TypeReaders;

public class CSteamIDReader : TypeReader<CSteamID>
{
    public override Task<TypeConverterResult> ReadAsync(IInteractionContext context, string option, IServiceProvider services)
    {
        if (!ulong.TryParse(option, out var result))
            return Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.ConvertFailed, $"{option} cannot be converted to a valid 64 Steam ID."));
        var steamID = new CSteamID(result);
        if (!steamID.IsValid())
            return Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.ConvertFailed, $"{option} cannot be converted to a valid 64 Steam ID."));
        return Task.FromResult(TypeConverterResult.FromSuccess(steamID));
    }
}