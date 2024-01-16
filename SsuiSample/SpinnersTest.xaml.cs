using SolidShineUi;
using System;
using System.Collections.Generic;
using System.Text;
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
    /// Interaction logic for SpinnersTest.xaml
    /// </summary>
    public partial class SpinnersTest : UserControl
    {
        public SpinnersTest()
        {
            InitializeComponent();
        }

        #region ColorScheme

        public event DependencyPropertyChangedEventHandler ColorSchemeChanged;

        public static DependencyProperty ColorSchemeProperty
            = DependencyProperty.Register("ColorScheme", typeof(ColorScheme), typeof(SpinnersTest),
            new FrameworkPropertyMetadata(new ColorScheme(), new PropertyChangedCallback(OnColorSchemeChanged)));

        public static void OnColorSchemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorScheme cs = e.NewValue as ColorScheme;

            if (d is SpinnersTest s)
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

        private void nud9_ValueChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            brdrColor.Background = new SolidColorBrush(ColorsHelper.CreateFromHex(nud9.Value.ToString("X6")));
        }

        private void nud9_Loaded(object sender, RoutedEventArgs e)
        {
            brdrColor.Background = new SolidColorBrush(ColorsHelper.CreateFromHex(nud9.Value.ToString("X6")));
        }
    }
}
