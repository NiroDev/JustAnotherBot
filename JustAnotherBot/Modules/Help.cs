using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

using JABot.Models.Config;

namespace BWBot.Modules
{
    public class HelpModule : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService m_Commands;
        private readonly IServiceProvider m_Provider;

        public HelpModule(CommandService commands, IServiceProvider provider)
        {
            m_Commands = commands;
            m_Provider = provider;
        }

        private string commandToString(CommandInfo command)
        {
            string value = "**" + Config.Instance.data().prefix;
            if (command.Module.Aliases.First().Length != 0)
                value += command.Module.Aliases.First() + ' ';
            value += command.Name + "**";
            if (command.Aliases.Count > 1)
            {
                value += " ( ";
                foreach (var alias in command.Aliases)
                    value += Config.Instance.data().prefix + alias + ' ';
                value += ")";
            }
            if (command.Parameters.Count > 0)
            {
                value += " *[Syntax: " + Config.Instance.data().prefix + command.Name;
                foreach (var parameter in command.Parameters)
                {
                    value += " <" + parameter + '>';
                }
                value += "]*";
            }
            return value + "\n" + command.Summary + "\n\n";
        }

        [Command("help", RunMode = RunMode.Async)]
        [Alias("help", "h")]
        [Summary("Listet alle Module mit Name und Beschreibung.")]
        public async Task HelpAsync()
        {
            // Get all the modules.
            var modules = m_Commands.Modules.Where(x => !string.IsNullOrWhiteSpace(x.Summary));

            // Create an embed builder.
            var emb = new EmbedBuilder();

            // For each module...
            foreach (var module in modules)
            {
                string value = "";
                foreach (var command in module.Commands)
                {
                    var result = await command.CheckPreconditionsAsync(Context, m_Provider);
                    if (result.IsSuccess)
                    {
                        value += commandToString(command);
                    }
                }
                if (value.Length != 0)
                    emb.AddField(module.Name, "" + module.Summary + "\n\n" + value + ".");
            }

            var channel = await Context.Message.Author.GetOrCreateDMChannelAsync();
            if (emb.Fields.Count <= 0)
                await channel.SendMessageAsync("Modulinformationen konnten nicht geladen werden. Versuchen Sie es später erneut.");
            else
                await channel.SendMessageAsync("", false, emb);
            await channel.CloseAsync();
        }

        [Command("help", RunMode = RunMode.Async)]
        [Alias("help", "h")]
        [Summary("Listet alle Befehle für das jeweilige Modul.")]
        public async Task HelpAsync(string moduleName)
        {
            // Get the module in question.
            var module = m_Commands.Modules.FirstOrDefault(x => x.Name.ToLower() == moduleName.ToLower());

            // If null, we chose a bad module.
            if (module == null)
            {
                await ReplyAsync($"Das Modul `{moduleName}` existiert nicht. Sind Sie sicher, dass es richtig geschrieben wurde?");
                await HelpAsync(); // Show the list of modules again.
                return;
            }

            // Find all it's commands.
            var commands = module.Commands.Where(x => !string.IsNullOrWhiteSpace(x.Summary));

            // If none of them have summaries or don't exist, return.
            if (!commands.Any())
            {
                await ReplyAsync($"Das Modul `{module.Name}` hat keine verfügbaren Kommentare.");
                return;
            }

            // Create an embed builder.
            var emb = new EmbedBuilder()
                .WithTitle(module.Name);

            // For each command...
            string value = "";
            foreach (var command in commands)
            {
                var result = await command.CheckPreconditionsAsync(Context, m_Provider);
                if (result.IsSuccess)
                {
                    value += commandToString(command);
                }
            }
            emb.WithDescription(value);

            var channel = await Context.Message.Author.GetOrCreateDMChannelAsync();
            if (value.Length == 0) // Added error checking in case we don't have summary tags yet.
                await channel.SendMessageAsync("Informationen zum Befehl nicht gefunden. Versuchen Sie es später erneut.");
            else
                await channel.SendMessageAsync("", false, emb);
            await channel.CloseAsync();
        }

    }
}