using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.ReactiveUI;
using Planum.ConsoleUI;
using Planum.ConsoleUI.ConsoleCommands;
using Planum.Models.BuisnessLogic.IRepo;
using Planum.Models.BuisnessLogic.Managers;
using Planum.Models.DataModels;
using System;
using System.Collections.Generic;

namespace Planum
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            ITaskRepo taskRepo = new TaskRepoFile();
            ITagRepo tagRepo = new TagRepoFile();
            IUserRepo userRepo = new UserRepoFile();

            ((TaskRepoFile)taskRepo).Reset();
            ((TagRepoFile)tagRepo).Reset();
            ((UserRepoFile)userRepo).Reset();

            ITaskConverter taskConverter = new TaskConverter();
            ITagConverter tagConverter = new TagConverter();
            IUserConverter userConverter = new UserConverter();

            ITaskManager taskManager = new TaskManager(taskRepo, taskConverter);
            ITagManager tagManager = new TagManager(tagRepo, taskManager, tagConverter);
            IUserManager userManager = new UserManager(userRepo, tagManager, taskManager, userConverter);

            List<ICommand> commands = new List<ICommand>() {
                new ExitCommand(),
                new DeleteUserCommand(userManager),
                new LogInCommand(userManager),
                new LogOutCommand(userManager),
                new ShowCurrentUserCommand(userManager),
                new ShowAllUsersCommand(userManager),
                new ShowUserCommand(userManager),
                new SignUpCommand(userManager),
                new UpdateUserCommand(userManager),
                new DeleteAllUsersCommand(userManager),
            };
            ConsoleShell consoleShell = new ConsoleShell(commands);
            consoleShell.MainLoop();
        }
        /*
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args) => BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace()
                .UseReactiveUI();
        */
    }
}
