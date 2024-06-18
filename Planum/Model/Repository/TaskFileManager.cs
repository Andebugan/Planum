using Planum.Config;
using Planum.Model.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Planum.Model.Repository
{
    public class TaskFileManager : ITaskFileManager
    {
        ConfigLoader config = new ConfigLoader();

        public TaskFileManager()
        {
            repoConfig.LoadConfig();
            CreateTaskFiles();
        }

        string FilePath { get; set; } = string.Empty;
        string BackupPath { get; set; } = string.Empty;

        protected void GetSavePath()
        {
            string dirName = repoConfig.config.TaskDirectoryName;
            string savePath = AppContext.BaseDirectory;

            string dirPath = Path.Combine(savePath, dirName);
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            FilePath = dirPath + "tasks";
        }

        protected void GetBackupPath()
        {
            string dirName = repoConfig.config.TaskBackupDirectoryName;
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
        */

        public IEnumerable<Task> Read()
        {
            List<Task> tasks = new List<Task>();

            Task? task = null;
            IEnumerable<Guid> parents = new List<Guid>();
            IEnumerable<Guid> children = new List<Guid>();

            IEnumerable<Deadline> deadlines = new List<Deadline>();
            Deadline? deadline = null;

            foreach (var line in File.ReadLines(FilePath))
            {
                var tmpline = line.TrimStart(new char[] { ' ', '-' });
                if (deadline is not null)
                {
                    if (tmpline.StartsWith("e: "))
                    {
                        deadline.enabled = true;
                    }
                    else if (tmpline.StartsWith("ded: "))
                    {
                        tmpline = tmpline.Remove(0, 5);
                        deadline.deadline = DateTime.ParseExact(tmpline, "H:m d.M.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None);
                    }
                    else if (tmpline.StartsWith("w: "))
                    {
                        tmpline = tmpline.Remove(0, 3);
                        deadline.warningTime = TimeSpan.ParseExact(tmpline, @"d\.h\:m", CultureInfo.InvariantCulture, TimeSpanStyles.None);
                    }
                    else if (tmpline.StartsWith("du: "))
                    {
                        tmpline = tmpline.Remove(0, 4);
                        deadline.duration = TimeSpan.ParseExact(tmpline, @"d\.h\:m", CultureInfo.InvariantCulture, TimeSpanStyles.None);
                    }
                    else if (tmpline.StartsWith("r: "))
                    {
                        deadline.enabled = true;
                    }
                    else if (tmpline.StartsWith("s: "))
                    {
                        tmpline = tmpline.Remove(0, 3);
                        deadline.repeatSpan = TimeSpan.ParseExact(tmpline, @"d\.h\:m", CultureInfo.InvariantCulture, TimeSpanStyles.None);
                    }
                    else if (tmpline.StartsWith("y: "))
                    {
                        tmpline = tmpline.Remove(0, 3);
                        deadline.repeatYears = int.Parse(tmpline);
                    }
                    else if (tmpline.StartsWith("m: "))
                    {
                        tmpline = tmpline.Remove(0, 3);
                        deadline.repeatMonths = int.Parse(tmpline);
                    }
                    else
                    {
                        deadlines.Append(deadline);
                        deadline = null;
                    }
                }

                if (tmpline.StartsWith("i: "))
                {
                    if (task is not null)
                    {
                        task.Parents = parents.ToList();
                        task.Children = children.ToList();

                        if (deadline is not null)
                            deadlines.Append(deadline);
                        task.Deadlines = deadlines.ToList();

                        tasks.Add(task);
                    }

                    parents = new List<Guid>();
                    children = new List<Guid>();

                    tmpline = tmpline.Remove(0, 3);
                    task = new Task();
                    task.Id = Guid.Parse(tmpline);
                }
                else if (task is null)
                {
                    throw new Exception($"Parsing error at: \"{line}\"");
                }
                else if (tmpline.StartsWith("n: "))
                {
                    task.Name = tmpline.Remove(0, 3);
                }
                else if (tmpline.StartsWith("d: "))
                {
                    task.Description = tmpline.Remove(0, 3);
                }
                else if (tmpline.StartsWith("p: "))
                {
                    parents.Append(Guid.Parse(tmpline.Remove(0, 3)));
                }
                else if (tmpline.StartsWith("c: "))
                {
                    children.Append(Guid.Parse(tmpline.Remove(0, 3)));
                }
                else if (tmpline.StartsWith("de: "))
                {
                    if (deadline is not null)
                        deadlines.Append(deadline);
                    deadline = new Deadline();
                }
            }

            if (task is not null)
            {
                task.Parents = parents.ToList();
                task.Children = children.ToList();

                if (deadline is not null)
                    deadlines.Append(deadline);
                task.Deadlines = deadlines.ToList();

                tasks.Add(task);
            }

            return tasks;
        }

        /*
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
