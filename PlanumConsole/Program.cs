using Planum.Config;
using Planum.Console;
using Planum.Console.Commands;
using Planum.Console.Commands.Selector;
using Planum.Console.Commands.Special;
using Planum.Console.Commands.Task;
using Planum.Console.Commands.View;
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
            var consoleConfig = ConsoleConfig.Load("./ConsoleConfig.json", logger);
            var repoConfig = RepoConfig.Load(consoleConfig.RepoConfigPath, logger);

            // repo
            var taskReader = new TaskMarkdownReader(logger, repoConfig);
            var taskWriter = new TaskMarkdownWriter(logger, repoConfig);
            var taskFileManager = new TaskFileManager(repoConfig, taskWriter, taskReader, logger);
            var taskRepo = new TaskRepo(logger, taskFileManager);

            // managers
            var taskValidationManager = new TaskValidationManager();
            var taskBufferManager = new TaskBufferManager(logger, taskRepo, taskValidationManager);

            // selector options
            var selectorIdOption = new SelectorIdOption(logger, consoleConfig,
                    new OptionInfo("Si", "select by id", "[match type][match filter type] guid"));
            var selectorNameOption = new SelectorNameOption(logger, consoleConfig,
                    new OptionInfo("Sn", "select by name", "[match type][match filter type] string"));
            var selectorDescriptionOption = new SelectorDescriptionOption(logger, consoleConfig,
                    new OptionInfo("Sd", "select by description", "[match type][match filter type] string"));
            var selectorChildOption = new SelectorChildOption(logger, consoleConfig,
                    new OptionInfo("Sc", "select by child id", "[match type][match filter type] guid"));
            var selectorParentOption = new SelectorParentOption(logger, consoleConfig,
                    new OptionInfo("Sp", "select by parent id", "[match type][match filter type] guid"));
            var selectorTagOption = new SelectorTagOption(logger, consoleConfig,
                    new OptionInfo("St", "select by tag", "[match type][match filter type] string"));

            var selectorDeadlineIdOption = new SelectorDeadlineIdOption(logger, consoleConfig,
                    new OptionInfo("SDi", "select by deadline id", "[match type][match filter type] guid"));
            var selectorDeadlineEnabledOption = new SelectorDeadlineEnabledOption(logger, consoleConfig,
                    new OptionInfo("SDe", "select by deadline enabled", "[match type][match filter type] bool"));
            var selectorDeadlineValueOption = new SelectorDeadlineValueOption(logger, consoleConfig,
                    new OptionInfo("SDv", "select by deadline value", "[match type][match filter type] datetime"));
            var selectorDeadlineWarningOption = new SelectorDeadlineWarningOption(logger, consoleConfig,
                    new OptionInfo("SDw", "select by deadline warning", "[match type][match filter type] timespan"));
            var selectorDeadlineDurationOption = new SelectorDeadlineDurationOption(logger, consoleConfig,
                    new OptionInfo("SDd", "select by deadline duration", "[match type][match filter type] timespan"));
            var selectorDeadlineRepeatSpanOption = new SelectorDeadlineRepeatSpanOption(logger, consoleConfig,
                    new OptionInfo("SDrs", "select by deadline repeat span", "[match type][match filter type] timespan"));
            var selectorDeadlineRepeatMonthsOption = new SelectorDeadlineRepeatMonthsOption(logger, consoleConfig,
                    new OptionInfo("SDrm", "select by deadline repeat months", "[match type][match filter type] int"));
            var selectorDeadlineRepeatYearsOption = new SelectorDeadlineRepeatYearsOption(logger, consoleConfig,
                    new OptionInfo("SDry", "select by deadline years", "[match type][match filter type] int"));
            var selectorDeadlineRepeatedOption = new SelectorDeadlineRepeatedOption(logger, consoleConfig,
                    new OptionInfo("SDr", "select by deadline repeat", "[match type][match filter type] bool"));

            // basic commands
            var taskCommitOption = new CommitOption(logger, new OptionInfo("C", "commit buffer changes into files at the end of command execution", ""), consoleConfig);
            var taskFilenameOption = new FilenameOption(logger, new OptionInfo("F", "specify filename for task to be stored in", " filename"), consoleConfig);

            var taskNameOption = new NameOption(logger, new OptionInfo("n", "specify task name", " name"), consoleConfig);
            var taskDescriptionOption = new DescriptionOption(logger, new OptionInfo("d", "specify task description", " description"), consoleConfig);
            var taskTagAddOption = new TagAddOption(logger, new OptionInfo("ta", "add new tag to the task", " tag"), consoleConfig);
            var taskTagRemoveOption = new TagRemoveOption(logger, new OptionInfo("tr", "remove tag from the task", " tag"), consoleConfig);
            var taskChildAddOption = new ChildAddOption(logger, taskBufferManager, new OptionInfo("ca", "add child to the task", " fuzzy_guid"), consoleConfig);
            var taskChildRemoveOption = new ChildRemoveOption(logger, taskBufferManager, new OptionInfo("cr", "remove child from the task", " fuzzy_guid"), consoleConfig);
            var taskParentAddOption = new ParentAddOption(logger, taskBufferManager, new OptionInfo("pa", "add parent to the task", " fuzzy_guid"), consoleConfig);
            var taskParentRemoveOption = new ParentRemoveOption(logger, taskBufferManager, new OptionInfo("pr", "remove parent from the task", " fuzzy_guid"), consoleConfig);

            var taskAddDeadlineOption = new AddDeadlineOption(logger, new OptionInfo("Da", "add new deadline to the task", ""), consoleConfig);
            var taskDeadlineValueOption = new ValueOption(logger, new OptionInfo("Dv", "specify Deadline deadline value", " datetime"), consoleConfig);
            var taskDeadlineWarningOption = new WarningOption(logger, new OptionInfo("Dw", "specify warning time", " timespan"), consoleConfig);
            var taskDeadlineDurationOption = new DurationOption(logger, new OptionInfo("Dd", "specify duration", " timespan"), consoleConfig);
            var taskDeadlineEnabledOption = new EnabledOption(logger, new OptionInfo("De", "specify enabled", " bool"), consoleConfig);
            var taskDeadlineNextAddOption = new NextAddOption(logger, taskBufferManager, new OptionInfo("Dna", "add task to be executed next after deadline completion", " fuzzy_guid"), consoleConfig);
            var taskDeadlineNextRemoveOption = new NextRemoveOption(logger, taskBufferManager, new OptionInfo("Dnr", "remove next task to be executed after pipeline completion", " fuzzy_guid"), consoleConfig);
            var taskDeadlineRemoveDeadlineOption = new RemoveDeadlineOption(logger, new OptionInfo("Drd", "remove deadline from task", " fuzzy_guid"), consoleConfig);
            var taskDeadlineRepeatSpanOption = new RepeatSpanOption(logger, new OptionInfo("Drs", "specify repeat span", " timespan"), consoleConfig);
            var taskDeadlineRepeatMonthsOption = new RepeatMonthsOption(logger, new OptionInfo("Drm", "specify repeat months", " int"), consoleConfig);
            var taskDeadlineRepeatYearsOption = new RepeatYearsOption(logger, new OptionInfo("Dry", "specify repeat years", " int"), consoleConfig);
            var taskDeadlineRepeatedOption = new RepeatedOption(logger, new OptionInfo("Dr", "enable or disable task repetition", " bool"), consoleConfig);

            ICommand createCommand = new CreateCommand(taskBufferManager,
                    new CommandInfo("create", "creates new task", " [options...]"),
                    new List<BaseOption<TaskCommandSettings>>
                    {
                        taskCommitOption,
                        taskFilenameOption,

                        taskNameOption,
                        taskDescriptionOption,
                        taskTagAddOption,
                        taskTagRemoveOption,
                        taskChildAddOption,
                        taskChildRemoveOption,
                        taskParentAddOption,
                        taskParentRemoveOption,

                        taskAddDeadlineOption,
                        taskDeadlineValueOption,
                        taskDeadlineWarningOption,
                        taskDeadlineDurationOption,
                        taskDeadlineEnabledOption,
                        taskDeadlineNextAddOption,
                        taskDeadlineNextRemoveOption,
                        taskDeadlineRemoveDeadlineOption,
                        taskDeadlineRepeatSpanOption,
                        taskDeadlineRepeatMonthsOption,
                        taskDeadlineRepeatYearsOption,
                        taskDeadlineRepeatedOption
                    },
                    logger);

            ICommand updateCommand = new UpdateCommand(taskBufferManager,
                    new List<SelectorBaseOption>()
                    {
                        selectorIdOption,
                        selectorNameOption,
                        selectorDescriptionOption,
                        selectorChildOption,
                        selectorParentOption,
                        selectorTagOption,

                        selectorDeadlineIdOption,
                        selectorDeadlineEnabledOption,
                        selectorDeadlineValueOption,
                        selectorDeadlineWarningOption,
                        selectorDeadlineDurationOption,
                        selectorDeadlineRepeatSpanOption,
                        selectorDeadlineRepeatMonthsOption,
                        selectorDeadlineRepeatYearsOption,
                        selectorDeadlineRepeatedOption
                    },
                    new CommandInfo("update", "update task values", " [options...]"),
                    new List<BaseOption<TaskCommandSettings>>()
                    {
                        taskCommitOption,
                        taskFilenameOption,

                        taskNameOption,
                        taskDescriptionOption,
                        taskTagAddOption,
                        taskTagRemoveOption,
                        taskChildAddOption,
                        taskChildRemoveOption,
                        taskParentAddOption,
                        taskParentRemoveOption,

                        taskAddDeadlineOption,
                        taskDeadlineValueOption,
                        taskDeadlineWarningOption,
                        taskDeadlineDurationOption,
                        taskDeadlineEnabledOption,
                        taskDeadlineNextAddOption,
                        taskDeadlineNextRemoveOption,
                        taskDeadlineRemoveDeadlineOption,
                        taskDeadlineRepeatSpanOption,
                        taskDeadlineRepeatMonthsOption,
                        taskDeadlineRepeatYearsOption,
                        taskDeadlineRepeatedOption
                    },
                    logger);

            ICommand deleteCommand = new DeleteCommand(taskBufferManager,
                    new List<SelectorBaseOption>()
                    {
                        selectorIdOption,
                        selectorNameOption,
                        selectorDescriptionOption,
                        selectorChildOption,
                        selectorParentOption,
                        selectorTagOption,

                        selectorDeadlineIdOption,
                        selectorDeadlineEnabledOption,
                        selectorDeadlineValueOption,
                        selectorDeadlineWarningOption,
                        selectorDeadlineDurationOption,
                        selectorDeadlineRepeatSpanOption,
                        selectorDeadlineRepeatMonthsOption,
                        selectorDeadlineRepeatYearsOption,
                        selectorDeadlineRepeatedOption
                    },
                    new CommandInfo("delete", "delete selected tasks", " [options...]"),
                    new List<BaseOption<TaskCommandSettings>>()
                    {
                        taskCommitOption
                    },
                    logger);

            // special commands
            var saveCommand = new SaveCommand(repoConfig,
                    taskBufferManager,
                    new CommandInfo("save", "saves current buffer back into the files", ""),
                    new List<BaseOption<TaskStorageCommandSettings>>() { },
                    logger);

            var loadCommand = new LoadCommand(repoConfig,
                    taskBufferManager,
                    new CommandInfo("load", "loads tasks into buffer from the files", ""),
                    new List<BaseOption<TaskStorageCommandSettings>>() { },
                    logger);

            var dirCommand = new DirCommand(repoConfig,
                    taskBufferManager,
                    new CommandInfo("dir", "allows editing and listing of directories which will be recursively searched for task files", " [options...]"),
                    new List<BaseOption<TaskStorageCommandSettings>>()
                    {
                        new AddDirOption(logger, new OptionInfo("a", "add path to directory into lookup paths", " path"), consoleConfig),
                        new RemoveDirOption(logger, new OptionInfo("r", "remove path to directory into lookup paths", " path"), consoleConfig),
                        new ListDirOption(logger, new OptionInfo("l", "list resulted lookup paths", ""), consoleConfig)
                    },
                    logger);

            var exitCommand = new ExitCommand(repoConfig,
                    new CommandInfo("exit", "exit from app", ""),
                    new List<BaseOption<ExitCommandSettings>>(),
                    logger);

            // view commands
            var listCommand = new ListCommand(repoConfig,
                    taskBufferManager,
                    new List<SelectorBaseOption>()
                    {
                        selectorIdOption,
                        selectorNameOption,
                        selectorDescriptionOption,
                        selectorChildOption,
                        selectorParentOption,
                        selectorTagOption,

                        selectorDeadlineIdOption,
                        selectorDeadlineEnabledOption,
                        selectorDeadlineValueOption,
                        selectorDeadlineWarningOption,
                        selectorDeadlineDurationOption,
                        selectorDeadlineRepeatSpanOption,
                        selectorDeadlineRepeatMonthsOption,
                        selectorDeadlineRepeatYearsOption,
                        selectorDeadlineRepeatedOption
                    },
                    new CommandInfo("list", "display tasks in list format", " [options...]"),
                    new List<BaseOption<ListCommandSettings>>()
                    {
                    },
                    logger
                );

            // console initialization
            var commands = new List<ICommand>() {
                createCommand,
                updateCommand,
                deleteCommand,

                saveCommand,
                loadCommand,
                dirCommand,
                exitCommand,

                listCommand
            };

            var helpCommand = new HelpCommand(
                    commands,
                    new CommandInfo("help", "display info about commands and their options", " [options...]"),
                    new List<BaseOption<HelpCommandSettings>>()
                    {
                        new HelpShowOptionsOption(logger, new OptionInfo("o", "show options", ""), consoleConfig),
                        new HelpOptionLikeOption(logger, new OptionInfo("ol", "filter option by name via string (if contains)", " string"), consoleConfig),
                        new HelpCommandLikeOption(logger, new OptionInfo("cl", "filter commands by name via string (if contains)", " string"), consoleConfig),
                    },
                    logger);

            commands.Add(helpCommand);

            var commandManager = new CommandManager(commands, exitCommand, logger);
            var consoleManager = new ConsoleManager(commandManager, logger);

            if (args.Length == 0)
                consoleManager.RunConsoleMode();
            else
                consoleManager.RunCommandMode(args);
        }
    }
}
