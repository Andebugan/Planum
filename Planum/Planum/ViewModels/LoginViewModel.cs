using Planum.Models.BuisnessLogic.Managers;
using ReactiveUI;

namespace Planum.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        protected IUserManager _userManager;
        protected ITaskManager _taskManager;
        protected ITagManager _tagManager;

        public LoginViewModel(IUserManager userManager, ITaskManager taskManager, ITagManager tagManager)
        {
            _userManager = userManager;
            _taskManager = taskManager;
            _tagManager = tagManager;
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

        public bool OnSignInClickCommand()
        {
            if (_userManager.SignIn(SignInLogin, SignInPassword) == null)
            {
                SignInLogin = "";
                SignInPassword = "";
                return false;
            }
            SignInLogin = "";
            SignInPassword = "";

            SignInMenuVisible = false;
            return true;
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
    }
}
