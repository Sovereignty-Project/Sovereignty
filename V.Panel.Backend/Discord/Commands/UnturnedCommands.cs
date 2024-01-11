using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Steamworks;
using V.Panel.Backend.Steam;

namespace V.Panel.Backend.Discord.Commands;

[Group("unturned", "Unturned Commands")]
public class UnturnedCommands  : InteractionModuleBase<SocketInteractionContext>
{
    [RequireRole("Developer")]
    public class AdminCommands : InteractionModuleBase<SocketInteractionContext>
    {
        
        private readonly SteamUtility _steamUtility;
        private readonly ILogger<AdminCommands> _logger;
        
        public AdminCommands(ILogger<AdminCommands> logger, SteamUtility steamUtility)
        {
            _logger = logger;
            _steamUtility = steamUtility;
        }
        
        [SlashCommand("information", "Investigate a player")]
        public async Task InformationCommand(string name = "", CSteamID steamID = default)
        {
            if (steamID == default && string.IsNullOrEmpty(name))
            {
                await RespondAsync("You must provide a name or steamID");
                return;
            }

            if (steamID == default) steamID = await ConvertNameToSteamID(name);

            var selectMenuBuilder = new SelectMenuBuilder()
                .WithCustomId("unturned_information_menu:" + steamID)
                .WithPlaceholder("Select a page")
                .AddOption("General", "1", "General information about the player", Emoji.Parse(":information_source:"))
                .AddOption("Combat", "2", "PVP information", Emoji.Parse(":crossed_swords:"))
                .AddOption("Punishments", "3", "Punishment history", Emoji.Parse(":page_with_curl:"))
                .WithType(ComponentType.SelectMenu)
                .WithMaxValues(1)
                .WithMinValues(1);

            var componentsBuilder = new ComponentBuilder()
                .WithSelectMenu(selectMenuBuilder);
            
            var page = await GetInformationPage(steamID, 1);
            
            await RespondAsync(embed: page.Build(), components: componentsBuilder.Build());

            Task<CSteamID> ConvertNameToSteamID(string name)
            {
                return Task.FromResult(CSteamID.Nil);
            }
        }
        
        [ComponentInteraction("unturned_information_menu:*", ignoreGroupNames: true)]
        public async Task InformationSelectMenu(CSteamID steamID, int[] selected)
        {
            // Only one option can be selected
            var pageNum = selected[0];
            
            KeyValuePair<CSteamID, int> key = new(steamID, pageNum);
            var page = await GetInformationPage(steamID, pageNum);

            await ((SocketMessageComponent)Context.Interaction).UpdateAsync(properties =>
            {
                properties.Embed = page.Build();
            });
        }
        
        private async Task<EmbedBuilder> GetInformationPage(CSteamID steamID, int pageNum)
        {
            KeyValuePair<CSteamID, int> key = new(steamID, pageNum);
            var page = pageNum switch
                {
                    1 => await GetGeneralPage(steamID),
                    2 => await GetCombatPage(steamID),
                    3 => await GetPunishmentsPage(steamID),
                    _ => await GetGeneralPage(steamID)
                };
            
            return page;
        }
        
        private async Task<EmbedBuilder> GetGeneralPage(CSteamID steamID)
        {
            var profile = await _steamUtility.GetSteamUserAsync(steamID);
            
            _logger.LogInformation($"Profile: {profile.Name} {profile.MemberSince.HasValue}");
            
            var author = new EmbedAuthorBuilder()
                .WithName(profile.Name)
                .WithIconUrl(profile.Avatar)
                .WithUrl($"https://steamcommunity.com/profiles/{steamID}/");
            
            var embed = new EmbedBuilder()
                .WithAuthor(author)
                .WithTitle("Player Information")
                .AddField(field =>
                {
                    field.WithName("Steam");
                    field.WithValue("""
                                    >>> Steam Name
                                    Steam 64 ID
                                    Profile Visibility
                                    Date Created
                                    """);
                    field.WithIsInline(true);
                })
                .AddField(field =>
                {
                    field.WithName(":wavy_dash:");
                    field.WithValue($"""
                                    :wavy_dash:[{profile.Name}](https://steamcommunity.com/profiles/{steamID}/)
                                    :wavy_dash:{steamID}
                                    :wavy_dash:{(profile.VisibilityState ? "Public" : "Private")}
                                    :wavy_dash:{(profile.MemberSince.HasValue ? $"<t:{profile.MemberSince.Value}:D>" : "Unknown")}
                                    """);
                    field.WithIsInline(true);
                })
                .AddField(field =>
                {
                    field.WithName(":wavy_dash:");
                    field.WithValue(":wavy_dash:");
                    field.WithIsInline(true);
                })
                .WithThumbnailUrl(profile.Avatar);

            return embed;
        }

        private async Task<EmbedBuilder> GetCombatPage(CSteamID steamID)
        {
            throw new NotImplementedException();
        }
        
        private async Task<EmbedBuilder> GetPunishmentsPage(CSteamID steamID)
        {
            throw new NotImplementedException();
        }
    }
}