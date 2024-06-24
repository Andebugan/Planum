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
        public static string commandConfigPath = "Config\\command_config.json";
        public static string repoConfigPath = "Config\\repo_config.json";

        public static T LoadConfig<T>(string configPath) {
            var exeName = System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase;
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

            using StreamReader r = new StreamReader(Path.Combine(systemPath, configPath));
            string json = r.ReadToEnd();

            var result = JsonConvert.DeserializeObject<T>(json);
            if (result is null)
                throw new ConfigException("Couldn't open config file");
            r.Close();

            return result; 
        }

        public static void SaveConfig<T>(string configPath, T config) {
            var exeName = System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase;
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
            File.WriteAllText(Path.Combine(systemPath, configPath), json);
        }
    }
}
