using System.Collections.Generic;
using Planum.Commands;
using Planum.Commands.Selector;
using Planum.Config;
using Planum.Logger;
using Planum.Model.Filters;
using Planum.Model.Managers;
using Planum.Repository;

namespace Planum
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // initialize logger
            var logger = new PlanumLogger();

            // initialize config
            var appConfig = AppConfig.Load(logger);
            var repoConfig = RepoConfig.Load(logger);

            // initialize repo
            var taskReader = new TaskMarkdownReader(logger, appConfig, repoConfig);
            var taskWriter = new TaskMarkdownWriter(logger, appConfig, repoConfig);
            var taskFileManager = new TaskFileManager(repoConfig, taskWriter, taskReader, logger);
            var taskRepo = new TaskRepo(taskFileManager);

            // initialize managers
            var taskValidationManager = new TaskValidationManager();
            var taskBufferManager = new TaskBufferManager(taskRepo, taskValidationManager);

            // initialize commands
            var commandConfig = CommandConfig.Load(logger);
            var selectorOptions = new List<IOption<TaskFilter>>() {
                new SelectorIdOption(commandConfig, new OptionInfo("s", "select task via id or name", "s[match type][logic operator] { value (name or id, full or partial) }"))
            };
            var selectorParser = new SelectorParser(taskBufferManager, selectorOptions);

            // create command
            var createCommandOptions = new List<IOption<CreateCommandSettings>>() {
                new CreateNameOption(new OptionInfo("n", "specify name of a command", "n [task name]"), commandConfig),
            };
            var createCommand = new CreateCommand(taskBufferManager, selectorParser, new CommandInfo("create", "creates new task", "create [options]"), createCommandOptions);
            
            var commands = new List<ICommand<ICommandSettings>>() {
                (ICommand<ICommandSettings>)createCommand,
            };

            var commandManager = new CommandManager(commands);
            var consoleManager = new ConsoleManager(commandManager);

            // run
            consoleManager.RunCommandMode(args);
        }
    }
}
