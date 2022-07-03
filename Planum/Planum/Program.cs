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
    // add task queue object
    // replace filter for standart filter in update, next, previous commands
    // upgrade help command (add stuff like help [name] and ect)
    // add commands for "no children", "no parents", "no tags", "no status" filters

    // IN PROGRESS:

    // 1.0.2
    // added -np, -nc, -nt, -nsq flags
    // fixed double show bug for -f-i show filter for tag
    // added parameters for update (opportunity to change selected fields like -d - description, -s - status and ect)
    // removed status id update when status index list wasn't changed

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
