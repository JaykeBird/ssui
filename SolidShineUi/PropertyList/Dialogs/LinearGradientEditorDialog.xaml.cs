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
using System.Windows.Shapes;

namespace SolidShineUi.PropertyList.Dialogs
{
    /// <summary>
    /// Interaction logic for LinearGradientEditorDialog.xaml
    /// </summary>
    public partial class LinearGradientEditorDialog : Window
    {
        /// <summary>
        /// Create a LinearGradientEditorDialog.
        /// </summary>
        public LinearGradientEditorDialog()
        {
            InitializeComponent();
        }
        
        public void LoadGradient(LinearGradientBrush lgb)
        {
            stopBar.GradientStops = lgb.GradientStops;
            edtPoints.SelectedHeight1 = lgb.StartPoint.Y;
            edtPoints.SelectedWidth1 = lgb.StartPoint.X;
            edtPoints.SelectedHeight2 = lgb.EndPoint.Y;
            edtPoints.SelectedWidth2 = lgb.EndPoint.X;
        }

        // taken from WPF source code
        private Point EndPointFromAngle(double angle)
        {
            angle = angle * 0.0055555555555555558 * Math.PI;
            return new Point(Math.Cos(angle), Math.Sin(angle));
        }

        public new bool DialogResult { get; set; } = false;

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
