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
    /// Interaction logic for FlatWindowTest.xaml
    /// </summary>
    public partial class FlatWindowTest : UserControl
    {
        public FlatWindowTest()
        {
            InitializeComponent();
        }

        #region ColorScheme

        public event DependencyPropertyChangedEventHandler ColorSchemeChanged;

        public static DependencyProperty ColorSchemeProperty
            = DependencyProperty.Register("ColorScheme", typeof(ColorScheme), typeof(FlatWindowTest),
            new FrameworkPropertyMetadata(new ColorScheme(), new PropertyChangedCallback(OnColorSchemeChanged)));

        public static void OnColorSchemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorScheme cs = e.NewValue as ColorScheme;

            if (d is FlatWindowTest s)
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

        Color selColor = Colors.Salmon;

        private void btnColorSet_Click(object sender, RoutedEventArgs e)
        {
            ColorPickerDialog cpd = new ColorPickerDialog(ColorScheme, selColor);
            cpd.ShowDialog();

            if (cpd.DialogResult)
            {
                selColor = cpd.SelectedColor;
                rdoCustomColor.IsChecked = true;
            }
        }

        private void btnDisplay_Click(object sender, RoutedEventArgs e)
        {
            FlatWindow fw = new FlatWindow
            {
                Width = nudWidth.Value,
                Height = nudHeight.Value,
                ColorScheme = rdoCurrentColor.IsChecked.GetValueOrDefault(true) ? ColorScheme : new ColorScheme(selColor),
                //CornerRadius = new CornerRadius(nudCornerRadius.Value)
            };

            fw.SourceInitialized += fw_SourceInitialized;

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
                    fw.CaptionDisplayType = ChromeButtons.CaptionType.Close;
                }
                else
                {
                    fw.CaptionDisplayType = ChromeButtons.CaptionType.MinimizeClose;
                }
            }
            else if (!chkMinimize.IsChecked)
            {
                fw.CaptionDisplayType = ChromeButtons.CaptionType.MaximizeClose;
            }
            else
            {
                fw.CaptionDisplayType = ChromeButtons.CaptionType.Full;
            }

            // set the window's title bar
            fw.Title = txtTitle.Text;

            if (rdoDisplayText.IsChecked.GetValueOrDefault(true))
            {
                fw.ShowTitle = true; // it's already true by default, but I'm putting this here anyway lol
            }
            else if (rdoDisplayImage.IsChecked.GetValueOrDefault(false))
            {
                fw.ShowTitle = false;

                // set up a TopLeftElement
                // with ShowTitle being set to false, you can create a different UI element to appear in the place of the title
                // (if you still show the title ("ShowTitle = true"), the TopLeftElement is displayed even left of the window title)
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
    }
}
