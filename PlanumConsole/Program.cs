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
            var taskRepo = new TaskRepo(taskFileManager);

            // managers
            var taskValidationManager = new TaskValidationManager();
            var taskBufferManager = new TaskBufferManager(taskRepo, taskValidationManager);

            // selector options
            var selectorIdOption = new SelectorIdOption(consoleConfig,
                    new OptionInfo("Si", "select by id", "[match type][match filter type] guid"));
            var selectorNameOption = new SelectorNameOption(consoleConfig,
                    new OptionInfo("Sn", "select by name", "[match type][match filter type] string"));
            var selectorDescriptionOption = new SelectorDescriptionOption(consoleConfig,
                    new OptionInfo("Sd", "select by description", "[match type][match filter type] string"));
            var selectorChildOption = new SelectorChildOption(consoleConfig,
                    new OptionInfo("Sc", "select by child id", "[match type][match filter type] guid"));
            var selectorParentOption = new SelectorParentOption(consoleConfig,
                    new OptionInfo("Sp", "select by parent id", "[match type][match filter type] guid"));
            var selectorTagOption = new SelectorTagOption(consoleConfig,
                    new OptionInfo("St", "select by tag", "[match type][match filter type] string"));

            var selectorDeadlineIdOption = new SelectorDeadlineIdOption(consoleConfig,
                    new OptionInfo("SDi", "select by deadline id", "[match type][match filter type] guid"));
            var selectorDeadlineEnabledOption = new SelectorDeadlineEnabledOption(consoleConfig,
                    new OptionInfo("SDe", "select by deadline enabled", "[match type][match filter type] bool"));
            var selectorDeadlineValueOption = new SelectorDeadlineValueOption(consoleConfig,
                    new OptionInfo("SDv", "select by deadline value", "[match type][match filter type] datetime"));
            var selectorDeadlineWarningOption = new SelectorDeadlineWarningOption(consoleConfig,
                    new OptionInfo("SDw", "select by deadline warning", "[match type][match filter type] timespan"));
            var selectorDeadlineDurationOption = new SelectorDeadlineDurationOption(consoleConfig,
                    new OptionInfo("SDd", "select by deadline duration", "[match type][match filter type] timespan"));
            var selectorDeadlineRepeatSpanOption = new SelectorDeadlineRepeatSpanOption(consoleConfig,
                    new OptionInfo("SDrs", "select by deadline repeat span", "[match type][match filter type] timespan"));
            var selectorDeadlineRepeatMonthsOption = new SelectorDeadlineRepeatMonthsOption(consoleConfig,
                    new OptionInfo("SDrm", "select by deadline repeat months", "[match type][match filter type] int"));
            var selectorDeadlineRepeatYearsOption = new SelectorDeadlineRepeatYearsOption(consoleConfig,
                    new OptionInfo("SDry", "select by deadline years", "[match type][match filter type] int"));
            var selectorDeadlineRepeatedOption = new SelectorDeadlineRepeatedOption(consoleConfig,
                    new OptionInfo("SDr", "select by deadline repeat", "[match type][match filter type] bool"));

            // basic commands
            var taskCommitOption = new CommitOption(new OptionInfo("C", "commit buffer changes into files at the end of command execution", ""), consoleConfig);
            var taskFilenameOption = new FilenameOption(new OptionInfo("F", "specify filename for task to be stored in", " filename"), consoleConfig);

            var taskNameOption = new NameOption(new OptionInfo("n", "specify task name", " name"), consoleConfig);
            var taskDescriptionOption = new DescriptionOption(new OptionInfo("d", "specify task description", " description"), consoleConfig);
            var taskTagAddOption = new TagAddOption(new OptionInfo("ta", "add new tag to the task", " tag"), consoleConfig);
            var taskTagRemoveOption = new TagRemoveOption(new OptionInfo("tr", "remove tag from the task", " tag"), consoleConfig);
            var taskChildAddOption = new ChildAddOption(taskBufferManager, new OptionInfo("ca", "add child to the task", " fuzzy_guid"), consoleConfig);
            var taskChildRemoveOption = new ChildRemoveOption(taskBufferManager, new OptionInfo("cr", "remove child from the task", " fuzzy_guid"), consoleConfig);
            var taskParentAddOption = new ParentAddOption(taskBufferManager, new OptionInfo("pa", "add parent to the task", " fuzzy_guid"), consoleConfig);
            var taskParentRemoveOption = new ParentRemoveOption(taskBufferManager, new OptionInfo("pr", "remove parent from the task", " fuzzy_guid"), consoleConfig);

            var taskAddDeadlineOption = new AddDeadlineOption(new OptionInfo("Da", "add new deadline to the task", ""), consoleConfig);
            var taskDeadlineValueOption = new ValueOption(new OptionInfo("Dv", "specify Deadline deadline value", " datetime"), consoleConfig);
            var taskDeadlineWarningOption = new WarningOption(new OptionInfo("Dw", "specify warning time", " timespan"), consoleConfig);
            var taskDeadlineDurationOption = new DurationOption(new OptionInfo("Dd", "specify duration", " timespan"), consoleConfig);
            var taskDeadlineEnabledOption = new EnabledOption(new OptionInfo("De", "specify enabled", " bool"), consoleConfig);
            var taskDeadlineNextAddOption = new NextAddOption(taskBufferManager, new OptionInfo("Dna", "add task to be executed next after deadline completion", " fuzzy_guid"), consoleConfig);
            var taskDeadlineNextRemoveOption = new NextRemoveOption(taskBufferManager, new OptionInfo("Dnr", "remove next task to be executed after pipeline completion", " fuzzy_guid"), consoleConfig);
            var taskDeadlineRemoveDeadlineOption = new RemoveDeadlineOption(new OptionInfo("Drd", "remove deadline from task", " fuzzy_guid"), consoleConfig);
            var taskDeadlineRepeatSpanOption = new RepeatSpanOption(new OptionInfo("Drs", "specify repeat span", " timespan"), consoleConfig);
            var taskDeadlineRepeatMonthsOption = new RepeatMonthsOption(new OptionInfo("Drm", "specify repeat months", " int"), consoleConfig);
            var taskDeadlineRepeatYearsOption = new RepeatYearsOption(new OptionInfo("Dry", "specify repeat years", " int"), consoleConfig);
            var taskDeadlineRepeatedOption = new RepeatedOption(new OptionInfo("Dr", "enable or disable task repetition", " bool"), consoleConfig);

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
                        new AddDirOption(new OptionInfo("a", "add path to directory into lookup paths", " path"), consoleConfig),
                        new RemoveDirOption(new OptionInfo("r", "remove path to directory into lookup paths", " path"), consoleConfig),
                        new ListDirOption(new OptionInfo("l", "list resulted lookup paths", ""), consoleConfig)
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
                        new HelpShowOptionsOption(new OptionInfo("o", "show options", ""), consoleConfig),
                        new HelpOptionLikeOption(new OptionInfo("ol", "filter option by name via string (if contains)", " string"), consoleConfig),
                        new HelpCommandLikeOption(new OptionInfo("cl", "filter commands by name via string (if contains)", " string"), consoleConfig),
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
