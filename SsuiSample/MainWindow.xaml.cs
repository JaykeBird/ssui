using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using SolidShineUi;

namespace SsuiSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : FlatWindow
    {

        public MainWindow()
        {
            InitializeComponent();
            ColorScheme = new ColorScheme(ColorsHelper.CreateFromHex("7AE"));
            SetupSidebar();
        }

        private void mnuExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void mnuColors_Click(object sender, RoutedEventArgs e)
        {
            ColorPickerDialog cpd = new ColorPickerDialog(ColorScheme, ColorScheme.MainColor);
            cpd.ShowDialog();

            if (cpd.DialogResult)
            {
                ColorScheme = new ColorScheme(cpd.SelectedColor);
            }
        }

        private void mnuLightTheme_Click(object sender, RoutedEventArgs e)
        {
            ColorScheme = ColorScheme.CreateLightTheme(ColorScheme.AccentMainColor);
        }

        private void mnuDarkTheme_Click(object sender, RoutedEventArgs e)
        {
            ColorScheme = ColorScheme.CreateDarkTheme(ColorScheme.AccentMainColor);
        }

        private void mnuHcTheme1_Click(object sender, RoutedEventArgs e)
        {
            ColorScheme = ColorScheme.GetHighContrastScheme(HighContrastOption.WhiteOnBlack);
        }

        private void mnuHcTheme2_Click(object sender, RoutedEventArgs e)
        {
            ColorScheme = ColorScheme.GetHighContrastScheme(HighContrastOption.GreenOnBlack);
        }

        private void mnuHcTheme3_Click(object sender, RoutedEventArgs e)
        {
            ColorScheme = ColorScheme.GetHighContrastScheme(HighContrastOption.BlackOnWhite);
        }

        void SetupSidebar()
        {
            foreach (UserControl item in grdTests.Children)
            {
                SelectableItem si = new SelectableItem(item.GetType().Name.Replace("Test", ""))
                {
                    Tag = item.Name,
                    Padding = new Thickness(3),
                    Height = 28
                };
                si.Click += si_Click;
                stkTabs.AddItem(si);
            }
        }

        private void si_Click(object sender, EventArgs e)
        {
            string name = (sender as SelectableItem).Tag as string;

            foreach (UserControl item in grdTests.Children)
            {
                item.Visibility = item.Name == name ? Visibility.Visible : Visibility.Collapsed;
            }

            lblStart.Visibility = Visibility.Collapsed;
        }

        #region Help menu
#pragma warning disable IDE0017 // Simplify object initialization
        private void mnuWebsite_Click(object sender, RoutedEventArgs e)
        {
            ProcessStartInfo psi = new ProcessStartInfo("https://jaykebird.com/software/ssui");

            psi.UseShellExecute = true;
            Process.Start(psi);
        }

        private void mnuGitHub_Click(object sender, RoutedEventArgs e)
        {
            ProcessStartInfo psi = new ProcessStartInfo("https://github.com/JaykeBird/Ssui");
            psi.UseShellExecute = true;
            Process.Start(psi);
        }

        private void mnuAbout_Click(object sender, RoutedEventArgs e)
        {
            About a = new About();
            a.Owner = this;
            a.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            a.ColorScheme = ColorScheme;
            a.ShowDialog();
        }
#pragma warning restore IDE0017 // Simplify object initialization
        #endregion
    }
}
