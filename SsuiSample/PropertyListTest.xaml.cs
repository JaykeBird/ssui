using SolidShineUi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
