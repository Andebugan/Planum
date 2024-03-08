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
    public class AppConfig
    {
        string configPath = "Config\\app_config.json";

        public AppConfigJson config = new AppConfigJson();

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
            config = JsonConvert.DeserializeObject<AppConfigJson>(json);
            r.Close();
        }
    }

    public class AppConfigJson
    {
        public string version { get; set; }

        public AppConfigJson()
        {
            version = string.Empty;
        }
    }
}