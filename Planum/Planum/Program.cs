using Planum.ConsoleUI;
using Planum.DataModels;
using Planum.Models.BuisnessLogic.IRepo;
using Planum.Models.BuisnessLogic.Managers;
using Planum.Models.DataModels;
using Serilog;

namespace Planum
{
    // TODO:
    // add range selection for ids
    // add overdue filer (possibly other time filters)
    // add current day filter
    // add task queue object
    // add tries for input with options (try again? (y/n))
    // replace filter for standart filter in update, next, previous commands

    // 1.0.1
    // added remove filer
    // fixed update tables for task
    // fixed complete command for repeated task
    // added version counter
    // fixed status filters (ignores incorrect filters)

    internal class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File("logs\\planumLogs.log", rollingInterval: RollingInterval.Day)
            .CreateLogger();

            IUserRepo userRepo = new UserRepoFile(new UserDTOComparator());
            ITaskRepo taskRepo = new TaskRepoFile(new TaskDTOComparator());
            ITagRepo tagRepo = new TagRepoFile(new TagDTOComparator());

            ITaskConverter taskConverter = new TaskConverter();
            ITagConverter tagConverter = new TagConverter();
            IUserConverter userConverter = new UserConverter();

            IUserManager userManager = new UserManager(userRepo, userConverter);
            ITaskManager taskManager = new TaskManager(taskRepo, taskConverter, userManager);
            ITagManager tagManager = new TagManager(tagRepo, taskManager, tagConverter, userManager);

            ConsoleShell console = new ConsoleShell(userManager, taskManager, tagManager);
            console.MainLoop();
        }
    }
}
