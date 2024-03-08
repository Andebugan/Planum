using Planum.Config;
using Planum.Model.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Planum.Model.Repository
{
    public class TaskFileManager
    {
        List<TaskFile> taskFiles = new List<TaskFile>();
        RepoConfig repoConfig = new RepoConfig();
        /*
         * last
         * finished creating new classed
         * needed to implement all functions
         * FileManager - get all/speficied tasks
         */
        public TaskFileManager()
        {
            // for each file in folder create task file instance

            repoConfig.LoadConfig();
            GetSaveDirectoryPath();
            GetBackupDirectoryPath();
            CreateTaskFiles();
        }


        string SaveDirectoryPath { get; set; } = string.Empty;
        string BackupDirectoryPath { get; set; } = string.Empty;

        protected void GetSaveDirectoryPath()
        {
            string dirName = repoConfig.config.TaskDirectoryName;
            string savePath = AppContext.BaseDirectory;

            string dirPath = Path.Combine(savePath, dirName);
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            SaveDirectoryPath = dirPath;
        }

        protected void GetBackupDirectoryPath()
        {
            string dirName = repoConfig.config.TaskBackupDirectoryName;
            string savePath = AppContext.BaseDirectory;

            string dirPath = Path.Combine(savePath, dirName);
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            BackupDirectoryPath = dirPath;
        }

        public void CreateTaskFiles()
        {
            string[] paths = Directory.GetFiles(SaveDirectoryPath);
            foreach (var path in paths)
            {
                TaskFile taskFile = new TaskFile();
                taskFile.Id = int.Parse(Path.GetFileName(path).Replace(".dat", ""));
                taskFile.Path = path;
                taskFile.MaxTaskCount = repoConfig.config.TaskFileTaskCount;
                taskFiles.Add(taskFile);
            }

            if (taskFiles.Any(x => x.Read().Exists(y => y.Id < x.StartId || y.Id >= x.EndId)))
            {
                List<Task> tasks = ReadAll();

                foreach (var taskFile in taskFiles)
                {
                    if (taskFile.Id != 0)
                        File.Delete(taskFile.Path);
                }

                taskFiles = new List<TaskFile>();

                Write(tasks);
            }

            if (paths.Length == 0)
            {
                TaskFile taskFile = new TaskFile();
                taskFile.Id = 0;
                taskFile.Path = Path.Combine(SaveDirectoryPath, "0.dat");
                taskFile.MaxTaskCount = repoConfig.config.TaskFileTaskCount;
                taskFiles.Add(taskFile);
            }

            Backup();
        }

        public int GetFreeId()
        {
            if (taskFiles.Exists(x => x.FreeIdsCount > 0))
                return taskFiles.Where(x => x.FreeIdsCount > 0).First().GetFreeId();
            return (taskFiles.Last().Id + 1) * taskFiles.Last().MaxTaskCount;
        }

        public List<Task> ReadAll()
        {
            List<Task> tasks = new List<Task>();
            foreach (var taskFile in taskFiles)
            {
                tasks = tasks.Concat(taskFile.Read()).ToList();
            }
            return tasks;
        }

        public List<Task> ReadWithIds(List<int> taskIds)
        {
            List<Task> tasks = new List<Task>();

            foreach (var taskFile in taskFiles.Where(x => taskIds.Exists(y => y >= x.StartId && y < x.EndId)))
            {
                tasks = tasks.Concat(taskFile.Read()).ToList();
            }

            return tasks.Where(x => taskIds.Contains(x.Id)).ToList();
        }

        public List<Task> ReadFromFiles(int[] fileIds)
        {
            List<Task> tasks = new List<Task>();

            foreach (var taskFile in taskFiles.Where(x => fileIds.Contains(x.Id)))
            {
                tasks = tasks.Concat(taskFile.Read()).ToList();
            }

            return tasks;
        }

        public List<Task> ReadWhereTaskIds(List<int> taskIds)
        {
            List<Task> tasks = new List<Task>();
            List<TaskFile> filteredTaskFiles = taskFiles.Where(x => taskIds.Exists(y => y >= x.StartId && y < x.EndId)).ToList();
            foreach (var taskFile in filteredTaskFiles)
            {
                tasks = tasks.Concat(taskFile.Read()).ToList();
            }
            return tasks;
        }

        public void Delete(List<int> taskIds)
        {
            foreach (var taskFile in taskFiles)
            {
                taskIds = taskFile.RemoveFromQueue(taskIds);
                if (taskIds.Count == 0)
                    break;
            }
        }

        public void Write(List<Task> tasks, bool add = false)
        {
            foreach (var taskFile in taskFiles)
            {
                tasks = taskFile.WriteFromQueue(tasks, add);
                if (tasks.Count == 0)
                    break;
            }

            while (tasks.Count > 0)
            {
                TaskFile taskFile = new TaskFile();
                taskFile.MaxTaskCount = repoConfig.config.TaskFileTaskCount;
                taskFile.Id = tasks.First().Id / taskFile.MaxTaskCount;
                taskFile.Path = Path.Combine(SaveDirectoryPath, taskFile.Id.ToString() + ".dat");
                taskFile.MaxTaskCount = repoConfig.config.TaskFileTaskCount;
                tasks = taskFile.WriteFromQueue(tasks, add);
                taskFiles.Add(taskFile);
            }

            DeleteEmpty();
        }

        public void DeleteEmpty()
        {
            List<TaskFile> empty = taskFiles.Where(x => x.Id != 0 && x.FreeIdsCount == x.MaxTaskCount).ToList();
            taskFiles = taskFiles.Except(empty).ToList();
            foreach (var taskFile in empty)
            {
                File.Delete(taskFile.Path);
            }
        }

        public void Backup(bool restore = false)
        {
            string[] from;
            string[] to;

            if (restore)
            {
                from = Directory.GetFiles(BackupDirectoryPath);
                to = Directory.GetFiles(SaveDirectoryPath);
            }
            else
            {
                from = Directory.GetFiles(SaveDirectoryPath);
                to = Directory.GetFiles(BackupDirectoryPath);
            }

            if (from.Length == 0)
                return;

            foreach (string file in to)
            {
                File.Delete(file);
            }

            string dest;
            if (restore)
                dest = SaveDirectoryPath;
            else
                dest = BackupDirectoryPath;

            foreach (string file in from)
            {
                File.Copy(file, Path.Combine(dest, Path.GetFileName(file)));
            }
        }
    }
}
