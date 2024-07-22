namespace Planum.Config
{
    public class AppConfig
    {
        public string RepoConfigPath = "RepoConfig.json";
        public string AppConfigPath = "AppConfig.json";

        public static AppConfig Load() => ConfigLoader.LoadConfig<AppConfig>(ConfigLoader.AppConfigPath, new AppConfig());

        public void Save() => ConfigLoader.SaveConfig<AppConfig>(ConfigLoader.AppConfigPath, this);
    }
}
