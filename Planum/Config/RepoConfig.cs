using Newtonsoft.Json;
using System.IO;
using System.Runtime.InteropServices;
using System;

namespace Planum.Config
{
    public class RepoConfig
    {
        string configPath = "Config\\repo_config.json";

        public RepoConfigJson config = new RepoConfigJson();

        public void LoadConfig()
        {
            var exeName = System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase;
            if (exeName is null)
                throw new Exception("Couldn't open config file");

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
                throw new Exception("Unknown operating system, can't load config files");


            var systemPath = Path.GetDirectoryName(exeName);
            if (systemPath is null)
                throw new Exception("Couldn't open config file");

            using StreamReader r = new StreamReader(Path.Combine(systemPath, configPath));
            string json = r.ReadToEnd();
            var result = JsonConvert.DeserializeObject<RepoConfigJson>(json);
            if (result is null)
                throw new Exception("Couldn't open config file");

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                config.TaskDirectoryName = config.TaskDirectoryName.Replace("\\", "/");
                config.TaskBackupDirectoryName = config.TaskBackupDirectoryName.Replace("\\", "/");
            }
            r.Close();
        }
    }

    public class RepoConfigJson
    {
        public string TaskDirectoryName { get; set; }
        public string TaskBackupDirectoryName { get; set; }

        public RepoConfigJson()
        {
            TaskDirectoryName = string.Empty;
            TaskBackupDirectoryName = string.Empty;
        }
    }
}
