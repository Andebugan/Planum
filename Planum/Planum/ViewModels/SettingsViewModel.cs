using Planum.Models.BuisnessLogic.Managers;
using ReactiveUI;

namespace Planum.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        protected IUserManager _userManager;
        protected ITaskManager _taskManager;
        protected ITagManager _tagManager;

        public SettingsViewModel(IUserManager userManager, ITaskManager taskManager, ITagManager tagManager)
        {
            _userManager = userManager;
            _taskManager = taskManager;
            _tagManager = tagManager;
        }

        private bool _changeUserMenuVisible = false;
        public bool ChangeUserMenuVisible
        {
            get => _changeUserMenuVisible;
            set =>  this.RaiseAndSetIfChanged(ref _changeUserMenuVisible, value);
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

        public void OnReturnFromChangeUserBtnClickCommand()
        {
            ChangeUserMenuVisible = false;
            ChangeUserLogin = "";
            ChangeUserPassword = "";
        }
    }
}
