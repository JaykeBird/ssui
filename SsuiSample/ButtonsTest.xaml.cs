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

        int executeCount = 0;

        private void Button_Execute(object sender, RoutedEventArgs e)
        {
            executeCount++;
            txtResult.Text = $"{(sender as FrameworkElement).Name} held, count {executeCount}";
        }

        private void Button_EndPress(object sender, RoutedEventArgs e)
        {
            executeCount = 0;
        }
    }
}
