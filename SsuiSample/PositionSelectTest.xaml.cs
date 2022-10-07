using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using SolidShineUi;

namespace SsuiSample
{
    /// <summary>
    /// Interaction logic for RelativePositionSelectTest.xaml
    /// </summary>
    public partial class PositionSelectTest : UserControl
    {
        public PositionSelectTest()
        {
            InitializeComponent();
        }

        #region ColorScheme

        public event DependencyPropertyChangedEventHandler ColorSchemeChanged;

        public static DependencyProperty ColorSchemeProperty
            = DependencyProperty.Register("ColorScheme", typeof(ColorScheme), typeof(PositionSelectTest),
            new FrameworkPropertyMetadata(new ColorScheme(), new PropertyChangedCallback(OnColorSchemeChanged)));

        public static void OnColorSchemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorScheme cs = e.NewValue as ColorScheme;

            if (d is PositionSelectTest s)
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

        bool _internalAction = false;

        private void nudWidth1_ValueChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_internalAction) return;

            selPos1.SelectedWidth = nudWidth1.Value;
        }

        private void nudHeight1_ValueChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_internalAction) return;

            selPos1.SelectedHeight = nudHeight1.Value;
        }

        private void selPos1_SelectedPositionChanged(object sender, EventArgs e)
        {
            if (nudHeight1 == null) return;

            _internalAction = true;

            nudHeight1.Value = selPos1.SelectedHeight;
            nudWidth1.Value = selPos1.SelectedWidth;

            _internalAction = false;
        }

        private void chkSnap_CheckChanged(object sender, RoutedEventArgs e)
        {
            selPos1.SnapToSnapLines = chkSnap.IsChecked;
        }

        private void nudSnapsize_ValueChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            selPos1.SnapDistance = nudSnapsize.Value;
        }
    }
}
