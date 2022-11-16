﻿using System;
using System.Collections.Generic;
using System.Linq;
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

                //GradientStopCollection gsc = new GradientStopCollection();
                //foreach (GradientStop item in value)
                //{
                //    gsc.Add(new GradientStop(item.Color, item.Offset));
                //}

                //_stops = gsc;
                SortStops();
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
            // TODO: maintain selected status
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
            txtCount.Text = (index + 1) + " of " + _stops.Count;

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

        #region Editor
        
        /// <summary>
        /// Raised when the SelectedGradientStop is changed.
        /// </summary>
#if NETCOREAPP
        public event EventHandler? GradientChanged;
#else
        public event EventHandler GradientChanged;
#endif

        void UpdateStopOffset(GradientStop stop, double offset)
        {
            foreach (GradientStopItem item in grdStops.Children)
            {
                if (item.GradientStop == stop)
                {
                    item.Offset = offset;
                    item.GradientStop.Offset = offset;
                }
            }

            SortStops();
            RenderBar();
            RenderStops();
            GradientChanged?.DynamicInvoke(this, EventArgs.Empty);
        }

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

        private void btnNextLeft_Click(object sender, RoutedEventArgs e)
        {
            if (_selected == null)
            {
                if (_stops.Count > 1)
                {
                    foreach (GradientStopItem item in grdStops.Children)
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
                    foreach (GradientStopItem item in grdStops.Children)
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
            if (_selected == null)
            {
                if (_stops.Count > 1)
                {
                    foreach (GradientStopItem item in grdStops.Children)
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
                    foreach (GradientStopItem item in grdStops.Children)
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
            if (_stops.Count <= 2) return;

            if (_selected != null)
            {
                grdStops.Children.Remove(_selected);
                _stops.Remove(_selected.GradientStop);
                Deselect();
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
            SelectStop(gsi);
            GradientChanged?.DynamicInvoke(this, EventArgs.Empty);
        }
        private void btnSwap_Click(object sender, RoutedEventArgs e)
        {
            foreach (GradientStop item in _stops)
            {
                item.Offset = 1 - item.Offset;
            }

            SortStops();
            RenderBar();
            RenderStops();
        }

        private void nudOffset_ValueChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
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
