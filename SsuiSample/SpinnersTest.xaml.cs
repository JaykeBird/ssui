using SolidShineUi;
using System;
using System.Windows;
using System.Windows.Media;

namespace SsuiSample
{
    /// <summary>
    /// Interaction logic for SpinnersTest.xaml
    /// </summary>
    public partial class SpinnersTest : ThemedUserControl
    {
        public SpinnersTest()
        {
            InitializeComponent();
        }

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
