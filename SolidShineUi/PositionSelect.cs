using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Controls;
using System.ComponentModel;
using SolidShineUi.Utils;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Shapes;

namespace SolidShineUi
{
    /// <summary>
    /// A control that allows a user to select a 2-D point (position) between (0,0) and (1,1).
    /// </summary>
    public class PositionSelect : ThemedControl
    {
        static PositionSelect()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PositionSelect), new FrameworkPropertyMetadata(typeof(PositionSelect)));
        }

        /// <summary>
        /// Create a new PositionSelect.
        /// </summary>
        public PositionSelect()
        {
            SetValue(HorizontalSnapPointsProperty, new ObservableCollection<double>());
            SetValue(VerticalSnapPointsProperty, new ObservableCollection<double>());

            SizeChanged += PositionSelect_SizeChanged;

            MouseDown += PositionSelect_MouseDown;
            MouseUp += PositionSelect_MouseUp;
            MouseMove += PositionSelect_MouseMove;
            MouseLeave += PositionSelect_MouseLeave;

            PreviewKeyUp += PositionSelect_PreviewKeyUp;
            PreviewKeyDown += PositionSelect_PreviewKeyDown;
            KeyUp += PositionSelect_KeyUp;

            KeyboardNavigation.SetDirectionalNavigation(this, KeyboardNavigationMode.Contained);
        }

        #region Template IO
        
        /// <inheritdoc/>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            LoadTemplateItems();
        }

        bool itemsLoaded = false;

        // bool _internalAction = false;

#if NETCOREAPP
        Ellipse? ellSelect = null;

        Grid? canVertical = null;
        Grid? canHorizontal = null;
        // Grid? grdOverall = null;
#else
        Ellipse ellSelect = null;

        Grid canVertical = null;
        Grid canHorizontal = null;
        // Grid grdOverall = null;
#endif

        void LoadTemplateItems()
        {
            if (!itemsLoaded)
            {
                //grdSelArea = (Canvas)GetTemplateChild("PART_Canvas");
                ellSelect = (Ellipse)GetTemplateChild("PART_Selector");
                canVertical = (Grid)GetTemplateChild("PART_VerticalLinesGrid");
                canHorizontal = (Grid)GetTemplateChild("PART_HorizontalLinesGrid");
                // grdOverall = (Grid)GetTemplateChild("PART_Grid");

                if (canVertical != null && canHorizontal != null && ellSelect != null)
                {
                    // put in event handlers here

                    itemsLoaded = true;
                }

            }
        }
        #endregion

        #region Brushes / Brush Handling

        #region Brush Properties

        /// <summary>
        /// Get or set the brush used for the selector ellipse in the PositionSelect.
        /// </summary>
        [Category("Brushes")]
        public Brush SelectorBrush
        {
            get => (Brush)GetValue(SelectorBrushProperty);
            set => SetValue(SelectorBrushProperty, value);
        }

        /// <summary>
        /// Get or set the brush to use for the background of the PositionSelect's box when it is disabled.
        /// </summary>
        [Category("Brushes")]
        public Brush BackgroundDisabledBrush
        {
            get => (Brush)GetValue(BackgroundDisabledBrushProperty);
            set => SetValue(BackgroundDisabledBrushProperty, value);
        }

        /// <summary>
        /// Get or set the brush to use for the border of the cotnrol, while the control is disabled.
        /// </summary>
        [Category("Brushes")]
        public Brush BorderDisabledBrush
        {
            get => (Brush)GetValue(BorderDisabledBrushProperty);
            set => SetValue(BorderDisabledBrushProperty, value);
        }

        /// <summary>
        /// Get or set the brush used for the check mark when the control is disabled.
        /// </summary>
        [Category("Brushes")]
        public Brush SelectorDisabledBrush
        {
            get => (Brush)GetValue(SelectorDisabledBrushProperty);
            set => SetValue(SelectorDisabledBrushProperty, value);
        }

        /// <summary>
        /// Get or set the brush used for the snap point lines.
        /// </summary>
        [Category("Brushes")]
        public Brush SnapLineBrush
        {
            get => (Brush)GetValue(SnapLineBrushProperty);
            set => SetValue(SnapLineBrushProperty, value);
        }

        /// <summary>
        /// Get or set the brush used for the outline when the control is keyboard focused.
        /// </summary>
        [Category("Brushes")]
        public Brush KeyboardFocusHighlight
        {
            get => (Brush)GetValue(KeyboardFocusHighlightProperty);
            set => SetValue(KeyboardFocusHighlightProperty, value);
        }

        /// <summary>The backing dependency property for <see cref="SelectorBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty SelectorBrushProperty = DependencyProperty.Register(
            "SelectorBrush", typeof(Brush), typeof(PositionSelect),
            new PropertyMetadata(new SolidColorBrush(ColorsHelper.Black)));

        /// <summary>The backing dependency property for <see cref="BackgroundDisabledBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty BackgroundDisabledBrushProperty = DependencyProperty.Register(
            "BackgroundDisabledBrush", typeof(Brush), typeof(PositionSelect),
            new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));

        /// <summary>The backing dependency property for <see cref="BorderDisabledBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty BorderDisabledBrushProperty = DependencyProperty.Register(
            "BorderDisabledBrush", typeof(Brush), typeof(PositionSelect),
            new PropertyMetadata(new SolidColorBrush(Colors.Gray)));

        /// <summary>The backing dependency property for <see cref="SelectorDisabledBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty SelectorDisabledBrushProperty = DependencyProperty.Register(
            "SelectorDisabledBrush", typeof(Brush), typeof(PositionSelect),
            new PropertyMetadata(new SolidColorBrush(Colors.DimGray)));

        /// <summary>The backing dependency property for <see cref="SnapLineBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty SnapLineBrushProperty = DependencyProperty.Register(
            "SnapLineBrush", typeof(Brush), typeof(PositionSelect),
            new PropertyMetadata(new SolidColorBrush(Colors.LightGray), (d, e) => d.PerformAs<PositionSelect>(r => r.UpdateSnapLineBrush())));

        /// <summary>The backing dependency property for <see cref="KeyboardFocusHighlight"/>. See the related property for details.</summary>
        public static readonly DependencyProperty KeyboardFocusHighlightProperty = DependencyProperty.Register(
            "KeyboardFocusHighlight", typeof(Brush), typeof(PositionSelect),
            new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));

        #endregion

        /// <summary>
        /// Updates the visuals of the snap lines in the control to match the SnapLineBrush property.
        /// </summary>
        protected void UpdateSnapLineBrush()
        {
            if (canVertical == null || canHorizontal == null) return;

#if NETCOREAPP
            foreach (UIElement? item in canVertical.Children)
#else
            foreach (UIElement item in canVertical.Children)
#endif
            {
                if (item != null && item is Border b)
                {
                    b.BorderBrush = SnapLineBrush;
                }
            }
#if NETCOREAPP
            foreach (UIElement? item in canHorizontal.Children)
#else
            foreach (UIElement item in canHorizontal.Children)
#endif
            {
                if (item != null && item is Border b)
                {
                    b.BorderBrush = SnapLineBrush;
                }
            }
        }

        #endregion

        #region Snaplines

        /// <summary>
        /// Get or set if the selector should snap to the snap lines within the control.
        /// </summary>
        public bool SnapToSnaplines { get => (bool)GetValue(SnapToSnaplinesProperty); set => SetValue(SnapToSnaplinesProperty, value); }

        /// <summary>
        /// A dependency property object backing the related property. See the property itself for more details.
        /// </summary>
        public static readonly DependencyProperty SnapToSnaplinesProperty
            = DependencyProperty.Register(nameof(SnapToSnaplines), typeof(bool), typeof(PositionSelect), new FrameworkPropertyMetadata(true));


        /// <summary>
        /// The distance, in pixels, within which the selector should snap to the nearest snap line.
        /// The larger the distance, the further the selector can be away from a snap line before it snaps to the line.
        /// </summary>
        public double SnapDistance { get => (double)GetValue(SnapDistanceProperty); set => SetValue(SnapDistanceProperty, value); }

        /// <summary>
        /// A dependency property object backing the related property. See the property itself for more details.
        /// </summary>
        public static readonly DependencyProperty SnapDistanceProperty
            = DependencyProperty.Register(nameof(SnapDistance), typeof(double), typeof(PositionSelect), new FrameworkPropertyMetadata(3.0));

        #region ObservableCollection handling

        /// <summary>
        /// Get or set the list of snap points that are displayed along the horizontal (X) axis of the control.
        /// <c>0.0</c> represents the far left of the control, and <c>1.0</c> represents the far right of the control.
        /// </summary>
        public ObservableCollection<double> HorizontalSnapPoints
        {
            get => (ObservableCollection<double>)GetValue(HorizontalSnapPointsProperty);
            set => SetValue(HorizontalSnapPointsProperty, value);
        }

        /// <summary>The backing dependency property for <see cref="HorizontalSnapPoints"/>. See the related property for details.</summary>
        public static readonly DependencyProperty HorizontalSnapPointsProperty
            = DependencyProperty.Register(nameof(HorizontalSnapPoints), typeof(ObservableCollection<double>), typeof(PositionSelect),
                new FrameworkPropertyMetadata(new PropertyChangedCallback((d, e) => d.PerformAs<PositionSelect>((r) => r.HorizontalSnapPointsChanged(r, e)))));

        /// <summary>
        /// Get or set the list of snap points that are displayed along the vertical (Y) axis of the control.
        /// <c>0.0</c> represents the far top of the control, and <c>1.0</c> represents the far bottom of the control.
        /// </summary>
        public ObservableCollection<double> VerticalSnapPoints
        {
            get => (ObservableCollection<double>)GetValue(VerticalSnapPointsProperty);
            set => SetValue(VerticalSnapPointsProperty, value);
        }

        /// <summary>The backing dependency property for <see cref="VerticalSnapPoints"/>. See the related property for details.</summary>
        public static readonly DependencyProperty VerticalSnapPointsProperty
            = DependencyProperty.Register(nameof(VerticalSnapPoints), typeof(ObservableCollection<double>), typeof(PositionSelect),
                new FrameworkPropertyMetadata(new PropertyChangedCallback((d, e) => d.PerformAs<PositionSelect>((r) => r.VerticalSnapPointsChanged(r, e)))));

        private void HorizontalSnapPointsChanged(PositionSelect sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is ObservableCollection<double> od)
            {
                od.CollectionChanged -= HorizontalSnapPoints_CollectionChanged;
            }
            if (e.NewValue is ObservableCollection<double> nd)
            {
                nd.CollectionChanged += HorizontalSnapPoints_CollectionChanged;
            }
        }

        private void VerticalSnapPointsChanged(PositionSelect sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is ObservableCollection<double> od)
            {
                od.CollectionChanged -= VerticalSnapPoints_CollectionChanged;
            }
            if (e.NewValue is ObservableCollection<double> nd)
            {
                nd.CollectionChanged += VerticalSnapPoints_CollectionChanged;
            }
        }


#if NETCOREAPP
        private void VerticalSnapPoints_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
#else
        private void VerticalSnapPoints_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
#endif
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    if (e.NewItems != null)
                    {
                        foreach (double? item in e.NewItems)
                        {
                            if (!item.HasValue) continue;
                            AddVerticalSnapPoint(item.Value);
                        }
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    if (e.OldItems != null)
                    {
                        foreach (double? item in e.OldItems)
                        {
                            if (!item.HasValue) continue;
                            RemoveVerticalSnapPoint(item.Value);
                        }
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    // add one, remove one
                    if (e.NewItems != null)
                    {
                        foreach (double? item in e.NewItems)
                        {
                            if (!item.HasValue) continue;
                            AddVerticalSnapPoint(item.Value);
                        }
                    }
                    if (e.OldItems != null)
                    {
                        foreach (double? item in e.OldItems)
                        {
                            if (!item.HasValue) continue;
                            RemoveVerticalSnapPoint(item.Value);
                        }
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    // requires nothing
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    // clear all
                    canVertical?.Children.Clear();
                    break;
                default:
                    // don't know? do nothing
                    break;
            }
        }

#if NETCOREAPP
        private void HorizontalSnapPoints_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
#else
        private void HorizontalSnapPoints_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
#endif
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    if (e.NewItems != null)
                    {
                        foreach (double? item in e.NewItems)
                        {
                            if (!item.HasValue) continue;
                            AddHorizontalSnapPoint(item.Value);
                        }
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    if (e.OldItems != null)
                    {
                        foreach (double? item in e.OldItems)
                        {
                            if (!item.HasValue) continue;
                            RemoveHorizontalSnapPoint(item.Value);
                        }
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    if (e.NewItems != null)
                    {
                        foreach (double? item in e.NewItems)
                        {
                            if (!item.HasValue) continue;
                            AddHorizontalSnapPoint(item.Value);
                        }
                    }
                    if (e.OldItems != null)
                    {
                        foreach (double? item in e.OldItems)
                        {
                            if (!item.HasValue) continue;
                            RemoveHorizontalSnapPoint(item.Value);
                        }
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    // requires nothing
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    // clear all
                    canHorizontal?.Children.Clear();
                    break;
                default:
                    // don't know? do nothing
                    break;
            }
        }
        #endregion

        #region Internal Add/Remove methods

        private void AddVerticalSnapPoint(double point)
        {
            if (canVertical == null)
            {
                return;
            }

            //double sshalf = SelectorSize / 2;
            Border b = new Border();
            b.BorderThickness = new Thickness(0.75);
            b.BorderBrush = SnapLineBrush;
            b.Width = 1;
            b.Tag = point;
            b.HorizontalAlignment = HorizontalAlignment.Left;
            b.VerticalAlignment = VerticalAlignment.Stretch;
            b.Margin = new Thickness(GetCanvasWidth() * point, 0, 0, 0);
            b.SnapsToDevicePixels = true;
            b.UseLayoutRounding = false;

            canVertical.Children.Add(b);
        }

        private void RemoveVerticalSnapPoint(double point)
        {
            if (canVertical == null) return;

            List<UIElement> toRemove = new List<UIElement>();

#if NETCOREAPP
            foreach (UIElement? item in canVertical.Children)
#else
            foreach (UIElement item in canVertical.Children)
#endif
            {
                if (item != null && item is Border b)
                {
                    if (b.Tag is double d)
                    {
                        if (d == point)
                        {
                            toRemove.Add(b);
                        }
                    }
                }
            }

            foreach (UIElement item in toRemove)
            {
                canVertical.Children.Remove(item);
            }
        }

        private void AddHorizontalSnapPoint(double point)
        {
            if (canHorizontal == null)
            {
                return;
            }

            //double sshalf = SelectorSize / 2;
            Border b = new Border();
            b.BorderThickness = new Thickness(0.75);
            b.BorderBrush = SnapLineBrush;
            b.Height = 1;
            b.Tag = point;
            b.HorizontalAlignment = HorizontalAlignment.Stretch;
            b.VerticalAlignment = VerticalAlignment.Top;
            b.Margin = new Thickness(0, GetCanvasHeight() * point, 0, 0);
            b.SnapsToDevicePixels = true;
            b.UseLayoutRounding = false;

            canHorizontal.Children.Add(b);
        }

        private void RemoveHorizontalSnapPoint(double point)
        {
            if (canHorizontal == null) return;

            List<UIElement> toRemove = new List<UIElement>();

#if NETCOREAPP
            foreach (UIElement? item in canHorizontal.Children)
#else
            foreach (UIElement item in canHorizontal.Children)
#endif
            {
                if (item != null && item is Border b)
                {
                    if (b.Tag is double d)
                    {
                        if (d == point)
                        {
                            toRemove.Add(b);
                        }
                    }
                }
            }

            foreach (UIElement item in toRemove)
            {
                canHorizontal.Children.Remove(item);
            }
        }
        #endregion

        private void PositionSelect_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // here, we will be shifting the snaplines and selector around to match the new size of the control
            double sshalf = SelectorSize / 2;

            // clear the cached values, since they'll now be different
            _canvasWidth = double.NaN;
            _canvasHeight = double.NaN;

            if (e.WidthChanged)
            {
                if (canVertical != null)
                {
                    // snaplines
#if NETCOREAPP
                    foreach (UIElement? item in canVertical.Children)
#else
                    foreach (UIElement item in canVertical.Children)
#endif
                    {
                        if (item != null && item is Border b)
                        {
                            if (b.Tag is double d)
                            {
                                b.Margin = new Thickness(GetCanvasWidth() * d, 0, 0, 0);
                                //b.Margin = new Thickness(grdSelArea.ActualWidth * d + sshalf, 0, 0, 0);
                            }
                        }
                    }
                }

                // selector
                if (ellSelect != null) Canvas.SetLeft(ellSelect, (GetCanvasWidth() * SelectedX) - sshalf);
            }

            if (e.HeightChanged)
            {
                if (canHorizontal != null)
                {
                    // snaplines
#if NETCOREAPP
                    foreach (UIElement? item in canHorizontal.Children)
#else
                    foreach (UIElement item in canHorizontal.Children)
#endif
                    {
                        if (item != null && item is Border b)
                        {
                            if (b.Tag is double d)
                            {
                                b.Margin = new Thickness(0, GetCanvasHeight() * d, 0, 0);
                                //b.Margin = new Thickness(0, grdSelArea.ActualHeight * d + sshalf, 0, 0);
                            }
                        }
                    }
                }

                // selector
                if (ellSelect != null) Canvas.SetTop(ellSelect, (GetCanvasHeight() * SelectedY) - sshalf);
            }
        }

        #endregion

        #region Keyboard Controls

        /// <summary>
        /// Get or set the amount the selector is moved each time an arrow key is pressed (while the control is focused).
        /// </summary>
        public double KeyMoveStep { get => (double)GetValue(KeyMoveStepProperty); set => SetValue(KeyMoveStepProperty, value); }

        /// <summary>
        /// A dependency property object backing the related property. See the property itself for more details.
        /// </summary>
        public static readonly DependencyProperty KeyMoveStepProperty
            = DependencyProperty.Register("KeyMoveStep", typeof(double), typeof(RelativePositionSelect),
            new FrameworkPropertyMetadata(0.05));


        //private void RelativePositionSelect_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        //{
        //    // brdrKeyFocus.BorderBrush = Colors.Transparent.ToBrush();
        //}

        //private void RelativePositionSelect_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        //{
        //    // brdrKeyFocus.BorderBrush = KeyboardFocusHighlight;
        //}

        private void PositionSelect_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    SelectedX -= KeyMoveStep;
                    break;
                case Key.Right:
                    SelectedX += KeyMoveStep;
                    break;
                case Key.Up:
                    SelectedY -= KeyMoveStep;
                    break;
                case Key.Down:
                    SelectedY += KeyMoveStep;
                    break;
                case Key.Home:
                    SelectedX = 0;
                    break;
                case Key.End:
                    SelectedX = 1;
                    break;
                case Key.PageUp:
                    SelectedY = 0;
                    break;
                case Key.PageDown:
                    SelectedY = 1;
                    break;
                default:
                    break;
            }
        }

        //private void RelativePositionSelect_KeyDown(object sender, KeyEventArgs e)
        //{

        //}

        private void PositionSelect_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                case Key.Right:
                case Key.Up:
                case Key.Down:
                case Key.Home:
                case Key.End:
                case Key.PageUp:
                case Key.PageDown:
                    if (e.OriginalSource == this)
                    {
                        //e.Handled = true;
                    }
                    break;
                default:
                    break;
            }
        }

        private void PositionSelect_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                case Key.Right:
                case Key.Up:
                case Key.Down:
                case Key.Home:
                case Key.End:
                case Key.PageUp:
                case Key.PageDown:
                    if (e.OriginalSource == this)
                    {
                        e.Handled = true;
                    }
                    break;
                default:
                    break;
            }
        }

        #endregion

        /// <summary>
        /// Get or set the size of the selector. The larger the selector, the easier it will be to see and also to click and drag, 
        /// but also harder to precisely get to or visualize a specific value (although snaplines can help rectify this).
        /// </summary>
        public double SelectorSize { get => (double)GetValue(SelectorSizeProperty); set => SetValue(SelectorSizeProperty, value); }

        /// <summary>The backing dependency property for <see cref="SelectorSize"/>. See the related property for details.</summary>
        public static readonly DependencyProperty SelectorSizeProperty
            = DependencyProperty.Register(nameof(SelectorSize), typeof(double), typeof(PositionSelect),
            new FrameworkPropertyMetadata(9.0));

        #region Mouse Events

        bool selectMode = false;

        private void PositionSelect_MouseDown(object sender, MouseButtonEventArgs e)
        {
            selectMode = true;

            // get point from mouse
            Point p = Mouse.GetPosition(this);
            SelectPoint(p);
        }

        private void PositionSelect_MouseUp(object sender, MouseButtonEventArgs e)
        {
            selectMode = false;
        }

        private void PositionSelect_MouseMove(object sender, MouseEventArgs e)
        {
            if (selectMode)
            {
                // get point from mouse
                Point p = Mouse.GetPosition(this);
                SelectPoint(p);
            }
        }

        private void PositionSelect_MouseLeave(object sender, MouseEventArgs e)
        {
            selectMode = false;
        }

        #endregion

        #region SelectorPosition Properties

        //private double oWidth = 0.5; // X
        //private double oHeight = 0.5; // Y

        bool _updateValues = true;

        /// <summary>
        /// Get or set the selected point, on both the X and Y axes.
        /// This is how far the selector is from the top-left corner of the control, on a relative scale from <c>0.0</c> to <c>1.0</c> in both axes.
        /// </summary>
        /// <remarks>
        /// When setting this property, the event SelectedPositionChanged will fire twice: once after the width (X) is changed, and once after the height (Y) is changed.
        /// </remarks>
        public Point SelectedPoint { get => (Point)GetValue(SelectedPointProperty); set => SetValue(SelectedPointProperty, value); }

        /// <summary>The backing dependency property for <see cref="SelectedPoint"/>. See the related property for details.</summary>
        public static readonly DependencyProperty SelectedPointProperty
            = DependencyProperty.Register(nameof(SelectedPoint), typeof(Point), typeof(PositionSelect),
            new FrameworkPropertyMetadata(new Point(0.5, 0.5), (d, e) => d.PerformAs<PositionSelect>((o) => o.OnSelectedPointChanged(e))));

        private void OnSelectedPointChanged(DependencyPropertyChangedEventArgs e)
        {
            if (!_updateValues) return;

            if (e.NewValue is Point p)
            {
                // validate values
                Point newP = new Point(ValidateValue(p.X), ValidateValue(p.Y));

                if (newP != p)
                {
                    // update the property and start over
                    SelectedPoint = newP;
                    return;
                }

                _updateValues = false;

                SelectedX = p.X;
                SelectedY = p.Y;

                _updateValues = true;

                Point o = new Point(0, 0);
                if (e.OldValue is Point p2)
                {
                    o = p2;
                }

                PositionEllipse(p);
                RaisePositionChangedEvent(o, p);
            }

        }

        /// <summary>
        /// Get or set the selected value on the horizontal (X) axis.
        /// This is how far from the left edge of the control that the selector is, on a relative scale from <c>0.0</c> to <c>1.0</c>.
        /// </summary>
        public double SelectedX { get => (double)GetValue(SelectedXProperty); set => SetValue(SelectedXProperty, value); }

        /// <summary>The backing dependency property for <see cref="SelectedX"/>. See the related property for details.</summary>
        public static readonly DependencyProperty SelectedXProperty
            = DependencyProperty.Register(nameof(SelectedX), typeof(double), typeof(PositionSelect),
            new FrameworkPropertyMetadata(0.5, (d, e) => d.PerformAs<PositionSelect>((o) => o.OnSelectedXChange(e))));


        /// <summary>
        /// Get or set the selected value on the vertical (Y) axis.
        /// This is how far from the top of the control that the selector is, on a relative scale from <c>0.0</c> to <c>1.0</c>.
        /// </summary>
        public double SelectedY { get => (double)GetValue(SelectedYProperty); set => SetValue(SelectedYProperty, value); }

        /// <summary>The backing dependency property for <see cref="SelectedY"/>. See the related property for details.</summary>
        public static readonly DependencyProperty SelectedYProperty
            = DependencyProperty.Register(nameof(SelectedY), typeof(double), typeof(PositionSelect),
            new FrameworkPropertyMetadata(0.5, (d, e) => d.PerformAs<PositionSelect>((o) => o.OnSelectedYChange(e))));


        private void OnSelectedXChange(DependencyPropertyChangedEventArgs e)
        {
            if (!_updateValues) return;

            if (e.NewValue is double nx)
            {
                // validate values
                double newP = ValidateValue(nx);

                if (newP != nx)
                {
                    // update the property and start over
                    SelectedX = newP;
                    return;
                }

                _updateValues = false;

                Point p = new Point(nx, SelectedY);

                SelectedPoint = p;

                _updateValues = true;

                Point o = new Point(0, SelectedY);
                if (e.OldValue is double p2)
                {
                    o.X = p2;
                }

                PositionEllipse(p);
                RaisePositionChangedEvent(o, p);
            }

        }

        private void OnSelectedYChange(DependencyPropertyChangedEventArgs e)
        {
            if (!_updateValues) return;

            if (e.NewValue is double ny)
            {
                // validate values
                double newP = ValidateValue(ny);

                if (newP != ny)
                {
                    // update the property and start over
                    SelectedY = newP;
                    return;
                }

                _updateValues = false;

                Point p = new Point(SelectedX, ny);

                SelectedPoint = p;

                _updateValues = true;

                Point o = new Point(SelectedX, 0);
                if (e.OldValue is double p2)
                {
                    o.Y = p2;
                }

                PositionEllipse(p);
                RaisePositionChangedEvent(o, p);
            }

        }

        #region Property Helper Methods / SelectedPositionChanged event

        double ValidateValue(double input)
        {
            if (input < 0) return 0;
            else if (input > 1) return 1;
            else return input;
        }

        void PositionEllipse(Point p)
        {
            if (ellSelect == null) return;

            double sshalf = SelectorSize / 2;
            Canvas.SetTop(ellSelect, (GetCanvasHeight() * p.Y) - sshalf);
            Canvas.SetLeft(ellSelect, (GetCanvasWidth() * p.X) - sshalf);
        }

        /// <summary>
        /// Raise the <see cref="SelectedPositionChanged"/> routed event, reporting both the old and new values for <see cref="SelectedPoint"/>.
        /// </summary>
        /// <param name="oldValue">the old value of <see cref="SelectedPoint"/> prior to the change</param>
        /// <param name="newValue">the new value of <see cref="SelectedPoint"/> after the change</param>
        protected void RaisePositionChangedEvent(Point oldValue, Point newValue)
        {
            RoutedPropertyChangedEventArgs<Point> re = new RoutedPropertyChangedEventArgs<Point>
                (oldValue, newValue, SelectedPositionChangedEvent);
            re.Source = this;
            RaiseEvent(re);
        }

        /// <summary>
        /// The backing routed event object for <see cref="SelectedPositionChanged"/>. Please see the related event for details.
        /// </summary>
        public static readonly RoutedEvent SelectedPositionChangedEvent = EventManager.RegisterRoutedEvent(
            nameof(SelectedPositionChanged), RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<Point>), typeof(PositionSelect));

        /// <summary>
        /// Raised when the <see cref="SelectedPoint"/> property is changed.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<Point> SelectedPositionChanged
        {
            add { AddHandler(SelectedPositionChangedEvent, value); }
            remove { RemoveHandler(SelectedPositionChangedEvent, value); }
        }

        #endregion

        #endregion

        #region Canvas Measurements / Internal SelectPoint

        // cache the measurements
        double _canvasWidth = double.NaN;
        double _canvasHeight = double.NaN;

        private double GetCanvasWidth()
        {
            return double.IsNaN(_canvasWidth) ? ActualWidth - BorderThickness.Left - BorderThickness.Right - SelectorSize : _canvasWidth;
        }

        private double GetCanvasHeight()
        {
            return double.IsNaN(_canvasHeight) ? ActualHeight - BorderThickness.Top - BorderThickness.Bottom - SelectorSize : _canvasHeight;
        }

        private void SelectPoint(Point p)
        {
            // make sure point is in bounds
            if (p.X < 0)
            {
                p.X = 0;
            }
            else if (p.X > GetCanvasWidth())
            {
                p.X = GetCanvasWidth();
            }

            if (p.Y < 0)
            {
                p.Y = 0;
            }
            else if (p.Y > GetCanvasHeight())
            {
                p.Y = GetCanvasHeight();
            }

            double sshalf = SelectorSize / 2;

            // check for snap positions
            if (SnapToSnaplines && canVertical != null && canHorizontal != null)
            {
                double widthMin = p.X - SnapDistance;
                double widthMax = p.X + SnapDistance;
                double heightMin = p.Y - SnapDistance;
                double heightMax = p.Y + SnapDistance;

                // check vertical snap points
#if NETCOREAPP
                foreach (UIElement? item in canVertical.Children)
#else
                foreach (UIElement item in canVertical.Children)
#endif
                {
                    if (item != null && item is Border b)
                    {
                        // check if the margin is near the point (by considering the SnapDistance)
                        if (b.Margin.Left > widthMin && b.Margin.Left < widthMax)
                        {
                            // gotcha! snap!
                            p.X = b.Margin.Left;
                        }
                    }
                }

                // check horizontal snap points
#if NETCOREAPP
                foreach (UIElement? item in canHorizontal.Children)
#else
                foreach (UIElement item in canHorizontal.Children)
#endif
                {
                    if (item != null && item is Border b)
                    {
                        // check if the margin is near the point (by considering the SnapDistance)
                        if (b.Margin.Top > heightMin && b.Margin.Top < heightMax)
                        {
                            // gotcha! snap!
                            p.Y = b.Margin.Top;
                        }
                    }
                }
            }

            // move selector to this new point
            if (ellSelect != null)
            {
                Canvas.SetLeft(ellSelect, p.X - sshalf);
                Canvas.SetTop(ellSelect, p.Y - sshalf);
            }

            // update outputs
            _updateValues = false;

            Point oldValue = new Point(SelectedX, SelectedY);

            SelectedX = p.X / GetCanvasWidth();
            SelectedY = p.Y / GetCanvasHeight();

            _updateValues = true;

            RaisePositionChangedEvent(oldValue, new Point(SelectedX, SelectedY));
        }

        #endregion
    }
}
