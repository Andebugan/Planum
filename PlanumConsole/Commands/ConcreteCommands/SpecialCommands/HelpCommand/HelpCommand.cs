using Planum.Logger;

namespace Planum.Console.Commands.Special
{
    public class HelpCommand: BaseCommand<HelpCommandSettings>
    {
        public List<ICommand> Commands { get; set; } = new List<ICommand>(); 

        public HelpCommand(List<ICommand> commands, CommandInfo commandInfo, List<BaseOption<HelpCommandSettings>> commandOptions, ILoggerWrapper logger) : base(commandInfo, commandOptions, logger)
        {
            Commands = commands;
        }

        public override List<string> Execute(ref IEnumerator<string> args)
        {
            Logger.Log("Executing help command");
            var lines = new List<string>();

            var commandSettings = new HelpCommandSettings();
            if (!ParseSettings(ref args, ref lines, ref commandSettings))
                return lines;

            IEnumerable<ICommand> displayedCommands = Commands.ToList();
            if (commandSettings.CommandNameLikeString != "")
                displayedCommands = displayedCommands.Where(x => x.CommandInfo.Name.Contains(commandSettings.CommandNameLikeString));
            displayedCommands = displayedCommands.OrderBy(x => x.CommandInfo.Name);

            foreach (var command in displayedCommands)
            {
                lines.Add(ConsoleSpecial.AddStyle(command.CommandInfo.Name, TextStyle.Bold, TextForegroundColor.BrightCyan) +
                        ConsoleSpecial.AddStyle(command.CommandInfo.Usage, foregroundColor: TextForegroundColor.BrightYellow) + " - " + command.CommandInfo.Description);

                IEnumerable<IOption> options = new List<IOption>();
                if (commandSettings.ShowOptions)
                {
                    options = command.CommandOptions;
                    if (commandSettings.CommandOptionLikeString != "")
                        options = options.Where(x => x.OptionInfo.Name.Contains(commandSettings.CommandOptionLikeString));
                    options = options.OrderBy(x => x.OptionInfo.Name);
                }

                foreach (var option in options)
                {
                    lines.Add("    " + 
                            ConsoleSpecial.AddStyle(option.OptionInfo.Name, TextStyle.Bold, TextForegroundColor.BrightYellow) +
                            ConsoleSpecial.AddStyle(option.OptionInfo.Usage, TextStyle.Dim, TextForegroundColor.Yellow) +
                            " - " +
                            option.OptionInfo.Description);
                }
            }

            Logger.Log("Successfully executed help command");
            return lines;
        }
    }
}
