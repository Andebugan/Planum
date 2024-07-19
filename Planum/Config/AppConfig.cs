namespace Planum.Config
{
    public class AppConfig
    {
        public string ChecklistTaskName { get; set; } = ".checklist";
        public string OverdueTaskName { get; set; } = ".overdue";
        public string InProgressTaskName { get; set; } = ".progress";
        public string WarningTaskName { get; set; } = ".warning";

        public string RepoConfigPath = "RepoConfig.json";
        public string AppConfigPath = "AppConfig.json";

        public static AppConfig Load() => ConfigLoader.LoadConfig<AppConfig>(ConfigLoader.AppConfigPath, new AppConfig());

        public void Save() => ConfigLoader.SaveConfig<AppConfig>(ConfigLoader.AppConfigPath, this);
    }
}
