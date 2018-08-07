using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;

using Discord;
using Discord.Commands;
using Discord.Rest;

using JABot.Attributes;

using static JABot.Enums.CustomRoles;

namespace JABot.Modules
{
    [Name("Allgemein")]
    [Summary("Allgemeine Befehle unterschiedlichster Art.")]
    public class GeneralModule : ModuleBase<SocketCommandContext>
    {
        [Command("del"), Summary("Löscht die letzten x Nachrichten. Zahlen zwischen 1 und 99 möglich.")]
        [RequireRole(CustomRole.Moderator)]
        public async Task DelAsync([Remainder]int num)
        {
            if (Context.IsPrivate)
            {
                var embed = new EmbedBuilder()
                    .WithColor(Color.Red)
                    .WithDescription("Privat gesendete Nachrichten können nicht gelöscht werden.");
                await Context.Channel.SendMessageAsync("", false, embed.Build());
                return;
            }

            var messages = await Context.Channel.GetMessagesAsync(num + 1).Flatten();
            await Context.Channel.DeleteMessagesAsync(messages);
        }
    }
}
