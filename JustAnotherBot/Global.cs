using Discord.WebSocket;

namespace JABot
{
    internal static class Global
    {
        internal static DiscordSocketClient client { get; set; }
        internal static ulong MessageIdToTrack { get; set; }
    }
}
