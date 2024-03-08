using Newtonsoft.Json;
using System.IO;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using System.IO;
using System.Runtime.InteropServices;
using Planum.Config;
using System;
using Alba.CsConsoleFormat;
using System.Collections.Generic;

namespace Planum.Config
{
    public class CommandsConfig
    {
        string configPath = "Config\\command_config.json";

        public CommandConfigJson config = new CommandConfigJson();

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
            string path = Path.Combine(systemPath, configPath);
            if (!File.Exists(path))
                return;
            using StreamReader r = new StreamReader(path);
            string json = r.ReadToEnd();
            config = JsonConvert.DeserializeObject<CommandConfigJson>(json);
            r.Close();
        }

        public void SaveConfig()
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
            string json = JsonConvert.SerializeObject(config);
            File.WriteAllText(Path.Combine(systemPath, configPath), json);
        }
    }

    public class CommandData
    {
        public bool callOnStart = false;
        public Dictionary<string, List<string>> aliases = new Dictionary<string, List<string>>();
    }

    public class CommandConfigJson
    {
        public Dictionary<string, CommandData> commands = new Dictionary<string, CommandData>();
    }
}