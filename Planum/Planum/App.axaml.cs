using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Planum.ViewModels;
using Planum.Views;
using Planum.DataModels;
using Planum.Models.BuisnessLogic.IRepo;
using Planum.Models.BuisnessLogic.Managers;
using Planum.Models.DataModels;

namespace Planum
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                IUserRepo userRepo = new UserRepoFile(new UserDTOComparator());
                ITaskRepo taskRepo = new TaskRepoFile(new TaskDTOComparator());
                ITagRepo tagRepo = new TagRepoFile(new TagDTOComparator());

                /*
                ((TaskRepoFile)taskRepo).Reset();
                ((TagRepoFile)tagRepo).Reset();
                ((UserRepoFile)userRepo).Reset();
                */

                ITaskConverter taskConverter = new TaskConverter();
                ITagConverter tagConverter = new TagConverter();
                IUserConverter userConverter = new UserConverter();

                IUserManager userManager = new UserManager(userRepo, userConverter);
                ITaskManager taskManager = new TaskManager(taskRepo, taskConverter, userManager);
                ITagManager tagManager = new TagManager(tagRepo, taskManager, tagConverter, userManager);

                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(userManager, taskManager, tagManager),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
