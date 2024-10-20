using Planum.Config;
using Planum.Logger;
using Planum.Model.Managers;

namespace Planum.Console.Commands.Special
{
    public class DirCommand : BaseCommand<TaskStorageCommandSettings>
    {
        TaskBufferManager TaskBufferManager { get; set; }
        RepoConfig RepoConfig { get; set; }

        public DirCommand(RepoConfig repoConfig, TaskBufferManager taskBufferManager, CommandInfo commandInfo, List<BaseOption<TaskStorageCommandSettings>> commandOptions, ILoggerWrapper logger) : base(commandInfo, commandOptions, logger)
        {
            TaskBufferManager = taskBufferManager;
            RepoConfig = repoConfig;
        }

        public override List<string> Execute(ref IEnumerator<string> args)
        {
            Logger.Log("Executing dir command");
            var lines = new List<string>();

            var commandSettings = new TaskStorageCommandSettings();
            commandSettings.TaskLookupPaths = RepoConfig.TaskLookupPaths;
            if (!ParseSettings(ref args, ref lines, ref commandSettings))
                return lines;

            RepoConfig.TaskLookupPaths = commandSettings.TaskLookupPaths;
            RepoConfig.Save(Logger);

            if (commandSettings.ListResult)
            {
                if (commandSettings.TaskLookupPaths.Any())
                {
                    lines.Add(ConsoleSpecial.AddStyle("Watched directories (recursive)", TextStyle.Bold, TextForegroundColor.Cyan));
                    foreach (var path in commandSettings.TaskLookupPaths)
                        lines.Add(ConsoleSpecial.AddStyle($"- {path}"));
                }
                else
                    lines.Add(ConsoleSpecial.AddStyle("No directories are watched", TextStyle.Bold, TextForegroundColor.Cyan));
            }

            Logger.Log("Successfully executed dir command");
            return lines;
        }
    }
}
