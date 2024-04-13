using Avalonia;
using Avalonia.Controls;
using Avalonia.Dialogs;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using SolidShineUi;
using System.Diagnostics;

namespace SsuiSampleAvalonia
{
    public partial class MainWindow : Window
    {
        ColorScheme cs = new ColorScheme(Avalonia.Media.Colors.CornflowerBlue);

        public MainWindow()
        {
            InitializeComponent();

            btnMain.ColorScheme = cs;
            mnuMain.ColorScheme = cs;
            btnSplit.ColorScheme = cs;
            Background = cs.BackgroundColor.ToBrush();

            MenuItem mi = mnuNew;
            string clickName = MenuItem.ClickEvent.Name;
        }

        private void FlatButton_Click(object? sender, RoutedEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                Debugger.Log(0, "UI", "HELLO\n");
            }
            //AboutAvaloniaDialog aad = new AboutAvaloniaDialog();
            //aad.ShowDialog(this);
        }

        private void MenuItem_Click_1(object? sender, RoutedEventArgs e)
        {

        }

        private void mnuExit_Click(object? sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
