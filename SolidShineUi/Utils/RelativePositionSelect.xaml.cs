using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows.Shapes;

namespace SolidShineUi.Utils
{
    /// <summary>
    /// Interaction logic for RelativePositionSelect.xaml
    /// </summary>
    public partial class RelativePositionSelect : UserControl
    {
        public RelativePositionSelect()
        {
            InitializeComponent();

            HorizontalSnapPoints.CollectionChanged += HorizontalSnapPoints_CollectionChanged;
            VerticalSnapPoints.CollectionChanged += VerticalSnapPoints_CollectionChanged;
            SizeChanged += RelativePositionSelect_SizeChanged;
            GotKeyboardFocus += RelativePositionSelect_GotKeyboardFocus;
            LostKeyboardFocus += RelativePositionSelect_LostKeyboardFocus;
            PreviewKeyDown += RelativePositionSelect_PreviewKeyDown;
            PreviewKeyUp += RelativePositionSelect_PreviewKeyUp;
            KeyDown += RelativePositionSelect_KeyDown;
            KeyUp += RelativePositionSelect_KeyUp;

            //KeyboardNavigation.SetTabNavigation(this, KeyboardNavigationMode.Continue);
            KeyboardNavigation.SetDirectionalNavigation(this, KeyboardNavigationMode.Contained);
        }

        #region Color Scheme
#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? ColorSchemeChanged;
#else
        public event DependencyPropertyChangedEventHandler ColorSchemeChanged;
#endif

        public static readonly DependencyProperty ColorSchemeProperty
            = DependencyProperty.Register("ColorScheme", typeof(ColorScheme), typeof(RelativePositionSelect),
            new FrameworkPropertyMetadata(new ColorScheme(), new PropertyChangedCallback(OnColorSchemeChanged)));

        public static void OnColorSchemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
#if NETCOREAPP
            ColorScheme cs = (e.NewValue as ColorScheme)!;
#else
            ColorScheme cs = e.NewValue as ColorScheme;
#endif

            if (d is RelativePositionSelect r)
            {
                r.ColorSchemeChanged?.Invoke(d, e);
                r.ApplyColorScheme(cs);
            }
        }

        /// <summary>
        /// Get or set the color scheme used for this RelativePositionSelect. For easier color scheme management, bind this to the window or larger control you're using.
        /// </summary>
        public ColorScheme ColorScheme
        {
            get => (ColorScheme)GetValue(ColorSchemeProperty);
            set => SetValue(ColorSchemeProperty, value);
        }

        public void ApplyColorScheme(ColorScheme cs)
        {
            if (cs == null)
            {
                return;
            }
            if (cs != ColorScheme)
            {
                ColorScheme = cs;
                return;
            }

            if (cs.IsHighContrast)
            {
                ControlBackground = cs.BackgroundColor.ToBrush();
                BackgroundDisabledBrush = cs.BackgroundColor.ToBrush();
                SelectorBrush = cs.ForegroundColor.ToBrush();
                SnapLineBrush = cs.BorderColor.ToBrush();
                KeyboardFocusHighlight = cs.SecondHighlightColor.ToBrush();
            }
            else
            {
                ControlBackground = cs.LightBackgroundColor.ToBrush();
                BackgroundDisabledBrush = cs.LightDisabledColor.ToBrush();
                SelectorBrush = cs.HighlightColor.ToBrush();
                SnapLineBrush = cs.SecondaryColor.ToBrush();
                KeyboardFocusHighlight = cs.ThirdHighlightColor.ToBrush();
            }

            BorderBrush = cs.BorderColor.ToBrush();
            BackgroundDisabledBrush = cs.LightDisabledColor.ToBrush();
            BorderDisabledBrush = cs.DarkDisabledColor.ToBrush();
            SelectorDisabledBrush = cs.DarkDisabledColor.ToBrush();
            Foreground = cs.ForegroundColor.ToBrush();
        }
        #endregion

        #region Brushes

        /// <summary>
        /// Get or set the brush used for the background of the RelativePositionSelect's box.
        /// </summary>
        [Category("Brushes")]
        public Brush ControlBackground
        {
            get => (Brush)GetValue(ControlBackgroundProperty);
            set => SetValue(ControlBackgroundProperty, value);
        }

        /// <summary>
        /// Get or set the brush used for the selector ellipse in the RelativePositionSelect.
        /// </summary>
        [Category("Brushes")]
        public Brush SelectorBrush
        {
            get => (Brush)GetValue(SelectorBrushProperty);
            set => SetValue(SelectorBrushProperty, value);
        }

        /// <summary>
        /// Get or set the brush to use for the background of the RelativePositionSelect's box when it is disabled.
        /// </summary>
        [Category("Brushes")]
        public Brush BackgroundDisabledBrush
        {
            get => (Brush)GetValue(BackgroundDisabledBrushProperty);
            set => SetValue(BackgroundDisabledBrushProperty, value);
        }

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
        /// Get or set the brush used for the border of the RelativePositionSelect's box.
        /// </summary>
        [Category("Brushes")]
        public new Brush BorderBrush
        {
            get => (Brush)GetValue(BorderBrushProperty);
            set => SetValue(BorderBrushProperty, value);
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

        public static readonly DependencyProperty ControlBackgroundProperty = DependencyProperty.Register(
            "ControlBackground", typeof(Brush), typeof(RelativePositionSelect),
            new PropertyMetadata(new SolidColorBrush(ColorsHelper.White)));

        public static readonly DependencyProperty SelectorBrushProperty = DependencyProperty.Register(
            "SelectorBrush", typeof(Brush), typeof(RelativePositionSelect),
            new PropertyMetadata(new SolidColorBrush(ColorsHelper.Black)));

        public static readonly DependencyProperty BackgroundDisabledBrushProperty = DependencyProperty.Register(
            "BackgroundDisabledBrush", typeof(Brush), typeof(RelativePositionSelect),
            new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));

        public static readonly DependencyProperty BorderDisabledBrushProperty = DependencyProperty.Register(
            "BorderDisabledBrush", typeof(Brush), typeof(RelativePositionSelect),
            new PropertyMetadata(new SolidColorBrush(Colors.Gray)));

        public static readonly DependencyProperty SelectorDisabledBrushProperty = DependencyProperty.Register(
            "SelectorDisabledBrush", typeof(Brush), typeof(RelativePositionSelect),
            new PropertyMetadata(new SolidColorBrush(Colors.DimGray)));

        public static readonly new DependencyProperty BorderBrushProperty = DependencyProperty.Register(
            "BorderBrush", typeof(Brush), typeof(RelativePositionSelect),
            new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        public static readonly DependencyProperty SnapLineBrushProperty = DependencyProperty.Register(
            "SnapLineBrush", typeof(Brush), typeof(RelativePositionSelect),
            new PropertyMetadata(new SolidColorBrush(Colors.LightGray), OnSnapLineBrushChanged));

        public static readonly DependencyProperty KeyboardFocusHighlightProperty = DependencyProperty.Register(
            "KeyboardFocusHighlight", typeof(Brush), typeof(RelativePositionSelect),
            new PropertyMetadata(new SolidColorBrush(Colors.LightGray), OnKeyboardFocusHighlightBrushChanged));

        public static void OnSnapLineBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RelativePositionSelect r)
            {
                r.UpdateSnapLineBrush();
            }
        }

        internal protected void UpdateSnapLineBrush()
        {
            foreach (UIElement item in canVertical.Children)
            {
                if (item is Border b)
                {
                    b.BorderBrush = SnapLineBrush;
                }
            }
            foreach (UIElement item in canHorizontal.Children)
            {
                if (item is Border b)
                {
                    b.BorderBrush = SnapLineBrush;
                }
            }
        }

        public static void OnKeyboardFocusHighlightBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RelativePositionSelect r)
            {
                r.UpdateSnapLineBrush();
            }
        }

        internal protected void UpdateKeyboardFocusHighlightBrush()
        {
            if (IsKeyboardFocused || HasEffectiveKeyboardFocus)
            {
                brdrKeyFocus.BorderBrush = KeyboardFocusHighlight;
            }
        }

        #endregion

        #region Snap Points

        public bool SnapToSnapLines { get; set; } = true;

        public double SnapDistance { get; set; } = 3;

        public ObservableCollection<double> HorizontalSnapPoints { get; set; } = new ObservableCollection<double>();
        public ObservableCollection<double> VerticalSnapPoints { get; set; } = new ObservableCollection<double>();

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
                    canVertical.Children.Clear();
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
                    canHorizontal.Children.Clear();
                    break;
                default:
                    // don't know? do nothing
                    break;
            }
        }

        private void AddVerticalSnapPoint(double point)
        {
            double sshalf = SelectorSize / 2;
            Border b = new Border();
            b.BorderThickness = new Thickness(0.75);
            b.BorderBrush = SnapLineBrush;
            b.Width = 1;
            b.Tag = point;
            b.HorizontalAlignment = HorizontalAlignment.Left;
            b.VerticalAlignment = VerticalAlignment.Stretch;
            b.Margin = new Thickness(grdSelArea.ActualWidth * point + sshalf, 0, 0, 0);
            b.SnapsToDevicePixels = true;
            b.UseLayoutRounding = false;

            canVertical.Children.Add(b);
        }

        private void RemoveVerticalSnapPoint(double point)
        {
            List<UIElement> toRemove = new List<UIElement>();

            foreach (UIElement item in canVertical.Children)
            {
                if (item is Border b)
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
            double sshalf = SelectorSize / 2;
            Border b = new Border();
            b.BorderThickness = new Thickness(0.75);
            b.BorderBrush = SnapLineBrush;
            b.Height = 1;
            b.Tag = point;
            b.HorizontalAlignment = HorizontalAlignment.Stretch;
            b.VerticalAlignment = VerticalAlignment.Top;
            b.Margin = new Thickness(0, grdSelArea.ActualHeight * point + sshalf, 0, 0);
            b.SnapsToDevicePixels = true;
            b.UseLayoutRounding = false;

            canHorizontal.Children.Add(b);
        }

        private void RemoveHorizontalSnapPoint(double point)
        {
            List<UIElement> toRemove = new List<UIElement>();

            foreach (UIElement item in canHorizontal.Children)
            {
                if (item is Border b)
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

        private void RelativePositionSelect_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double sshalf = SelectorSize / 2;

            if (e.WidthChanged)
            {
                foreach (UIElement item in canVertical.Children)
                {
                    if (item is Border b)
                    {
                        if (b.Tag is double d)
                        {
                            b.Margin = new Thickness(grdSelArea.ActualWidth * d + sshalf, 0, 0, 0);
                        }
                    }
                }
                Canvas.SetLeft(ellSelect, (grdSelArea.ActualWidth * SelectedWidth) - sshalf);
            }

            if (e.HeightChanged)
            {
                foreach (UIElement item in canHorizontal.Children)
                {
                    if (item is Border b)
                    {
                        if (b.Tag is double d)
                        {
                            b.Margin = new Thickness(0, grdSelArea.ActualHeight * d + sshalf, 0, 0);
                        }
                    }
                }
                Canvas.SetTop(ellSelect, (grdSelArea.ActualHeight * SelectedHeight) - sshalf);
            }
        }

        #endregion

        #region Keyboard Controls

        /// <summary>
        /// Get or set the amount the selector is moved each time an arrow key is pressed (while the control is focused).
        /// </summary>
        public double KeyMoveStep { get; set; } = 0.05;

        private void RelativePositionSelect_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            brdrKeyFocus.BorderBrush = Colors.Transparent.ToBrush();
        }

        private void RelativePositionSelect_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            brdrKeyFocus.BorderBrush = KeyboardFocusHighlight;
        }

        private void RelativePositionSelect_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    SelectedWidth -= KeyMoveStep;
                    break;
                case Key.Right:
                    SelectedWidth += KeyMoveStep;
                    break;
                case Key.Up:
                    SelectedHeight -= KeyMoveStep;
                    break;
                case Key.Down:
                    SelectedHeight += KeyMoveStep;
                    break;
                case Key.Home:
                    SelectedWidth = 0;
                    break;
                case Key.End:
                    SelectedWidth = 1;
                    break;
                case Key.PageUp:
                    SelectedHeight = 0;
                    break;
                case Key.PageDown:
                    SelectedHeight = 1;
                    break;
                default:
                    break;
            }
        }

        private void RelativePositionSelect_KeyDown(object sender, KeyEventArgs e)
        {
    
        }

        private void RelativePositionSelect_PreviewKeyUp(object sender, KeyEventArgs e)
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

        private void RelativePositionSelect_PreviewKeyDown(object sender, KeyEventArgs e)
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

        public double SelectorSize
        {
            get { return ellSelect.Width; }
            set
            {
                ellSelect.Width = value;
                ellSelect.Height = value;

                double sshalf = value / 2;
                grdSelArea.Margin = new Thickness(sshalf);
                brdrKeyFocus.BorderThickness = new Thickness(sshalf);
            }
        }

        bool selectMode = false;

        private void grdGuidelines_MouseDown(object sender, MouseButtonEventArgs e)
        {
            selectMode = true;

            // get point from mouse
            Point p = Mouse.GetPosition(grdSelArea);
            SelectPoint(p);
        }

        private void grdGuidelines_MouseUp(object sender, MouseButtonEventArgs e)
        {
            selectMode = false;
        }

        private void grdGuidelines_MouseMove(object sender, MouseEventArgs e)
        {
            if (selectMode)
            {
                // get point from mouse
                Point p = Mouse.GetPosition(grdSelArea);
                SelectPoint(p);
            }
        }

        private void SelectPoint(Point p)
        {
            // make sure point is in bounds
            if (p.X < 0)
            {
                p.X = 0;
            }
            else if (p.X > grdSelArea.ActualWidth)
            {
                p.X = grdSelArea.ActualWidth;
            }

            if (p.Y < 0)
            {
                p.Y = 0;
            }
            else if (p.Y > grdSelArea.ActualHeight)
            {
                p.Y = grdSelArea.ActualHeight;
            }

            double sshalf = SelectorSize / 2;

            // check for snap positions
            if (SnapToSnapLines)
            {
                double widthMin = p.X - SnapDistance;
                double widthMax = p.X + SnapDistance;
                double heightMin = p.Y - SnapDistance;
                double heightMax = p.Y + SnapDistance;

                // check vertical snap points
                foreach (UIElement item in canVertical.Children)
                {
                    if (item is Border b)
                    {
                        if (b.Margin.Left > widthMin && b.Margin.Left < widthMax)
                        {
                            p.X = b.Margin.Left - sshalf;
                        }
                    }
                }

                // check horizontal snap points
                foreach (UIElement item in canHorizontal.Children)
                {
                    if (item is Border b)
                    {
                        if (b.Margin.Top > heightMin && b.Margin.Top < heightMax)
                        {
                            p.Y = b.Margin.Top - sshalf;
                        }
                    }
                }
            }

            // move selector to this new point
            //ellSelect.Margin = new Thickness(p.X - sshalf, p.Y - sshalf, 0, 0);
            Canvas.SetLeft(ellSelect, p.X - sshalf);
            Canvas.SetTop(ellSelect, p.Y - sshalf);

            // update outputs
            oWidth = p.X / grdSelArea.ActualWidth;
            oHeight = p.Y / grdSelArea.ActualHeight;
            SelectedPositionChanged?.Invoke(this, EventArgs.Empty);
        }

        private double oWidth = 0.5;
        private double oHeight = 0.5;

        public double SelectedWidth
        {
            get { return oWidth; }
            set
            {
                double val = value;
                if (val < 0)
                {
                    val = 0;
                }
                else if (val > 1)
                {
                    val = 1;
                }

                double sshalf = SelectorSize / 2;
                oWidth = val;
                //Thickness th = ellSelect.Margin;
                //ellSelect.Margin = new Thickness((grdSelArea.ActualWidth * val) - sshalf, th.Top, 0, 0);
                Canvas.SetLeft(ellSelect, (grdSelArea.ActualWidth * val) - sshalf);
                SelectedPositionChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public double SelectedHeight
        {
            get { return oHeight; }
            set
            {
                double val = value;
                if (val < 0)
                {
                    val = 0;
                }
                else if (val > 1)
                {
                    val = 1;
                }

                double sshalf = SelectorSize / 2;
                oHeight = val;
                //Thickness th = ellSelect.Margin;
                //ellSelect.Margin = new Thickness(th.Left, (grdSelArea.ActualHeight * val) - sshalf, 0, 0);
                Canvas.SetTop(ellSelect, (grdSelArea.ActualHeight * val) - sshalf);
                SelectedPositionChanged?.Invoke(this, EventArgs.Empty);
            }
        }

#if NETCOREAPP
        public event EventHandler? SelectedPositionChanged;
#else
        public event EventHandler SelectedPositionChanged;
#endif

        private void grdGuidelines_MouseLeave(object sender, MouseEventArgs e)
        {
            selectMode = false;
        }
    }
}
