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

        private void selPos1_SelectedPositionChanged(object sender, RoutedPropertyChangedEventArgs<Point> e)
        {
            // this event handler is raised whenever the selected position (the location of the selector) is changed
            // e.OldValue and e.NewValue can be used to check and compare the change in values
            // (note that while the mouse is dragging the selector, that can cause this handler to be raised a lot)
        }
    }
}
