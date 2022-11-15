using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SolidShineUi.Utils
{
    /// <summary>
    /// Interaction logic for GradientBar.xaml
    /// </summary>
    public partial class GradientBar : UserControl
    {
        /// <summary>
        /// Create a new GradientBar.
        /// </summary>
        public GradientBar()
        {
            InitializeComponent();
            Loaded += GradientBar_Loaded;
        }

        private void GradientBar_Loaded(object sender, RoutedEventArgs e)
        {
            RenderStops();
            //throw new NotImplementedException();
        }

        private GradientStopCollection _stops = new GradientStopCollection();
#if NETCOREAPP
        private GradientStopItem? _selected = null;
#else
        private GradientStopItem _selected = null;
#endif

        /// <summary>
        /// Get or set the list of gradient stops to display in this GradientBar.
        /// </summary>
        public GradientStopCollection GradientStops
        {
            get
            {
                return _stops;
            }
            set
            {
                _stops = value;
                RenderBar();
                RenderStops();    
            }
        }

        void RenderBar()
        {
            eleGradient.Background = new LinearGradientBrush(_stops, 0.0);
        }

        void RenderStops()
        {
            grdStops.Children.Clear();
            foreach (GradientStop stop in _stops)
            {
                GradientStopItem gsi = new GradientStopItem(stop);
                // set gsi left margin to relative position in grdStops (offset * brdrGradient.ActualWidth + 20)
                gsi.Margin = new Thickness(stop.Offset * brdrGradient.ActualWidth, 0, 0, 0);
                gsi.HorizontalAlignment = HorizontalAlignment.Left;
                gsi.VerticalAlignment = VerticalAlignment.Center;
                gsi.Click += gsi_Click;
                gsi.IsSelected = false;

                // hook up selected, deselected events
                grdStops.Children.Add(gsi);
            }
        }

        private void gsi_Click(object sender, RoutedEventArgs e)
        {
            if (sender == _selected) return; // this one is already selected

            GradientStopItem gsi = (GradientStopItem)sender;
            if (gsi == null) return;

            if (_selected != null) _selected.IsSelected = false;

            _selected = gsi;
            gsi.IsSelected = true;
            LoadSelectedStop();
        }

        void LoadSelectedStop()
        {

        }
    }
}
