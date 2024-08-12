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

        public ColorScheme ColorScheme { get => GetValue(ColorSchemeProperty); set => SetValue(ColorSchemeProperty, value); }

        /// <summary>The backing styled property for <see cref="ColorScheme"/>. See the related property for details.</summary>
        public static readonly StyledProperty<ColorScheme> ColorSchemeProperty
            = AvaloniaProperty.Register<MainWindow, ColorScheme>(nameof(ColorScheme), new ColorScheme());

        public MainWindow()
        {
            InitializeComponent();

            ColorScheme = cs;
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
