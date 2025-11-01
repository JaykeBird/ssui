using System;
using System.Windows;
using SolidShineUi;

namespace SsuiSample
{
    /// <summary>
    /// Interaction logic for ButtonsTest.xaml
    /// </summary>
    public partial class ButtonsTest : ThemedUserControl
    {
        public ButtonsTest()
        {
            InitializeComponent();
        }

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

        // this can be used if you have buttons that you want to apply special colors to, while also using the SsuiTheme
        // when the SsuiTheme is set, all existing brush data is overwritten with the brushes in the SsuiTheme
        // (the recommended scenario, though, is to set an accent theme and then set the button to use the accent theme)
        private void Button_SsuiThemeChanged(object sender, RoutedEventArgs e)
        {
            if (sender is FlatButton fb)
            {
                fb.Background = ColorsHelper.Purple.ToBrush();
                // fb.ForegroundHighlightBrush = ColorsHelper.Orange.ToBrush();
            }
        }
    }
}
