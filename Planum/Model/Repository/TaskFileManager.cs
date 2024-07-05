using Planum.Config;
using Planum.Model.Entities;
using Planum.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
#nullable enable

namespace Planum.Model.Repository
{
    public class FileParsingException : Exception
    {
        public FileParsingException(string message) : base(message) { }
    }

    public class FileWritingException : Exception
    {
        public FileWritingException(string message) : base(message) { }
    }

    public enum ValueHeaderType
    {
        EMPTY,
        UNKNOWN,
        TASK_MARKER,
        ID,
        NAME,
        DESCRIPTION,
        PARENT,
        CHILD,
        DEADLINE_HEAD,
        ENABLED,
        DEADLINE,
        WARNING,
        DURATION,
        REPEATED,
        SPAN,
        YEARS,
        MONTHS
    }

    public static class TaskValueHeaderFormat
    {
        public static string taskItemSymbol = "- ";
        public static string taskItemTab = "  ";

        public static Dictionary<string, ValueHeaderType> stringToValueTypeDict = new Dictionary<string, ValueHeaderType>()
        {
            // task marker
            { "<planum>", ValueHeaderType.TASK_MARKER },
            // id
            { taskItemSymbol + "i:", ValueHeaderType.ID },
            // name
            { taskItemSymbol + "n:", ValueHeaderType.NAME },
            // description 
            { taskItemSymbol + "d:", ValueHeaderType.DESCRIPTION },
            // parent
            { taskItemSymbol + "p:", ValueHeaderType.PARENT },
            // children
            { taskItemSymbol + "c:", ValueHeaderType.CHILD },
            // deadline header
            { taskItemSymbol + "D:", ValueHeaderType.DEADLINE_HEAD },
            // enabled
            { taskItemTab + taskItemSymbol + "e:", ValueHeaderType.ENABLED },
            // deadline
            { taskItemTab + taskItemSymbol + "d:", ValueHeaderType.DEADLINE },
            // warning
            { taskItemTab + taskItemSymbol + "w:", ValueHeaderType.WARNING },
            // duration
            { taskItemTab + taskItemSymbol + "du:", ValueHeaderType.DURATION },
            // repeated
            { taskItemTab + taskItemSymbol + "r:", ValueHeaderType.REPEATED },
            // repeat span
            { taskItemTab + taskItemSymbol + "s:", ValueHeaderType.SPAN },
            // repeat years
            { taskItemTab + taskItemSymbol + "y:", ValueHeaderType.YEARS },
            // repeat months
            { taskItemTab + taskItemSymbol + "m:", ValueHeaderType.MONTHS },
        };

        public static Dictionary<ValueHeaderType, string> valueTypeToStringDict = new Dictionary<ValueHeaderType, string>()
        {
            // task marker
            { ValueHeaderType.TASK_MARKER, "<planum>" },
            // id
            { ValueHeaderType.ID, taskItemSymbol + "i:" },
            // name
            { ValueHeaderType.NAME, taskItemSymbol + "n:" },
            // description 
            { ValueHeaderType.DESCRIPTION, taskItemSymbol + "d:" },
            // parent
            { ValueHeaderType.PARENT, taskItemSymbol + "p:" },
            // children
            { ValueHeaderType.CHILD, taskItemSymbol + "c:" },
            // deadline header
            { ValueHeaderType.DEADLINE_HEAD, taskItemSymbol + "D:" },
            // enabled
            { ValueHeaderType.ENABLED, taskItemTab + taskItemSymbol + "e:" },
            // deadline
            { ValueHeaderType.DEADLINE, taskItemTab + taskItemSymbol + "d:" },
            // warning
            { ValueHeaderType.WARNING, taskItemTab + taskItemSymbol + "w:" },
            // duration
            { ValueHeaderType.DURATION, taskItemTab + taskItemSymbol + "du:" },
            // repeated
            { ValueHeaderType.REPEATED, taskItemTab + taskItemSymbol + "r:" },
            // repeat span
            { ValueHeaderType.SPAN, taskItemTab + taskItemSymbol + "s:" },
            // repeat years
            { ValueHeaderType.YEARS, taskItemTab + taskItemSymbol + "y:" },
            // repeat months
            { ValueHeaderType.MONTHS, taskItemTab + taskItemSymbol + "m:" }
        };

        public static ValueHeaderType GetLineHeaderType(string line)
        {
            if (line.Trim().Replace("\n", "").Length == 0)
                return ValueHeaderType.EMPTY;
            var tmpline = line.TrimStart(new char[] { ' ', '-' }).TrimEnd();
            var formatMatches = stringToValueTypeDict.Keys.ToList().Where(x => tmpline.StartsWith(x));
            if (formatMatches.Any())
                return stringToValueTypeDict[formatMatches.First()];
            else
                return ValueHeaderType.UNKNOWN;
        }

        public static string GetLineValueType(string line, ValueHeaderType valueHeaderType)
        {
            if (valueHeaderType == ValueHeaderType.UNKNOWN || valueHeaderType == ValueHeaderType.EMPTY || valueHeaderType == ValueHeaderType.TASK_MARKER)
                throw new Exception("Incorrect task value header type, can't get value from string");
            return line.Remove(0, valueTypeToStringDict[valueHeaderType].Length).Trim();
        }

        public static string AddLineValueType(string line, ValueHeaderType valueHeaderType)
        {
            if (valueHeaderType == ValueHeaderType.UNKNOWN || valueHeaderType == ValueHeaderType.EMPTY)
                throw new Exception("Incorrect task value header type, can't add value type to string");
            return valueTypeToStringDict[valueHeaderType] + " " + line;
        }
    }

    public class TaskFileManager : ITaskFileManager
    {
        RepoConfig repoConfig = new RepoConfig();
        AppConfig appConfig = new AppConfig();

        public TaskFileManager()
        {
            appConfig = ConfigLoader.LoadConfig<AppConfig>(ConfigLoader.AppConfigPath, appConfig);
            repoConfig = ConfigLoader.LoadConfig<RepoConfig>(appConfig.RepoConfigPath, repoConfig);
            CreateTaskFiles();
        }

        string filePath = string.Empty;
        public string FilePath { get => filePath; }
        string backupPath = string.Empty;
        public string BackupPath { get => backupPath; }

        protected void GetSavePath()
        {
            string savePath = AppContext.BaseDirectory;
            filePath = Path.Combine(savePath, repoConfig.TaskFilename);
        }

        protected void GetBackupPath()
        {
            string savePath = AppContext.BaseDirectory;
            backupPath = Path.Combine(savePath, repoConfig.TaskBackupFilename);
        }

        public void CreateTaskFiles()
        {
            GetBackupPath();
            GetSavePath();
            if (!File.Exists(FilePath))
                File.Create(FilePath).Close();
            if (!File.Exists(BackupPath))
                File.Create(BackupPath).Close();
        }

        public void Clear()
        {
            File.WriteAllLines(FilePath, new string[] { });
        }

        public void Backup()
        {
            // copy main file to backup file
            File.Copy(FilePath, BackupPath, true);
        }

        public void Restore()
        {
            // copy backup file to main file
            File.Copy(BackupPath, FilePath, true);
        }

        // NOTE: user can specify several children or parents with one name because of parsing features, but must be carefull to not accidentally add more tasks than needed
        public void ReadReferencePass(string path, ref IEnumerable<PlanumTask> tasks, ref Dictionary<Guid, List<string>> children, ref Dictionary<Guid, List<string>> parents)
        {
            foreach (var task in tasks)
            {
                foreach (var child in children[task.Id])
                {
                    IEnumerable<PlanumTask> identifiedTasks = TaskValueParser.ParseIdentity(child, child, tasks);
                    if (identifiedTasks.Count() == 0)
                        throw new FileParsingException($"Unable to find child: {child}, for task id:{task.Id.ToString()}, name:{task.Name}, at path: {path}");
                    task.Children.Concat(identifiedTasks.Select(x => x.Id));
                }

                foreach (var parent in parents[task.Id])
                {
                    IEnumerable<PlanumTask> identifiedTasks = TaskValueParser.ParseIdentity(parent, parent, tasks);
                    if (identifiedTasks.Count() == 0)
                        throw new FileParsingException($"Unable to find parent: {parent}, for task id:{task.Id.ToString()}, name:{task.Name}, at path: {path}");
                    task.Children.Concat(identifiedTasks.Select(x => x.Id));
                }
            }
        }

        /*
         * <planum>
         * - i[d]: {guid}
         * - n[ame]: {string}
         * - d[escription]: {string}
         * - p[arent]: {guid}
         * ...
         * - c[hildre]: {guid}
         * ...
         * - de[adline]:
         *     - e[nabled]
         *     - ded[adline]: {hh:mm dd.mm.yyyy}
         *     - w[arning]: {dd.hh.mm}
         *     - du[ration]: {dd.hh.mm}
         *     - r[peated] {true\false}
         *     - s[pan]: {dd.hh.mm}
         *     - y[ears]: {int}
         *     - m[onths]: {int}
         * <- ends with empty line/<planum> for next task after it
        */
        public IEnumerable<PlanumTask> ReadMainPass(string path, ref Dictionary<Guid, List<string>> children, ref Dictionary<Guid, List<string>> parents)
        {
            List<PlanumTask> tasks = new List<PlanumTask>();
            if (!File.Exists(path))
                throw new($"Task file at path {path} doesn't exist");

            PlanumTask? task = null;
            List<Deadline> deadlines = new List<Deadline>();
            Deadline? deadline = null;

            foreach (var line in File.ReadAllLines(path))
            {
                var lineHeader = TaskValueHeaderFormat.GetLineHeaderType(line);
                if (lineHeader == ValueHeaderType.TASK_MARKER)
                {
                    if (task is not null)
                    {
                        task.Deadlines = deadlines.ToList();
                        deadlines.Clear();
                        tasks.Add(task);
                    }
                    else
                    {
                        task = new PlanumTask();
                        task.Id = Guid.NewGuid();
                        children[task.Id] = new List<string>();
                        parents[task.Id] = new List<string>();
                    }
                    continue;
                }

                if (task is null)
                    continue;

                // ID
                if (lineHeader == ValueHeaderType.ID)
                {
                    var valueStr = TaskValueHeaderFormat.GetLineValueType(line, lineHeader);
                    Guid id = new Guid();
                    if (!ValueParser.Parse(ref id, valueStr))
                        throw new FileParsingException($"Can't parse task id at path: {path}, at line: {line}");
                    task.Id = id;
                    if (tasks.Exists(x => x.Id == task.Id))
                        throw new FileParsingException($"Found duplicate task ID at path: {path}");
                }
                // Name
                else if (lineHeader == ValueHeaderType.NAME)
                    task.Name = TaskValueHeaderFormat.GetLineValueType(line, lineHeader);
                // Description
                else if (lineHeader == ValueHeaderType.DESCRIPTION)
                    task.Description = TaskValueHeaderFormat.GetLineValueType(line, lineHeader);
                // Child
                else if (lineHeader == ValueHeaderType.CHILD)
                    children[task.Id].Add(TaskValueHeaderFormat.GetLineValueType(line, lineHeader));
                // Parent
                else if (lineHeader == ValueHeaderType.PARENT)
                    parents[task.Id].Add(TaskValueHeaderFormat.GetLineValueType(line, lineHeader));
                // Deadline
                else if (lineHeader == ValueHeaderType.DEADLINE_HEAD)
                {
                    if (deadline is not null)
                        deadlines.Add(deadline);
                    deadline = new Deadline();
                }
                // Deadline existence check
                else if (deadline is null)
                    throw new FileParsingException($"Can't parse task value at path: {path}, at line {line}");
                // Deadline enabled
                else if (lineHeader == ValueHeaderType.ENABLED)
                    deadline.enabled = true;
                // Deadline deadline 
                else if (lineHeader == ValueHeaderType.DEADLINE)
                {
                    var valueStr = TaskValueHeaderFormat.GetLineValueType(line, lineHeader);
                    if (!ValueParser.Parse(ref deadline.enabled, valueStr))
                        throw new FileParsingException($"Can't parse task value at path: {path}, at line {line}");
                }
                // Deadline warning 
                else if (lineHeader == ValueHeaderType.WARNING)
                {
                    var valueStr = TaskValueHeaderFormat.GetLineValueType(line, lineHeader);
                    if (!ValueParser.Parse(ref deadline.enabled, valueStr))
                        throw new FileParsingException($"Can't parse task value at path: {path}, at line {line}");
                }
                // Deadline duration 
                else if (lineHeader == ValueHeaderType.DURATION)
                {
                    var valueStr = TaskValueHeaderFormat.GetLineValueType(line, lineHeader);
                    if (!ValueParser.Parse(ref deadline.enabled, valueStr))
                        throw new FileParsingException($"Can't parse task value at path: {path}, at line {line}");
                }
                // Deadline repeated 
                else if (lineHeader == ValueHeaderType.REPEATED)
                    deadline.repeated = true;
                // Deadline repeat span
                else if (lineHeader == ValueHeaderType.SPAN)
                {
                    var valueStr = TaskValueHeaderFormat.GetLineValueType(line, lineHeader);
                    if (!ValueParser.Parse(ref deadline.enabled, valueStr))
                        throw new FileParsingException($"Can't parse task value at path: {path}, at line {line}");
                }
                // Deadline repeat years
                else if (lineHeader == ValueHeaderType.YEARS)
                {
                    var valueStr = TaskValueHeaderFormat.GetLineValueType(line, lineHeader);
                    if (!ValueParser.Parse(ref deadline.enabled, valueStr))
                        throw new FileParsingException($"Can't parse task value at path: {path}, at line {line}");
                }
                // Deadline months
                else if (lineHeader == ValueHeaderType.MONTHS)
                {
                    var valueStr = TaskValueHeaderFormat.GetLineValueType(line, lineHeader);
                    if (!ValueParser.Parse(ref deadline.enabled, valueStr))
                        throw new FileParsingException($"Can't parse task value at path: {path}, at line {line}");
                }
                // End of task
                else if (lineHeader == ValueHeaderType.EMPTY)
                {
                    if (deadline is not null)
                        deadlines.Add(deadline);
                    task.Deadlines = deadlines.ToList();
                    deadlines.Clear();
                    tasks.Add(task);
                }
                else if (lineHeader == ValueHeaderType.UNKNOWN)
                    throw new FileParsingException($"Encountered unknown task value header at path: {path}, at line {line}");
            }

            if (task is not null)
            {
                if (deadline is not null)
                    deadlines.Add(deadline);
                task.Deadlines = deadlines.ToList();
                tasks.Add(task);
            }

            return tasks;
        }

        // TODO: add config update for new tasks (id get's added to lookup paths)
        public IEnumerable<PlanumTask> Read()
        {
            IEnumerable<PlanumTask> tasks = new List<PlanumTask>();

            Dictionary<Guid, List<string>> children = new Dictionary<Guid, List<string>>();
            Dictionary<Guid, List<string>> parents = new Dictionary<Guid, List<string>>();

            tasks.Concat(ReadMainPass(FilePath, ref children, ref parents));
            foreach (var id in repoConfig.TaskLookupPaths.Keys)
            {
                // parse markdown files at path
                DirectoryInfo dir = new DirectoryInfo(repoConfig.TaskLookupPaths[id]);
                foreach (var file in dir.GetFiles(repoConfig.TaskFileSearchPattern))
                {
                    var path = Path.Join(repoConfig.TaskLookupPaths[id], file.Name);
                    tasks.Concat(ReadMainPass(path, ref children, ref parents));
                }
            }

            ReadReferencePass(FilePath, ref tasks, ref children, ref parents);
            foreach (var id in repoConfig.TaskLookupPaths.Keys)
            {
                // parse markdown files at path
                DirectoryInfo dir = new DirectoryInfo(repoConfig.TaskLookupPaths[id]);
                foreach (var file in dir.GetFiles(repoConfig.TaskFileSearchPattern))
                {
                    var path = Path.Join(repoConfig.TaskLookupPaths[id], file.Name);
                    ReadReferencePass(repoConfig.TaskLookupPaths[id], ref tasks, ref children, ref parents);
                }
            }

            return tasks;
        }

        public void Write(IEnumerable<PlanumTask> tasks)
        {
            Dictionary<string, List<PlanumTask>> taskPaths = new Dictionary<string, List<PlanumTask>>();
            List<PlanumTask> orphanTasks = new List<PlanumTask>();

            foreach (var task in tasks)
            {
                if (!repoConfig.TaskLookupPaths.ContainsKey(task.Id))
                    orphanTasks.Add(task);
                else
                {
                    if (!taskPaths.ContainsKey(repoConfig.TaskLookupPaths[task.Id]))
                        taskPaths[repoConfig.TaskLookupPaths[task.Id]] = new List<PlanumTask>();
                    taskPaths[repoConfig.TaskLookupPaths[task.Id]].Add(task);
                }
            }

            foreach (var path in taskPaths.Keys)
                orphanTasks = orphanTasks.Concat(WriteToFile(path, taskPaths[path])).ToList();
            WriteToFile(FilePath, orphanTasks);
        }

        /*
         * <planum> 1st
         * i[d]: {guid} 2nd
         * n[ame]: {string} any order
         * d[escription]: {string} any order
         * p[arent]: {guid} any order
         * ...
         * c[hildre]: {guid}  any order
         * ...
         * de[adline]: (all the items below must be after deadline)
         * - e[nabled]
         * - ded[adline]: {hh:mm dd.mm.yyyy}
         * - w[arning]: {dd.hh.mm}
         * - du[ration]: {dd.hh.mm}
         * - r[peated] {true\false}
         * - s[pan]: {dd.hh.mm}
         * - y[ears]: {int}
         * - m[onths]: {int}
         * <- ends with empty line/<planum> for next task after it
        */
        // here tasks are from lookup dictionary specificaly for this file
        protected List<PlanumTask> WriteToFile(string path, IEnumerable<PlanumTask> tasks)
        {
            if (!File.Exists(path))
                throw new FileWritingException($"Unable to open file at path: {path}");

            var lines = File.ReadAllLines(path);
            var tasksList = tasks.ToList();
            List<string> newLines = new List<string>();
            var orphanTaskList = new List<PlanumTask>();

            string id = string.Empty;
            string name = string.Empty;
            bool isTaskLines = false;

            var lineValueType = ValueHeaderType.UNKNOWN;

            foreach (var line in lines)
            {
                lineValueType = TaskValueHeaderFormat.GetLineHeaderType(line);
                // Add task to file
                if (isTaskLines && (lineValueType == ValueHeaderType.EMPTY || lineValueType == ValueHeaderType.TASK_MARKER))
                {
                    var parsedTasks = TaskValueParser.ParseIdentity(id, name, tasks);
                    if (!parsedTasks.Any())
                        throw new FileWritingException($"Unable to find task with id: {id} or name: {name} for file at path: {path} at line {line}");
                    if (parsedTasks.Count() > 1)
                        throw new FileWritingException($"Unable to find unique task with id: {id} or name: {name} for file at path: {path} at line {line}");
                    newLines = newLines.Concat(WriteTask(parsedTasks.First())).ToList();
                    if (lineValueType != ValueHeaderType.TASK_MARKER)
                        isTaskLines = false;
                    id = string.Empty;
                    name = string.Empty;
                }
                // Start search for id/name
                else if (lineValueType == ValueHeaderType.TASK_MARKER)
                {
                    id = string.Empty;
                    name = string.Empty;
                    isTaskLines = true;
                }
                else if (isTaskLines && lineValueType == ValueHeaderType.ID)
                    id = TaskValueHeaderFormat.GetLineValueType(line, ValueHeaderType.ID);
                else if (isTaskLines && lineValueType == ValueHeaderType.NAME)
                    name = TaskValueHeaderFormat.GetLineValueType(line, ValueHeaderType.NAME);
                else
                    newLines.Add(line);
            }

            // Check isTaskLines, add last task
            if (isTaskLines)
            {
                var parsedTasks = TaskValueParser.ParseIdentity(id, name, tasks);
                if (!parsedTasks.Any())
                    throw new FileWritingException($"Unable to find task with id: {id} or name: {name} for file at path: {path} at line {lines.Last()}");
                if (parsedTasks.Count() > 1)
                    throw new FileWritingException($"Unable to find unique task with id: {id} or name: {name} for file at path: {path} at line {lines.Last()}");
                newLines = newLines.Concat(WriteTask(parsedTasks.First())).ToList();
                if (lineValueType != ValueHeaderType.TASK_MARKER)
                    isTaskLines = false;
                id = string.Empty;
                name = string.Empty;
            }

            File.WriteAllLines(path, newLines);

            return orphanTaskList;
        }

        protected IEnumerable<string> WriteTask(PlanumTask task)
        {
            var linesList = new List<string>();
            linesList.Add(TaskValueHeaderFormat.AddLineValueType("", ValueHeaderType.TASK_MARKER));
            linesList.Add(TaskValueHeaderFormat.AddLineValueType(task.Id.ToString(), ValueHeaderType.ID));
            if (task.Name != string.Empty)
                linesList.Add(TaskValueHeaderFormat.AddLineValueType(task.Name, ValueHeaderType.NAME));
            if (task.Description != string.Empty)
                linesList.Add(TaskValueHeaderFormat.AddLineValueType(task.Description, ValueHeaderType.DESCRIPTION));
            foreach (var parent in task.Parents)
                linesList.Add(TaskValueHeaderFormat.AddLineValueType(parent.ToString(), ValueHeaderType.PARENT));
            foreach (var child in task.Children)
                linesList.Add(TaskValueHeaderFormat.AddLineValueType(child.ToString(), ValueHeaderType.CHILD));
            task.Deadlines.ToList().Sort((x, y) => DateTime.Compare(x.deadline, y.deadline));
            foreach (var deadline in task.Deadlines)
            {
                linesList.Add(TaskValueHeaderFormat.AddLineValueType("", ValueHeaderType.DEADLINE_HEAD));
                if (deadline.enabled)
                    linesList.Add(TaskValueHeaderFormat.AddLineValueType("", ValueHeaderType.ENABLED));
                linesList.Add(TaskValueHeaderFormat.AddLineValueType(deadline.deadline.ToString("H:m d.M.yyyy"), ValueHeaderType.DEADLINE));
                if (deadline.warningTime != TimeSpan.Zero)
                    linesList.Add(TaskValueHeaderFormat.AddLineValueType(deadline.warningTime.ToString(@"d\.h\:m"), ValueHeaderType.WARNING));
                if (deadline.duration != TimeSpan.Zero)
                    linesList.Add(TaskValueHeaderFormat.AddLineValueType(deadline.duration.ToString(@"d\.h\:m"), ValueHeaderType.DURATION));
                if (deadline.repeated)
                    linesList.Add(TaskValueHeaderFormat.AddLineValueType("", ValueHeaderType.REPEATED));
                if (deadline.repeatSpan != TimeSpan.Zero)
                    linesList.Add(TaskValueHeaderFormat.AddLineValueType(deadline.repeatSpan.ToString(@"d\.h\:m"), ValueHeaderType.SPAN));
                if (deadline.repeatYears > 0)
                    linesList.Add(TaskValueHeaderFormat.AddLineValueType(deadline.repeatYears.ToString(), ValueHeaderType.REPEATED));
                if (deadline.repeatMonths > 0)
                    linesList.Add(TaskValueHeaderFormat.AddLineValueType(deadline.repeatMonths.ToString(), ValueHeaderType.REPEATED));
            }
            linesList.Add("");
            return linesList;
        }
    }
}
