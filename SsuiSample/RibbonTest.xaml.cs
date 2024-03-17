using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SolidShineUi;
using SolidShineUi.Ribbon;

namespace SsuiSample
{
    /// <summary>
    /// Interaction logic for RibbonTest.xaml
    /// </summary>
    public partial class RibbonTest : UserControl
    {
        public RibbonTest()
        {
            InitializeComponent();

            ribbonCbb.Items.Add("Item 1");
            ribbonCbb.Items.Add("Item 2");
            ribbonCbb.Items.Add("Item 3");
            ribbonCbb.SelectedIndex = 0;
        }


        #region Color Scheme
        /// <summary>
        /// Raised when the ColorScheme property is changed.
        /// </summary>
#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? ColorSchemeChanged;
#else
        public event DependencyPropertyChangedEventHandler ColorSchemeChanged;
#endif


        /// <summary>
        /// A dependency property object backing the related ColorScheme property. See <see cref="ColorScheme"/> for more details.
        /// </summary>
        public static readonly DependencyProperty ColorSchemeProperty
            = DependencyProperty.Register("ColorScheme", typeof(ColorScheme), typeof(RibbonTest),
            new FrameworkPropertyMetadata(new ColorScheme(), new PropertyChangedCallback(OnColorSchemeChanged)));

        /// <summary>
        /// Perform an action when the ColorScheme property has changed. Primarily used internally.
        /// </summary>
        /// <param name="d">The object containing the property that changed.</param>
        /// <param name="e">Event arguments about the property change.</param>
        public static void OnColorSchemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
#if NETCOREAPP
            ColorScheme cs = (e.NewValue as ColorScheme)!;
#else
            ColorScheme cs = e.NewValue as ColorScheme;
#endif

            if (d is RibbonTest c)
            {
                c.ColorSchemeChanged?.Invoke(d, e);
                c.ApplyColorScheme(cs);
            }
        }

        /// <summary>
        /// Get or set the color scheme used for this control. The color scheme can quickly apply a whole visual style to your control.
        /// </summary>
        public ColorScheme ColorScheme
        {
            get => (ColorScheme)GetValue(ColorSchemeProperty);
            set => SetValue(ColorSchemeProperty, value);
        }

        /// <summary>
        /// Apply a color scheme to this control. The color scheme can quickly apply a whole visual style to the control.
        /// </summary>
        /// <param name="cs">The color scheme to apply.</param>
        public void ApplyColorScheme(ColorScheme cs)
        {
            if (cs == null)
            {
                return;
            }
            if (cs != ColorScheme)
            {
                ColorScheme = cs;
                return;
            }
        }
        #endregion

        private void chkContextual_CheckChanged(object sender, RoutedEventArgs e)
        {
            tabContextual.Visibility = chkContextual.IsChecked ? Visibility.Visible : Visibility.Collapsed;
        }

        private void mnuWidths_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Ribbon width: " + ribbon.ActualWidth.ToString());
            sb.AppendLine();

            foreach (RibbonTab item in ribbon.Items)
            {
                var totalDesired = item.Items.Sum(i => i.DesiredSize.Width);
                var totalActual = item.Items.Sum(i => i.ActualWidth);
                sb.Append($"{item.Title}: desired width: {totalDesired}, actual: {totalActual}");
                sb.AppendLine(" " + (ribbon.ActualWidth > totalActual ? "ribbon size good" : "ribbon too small"));
            }

            txtStatus.Text = sb.ToString();
        }

        private void mnuGroupsCompacted_Click(object sender, RoutedEventArgs e)
        {
            if (ribbon.SelectedTab == null)
            {
                txtStatus.Text = "no tab selected!";
                return;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Selected tab: " + ribbon.SelectedTab.Title);
            sb.AppendLine();

            foreach (RibbonGroup item in ribbon.SelectedTab.Items)
            {
                sb.AppendLine($"{item.Title}: compacted: {item.CompactSize}, width: {item.ActualWidth}");
            }

            txtStatus.Text = sb.ToString();
        }
    }
}
