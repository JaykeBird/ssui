using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace SolidShineUi
{
    public class SelectableUserControl : System.Windows.Controls.UserControl
    {

        public SelectableUserControl()
        {
            MouseEnter += UserControl_MouseEnter;
            MouseLeave += UserControl_MouseLeave;
            TouchEnter += UserControl_TouchEnter;
            TouchLeave += UserControl_TouchLeave;
            StylusEnter += UserControl_StylusEnter;
            StylusLeave += UserControl_StylusLeave;

            GotKeyboardFocus += UserControl_GotKeyboardFocus;
            LostKeyboardFocus += UserControl_LostKeyboardFocus;

            MouseDown += UserControl_MouseDown;
            MouseUp += UserControl_MouseUp;
            TouchDown += UserControl_TouchDown;
            TouchUp += UserControl_TouchUp;
            StylusDown += UserControl_StylusDown;
            StylusUp += UserControl_StylusUp;

            KeyDown += UserControl_KeyDown;

            Focusable = true;
            IsTabStop = true;

            base.Background = Background;
        }

        #region Selection Handling

        bool canSel = true;
        public bool CanSelect
        {
            get
            {
                return canSel;
            }
            set
            {
                bool curVal = canSel;

                canSel = value;
                if (!value)
                {
                    sel = false;
                    base.Background = Background;
                }

                if (curVal != canSel)
                {
                    CanSelectChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        bool sel = false;
        public bool IsSelected
        {
            get
            {
                return sel;
            }
            set
            {
                if (CanSelect)
                {
                    bool curVal = sel;

                    sel = value;
                    if (sel)
                    {
                        base.Background = SelectedBrush;
                    }
                    else
                    {
                        base.Background = Background;
                    }

                    if (curVal != sel)
                    {
                        SelectionChanged?.Invoke(this, EventArgs.Empty);
                    }
                }
            }
        }

        static Color transparent = Color.FromArgb(1, 0, 0, 0);

        [Category("Brushes")]
        public Brush ClickBrush
        {
            get => (Brush)GetValue(ClickBrushProperty);
            set => SetValue(ClickBrushProperty, value);
        }

        [Category("Brushes")]
        public new Brush Background
        {
            get => (Brush)GetValue(BackgroundProperty);
            set => SetValue(BackgroundProperty, value);
        }

        [Category("Brushes")]
        public Brush SelectedBrush
        {
            get => (Brush)GetValue(SelectedBrushProperty);
            set => SetValue(SelectedBrushProperty, value);
        }

        [Category("Brushes")]
        public Brush HighlightBrush
        {
            get => (Brush)GetValue(HighlightBrushProperty);
            set => SetValue(HighlightBrushProperty, value);
        }

        public new static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register(
            "Background", typeof(Brush), typeof(SelectableUserControl),
            new PropertyMetadata(transparent.ToBrush()));

        public static readonly DependencyProperty ClickBrushProperty = DependencyProperty.Register(
            "ClickBrush", typeof(Brush), typeof(SelectableUserControl),
            new PropertyMetadata(Colors.Gainsboro.ToBrush()));

        public static readonly DependencyProperty SelectedBrushProperty = DependencyProperty.Register(
            "SelectedBrush", typeof(Brush), typeof(SelectableUserControl),
            new PropertyMetadata(Colors.WhiteSmoke.ToBrush()));

        public static readonly DependencyProperty HighlightBrushProperty = DependencyProperty.Register(
            "HighlightBrush", typeof(Brush), typeof(SelectableUserControl),
            new PropertyMetadata(Colors.LightGray.ToBrush()));

        bool performingClick = false;
        bool highlighting = false;
        bool rightClick = false;

#if NETCOREAPP
        public event EventHandler? SelectionChanged;
        public event EventHandler? CanSelectChanged;
        public event EventHandler? Click;
        public event EventHandler? RightClick;
#else
        public event EventHandler SelectionChanged;
        public event EventHandler CanSelectChanged;
        public event EventHandler Click;
        public event EventHandler RightClick;
#endif

        /// <summary>
        /// Make sure the control's visuals match the set brush properties. Call this if the parent's ColorScheme was changed.
        /// </summary>
        public void UpdateBrushes()
        {
            if (performingClick)
            {
                base.Background = ClickBrush;
            }
            else if (highlighting)
            {
                base.Background = HighlightBrush;
            }
            else if (IsSelected)
            {
                base.Background = SelectedBrush;
            }
            else
            {
                base.Background = Background;
            }
        }

        /// <summary>
        /// When overridden by a derived class, this method is automatically called each time the color scheme is updated by the parent SelectPanel. Use this to update child controls.
        /// </summary>
        /// <param name="cs">The new color scheme.</param>
        public virtual void ApplyColorScheme(ColorScheme cs)
        {
            if (IsEnabled)
            {
                Foreground = cs.ForegroundColor.ToBrush();
            }
            else
            {
                Foreground = cs.DarkDisabledColor.ToBrush();
            }
        }

        #region User Inputs

        void Highlight()
        {
            highlighting = true;
            base.Background = HighlightBrush;
        }

        void Unhighlight()
        {
            highlighting = false;

            if (IsSelected)
            {
                base.Background = SelectedBrush;
            }
            else
            {
                base.Background = Background;
            }
        }

        void InitiateClick()
        {
            performingClick = true;

            base.Background = ClickBrush;
        }

        void PerformClick()
        {
            if (performingClick)
            {
                if (rightClick)
                {
                    RightClick?.Invoke(this, EventArgs.Empty);
                    rightClick = false;
                }
                else
                {
                    IsSelected = true;
                    Click?.Invoke(this, EventArgs.Empty);
                }

                performingClick = false;
            }
        }

        #region Base Event Handlers

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            InitiateClick();
        }

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
            {
                rightClick = true;
            }

            PerformClick();
        }

#if NETCOREAPP
        private void UserControl_TouchDown(object? sender, TouchEventArgs e)
        {
            InitiateClick();
        }

        private void UserControl_TouchUp(object? sender, TouchEventArgs e)
        {
            PerformClick();
        }

#else
        private void UserControl_TouchDown(object sender, TouchEventArgs e)
        {
            InitiateClick();
        }

        private void UserControl_TouchUp(object sender, TouchEventArgs e)
        {
            PerformClick();
        }
#endif

        private void UserControl_StylusDown(object sender, StylusDownEventArgs e)
        {
            InitiateClick();
        }

        private void UserControl_StylusUp(object sender, StylusEventArgs e)
        {
            PerformClick();
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            Highlight();
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            Unhighlight();
        }

#if NETCOREAPP
        private void UserControl_TouchEnter(object? sender, TouchEventArgs e)
        {
            Highlight();
        }

        private void UserControl_TouchLeave(object? sender, TouchEventArgs e)
        {
            Unhighlight();
        }

#else
        private void UserControl_TouchEnter(object sender, TouchEventArgs e)
        {
            Highlight();
        }

        private void UserControl_TouchLeave(object sender, TouchEventArgs e)
        {
            Unhighlight();
        }
#endif

        private void UserControl_StylusEnter(object sender, StylusEventArgs e)
        {
            Highlight();
        }

        private void UserControl_StylusLeave(object sender, StylusEventArgs e)
        {
            Unhighlight();
        }

        private void UserControl_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            Highlight();
        }

        private void UserControl_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            Unhighlight();
        }

        #endregion

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Space)
            {
                performingClick = true;
                PerformClick();
            }
            else if (e.Key == Key.Apps)
            {
                performingClick = true;
                rightClick = true;
                PerformClick();
            }
        }

        #endregion
        #endregion
    }
}
