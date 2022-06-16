using Planum.ConsoleUI;
using Planum.ConsoleUI.ConsoleCommands;
using Planum.ConsoleUI.ConsoleViews;
using Planum.DataModels;
using Planum.Models.BuisnessLogic.Entities;
using Planum.Models.BuisnessLogic.IRepo;
using Planum.Models.BuisnessLogic.Managers;
using Planum.Models.DataModels;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;


namespace Planum
{
    // TODO:
    // add move status command with steps
    // fix filter holes - sometimes selector gets bugged and does not detect false filters

    internal class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Debug()
            .WriteTo.File("logs\\planumLogs.log", rollingInterval: RollingInterval.Day)
            .CreateLogger();

            IUserRepo userRepo = new UserRepoFile(new UserDTOComparator());
            ITaskRepo taskRepo = new TaskRepoFile(new TaskDTOComparator());
            ITagRepo tagRepo = new TagRepoFile(new TagDTOComparator());

            //((TaskRepoFile)taskRepo).Reset();

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
