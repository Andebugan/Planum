using Planum.Config;
using Planum.Logger;
using Planum.Model.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
#nullable enable

namespace Planum.Repository
{
    public class TaskFileManager: ITaskFileManager
    {
        RepoConfig RepoConfig { get; set; }
        TaskMarkdownWriter PlanumTaskWriter { get; set; }
        TaskMarkdownReader PlanumTaskReader { get; set; }
        ILoggerWrapper Logger { get; set; }

        public TaskFileManager(RepoConfig repoConfig, TaskMarkdownWriter planumTaskWriter, TaskMarkdownReader planumTaskReader, ILoggerWrapper logger)
        {
            RepoConfig = repoConfig;
            PlanumTaskWriter = planumTaskWriter;
            PlanumTaskReader = planumTaskReader;
            Logger = logger;
        }

        protected void ReadFromFile(string path, IList<PlanumTask> tasks, Dictionary<Guid, IList<string>> children, Dictionary<Guid, IList<string>> parents, Dictionary<Guid, IList<string>> next, ref ReadStatus readStatus)
        {
            Logger.Log(LogLevel.INFO, message: $"Reading tasks from file: {path}");
            if (!File.Exists(path))
            {
                Logger.Log(LogLevel.WARN, message: $"Can't read from file, because it does not exist: {path}");
                foreach (var taskId in RepoConfig.TaskLookupPaths[path])
                    readStatus.ReadStatuses.Add(new TaskReadStatus(new PlanumTask(taskId), TaskReadStatusType.UNABLE_TO_FIND_TASK_FILE, path));
                return;
            }
                
            IEnumerator<string> linesEnumerator = (IEnumerator<string>)(File.ReadAllLines(path).ToList().GetEnumerator());
            while (linesEnumerator.MoveNext() && readStatus.CheckOkStatus())
            {
                var readStatuses = readStatus.ReadStatuses;
                var guid = PlanumTaskReader.ReadTask(ref linesEnumerator, ref readStatuses, tasks, children, parents, next);
                readStatus.ReadStatuses = readStatuses;
            }

            linesEnumerator.Dispose();
            Logger.Log(LogLevel.INFO, message: $"Task read complete");
        }

        public IEnumerable<PlanumTask> Read(ref ReadStatus readStatus)
        {
            Logger.Log(LogLevel.INFO, message: $"Read starting, checking paths in task lookup paths");

            IList<PlanumTask> tasks = new List<PlanumTask>();
            Dictionary<Guid, IList<string>> children = new Dictionary<Guid, IList<string>>();
            Dictionary<Guid, IList<string>> parents = new Dictionary<Guid, IList<string>>();
            Dictionary<Guid, IList<string>> next = new Dictionary<Guid, IList<string>>();

            foreach (var path in RepoConfig.TaskLookupPaths.Keys)
                ReadFromFile(path, tasks, children, parents, next, ref readStatus);
            if (readStatus.CheckOkStatus())
                PlanumTaskReader.ParseIdentities(tasks, children, parents, next);

            Logger.Log(LogLevel.INFO, message: $"Read completed, success: {readStatus.CheckOkStatus()}");
            return tasks;
        }

        protected List<Guid> WriteUpdateToFile(IEnumerable<PlanumTask> tasks, ref IEnumerator<string> linesEnumerator, ref List<string> newLines, ref ReadStatus readStatus)
        {
            List<Guid> writtenIds = new List<Guid>();
            IEnumerable<Guid> taskIds = tasks.Select(x => x.Id);

            Logger.Log(LogLevel.INFO, message: $"Task update start");
            while (linesEnumerator.MoveNext() && readStatus.CheckOkStatus())
            {
                if (linesEnumerator.Current.StartsWith(RepoConfig.TaskMarkerStartSymbol) && linesEnumerator.Current.EndsWith(RepoConfig.TaskMarkerEndSymbol))
                {
                    var readStatuses = readStatus.ReadStatuses;
                    var taskId = PlanumTaskReader.ReadSkipTask(ref linesEnumerator, ref readStatuses);
                    readStatus.ReadStatuses = readStatuses;

                    if (taskIds.Contains(taskId))
                    {
                        PlanumTask newTask = tasks.Where(x => x.Id == taskId).First();
                        writtenIds.Add(taskId);
                        PlanumTaskWriter.WriteTask(newLines, newTask, tasks);
                    }
                }
                else
                    newLines.Add(linesEnumerator.Current);
            }

            Logger.Log(LogLevel.INFO, message: $"Task update complete");
            return writtenIds;
        }

        protected void WriteNewToFile(IEnumerable<PlanumTask> tasks, IEnumerable<Guid> writtenIds, ref IEnumerator<string> linesEnumerator, ref List<string> newLines)
        {
            Logger.Log(LogLevel.INFO, message: $"Task insert start");
            IEnumerable<Guid> taskIds = tasks.Select(x => x.Id);
            var insertedIds = taskIds.Except(writtenIds);
            foreach (var id in insertedIds)
            {
                PlanumTask newTask = tasks.Where(x => x.Id == id).First();
                PlanumTaskWriter.WriteTask(newLines, newTask, tasks);
            }
            Logger.Log(LogLevel.INFO, message: $"Task insert complete");
        }

        protected void WriteToFile(string path, IEnumerable<PlanumTask> tasks, ref WriteStatus writeStatus, ref ReadStatus readStatus, ref Dictionary<string, IEnumerable<string>> fileLines)
        {
            Logger.Log(LogLevel.INFO, message: $"Writing tasks into file: {path}");
            if (!File.Exists(path))
            {
                foreach (var task in tasks)
                    writeStatus.WriteStatuses.Add(new TaskWriteStatus(task, TaskWriteStatusType.UNABLE_TO_FIND_TASK_FILE, path));
                return;
            }

            IEnumerable<string> lines = File.ReadAllLines(path);
            List<string> newLines = new List<string>();
            IEnumerator<string> linesEnumerator = (IEnumerator<string>)lines.GetEnumerator();

            List<Guid> updatedIds = WriteUpdateToFile(tasks, ref linesEnumerator, ref newLines, ref readStatus);
            WriteNewToFile(tasks, updatedIds, ref linesEnumerator, ref newLines);
            linesEnumerator.Dispose();

            RepoConfig.TaskLookupPaths[path] = tasks.Select(x => x.Id).ToHashSet();

            if (writeStatus.CheckOkStatus())
                fileLines[path] = newLines;
            Logger.Log(LogLevel.INFO, message: $"Write comleted, success: {writeStatus.CheckOkStatus() && readStatus.CheckOkStatus()}");
        }

        public void Write(IEnumerable<PlanumTask> tasks, ref WriteStatus writeStatus, ref ReadStatus readStatus)
        {
            Logger.Log(LogLevel.INFO, message: $"Write starting");
            Dictionary<string, IEnumerable<string>> fileLines = new Dictionary<string, IEnumerable<string>>();
            foreach (var filepath in RepoConfig.TaskLookupPaths.Keys)
                WriteToFile(filepath, tasks.Where(x => RepoConfig.TaskLookupPaths[filepath].Contains(x.Id)), ref writeStatus, ref readStatus, ref fileLines);

            if (writeStatus.CheckOkStatus())
            {
                foreach (var fpath in fileLines.Keys)
                    File.WriteAllLines(fpath, fileLines[fpath]);
                RepoConfig.Save(Logger);
            }

            Logger.Log(LogLevel.INFO, message: $"Write finished");
        }
    }
}
