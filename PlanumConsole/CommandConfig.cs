using Planum.Logger;

namespace Planum.Config
{
    /// <summary>Data for specific command</summary>
    public class ConsoleConfigData
    {
        /// <summary>If true command is executed on start</summary>
        public bool CallOnStart { get; set; }= false;
        /// <summary>Arguments for call on start execution</summary>
        public Dictionary<string, List<string>> CallOnStartArgs { get; set; } = new Dictionary<string, List<string>>();
        /// <summary>Dict of aliases for commands (./Config.md)</summary>
        public Dictionary<string, List<string>> Aliases { get; set; } = new Dictionary<string, List<string>>();
    }

    /// <summary>Configuration for commands</summary>
    public class ConsoleConfig
    {
        public string OptionPrefix { get; set; } = "-";
        public Dictionary<string, ConsoleConfigData> Commands { get; set; } = new Dictionary<string, ConsoleConfigData>();

        protected string ConsoleConfigPath = "./CommandConfig.json";
        public string RepoConfigPath = "./RepoConfig.json";

        /// <summary>Load command config from path defined in app config</summary>
        public static ConsoleConfig Load(string commandConfigPath, ILoggerWrapper logger)
        {
            logger.Log("Loading command config", LogLevel.INFO);
            var config = ConfigLoader.LoadConfig<ConsoleConfig>(commandConfigPath, new ConsoleConfig(), logger);
            logger.Log("Command config loaded", LogLevel.INFO);
            return config;
        }

        /// <summary>Save command config to path defined in app config</summary>
        public void Save(ILoggerWrapper logger)
        {
            logger.Log("Saving command config", LogLevel.INFO);
            ConfigLoader.SaveConfig<ConsoleConfig>(ConsoleConfigPath, this, logger);
            logger.Log("Command config saved", LogLevel.INFO);

        }
    }
}
