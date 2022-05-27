using System.Diagnostics;

namespace Planum.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public string Greeting => "Welcome to Avalonia!";

        public void OnClickCommand()
        {
            Debug.WriteLine("Clicked button");
        }
    }
}
