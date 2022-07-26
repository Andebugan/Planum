using Newtonsoft.Json;
using System.IO;
using System.Runtime.InteropServices;

namespace Planum.Config
{
    public static class ConfigData
    {
        static string configPath = "Config\\config.json";

        public static ConfigJson LoadConfig()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                configPath = configPath.Replace("\\", "/");
                var exeName = System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase;
                exeName = exeName.Replace("file://", "");
                var systemPath = Path.GetDirectoryName(exeName);
                using StreamReader r = new StreamReader(Path.Combine("", configPath));
                string json = r.ReadToEnd();
                ConfigJson config = JsonConvert.DeserializeObject<ConfigJson>(json);
                config.TagRepoFilePath = config.TagRepoFilePath.Replace("\\", "/");
                config.TaskRepoFilePath = config.TaskRepoFilePath.Replace("\\", "/");
                config.UserRepoFilePath = config.UserRepoFilePath.Replace("\\", "/");
                return config;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var exeName = System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase;
                exeName = exeName.Replace("file:///", "");
                var systemPath = Path.GetDirectoryName(exeName);
                using StreamReader r = new StreamReader(Path.Combine(systemPath, configPath));
                string json = r.ReadToEnd();
                return JsonConvert.DeserializeObject<ConfigJson>(json);
            }
            throw new System.ExecutionEngineException();
        }
    }

    public class ConfigJson
    {
        public string TagRepoFilePath { get; set; }
        public string TaskRepoFilePath { get; set; }
        public string UserRepoFilePath { get; set; }
        public string version { get; set; }
    }
}
