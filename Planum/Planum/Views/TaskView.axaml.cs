using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Planum.Views
{
    public partial class TaskView : UserControl
    {
        public TaskView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
