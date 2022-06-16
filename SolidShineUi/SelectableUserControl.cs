using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace SolidShineUi
{

    /// <summary>
    /// The basic control that can be added into a SelectPanel. Extend this class to create your own UI elements to use with the SelectPanel.
    /// </summary>
    public class SelectableUserControl : System.Windows.Controls.UserControl //, IEquatable<SelectableUserControl>
    {
        /// <summary>
        /// Create a SelectableUserControl.
        /// </summary>
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
            //UniqueIdentifier = Guid.NewGuid();

            base.Background = Background;
        }

        //public Guid UniqueIdentifier { get; set; }

        #region Selection Handling

        bool canSel = true;

        /// <summary>
        /// Get or set if this control can be selected.
        /// </summary>
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

        /// <summary>
        /// Get or set if this control is currently selected.
        /// </summary>
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

        /// <summary>
        /// Get or set the brush to use for the background of this control while it is being clicked.
        /// </summary>
        [Category("Brushes")]
        public Brush ClickBrush
        {
            get => (Brush)GetValue(ClickBrushProperty);
            set => SetValue(ClickBrushProperty, value);
        }

        /// <summary>
        /// Get or set the brush to use for the background of this control.
        /// </summary>
        [Category("Brushes")]
        public new Brush Background
        {
            get => (Brush)GetValue(BackgroundProperty);
            set => SetValue(BackgroundProperty, value);
        }

        /// <summary>
        /// Get or set the brush to use for the background of this control while it is selected.
        /// </summary>
        [Category("Brushes")]
        public Brush SelectedBrush
        {
            get => (Brush)GetValue(SelectedBrushProperty);
            set => SetValue(SelectedBrushProperty, value);
        }

        /// <summary>
        /// Get or set the brush to use for the background of this contol while it is highlighted (i.e. has a mouse over it, or has keyboard focus).
        /// </summary>
        [Category("Brushes")]
        public Brush HighlightBrush
        {
            get => (Brush)GetValue(HighlightBrushProperty);
            set => SetValue(HighlightBrushProperty, value);
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
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
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        bool performingClick = false;
        bool highlighting = false;
        bool rightClick = false;

#if NETCOREAPP
        /// <summary>
        /// Raised if the IsSelected property is changed.
        /// </summary>
        public event EventHandler? SelectionChanged;
        /// <summary>
        /// Raised if the CanSelect property is changed.
        /// </summary>
        public event EventHandler? CanSelectChanged;
        /// <summary>
        /// Raised when the control is clicked.
        /// </summary>
        public event EventHandler? Click;
        /// <summary>
        /// Raised when the control is right-clicked.
        /// </summary>
        public event EventHandler? RightClick;
#else
        /// <summary>
        /// Raised if the IsSelected property is changed.
        /// </summary>
        public event EventHandler SelectionChanged;
        /// <summary>
        /// Raised if the CanSelect property is changed.
        /// </summary>
        public event EventHandler CanSelectChanged;
        /// <summary>
        /// Raised when the control is clicked.
        /// </summary>
        public event EventHandler Click;
        /// <summary>
        /// Raised when the control is right-clicked.
        /// </summary>
        public event EventHandler RightClick;
#endif

        /// <summary>
        /// Make sure the control's visuals match the set brush properties. Call this if the parent's ColorScheme was changed.
        /// </summary>
        private void UpdateBrushes()
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
            UpdateBrushes();
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

//#if NETCOREAPP
//        public bool Equals([AllowNull] SelectableUserControl other)
//#else
//        public bool Equals(SelectableUserControl other)
//#endif
//        {
//            if (other == null) return false;
//            else
//            {
//                return other.UniqueIdentifier == UniqueIdentifier;
//            }
//        }
    }
}
