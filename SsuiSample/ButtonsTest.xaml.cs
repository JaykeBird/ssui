using SolidShineUi;
using System.Windows;
using System.Windows.Controls;

namespace SsuiSample
{
    /// <summary>
    /// Interaction logic for ButtonsTest.xaml
    /// </summary>
    public partial class ButtonsTest : UserControl
    {
        public ButtonsTest()
        {
            InitializeComponent();
        }

        #region ColorScheme

        /// <summary>
        /// Raised when the value of <see cref="ColorScheme"/> changed.
        /// </summary>
#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? ColorSchemeChanged;
#else
        public event DependencyPropertyChangedEventHandler ColorSchemeChanged;
#endif

        public static DependencyProperty ColorSchemeProperty
            = DependencyProperty.Register("ColorScheme", typeof(ColorScheme), typeof(ButtonsTest),
            new FrameworkPropertyMetadata(new ColorScheme(), new PropertyChangedCallback(OnColorSchemeChanged)));

        public static void OnColorSchemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is ColorScheme cs)
            {
                if (d is ButtonsTest s)
                {
                    s.ColorSchemeChanged?.Invoke(d, e);
                    s.ApplyColorScheme(cs);
                }
            }
        }

        public ColorScheme ColorScheme
        {
            get => (ColorScheme)GetValue(ColorSchemeProperty);
            set => SetValue(ColorSchemeProperty, value);
        }

        public void ApplyColorScheme(ColorScheme cs)
        {
            if (cs != ColorScheme)
            {
                ColorScheme = cs;
                return;
            }
        }
        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            txtResult.Text = $"{(sender as FrameworkElement)?.Name} was clicked";
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            var fb = sender as FlatButton;
            txtResult.Text = $"{fb?.Name} was clicked. IsSelected = {fb?.IsSelected}";
        }

        private void Button_RightClick(object sender, RoutedEventArgs e)
        {
            txtResult.Text = $"{(sender as FrameworkElement)?.Name} was right-clicked";
        }

        private void Button_MenuClick(object sender, RoutedEventArgs e)
        {
            txtResult.Text = $"{(sender as FrameworkElement)?.Name} was clicked on the menu";
        }
    }
}
