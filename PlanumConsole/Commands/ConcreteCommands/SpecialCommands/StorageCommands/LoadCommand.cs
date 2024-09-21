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

        public List<string> Execute(TaskStorageCommandSettings commandSettings)
        {
            Logger.Log("Executing load command without pasring");
            var lines = new List<string>();

            var saveWriteStatus = new WriteStatus();
            var saveReadStatus = new ReadStatus();

            var loadReadStatus = new ReadStatus();
            TaskBufferManager.Load(ref loadReadStatus);
            if (!loadReadStatus.CheckOkStatus())
            {
                foreach (var status in loadReadStatus.ReadStatuses.Where(x => x.Status != TaskReadStatusType.OK))
                {
                    lines.Add(ConsoleSpecial.AddStyle($"Unable to read task on save:", TextStyle.Normal, TextForegroundColor.Red));
                    lines.Add(ConsoleSpecial.AddStyle($"    message: {status.Message}", TextStyle.Normal));
                    string id = status.Victim is null ? "" : status.Victim.Id.ToString();
                    lines.Add(ConsoleSpecial.AddStyle($"    id:      {id}", TextStyle.Normal));
                    string name = status.Victim is null ? "" : status.Victim.Name;
                    lines.Add(ConsoleSpecial.AddStyle($"    name:    {name}", TextStyle.Normal));
                    lines.Add(ConsoleSpecial.AddStyle($"    file:    {status.FilePath}", TextStyle.Normal));
                    lines.Add(ConsoleSpecial.AddStyle($"    line num:{status.LineNumber}", TextStyle.Normal));
                    lines.Add(ConsoleSpecial.AddStyle($"    line:    {status.Line}", TextStyle.Normal));
                }
            }

            Logger.Log("Successfully executed load command without parsing");
            return lines;

        }

        public override List<string> Execute(ref IEnumerator<string> args)
        {
            Logger.Log("Executing load command");
            var lines = new List<string>();

            var commandSettings = new TaskStorageCommandSettings();
            commandSettings.TaskLookupPaths = RepoConfig.TaskLookupPaths;
            if (!ParseSettings(ref args, ref lines, ref commandSettings))
                return lines;

            var saveWriteStatus = new WriteStatus();
            var saveReadStatus = new ReadStatus();

            var loadReadStatus = new ReadStatus();
            TaskBufferManager.Load(ref loadReadStatus);
            if (!loadReadStatus.CheckOkStatus())
            {
                foreach (var status in loadReadStatus.ReadStatuses.Where(x => x.Status != TaskReadStatusType.OK))
                {
                    lines.Add(ConsoleSpecial.AddStyle($"Unable to read task on save:", TextStyle.Normal, TextForegroundColor.Red));
                    lines.Add(ConsoleSpecial.AddStyle($"    message: {status.Message}", TextStyle.Normal));
                    string id = status.Victim is null ? "" : status.Victim.Id.ToString();
                    lines.Add(ConsoleSpecial.AddStyle($"    id:      {id}", TextStyle.Normal));
                    string name = status.Victim is null ? "" : status.Victim.Name;
                    lines.Add(ConsoleSpecial.AddStyle($"    name:    {name}", TextStyle.Normal));
                    lines.Add(ConsoleSpecial.AddStyle($"    file:    {status.FilePath}", TextStyle.Normal));
                    lines.Add(ConsoleSpecial.AddStyle($"    line num:{status.LineNumber}", TextStyle.Normal));
                    lines.Add(ConsoleSpecial.AddStyle($"    line:    {status.Line}", TextStyle.Normal));
                }
            }

            Logger.Log("Successfully executed load command");
            return lines;
        }
    }
}
