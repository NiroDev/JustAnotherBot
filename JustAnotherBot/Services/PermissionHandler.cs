using System.Collections.Generic;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using System.Threading.Tasks;

namespace JABot.Services
{
    public class PermissionHandler
    {
        public static bool checkForPermission(SocketGuildUser user, List<string> acceptedRoles)
        {
            if (user.GuildPermissions.Has(GuildPermission.Administrator))
                return true;

            foreach (var role in user.Roles)
            {
                if (acceptedRoles.Contains(role.Name))
                {
                    return true;
                }
            }
            return false;
        }
        static async Task<bool> acceptForChannels(ISocketMessageChannel channel, List<string> acceptedChannels)
        {
            if( !acceptedChannels.Contains(channel.Name) )
            {
                await MessageHandler.commandPermissionErrorIn(channel);
                return false;
            }
            return true;
        }
        static async Task<bool> declineForChannels(ISocketMessageChannel channel, List<string> declinedChannels)
        {
            if (declinedChannels.Contains(channel.Name))
            {
                await MessageHandler.commandPermissionErrorIn(channel);
                return false;
            }
            return true;
        }
    }
}
