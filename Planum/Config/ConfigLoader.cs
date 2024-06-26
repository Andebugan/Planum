using Newtonsoft.Json;
using System.IO;
using System.Runtime.InteropServices;
using System;

namespace Planum.Config
{
    public class ConfigException: Exception {
        public ConfigException(string message): base(message) { }
    }

    public static class ConfigLoader
    {
        public static string AppConfigPath = "Config\\Config.json";

        public static T LoadConfig<T>(string configPath) {
            var exeName = System.AppDomain.CurrentDomain.BaseDirectory;
            if (exeName is null)
                throw new ConfigException("Unable to get path to executable");

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                configPath = configPath.Replace("\\", "/");
                exeName = exeName.Replace("file://", "");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                exeName = exeName.Replace("file:///", "");
            }
            else
                throw new ConfigException("Unknown operating system, can't load config files");

            var systemPath = Path.GetDirectoryName(exeName);
            if (systemPath is null)
                throw new ConfigException("Couldn't open config file");

            var filepath = Path.Combine(systemPath, configPath);
            if (!File.Exists(filepath))
                File.Create(filepath);

            using StreamReader r = new StreamReader(filepath);
            string json = r.ReadToEnd();

            var result = JsonConvert.DeserializeObject<T>(json);
            if (result is null)
                throw new ConfigException("Couldn't open config file");
            r.Close();

            return result; 
        }

        public static void SaveConfig<T>(string configPath, T config) {
            var exeName = System.AppDomain.CurrentDomain.BaseDirectory;
            if (exeName is null)
                throw new ConfigException("Couldn't open config file");

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                configPath = configPath.Replace("\\", "/");
                exeName = exeName.Replace("file://", "");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                exeName = exeName.Replace("file:///", "");
            }
            else
                throw new ConfigException("Unknown operating system, can't load config files");


            var systemPath = Path.GetDirectoryName(exeName);
            if (systemPath is null)
                throw new ConfigException("Couldn't open config directory");
            string json = JsonConvert.SerializeObject(config);

            var filepath = Path.Combine(systemPath, configPath);
            if (!File.Exists(filepath))
                File.Create(filepath);

            File.WriteAllText(filepath, json);
        }
    }
}
