using Planum.ConsoleUI.CommandProcessor;
using Planum.ConsoleUI.ConsoleCommands.TaskCommands;
using Planum.ConsoleUI.UI;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Planum.ConsoleUI.ConsoleCommands.CommonCommands
{
    public class ConfigCommand : BaseCommand
    {
        BoolValueOption callOnStartOption = new BoolValueOption("cos", "enable call on start for specified commands", "", false);

        StringListOption aliasOption = new StringListOption("a", "add or change alias, options must be entered withoud delimeter", "<alias name> | <option_1> | ... | <option_n>", new List<string>());

        StringOption removeAliasOption = new StringOption("ra", "remove alias with specified name", "<alias name>", "");

        BoolSettingOption showConfigParamsOption = new BoolSettingOption("s", "show config params for specified commands", "", false);

        public class CommandOption : BaseOption<List<string>>
        {
            ConfigCommand configCommand;

            public CommandOption(ConfigCommand configCommand, string name, string description, string usage, List<string> Default, bool optional = true, bool used = false) : base(name, description, usage, Default, optional, used)
            {
                this.configCommand = configCommand;
                Used = true;
            }

            public override bool GetValue(ref List<string> args, ref string error)
            {
                bool result = ArgumentParser.Parse(ref value, ref args);
                if (!result)
                    return false;
                foreach (var name in value)
                {
                    if (!configCommand.commands.Exists(x => x.GetName() == name))
                    {
                        error = "command with name: " + name + " does not exist";
                        return false;
                    }
                }

                return true;
            }

            public override void Reset()
            {
                value = new List<string>();
            }
        }

        CommandOption commandOption;

        public List<ICommand> commands = new List<ICommand>();

        public ConfigCommand() : base("config", "configures app parameters", "[command names] [options]")
        {
            commandOption = new CommandOption(this, "", "specify which commands config chagnes being applied", "<name_1> | <name_2> | ... | <name_n>", new List<string>(), false);

            options.Add(commandOption);
            options.Add(callOnStartOption);
            options.Add(aliasOption);
            options.Add(removeAliasOption);
            options.Add(showConfigParamsOption);
        }

        public override void Execute(List<string> args)
        {
            if (!ProcessOptions(ref args))
                return;

            commandsConfig.LoadConfig();

            if (aliasOption.Used)
            {
                if (aliasOption.value.Count < 2)
                {
                    ConsoleFormat.PrintError(aliasOption.Name + " error, options for alias unspecified");
                    return;
                }

                string aliasName = aliasOption.value.First();
                List<string> aliasVals = new List<string>();
                foreach (var val in aliasOption.value.GetRange(1, aliasOption.value.Count - 1))
                {
                    aliasVals.Add(ArgumentParser.CommandDelimeter + val);
                }
                aliasVals.Insert(0, aliasName);
                aliasOption.value = aliasVals;
            }

            List<string> commandNames = commandOption.value;
            
            if (commandNames.Count == 0)
            {
                commandNames = commands.Select(x => x.GetName()).ToList();
            }

            bool usedOptions = false;
            foreach (var option in options)
            {
                if (option.Name != commandOption.Name && option.Used)
                    usedOptions = true;
            }

            if (!usedOptions)
            {
                ConsoleFormat.PrintWarning("no options used - nothing to output");
                return;
            }

            foreach (string name in commandNames)
            {
                if (callOnStartOption.Used)
                    commandsConfig.config.commands[name].callOnStart = callOnStartOption.value;

                if (aliasOption.Used)
                {
                    if (commandsConfig.config.commands[name].aliases.ContainsKey(aliasOption.value[0]))
                    {
                        commandsConfig.config.commands[name].aliases[aliasOption.value[0]] = aliasOption.value.GetRange(1, aliasOption.value.Count - 1);
                    }
                    else
                        commandsConfig.config.commands[name].aliases.Add(aliasOption.value[0], aliasOption.value.GetRange(1, aliasOption.value.Count - 1));
                }

                if (removeAliasOption.Used)
                {
                    if (commandsConfig.config.commands[name].aliases.ContainsKey(removeAliasOption.value))
                        commandsConfig.config.commands[name].aliases.Remove(removeAliasOption.value);
                }

                if (showConfigParamsOption.Used)
                {
                    ConsoleFormat.PrintMessage("command: ", name, ConsoleColor.Cyan);
                    if (commandsConfig.config.commands[name].callOnStart)
                        ConsoleFormat.PrintMessage("call on start: ", "true", ConsoleColor.Gray, ConsoleColor.DarkGreen);
                    else
                        ConsoleFormat.PrintMessage("call on start: ", "false", ConsoleColor.Gray, ConsoleColor.DarkRed);
                    ConsoleFormat.PrintMessage("aliases:", ConsoleColor.White);
                    foreach (var key in commandsConfig.config.commands[name].aliases.Keys)
                    {
                        ConsoleFormat.PrintMessage("    alias: ", key, ConsoleColor.Yellow);
                        string options = "";
                        foreach (var option in commandsConfig.config.commands[name].aliases[key])
                            options += option + " ";
                        ConsoleFormat.PrintMessage("    options: ", options, ConsoleColor.DarkYellow);
                    }
                    Console.WriteLine();
                }
            }

            if (callOnStartOption.Used || aliasOption.Used || removeAliasOption.Used)
                ConsoleFormat.PrintSuccess("succesfully changed config");
            commandsConfig.SaveConfig();
        }
    }
}
