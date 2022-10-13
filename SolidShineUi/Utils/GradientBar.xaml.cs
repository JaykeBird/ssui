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

                // hook up selected, deselected events
                grdStops.Children.Add(gsi);
            }
        }
    }
}
