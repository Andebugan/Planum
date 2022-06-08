using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Planum.Views
{
    public partial class TagView : UserControl
    {
        public TagView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
