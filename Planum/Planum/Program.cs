using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.ReactiveUI;
using Planum.ConsoleUI;
using Planum.ConsoleUI.ConsoleCommands;
using Planum.DataModels;
using Planum.Models.BuisnessLogic.IRepo;
using Planum.Models.BuisnessLogic.Managers;
using Planum.Models.DataModels;
using System;
using System.Collections.Generic;


namespace Planum
{
    internal class Program
    {
        /*
        public static void Main(string[] args)
        {
            
            IUserRepo userRepo = new UserRepoFile(new UserDTOComparator());
            ITaskRepo taskRepo = new TaskRepoFile(new TaskDTOComparator());
            ITagRepo tagRepo = new TagRepoFile(new TagDTOComparator());

            //((TaskRepoFile)taskRepo).Reset();
            //((TagRepoFile)tagRepo).Reset();
            //((UserRepoFile)userRepo).Reset();

            ITaskConverter taskConverter = new TaskConverter();
            ITagConverter tagConverter = new TagConverter();
            IUserConverter userConverter = new UserConverter();

            IUserManager userManager = new UserManager(userRepo, userConverter);
            ITaskManager taskManager = new TaskManager(taskRepo, taskConverter, userManager);
            ITagManager tagManager = new TagManager(tagRepo, taskManager, tagConverter, userManager);

            List<ICommand> commands = new List<ICommand>() {
                new ExitCommand(),
                new DeleteUserCommand(userManager, taskManager, tagManager),
                new LogInCommand(userManager),
                new LogOutCommand(userManager),
                new ShowCurrentUserCommand(userManager),
                new ShowAllUsersCommand(userManager),
                new ShowUserCommand(userManager),
                new SignUpCommand(userManager),
                new UpdateUserCommand(userManager),

                new CreateTagCommand(tagManager, userManager),
                new DeleteTagCommand(tagManager, userManager),
                new DeleteAllTagsCommand(tagManager, userManager),
                new ShowAllTagsCommand(tagManager, userManager),
                new ShowTagCommand(tagManager, userManager),
                new UpdateTagCommand(tagManager, userManager),

                new AddChildCommand(taskManager, tagManager, userManager),
                new AddParentCommand(taskManager, userManager),
                new ArchiveTaskCommand(taskManager, userManager),
                new ClearChildrenCommand(taskManager, userManager),
                new ClearParentsCommand(taskManager, userManager),
                new ClearTagsCommand(taskManager, userManager),
                new CreateTaskCommand(taskManager, userManager),
                new DeleteAllTasksCommand(taskManager, userManager),
                new DeleteTaskCommand(taskManager, userManager),
                new RemoveChildCommand(taskManager, userManager),
                new RemoveParentCommand(taskManager, userManager),
                new RemoveTagCommand(taskManager, userManager),
                new ShowAllArchivedTasksCommand(taskManager, userManager),
                new ShowAllTasksCommand(taskManager, userManager),
                new ShowArchivedTaskCommand(taskManager, userManager),
                new ShowTaskCommand(taskManager, userManager),
                new UnarchiveTaskCommand(taskManager, userManager),
                new UpdateTaskCommand(taskManager, userManager),
            };
            ConsoleShell consoleShell = new ConsoleShell(commands);
            consoleShell.MainLoop();
        }
        */

        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        
        [STAThread]
        public static void Main(string[] args)
        {
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
