using Planum.Config;
using Planum.Model.Entities;
using Planum.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Planum.Model.Repository
{
    public class FileParsingException : Exception
    {
        public FileParsingException(string message) : base(message) { }
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

        public TaskFileManager()
        {
            repoConfig = ConfigLoader.LoadConfig<RepoConfig>(ConfigLoader.repoConfigPath);
            CreateTaskFiles();
        }

        string FilePath { get; set; } = string.Empty;
        string BackupPath { get; set; } = string.Empty;

        protected void GetSavePath()
        {
            string dirName = repoConfig.TaskDirectoryName;
            string savePath = AppContext.BaseDirectory;

            string dirPath = Path.Combine(savePath, dirName);
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            FilePath = dirPath + "tasks";
        }

        protected void GetBackupPath()
        {
            string dirName = repoConfig.TaskBackupDirectoryName;
            string savePath = AppContext.BaseDirectory;

            string dirPath = Path.Combine(savePath, dirName);
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            BackupPath = dirPath + "backup";
        }

        public void CreateTaskFiles()
        {
            if (!File.Exists(FilePath))
                File.Create(FilePath);
            if (!File.Exists(BackupPath))
                File.Create(BackupPath);
        }

        public void Backup()
        {
            // copy main file to backup file
            File.Copy(FilePath, BackupPath);
        }

        public void Restore()
        {
            // copy backup file to main file
            File.Copy(BackupPath, FilePath);
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
         * 
        */

        // NOTE: user can specify several children or parents with one name because of parsing features, but must be carefull to not accidentally add more tasks than needed
        public void ReadReferencePass(string filePath, ref IEnumerable<Task> tasks, ref Dictionary<Guid, List<string>> children, ref Dictionary<Guid, List<string>> parents)
        {
            foreach (var task in tasks)
            {
                foreach (var child in children[task.Id])
                {
                    IEnumerable<Task> identifiedTasks = new List<Task>();
                    TaskValueParser.ParseIdentity(ref identifiedTasks, child, tasks);
                    if (identifiedTasks.Count() == 0)
                    {
                        throw new FileParsingException($"Unable to find child: {child}, for task id:{task.Id.ToString()}, name:{task.Name}, at path: {filePath}");
                    }
                    task.Children.Concat(identifiedTasks.Select(x => x.Id));
                }

                foreach (var parent in parents[task.Id])
                {
                    IEnumerable<Task> identifiedTasks = new List<Task>();
                    TaskValueParser.ParseIdentity(ref identifiedTasks, parent, tasks);
                    if (identifiedTasks.Count() == 0)
                    {
                        throw new FileParsingException($"Unable to find parent: {parent}, for task id:{task.Id.ToString()}, name:{task.Name}, at path: {filePath}");
                    }
                    task.Children.Concat(identifiedTasks.Select(x => x.Id));
                }
            }
        }

        public IEnumerable<Task> ReadMainPass(string filePath, ref Dictionary<Guid, List<string>> children, ref Dictionary<Guid, List<string>> parents)
        {
            List<Task> tasks = new List<Task>();
            if (!File.Exists(filePath))
                throw new ($"Task file at path: {filePath}, doesn't exist");

            Task? task = null;
            IEnumerable<Deadline> deadlines = new List<Deadline>();
            Deadline? deadline = null;

            foreach (var line in File.ReadLines(filePath))
            {
                var tmpline = line.TrimStart(new char[] { ' ', '-' }).TrimEnd();
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

                if (TaskSaveFormat.deadlineStart.Where(x => tmpline.StartsWith(x)).Any())
                {
                    if (task is not null)
                        tasks.Add(task);
                    task = new Task();
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

        public IEnumerable<Task> Read()
        {
            IEnumerable<Task> tasks = new List<Task>();

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
        */
        public void WriteToFile(string filePath, IEnumerable<Task> tasks)
        {
               
        }

        public void Write(IEnumerable<Task> tasks)
        {
            List<string> lines = new List<string>();
            foreach (var task in tasks)
            {
                lines.Append("i: " + task.Id.ToString());
                if (task.Name != string.Empty)
                {
                    lines.Append("n: " + task.Name);
                }
                if (task.Description != string.Empty)
                {
                    lines.Append("d: " + task.Description);
                }
                if (task.Parents.Count() > 0)
                {
                    foreach (var parent in task.Parents)
                    {
                        lines.Append("p:" + parent.ToString());
                    }
                }
                if (task.Children.Count() > 0)
                {
                    foreach (var child in task.Children)
                    {
                        lines.Append("c:" + child.ToString());
                    }
                }
                if (task.Deadlines.Count() > 0)
                {
                    foreach (var deadline in task.Deadlines)
                    {
                        lines.Append("de:");
                        if (deadline.enabled)
                            lines.Append("- e");

                        lines.Append("- ded:" + deadline.deadline.ToString("H:m d.M.yyyy"));
                        if (deadline.warningTime != TimeSpan.Zero)
                            lines.Append("- w:" + deadline.warningTime.ToString(@"d\.h\:m"));
                        if (deadline.duration != TimeSpan.Zero)
                            lines.Append("- du:" + deadline.duration.ToString(@"d\.h\:m"));
                        if (deadline.repeated)
                            lines.Append("- r");
                        if (deadline.repeatSpan != TimeSpan.Zero)
                            lines.Append("- s:" + deadline.repeatSpan.ToString(@"d\.h\:m"));
                        if (deadline.repeatYears > 0)
                            lines.Append("- y:" + deadline.repeatYears.ToString());
                        if (deadline.repeatMonths > 0)
                            lines.Append("- m:" + deadline.repeatMonths.ToString());
                    }
                }
            }
        }
    }
}
