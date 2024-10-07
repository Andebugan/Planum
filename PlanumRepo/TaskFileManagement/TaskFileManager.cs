using Planum.Config;
using Planum.Logger;
using Planum.Model;
using Planum.Model.Entities;
using Planum.Model.Exporters;
#nullable enable

namespace Planum.Repository
{
    public class TaskFileManager: ITaskFileManager
    {
        ModelConfig ModelConfig { get; set; }
        RepoConfig RepoConfig { get; set; }
        TaskMarkdownReader TaskReader { get; set; }
        TaskMarkdownExporter TaskExporter { get; set; }
        ILoggerWrapper Logger { get; set; }

        public TaskFileManager(ModelConfig modelConfig, RepoConfig repoConfig, TaskMarkdownReader taskReader, TaskMarkdownExporter taskExporter, ILoggerWrapper logger)
        {
            ModelConfig = modelConfig;
            RepoConfig = repoConfig;
            TaskReader = taskReader;
            TaskExporter = taskExporter;
            Logger = logger;
        }

        protected void ReadFromFile(string path, ref List<PlanumTaskDTO> tasks)
        {
            Logger.Log(message: $"Collecting tasks from file: {path}", LogLevel.INFO);
                
            IEnumerator<string> enumerator = (IEnumerator<string>)(File.ReadAllLines(path).ToList().GetEnumerator());
            try
            {
                while (enumerator.MoveNext())
                    TaskReader.ReadTask(ref enumerator, ref tasks);
            }
            finally
            {
                enumerator.Dispose();
            }

            Logger.Log($"Task read complete", LogLevel.INFO);
        }

        protected HashSet<string> SearchForMarkdownFiles(string startPath, HashSet<string> filePaths)
        {
            Logger.Log("Searching for markdown files", LogLevel.INFO);
            if (File.Exists(startPath))
            {
                filePaths.Add(startPath);
                return filePaths;
            }

            List<DirectoryInfo> directoryQueue = new List<DirectoryInfo>();

            var directoryInfo = new DirectoryInfo(startPath);
            directoryQueue.Add(directoryInfo);
            IEnumerable<DirectoryInfo> levelQueue = directoryInfo.GetDirectories();
            while (levelQueue.Any())
            {
                IEnumerable<DirectoryInfo> newLevelQueue = new DirectoryInfo[] {};
                foreach (var dirInfo in levelQueue)
                {
                    newLevelQueue = newLevelQueue.Concat(dirInfo.GetDirectories());
                    directoryQueue.Add(dirInfo);
                }
                levelQueue = newLevelQueue;
            }

            foreach (var dirInfo in directoryQueue)
            {
                var files = dirInfo.GetFiles().Where(x => x.Extension == ".md");
                foreach (var fileInfo in files)
                    filePaths.Add(fileInfo.FullName);
            }

            Logger.Log("Search complete", LogLevel.INFO);
            return filePaths;
        }

        public IEnumerable<PlanumTask> Read()
        {
            Logger.Log($"Read starting", LogLevel.INFO);

            List<PlanumTaskDTO> taskDTOs = new List<PlanumTaskDTO>();
            HashSet<string> filePaths = new HashSet<string>();

            foreach (var path in RepoConfig.TaskLookupPaths)
            {
                if (!File.Exists(path) && !Directory.Exists(path))
                    throw new TaskRepoException($"Unable to find path of directory: \"{path}\"");
                else
                    filePaths = SearchForMarkdownFiles(path, filePaths);
            }

            foreach (var path in filePaths)
                ReadFromFile(path, ref taskDTOs);
            
            var taskDicts = new Dictionary<Guid, string>();
            foreach (var taskDTO in taskDTOs)
                taskDicts[taskDTO.Id] = taskDTO.Name;
            
            var tasks = taskDTOs.Select(x => x.ToPlanumTask(taskDicts));

            Logger.Log($"Read completed", LogLevel.INFO);
            return tasks;
        }

        protected List<Guid> WriteUpdateToFile(IEnumerable<PlanumTask> tasks, ref IEnumerator<string> enumerator, ref List<string> newLines)
        {
            List<Guid> writtenIds = new List<Guid>();
            IEnumerable<Guid> taskIds = tasks.Select(x => x.Id);

            Logger.Log($"Task update start", LogLevel.INFO);
            while (enumerator.Current != null && enumerator.MoveNext())
            {
                if (enumerator.Current.StartsWith(ModelConfig.TaskMarkerStartSymbol) && enumerator.Current.EndsWith(ModelConfig.TaskMarkerEndSymbol))
                {
                    // parse marker
                    var tmpTasks = new List<PlanumTaskDTO>();
                    var taskId = TaskReader.ReadTask(ref enumerator, ref tmpTasks);
                    
                    if (taskIds.Contains(taskId))
                    {
                        PlanumTask newTask = tasks.Where(x => x.Id == taskId).First();
                        writtenIds.Add(taskId);
                        TaskExporter.WriteTask(newLines, newTask, tasks);
                    }
                }
                else
                    newLines.Add(enumerator.Current);
            }

            Logger.Log($"Task update complete", LogLevel.INFO);
            return writtenIds;
        }

        protected void WriteNewToFile(IEnumerable<PlanumTask> tasks, IEnumerable<Guid> writtenIds, ref IEnumerator<string> enumerator, ref List<string> newLines)
        {
            Logger.Log($"Task insert start", LogLevel.INFO);
            IEnumerable<Guid> taskIds = tasks.Select(x => x.Id);
            var insertedIds = taskIds.Except(writtenIds);
            foreach (var id in insertedIds)
            {
                PlanumTask newTask = tasks.Where(x => x.Id == id).First();
                TaskExporter.WriteTask(newLines, newTask, tasks);
            }
            Logger.Log($"Task insert complete", LogLevel.INFO);
        }

        protected void WriteToFile(string path, IEnumerable<PlanumTask> tasks, ref Dictionary<string, IEnumerable<string>> fileLines)
        {
            Logger.Log($"Writing tasks into file: {path}", LogLevel.INFO);
            if (!File.Exists(path))
                throw new TaskRepoException($"Unable to find task file at path: \"{path}\"");

            IEnumerable<string> lines = File.ReadAllLines(path);
            List<string> newLines = new List<string>();
            IEnumerator<string> enumerator = (IEnumerator<string>)lines.GetEnumerator();

            try
            {
                List<Guid> updatedIds = WriteUpdateToFile(tasks, ref enumerator, ref newLines);
                WriteNewToFile(tasks, updatedIds, ref enumerator, ref newLines);
            }
            finally
            {
                enumerator.Dispose();
            }

            fileLines[path] = newLines;
            Logger.Log($"Write comleted", LogLevel.INFO);
        }

        public void Write(IEnumerable<PlanumTask> tasks)
        {
            Logger.Log($"Write starting", LogLevel.INFO);
            Dictionary<string, IEnumerable<string>> fileLines = new Dictionary<string, IEnumerable<string>>();

            var filepaths = new Dictionary<string, List<Guid>>();
            foreach (var task in tasks)
            {
                if (filepaths.ContainsKey(task.SaveFile))
                    filepaths[task.SaveFile].Add(task.Id);
                else
                    filepaths[task.SaveFile] = new List<Guid>() { task.Id };
            }

            foreach (var filepath in filepaths.Keys)
                WriteToFile(filepath, tasks.Where(x => filepaths[filepath].Contains(x.Id)), ref fileLines);

            foreach (var fpath in fileLines.Keys)
                File.WriteAllLines(fpath, fileLines[fpath]);
            RepoConfig.TaskLookupPaths.Clear();
            RepoConfig.TaskLookupPaths = fileLines.Keys.ToHashSet();
            RepoConfig.Save(Logger);

            Logger.Log($"Write finished", LogLevel.INFO);
        }
    }
}
