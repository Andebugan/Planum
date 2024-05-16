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
        RepoConfig repoConfig = new RepoConfig();

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
         * i[d]: {Guid} 
         * n[ame]: {string}
         * d[escription]: {string} 
         * p[arent]: {Guid}
         * ...
         * c[hildre]: {Guid}
         * ...
         * d[eadline]:
         * - e[nabled]: {true\false}
         * - de[adline]: {hh:mm dd.mm.yyyy}
         * - w[arning]: {dd.hh.mm}
         * - du[ration]: {dd.hh.mm}
         * - r[peated]: {true\false}
         * - s[pan]: {dd.hh.mm}
         * - y[ears]: {int}
         * - m[onths]: {int}
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
                    if (tmpline.StartsWith("e: ")) {
                        tmpline = tmpline.Remove(0, 3);
                        deadline.enabled = bool.Parse(tmpline);
                    } else if (tmpline.StartsWith("de: ")) {
                        tmpline = tmpline.Remove(0, 4);
                        deadline.deadline = DateTime.ParseExact(tmpline, "H:m d.M.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None);
                    } else if (tmpline.StartsWith("w: ")) {
                        tmpline = tmpline.Remove(0, 3);
                        deadline.warningTime = TimeSpan.ParseExact(tmpline, @"d\.h\:m", CultureInfo.InvariantCulture, TimeSpanStyles.None);
                    } else if (tmpline.StartsWith("du: ")) {
                        tmpline = tmpline.Remove(0, 4);
                        deadline.duration = TimeSpan.ParseExact(tmpline, @"d\.h\:m", CultureInfo.InvariantCulture, TimeSpanStyles.None);
                    } else if (tmpline.StartsWith("r: ")) {
                        tmpline = tmpline.Remove(0, 3);
                        deadline.enabled = bool.Parse(tmpline);
                    } else if (tmpline.StartsWith("s: ")) {
                        tmpline = tmpline.Remove(0, 3);
                        deadline.repeatSpan = TimeSpan.ParseExact(tmpline, @"d\.h\:m", CultureInfo.InvariantCulture, TimeSpanStyles.None);
                    } else if (tmpline.StartsWith("y: ")) {
                        tmpline = tmpline.Remove(0, 3);
                        deadline.repeatYears = int.Parse(tmpline);
                    } else if (tmpline.StartsWith("m: ")) {
                        tmpline = tmpline.Remove(0, 3);
                        deadline.repeatMonths = int.Parse(tmpline);
                    } else {
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
                else if (tmpline.StartsWith("d: "))
                {
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

        public void Write(IEnumerable<Task> tasks)
        {
            throw new NotImplementedException();
        }
    }
}
