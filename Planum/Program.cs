using Planum.Config;
using Planum.ConsoleUI;
using Planum.Model.Managers;


namespace Planum
{
  internal class Program
  {
    static void Main(string[] args)
    {
      AppConfig appConfig = new AppConfig();
      appConfig.LoadConfig();

      TaskManager taskManager = new TaskManager();
      taskManager.Backup();

      ConsoleShell consoleShell = new ConsoleShell(taskManager, appConfig);
      consoleShell.MainLoop(args);
    }
  }
}
