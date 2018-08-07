using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Threading.Tasks;

using JABot.Models.Config;

namespace JABot
{
    class Program
    {
        static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();
        
        private CommandService commands;
        private IServiceProvider services;

        public async Task RunBotAsync()
        {
            Global.client = new DiscordSocketClient();
            commands = new CommandService();

            services = new ServiceCollection()
                .AddSingleton(Global.client)
                .AddSingleton(commands)
                .BuildServiceProvider();

            string botToken = Config.Instance.data().token;

            await Global.client.SetGameAsync(Config.Instance.data().prefix + "help");

            //event subscriptions
            Global.client.Log += Log;
            Global.client.ReactionAdded += OnReactionAdded;
            Global.client.Ready += Ready;

            await RegisterCommandsAsync();

            await Global.client.LoginAsync(Discord.TokenType.Bot, botToken );

            await Global.client.StartAsync();

            await Task.Delay(-1);
        }

        private Task Log(LogMessage arg)
        {
            Console.WriteLine(arg);

            return Task.FromResult(arg);
        }
        private Task Ready()
        {
            return Global.client.SetStatusAsync(UserStatus.AFK);
        }

        public async Task RegisterCommandsAsync()
        {
            Global.client.MessageReceived += HandleCommandAsync;

            await commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        private async Task HandleCommandAsync( SocketMessage arg )
        {
            var message = arg as SocketUserMessage;

            if( message == null || message.Author.IsBot )
                return;

            int argPos = 0;

            if (message.HasStringPrefix(Config.Instance.data().prefix, ref argPos) || message.HasMentionPrefix(Global.client.CurrentUser, ref argPos))
            {
                var context = new SocketCommandContext(Global.client, message);

                if (!context.IsPrivate)
                    await context.Message.DeleteAsync();

                var result = await commands.ExecuteAsync(context, argPos, services);

                if (!result.IsSuccess)
                {
                    EmbedBuilder embed = new EmbedBuilder()
                        .WithColor(Color.Red)
                        .WithDescription(result.ErrorReason);
                    switch (result.Error)
                    {
                        case CommandError.BadArgCount:
                            embed.WithTitle("Falsche Parameter");
                            break;
                        default:
                            embed.WithTitle("Unbekannter Fehler");
                            break;
                    }
                    await context.Channel.SendMessageAsync(string.Empty, false, embed);
                }
            }
        }

        private async Task OnReactionAdded(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (reaction.User.Value.IsBot)
                return;

            // Reaction handling
            if (reaction.MessageId == Global.MessageIdToTrack)
            {
                if (reaction.Emote.Name == "test")
                {
                    await channel.SendMessageAsync(reaction.User.Value.Username + " hat auf ein Test reagiert.");
                }
            }
        }
    }
}
