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
            // logger
            var logger = new PlanumLogger(where: LogWhere.CONSOLE, level: LogLevel.INFO);

            // config
            var appConfig = AppConfig.Load(logger);
            var repoConfig = RepoConfig.Load(appConfig, logger);

            // repo
            var taskReader = new TaskMarkdownReader(logger, repoConfig);
            var taskWriter = new TaskMarkdownWriter(logger, repoConfig);
            var taskFileManager = new TaskFileManager(appConfig, repoConfig, taskWriter, taskReader, logger);
            var taskRepo = new TaskRepo(taskFileManager);

            // managers
            var taskValidationManager = new TaskValidationManager();
            var taskBufferManager = new TaskBufferManager(taskRepo, taskValidationManager);

            // commands
            var commandConfig = CommandConfig.Load(appConfig, logger);

            // selector options

            var selectorIdOption = new SelectorIdOption(commandConfig,
                    new OptionInfo("Si", "select by id", "[match type][match filter type] guid"));
            var selectorNameOption = new SelectorNameOption(commandConfig,
                    new OptionInfo("Sn", "select by name", "[match type][match filter type] string"));
            var selectorDescriptionOption = new SelectorDescriptionOption(commandConfig,
                    new OptionInfo("Sd", "select by description", "[match type][match filter type] string"));
            var selectorChildOption = new SelectorChildOption(commandConfig,
                    new OptionInfo("Sc", "select by child id", "[match type][match filter type] guid"));
            var selectorParentOption = new SelectorParentOption(commandConfig,
                    new OptionInfo("Sp", "select by parent id", "[match type][match filter type] guid"));
            var selectorTagOption = new SelectorTagOption(commandConfig,
                    new OptionInfo("St", "select by tag", "[match type][match filter type] string"));

            var selectorDeadlineIdOption = new SelectorDeadlineIdOption(commandConfig,
                    new OptionInfo("SDi", "select by deadline id", "[match type][match filter type] guid"));
            var selectorDeadlineEnabledOption = new SelectorDeadlineEnabledOption(commandConfig,
                    new OptionInfo("SDe", "select by deadline enabled", "[match type][match filter type] bool"));
            var selectorDeadlineValueOption = new SelectorDeadlineValueOption(commandConfig,
                    new OptionInfo("SDv", "select by deadline value", "[match type][match filter type] datetime"));
            var selectorDeadlineWarningOption = new SelectorDeadlineWarningOption(commandConfig,
                    new OptionInfo("SDw", "select by deadline warning", "[match type][match filter type] timespan"));
            var selectorDeadlineDurationOption = new SelectorDeadlineDurationOption(commandConfig,
                    new OptionInfo("SDd", "select by deadline duration", "[match type][match filter type] timespan"));
            var selectorDeadlineRepeatSpanOption = new SelectorDeadlineRepeatSpanOption(commandConfig,
                    new OptionInfo("SDrs", "select by deadline repeat span", "[match type][match filter type] timespan"));
            var selectorDeadlineRepeatMonthsOption = new SelectorDeadlineRepeatMonthsOption(commandConfig,
                    new OptionInfo("SDrm", "select by deadline repeat months", "[match type][match filter type] int"));
            var selectorDeadlineRepeatYearsOption = new SelectorDeadlineRepeatYearsOption(commandConfig,
                    new OptionInfo("SDry", "select by deadline years", "[match type][match filter type] int"));
            var selectorDeadlineRepeatedOption = new SelectorDeadlineRepeatedOption(commandConfig,
                    new OptionInfo("SDr", "select by deadline repeat", "[match type][match filter type] bool"));

            // basic commands
            var taskCommitOption = new CommitOption();
            var taskFilenameOption = new FilenameOption();

            var taskNameOption = new NameOption();
            var taskDescriptionOption = new DescriptionOption();
            var taskTagAddOption = new TagAddOption();
            var taskTagRemoveOption = new TagRemoveOption();
            var taskChildAddOption = new ChildAddOption();
            var taskChildRemoveOption = new ChildRemoveOption();
            var taskParentAddOption = new ParentAddOption();
            var taskParentRemoveOption = new ParentRemoveOption();

            var taskAddDeadlineOption = new AddDeadlineOption();
            var taskDeadlineValueOption = new ValueOption();
            var taskDeadlineWarningOption = new WarningOption();
            var taskDeadlineDurationOption = new DurationOption();
            var taskDeadlineEnabledOption = new EnabledOption();
            var taskDeadlineNextAddOption = new NextAddOption();
            var taskDeadlineNextRemoveOption = new NextRemoveOption();
            var taskDeadlineRemoveDeadlineOption = new RemoveDeadlineOption();
            var taskDeadlineRepeatSpanOption = new RepeatSpanOption();
            var taskDeadlineRepeatMonthsOption = new RepeatMonthsOption();
            var taskDeadlineRepeatYearsOption = new RepeatYearsOption();
            var taskDeadlineRepeatedOption = new RepeatedOption();

            ICommand createCommand = new CreateCommand(taskBufferManager,
                    new CommandInfo("create", "creates new task", "create [options...]"),
                    new List<BaseOption<TaskCommandSettings>> {
                    },
                    logger);

            ICommand updateCommand = new UpdateCommand(taskBufferManager,
                    new List<SelectorBaseOption>() {
                    },
                    new CommandInfo("update", "update task values", "update [options...]"),
                    new List<BaseOption<TaskCommandSettings>>() {
                    },
                    logger);

            ICommand deleteCommand =  = new DeleteCommand(taskBufferManager,
                    new List<SelectorBaseOption>() {
                    },
                    new CommandInfo("delete", "delete selected tasks", "delete [options...]"),
                    new List<BaseOption<TaskCommandSettings>>() {
                    },
                    logger);

            // special commands

            // view commands

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
