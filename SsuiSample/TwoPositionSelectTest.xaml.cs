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
    public partial class TwoPositionSelectTest : UserControl
    {
        public TwoPositionSelectTest()
        {
            InitializeComponent();
        }

        #region ColorScheme

        public event DependencyPropertyChangedEventHandler ColorSchemeChanged;

        public static DependencyProperty ColorSchemeProperty
            = DependencyProperty.Register("ColorScheme", typeof(ColorScheme), typeof(TwoPositionSelectTest),
            new FrameworkPropertyMetadata(new ColorScheme(), new PropertyChangedCallback(OnColorSchemeChanged)));

        public static void OnColorSchemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorScheme cs = e.NewValue as ColorScheme;

            if (d is TwoPositionSelectTest s)
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

            selPos1.SelectedWidth1 = nudWidth1.Value;
        }

        private void nudHeight1_ValueChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_internalAction) return;

            selPos1.SelectedHeight1 = nudHeight1.Value;
        }

        private void nudWidth2_ValueChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_internalAction) return;

            selPos1.SelectedWidth2 = nudWidth2.Value;
        }

        private void nudHeight2_ValueChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_internalAction) return;

            selPos1.SelectedHeight2 = nudHeight2.Value;
        }

        private void selPos1_SelectedPositionChanged(object sender, EventArgs e)
        {
            if (nudHeight1 == null) return;

            _internalAction = true;

            nudHeight1.Value = selPos1.SelectedHeight1;
            nudWidth1.Value = selPos1.SelectedWidth1;

            nudHeight2.Value = selPos1.SelectedHeight2;
            nudWidth2.Value = selPos1.SelectedWidth2;

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
