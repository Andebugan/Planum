using System;
using System.Collections.Generic;

namespace Planum.Config
{
    public class RepoConfig
    {
        public string TaskFilename { get; set; }
        public string TaskBackupFilename { get; set; }
        public string TaskFileSearchPattern { get; set; }
        public Dictionary<Guid, string> TaskLookupPaths { get; set; }

        public RepoConfig()
        {
            TaskFilename = "tasks.md";
            TaskBackupFilename = "tasks_backup.md";
            TaskFileSearchPattern = "*.md";
            TaskLookupPaths = new Dictionary<Guid, string>();
        }
    }
}
