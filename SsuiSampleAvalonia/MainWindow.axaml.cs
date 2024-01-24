using Avalonia;
using Avalonia.Controls;
using Avalonia.Dialogs;
using Avalonia.Markup.Xaml;
using SolidShineUi;

namespace SsuiSampleAvalonia
{
    public partial class MainWindow : Window
    {
        ColorScheme cs = new ColorScheme(Avalonia.Media.Colors.Sienna);

        public MainWindow()
        {
            InitializeComponent();

            btnMain.ColorScheme = cs;
            mnuMain.ColorScheme = cs;
        }

        private void FlatButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {

            //AboutAvaloniaDialog aad = new AboutAvaloniaDialog();
            //aad.ShowDialog(this);
        }
    }
}
