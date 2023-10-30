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
    /// Interaction logic for ButtonsTest.xaml
    /// </summary>
    public partial class ButtonsTest : UserControl
    {
        public ButtonsTest()
        {
            InitializeComponent();
        }

        #region ColorScheme

        public event DependencyPropertyChangedEventHandler ColorSchemeChanged;

        public static DependencyProperty ColorSchemeProperty
            = DependencyProperty.Register("ColorScheme", typeof(ColorScheme), typeof(ButtonsTest),
            new FrameworkPropertyMetadata(new ColorScheme(), new PropertyChangedCallback(OnColorSchemeChanged)));

        public static void OnColorSchemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorScheme cs = e.NewValue as ColorScheme;

            if (d is ButtonsTest s)
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            txtResult.Text = $"{(sender as FrameworkElement).Name} was clicked";
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            FlatButton fb = sender as FlatButton;
            txtResult.Text = $"{fb.Name} was clicked. IsSelected = {fb.IsSelected}";
        }

        private void Button_RightClick(object sender, RoutedEventArgs e)
        {
            txtResult.Text = $"{(sender as FrameworkElement).Name} was right-clicked";
        }

        private void Button_MenuClick(object sender, RoutedEventArgs e)
        {
            txtResult.Text = $"{(sender as FrameworkElement).Name} was clicked on the menu";
        }
    }
}
