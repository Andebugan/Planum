using Newtonsoft.Json;
using System.IO;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using System.IO;
using System.Runtime.InteropServices;
using Planum.Config;
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
            using StreamReader r = new StreamReader(Path.Combine(systemPath, configPath));
            string json = r.ReadToEnd();
            config = JsonConvert.DeserializeObject<RepoConfigJson>(json);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                config.TaskMetafileName = config.TaskMetafileName.Replace("\\", "/");
                config.TaskDirectoryName = config.TaskDirectoryName.Replace("\\", "/");
                config.TaskBackupDirectoryName = config.TaskBackupDirectoryName.Replace("\\", "/");
            }
            r.Close();
        }
    }

    public class RepoConfigJson
    {
        public string TaskMetafileName { get; set; }
        public string TaskDirectoryName { get; set; }
        public string TaskBackupDirectoryName { get; set; }
        public int TaskFileTaskCount { get; set; }

        public RepoConfigJson()
        {
            TaskMetafileName = string.Empty;
            TaskDirectoryName = string.Empty;
            TaskBackupDirectoryName = string.Empty;
            TaskFileTaskCount = 0;
        }
    }
}