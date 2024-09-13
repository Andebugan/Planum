using System.Collections.Generic;
using Planum.Config;
using Planum.Console;
using Planum.Console.Commands;
using Planum.Console.Commands.Selector;
using Planum.Console.Commands.Task;
using Planum.Logger;
using Planum.Model.Managers;
using Planum.Repository;

namespace Planum
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // initialize logger
            var logger = new PlanumLogger(where: LogWhere.CONSOLE, level: LogLevel.INFO);

            // initialize config
            var appConfig = AppConfig.Load(logger);
            var repoConfig = RepoConfig.Load(appConfig, logger);

            // initialize repo
            var taskReader = new TaskMarkdownReader(logger, appConfig, repoConfig);
            var taskWriter = new TaskMarkdownWriter(logger, appConfig, repoConfig);
            var taskFileManager = new TaskFileManager(appConfig, repoConfig, taskWriter, taskReader, logger);
            var taskRepo = new TaskRepo(taskFileManager);

            // initialize managers
            var taskValidationManager = new TaskValidationManager();
            var taskBufferManager = new TaskBufferManager(taskRepo, taskValidationManager);

            // initialize commands
            var commandConfig = CommandConfig.Load(appConfig, logger);
            var selectorOptions = new List<SelectorBaseOption>() {
                new SelectorIdOption(commandConfig, new OptionInfo("s", "select task via id or name", "s[match type][logic operator] { value (name or id, full or partial) }"))
            };

            // create command
            var createCommandOptions = new List<BaseOption<TaskCommandSettings>>() {
                new NameOption(new OptionInfo("n", "specify name of a command", "n [task name]"), commandConfig),
            };

            ICommand createCommand = new CreateCommand(taskBufferManager, selectorOptions, new CommandInfo("create", "creates new task", "create [options]"), createCommandOptions, logger);

            var commands = new List<ICommand>() {
                createCommand,
            };

            var commandManager = new CommandManager(commands, logger);
            var consoleManager = new ConsoleManager(commandManager, logger);

            // run
            if (args.Length == 0)
                consoleManager.RunConsoleMode();
            else
                consoleManager.RunCommandMode(args);
        }
    }
}
