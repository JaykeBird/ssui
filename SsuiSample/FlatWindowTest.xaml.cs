using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SolidShineUi;

namespace SsuiSample
{
    /// <summary>
    /// Interaction logic for FlatWindowTest.xaml
    /// </summary>
    public partial class FlatWindowTest : ThemedUserControl
    {
        public FlatWindowTest()
        {
            InitializeComponent();
        }

        FlatWindow fwRunning = null;

//#if NETCOREAPP
//        FlatWindow? fwRunning = null;
//#else
//        FlatWindow fwRunning = null;
//#endif

        Color selColor = Colors.Salmon;

        private void btnColorSet_Click(object sender, RoutedEventArgs e)
        {
            ColorPickerDialog cpd = new ColorPickerDialog(selColor);
            cpd.SsuiTheme = TryGetSsuiAppTheme();
            cpd.ShowDialog();

            if (cpd.DialogResult)
            {
                selColor = cpd.SelectedColor;
                rdoCustomColor.IsChecked = true;
            }
        }

        SsuiAppTheme TryGetSsuiAppTheme()
        {
            if (SsuiTheme is SsuiAppTheme sat)
            {
                // in most cases, it should be this - the inherited SsuiTheme should be an SsuiAppTheme
                return sat;
            }
            else if (Window.GetWindow(this) is ThemedWindow fw)
            {
                // okay, let's try to pull from the parent window if possible, as it should have a SsuiAppTheme as its theme
                return fw.SsuiTheme;
            }
            else
            {
                // okay, I guess we'll just go with the default
                return new SsuiAppTheme();
            }
        }

        private void btnDisplay_Click(object sender, RoutedEventArgs e)
        {
            if (fwRunning != null)
            {
                fwRunning.Focus();
                return;
            }

            FlatWindow fw = new FlatWindow
            {
                Width = nudWidth.Value,
                Height = nudHeight.Value,
                SsuiTheme = rdoCurrentColor.IsChecked.GetValueOrDefault(true) ? TryGetSsuiAppTheme() : new SsuiAppTheme(selColor),
            };

            if (btnSetIcon.SelectedFiles.Count > 0)
            {
                fw.Icon = new BitmapImage(new Uri(btnSetIcon.SelectedFiles[0]));
                fw.ShowIcon = true;
            }
            else
            {
                fw.ShowIcon = false;
            }

            fw.SourceInitialized += fw_SourceInitialized;
            fw.Closed += Fw_Closed;

            // set up the TopRightElement
            if (chkTopRight.IsChecked)
            {
                // TopRightElement can also be set via XAML, but here I'm using C#
                TextBlock tb = new TextBlock();
                tb.Text = "Top-right element!";
                tb.Margin = new Thickness(0, 0, 5, 0);
                tb.VerticalAlignment = VerticalAlignment.Center;

                fw.TopRightElement = tb;
            }

            // set which buttons to display in the top-right of the title bar (caption bar)
            // The CaptionDisplayType property is purely visual; the DisableMinimizeAction and DisableMaximizeAction functions are the actual functions that prevent these actions
            if (!chkMaximize.IsChecked)
            {
                if (!chkMinimize.IsChecked)
                {
                    fw.CaptionDisplayType = CaptionType.Close;
                }
                else
                {
                    fw.CaptionDisplayType = CaptionType.MinimizeClose;
                }
            }
            else if (!chkMinimize.IsChecked)
            {
                fw.CaptionDisplayType = CaptionType.MaximizeClose;
            }
            else
            {
                fw.CaptionDisplayType = CaptionType.Full;
            }

            // set the window's title bar
            fw.Title = txtTitle.Text;

            if (rdoDisplayText.IsChecked.GetValueOrDefault(true))
            {
                fw.ShowTitle = true; // it's already true by default, but I'm putting it here for clarity
            }
            else if (rdoDisplayImage.IsChecked.GetValueOrDefault(false))
            {
                fw.ShowTitle = false;

                // set up a TopLeftElement
                // with ShowTitle being set to false, you can create a different UI element to appear in the place of the title
                // (if you still show the title ("ShowTitle = true"), the TopLeftElement is displayed to the left of the window title)
                // just like TopRightElement, this can be set via XAML but I'm creating it in C# here
                Image uii = new Image
                {
                    Height = 16,
                    Width = 16,
                    Margin = new Thickness(5, 0, 0, 0),
                    Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/SmileyColor.png", UriKind.RelativeOrAbsolute)),
                };

                fw.TopLeftElement = uii;
            }
            else
            {
                fw.ShowTitle = false;
            }

            fw.Show();
            fwRunning = fw;
            btnShowProperties.Visibility = Visibility.Visible;
            propList.LoadObject(fw);
        }

        private void Fw_Closed(object sender, EventArgs e)
        {
            fwRunning = null;
            colProperties.MinWidth = 0;
            colProperties.Width = new GridLength(0, GridUnitType.Pixel);
            btnShowProperties.Visibility = Visibility.Collapsed;
            propList.Clear();
        }

        private void fw_SourceInitialized(object sender, EventArgs e)
        {
            if (sender is FlatWindow fw)
            {
                Dispatcher.Invoke(() =>
                {
                    if (!chkMaximize.IsChecked)
                    {
                        fw.DisableMaximizeAction();
                    }

                    if (!chkMinimize.IsChecked)
                    {
                        fw.DisableMinimizeAction();
                    }
                });
            }
        }

        private void btnShowProperties_Click(object sender, RoutedEventArgs e)
        {
            if (colProperties.ActualWidth > 0)
            {
                colProperties.MinWidth = 0;
                colProperties.Width = new GridLength(0, GridUnitType.Pixel);
            }
            else
            {
                colProperties.Width = new GridLength(4, GridUnitType.Star);
                colProperties.MinWidth = 6;
            }
        }

        private void control_Unloaded(object sender, RoutedEventArgs e)
        {
            fwRunning?.Close();
        }

        private void btnSetIcon_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (btnSetIcon.SelectedFiles.Count > 0)
            {
                imgIcon.Source = new BitmapImage(new Uri(btnSetIcon.SelectedFiles[0]));
            }
            else
            {
                imgIcon.Source = null;
            }
        }
    }
}
