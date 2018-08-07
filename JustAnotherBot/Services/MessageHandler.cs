using Discord;
using Discord.WebSocket;

using System.Threading.Tasks;

namespace JABot.Services
{
    public class MessageHandler
    {
        public static async Task sendErrorInChannel(IDMChannel channel, string text)
        {
            await channel.SendMessageAsync("", false, new EmbedBuilder().WithColor(Color.Red).WithDescription(text).Build());
        }

        public static async Task sendErrorInChannel(ISocketMessageChannel channel, string text)
        {
            await channel.SendMessageAsync("", false, new EmbedBuilder().WithColor(Color.Red).WithDescription(text).Build());
        }

        public static async Task sendSuccessInChannel(IDMChannel channel, string text)
        {
            await channel.SendMessageAsync("", false, new EmbedBuilder().WithColor(Color.Green).WithDescription(text).Build());
        }

        public static async Task sendSuccessInChannel(ISocketMessageChannel channel, string text)
        {
            await channel.SendMessageAsync("", false, new EmbedBuilder().WithColor(Color.Green).WithDescription(text).Build());
        }

        public static async Task commandPermissionErrorIn(ISocketMessageChannel channel)
        {
            await sendErrorInChannel(channel, "Du verfügst nicht über die Rechte um diesen Befehl ausführen zu können.");
        }

        public static async Task deleteMessageWithId(ISocketMessageChannel channel, ulong id)
        {
            if (id == 0 || channel == null)
                return;
            var msg = await channel.GetMessageAsync(id);
            if (msg != null)
            {
                await msg.DeleteAsync();
            }
        }
    }
}
