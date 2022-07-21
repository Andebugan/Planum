using Planum.ConsoleUI;
using Planum.DataModels;
using Planum.Models.BuisnessLogic.IRepo;
using Planum.Models.BuisnessLogic.Managers;
using Planum.Models.DataModels;
using Serilog;

namespace Planum
{
    // TODO:
    // add filters to update command, if more then one - give choice (which task do you wish to update)
    // replace filter for standart filter in update, next, previous commands
    // add range selection for ids
    // add mindmap
    // add dedicated static classes for different type of messages to ease programming and add general functions
    // add task/tag name for filters

    // IN PROGRESS:
    // add calendar

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
