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

        public MainWindowViewModel(IUserManager userManager, ITaskManager taskManager, ITagManager tagManager)
        {
            _userManager = userManager;
            _taskManager = taskManager;
            _tagManager = tagManager;
        }

        // Tab controlls
        private bool _logInMenuTabVisible = false;
        public bool LogInMenuTabVisible
        {
            get => _logInMenuTabVisible;
            set
            {
                this.RaiseAndSetIfChanged(ref _logInMenuTabVisible, value);
            }
        }

        private bool _mainMenuTabVisible = false;
        public bool MainMenuTabVisible
        {
            get => _mainMenuTabVisible;
            set
            {
                this.RaiseAndSetIfChanged(ref _mainMenuTabVisible, value);
            }
        }

        private bool _settingsTabVisible = false;
        public bool SettingsTabVisible
        {
            get => _settingsTabVisible;
            set
            {
                this.RaiseAndSetIfChanged(ref _settingsTabVisible, value);
            }
        }

        private bool _listTabVisibe = false;
        public bool ListTabVisible
        {
            get => _listTabVisibe;
            set
            {
                this.RaiseAndSetIfChanged(ref _listTabVisibe, value);
            }
        }

        // Sign In
        private bool _signInMenuVisible = false;
        public bool SignInMenuVisible
        {
            get => _signInMenuVisible;
            set => this.RaiseAndSetIfChanged(ref _signInMenuVisible, value);
        }

        private string _signInLogin = "";
        public string SignInLogin
        {
            get => _signInLogin;
            set => this.RaiseAndSetIfChanged(ref _signInLogin, value);
        }

        private string _signInPassword = "";
        public string SignInPassword
        {
            get => _signInPassword;
            set => this.RaiseAndSetIfChanged(ref _signInPassword, value);
        }

        public void OnShowSignInMenuBtnClickCommand()
        {
            SignInMenuVisible = true;
            SignInLogin = "";
            SignInPassword = "";
        }

        public void OnReturnFromSignInBtnClickCommand()
        {
            SignInMenuVisible = false;
            SignInLogin = "";
            SignInPassword = "";
        }

        public void OnSignInClickCommand()
        {
            if (_userManager.SignIn(SignInLogin, SignInPassword) == null)
            {
                SignInLogin = "";
                SignInPassword = "";
                return;
            }
            SignInLogin = "";
            SignInPassword = "";

            LogInMenuTabVisible = false;
            MainMenuTabVisible = true;
            SignInMenuVisible = false;
        }

        // Sign Up
        private bool _signUpMenuVisible = false;
        public bool SignUpMenuVisible
        {
            get => _signUpMenuVisible;
            set => this.RaiseAndSetIfChanged(ref _signUpMenuVisible, value);
        }

        private string _signUpLogin = "";
        public string SignUpLogin
        {
            get => _signUpLogin;
            set => this.RaiseAndSetIfChanged(ref _signUpLogin, value);
        }

        private string _signUpPassword = "";
        public string SignUpPassword
        {
            get => _signUpPassword;
            set => this.RaiseAndSetIfChanged(ref _signUpPassword, value);
        }

        public void OnShowSignUpMenuBtnClickCommand()
        {
            SignUpMenuVisible = true;
            SignUpLogin = "";
            SignUpPassword = "";
        }

        public void OnReturnFromSignUpBtnClickCommand()
        {
            SignUpMenuVisible = false;
            SignUpLogin = "";
            SignUpPassword = "";
        }

        public void OnSignUpClickCommand()
        {
            if (string.IsNullOrEmpty(SignUpLogin) || string.IsNullOrEmpty(SignUpPassword))
            {
                SignUpLogin = "";
                SignUpPassword = "";
                return;
            }
            _userManager.CreateUser(SignUpLogin, SignUpPassword);
            SignUpLogin = "";
            SignUpPassword = "";
            SignUpMenuVisible = false;
        }

        // Main menu
        public void OnShowListTabBtnClick()
        {
            MainMenuTabVisible = false;
            ListTabVisible = true;
            LoadTasks();
        }

        public void OnShowSettingsTabBtnClick()
        {
            MainMenuTabVisible = false;
            SettingsTabVisible = true;
        }

        public void OnLogOutBtnClick()
        {
            MainMenuTabVisible = false;
            LogInMenuTabVisible = true;
            _userManager.CurrentUser = null;
        }

        // Settings

        private bool _changeUserMenuVisible = false;
        public bool ChangeUserMenuVisible
        {
            get => _changeUserMenuVisible;
            set
            {
                this.RaiseAndSetIfChanged(ref _changeUserMenuVisible, value);
            }
        }

        private string _changeUserLogin = "";
        public string ChangeUserLogin
        {
            get => _changeUserLogin;
            set => this.RaiseAndSetIfChanged(ref _changeUserLogin, value);
        }

        private string _changeUserPassword = "";
        public string ChangeUserPassword
        {
            get => _changeUserPassword;
            set => this.RaiseAndSetIfChanged(ref _changeUserPassword, value);
        }

        public void OnChangeProfileSettingsBtnClick()
        {
            ChangeUserMenuVisible = true;
            ChangeUserLogin = "";
            ChangeUserPassword = "";
        }

        public void OnChangeUserClickCommand()
        {
            if (string.IsNullOrEmpty(ChangeUserLogin) || string.IsNullOrEmpty(ChangeUserPassword))
            {
                ChangeUserLogin = "";
                ChangeUserPassword = "";
                return;
            }
            _userManager.UpdateUser(_userManager.CurrentUser.Id, ChangeUserLogin, ChangeUserPassword);
            ChangeUserLogin = "";
            ChangeUserPassword = "";
            ChangeUserMenuVisible = false;
        }

        public void OnDeleteProfileBtnClick()
        {
            _userManager.DeleteUser(_taskManager, _tagManager);
            LogInMenuTabVisible = true;
            SettingsTabVisible = false;
        }

        public void OnReturnFromSettingsBtnClick()
        {
            SettingsTabVisible = false;
            MainMenuTabVisible = true;
        }

        public void OnReturnFromChangeUserBtnClickCommand()
        {
            ChangeUserMenuVisible = false;
            ChangeUserLogin = "";
            ChangeUserPassword = "";
        }

        // List view

        private ObservableCollection<TaskViewDTO> _taskList;
        public ObservableCollection<TaskViewDTO> TaskList
        { 
            get => _taskList;
            set => this.RaiseAndSetIfChanged(ref _taskList, value);
        }

        public void LoadTasks()
        {
            List<Task> tasks = _taskManager.GetAllTasks();
            TaskList = new ObservableCollection<TaskViewDTO>();
            foreach (Task task in tasks)
            {
                TaskList.Add(new TaskViewDTO(task.Id, task.StartTime, task.Deadline, task.RepeatPeriod, task.TagIds,
                    task.ParentIds, task.ChildIds, task.Name, task.Timed, task.UserId, task.Description, task.IsRepeated));
            }
        }

        public void OnReturnFromListTabBtnClick()
        {
            ListTabVisible = false;
            MainMenuTabVisible = true;
        }
    }
}
