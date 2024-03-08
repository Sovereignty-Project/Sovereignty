using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Sovereignty.Realm.Steam;
using Steamworks;

namespace Sovereignty.Realm.Discord.Commands;

[Group("unturned", "Unturned Commands")]
public class UnturnedCommands  : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("modal", "Simple test modal")]
    public async Task ModalCommand()
    {
        var modal = new ModalBuilder()
            .WithTitle("Test Modal")
            .WithCustomId("test_modal")
            .AddTextInput(new TextInputBuilder()
                .WithLabel("Test input")
                .WithPlaceholder("Test placeholder")
                .WithCustomId("test_input")
            )
            .AddComponents([
                new SelectMenuBuilder()
                    .AddOption("HI", "HI")
                    .WithPlaceholder("Test select")
                    .WithCustomId("test_select")
                    .Build()
            ], 0);

        await Context.Interaction.RespondWithModalAsync(modal.Build());
    }
    
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
            var code = """
                       <svg viewBox="114.48 132.722 277.233 245.51" xmlns="http://www.w3.org/2000/svg" class="heatmap svelte-kj8ea4">
                           <rect x="115" y="192.787" width="92.5" height="46.466" style="fill: rgb(216, 216, 216);"></rect>
                           <rect x="298" y="192.829" width="92.5" height="46.506" style="fill: rgb(216, 216, 216);"></rect>
                           <rect x="206.7" y="192.847" width="92.571" height="91.52" style="fill: rgb(216, 216, 216);"></rect>
                           <rect x="254.049" y="284" width="45.486" height="94" style="fill: rgb(216, 216, 216);"></rect>
                           <rect x="206.7" y="284" width="47.524" height="94" style="fill: rgb(216, 216, 216);"></rect>
                           <rect x="225.591" y="132.722" width="55.421" height="55.584" style="fill: rgb(216, 216, 216);"></rect>
                           <rect x="238.89" y="188.142" width="29.038" height="5.137" style="fill: rgb(216, 216, 216);"></rect>
                       </svg>
                       """;
            
            var profile = await _steamUtility.GetSteamUserAsync(steamID);
            
            _logger.LogInformation($"Profile: {profile.Name} {profile.MemberSince.HasValue}");
            
            var author = new EmbedAuthorBuilder()
                .WithName(profile.Name)
                .WithIconUrl(profile.Avatar)
                .WithUrl($"https://steamcommunity.com/profiles/{steamID}/");

            var embed = new EmbedBuilder()
                .WithAuthor(author)
                .WithTitle("Player Information")
                .WithImageUrl($"attachment://{Path.GetFileName("./heatmap.jpg")}");
            
            return embed;
        }
        
        private async Task<EmbedBuilder> GetPunishmentsPage(CSteamID steamID)
        {
            throw new NotImplementedException();
        }
    }
}