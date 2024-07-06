using System;
using System.Collections.Generic;
using System.Linq;

namespace Planum.Config
{
    public class RepoConfigDto
    {
        public string TaskFilename { get; set; }
        public string TaskBackupFilename { get; set; }
        public string TaskFileSearchPattern { get; set; }
        public Dictionary<string, List<Guid>> TaskLookupPaths { get; set; }

        public RepoConfigDto()
        {
            TaskFilename = "tasks.md";
            TaskBackupFilename = "tasks_backup.md";
            TaskFileSearchPattern = "*.md";
            TaskLookupPaths = new Dictionary<string, List<Guid>>();
        }

        public RepoConfigDto(RepoConfig config)
        {
            TaskFilename = config.TaskFilename;
            TaskBackupFilename = config.TaskBackupFilename;
            TaskFileSearchPattern = config.TaskFileSearchPattern;
            TaskLookupPaths = new Dictionary<string, List<Guid>>();

            foreach (var key in config.TaskLookupPaths.Keys)
                TaskLookupPaths[key] = config.TaskLookupPaths[key].ToList();
        }

        public RepoConfig FromDto()
        {
            RepoConfig config = new RepoConfig();
            config.TaskFilename = TaskFilename;
            config.TaskBackupFilename = TaskBackupFilename;
            config.TaskFileSearchPattern = TaskFileSearchPattern;

            foreach (var key in TaskLookupPaths.Keys)
                config.TaskLookupPaths[key] = TaskLookupPaths[key].ToHashSet();
            return config;
        }
    }

    public class RepoConfig
    {
        public string TaskFilename { get; set; }
        public string TaskBackupFilename { get; set; }
        public string TaskFileSearchPattern { get; set; }
        public Dictionary<string, HashSet<Guid>> TaskLookupPaths { get; set; }

        public RepoConfig()
        {
            TaskFilename = "tasks.md";
            TaskBackupFilename = "tasks_backup.md";
            TaskFileSearchPattern = "*.md";
            TaskLookupPaths = new Dictionary<string, HashSet<Guid>>();
        }
    }
}
