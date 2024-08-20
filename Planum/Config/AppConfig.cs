using Planum.Logger;

namespace Planum.Config
{
    public class AppConfig
    {
        public string RepoConfigPath = "RepoConfig.json";
        public string CommandConfigPath = "CommandConfig.json";
        public string AppConfigPath = "AppConfig.json";

        public static AppConfig Load(ILoggerWrapper logger) => ConfigLoader.LoadConfig<AppConfig>(ConfigLoader.AppConfigPath, new AppConfig(), logger);

        public void Save(ILoggerWrapper logger) => ConfigLoader.SaveConfig<AppConfig>(ConfigLoader.AppConfigPath, this, logger);
    }
}
