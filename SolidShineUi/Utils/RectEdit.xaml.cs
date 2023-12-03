using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SolidShineUi.Utils
{
    /// <summary>
    /// An editor control that can be used to edit <see cref="Rect"/> instances.
    /// </summary>
    public partial class RectEdit : UserControl
    {
        /// <summary>
        /// Create a RectEdit.
        /// </summary>
        public RectEdit()
        {
            InitializeComponent();
        }

        #region Brushes

        /// <summary>
        /// Get or set the stroke color to use for the rectangle visual in this editor.
        /// </summary>
        public Brush StrokeColor { get => (Brush)GetValue(StrokeColorProperty); set => SetValue(StrokeColorProperty, value); }

        /// <summary>The backing dependency property for <see cref="StrokeColor"/>. See the related property for details.</summary>
        public static DependencyProperty StrokeColorProperty
            = DependencyProperty.Register("StrokeColor", typeof(Brush), typeof(RectEdit),
            new FrameworkPropertyMetadata(Colors.Black.ToBrush()));

        /// <summary>
        /// Get or set the background brush for the buttons in the spinner controls in this editor.
        /// </summary>
        public Brush ButtonBackground { get => (Brush)GetValue(ButtonBackgroundProperty); set => SetValue(ButtonBackgroundProperty, value); }

        /// <summary>The backing dependency property for <see cref="ButtonBackground"/>. See the related property for details.</summary>
        public static DependencyProperty ButtonBackgroundProperty
            = DependencyProperty.Register("ButtonBackground", typeof(Brush), typeof(RectEdit),
            new FrameworkPropertyMetadata(Colors.White.ToBrush()));

        /// <summary>
        /// Get or set the highlight brush to use with the buttons in the spinner controls in this editor.
        /// </summary>
        public Brush ButtonHighlightBrush { get => (Brush)GetValue(ButtonHighlightBrushProperty); set => SetValue(ButtonHighlightBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="ButtonHighlightBrush"/>. See the related property for details.</summary>
        public static DependencyProperty ButtonHighlightBrushProperty
            = DependencyProperty.Register("ButtonHighlightBrush", typeof(Brush), typeof(RectEdit),
            new FrameworkPropertyMetadata(Colors.LightGray.ToBrush()));

        /// <summary>
        /// Get or set the click brush to use with the buttons in the spinner controls in this editor.
        /// </summary>
        public Brush ButtonClickBrush { get => (Brush)GetValue(ButtonClickBrushProperty); set => SetValue(ButtonClickBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="ButtonClickBrush"/>. See the related property for details.</summary>
        public static DependencyProperty ButtonClickBrushProperty
            = DependencyProperty.Register("ButtonClickBrush", typeof(Brush), typeof(RectEdit),
            new FrameworkPropertyMetadata(Colors.Gainsboro.ToBrush()));

        /// <summary>
        /// Get or set the border brush to use with the spinner controls in this editor.
        /// </summary>
        public Brush SpinnerBorderBrush { get => (Brush)GetValue(SpinnerBorderBrushProperty); set => SetValue(SpinnerBorderBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="SpinnerBorderBrush"/>. See the related property for details.</summary>
        public static DependencyProperty SpinnerBorderBrushProperty
            = DependencyProperty.Register("SpinnerBorderBrush", typeof(Brush), typeof(RectEdit),
            new FrameworkPropertyMetadata(Colors.Black.ToBrush()));

        /// <summary>
        /// Get or set the disabled brush to use with the spinner controls in this editor.
        /// </summary>
        public Brush ButtonDisabledBrush { get => (Brush)GetValue(ButtonDisabledBrushProperty); set => SetValue(ButtonDisabledBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="ButtonDisabledBrush"/>. See the related property for details.</summary>
        public static DependencyProperty ButtonDisabledBrushProperty
            = DependencyProperty.Register("ButtonDisabledBrush", typeof(Brush), typeof(RectEdit),
            new FrameworkPropertyMetadata(Colors.Gray.ToBrush()));


        #endregion

        #region MeasureType

        /// <summary>
        /// Get or set if the "Measure by" combo box should be displayed at the top of the 
        /// </summary>
        public bool ShowMeasureTypeOptions { get => (bool)GetValue(ShowMeasureTypeOptionsProperty); set => SetValue(ShowMeasureTypeOptionsProperty, value); }

        /// <summary>The backing dependency property for <see cref="ShowMeasureTypeOptions"/>. See the related property for details.</summary>
        public static DependencyProperty ShowMeasureTypeOptionsProperty
            = DependencyProperty.Register("ShowMeasureTypeOptions", typeof(bool), typeof(RectEdit),
            new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Get or set how to measure the users inputs to create a <see cref="Rect"/>. By default, this is <see cref="RectEditMeasureType.OriginAndSize"/>.
        /// </summary>
        /// <remarks>
        /// A <see cref="Rect"/> can be created by either setting the top-left point and then the rectangle's size, or by setting the top-left and bottom-right points,
        /// and creating a rectangle with those points as opposite corners. With this property, it can be changed which method the user uses.
        /// <para/>
        /// This can be changed by the user if <see cref="ShowMeasureTypeOptions"/> is set to <c>true</c>.
        /// </remarks>
        public RectEditMeasureType MeasureType { get => (RectEditMeasureType)GetValue(MeasureTypeProperty); set => SetValue(MeasureTypeProperty, value); }

        /// <summary>The backing dependency property for <see cref="MeasureType"/>. See the related property for details.</summary>
        public static DependencyProperty MeasureTypeProperty
            = DependencyProperty.Register("MeasureType", typeof(RectEditMeasureType), typeof(RectEdit),
            new FrameworkPropertyMetadata(RectEditMeasureType.OriginAndSize, OnInternalMeasureTypeChanged));

        /// <summary>
        /// Raisede when <see cref="MeasureType"/> changed.
        /// </summary>
#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? MeasureTypeChanged;
#else
        public event DependencyPropertyChangedEventHandler MeasureTypeChanged;
#endif

        private static void OnInternalMeasureTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RectEdit r)
            {
                r.OnMeasureTypeChanged(e);
            }
        }

        bool _internalAction = false;

        private void cbbMeasure_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbbMeasure.SelectedIndex == -1)
            {
                cbbMeasure.SelectedIndex = 0;
                return;
            }
            else if (_internalAction) { return; }
            MeasureType = (RectEditMeasureType)cbbMeasure.SelectedIndex;
        }

        private void OnMeasureTypeChanged(DependencyPropertyChangedEventArgs e)
        {
            _internalAction = true;
            if (cbbMeasure.SelectedIndex != (int)MeasureType)
            {
                cbbMeasure.SelectedIndex = (int)MeasureType;
            }
            _internalAction = false;
            RenderMeasureType();
            MeasureTypeChanged?.Invoke(this, e);
        }

        void RenderMeasureType()
        {
            if (MeasureType == RectEditMeasureType.OriginAndSize)
            {
                if (stkPoint2.Visibility == Visibility.Visible) { UpdateFromPointToWH(); }

                stkHeight.Visibility = Visibility.Visible;
                stkWidth.Visibility = Visibility.Visible;
                stkPoint2.Visibility = Visibility.Collapsed;

                pathBRCorner.Visibility = Visibility.Visible;
                pathBRPoint.Visibility = Visibility.Collapsed;
                pathRight.Margin = new Thickness(3.5, -0.5, 0, 0);
                pathBottom.Margin = new Thickness(-0.5, 0, -0.5, 0);

                colHeightNud.Width = new GridLength(1, GridUnitType.Star);
                colHeightNud.MinWidth = 60;
            }
            else
            {
                if (stkHeight.Visibility == Visibility.Visible) { UpdateFromWHToPoint(); }

                stkHeight.Visibility = Visibility.Collapsed;
                stkWidth.Visibility = Visibility.Collapsed;
                stkPoint2.Visibility = Visibility.Visible;

                pathBRCorner.Visibility = Visibility.Collapsed;
                pathBRPoint.Visibility = Visibility.Visible;
                pathRight.Margin = new Thickness(3.5, -0.5, 0, 2);
                pathBottom.Margin = new Thickness(-0.5, 0, 2, 0);

                colHeightNud.MinWidth = 0;
                colHeightNud.Width = new GridLength(0, GridUnitType.Pixel);
            }

            void UpdateFromPointToWH()
            {
                // width and height cannot be under 0, so if the user put in a lower-right point that ends up being less than 0, then I'm setting to 0
                nudWidth.Value = nudPoint2X.Value < nudPoint1X.Value ? 0 : nudPoint2X.Value - nudPoint1X.Value;
                nudHeight.Value = nudPoint2Y.Value < nudPoint1Y.Value ? 0 : nudPoint2Y.Value - nudPoint1Y.Value;
            }

            void UpdateFromWHToPoint()
            {
                nudPoint2X.Value = nudPoint1X.Value + nudWidth.Value;
                nudPoint2Y.Value = nudPoint1Y.Value + nudHeight.Value;
            }
        }

#endregion

        #region Limit To Percentage Range

        /// <summary>
        /// Get or set if the input should be limited to a range of 0.0 to 1.0, which can be converted into a percentage.
        /// </summary>
        /// <remarks>
        /// This is useful for situations when you want a <see cref="Rect"/> to represent a percentage of the total area, rather than absolute values,
        /// like when <see cref="TileBrush.ViewboxUnits"/> is set to <see cref="BrushMappingMode.RelativeToBoundingBox"/>.
        /// </remarks>
        public bool LimitToPercentageRange { get => (bool)GetValue(LimitToPercentageRangeProperty); set => SetValue(LimitToPercentageRangeProperty, value); }

        /// <summary>The backing dependency property for <see cref="LimitToPercentageRange"/>. See the related property for details.</summary>
        public static DependencyProperty LimitToPercentageRangeProperty
            = DependencyProperty.Register("LimitToPercentageRange", typeof(bool), typeof(RectEdit),
            new FrameworkPropertyMetadata(false, OnInternalLimitToPercentageRangeChanged));

        private static void OnInternalLimitToPercentageRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RectEdit r)
            {
                r.OnLimitToPercentageRangeChanged();
            }
        }

        private void OnLimitToPercentageRangeChanged()
        {
            if (LimitToPercentageRange)
            {
                SetNud(nudHeight, true);
                SetNud(nudWidth, true);
                SetNud(nudPoint1X, true);
                SetNud(nudPoint1Y, true);
                SetNud(nudPoint2X, true);
                SetNud(nudPoint2Y, true);

                nudPoint1X.MinValue = 0;
                nudPoint1Y.MinValue = 0;
                nudPoint2X.MinValue = 0;
                nudPoint2Y.MinValue = 0;
            }
            else
            {
                SetNud(nudHeight, false);
                SetNud(nudWidth, false);
                SetNud(nudPoint1X, false);
                SetNud(nudPoint1Y, false);
                SetNud(nudPoint2X, false);
                SetNud(nudPoint2Y, false);

                nudPoint1X.MinValue = double.MinValue;
                nudPoint1Y.MinValue = double.MinValue;
                nudPoint2X.MinValue = double.MinValue;
                nudPoint2Y.MinValue = double.MinValue;
            }

            void SetNud(DoubleSpinner ds, bool value)
            {
                if (value)
                {
                    ds.MaxValue = 1;
                    ds.Step = 0.05;
                }
                else
                {
                    ds.MaxValue = double.MaxValue;
                    ds.Step = 1;
                }
            }
        }

        #endregion

        #region Rect IO

        /// <summary>
        /// Load in a <see cref="Rect"/> into this editor control.
        /// </summary>
        /// <param name="rect">the value to load</param>
        public void LoadRect(Rect rect)
        {
            nudPoint1X.Value = rect.X;
            nudPoint1Y.Value = rect.Y;
            nudHeight.Value = rect.Height;
            nudWidth.Value = rect.Width;
            nudPoint2X.Value = rect.Right;
            nudPoint2Y.Value = rect.Bottom;
        }

        /// <summary>
        /// Get a <see cref="Rect"/> instance, based upon the values inputted into this editor.
        /// </summary>
        /// <returns>A new <see cref="Rect"/> that contains the inputted values.</returns>
        public Rect GetRect()
        {
            if (MeasureType == RectEditMeasureType.OriginAndSize)
            {
                return new Rect(nudPoint1X.Value, nudPoint1Y.Value, nudWidth.Value, nudHeight.Value);
            }
            else
            {
                return new Rect(new Point(nudPoint1X.Value, nudPoint1Y.Value), new Point(nudPoint2X.Value, nudPoint2Y.Value));
            }
        }

        #endregion

    }

    /// <summary>
    /// The method by which a Rect's values are set via a <see cref="RectEdit"/>.
    /// </summary>
    public enum RectEditMeasureType
    {
        /// <summary>
        /// Measure a Rect by using the top-left X and Y values, and the Rect's Width and Height.
        /// </summary>
        OriginAndSize = 0,
        /// <summary>
        /// Measure a Rect by listing two points, which will form the two opposing corners of a Rect.
        /// </summary>
        TwoPoints = 1
    }
}
