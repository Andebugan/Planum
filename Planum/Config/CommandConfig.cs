using System.Collections.Generic;
using Planum.Logger;

namespace Planum.Config
{
    public class CommandData
    {
        public bool callOnStart = false;
        public Dictionary<string, List<string>> callOnStartArgs = new Dictionary<string, List<string>>();
        public Dictionary<string, List<string>> aliases = new Dictionary<string, List<string>>();
    }

    public class CommandConfig
    {
        public string OptionPrefix { get; set; } = "-";
        public Dictionary<string, CommandData> commands = new Dictionary<string, CommandData>();

        public static CommandConfig Load(ILoggerWrapper logger)
        {
            logger.Log(LogLevel.INFO, "Loading app config");
            var appConfig = AppConfig.Load(logger);
            logger.Log(LogLevel.INFO, "Loading command config");
            return ConfigLoader.LoadConfig<CommandConfig>(appConfig.CommandConfigPath, new CommandConfig(), logger);
        }

        public void Save(ILoggerWrapper logger)
        {
            logger.Log(LogLevel.INFO, "Loading app config");
            var appConfig = AppConfig.Load(logger);
            logger.Log(LogLevel.INFO, "Saving command config");
            ConfigLoader.SaveConfig<CommandConfig>(appConfig.CommandConfigPath, this, logger);
        }
    }
}
