using Planum.ConsoleUI;
using Planum.Model.Managers;


namespace Planum
{
    internal class Program
    {
        static void Main(string[] args)
        {
            TaskManager taskManager = new TaskManager();
            taskManager.Backup();

            ConsoleShell consoleShell = new ConsoleShell(taskManager);
            consoleShell.MainLoop(args);
        }
    }
}
