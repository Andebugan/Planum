using Planum.ConsoleUI.CommandProcessor;
using Planum.ConsoleUI.ConsoleCommands.TaskCommands;
using Planum.ConsoleUI.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Planum.ConsoleUI.ConsoleCommands.CommonCommands
{
    public class HelpCommand : BaseCommand
    {

        public class CommandOption : BaseOption<List<string>>
        {
            HelpCommand helpCommand;

            public CommandOption(HelpCommand helpCommand, string name, string description, string usage, List<string> Default, bool optional = true, bool used = false) : base(name, description, usage, Default, optional, used)
            {
                this.helpCommand = helpCommand;
                Used = true;
            }

            public override bool GetValue(ref List<string> args, ref string error)
            {
                bool result = ArgumentParser.Parse(ref value, ref args);
                if (!result)
                    return false;
                foreach (var name in value)
                {
                    if (!helpCommand.commands.Exists(x => x.GetName() == name))
                    {
                        error = "command with name: " + name + " does not exist";
                        return false;
                    }
                }
                Used = true;
                return true;
            }

            public override void Reset()
            {
                value = new List<string>();
            }
        }

        CommandOption commandOption;

        public class ShowOptionsOption : BaseOption<bool>
        {
            public ShowOptionsOption(string name, string description, string usage, bool Default, bool optional = true, bool used = false) : base(name, description, usage, Default, optional, used) { }

            public override bool GetValue(ref List<string> args, ref string error)
            {
                args.RemoveAt(0);
                Used = true;
                value = true;
                return true;
            }
        }

        ShowOptionsOption showOptionsOption = new ShowOptionsOption("o", "add information about command options to the result", "", false, true);

        BoolSettingOption showOptionOptional = new BoolSettingOption("oo", "add command option optional to the result", "", false);

        BoolSettingOption showOptionDefault = new BoolSettingOption("od", "add command option default value to the result", "", false);

        BoolSettingOption showAll = new BoolSettingOption("all", "show all avaliable information", "", false);

        public List<ICommand> commands = new List<ICommand>();

        public HelpCommand(): base("help", "provides information about commands (use -all to see all information about commands)", "[options]")
        {
            commandOption = new CommandOption(this, "", "specify which commands to display information about", "<name_1> | <name_2> | ... | <name_n>", new List<string>());

            options.Add(commandOption);
            options.Add(showOptionsOption);
            options.Add(showOptionOptional);
            options.Add(showOptionDefault);
            options.Add(showAll);
        }

        public override void Execute(List<string> args)
        {
            if (!ProcessOptions(ref args))
                return;

            List<string> commandNames = commandOption.value;

            bool displayAll = false;
            if (commandNames.Count == 0)
            {
                displayAll = true;
            }

            if (showAll.value)
            {
                showOptionsOption.value = true;
                showOptionOptional.value = true;
                showOptionDefault.value = true;
            }

            commands.Sort((x, y) => String.Compare(x.GetName(), y.GetName()));

            if (showAll.Used)
            {
                ConsoleFormat.PrintMessage("special symbols: " + ArgumentParser.GetSpecialSymbols());
            }

            foreach (var command in commands)
            {
                if ((commandNames.Contains(command.GetName()) || displayAll) && command.IsAvaliable())
                {
                    ConsoleFormat.PrintMessage(command.GetUsage(), " " + command.GetDescription());
                    
                    List<IOption> commandOptions = command.GetOptions();
                    commandOptions.Sort((x, y) => String.Compare(x.Name, y.Name));
                    if (commandOptions.Count > 0 && showOptionsOption.value)
                    {
                        ConsoleFormat.PrintMessage("OPTIONS:", ConsoleColor.Gray);
                        foreach (var option in commandOptions)
                        {
                            ConsoleFormat.PrintMessage("    " + option.Usage, " " + option.Description, ConsoleColor.DarkYellow);
                            if (showOptionOptional.value)
                                ConsoleFormat.PrintMessage("    OPTIONAL: ", option.Optional.ToString());
                            if (showOptionDefault.value)
                                ConsoleFormat.PrintMessage("    DEFAULT: ", option.GetDefault());
                        }
                    }

                    if (command == commands[commands.Count - 1])
                    {
                        Console.WriteLine();
                    }
                }
            }
        }
    }
}
