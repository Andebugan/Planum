using Planum.Config;
using Planum.Logger;
using Planum.Model.Managers;
using Planum.Repository;

namespace Planum.Console.Commands.Special
{
    public class LoadCommand : BaseCommand<TaskStorageCommandSettings>
    {
        TaskBufferManager TaskBufferManager { get; set; }
        RepoConfig RepoConfig { get; set; }

        public LoadCommand(RepoConfig repoConfig, TaskBufferManager taskBufferManager, CommandInfo commandInfo, List<BaseOption<TaskStorageCommandSettings>> commandOptions, ILoggerWrapper logger) : base(commandInfo, commandOptions, logger)
        {
            TaskBufferManager = taskBufferManager;
            RepoConfig = repoConfig;
        }

        public override List<string> Execute(ref IEnumerator<string> args)
        {
            Logger.Log("Executing load command");
            var lines = new List<string>();

            var commandSettings = new TaskStorageCommandSettings();
            commandSettings.TaskLookupPaths = RepoConfig.TaskLookupPaths;
            if (!ParseSettings(ref args, ref lines, ref commandSettings))
                return lines;

            try
            {
                TaskBufferManager.Load();
            }
            catch (Exception e)
            {
                lines.Add(ConsoleSpecial.AddStyle(e.Message, foregroundColor: ConsoleInfoColors.Error));
            }

            Logger.Log("Successfully executed load command");
            return lines;
        }
    }
}
