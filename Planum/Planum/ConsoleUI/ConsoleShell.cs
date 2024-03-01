using Planum.Config;
using Planum.ConsoleUI.ConsoleCommands.CommonCommands;
using Planum.ConsoleUI.ConsoleCommands.TaskCommands;
using Planum.ConsoleUI.UI;
using Planum.Model.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;

namespace Planum.ConsoleUI
{
    public class ConsoleShell
    {
        protected List<ICommand> commands;
        ExitCommand exitCommand = new ExitCommand();
        HelpCommand helpCommand = new HelpCommand();
        ConfigCommand configCommand = new ConfigCommand();
        public AppConfig appConfig;
        CommandsConfig commandsConfig;

        TaskManager taskManager;

        public ConsoleShell(TaskManager taskManager, AppConfig appConfig)
        {
            this.appConfig = appConfig;
            this.commandsConfig = new CommandsConfig(); 
            this.taskManager = taskManager;

            ShowTaskGridCommand showTaskGridCommand = new ShowTaskGridCommand(taskManager);
            CreateTaskCommand createTaskCommand = new CreateTaskCommand(taskManager, showTaskGridCommand);
            UpdateTaskCommand updateTaskCommand = new UpdateTaskCommand(taskManager, showTaskGridCommand);

            List<ICommand> commands = new List<ICommand>()
            {
                exitCommand,
                helpCommand,

                new CompleteTaskCommand(taskManager),
                createTaskCommand,
                updateTaskCommand,
                new DeleteTaskCommand(taskManager),
                showTaskGridCommand,
                new ShowTreeCommand(taskManager),
                new ShowTaskCaledarCommand(taskManager),
                new CopyTaskCommand(taskManager),
                new BackupTaskCommand(taskManager),
                new UndoCommand(taskManager),
                new DelayTaskCommand(taskManager),
                new SummaryTaskCommand(taskManager),
                configCommand
            };

            commandsConfig.LoadConfig();

            configCommand.commands = commands;
            helpCommand.commands = commands;

            this.commands = commands;
        }

        protected void WriteGreeting()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.ForegroundColor = ConsoleColor.Blue;
            string text = "╭────╮                                      \n" +
                          "│ ╭╮ │ ╭─╮    ╭────╮ ╭────╮ ╭─╮╭─╮ ╭───────╮\n" +
                          "│ ╰╯ │ │ │    │ ╭╮ │ │ ╭╮ │ │ ││ │ │ ╭╮ ╭╮ │\n" +
                          "│ ╭──╯ │ │    │ ╰╯ │ │ ││ │ │ ││ │ │ ││ ││ │\n" +
                          "│ │    │ ╰──╮ │ ╭╮ │ │ ││ │ │ ╰╯ │ │ ││ ││ │\n" +
                          "╰─╯    ╰────╯ ╰─╯╰─╯ ╰─╯╰─╯ ╰────╯ ╰─╯╰─╯╰─╯";
            Console.Write(text);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(" " + appConfig.config.version);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void MainLoop(string[] consoleArgs)
        {
            bool consoleMode = false;
            if (consoleArgs.Length > 0)
                consoleMode = true;
            else
                WriteGreeting();

            // call on start processing
            foreach (var command in commands)
            {
                if (commandsConfig.config.commands[command.GetName()].callOnStart)
                {
                    List<string> args = new List<string>() { command.GetName() };

                    if (command.IsCommand(args))
                    {
                        if (command.IsAvaliable() == false)
                        {
                            ConsoleFormat.PrintWarning("command is not avaliable");
                        }
                        command.Execute(args);
                    }
                }
            }

            while (!exitCommand.exit)
            {
                taskManager.CheckAutorepeat();
                commandsConfig.LoadConfig();

                string? input = "";
                List<string> args = new List<string>();
                if (!consoleMode)
                {
                    Console.Write("> ");
                    input = Console.ReadLine().Trim();
                    if (string.IsNullOrEmpty(input)) { continue; }

                    args = new List<string>(input.Split(' '));
                }
                else
                {
                    args = new List<string>(consoleArgs);
                    foreach (var arg in args)
                        input += arg + " ";
                }

                bool foundCommand = false;
                foreach (var command in commands)
                {
                    if (command.IsCommand(args))
                    {
                        foundCommand = true;
                        if (command.IsAvaliable() == false)
                        {
                            ConsoleFormat.PrintWarning("command is not avaliable");
                        }

                        command.Execute(args);
                        break;
                    }
                }

                if (!foundCommand)
                {
                    ConsoleFormat.PrintError("command: " + input + " doesn't match with existing commands");
                }

                if (consoleMode)
                    exitCommand.exit = true;
            }
        }
    }
}
