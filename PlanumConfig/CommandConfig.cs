using System.Collections.Generic;
using Planum.Logger;

namespace Planum.Config
{
    /// <summary>Data for specific command</summary>
    public class CommandData
    {
        /// <summary>If true command is executed on start</summary>
        public bool callOnStart = false;
        /// <summary>Arguments for call on start execution</summary>
        public Dictionary<string, List<string>> callOnStartArgs = new Dictionary<string, List<string>>();
        /// <summary>Dict of aliases for commands (./Config.md)</summary>
        public Dictionary<string, List<string>> aliases = new Dictionary<string, List<string>>();
    }

    /// <summary>Configuration for commands</summary>
    public class CommandConfig
    {
        public string OptionPrefix { get; set; } = "-";
        public Dictionary<string, CommandData> commands = new Dictionary<string, CommandData>();

        /// <summary>Load command config from path defined in app config</summary>
        public static CommandConfig Load(AppConfig appConfig, ILoggerWrapper logger)
        {
            logger.Log("Loading command config", LogLevel.INFO);
            return ConfigLoader.LoadConfig<CommandConfig>(appConfig.CommandConfigPath, new CommandConfig(), logger);
        }

        /// <summary>Save command config to path defined in app config</summary>
        public void Save(AppConfig appConfig, ILoggerWrapper logger)
        {
            logger.Log("Saving command config", LogLevel.INFO);
            ConfigLoader.SaveConfig<CommandConfig>(appConfig.CommandConfigPath, this, logger);
        }
    }
}
