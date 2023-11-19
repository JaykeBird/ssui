using SolidShineUi.Utils;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SolidShineUi.PropertyList.Dialogs
{
    /// <summary>
    /// A dialog for creating and editing radial gradients, specifically <see cref="RadialGradientBrush"/> objects.
    /// </summary>
    public partial class RadialGradientEditorDialog : FlatWindow
    {
        /// <summary>
        /// Create a LinearGradientEditorDialog.
        /// </summary>
        public RadialGradientEditorDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Create a LinearGradientEditorDialog, with the color scheme pre-defined.
        /// </summary>
        public RadialGradientEditorDialog(ColorScheme cs)
        {
            InitializeComponent();
            ColorScheme = cs;
        }

        /// <summary>
        /// Create a LinearGradientEditorDialog, with a linear gradient brush preloaded.
        /// </summary>
        public RadialGradientEditorDialog(RadialGradientBrush br)
        {
            InitializeComponent();
            LoadGradient(br);
        }

        /// <summary>
        /// Create a LinearGradientEditorDialog, with the color scheme pre-defined and the linear gradient brush preloaded.
        /// </summary>
        public RadialGradientEditorDialog(ColorScheme cs, RadialGradientBrush br)
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
            //    nudStartX.Value = edtStart.SelectedWidth1;
            //    nudStartY.Value = edtStart.SelectedHeight1;

            //    nudEndX.Value = edtStart.SelectedWidth2;
            //    nudEndY.Value = edtStart.SelectedHeight2;
            //});
        }

        /// <summary>
        /// Load in a <see cref="RadialGradientBrush"/> into this dialog for viewing/editing.
        /// </summary>
        /// <param name="rgb">The RadialGradientBrush to load in.</param>
        public void LoadGradient(RadialGradientBrush rgb)
        {
            stopBar.GradientStops = rgb.GradientStops;

            if (rgb.MappingMode == BrushMappingMode.RelativeToBoundingBox)
            {
                edtStart.SelectedHeight = rgb.GradientOrigin.Y;
                edtStart.SelectedWidth = rgb.GradientOrigin.X;

                edtCenter.SelectedHeight = rgb.Center.Y;
                edtCenter.SelectedWidth = rgb.Center.X;

                nudSizeX.Value = rgb.RadiusX;
                nudSizeY.Value = rgb.RadiusY;
            }
            else
            {
                nudEndX.Visibility = Visibility.Collapsed;
                nudEndY.Visibility = Visibility.Collapsed;
                nudStartX.Visibility = Visibility.Collapsed;
                nudStartY.Visibility = Visibility.Collapsed;
                nudSizeX.Visibility = Visibility.Collapsed;
                nudSizeY.Visibility = Visibility.Collapsed;

                nudEndAX.Visibility = Visibility.Visible;
                nudEndAY.Visibility = Visibility.Visible;
                nudStartAX.Visibility = Visibility.Visible;
                nudStartAY.Visibility = Visibility.Visible;
                nudSizeAX.Visibility = Visibility.Visible;
                nudSizeAY.Visibility = Visibility.Visible;

                edtStart.IsEnabled = false;
                edtCenter.IsEnabled = false;

                nudStartAX.Value = rgb.GradientOrigin.X;
                nudStartAY.Value = rgb.GradientOrigin.Y;
                nudEndAX.Value = rgb.Center.X;
                nudEndAY.Value = rgb.Center.Y;

                nudSizeAX.Value = rgb.RadiusX;
                nudSizeAY.Value = rgb.RadiusY;
            }

            cbbMappingMode.SelectedEnumValue = rgb.MappingMode;
            cbbSpreadMethod.SelectedEnumValue = rgb.SpreadMethod;
            nudOpacity.Value = rgb.Opacity;
        }

        /// <summary>
        /// Get a <see cref="RadialGradientBrush"/> based upon the options selected in this dialog.
        /// </summary>
        public RadialGradientBrush GetGradientBrush()
        {
            RadialGradientBrush rgb = new RadialGradientBrush();

            rgb.GradientStops = stopBar.GradientStops;
            rgb.MappingMode = cbbMappingMode.SelectedEnumValueAsEnum<BrushMappingMode>();
            rgb.SpreadMethod = cbbSpreadMethod.SelectedEnumValueAsEnum<GradientSpreadMethod>();

            rgb.Opacity = nudOpacity.Value;

            if (rgb.MappingMode == BrushMappingMode.RelativeToBoundingBox)
            {
                rgb.GradientOrigin = new Point(edtStart.SelectedWidth, edtStart.SelectedHeight);
                rgb.Center = new Point(edtCenter.SelectedWidth, edtCenter.SelectedHeight);
                rgb.RadiusX = nudSizeX.Value;
                rgb.RadiusY = nudSizeY.Value;
            }
            else
            {
                rgb.GradientOrigin = new Point(nudStartAX.Value, nudStartAY.Value);
                rgb.Center = new Point(nudEndAX.Value, nudEndAY.Value);
                rgb.RadiusX = nudSizeAX.Value;
                rgb.RadiusY = nudSizeAY.Value;
            }

            return rgb;
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

        bool updateValues = false;

        private void edtStart_SelectedPositionChanged(object sender, EventArgs e)
        {
            if (nudStartX == null) return;

            RunUpdateAction(() =>
            {
                nudStartX.Value = edtStart.SelectedWidth;
                nudStartY.Value = edtStart.SelectedHeight;
            });

            UpdatePreview();
        }

        private void edtCenter_SelectedPositionChanged(object sender, EventArgs e)
        {
            if (nudEndX == null) return;

            RunUpdateAction(() =>
            {
                nudEndX.Value = edtCenter.SelectedWidth;
                nudEndY.Value = edtCenter.SelectedHeight;
            });

            UpdatePreview();
        }

#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable IDE0060 // Remove unused parameter
        private void nudStartX_ValueChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            RunUpdateAction(() =>
            {
                edtStart.SelectedWidth = nudStartX.Value;
            });
        }

        private void nudStartY_ValueChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            RunUpdateAction(() =>
            {
                edtStart.SelectedHeight = nudStartY.Value;
            });
        }

        private void nudEndX_ValueChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            RunUpdateAction(() =>
            {
                edtCenter.SelectedWidth = nudEndX.Value;
            });
        }

        private void nudEndY_ValueChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            RunUpdateAction(() =>
            {
                edtCenter.SelectedWidth = nudEndY.Value;
            });
        }
#pragma warning restore IDE0051 // Remove unused private members
#pragma warning restore IDE0060 // Remove unused parameter

        void RunUpdateAction(Action a)
        {
            if (updateValues) return;
            updateValues = true;

            a.Invoke();

            updateValues = false;
        }

        private void cbbMappingMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbbMappingMode.SelectedEnumValueAsEnum<BrushMappingMode>() == BrushMappingMode.Absolute)
            {
                nudEndX.Visibility = Visibility.Collapsed;
                nudEndY.Visibility = Visibility.Collapsed;
                nudStartX.Visibility = Visibility.Collapsed;
                nudStartY.Visibility = Visibility.Collapsed;
                nudSizeX.Visibility = Visibility.Collapsed;
                nudSizeY.Visibility = Visibility.Collapsed;

                nudEndAX.Visibility = Visibility.Visible;
                nudEndAY.Visibility = Visibility.Visible;
                nudStartAX.Visibility = Visibility.Visible;
                nudStartAY.Visibility = Visibility.Visible;
                nudSizeAX.Visibility = Visibility.Visible;
                nudSizeAY.Visibility = Visibility.Visible;

                edtStart.IsEnabled = false;
                edtCenter.IsEnabled = false;
            }
            else
            {
                nudEndX.Visibility = Visibility.Visible;
                nudEndY.Visibility = Visibility.Visible;
                nudStartX.Visibility = Visibility.Visible;
                nudStartY.Visibility = Visibility.Visible;
                nudSizeX.Visibility = Visibility.Visible;
                nudSizeY.Visibility = Visibility.Visible;

                nudEndAX.Visibility = Visibility.Collapsed;
                nudEndAY.Visibility = Visibility.Collapsed;
                nudStartAX.Visibility = Visibility.Collapsed;
                nudStartAY.Visibility = Visibility.Collapsed;
                nudSizeAX.Visibility = Visibility.Collapsed;
                nudSizeAY.Visibility = Visibility.Collapsed;

                edtStart.IsEnabled = true;
                edtCenter.IsEnabled = true;
            }
            UpdatePreview();
        }

        private void cbbSpreadMethod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdatePreview();
        }

#pragma warning disable IDE0060 // Remove unused parameter
#pragma warning disable IDE0051 // Remove unused private members
        private void nudOpacity_ValueChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdatePreview();
        }

        void UpdatePreview()
        {
            if (nudOpacity == null || brdrPreview == null) return;

            RadialGradientBrush rgb = new RadialGradientBrush();
            rgb.Opacity = nudOpacity.Value;
            rgb.SpreadMethod = cbbSpreadMethod.SelectedEnumValueAsEnum<GradientSpreadMethod>();
            rgb.MappingMode = cbbMappingMode.SelectedEnumValueAsEnum<BrushMappingMode>();
            rgb.GradientStops = stopBar.GradientStops;

            if (nudEndAX.Visibility == Visibility.Visible)
            {
                rgb.GradientOrigin = new Point(nudStartAX.Value, nudStartAY.Value);
                rgb.Center = new Point(nudEndAX.Value, nudEndAY.Value);
                rgb.RadiusX = nudSizeAX.Value;
                rgb.RadiusY = nudSizeAY.Value;
            }
            else
            {
                rgb.GradientOrigin = new Point(edtStart.SelectedWidth, edtStart.SelectedHeight);
                rgb.Center = new Point(edtCenter.SelectedWidth, edtCenter.SelectedHeight);
                rgb.RadiusX = nudSizeX.Value;
                rgb.RadiusY = nudSizeY.Value;
            }

            grdPreview.Background = rgb;
        }

        private void stopBar_GradientChanged(object sender, EventArgs e)
        {
            UpdatePreview();
        }

        private void nudSize_ValueChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdatePreview();
        }
#pragma warning restore IDE0051 // Remove unused private members
#pragma warning restore IDE0060 // Remove unused parameter
    }
}
