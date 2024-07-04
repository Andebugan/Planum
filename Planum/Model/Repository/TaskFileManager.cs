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

    public static class TaskSaveFormat
    {
        public static string[] id = { "i: ", "id: " };
        public static string[] name = { "n: ", "name: " };
        public static string[] description = { "d: ", "description: " };
        public static string[] parent = { "p: ", "parent: " };
        public static string[] children = { "c: ", "children: " };
        public static string[] deadlineStart = { "de: ", "deadline: " };
        public static string[] enabled = { "e", "enabled" };
        public static string[] deadline = { "ded: ", "deadline: " };
        public static string[] warning = { "w: ", "warning: " };
        public static string[] duration = { "du: ", "duration: " };
        public static string[] repeated = { "r ", "repeated" };
        public static string[] span = { "s: ", "span: " };
        public static string[] years = { "y: ", "years: " };
        public static string[] months = { "m: ", "months: " };
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

        /*
         * <planum.task>
         * i[d]: {guid} 
         * n[ame]: {string}
         * d[escription]: {string} 
         * p[arent]: {guid}
         * ...
         * c[hildre]: {guid}
         * ...
         * de[eadline]:
         * - e[nabled]
         * - ded[adline]: {hh:mm dd.mm.yyyy}
         * - w[arning]: {dd.hh.mm}
         * - du[ration]: {dd.hh.mm}
         * - r[peated]
         * - s[pan]: {dd.hh.mm}
         * - y[ears]: {int}
         * - m[onths]: {int}
         * ...
         * <planum.task>
        */

        // NOTE: user can specify several children or parents with one name because of parsing features, but must be carefull to not accidentally add more tasks than needed
        public void ReadReferencePass(string filePath, ref IEnumerable<PlanumTask> tasks, ref Dictionary<Guid, List<string>> children, ref Dictionary<Guid, List<string>> parents)
        {
            foreach (var task in tasks)
            {
                foreach (var child in children[task.Id])
                {
                    IEnumerable<PlanumTask> identifiedTasks = new List<PlanumTask>();
                    TaskValueParser.ParseIdentity(ref identifiedTasks, child, tasks);
                    if (identifiedTasks.Count() == 0)
                    {
                        throw new FileParsingException($"Unable to find child: {child}, for task id:{task.Id.ToString()}, name:{task.Name}, at path: {filePath}");
                    }
                    task.Children.Concat(identifiedTasks.Select(x => x.Id));
                }

                foreach (var parent in parents[task.Id])
                {
                    IEnumerable<PlanumTask> identifiedTasks = new List<PlanumTask>();
                    TaskValueParser.ParseIdentity(ref identifiedTasks, parent, tasks);
                    if (identifiedTasks.Count() == 0)
                    {
                        throw new FileParsingException($"Unable to find parent: {parent}, for task id:{task.Id.ToString()}, name:{task.Name}, at path: {filePath}");
                    }
                    task.Children.Concat(identifiedTasks.Select(x => x.Id));
                }
            }
        }

        public IEnumerable<PlanumTask> ReadMainPass(string filePath, ref Dictionary<Guid, List<string>> children, ref Dictionary<Guid, List<string>> parents)
        {
            List<PlanumTask> tasks = new List<PlanumTask>();
            if (!File.Exists(filePath))
            {
                throw new($"Task file at path {filePath} doesn't exist");
            }

            PlanumTask? task = null;
            IEnumerable<Deadline> deadlines = new List<Deadline>();
            Deadline? deadline = null;
            bool taskZone = false;

            foreach (var line in File.ReadLines(filePath))
            {
                var tmpline = line.TrimStart(new char[] { ' ', '-' }).TrimEnd();
                if (tmpline.StartsWith("<planum.task>"))
                {
                    taskZone = !taskZone;
                }

                if (!taskZone)
                {
                    if (task is not null)
                        tasks.Add(task);
                    else
                        task = new PlanumTask();
                }

                var parsingError = false;

                if (deadline is not null)
                {
                    if (TaskSaveFormat.enabled.Where(x => tmpline.StartsWith(x)).Any())
                        deadline.enabled = true;
                    else if (TaskSaveFormat.deadline.Where(x => tmpline.StartsWith(x)).Any())
                        parsingError = ValueParser.Parse(ref deadline.deadline, tmpline.Remove(0, TaskSaveFormat.deadline.Where(x => tmpline.StartsWith(x)).First().Length));
                    else if (TaskSaveFormat.warning.Where(x => tmpline.StartsWith(x)).Any())
                        parsingError = ValueParser.Parse(ref deadline.warningTime, tmpline.Remove(0, TaskSaveFormat.warning.Where(x => tmpline.StartsWith(x)).First().Length));
                    else if (TaskSaveFormat.duration.Where(x => tmpline.StartsWith(x)).Any())
                        parsingError = ValueParser.Parse(ref deadline.duration, tmpline.Remove(0, TaskSaveFormat.duration.Where(x => tmpline.StartsWith(x)).First().Length));
                    else if (TaskSaveFormat.repeated.Where(x => tmpline.StartsWith(x)).Any())
                        deadline.repeated = true;
                    else if (TaskSaveFormat.span.Where(x => tmpline.StartsWith(x)).Any())
                        parsingError = ValueParser.Parse(ref deadline.repeatSpan, tmpline.Remove(0, TaskSaveFormat.span.Where(x => tmpline.StartsWith(x)).First().Length));
                    else if (TaskSaveFormat.years.Where(x => tmpline.StartsWith(x)).Any())
                        parsingError = ValueParser.Parse(ref deadline.repeatYears, tmpline.Remove(0, TaskSaveFormat.years.Where(x => tmpline.StartsWith(x)).First().Length));
                    else if (TaskSaveFormat.months.Where(x => tmpline.StartsWith(x)).Any())
                        parsingError = ValueParser.Parse(ref deadline.repeatMonths, tmpline.Remove(0, TaskSaveFormat.months.Where(x => tmpline.StartsWith(x)).First().Length));
                    else
                    {
                        deadlines.Append(deadline);
                        deadline = null;
                    }
                }

                if (TaskSaveFormat.id.Where(x => tmpline.StartsWith(x)).Any())
                {
                    if (task is null)
                        throw new FileParsingException($"Unresolved task exception at path: {filePath}, at line: {line}");
                    tmpline = tmpline.Remove(0, TaskSaveFormat.deadline.Where(x => tmpline.StartsWith(x)).First().Length);
                    if (tmpline.Length == 0)
                        task.Id = Guid.NewGuid();
                    else
                    {
                        Guid value = new Guid();
                        parsingError = ValueParser.Parse(ref value, tmpline);
                        task.Id = value;
                    }

                    if (tasks.Exists(x => x.Id == task.Id))
                        throw new FileParsingException($"Found duplicate task ID at path: {filePath}");
                }
                else if (task is null)
                    throw new FileParsingException($"Parsing error at: \"{line}\"");
                else if (TaskSaveFormat.name.Where(x => tmpline.StartsWith(x)).Any())
                    task.Name = tmpline.Remove(0, TaskSaveFormat.name.Where(x => tmpline.StartsWith(x)).First().Length);
                else if (TaskSaveFormat.description.Where(x => tmpline.StartsWith(x)).Any())
                    task.Description = tmpline.Remove(0, TaskSaveFormat.description.Where(x => tmpline.StartsWith(x)).First().Length);
                else if (TaskSaveFormat.children.Where(x => tmpline.StartsWith(x)).Any())
                    children[task.Id].Append(tmpline.Remove(0, TaskSaveFormat.children.Where(x => tmpline.StartsWith(x)).First().Length));
                else if (TaskSaveFormat.parent.Where(x => tmpline.StartsWith(x)).Any())
                    parents[task.Id].Append(tmpline.Remove(0, TaskSaveFormat.parent.Where(x => tmpline.StartsWith(x)).First().Length));
                else if (TaskSaveFormat.deadlineStart.Where(x => tmpline.StartsWith(x)).Any())
                {
                    if (deadline is not null)
                        deadlines.Append(deadline);
                    deadline = new Deadline();
                }

                if (parsingError)
                {
                    throw new FileParsingException($"Unable to parse value at line: {line}\n at path: {filePath}");
                }
            }

            if (task is not null)
            {
                if (deadline is not null)
                    deadlines.Append(deadline);
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
        }

        /*
         * <planum.task>
         * i[d]: {guid} 
         * n[ame]: {string}
         * d[escription]: {string} 
         * p[arent]: {guid}
         * ...
         * c[hildre]: {guid}
         * ...
         * de[adline]:
         * - e[nabled]
         * - ded[adline]: {hh:mm dd.mm.yyyy}
         * - w[arning]: {dd.hh.mm}
         * - du[ration]: {dd.hh.mm}
         * - r[peated] {true\false}
         * - s[pan]: {dd.hh.mm}
         * - y[ears]: {int}
         * - m[onths]: {int}
         * <planum.task>
        */
        // here tasks are from lookup dictionary specificaly for this file
        protected void WriteToFile(string filePath, ref IEnumerable<PlanumTask> tasks)
        {
            if (!File.Exists(filePath))
                throw new FileWritingException($"Unable to open file at path: {filePath}");

            var lines = File.ReadLines(filePath);
            IEnumerable<string> newLines = new List<string>();

            bool taskZone = false;
            string name = string.Empty;
            Guid id = Guid.Empty;

            foreach (var line in lines)
            {
                // find <planum.task> marker
                if (line.StartsWith("<planum.task>"))
                {
                    if (taskZone) 
                    {
                        // if task exists - add it to lines, else throw error
                        if (id == Guid.Empty)
                        {
                            if (tasks.Where(x => x.Name == name).Count() > 1)
                                throw new FileWritingException($"Unable to identify uniqie tasks due to id and name ambiguity at path: {filePath}, line: {line}, task name: {name}");
                            if (tasks.Where(x => x.Name == name).Count() < 1)
                                throw new FileWritingException($"Unable to identify task by id or name at path: {filePath}, line: {line}, task name: {name}");
                            WriteTask(ref newLines, tasks.Where(x => x.Name == name).First());
                        }
                        else 
                            WriteTask(ref newLines, tasks.Where(x => x.Id == id).First());
                    }
                    taskZone = !taskZone;
                }
                else if (taskZone)
                {
                    // find id inside task and find it in task buffer
                    // if id is empty -> new task, search by name, if same names in one file, throw exception (TODO: find better way to track new task positions in file)
                    var tmpline = line.TrimStart(new char[] { ' ', '-' }).TrimEnd();
                    if (TaskSaveFormat.id.Where(x => tmpline.StartsWith(x)).Any())
                    {
                        tmpline = tmpline.Remove(0, TaskSaveFormat.deadline.Where(x => tmpline.StartsWith(x)).First().Length);
                        if (tmpline.Length > 0)
                            if (!ValueParser.Parse(ref id, tmpline))
                                throw new FileWritingException($"Couldn't parse task id at path: {filePath}, line: {line}");
                    }
                    if (TaskSaveFormat.name.Where(x => tmpline.StartsWith(x)).Any())
                        name = tmpline.Remove(0, TaskSaveFormat.name.Where(x => tmpline.StartsWith(x)).First().Length);
                }
                else
                    newLines.Append(line);
            }
        }

        protected void WriteTask(ref IEnumerable<string> lines, PlanumTask task)
        {
            lines.Append("<planum.task>");
            lines.Append(TaskSaveFormat.id.First() + task.Id.ToString());
            if (task.Name != string.Empty)
            {
                lines.Append(TaskSaveFormat.name.First() + task.Name);
            }
            if (task.Description != string.Empty)
            {
                lines.Append(TaskSaveFormat.description.First() + task.Description);
            }
            if (task.Parents.Count() > 0)
            {
                foreach (var parent in task.Parents)
                {
                    lines.Append(TaskSaveFormat.parent.First() + parent.ToString());
                }
            }
            if (task.Children.Count() > 0)
            {
                foreach (var child in task.Children)
                {
                    lines.Append(TaskSaveFormat.children.First() + child.ToString());
                }
            }
            if (task.Deadlines.Count() > 0)
            {
                task.Deadlines.ToList().Sort((x, y) => DateTime.Compare(x.deadline, y.deadline));
                foreach (var deadline in task.Deadlines)
                {
                    lines.Append(TaskSaveFormat.deadline.First());
                    if (deadline.enabled)
                        lines.Append(TaskSaveFormat.enabled.First());

                    lines.Append("- " + TaskSaveFormat.deadline.First() + deadline.deadline.ToString("H:m d.M.yyyy"));
                    if (deadline.warningTime != TimeSpan.Zero)
                        lines.Append("- " + TaskSaveFormat.deadline.First() + deadline.warningTime.ToString(@"d\.h\:m"));
                    if (deadline.duration != TimeSpan.Zero)
                        lines.Append("- " + TaskSaveFormat.duration.First() + deadline.duration.ToString(@"d\.h\:m"));
                    if (deadline.repeated)
                        lines.Append("- " + TaskSaveFormat.repeated.First());
                    if (deadline.repeatSpan != TimeSpan.Zero)
                        lines.Append("- " + TaskSaveFormat.span.First() + deadline.repeatSpan.ToString(@"d\.h\:m"));
                    if (deadline.repeatYears > 0)
                        lines.Append("- " + TaskSaveFormat.years.First() + deadline.repeatYears.ToString());
                    if (deadline.repeatMonths > 0)
                        lines.Append("- " + TaskSaveFormat.months.First() + deadline.repeatMonths.ToString());
                }
            }
            lines.Append("<planum.task>");
        }
    }
}
