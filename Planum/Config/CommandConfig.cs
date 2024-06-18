using System.Collections.Generic;

namespace Planum.Config
{
    public class CommandData
    {
        public bool callOnStart = false;
        public Dictionary<string, List<string>> callOnStartArgs = new Dictionary<string, List<string>>();
        public Dictionary<string, List<string>> aliases = new Dictionary<string, List<string>>();
    }

    public class CommandConfig
    {
        public Dictionary<string, CommandData> commands = new Dictionary<string, CommandData>();
    }
}
