using Planum.Config;
using Planum.Model.Entities;
using System;
using System.Collections.Generic;
using System.IO;

namespace Planum.Model.Repository
{
    public class TaskFileManager: ITaskFileManager
    {
        RepoConfig repoConfig = new RepoConfig();

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
           
        }

        public void Backup() {
          // copy main file to backup file
        }

        public void Restore() {
          // copy backup file to main file
        }
        
        /*
         * id: name
         * d: description
         * p: parents
         * c: children
         * t:
         * - e: [x/v]
         * - s: [h:m d.m.y]
         * - d: [h:m d.m.y]
         * - r:
         *   - e: [x/v]
         *   - a: [x/v]
         *   - rt: [h:m d.m.y]
         * a: arhived [x/v]
         * cl:
         * - [x/v] description 1
         * - [x/v] description 2
         * - ...
        */

        public IEnumerable<Task> Read()
        {
            throw new NotImplementedException();
        }

        public void Write(IEnumerable<Task> tasks)
        {
            throw new NotImplementedException();
        }
    }
}
