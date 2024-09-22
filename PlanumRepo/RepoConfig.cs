using Planum.Logger;

namespace Planum.Config
{
    ///<summary>Repo config DTO</summary>
    public class RepoConfigJsonDTO
    {
        public string TaskFileFilterPattern { get; set; } = "*.md";
        public List<string> TaskLookupPaths { get; set; } = new List<string>();
    }

    ///<summary>Repository parsing settings config</summary>
    public class RepoConfig
    {
        public string TaskFileFilterPattern { get; set; } = "*.md";
        public HashSet<string> TaskLookupPaths = new HashSet<string>();
        protected string RepoConfigPath { get; set; } = "./RepoConfig.json";

        public RepoConfig(string configPath) => RepoConfigPath = configPath;

        static RepoConfigJsonDTO ToJsonDTO(RepoConfig config)
        {
            RepoConfigJsonDTO configJsonDTO = new RepoConfigJsonDTO();
            configJsonDTO.TaskFileFilterPattern = config.TaskFileFilterPattern;
            configJsonDTO.TaskLookupPaths = config.TaskLookupPaths.ToList();
                        return configJsonDTO;
        }

        static RepoConfig FromJsonDTO(RepoConfigJsonDTO configJsonDTO, string configPath)
        {
            RepoConfig config = new RepoConfig(configPath);

            config.TaskFileFilterPattern = configJsonDTO.TaskFileFilterPattern;
            config.TaskLookupPaths = configJsonDTO.TaskLookupPaths.ToHashSet();
            return config;
        }

        public static RepoConfig Load(string repoConfigPath, ILoggerWrapper logger)
        {
            logger.Log("Loading repo config", LogLevel.INFO);
            RepoConfigJsonDTO configDTO = ConfigLoader.LoadConfig<RepoConfigJsonDTO>(repoConfigPath, new RepoConfigJsonDTO(), logger);
            var repoConfig = FromJsonDTO(configDTO, repoConfigPath);
            logger.Log("Repo config loaded", LogLevel.INFO);
            return repoConfig;
        }

        public void Save(ILoggerWrapper logger)
        {
            logger.Log("Saving repo config", LogLevel.INFO);
            ConfigLoader.SaveConfig<RepoConfigJsonDTO>(RepoConfigPath, ToJsonDTO(this), logger);
            logger.Log("Repo config saved", LogLevel.INFO);
        }
    }
}
