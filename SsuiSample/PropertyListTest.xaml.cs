using SolidShineUi;
using System.Windows;
using System.Windows.Controls;

namespace SsuiSample
{
    /// <summary>
    /// Interaction logic for PropertyListTest.xaml
    /// </summary>
    public partial class PropertyListTest : UserControl
    {
        public PropertyListTest()
        {
            InitializeComponent();
        }

        #region ColorScheme

        public event DependencyPropertyChangedEventHandler ColorSchemeChanged;

        public static DependencyProperty ColorSchemeProperty
            = DependencyProperty.Register("ColorScheme", typeof(ColorScheme), typeof(PropertyListTest),
            new FrameworkPropertyMetadata(new ColorScheme(), new PropertyChangedCallback(OnColorSchemeChanged)));

        public static void OnColorSchemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorScheme cs = e.NewValue as ColorScheme;

            if (d is PropertyListTest s)
            {
                s.ColorSchemeChanged?.Invoke(d, e);
                s.ApplyColorScheme(cs);
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

        private void btnSel1_Click(object sender, RoutedEventArgs e)
        {
            prop.LoadObject(cbbComboBox);
        }

        private void btnSel2_Click(object sender, RoutedEventArgs e)
        {
            prop.LoadObject(txtTextBox);
        }

        private void btnSel3_Click(object sender, RoutedEventArgs e)
        {
            prop.LoadObject(btnButton);
        }

        private void btnSel4_Click(object sender, RoutedEventArgs e)
        {
            prop.LoadObject(ctrFileSelect);
        }
    }
}
