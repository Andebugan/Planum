using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.ReactiveUI;
using Planum.ConsoleUI;
using Planum.ConsoleUI.ConsoleCommands;
using Planum.DataModels;
using Planum.Models.BuisnessLogic.IRepo;
using Planum.Models.BuisnessLogic.Managers;
using Planum.Models.DataModels;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;


namespace Planum
{
    internal class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        

        [STAThread]
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Debug()
            .WriteTo.File("logs\\planumLogs.log", rollingInterval: RollingInterval.Day)
            .CreateLogger();

            string displayType = Config.ConfigData.LoadConfig().DisplayType;
            if (displayType == "console")
            {
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
            if (displayType == "desktop")
                BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace()
                .UseReactiveUI();
    }
}
