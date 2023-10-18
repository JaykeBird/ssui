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
            gsi.Click += gsi_Click;
            gsi.IsSelected = false;

            grdStops.Children.Add(gsi);

            return gsi;
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

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly DependencyProperty ColorSchemeProperty
            = DependencyProperty.Register("ColorScheme", typeof(ColorScheme), typeof(GradientBar),
            new FrameworkPropertyMetadata(new ColorScheme(), new PropertyChangedCallback(OnColorSchemeChanged)));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Perform an action when the ColorScheme property has changed. Primarily used internally.
        /// </summary>
        /// <param name="d">The object containing the property that changed.</param>
        /// <param name="e">Event arguments about the property change.</param>
        public static void OnColorSchemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
#if NETCOREAPP
            ColorScheme cs = (e.NewValue as ColorScheme)!;
#else
            ColorScheme cs = e.NewValue as ColorScheme;
#endif

            if (d is GradientBar s)
            {
                s.ColorSchemeChanged?.Invoke(d, e);
                s.ApplyColorScheme(cs);
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

        void Deselect()
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
        /// <remarks>Use <see cref="GradientStops"/> to get a list of the stops currently in this gradient bar</remarks>
        public void SelectStop(GradientStop gs)
        {
            if (_stops.Contains(gs))
            {
                foreach (GradientStopItem gsi in grdStops.Children)
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
        /// <remarks>Use <see cref="GradientStops"/> to get a list of the stops currently in this gradient bar</remarks>
        public void SelectStop(int index)
        {
            if (index >= _stops.Count || index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "The index is outside the number of stops in this editor");
            }

            GradientStop gradientStop = _stops[index];

            foreach (GradientStopItem gsi in grdStops.Children)
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
        /// Get the gradient stop that is currently selected, or returns <c>null</c> if no gradient stop is selected.
        /// </summary>
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
        }
        #endregion

        #region ShowControls

        /// <summary>
        /// Get or set if editing and navigation controls should be visible in the gradient bar. If not, then only the bar and stops are shown.
        /// </summary>
        public bool ShowControls { get => (bool)GetValue(ShowControlsProperty); set => SetValue(ShowControlsProperty, value); }

        /// <summary>The backing dependency property for <see cref="ShowControls"/>. See the related property for details.</summary>
        public static DependencyProperty ShowControlsProperty
            = DependencyProperty.Register("ShowControls", typeof(bool), typeof(GradientBar),
            new FrameworkPropertyMetadata(true));


        #endregion

        #region Editor

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
            GradientChanged?.DynamicInvoke(this, EventArgs.Empty);
        }

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
            }

            if (_selected == null)
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

        private void btnNextRight_Click(object sender, RoutedEventArgs e)
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
            }

            if (_selected == null)
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
                    // find the next stop to the left
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

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_stops.Count <= 1) return;

            if (_selected != null)
            {
                grdStops.Children.Remove(_selected);
                _stops.Remove(_selected.GradientStop);
                Deselect();
                CountStops();
                GradientChanged?.DynamicInvoke(this, EventArgs.Empty);
            }
        }

        private void btnAddStop_Click(object sender, RoutedEventArgs e)
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
            GradientChanged?.DynamicInvoke(this, EventArgs.Empty);
        }
        private void btnSwap_Click(object sender, RoutedEventArgs e)
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

        private void nudOffset_ValueChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_selected == null) return;

            UpdateStopOffset(_selected, nudOffset.Value);
        }

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
                GradientChanged?.DynamicInvoke(this, EventArgs.Empty);
            }
        }
    }
}
