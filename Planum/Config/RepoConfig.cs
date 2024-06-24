using System;
using System.Collections.Generic;

namespace Planum.Config
{
    public class RepoConfig
    {
        public string TaskDirectoryName { get; set; }
        public string TaskBackupDirectoryName { get; set; }
        public string TaskFileSearchPattern { get; set; }
        public Dictionary<Guid, string> TaskLookupPaths { get; set; }

        public RepoConfig()
        {
            TaskDirectoryName = string.Empty;
            TaskBackupDirectoryName = string.Empty;
            TaskFileSearchPattern = string.Empty;
            TaskLookupPaths = new Dictionary<Guid, string>();
        }
    }
}
