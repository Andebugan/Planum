using Planum.Models.BuisnessLogic.Entities;
using Planum.Models.BuisnessLogic.Managers;
using ReactiveUI;
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

        public MainWindowViewModel(IUserManager userManager, ITaskManager taskManager, ITagManager tagManager)
        {
            _userManager = userManager;
            _taskManager = taskManager;
            _tagManager = tagManager;
            LoginViewModel = new LoginViewModel(userManager, taskManager, tagManager);
            MainMenuViewModel = new MainMenuViewModel();
            SettingsViewModel = new SettingsViewModel(userManager, taskManager, tagManager);
            TaskListViewModel = new TaskListViewModel(userManager, taskManager, tagManager);
            CurrentWindow = LoginViewModel;
        }

        public void OnSignInBtnClick()
        {
            if (LoginViewModel.OnSignInClickCommand())
                CurrentWindow = MainMenuViewModel;
        }

        public void OnShowListTabBtnClick()
        {
            CurrentWindow = TaskListViewModel;
            TaskListViewModel.LoadTasks();
            TaskListViewModel.LoadTags();
        }

        public void OnShowSettingsTabBtnClick()
        {
            CurrentWindow = SettingsViewModel;
        }

        public void OnLogOutBtnClick()
        {
            _userManager.CurrentUser = null;
            CurrentWindow = LoginViewModel;
        }

        public void OnDeleteProfileBtnClick()
        {
            _userManager.DeleteUser(_taskManager, _tagManager);
            CurrentWindow = LoginViewModel;
        }

        public void OnReturnFromSettingsBtnClick()
        {
            CurrentWindow = MainMenuViewModel;
        }

        public void OnReturnFromListTabBtnClick()
        {
            CurrentWindow = MainMenuViewModel;
        }
    }
}
