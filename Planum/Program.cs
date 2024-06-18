using Planum.ConsoleUI;
using Planum.Model.Managers;

namespace Planum
{
    internal class Program
    {

        static string commandConfigPath = "Config\\command_config.json";
        static string repoConfigPath = "Config\\repo_config.json";

        static void Main(string[] args)
        {
            TaskManager taskManager = new TaskManager();
            taskManager.Backup();

            ConsoleShell consoleShell = new ConsoleShell(taskManager);
            consoleShell.MainLoop(args);
        }
    }
}
