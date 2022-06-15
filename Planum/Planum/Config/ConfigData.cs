using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planum.Config
{
    public static class ConfigData
    {
        static string configPath = "Config\\config.json";

        public static ConfigJson LoadConfig()
        {
            var exeName = System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase;
            exeName = exeName.Replace("file:///", "");
            var systemPath = Path.GetDirectoryName(exeName);
            using StreamReader r = new StreamReader(Path.Combine(systemPath, configPath));
            string json = r.ReadToEnd();
            return JsonConvert.DeserializeObject<ConfigJson>(json);
        }
    }

    public class ConfigJson
    {
        public string TagRepoFilePath { get; set; }
        public string TaskRepoFilePath { get; set; }
        public string UserRepoFilePath { get; set; }
    }
}
