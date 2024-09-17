using Planum.Logger;

namespace Planum.Config
{
    /// <summary>Global config class, contains loading strings</summary>
    public class AppConfig
    {

        /// <summary>Path to repo config file</summary>
        public string RepoConfigPath { get; set; } = "RepoConfig.json";
        /// <summary>Path to command config file</summary>
        public string CommandConfigPath { get; set; } = "CommandConfig.json";
        /// <summary>Path to app config file</summary>
        public string AppConfigPath { get; set; } = "AppConfig.json";

        /// <summary>Loads app config to specified location</summary>
        public static AppConfig Load(ILoggerWrapper logger) => ConfigLoader.LoadConfig<AppConfig>(ConfigLoader.AppConfigPath, new AppConfig(), logger);

        /// <summary>Save app config to specified location</summary>
        public void Save(ILoggerWrapper logger) => ConfigLoader.SaveConfig<AppConfig>(ConfigLoader.AppConfigPath, this, logger);
    }
}
