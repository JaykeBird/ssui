using Avalonia;
using Avalonia.Controls;
using Avalonia.Dialogs;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using SolidShineUi;
using System.Diagnostics;

namespace SsuiSampleAvalonia
{
    public partial class MainWindow : ThemedWindow
    {
        //ColorScheme cs = new ColorScheme(Avalonia.Media.Colors.CornflowerBlue);

        //public ColorScheme ColorScheme { get => GetValue(ColorSchemeProperty); set => SetValue(ColorSchemeProperty, value); }

        ///// <summary>The backing styled property for <see cref="ColorScheme"/>. See the related property for details.</summary>
        //public static readonly StyledProperty<ColorScheme> ColorSchemeProperty
        //    = AvaloniaProperty.Register<MainWindow, ColorScheme>(nameof(ColorScheme), new ColorScheme());

        public MainWindow()
        {
            InitializeComponent();

            SsuiTheme = new SsuiAppTheme(Avalonia.Media.Colors.CornflowerBlue);

            //ColorScheme = cs;
            //Background = cs.BackgroundColor.ToBrush();

            MenuItem mi = mnuNew;
            string clickName = MenuItem.ClickEvent.Name;
        }

        private async void FlatButton_Click(object? sender, RoutedEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                Debugger.Log(0, "UI", "HELLO\n");
            }

            StringInputDialog dialog = new StringInputDialog()
            {
                Title = "String Input Dialog Test",
                Description = "Write a message",
            };
            dialog.SsuiTheme = this.SsuiTheme;
            await dialog.DisplayAsWindow().ShowDialog(this);
            if (dialog.DialogResult)
            {
                Debugger.Log(0, "UI", dialog.Value);
            }
        }
        private void btnMain_RightClick(object? sender, RoutedEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                Debugger.Log(0, "UI", "RIGHT HELLO\n");
            }
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
