using Planum.ConsoleUI;
using Planum.DataModels;
using Planum.Models.BuisnessLogic.IRepo;
using Planum.Models.BuisnessLogic.Managers;
using Planum.Models.DataModels;
using Serilog;

namespace Planum
{
    // NOTES:
    // maybe switch to spectre console package in the future

    // TODO:
    // add filters to update command - update selected with shown parameters
    // replace filter for standart filter in update, next, previous, delete commands
    // add range selection for ids
    // add mindmap
    // add dedicated static classes for different type of messages to ease programming and add general functions
    // add task/tag name for filters
    // find what cases strange error with archived tasks
    // implement backups
    // add parameters for create for default value

    // IN PROGRESS:
    // add calendar

    // 1.1.0 (calendar update)
    // added fix command for tasks
    // added mandatory task validation for task in order to fix doubles deletion
    // added the ability to create task only with start time and default deadline

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
