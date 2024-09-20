using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Planum.Logger;


namespace Planum.Config
{
    ///<summary>Config loading has gone wrong</summary>
    public class ConfigException : Exception
    {
        public ConfigException(string message) : base(message) { }
    }

    ///<summary>Generic JSON config loader</summary>
    public static class ConfigLoader
    {
        ///<summary>Relative path to app config</summary>
        public static string AppConfigPath = "Config\\Config.json";

        ///<summary>Loads json config from path</summary>
        ///<exception cref="ConfigException">Config loader exception</exception>
        public static T LoadConfig<T>(string configPath, T defaultConfig, ILoggerWrapper logger)
        {
            logger.Log($"Loading config from {configPath}", LogLevel.INFO);
            var exeName = System.AppDomain.CurrentDomain.BaseDirectory;
            if (exeName is null)
                throw new ConfigException("Unable to get path to executable");

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                configPath = configPath.Replace("\\", "/");
                exeName = exeName.Replace("file://", "");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                exeName = exeName.Replace("file:///", "");
            else
                throw new ConfigException("Unknown operating system, can't load config files");

            var systemPath = Path.GetDirectoryName(exeName);
            if (systemPath is null)
                throw new ConfigException("Couldn't find executable directory");

            var filepath = Path.Combine(systemPath, configPath);
            if (!File.Exists(filepath))
            {
                if (!Directory.Exists(Path.GetDirectoryName(filepath)))
                {
                    var path = Path.GetDirectoryName(filepath);
                    if (path is null)
                        throw new ConfigException("Unable to get name of the config directory");
                    Directory.CreateDirectory(path);
                }
                SaveConfig<T>(configPath, defaultConfig, logger);
                return defaultConfig;
            }

            T result;
            using (var r = new StreamReader(filepath))
            {
                string json = r.ReadToEnd();

                var jsonResult = JsonConvert.DeserializeObject<T>(json);
                if (jsonResult is null)
                    throw new ConfigException($"Couldn't read json from config file {filepath}");
                result = jsonResult;
            }
            return result;
        }

        ///<summary>Saves json config from path</summary>
        ///<exception cref="ConfigException">Config loader exception</exception>
        public static void SaveConfig<T>(string configPath, T config, ILoggerWrapper logger)
        {
            logger.Log($"Saving config at \"{configPath}\"", LogLevel.INFO);
            var exeName = System.AppDomain.CurrentDomain.BaseDirectory;
            if (exeName is null)
                throw new ConfigException("Couldn't open config file");

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                configPath = configPath.Replace("\\", "/");
                exeName = exeName.Replace("file://", "");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                exeName = exeName.Replace("file:///", "");
            else
                throw new ConfigException("Unknown operating system, can't load config files");


            var systemPath = Path.GetDirectoryName(exeName);
            if (systemPath is null)
                throw new ConfigException("Couldn't open config directory");
            string json = JsonConvert.SerializeObject(config);

            var filepath = Path.Combine(systemPath, configPath);
            File.WriteAllText(filepath, json);
        }
    }
}
