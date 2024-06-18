namespace Planum.Config
{
    public class RepoConfig
    {
        public string TaskDirectoryName { get; set; }
        public string TaskBackupDirectoryName { get; set; }

        public RepoConfig()
        {
            TaskDirectoryName = string.Empty;
            TaskBackupDirectoryName = string.Empty;
        }
    }
}
