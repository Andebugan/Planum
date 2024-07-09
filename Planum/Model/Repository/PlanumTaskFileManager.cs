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
    /*
     * <planum:guid>
     * - [ ] n(ame): {string}
     * - d(escription): {string}
     * - [ ] p(arent): {guid}
     * ...
     * - [ ] c(hildren): {string} | [guid] (name or/and guid if name was not provided or name is not unique)
     * ...
     * - [ ] D(eadline): ([x] - disabled/complete, [ ] - enabled)
     *     - d(adline]: {hh:mm dd.mm.yyyy}
     *     - w(arning]: {dd.hh.mm}
     *     - du(ration]: {dd.hh.mm}
     *     - [ ] r(epeat duration): {y m d.hh:mm}
     * ...
     * - [ ] {string} [| {guid}] (name or/and guid if name was not provided or name is not unique) (level 1 checklist)
     *     - d(escription)
     *     - [ ] D(eadline)
     *        - ...
     *        - [ ] {string} [| {guid}] (name or/and guid if name was not provided or name is not unique) (level 1 checklist)
     *        - ...
     * ...
     * <- ends with empty line/<planum> for next task after it
    */
    public class PlanumTaskFileManager : IPlanumTaskFileManager
    {
        RepoConfig RepoConfig { get; set; }
        AppConfig AppConfig { get; set; }
        IPlanumTaskWriter TaskWriter { get; set; }
        IPlanumTaskReader TaskReader { get; set; }

        public PlanumTaskFileManager(AppConfig appConfig, RepoConfig repoConfig, IPlanumTaskReader taskReader, IPlanumTaskWriter taskWriter)
        {
            AppConfig = appConfig;
            RepoConfig = repoConfig;

            TaskReader = taskReader;
            TaskWriter = taskWriter;

            CreateTaskFiles();
        }

        string filePath = string.Empty;
        public string FilePath { get => filePath; }
        string backupPath = string.Empty;
        public string BackupPath { get => backupPath; }

        protected void GetSavePath()
        {
            string savePath = AppContext.BaseDirectory;
            filePath = Path.Combine(savePath, RepoConfig.TaskFilename);
        }

        protected void GetBackupPath()
        {
            string savePath = AppContext.BaseDirectory;
            backupPath = Path.Combine(savePath, RepoConfig.TaskBackupFilename);
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

        public IEnumerable<PlanumTask> ReadFromFile(IEnumerable<PlanumTask> tasks, ref IPlanumTaskReader reader)
        {
            return new PlanumTask[] {};
        }

        public IEnumerable<PlanumTask> Read()
        {
            IEnumerable<PlanumTask> tasks = new List<PlanumTask>();
            
            return tasks;
        }

        protected void WriteToFile(string path, IEnumerable<PlanumTask> tasks, bool create = false)
        {

        }

        public void Write(IEnumerable<PlanumTask> tasks)
        {

        }
    }
}
