using System;
using System.Collections.Generic;
using System.Windows;
using SolidShineUi;

namespace SsuiSample
{
    /// <summary>
    /// Interaction logic for RelativePositionSelectTest.xaml
    /// </summary>
    public partial class PositionSelectTest : ThemedUserControl
    {
        public PositionSelectTest()
        {
            InitializeComponent();
        }

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
