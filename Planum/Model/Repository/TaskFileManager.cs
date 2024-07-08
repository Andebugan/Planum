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

    /*
     * <planum>
     * - [ ] t(ask): {string} [| {guid}] (name or/and guid if name was not provided or name is not uqique)
     * - d(escription): {string}
     * - [ ] p(arent): {guid}
     * ...
     * - [ ] c(hildren): {string} | [guid] (name or/and guid if name was not provided or name is not unique)
     * ...
     * - D(eadline): [e(nabled)][r(epeated)]
     *     - d(adline]: {hh:mm dd.mm.yyyy}
     *     - w(arning]: {dd.hh.mm}
     *     - du(ration]: {dd.hh.mm}
     *     - r(epeat duration): {y m d.hh:mm}
     * ...
     * - [ ] (level 1 checklist)
     *     - n(name)
     *     - d(escription)
     *     - D(eadline)
     *        - ...
     *     - [ ] (level 2 checklist)
     *        - ...
     * ...
     * <- ends with empty line/<planum> for next task after it
    */
    public class TaskFileManager : ITaskFileManager
    {
        RepoConfig repoConfig = new RepoConfig();
        AppConfig appConfig = new AppConfig();

        public TaskFileManager()
        {
            appConfig = ConfigLoader.LoadConfig<AppConfig>(ConfigLoader.AppConfigPath, new AppConfig());
            repoConfig = ConfigLoader.LoadConfig<RepoConfig>(appConfig.RepoConfigPath, new RepoConfig());
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

        public void Clear() => File.WriteAllLines(FilePath, new string[] { "" });
        public void Backup() => File.Copy(FilePath, BackupPath, true);
        public void Restore() => File.Copy(BackupPath, FilePath, true);

        // NOTE: user can specify several children or parents with one name because of parsing features, but must be carefull to not accidentally add more tasks than needed
        public void ReadReferencePass(string path, IEnumerable<PlanumTask> tasks, Dictionary<Guid, List<string>> children, Dictionary<Guid, List<string>> parents)
        {
            foreach (var task in tasks)
            {
                if (children.ContainsKey(task.Id))
                {
                    foreach (var child in children[task.Id])
                    {
                        IEnumerable<PlanumTask> identifiedTasks = TaskValueParser.ParseIdentity(child, child, tasks);
                        if (identifiedTasks.Count() == 0)
                            throw new FileParsingException($"Unable to find child: \"{child}\", for task id:{task.Id.ToString()}, name:{task.Name}, at path: \"{path}\"");
                        task.AddChildren(identifiedTasks.Select(x => x.Id));
                    }
                }


                if (parents.ContainsKey(task.Id))
                {
                    foreach (var parent in parents[task.Id])
                    {
                        IEnumerable<PlanumTask> identifiedTasks = TaskValueParser.ParseIdentity(parent, parent, tasks);
                        if (identifiedTasks.Count() == 0)
                            throw new FileParsingException($"Unable to find parent: \"{parent}\", for task id:{task.Id.ToString()}, name:{task.Name}, at path: \"{path}\"");
                        task.AddParents(identifiedTasks.Select(x => x.Id));
                    }
                }
            }
        }

        public IEnumerable<PlanumTask> ReadMainPass(string path, Dictionary<Guid, List<string>> children, Dictionary<Guid, List<string>> parents)
        {
            List<PlanumTask> tasks = new List<PlanumTask>();
            if (!File.Exists(path))
                throw new($"Task file at path \"{path}\" doesn't exist");

            PlanumTask? task = null;
            HashSet<Deadline> deadlines = new HashSet<Deadline>();
            Deadline? deadline = null;

            foreach (var line in File.ReadAllLines(path))
            {
                var lineHeader = TaskValueHeaderFormat.GetLineHeaderType(line);
                if (lineHeader == HeaderValueTypes.TASK_MARKER)
                {
                    if (task is not null)
                    {
                        task.Deadlines = deadlines.ToHashSet();
                        deadlines.Clear();
                        deadline = null;
                        tasks.Add(task);
                    }
                    else
                    {
                        task = new PlanumTask();
                        task.Id = Guid.NewGuid();
                        children[task.Id] = new List<string>();
                        parents[task.Id] = new List<string>();
                        deadline = null;
                    }
                }
                else if (task is null)
                    continue;
                // ID
                else if (lineHeader == HeaderValueTypes.ID)
                {
                    var valueStr = TaskValueHeaderFormat.GetLineValueType(line, lineHeader);
                    Guid id = new Guid();
                    if (!ValueParser.Parse(ref id, valueStr))
                        throw new FileParsingException($"Can't parse task id at path: \"{path}\", at line: \"{line}\"");
                    task.Id = id;
                    if (tasks.Exists(x => x.Id == task.Id))
                        throw new FileParsingException($"Found duplicate task ID at path: \"{path}\"");
                }
                // Name
                else if (lineHeader == HeaderValueTypes.NAME)
                    task.Name = TaskValueHeaderFormat.GetLineValueType(line, lineHeader);
                // Description
                else if (lineHeader == HeaderValueTypes.DESCRIPTION)
                    task.Description = TaskValueHeaderFormat.GetLineValueType(line, lineHeader);
                // Child
                else if (lineHeader == HeaderValueTypes.CHILD)
                {
                    if (!children.ContainsKey(task.Id))
                        children[task.Id] = new List<string>();
                    children[task.Id].Add(TaskValueHeaderFormat.GetLineValueType(line, lineHeader));
                }
                // Parent
                else if (lineHeader == HeaderValueTypes.PARENT)
                {
                    if (!parents.ContainsKey(task.Id))
                        parents[task.Id] = new List<string>();
                    parents[task.Id].Add(TaskValueHeaderFormat.GetLineValueType(line, lineHeader));
                }
                // Deadline + repeated + enabled
                else if (lineHeader == HeaderValueTypes.DEADLINE_HEAD)
                {
                    if (deadline is not null)
                        deadlines.Add(deadline);
                    deadline = new Deadline();
                    var deadlineStr = TaskValueHeaderFormat.GetLineValueType(line, HeaderValueTypes.DEADLINE_HEAD);
                    if (deadlineStr.Contains('e'))
                        deadline.enabled = true;
                    if (deadlineStr.Contains('r'))
                        deadline.repeated = true;
                }
                // End of task
                else if (lineHeader == HeaderValueTypes.EMPTY)
                {
                    if (deadline is not null)
                        deadlines.Add(deadline);
                    task.Deadlines = deadlines.ToHashSet();
                    deadlines.Clear();
                    tasks.Add(task);
                    deadline = null;
                    task = null;
                }
                // Deadline existence check
                else if (deadline is null)
                    throw new FileParsingException($"Can't parse task value at path: \"{path}\", at line \"{line}\"");
                // Deadline deadline 
                else if (lineHeader == HeaderValueTypes.DEADLINE)
                {
                    var valueStr = TaskValueHeaderFormat.GetLineValueType(line, lineHeader);
                    if (!ValueParser.Parse(ref deadline.deadline, valueStr))
                        throw new FileParsingException($"Can't parse task value at path: \"{path}\", at line \"{line}\"");
                }
                // Deadline warning 
                else if (lineHeader == HeaderValueTypes.WARNING)
                {
                    var valueStr = TaskValueHeaderFormat.GetLineValueType(line, lineHeader);
                    if (!ValueParser.Parse(ref deadline.warningTime, valueStr))
                        throw new FileParsingException($"Can't parse task value at path: \"{path}\", at line \"{line}\"");
                }
                // Deadline duration 
                else if (lineHeader == HeaderValueTypes.DURATION)
                {
                    var valueStr = TaskValueHeaderFormat.GetLineValueType(line, lineHeader);
                    if (!ValueParser.Parse(ref deadline.duration, valueStr))
                        throw new FileParsingException($"Can't parse task value at path: \"{path}\", at line \"{line}\"");
                }
                // Deadline repeat span
                else if (lineHeader == HeaderValueTypes.SPAN)
                {
                    var valueStr = TaskValueHeaderFormat.GetLineValueType(line, lineHeader);
                    if (!ValueParser.Parse(ref deadline.repeatSpan, valueStr))
                        throw new FileParsingException($"Can't parse task value at path: \"{path}\", at line \"{line}\"");
                }
                // Deadline repeat years
                else if (lineHeader == HeaderValueTypes.YEARS)
                {
                    var valueStr = TaskValueHeaderFormat.GetLineValueType(line, lineHeader);
                    if (!ValueParser.Parse(ref deadline.repeatYears, valueStr))
                        throw new FileParsingException($"Can't parse task value at path: \"{path}\", at line \"{line}\"");
                }
                // Deadline months
                else if (lineHeader == HeaderValueTypes.MONTHS)
                {
                    var valueStr = TaskValueHeaderFormat.GetLineValueType(line, lineHeader);
                    if (!ValueParser.Parse(ref deadline.repeatMonths, valueStr))
                        throw new FileParsingException($"Can't parse task value at path: \"{path}\", at line \"{line}\"");
                }
                else if (lineHeader == HeaderValueTypes.UNKNOWN)
                    throw new FileParsingException($"Encountered unknown task value header at path: \"{path}\", at line \"{line}\"");
            }

            if (task is not null)
            {
                if (deadline is not null)
                    deadlines.Add(deadline);
                task.Deadlines = deadlines.ToHashSet();
                tasks.Add(task);
            }

            return tasks;
        }

        public IEnumerable<PlanumTask> Read()
        {
            IEnumerable<PlanumTask> tasks = new List<PlanumTask>();

            Dictionary<Guid, List<string>> children = new Dictionary<Guid, List<string>>();
            Dictionary<Guid, List<string>> parents = new Dictionary<Guid, List<string>>();

            tasks = tasks.Concat(ReadMainPass(FilePath, children, parents));
            List<string> paths = repoConfig.TaskLookupPaths.Keys.ToList();
            foreach (var path in paths)
                tasks = tasks.Concat(ReadMainPass(path, children, parents));

            ReadReferencePass(FilePath, tasks, children, parents);
            foreach (var path in paths)
                ReadReferencePass(path, tasks, children, parents);

            return tasks;
        }

        public void Write(IEnumerable<PlanumTask> tasks)
        {
            if (!tasks.Any())
            {
                File.WriteAllLines(FilePath, new string[] { "" });
                return;
            }

            List<PlanumTask> orphanTasks = new List<PlanumTask>();
            IEnumerable<Guid> lookupTaskIds = new List<Guid>();

            foreach (var ids in repoConfig.TaskLookupPaths.Values)
                lookupTaskIds = lookupTaskIds.Concat(ids);
            orphanTasks = tasks.Where(x => !lookupTaskIds.Contains(x.Id)).ToList();

            foreach (var path in repoConfig.TaskLookupPaths.Keys)
                WriteToFile(path, tasks.Where(x => repoConfig.TaskLookupPaths[path].Contains(x.Id)));
            WriteToFileForce(FilePath, orphanTasks);
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
        // here tasks are from lookup dictionary specificaly for this file
        protected void WriteToFile(string path, IEnumerable<PlanumTask> tasks)
        {
            if (!File.Exists(path))
                throw new FileWritingException($"Unable to open file at path: \"{path}\"");

            var lines = File.ReadAllLines(path);
            var tasksList = tasks.ToList();
            List<string> newLines = new List<string>();
            var orphanTaskList = tasks.ToList();

            string id = string.Empty;
            string name = string.Empty;
            bool isTaskLines = false;

            var lineValueType = HeaderValueTypes.UNKNOWN;

            foreach (var line in lines)
            {
                lineValueType = TaskValueHeaderFormat.GetLineHeaderType(line);
                // Add task to file
                if (isTaskLines && (lineValueType == HeaderValueTypes.EMPTY || lineValueType == ValueHeaderType.TASK_MARKER))
                {
                    var parsedTasks = TaskValueParser.ParseIdentity(id, name, tasks);
                    if (!parsedTasks.Any())
                        throw new FileWritingException($"Unable to find task with id: {id} or name: {name} for file at path: \"{path}\" at line \"{line}\"");
                    if (parsedTasks.Count() > 1)
                        throw new FileWritingException($"Unable to find unique task with id: {id} or name: {name} for file at path: \"{path}\" at line \"{line}\"");
                    newLines = newLines.Concat(WriteTask(parsedTasks.First())).ToList();
                    orphanTaskList.Remove(parsedTasks.First());
                    if (lineValueType != HeaderValueTypes.TASK_MARKER)
                        isTaskLines = false;
                    id = string.Empty;
                    name = string.Empty;
                }
                // Start search for id/name
                else if (lineValueType == HeaderValueTypes.TASK_MARKER)
                {
                    id = string.Empty;
                    name = string.Empty;
                    isTaskLines = true;
                }
                else if (isTaskLines && lineValueType == HeaderValueTypes.ID)
                    id = TaskValueHeaderFormat.GetLineValueType(line, HeaderValueTypes.ID);
                else if (isTaskLines && lineValueType == HeaderValueTypes.NAME)
                    name = TaskValueHeaderFormat.GetLineValueType(line, HeaderValueTypes.NAME);
                else
                    newLines.Add(line);
            }

            // Check isTaskLines, add last task
            if (isTaskLines)
            {
                var parsedTasks = TaskValueParser.ParseIdentity(id, name, tasks);
                if (!parsedTasks.Any())
                    throw new FileWritingException($"Unable to find task with id: {id} or name: {name} for file at path: \"{path}\" at line {lines.Last()}");
                if (parsedTasks.Count() > 1)
                    throw new FileWritingException($"Unable to find unique task with id: {id} or name: {name} for file at path: \"{path}\" at line {lines.Last()}");
                newLines = newLines.Concat(WriteTask(parsedTasks.First())).ToList();
                orphanTaskList.Remove(parsedTasks.First());
                if (lineValueType != HeaderValueTypes.TASK_MARKER)
                    isTaskLines = false;
                id = string.Empty;
                name = string.Empty;
            }

            // add orphan tasks at the start of the file
            var orphanTaskLines = new List<string>();
            foreach (var task in orphanTaskList)
                orphanTaskLines = orphanTaskLines.Concat(WriteTask(task)).ToList();
            newLines = orphanTaskLines.Concat(newLines).ToList();

            File.WriteAllLines(path, newLines);
        }

        protected void WriteToFileForce(string path, IEnumerable<PlanumTask> tasks)
        {
            if (!File.Exists(path))
                throw new FileWritingException($"Unable to open file at path: \"{path}\"");

            var lines = new List<string>();
            foreach (var task in tasks)
                lines = lines.Concat(WriteTask(task)).ToList();

            File.WriteAllLines(path, lines);
        }

        protected IEnumerable<string> WriteTask(PlanumTask task)
        {
            var linesList = new List<string>();
            linesList.Add(TaskValueHeaderFormat.AddLineValueType("", HeaderValueTypes.TASK_MARKER));
            linesList.Add(TaskValueHeaderFormat.AddLineValueType(task.Id.ToString(), HeaderValueTypes.ID));
            if (task.Name != string.Empty)
                linesList.Add(TaskValueHeaderFormat.AddLineValueType(task.Name, HeaderValueTypes.NAME));
            if (task.Description != string.Empty)
                linesList.Add(TaskValueHeaderFormat.AddLineValueType(task.Description, HeaderValueTypes.DESCRIPTION));
            foreach (var parent in task.Parents)
                linesList.Add(TaskValueHeaderFormat.AddLineValueType(parent.ToString(), HeaderValueTypes.PARENT));
            foreach (var child in task.Children)
                linesList.Add(TaskValueHeaderFormat.AddLineValueType(child.ToString(), HeaderValueTypes.CHILD));
            foreach (var deadline in task.Deadlines)
            {
                var deadlineStr = "";
                if (deadline.enabled)
                    deadlineStr += 'e';
                if (deadline.repeated)
                    deadlineStr += 'r';
                linesList.Add(TaskValueHeaderFormat.AddLineValueType(deadlineStr, HeaderValueTypes.DEADLINE_HEAD));
                linesList.Add(TaskValueHeaderFormat.AddLineValueType(deadline.deadline.ToString("H:m d.M.yyyy"), HeaderValueTypes.DEADLINE));
                if (deadline.warningTime != TimeSpan.Zero)
                    linesList.Add(TaskValueHeaderFormat.AddLineValueType(deadline.warningTime.ToString(@"d\.h\:m"), HeaderValueTypes.WARNING));
                if (deadline.duration != TimeSpan.Zero)
                    linesList.Add(TaskValueHeaderFormat.AddLineValueType(deadline.duration.ToString(@"d\.h\:m"), HeaderValueTypes.DURATION));
                if (deadline.repeatSpan != TimeSpan.Zero)
                    linesList.Add(TaskValueHeaderFormat.AddLineValueType(deadline.repeatSpan.ToString(@"d\.h\:m"), HeaderValueTypes.SPAN));
                if (deadline.repeatYears > 0)
                    linesList.Add(TaskValueHeaderFormat.AddLineValueType(deadline.repeatYears.ToString(), HeaderValueTypes.YEARS));
                if (deadline.repeatMonths > 0)
                    linesList.Add(TaskValueHeaderFormat.AddLineValueType(deadline.repeatMonths.ToString(), HeaderValueTypes.MONTHS));
            }
            linesList.Add("");
            return linesList;
        }
    }
}
