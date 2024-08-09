using Newtonsoft.Json;
using System.IO;
using System.Runtime.InteropServices;
using System;
using Planum.Logger;

namespace Planum.Config
{
    public class ConfigException : Exception
    {
        public ConfigException(string message) : base(message) { }
    }

    public static class ConfigLoader
    {
        public static string AppConfigPath = "Config\\Config.json";

        public static T LoadConfig<T>(string configPath, T defaultConfig, ILoggerWrapper logger)
        {
            logger.Log(LogLevel.DEBUG, "Searching for executable directory");
            var exeName = System.AppDomain.CurrentDomain.BaseDirectory;
            if (exeName is null)
                throw new ConfigException("Unable to get path to executable");

            logger.Log(LogLevel.DEBUG, "Changing paths for detected OS platform");
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                configPath = configPath.Replace("\\", "/");
                exeName = exeName.Replace("file://", "");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                exeName = exeName.Replace("file:///", "");
            else
                throw new ConfigException("Unknown operating system, can't load config files");

            logger.Log(LogLevel.DEBUG, "Getting directory name");
            var systemPath = Path.GetDirectoryName(exeName);
            if (systemPath is null)
                throw new ConfigException("Couldn't find executable directory");

            var filepath = Path.Combine(systemPath, configPath);
            logger.Log(LogLevel.DEBUG, $"Trying to load config from file at path: {filepath}");
            if (!File.Exists(filepath))
            {
                logger.Log(LogLevel.DEBUG, $"File not found, creating");
                if (!Directory.Exists(Path.GetDirectoryName(filepath)))
                {
                    logger.Log(LogLevel.DEBUG, $"Directory not found, creating");
                    var path = Path.GetDirectoryName(filepath);
                    if (path is null)
                        throw new ConfigException("Unable to get name of the config directory");
                    Directory.CreateDirectory(path);
                }
                logger.Log(LogLevel.DEBUG, $"Saving default config");
                SaveConfig<T>(configPath, defaultConfig, logger);
                logger.Log(LogLevel.DEBUG, $"Save finished, returning default config");
                return defaultConfig;
            }

            T result;
            using (var r = new StreamReader(filepath))
            {
                logger.Log(LogLevel.DEBUG, $"Reading json at path: {filepath}");
                string json = r.ReadToEnd();

                var jsonResult = JsonConvert.DeserializeObject<T>(json);
                if (jsonResult is null)
                    throw new ConfigException($"Couldn't read json from config file {filepath}");
                result = jsonResult;
                logger.Log(LogLevel.DEBUG, $"Parsing successfull, returning result");
            }
            return result;
        }

        public static void SaveConfig<T>(string configPath, T config, ILoggerWrapper logger)
        {
            logger.Log(LogLevel.DEBUG, "Searching for executable directory");
            var exeName = System.AppDomain.CurrentDomain.BaseDirectory;
            if (exeName is null)
                throw new ConfigException("Couldn't open config file");

            logger.Log(LogLevel.DEBUG, "Changing paths for detected OS platform");
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                configPath = configPath.Replace("\\", "/");
                exeName = exeName.Replace("file://", "");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                exeName = exeName.Replace("file:///", "");
            else
                throw new ConfigException("Unknown operating system, can't load config files");


            logger.Log(LogLevel.DEBUG, "Getting directory name");
            var systemPath = Path.GetDirectoryName(exeName);
            if (systemPath is null)
                throw new ConfigException("Couldn't open config directory");
            logger.Log(LogLevel.DEBUG, "Serializing repo config data to json");
            string json = JsonConvert.SerializeObject(config);

            var filepath = Path.Combine(systemPath, configPath);
            logger.Log(LogLevel.DEBUG, $"Saving result to config at path: {filepath}");
            File.WriteAllText(filepath, json);
            logger.Log(LogLevel.DEBUG, $"Config saved complete");
        }
    }
}
