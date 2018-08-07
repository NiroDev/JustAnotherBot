using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static JABot.Enums.CustomRoles;
using JABot.Services;
using Discord.Rest;
using Discord.WebSocket;

namespace JABot.Attributes
{
    public class RequireRole : PreconditionAttribute
    {
        private CustomRole role;
        public RequireRole(CustomRole role = CustomRole.User)
        {
            this.role = role;
        }

        public async override Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IServiceProvider map)
        {
            if (!(context is SocketCommandContext))
                return PreconditionResult.FromError("null");
            SocketCommandContext con = context as SocketCommandContext;
            SocketGuildUser user = null;
            if( con.IsPrivate )
            {
                bool found = false;
                foreach( var guild in Global.client.Guilds)
                {
                    foreach (var guildUser in guild.Users)
                    {
                        if (guildUser.Id == context.User.Id)
                        {
                            user = guildUser;
                            found = true;
                            break;
                        }
                    }
                    if (found)
                        break;
                }
            }
            else
            {
                user = con.User as SocketGuildUser;
            }
            if (PermissionHandler.checkForPermission(user, new List<string> { ToFriendlyString(role) }) )
                return PreconditionResult.FromSuccess();
            return PreconditionResult.FromError($"Du benötigst mindestens die **{ToFriendlyString(role)}** Rolle zum Ausführen dieses Befehls.");
        }
    }
}
