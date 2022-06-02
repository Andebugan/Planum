using Planum.Models.BuisnessLogic.Entities;
using Planum.Models.BuisnessLogic.Managers;
using ReactiveUI;
using Serilog;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Planum.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        protected IUserManager _userManager;
        protected ITaskManager _taskManager;
        protected ITagManager _tagManager;

        protected LoginViewModel LoginViewModel { get; }
        protected MainMenuViewModel MainMenuViewModel { get; }
        protected SettingsViewModel SettingsViewModel { get; }
        protected TaskListViewModel TaskListViewModel { get; }

        private ViewModelBase _currentWindow;
        public ViewModelBase CurrentWindow
        {
            get => _currentWindow;
            set => this.RaiseAndSetIfChanged(ref _currentWindow, value);
        }

        public MainWindowViewModel(IUserManager userManager, ITaskManager taskManager, ITagManager tagManager,
            ITagViewDTOConverter tagViewDTOConverter, ITaskViewDTOConverter taskViewDTOConverter)
        {
            _userManager = userManager;
            _taskManager = taskManager;
            _tagManager = tagManager;
            LoginViewModel = new LoginViewModel(userManager, taskManager, tagManager);
            MainMenuViewModel = new MainMenuViewModel();
            SettingsViewModel = new SettingsViewModel(userManager, taskManager, tagManager);
            TaskListViewModel = new TaskListViewModel(userManager, taskManager, tagManager, tagViewDTOConverter, taskViewDTOConverter);
            CurrentWindow = LoginViewModel;
        }

        public void OnSignInBtnClick()
        {
            Log.Information("Sign in button clicked");
            if (LoginViewModel.OnSignInClickCommand())
                CurrentWindow = MainMenuViewModel;
        }

        public void OnShowListTabBtnClick()
        {
            Log.Information("Show list tab button clicked");
            CurrentWindow = TaskListViewModel;
            TaskListViewModel.LoadTasks();
            TaskListViewModel.LoadTags();
        }

        public void OnShowSettingsTabBtnClick()
        {
            Log.Information("Show settings tab button clicked");
            CurrentWindow = SettingsViewModel;
        }

        public void OnLogOutBtnClick()
        {
            Log.Information("Log out button clicked");
            _userManager.CurrentUser = null;
            CurrentWindow = LoginViewModel;
        }

        public void OnDeleteProfileBtnClick()
        {
            Log.Information("Delete profile button clicked");
            _userManager.DeleteUser(_taskManager, _tagManager);
            CurrentWindow = LoginViewModel;
        }

        public void OnReturnFromSettingsBtnClick()
        {
            Log.Information("Return from settings button clicked");
            CurrentWindow = MainMenuViewModel;
        }

        public void OnReturnFromListTabBtnClick()
        {
            Log.Information("Return from list tab button clicked");
            CurrentWindow = MainMenuViewModel;
        }
    }
}
