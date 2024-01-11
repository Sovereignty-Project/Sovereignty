using Discord;
using Discord.Interactions;
using Steamworks;

namespace V.Panel.Backend.Discord.TypeConverters;

public class CSteamIDConverter : TypeConverter<CSteamID>
{
    public override ApplicationCommandOptionType GetDiscordType() => ApplicationCommandOptionType.String;
    
    public override Task<TypeConverterResult> ReadAsync(IInteractionContext context, IApplicationCommandInteractionDataOption option, IServiceProvider services)
    {
        if (!ulong.TryParse((string)option.Value, out var result))
            return Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.ConvertFailed, $"{option.Value} cannot be converted to a valid 64 Steam ID."));
        var steamID = new CSteamID(result);
        if (!steamID.IsValid())
            return Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.ConvertFailed, $"{option.Value} cannot be converted to a valid 64 Steam ID."));
        return Task.FromResult(TypeConverterResult.FromSuccess(steamID));
    }
    
    public override void Write(ApplicationCommandOptionProperties properties, IParameterInfo parameterInfo)
    {
        properties.MinLength = 16;
        properties.MaxLength = 18;
    }
}