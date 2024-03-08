using Planum.Config;
using Planum.ConsoleUI.ConsoleCommands.TaskCommands;
using Planum.ConsoleUI.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Planum.ConsoleUI.CommandProcessor
{
    public class BaseCommand : ICommand
    {
        public List<IOption> options = new List<IOption>();
        public List<IOption> usedOptions = new List<IOption>();

        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual string Usage { get; set; }

        public bool defaultOptionsAdded = false;

        public CommandsConfig commandsConfig;

        BoolSettingOption useDefaultArgsOption = new BoolSettingOption("dud", "don't use default options option", "", false);

        public BaseCommand(string name, string description, string usage)
        {
            Name = name;
            Description = description;
            if (usage != "")
                Usage = name + " " + usage;
            else
                Usage = name;

            options.Add(useDefaultArgsOption);

            commandsConfig = new CommandsConfig();

            commandsConfig.LoadConfig();
            if (!commandsConfig.config.commands.ContainsKey(Name))
            {
                CommandData commandData = new CommandData();
                commandsConfig.config.commands.Add(Name, commandData);
            }
            commandsConfig.SaveConfig();
        }

        public bool ProcessOptions(ref List<string> args, string namelessOption = "")
        {
            foreach (var option in options)
                option.Reset();

            string[] name = GetName().Split();
            if (args.Count < name.Length)
            {
                ConsoleFormat.PrintError("incorrect command name");
                return false;
            }

            for (int i = 0; i < name.Length; i++)
            {
                if (name[i] != args[i])
                {
                    ConsoleFormat.PrintError("incorrect command name");
                    return false;
                }
            }

            args.RemoveRange(0, name.Length);

            usedOptions = new List<IOption>();

            int count;
            defaultOptionsAdded = false;
            while (args.Count > 0)
            {
                count = args.Count;

                if (args.Count != 0 && commandsConfig.config.commands[Name].aliases.ContainsKey(args.First()))
                {
                    string alias = args[0];
                    args.RemoveAt(0);
                    args = commandsConfig.config.commands[Name].aliases[alias].Concat(args).ToList();
                    count = args.Count;
                }

                foreach (var option in options)
                {
                    if (args.Count == 0)
                        break;
                    if (!args[0].StartsWith(ArgumentParser.CommandDelimeter) && namelessOption == option.Name)
                    {
                        string error = "";
                        if (!option.GetValue(ref args, ref error))
                        {
                            ConsoleFormat.PrintError("incorrect option usage: " + option.Name + "\nerror: " + error);
                            return false;
                        }
                        usedOptions.Add(option);
                    }
                    else if (args[0] == option.Name && option.Used == false)
                    {
                        string error = "";
                        if (!option.GetValue(ref args, ref error))
                        {
                            ConsoleFormat.PrintError("incorrect option usage: " + option.Name + "\nerror: " + error);
                            return false;
                        }
                        usedOptions.Add(option);
                    }
                }

                if (args.Count != 0 && count == args.Count)
                {
                    ConsoleFormat.PrintError("unknown option: " + args[0]);
                    return false;
                }

                if (useDefaultArgsOption.Used && !defaultOptionsAdded)
                {
                    args = args.Concat(commandsConfig.config.commands[Name].aliases["--default"]).ToList();
                    defaultOptionsAdded = true;
                }
            }

            foreach (var option in options)
            {
                if (option.Optional == false && option.Used == false)
                {
                    ConsoleFormat.PrintError("mandatory option not used: " + option.Name);
                    return false;
                }
            }

            return true;
        }

        public virtual void Execute(List<string> args)
        {
            
        }

        public virtual string GetDescription()
        {
            return Description;
        }

        public virtual string GetName()
        {
            return Name;
        }

        public virtual List<IOption> GetOptions()
        {
            return options;
        }

        public virtual string GetUsage()
        {
            return Usage;
        }

        public virtual bool IsAvaliable()
        {
            return true;
        }

        public virtual bool IsCommand(List<string> args)
        {
            string[] name = GetName().Split();
            int n = name.Length;
            if (args.Count < n)
                return false;
            else
            {
                for (int i = 0; i < n; i++)
                {
                    if (name[i] != args[i])
                        return false;
                }
            }
            return true;
        }
    }
}
