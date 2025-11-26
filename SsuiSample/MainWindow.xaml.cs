using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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

            defaultCulture = CultureInfo.CurrentCulture;

            ColorScheme cs = new ColorScheme(ColorsHelper.CreateFromHex("#77AAEE"), ColorsHelper.CreateFromHex("#EEAA77"));
            //cs.MenusUseAccent = true;
            ColorScheme = cs;

            SetupSidebar();
            KeyDown += (s, e) =>
            {
                if (e.Key == Key.F8)
                {
                    Point pos = Mouse.GetPosition(this);
                    if (TestIfPointIsMaximizeButton(pos))
                    {
                        Debug.Print("MAX BUTTON");
                    }
                    else
                    {
                        Debug.Print("NOT MAX BUTTON");
                    }
                }
            };
        }

        private bool TestIfPointIsMaximizeButton(Point p)
        {
            if (p.Y > CaptionHeight + 3) return false;

            double borderTop = BorderThickness.Top;
            double borderRight = BorderThickness.Right;
            // next, let's get the width of a button
            int captionFontSize = 16; // the caption font size is hardset in the theme, in the future maybe I'll open it to be changed
            double btnWidth = captionFontSize + CaptionButtonPadding.Left + CaptionButtonPadding.Right;
            double btnHeight = captionFontSize + CaptionButtonPadding.Top + CaptionButtonPadding.Bottom;

            double maxButtonRightBound = ActualWidth - borderRight - btnWidth; // this is the right edge of the maximize/restore button
            double maxButtonLeftBound = maxButtonRightBound - btnWidth; // this is the left edge of the maximize/restore button
            double maxButtonTopBound = borderTop;
            double maxButtonBottomBound = borderTop + btnHeight;

            // if true, this is MAXIMIZE BUTTON
            return p.X > maxButtonLeftBound && p.X < maxButtonRightBound && p.Y > maxButtonTopBound && p.Y < maxButtonBottomBound;
        }

        private void mnuExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        CultureInfo defaultCulture;

        private void mnuCulture_Click(object sender, RoutedEventArgs e)
        {
            StringInputDialog sid = new StringInputDialog(ColorScheme, "Set Current Culture", "Set the culture to use for this window", CultureInfo.CurrentCulture.Name);
            sid.ValidationFunction = (v) =>
            {
                if (string.IsNullOrWhiteSpace(v)) return false;
                try
                {
                    CultureInfo ci = new CultureInfo(v);
                }
                catch (CultureNotFoundException)
                {
                    if (string.Equals(v, "invariant", StringComparison.InvariantCultureIgnoreCase) || string.Equals(v, "default", StringComparison.InvariantCultureIgnoreCase))
                    {
                        // special case for invariant or default (use system) culture
                        return true;
                    }
                    return false;
                }
                return true;
            };
            sid.ValidationFailureString = "Not a valid culture name";
            sid.ShowDialog();

            if (sid.DialogResult)
            {
                if (string.Equals(sid.Value, "invariant", StringComparison.InvariantCultureIgnoreCase))
                {
                    Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                    Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
                }
                else if (string.Equals(sid.Value, "default", StringComparison.InvariantCultureIgnoreCase))
                {
                    Thread.CurrentThread.CurrentCulture = defaultCulture;
                    Thread.CurrentThread.CurrentUICulture = defaultCulture;
                }
                else
                {
                    Thread.CurrentThread.CurrentCulture = new CultureInfo(sid.Value);
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo(sid.Value);
                }
            }
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

        private void mnuDefaultCs_Click(object sender, RoutedEventArgs e)
        {
            ColorScheme = new ColorScheme();
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
                stkTabs.Items.Add(si);
            }
        }

        private void si_Click(object sender, RoutedEventArgs e)
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
