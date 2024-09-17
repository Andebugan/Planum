using System.Collections.Generic;
using Planum.Config;
using Planum.Console;
using Planum.Console.Commands;
using Planum.Console.Commands.Selector;
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
            var taskCommitOption = new CommitOption(new OptionInfo("C", "commit buffer changes into files at the end of command execution", ""), commandConfig);
            var taskFilenameOption = new FilenameOption(new OptionInfo("F", "specify filename for task to be stored in", " filename"), commandConfig);

            var taskNameOption = new NameOption(new OptionInfo("n", "specify task name", " name"), commandConfig);
            var taskDescriptionOption = new DescriptionOption(new OptionInfo("d", "specify task description", " description"), commandConfig);
            var taskTagAddOption = new TagAddOption(new OptionInfo("ta", "add new tag to the task", " tag"), commandConfig);
            var taskTagRemoveOption = new TagRemoveOption(new OptionInfo("tr", "remove tag from the task", " tag"), commandConfig);
            var taskChildAddOption = new ChildAddOption(taskBufferManager, new OptionInfo("ca", "add child to the task", " fuzzy_guid"), commandConfig);
            var taskChildRemoveOption = new ChildRemoveOption(taskBufferManager, new OptionInfo("cr", "remove child from the task", " fuzzy_guid"), commandConfig);
            var taskParentAddOption = new ParentAddOption(taskBufferManager, new OptionInfo("pa", "add parent to the task", " fuzzy_guid"), commandConfig);
            var taskParentRemoveOption = new ParentRemoveOption(taskBufferManager, new OptionInfo("pr", "remove parent from the task", " fuzzy_guid"), commandConfig);

            var taskAddDeadlineOption = new AddDeadlineOption(new OptionInfo("Da", "add new deadline to the task", ""), commandConfig);
            var taskDeadlineValueOption = new ValueOption(new OptionInfo("Dv", "specify Deadline deadline value", " datetime"), commandConfig);
            var taskDeadlineWarningOption = new WarningOption(new OptionInfo("Dw", "specify warning time", " timespan"), commandConfig);
            var taskDeadlineDurationOption = new DurationOption(new OptionInfo("Dd", "specify duration", " timespan"), commandConfig);
            var taskDeadlineEnabledOption = new EnabledOption(new OptionInfo("De", "specify enabled", " bool"), commandConfig);
            var taskDeadlineNextAddOption = new NextAddOption(taskBufferManager, new OptionInfo("Dna", "add task to be executed next after deadline completion", " fuzzy_guid"), commandConfig);
            var taskDeadlineNextRemoveOption = new NextRemoveOption(taskBufferManager, new OptionInfo("Dnr", "remove next task to be executed after pipeline completion", " fuzzy_guid"), commandConfig);
            var taskDeadlineRemoveDeadlineOption = new RemoveDeadlineOption(new OptionInfo("Drd", "remove deadline from task", " fuzzy_guid"), commandConfig);
            var taskDeadlineRepeatSpanOption = new RepeatSpanOption(new OptionInfo("Drs", "specify repeat span", " timespan"), commandConfig);
            var taskDeadlineRepeatMonthsOption = new RepeatMonthsOption(new OptionInfo("Drm", "specify repeat months", " int"), commandConfig);
            var taskDeadlineRepeatYearsOption = new RepeatYearsOption(new OptionInfo("Dry", "specify repeat years", " int"), commandConfig);
            var taskDeadlineRepeatedOption = new RepeatedOption(new OptionInfo("Dr", "enable or disable task repetition", " bool"), commandConfig);

            ICommand createCommand = new CreateCommand(taskBufferManager,
                    new CommandInfo("create", "creates new task", "create [options...]"),
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
                    new CommandInfo("update", "update task values", "update [options...]"),
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
                    new CommandInfo("delete", "delete selected tasks", "delete [options...]"),
                    new List<BaseOption<TaskCommandSettings>>()
                    {
                        taskCommitOption
                    },
                    logger);

            // special commands

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

                listCommand
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
