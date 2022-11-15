using SolidShineUi.Utils;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SolidShineUi.PropertyList.Dialogs
{
    /// <summary>
    /// Interaction logic for LinearGradientEditorDialog.xaml
    /// </summary>
    public partial class LinearGradientEditorDialog : FlatWindow
    {
        /// <summary>
        /// Create a LinearGradientEditorDialog.
        /// </summary>
        public LinearGradientEditorDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Create a LinearGradientEditorDialog, with the color scheme pre-defined.
        /// </summary>
        public LinearGradientEditorDialog(ColorScheme cs)
        {
            InitializeComponent();
            ColorScheme = cs;
        }

        /// <summary>
        /// Create a LinearGradientEditorDialog, with a linear gradient brush preloaded.
        /// </summary>
        public LinearGradientEditorDialog(LinearGradientBrush br)
        {
            InitializeComponent();
            LoadGradient(br);
        }

        /// <summary>
        /// Create a LinearGradientEditorDialog, with the color scheme pre-defined and the linear gradient brush preloaded.
        /// </summary>
        public LinearGradientEditorDialog(ColorScheme cs, LinearGradientBrush br)
        {
            InitializeComponent();
            ColorScheme = cs;
            LoadGradient(br);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //if (nudStartX == null) return;

            //RunUpdateAction(() =>
            //{
            //    nudStartX.Value = edtPoints.SelectedWidth1;
            //    nudStartY.Value = edtPoints.SelectedHeight1;

            //    nudEndX.Value = edtPoints.SelectedWidth2;
            //    nudEndY.Value = edtPoints.SelectedHeight2;
            //});

            imgDown.Source = IconLoader.LoadIcon("DownArrow", ColorScheme);
            imgUp.Source = IconLoader.LoadIcon("UpArrow", ColorScheme);
            imgLeft.Source = IconLoader.LoadIcon("LeftArrow", ColorScheme);
            imgRight.Source = IconLoader.LoadIcon("RightArrow", ColorScheme);
        }

        /// <summary>
        /// Load in a <see cref="LinearGradientBrush"/> into this dialog for viewing/editing.
        /// </summary>
        /// <param name="lgb">The LinearGradientBrush to load in.</param>
        public void LoadGradient(LinearGradientBrush lgb)
        {
            stopBar.GradientStops = lgb.GradientStops;

            if (lgb.MappingMode == BrushMappingMode.RelativeToBoundingBox)
            {
                edtPoints.SelectedHeight1 = lgb.StartPoint.Y;
                edtPoints.SelectedWidth1 = lgb.StartPoint.X;
                edtPoints.SelectedHeight2 = lgb.EndPoint.Y;
                edtPoints.SelectedWidth2 = lgb.EndPoint.X;
            }
            else
            {
                nudEndX.Visibility = Visibility.Collapsed;
                nudEndY.Visibility = Visibility.Collapsed;
                nudStartX.Visibility = Visibility.Collapsed;
                nudStartY.Visibility = Visibility.Collapsed;

                nudEndAX.Visibility = Visibility.Visible;
                nudEndAY.Visibility = Visibility.Visible;
                nudStartAX.Visibility = Visibility.Visible;
                nudStartAY.Visibility = Visibility.Visible;

                edtPoints.IsEnabled = false;

                nudStartAX.Value = lgb.StartPoint.X;
                nudStartAY.Value = lgb.StartPoint.Y;
                nudEndAX.Value = lgb.EndPoint.X;
                nudEndAY.Value = lgb.EndPoint.Y;
            }

            cbbMappingMode.SelectedEnumValue = lgb.MappingMode;
            cbbSpreadMethod.SelectedEnumValue = lgb.SpreadMethod;
            nudOpacity.Value = lgb.Opacity;
        }

        /// <summary>
        /// Get a <see cref="LinearGradientBrush"/> based upon the options selected in this dialog.
        /// </summary>
        public LinearGradientBrush GetGradientBrush()
        {
            LinearGradientBrush lgb = new LinearGradientBrush();

            lgb.GradientStops = stopBar.GradientStops;
            lgb.MappingMode = cbbMappingMode.SelectedEnumValueAsEnum<BrushMappingMode>();
            lgb.SpreadMethod = cbbSpreadMethod.SelectedEnumValueAsEnum<GradientSpreadMethod>();

            lgb.Opacity = nudOpacity.Value;

            if (lgb.MappingMode == BrushMappingMode.RelativeToBoundingBox)
            {
                lgb.StartPoint = new Point(edtPoints.SelectedWidth1, edtPoints.SelectedHeight1);
                lgb.EndPoint = new Point(edtPoints.SelectedWidth2, edtPoints.SelectedHeight2);
            }
            else
            {
                lgb.StartPoint = new Point(nudStartAX.Value, nudStartAY.Value);
                lgb.EndPoint = new Point(nudEndAX.Value, nudEndAY.Value);
            }

            return lgb;
        }

        /// <summary>Get or set the result the user selected for this dialog; <c>true</c> is "OK", <c>false</c> is "Cancel" or the window was closed without making a choice.</summary>
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

        // taken from WPF source code
        private Point EndPointFromAngle(double angle)
        {
            angle = angle * 0.0055555555555555558 * Math.PI;
            return new Point(Math.Cos(angle), Math.Sin(angle));
        }

        bool updateValues = false;

        private void edtPoints_SelectedPositionChanged(object sender, EventArgs e)
        {
            if (nudStartX == null) return;

            RunUpdateAction(() =>
            {
                nudStartX.Value = edtPoints.SelectedWidth1;
                nudStartY.Value = edtPoints.SelectedHeight1;

                nudEndX.Value = edtPoints.SelectedWidth2;
                nudEndY.Value = edtPoints.SelectedHeight2;

                nudAngle.Value = Math.Round(GetAngle(), 1);
            });

            UpdatePreview();
        }

#pragma warning disable IDE0051 // Remove unused private members
        private void nudStartX_ValueChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            RunUpdateAction(() =>
            {
                edtPoints.SelectedWidth1 = nudStartX.Value;
                nudAngle.Value = Math.Round(GetAngle(), 1);
            });
        }

        private void nudStartY_ValueChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            RunUpdateAction(() =>
            {
                edtPoints.SelectedHeight1 = nudStartY.Value;
                nudAngle.Value = Math.Round(GetAngle(), 1);
            });
        }

        private void nudEndX_ValueChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            RunUpdateAction(() =>
            {
                edtPoints.SelectedWidth2 = nudEndX.Value;
                nudAngle.Value = Math.Round(GetAngle(), 1);
            });
        }

        private void nudEndY_ValueChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            RunUpdateAction(() =>
            {
                edtPoints.SelectedHeight2 = nudEndY.Value;
                nudAngle.Value = Math.Round(GetAngle(), 1);
            });
        }

        private void nudAngle_ValueChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            RunUpdateAction(() =>
            {
                Point p = EndPointFromAngle(nudAngle.Value);
                
                if (p.X < 0.0)
                {
                    if (p.Y < 0.0)
                    {
                        edtPoints.SelectedHeight2 = 0.0;
                        edtPoints.SelectedWidth2 = 0.0;

                        edtPoints.SelectedHeight1 = p.Y * -1;
                        edtPoints.SelectedWidth1 = p.X * -1;
                    }
                    else
                    {
                        edtPoints.SelectedHeight2 = 1.0;
                        edtPoints.SelectedWidth2 = 0.0;

                        edtPoints.SelectedHeight1 = 1.0 - p.Y;
                        edtPoints.SelectedWidth1 = p.X * -1;
                    }
                }
                else if (p.Y < 0.0)
                {
                    edtPoints.SelectedHeight1 = 1.0;
                    edtPoints.SelectedWidth1 = 0.0;

                    edtPoints.SelectedHeight2 = 1.0 - (p.Y * -1);
                    edtPoints.SelectedWidth2 = p.X;
                }
                else
                {
                    edtPoints.SelectedHeight1 = 0.0;
                    edtPoints.SelectedWidth1 = 0.0;

                    edtPoints.SelectedHeight2 = p.Y;
                    edtPoints.SelectedWidth2 = p.X;
                }

                nudStartX.Value = edtPoints.SelectedWidth1;
                nudStartY.Value = edtPoints.SelectedHeight1;

                nudEndX.Value = edtPoints.SelectedWidth2;
                nudEndY.Value = edtPoints.SelectedHeight2;
            });
        }
#pragma warning restore IDE0051 // Remove unused private members

        void RunUpdateAction(Action a)
        {
            if (updateValues) return;
            updateValues = true;

            a.Invoke();

            updateValues = false;
        }

        double GetAngle()
        {
            // https://stackoverflow.com/a/12892493/2987285
            Point p1 = new Point(edtPoints.SelectedWidth1, edtPoints.SelectedHeight1);
            Point p2 = new Point(edtPoints.SelectedWidth2, edtPoints.SelectedHeight2);

            double angle = Math.Atan2(p2.Y - p1.Y, p2.X - p1.X) * 180.0 / Math.PI;

            return angle < 0 ? 360 + angle : angle;
        }

        private void cbbMappingMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbbMappingMode.SelectedEnumValueAsEnum<BrushMappingMode>() == BrushMappingMode.Absolute)
            {
                nudEndX.Visibility = Visibility.Collapsed;
                nudEndY.Visibility = Visibility.Collapsed;
                nudStartX.Visibility = Visibility.Collapsed;
                nudStartY.Visibility = Visibility.Collapsed;

                nudEndAX.Visibility = Visibility.Visible;
                nudEndAY.Visibility = Visibility.Visible;
                nudStartAX.Visibility = Visibility.Visible;
                nudStartAY.Visibility = Visibility.Visible;

                edtPoints.IsEnabled = false;
            }
            else
            {
                nudEndX.Visibility = Visibility.Visible;
                nudEndY.Visibility = Visibility.Visible;
                nudStartX.Visibility = Visibility.Visible;
                nudStartY.Visibility = Visibility.Visible;

                nudEndAX.Visibility = Visibility.Collapsed;
                nudEndAY.Visibility = Visibility.Collapsed;
                nudStartAX.Visibility = Visibility.Collapsed;
                nudStartAY.Visibility = Visibility.Collapsed;

                edtPoints.IsEnabled = true;
            }
            UpdatePreview();
        }

        private void cbbSpreadMethod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdatePreview();
        }

        private void nudOpacity_ValueChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdatePreview();
        }

        void UpdatePreview()
        {
            if (nudOpacity == null || brdrPreview == null) return;

            LinearGradientBrush lgb = new LinearGradientBrush();
            lgb.Opacity = nudOpacity.Value;
            lgb.SpreadMethod = cbbSpreadMethod.SelectedEnumValueAsEnum<GradientSpreadMethod>();
            lgb.MappingMode = cbbMappingMode.SelectedEnumValueAsEnum<BrushMappingMode>();
            lgb.GradientStops = stopBar.GradientStops;

            if (nudEndAX.Visibility == Visibility.Visible)
            {
                lgb.StartPoint = new Point(nudStartAX.Value, nudStartAY.Value);
                lgb.EndPoint = new Point(nudEndAX.Value, nudEndAY.Value);
            }
            else
            {
                lgb.StartPoint = new Point(edtPoints.SelectedWidth1, edtPoints.SelectedHeight1);
                lgb.EndPoint = new Point(edtPoints.SelectedWidth2, edtPoints.SelectedHeight2);
            }

            grdPreview.Background = lgb;
        }

        private void btnAngleLeft_Click(object sender, RoutedEventArgs e)
        {
            nudAngle.Value = 180;
        }

        private void btnAngleUp_Click(object sender, RoutedEventArgs e)
        {
            nudAngle.Value = 270;
        }

        private void btnAngleRight_Click(object sender, RoutedEventArgs e)
        {
            nudAngle.Value = 0;
        }

        private void btnAngleDown_Click(object sender, RoutedEventArgs e)
        {
            nudAngle.Value = 90;
        }
    }
}
