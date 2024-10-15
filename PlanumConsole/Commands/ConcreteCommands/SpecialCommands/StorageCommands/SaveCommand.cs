using Planum.Config;
using Planum.Logger;
using Planum.Model.Managers;
using Planum.Repository;

namespace Planum.Console.Commands.Special
{
    public class SaveCommand : BaseCommand<TaskStorageCommandSettings>
    {
        TaskBufferManager TaskBufferManager { get; set; }
        RepoConfig RepoConfig { get; set; }

        public SaveCommand(RepoConfig repoConfig, TaskBufferManager taskBufferManager, CommandInfo commandInfo, List<BaseOption<TaskStorageCommandSettings>> commandOptions, ILoggerWrapper logger) : base(commandInfo, commandOptions, logger)
        {
            TaskBufferManager = taskBufferManager;
            RepoConfig = repoConfig;
        }

        public override List<string> Execute(ref IEnumerator<string> args)
        {
            Logger.Log("Executing save command");
            var lines = new List<string>();

            var commandSettings = new TaskStorageCommandSettings();
            commandSettings.TaskLookupPaths = RepoConfig.TaskLookupPaths;
            if (!ParseSettings(ref args, ref lines, ref commandSettings))
                return lines;

            try
            {
                TaskBufferManager.Save();
            }
            catch (Exception e)
            {
                lines.Add(ConsoleSpecial.AddStyle(e.Message, foregroundColor: ConsoleInfoColors.Error));
            }

            Logger.Log("Successfully executed save command");
            return lines;
        }
    }
}
