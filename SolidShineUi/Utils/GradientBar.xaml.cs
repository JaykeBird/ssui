using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SolidShineUi.Utils
{
    /// <summary>
    /// A control that displays (and allows editing of) the stops of a gradient.
    /// Used with the <see cref="PropertyList.Dialogs.LinearGradientEditorDialog"/> and <see cref="PropertyList.Dialogs.RadialGradientEditorDialog"/>.
    /// </summary>
    [Localizability(LocalizationCategory.None), DefaultEvent(nameof(GradientChanged))]
    public partial class GradientBar : UserControl
    {
        /// <summary>
        /// Create a new GradientBar.
        /// </summary>
        public GradientBar()
        {
            InitializeComponent();
            Loaded += GradientBar_Loaded;
            Deselect();
        }

        private void GradientBar_Loaded(object sender, RoutedEventArgs e)
        {
            RenderStops();
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RenderStops();
        }

        #region Gradient Stop Collection / Rendering

        private GradientStopCollection _stops = new GradientStopCollection();

        /// <summary>
        /// Get or set the list of gradient stops to display in this GradientBar.
        /// </summary>
        /// <remarks>
        /// When setting a list of gradient stops, a deep-copy clone of all the stops are actually made, to avoid situations where 
        /// edits are applied when not wanted or if the inputted collection is frozen.
        /// </remarks>
        public GradientStopCollection GradientStops
        {
            get
            {
                return _stops;
            }
            set
            {
                // create copies of existing gradient stops
                // to prevent changes still applying even if the user hits "Cancel"
                _stops = value.CloneCurrentValue();

                // now let's arrange them and get them on the UI
                SortStops();
                CountStops();
                RenderBar();
                RenderStops();
            }
        }

        void SortStops()
        {
            IOrderedEnumerable<GradientStop> gso = _stops.OrderBy((stop) => stop.Offset );
            _stops = new GradientStopCollection(gso);
        }

        void RenderBar()
        {
            eleGradient.Background = new LinearGradientBrush(_stops, 0.0);
        }

        /// <summary>
        /// Clears the existing gradient stop items, and re-renders them. This applies the correct positioning and brushes.
        /// </summary>
        void RenderStops()
        {
            GradientStop selStop = new GradientStop();
            if (_selected != null)
            {
                selStop = _selected.GradientStop;
            }

            grdStops.Children.Clear();
            foreach (GradientStop stop in _stops)
            {
                var gsi = CreateGradientStopItem(stop);
                if (stop == selStop)
                {
                    SelectStop(gsi);
                }
            }
        }

        GradientStopItem CreateGradientStopItem(GradientStop stop)
        {
            GradientStopItem gsi = new GradientStopItem(stop);
            // set gsi left margin to relative position in grdStops (offset * brdrGradient.ActualWidth + 20)
            gsi.Margin = new Thickness(stop.Offset * brdrGradient.ActualWidth, 0, 0, 0);
            gsi.HorizontalAlignment = HorizontalAlignment.Left;
            gsi.VerticalAlignment = VerticalAlignment.Center;
            gsi.StopFill = StopFill;
            gsi.StopSelectedFill = StopSelectedFill;
            gsi.StopBorderBrush = StopBorderBrush;
            gsi.StopBorderHighlightBrush = StopBorderHighlightBrush;
            gsi.Click += gsi_Click;
            gsi.IsSelected = false;

            grdStops.Children.Add(gsi);

            return gsi;
        }
        #endregion

        #region GradientStopItem Brushes

        /// <summary>
        /// Get or set the brush used for the background of the gradient stop items in the editor.
        /// </summary>
        [Category("Brushes")]
        public Brush StopFill { get => (Brush)GetValue(StopFillProperty); set => SetValue(StopFillProperty, value); }

        /// <summary>The backing dependency property for <see cref="StopFill"/>. See the related property for details.</summary>
        public static readonly DependencyProperty StopFillProperty
            = DependencyProperty.Register(nameof(StopFill), typeof(Brush), typeof(GradientBar),
            new FrameworkPropertyMetadata(Colors.White.ToBrush(), OnStopBrushesChanged));

        /// <summary>
        /// Get or set the brush used for the background of the currently selected gradient stop item in the editor.
        /// </summary>
        [Category("Brushes")]
        public Brush StopSelectedFill { get => (Brush)GetValue(StopSelectedFillProperty); set => SetValue(StopSelectedFillProperty, value); }

        /// <summary>The backing dependency property for <see cref="StopSelectedFill"/>. See the related property for details.</summary>
        public static readonly DependencyProperty StopSelectedFillProperty
            = DependencyProperty.Register(nameof(StopSelectedFill), typeof(Brush), typeof(GradientBar),
            new FrameworkPropertyMetadata(Colors.Gainsboro.ToBrush(), OnStopBrushesChanged));


        /// <summary>
        /// Get or set the brush used for the border of the gradient stop items in the editor.
        /// </summary>
        [Category("Brushes")]
        public Brush StopBorderBrush { get => (Brush)GetValue(StopBorderBrushProperty); set => SetValue(StopBorderBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="StopBorderBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty StopBorderBrushProperty
            = DependencyProperty.Register(nameof(StopBorderBrush), typeof(Brush), typeof(GradientBar),
            new FrameworkPropertyMetadata(Colors.Black.ToBrush(), OnStopBrushesChanged));

        /// <summary>
        /// Get or set the brush used for the background of the gradient stop items in the editor, while they are highlighted (i.e. focus, mouse-over).
        /// </summary>
        [Category("Brushes")]
        public Brush StopBorderHighlightBrush { get => (Brush)GetValue(StopBorderHighlightBrushProperty); set => SetValue(StopBorderHighlightBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="StopBorderHighlightBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty StopBorderHighlightBrushProperty
            = DependencyProperty.Register(nameof(StopBorderHighlightBrush), typeof(Brush), typeof(GradientBar),
            new FrameworkPropertyMetadata(Colors.DimGray.ToBrush(), OnStopBrushesChanged));

        private static void OnStopBrushesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is GradientBar o)
            {
                o.RenderStops();
            }
        }

        #endregion

        #region ColorScheme

        /// <summary>
        /// Raised when the ColorScheme property is changed.
        /// </summary>
#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? ColorSchemeChanged;
#else
        public event DependencyPropertyChangedEventHandler ColorSchemeChanged;
#endif

        /// <summary>
        /// The backing dependency property for <see cref="ColorScheme"/>. See the related property for details.
        /// </summary>
        public static readonly DependencyProperty ColorSchemeProperty
            = DependencyProperty.Register("ColorScheme", typeof(ColorScheme), typeof(GradientBar),
            new FrameworkPropertyMetadata(new ColorScheme(), new PropertyChangedCallback(OnColorSchemeChanged)));

        /// <summary>
        /// Perform an action when the ColorScheme property has changed. Primarily used internally.
        /// </summary>
        /// <param name="d">The object containing the property that changed.</param>
        /// <param name="e">Event arguments about the property change.</param>
        public static void OnColorSchemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is ColorScheme cs)
            {
                if (d is GradientBar s)
                {
                    s.ColorSchemeChanged?.Invoke(d, e);
                    s.ApplyColorScheme(cs);
                }
            }
        }

        /// <summary>
        /// Get or set the color scheme used for this control. The color scheme can quickly apply a whole visual style to your control.
        /// </summary>
        public ColorScheme ColorScheme
        {
            get => (ColorScheme)GetValue(ColorSchemeProperty);
            set => SetValue(ColorSchemeProperty, value);
        }

        /// <summary>
        /// Apply a color scheme to this control. The color scheme can quickly apply a whole visual style to the control.
        /// </summary>
        /// <param name="cs">The color scheme to apply.</param>
        public void ApplyColorScheme(ColorScheme cs)
        {
            if (cs != ColorScheme)
            {
                ColorScheme = cs;
                return;
            }

            imgAdd.Source = IconLoader.LoadIcon("Add", cs);
            imgDelete.Source = IconLoader.LoadIcon("Delete", cs);
            imgSwap.Source = IconLoader.LoadIcon("Transfer", cs);
            imgLeft.Source = IconLoader.LoadIcon("LeftArrow", cs);
            imgRight.Source = IconLoader.LoadIcon("RightArrow", cs);
            imgOffset.Source = IconLoader.LoadIcon("LeftRightArrow", cs);
        }

        #endregion

        #region Selection Handling

#if NETCOREAPP
        private GradientStopItem? _selected = null;
#else
        private GradientStopItem _selected = null;
#endif

        /// <summary>
        /// Raised when the SelectedGradientStop is changed.
        /// </summary>
#if NETCOREAPP
        public event EventHandler? SelectionChanged;
#else
        public event EventHandler SelectionChanged;
#endif

        private void gsi_Click(object sender, RoutedEventArgs e)
        {
            if (sender == _selected) return; // this one is already selected

            GradientStopItem gsi = (GradientStopItem)sender;
            if (gsi == null) return;

            if (_selected != null) _selected.IsSelected = false;

            SelectStop(gsi);
        }

        void SelectStop(GradientStopItem gsi)
        {
            _selected = gsi;
            gsi.IsSelected = true;
            btnDelete.IsEnabled = true;
            nudOffset.IsEnabled = true;
            nudOpacity.IsEnabled = true;
            btnColorChange.IsEnabled = true;

            SelectionChanged?.Invoke(this, EventArgs.Empty);

            int index = _stops.IndexOf(_selected.GradientStop);
            txtCount.Text = index + 1 + " of " + _stops.Count;

            brdrCBack.Opacity = 1.0;
            rectColor.Fill = _selected.Color.ToBrush();
            nudOffset.Value = _selected.Offset;
            nudOpacity.Value = _selected.Color.A;
        }

        /// <summary>
        /// Deselect all gradient stops, so no stops are selected.
        /// </summary>
        public void Deselect()
        {
            _selected = null;
            btnDelete.IsEnabled = false;
            nudOffset.IsEnabled = false;
            nudOpacity.IsEnabled = false;
            btnColorChange.IsEnabled = false;
            SelectionChanged?.Invoke(this, EventArgs.Empty);
            brdrCBack.Opacity = 0.4;
            rectColor.Fill = null;
            txtCount.Text = "(none selected)";
        }

        /// <summary>
        /// Select a gradient stop for editing.
        /// </summary>
        /// <param name="gs">the gradient stop to edit</param>
        /// <exception cref="ArgumentException">thrown if this gradient stop isn't in this gradient bar</exception>
        /// <remarks>
        /// Use <see cref="GradientStops"/> to get a list of the stops currently in this gradient bar;
        /// use <see cref="Deselect"/> to deselect all gradient stops.
        /// </remarks>
        public void SelectStop(GradientStop gs)
        {
            if (_stops.Contains(gs))
            {
#if NETCOREAPP3_1
                foreach (GradientStopItem? gsi in grdStops.Children)
#else
                foreach (GradientStopItem gsi in grdStops.Children)
#endif
                {
                    if (gsi == null) continue;
                    if (gsi.GradientStop == gs)
                    {
                        if (gsi == _selected) return; // this one is already selected
                        if (_selected != null) _selected.IsSelected = false;
                        SelectStop(gsi);
                    }
                }
            }
            else
            {
                throw new ArgumentException("This stop is not in this gradient editor. Use the GradientStops property to get the current stops in the editor.", nameof(gs));
            }
        }

        /// <summary>
        /// Select a gradient stop for editing.
        /// </summary>
        /// <param name="index">the index of the gradient stop to edit</param>
        /// <exception cref="ArgumentOutOfRangeException">thrown if the index is less than 0 OR if the index is above the number of gradient stops in the editor</exception>
        /// <remarks>
        /// Use <see cref="GradientStops"/> to get a list of the stops currently in this gradient bar;
        /// use <see cref="Deselect"/> to deselect all gradient stops.
        /// </remarks>
        public void SelectStop(int index)
        {
            if (index >= _stops.Count || index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "The index is outside the number of stops in this editor");
            }

            GradientStop gradientStop = _stops[index];

#if NETCOREAPP3_1
            foreach (GradientStopItem? gsi in grdStops.Children)
#else
            foreach (GradientStopItem gsi in grdStops.Children)
#endif
            {
                if (gsi == null) continue;

                if (gsi.GradientStop == gradientStop)
                {
                    if (gsi == _selected) return; // this one is already selected
                    if (_selected != null) _selected.IsSelected = false;
                    SelectStop(gsi);
                }
            }
        }

        /// <summary>
        /// Get or set the gradient stop that is currently selected, or returns <c>null</c> if no gradient stop is selected.
        /// </summary>
        /// <exception cref="ArgumentNullException">thrown if attempting to set this property to <c>null</c>; use <see cref="Deselect"/> instead</exception>
        /// <exception cref="ArgumentException">thrown if attempting to set this property to a <see cref="GradientStop"/> that isn't in this editor</exception>
#if NETCOREAPP
        public GradientStop? SelectedGradientStop
#else
        public GradientStop SelectedGradientStop
#endif
        {
            get
            {
                return _selected?.GradientStop;
            }
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                SelectStop(value);
            }
        }
#endregion

        #region ShowControls

        /// <summary>
        /// Get or set if editing and navigation controls should be visible in the gradient bar. If not, then only the bar and stops are shown.
        /// </summary>
        public bool ShowControls { get => (bool)GetValue(ShowControlsProperty); set => SetValue(ShowControlsProperty, value); }

        /// <summary>The backing dependency property for <see cref="ShowControls"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ShowControlsProperty
            = DependencyProperty.Register("ShowControls", typeof(bool), typeof(GradientBar),
            new FrameworkPropertyMetadata(true));


        #endregion

        #region Editor

        #region Editor Functions

        /// <summary>
        /// Select the gradient stop to the left of the currently selected gradient stop.
        /// </summary>
        /// <remarks>
        /// If no stop is currently selected, this tries to select the left-most stop.
        /// </remarks>
        public void SelectNextLeft()
        {
            // somehow the grdStops.Children collection changes while the button is being pressed???
            // I'm not wanting to figure out what's going on, so this is the solution I came up with
            List<GradientStopItem> items = new List<GradientStopItem>();
#if NETCOREAPP
            foreach (GradientStopItem? item in grdStops.Children)
#else
            foreach (GradientStopItem item in grdStops.Children)
#endif
            {
                if (item == null) continue;

                items.Add(item);
            } // now we have a copy of the collection

            if (_selected == null) // nothing is selected right now
            {
                if (_stops.Count > 1)
                {
                    foreach (GradientStopItem item in items)
                    {
                        if (item.GradientStop == _stops[0])
                        {
                            SelectStop(item);
                        }
                    }
                }
            }
            else
            {
                int index = _stops.IndexOf(_selected.GradientStop);
                if (index > 0)
                {
                    // find the next stop to the left
                    GradientStop gsn = _stops[index - 1];
                    foreach (GradientStopItem item in items)
                    {
                        if (item.GradientStop == gsn)
                        {
                            SelectStop(item);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Select the gradient stop to the right of the currently selected gradient stop.
        /// </summary>
        /// <remarks>
        /// If no stop is currently selected, this tries to select the right-most stop.
        /// </remarks>
        public void SelectNextRight()
        {
            // somehow the grdStops.Children collection changes while the button is being pressed???
            // I'm not wanting to figure out what's going on, so this is the solution I came up with
            List<GradientStopItem> items = new List<GradientStopItem>();
#if NETCOREAPP
            foreach (GradientStopItem? item in grdStops.Children)
#else
            foreach (GradientStopItem item in grdStops.Children)
#endif
            {
                if (item == null) continue;

                items.Add(item);
            } // now we have a copy of the collection

            if (_selected == null) // nothing is selected right now
            {
                if (_stops.Count > 1)
                {
                    foreach (GradientStopItem item in items)
                    {
                        if (item.GradientStop == _stops[_stops.Count - 1])
                        {
                            SelectStop(item);
                        }
                    }
                }
            }
            else
            {

                int index = _stops.IndexOf(_selected.GradientStop);
                if (index < _stops.Count - 1)
                {
                    // find the next stop to the right
                    GradientStop gsn = _stops[index + 1];
                    foreach (GradientStopItem item in items)
                    {
                        if (item.GradientStop == gsn)
                        {
                            SelectStop(item);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Delete the currently selected stop.
        /// </summary>
        /// <remarks>
        /// If there are 1 or no stops, or no stop is currently selected, this does nothing.
        /// </remarks>
        public void Delete()
        {
            if (_stops.Count <= 1) return;

            if (_selected != null)
            {
                grdStops.Children.Remove(_selected);
                _stops.Remove(_selected.GradientStop);
                Deselect();
                CountStops();
                GradientChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Adds a new gradient stop onto the gradient bar.
        /// </summary>
        /// <remarks>
        /// If a gradient stop is currently selected, this will add a stop that is halfway between the current stop and the one to its left.
        /// </remarks>
        public void AddStop()
        {
            double offset = 0.5;
            Color color = Colors.DimGray;

            SortStops();

            if (_selected != null)
            {
                // let's find the current stop and the previous one
                // offset = _selected.Offset;
                int exIndex = _stops.IndexOf(_selected.GradientStop);
                if (exIndex > 0)
                {
                    GradientStop cur = _selected.GradientStop;
                    GradientStop gsp = _stops[exIndex - 1];

                    color = ColorsHelper.Blend(cur.Color, gsp.Color, 0.5);
                    if ((cur.Offset - gsp.Offset) > 0.1)
                    {
                        offset = gsp.Offset + ((cur.Offset - gsp.Offset) / 2);
                    }
                }
                else if (exIndex == 0 && _selected.Offset > 0.1)
                {
                    offset = 0.0;
                }
            }

            GradientStop gs = new GradientStop(color, offset);
            _stops.Add(gs);
            var gsi = CreateGradientStopItem(gs);
            SortStops();
            RenderBar();
            SelectStop(gsi);
            CountStops();
            GradientChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Reverses the order of all the stops in the gradient (so the left-most stop becomes the right-most, and vice versa).
        /// </summary>
        public void Swap()
        {
#if NETCOREAPP
            GradientStop? gsi = null;
#else
            GradientStop gsi = null;
#endif
            if (_selected != null) gsi = _selected.GradientStop;
            Deselect();

            foreach (GradientStop item in _stops)
            {
                item.Offset = 1 - item.Offset;
            }

            SortStops();
            RenderBar();
            RenderStops();

            List<GradientStopItem> items = new List<GradientStopItem>();
#if NETCOREAPP
            foreach (GradientStopItem? item in grdStops.Children)
#else
            foreach (GradientStopItem item in grdStops.Children)
#endif
            {
                if (item == null) continue;

                items.Add(item);
            }

            foreach (GradientStopItem item in items)
            {
                if (item.GradientStop == gsi)
                {
                    SelectStop(item);
                }
            }
        }

        /// <summary>
        /// Update the currently selected stop with a new value.
        /// </summary>
        /// <param name="offset">the offset value to change it to</param>
        /// <remarks>This affects the currently selected stop, which you can access via <see cref="SelectedGradientStop"/></remarks>
        public void UpdateSelectedStop(double offset)
        {
            if (_selected == null) return;
            else UpdateSelectedStop(offset, _selected.Color);
        }

        /// <summary>
        /// Update the currently selected stop with a new value.
        /// </summary>
        /// <param name="color">the color value to change it to</param>
        /// <remarks>This affects the currently selected stop, which you can access via <see cref="SelectedGradientStop"/></remarks>
        public void UpdateSelectedStop(Color color)
        {
            if (_selected == null) return;
            else UpdateSelectedStop(_selected.Offset, color);
        }

        /// <summary>
        /// Update the currently selected stop with a new value.
        /// </summary>
        /// <param name="offset">the offset value to change it to</param>
        /// <param name="color">the color value to change it to</param>
        /// <remarks>This affects the currently selected stop, which you can access via <see cref="SelectedGradientStop"/></remarks>
        public void UpdateSelectedStop(double offset, Color color)
        {
            if (_selected == null) return;
            else
            {
                if (offset != _selected.Offset)
                {
                    UpdateStopOffset(_selected, offset);
                }

                if (color != _selected.Color)
                {
                    _selected.Color = color;
                    _selected.GradientStop.Color = color;

                    RenderBar();
                    RenderStops();
                }
                GradientChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        #endregion

        /// <summary>
        /// Raised when the SelectedGradientStop is changed.
        /// </summary>
#if NETCOREAPP
        public event EventHandler? GradientChanged;
#else
        public event EventHandler GradientChanged;
#endif

        //void UpdateStopOffset(GradientStop stop, double offset)
        //{
        //    foreach (GradientStopItem item in grdStops.Children)
        //    {
        //        if (item.GradientStop == stop)
        //        {
        //            item.Offset = offset;
        //            item.GradientStop.Offset = offset;
        //        }
        //    }

        //    SortStops();
        //    RenderBar();
        //    RenderStops();
        //    GradientChanged?.DynamicInvoke(this, EventArgs.Empty);
        //}

        void UpdateStopOffset(GradientStopItem stop, double offset)
        {
            if (stop == null) return;

            stop.Offset = offset;
            stop.GradientStop.Offset = offset;

            SortStops();
            RenderBar();
            RenderStops();
            GradientChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Checks if there are enough stops to have the delete button enabled.
        /// </summary>
        void CountStops()
        {
            if (_stops.Count > 1)
            {
                btnDelete.IsEnabled = true;
            }
            else
            {
                btnDelete.IsEnabled = false;
            }
        }

        private void btnNextLeft_Click(object sender, RoutedEventArgs e)
        {
            SelectNextLeft();
        }

        private void btnNextRight_Click(object sender, RoutedEventArgs e)
        {
            SelectNextRight();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            Delete();
        }

        private void btnAddStop_Click(object sender, RoutedEventArgs e)
        {
            AddStop();
        }

        private void btnSwap_Click(object sender, RoutedEventArgs e)
        {
            Swap();
        }

#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable IDE0060 // Remove unused parameter
        private void nudOffset_ValueChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_selected == null) return;

            UpdateStopOffset(_selected, nudOffset.Value);
        }
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore IDE0051 // Remove unused private members

        #endregion

        private void btnColorChange_Click(object sender, RoutedEventArgs e)
        {
            if (_selected == null) return;
            ColorPickerDialog cpd = new ColorPickerDialog(ColorScheme, _selected.Color);
            cpd.ShowDialog();

            if (cpd.DialogResult)
            {
                _selected.Color = cpd.SelectedColor;
                _selected.GradientStop.Color = cpd.SelectedColor;

                RenderBar();
                RenderStops();
                GradientChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
