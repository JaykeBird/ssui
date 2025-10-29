using SolidShineUi.Utils;
using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace SolidShineUi
{

    /// <summary>
    /// The basic control that can be added into a <see cref="SelectPanel"/>. Extend this class to create your own UI elements to use with the SelectPanel.
    /// </summary>
    [Localizability(LocalizationCategory.ListBox)]
    [DefaultEvent(nameof(IsSelectedChanged))]
    public class SelectableUserControl : ThemedUserControl, IClickSelectableControl
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

            Background = BaseBackground;
            Foreground = BaseForeground;

            // after certain changes, we'll update the brushes
            IsEnabledChanged += (d, e) => { UpdateBrushes(); };
            SsuiThemeApplied += (d, e) => { UpdateBrushes(); };

            // Background = transparent.ToBrush();
        }

        #region Brushes

        static Color transparent = Color.FromArgb(1, 0, 0, 0);

        /// <summary>
        /// Get or set the brush to use for the background of this control when in its default state (e.g., when not selected or highlighted).
        /// </summary>
        /// <remarks>
        /// Setting <c>Background</c> will only affect its background for the current state and time; once the state changes,
        /// the background will be overwritten with either this brush or one of the relevant other brushes.
        /// Instead, this brush should be set to control what the background should be when falling back to a default, base state.
        /// </remarks>
        [Category("Brushes")]
        public Brush BaseBackground
        {
            get => (Brush)GetValue(BaseBackgroundProperty);
            set => SetValue(BaseBackgroundProperty, value);
        }

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


        /// <summary>
        /// Get or set the brush to use for the foreground of this control when in its default state (e.g., when not selected or highlighted).
        /// </summary>
        /// <remarks>
        /// Setting <c>Foreground</c> will only affect its foreground for the current state and time; once the state changes,
        /// the foreground will be overwritten with either this brush or one of the relevant other brushes.
        /// Instead, this brush should be set to control what the foreground should be when falling back to a default, base state.
        /// </remarks>
        [Category("Brushes")]
        public Brush BaseForeground
        {
            get => (Brush)GetValue(BaseForegroundProperty);
            set => SetValue(BaseForegroundProperty, value);
        }

        /// <summary>
        /// Get or set the brush to use for the background of this control while it is being clicked.
        /// </summary>
        [Category("Brushes")]
        public Brush DisabledForeground
        {
            get => (Brush)GetValue(DisabledForegroundProperty);
            set => SetValue(DisabledForegroundProperty, value);
        }

        /// <summary>
        /// Get or set the brush to use for the background of this control while it is selected.
        /// </summary>
        [Category("Brushes")]
        public Brush SelectedForeground
        {
            get => (Brush)GetValue(SelectedForegroundProperty);
            set => SetValue(SelectedForegroundProperty, value);
        }

        /// <summary>
        /// Get or set the brush to use for the background of this contol while it is highlighted (i.e. has a mouse over it, or has keyboard focus).
        /// </summary>
        [Category("Brushes")]
        public Brush HighlightForeground
        {
            get => (Brush)GetValue(HighlightForegroundProperty);
            set => SetValue(HighlightForegroundProperty, value);
        }

        /// <summary>The backing dependency property for <see cref="BaseBackground"/>. See the related property for details.</summary>
        public static readonly DependencyProperty BaseBackgroundProperty = DependencyProperty.Register(
            nameof(BaseBackground), typeof(Brush), typeof(SelectableUserControl),
            new PropertyMetadata(transparent.ToBrush()));

        /// <summary>The backing dependency property for <see cref="ClickBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ClickBrushProperty = DependencyProperty.Register(
            nameof(ClickBrush), typeof(Brush), typeof(SelectableUserControl),
            new PropertyMetadata(Colors.Gainsboro.ToBrush()));

        /// <summary>The backing dependency property for <see cref="SelectedBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty SelectedBrushProperty = DependencyProperty.Register(
            nameof(SelectedBrush), typeof(Brush), typeof(SelectableUserControl),
            new PropertyMetadata(Colors.WhiteSmoke.ToBrush()));

        /// <summary>The backing dependency property for <see cref="HighlightBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty HighlightBrushProperty = DependencyProperty.Register(
            nameof(HighlightBrush), typeof(Brush), typeof(SelectableUserControl),
            new PropertyMetadata(Colors.LightGray.ToBrush()));

        /// <summary>The backing dependency property for <see cref="BaseForeground"/>. See the related property for details.</summary>
        public static readonly DependencyProperty BaseForegroundProperty = DependencyProperty.Register(
            nameof(BaseForeground), typeof(Brush), typeof(SelectableUserControl),
            new PropertyMetadata(Colors.Black.ToBrush()));

        /// <summary>The backing dependency property for <see cref="DisabledForeground"/>. See the related property for details.</summary>
        public static readonly DependencyProperty DisabledForegroundProperty = DependencyProperty.Register(
            nameof(DisabledForeground), typeof(Brush), typeof(SelectableUserControl),
            new PropertyMetadata(Colors.Black.ToBrush()));

        /// <summary>The backing dependency property for <see cref="SelectedForeground"/>. See the related property for details.</summary>
        public static readonly DependencyProperty SelectedForegroundProperty = DependencyProperty.Register(
            nameof(SelectedForeground), typeof(Brush), typeof(SelectableUserControl),
            new PropertyMetadata(Colors.Black.ToBrush()));

        /// <summary>The backing dependency property for <see cref="HighlightForeground"/>. See the related property for details.</summary>
        public static readonly DependencyProperty HighlightForegroundProperty = DependencyProperty.Register(
            nameof(HighlightForeground), typeof(Brush), typeof(SelectableUserControl),
            new PropertyMetadata(Colors.Black.ToBrush()));


        /// <summary>
        /// Make sure the control's visuals match the set brush properties. Call this if the parent's ColorScheme was changed.
        /// </summary>
        private void UpdateBrushes()
        {
            if (performingClick)
            {
                Background = ClickBrush;
            }
            else if (highlighting)
            {
                Background = HighlightBrush;
            }
            else if (IsSelected)
            {
                Background = SelectedBrush;
            }
            else
            {
                Background = BaseBackground;
            }

            if (!IsEnabled)
            {
                Foreground = DisabledForeground;
            }
            else if (highlighting || performingClick)
            {
                Foreground = HighlightForeground;
            }
            else if (IsSelected)
            {
                Foreground = SelectedForeground;
            }
            else
            {
                Foreground = BaseForeground;
            }
        }

        #endregion

        /// <summary>
        /// When overridden by a derived class, this method is automatically called each time the color scheme is updated by the parent SelectPanel. 
        /// Use this to update child controls.
        /// </summary>
        /// <param name="cs">The new color scheme.</param>
        public virtual void ApplyColorScheme(ColorScheme cs)
        {
            BaseForeground = cs.ForegroundColor.ToBrush();
            DisabledForeground = cs.DarkDisabledColor.ToBrush();

            UpdateBrushes();
        }

        /// <inheritdoc/>
        protected override void OnApplySsuiTheme(SsuiTheme ssuiTheme, bool useLightBorder = false, bool useAccentTheme = false)
        {
            base.OnApplySsuiTheme(ssuiTheme, useLightBorder, useAccentTheme);

            if (useAccentTheme && ssuiTheme is SsuiAppTheme ssuiAppTheme)
            {
                ApplyTheme(ssuiAppTheme.AccentTheme);
            }
            else
            {
                ApplyTheme(ssuiTheme);
            }

            BaseBackground = transparent.ToBrush();

            void ApplyTheme(SsuiTheme theme)
            {
                // Border brush already applied in base
                // BaseBackground is set to transparent above

                ApplyThemeBinding(HighlightBrushProperty, SsuiTheme.CheckBrushProperty, theme);
                ApplyThemeBinding(SelectedBrushProperty, SsuiTheme.SelectedBackgroundBrushProperty, theme);
                ApplyThemeBinding(ClickBrushProperty, SsuiTheme.ClickBrushProperty, theme);

                ApplyThemeBinding(BaseForegroundProperty, SsuiTheme.ForegroundProperty, theme);
                ApplyThemeBinding(HighlightForegroundProperty, SsuiTheme.HighlightForegroundProperty, theme);
                ApplyThemeBinding(SelectedForegroundProperty, SsuiTheme.ForegroundProperty, theme);
                ApplyThemeBinding(DisabledForegroundProperty, SsuiTheme.DisabledForegroundProperty, theme);
            }
        }

        #region Click/Selection Handling

        #region Selection Properties
        /// <summary>
        /// Get or set if this control should change its <see cref="IsSelected"/> value when you click on the control.
        /// </summary>
        /// <remarks>
        /// This allows more fine-tuned control over when and how this control can be selected. If this is <c>false</c>, then the user can only use the checkbox to directly 
        /// select or deselect this control. You can use <see cref="CanSelect"/> to globally disable selecting this control via any method.
        /// </remarks>
        public bool SelectOnClick { get => (bool)GetValue(SelectOnClickProperty); set => SetValue(SelectOnClickProperty, value); }

        /// <summary>The backing dependency property for <see cref="SelectOnClick"/>. See the related property for details.</summary>
        public static readonly DependencyProperty SelectOnClickProperty
            = DependencyProperty.Register("SelectOnClick", typeof(bool), typeof(SelectableUserControl),
            new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Get or set if this control can be selected.
        /// </summary>
        /// <remarks>
        /// If this is set to <c>false</c>, then this control cannot be selected via any method - even programmatically. Setting this to <c>false</c> will also deselect this control, 
        /// if currently selected. For more fine-tuned control, you can use <see cref="SelectOnClick"/> to limit how the user can select this control, 
        /// while still being able to change the selection status via <see cref="IsSelected"/>.
        /// </remarks>
        public bool CanSelect { get => (bool)GetValue(CanSelectProperty); set => SetValue(CanSelectProperty, value); }

        /// <summary>The backing dependency property for <see cref="CanSelect"/>. See the related property for details.</summary>
        public static readonly DependencyProperty CanSelectProperty
            = DependencyProperty.Register("CanSelect", typeof(bool), typeof(SelectableUserControl),
            new FrameworkPropertyMetadata(true, OnCanSelectChanged));

        private static void OnCanSelectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.PerformAs<SelectableUserControl, bool>(e.NewValue, (s, v) =>
            {
                if (v == false)
                {
                    if (s.IsSelected)
                    {
                        s.sel = false;

                        ItemSelectionChangedEventArgs re = 
                            new ItemSelectionChangedEventArgs(IsSelectedChangedEvent, true, false, SelectionChangeTrigger.DisableSelecting, s);
                        s.RaiseEvent(re);
                    }
                    s.Background = s.BaseBackground;
                    s.Foreground = s.BaseForeground;
                    //s.ChangeBaseBackground(s.Background);
                }

                s.CanSelectChanged?.Invoke(s, e);
            });
        }

        //private void ChangeBaseBackground(Brush newValue)
        //{
        //    base.Background = newValue;
        //}

        bool sel = false;

        /// <summary>
        /// Get or set if this control is currently selected.
        /// </summary>
        /// <remarks>
        /// If <see cref="CanSelect"/> is set to <c>false</c>, then this value will not be changed (silent fail).
        /// </remarks>
        public bool IsSelected
        {
            get
            {
                return sel;
            }
            set
            {
                SetIsSelectedWithSource(value, SelectionChangeTrigger.CodeUnknown);
            }
        }
        #endregion

        #region SetIsSelectedWithSource
        /// <summary>
        /// Set the <see cref="IsSelected"/> value of this control, while also defining how the selection was changed.
        /// </summary>
        /// <param name="value">The value to set <see cref="IsSelected"/> to.</param>
        /// <param name="triggerMethod">The source or method used to trigger the change in selection.</param>
        /// <param name="triggerSource">The object that triggered the change.</param>
#if NETCOREAPP
        public void SetIsSelectedWithSource(bool value, SelectionChangeTrigger triggerMethod, object? triggerSource = null)
#else
        public void SetIsSelectedWithSource(bool value, SelectionChangeTrigger triggerMethod, object triggerSource = null)
#endif
        {
            if (CanSelect)
            {
                bool curVal = sel;

                sel = value;
                if (sel)
                {
                    Background = SelectedBrush;
                    Foreground = SelectedForeground;
                }
                else
                {
                    Background = BaseBackground;
                    Foreground = BaseForeground;
                }

                if (curVal != sel)
                {
                    ItemSelectionChangedEventArgs e = new ItemSelectionChangedEventArgs(IsSelectedChangedEvent, curVal, sel, triggerMethod, triggerSource);
                    RaiseEvent(e);
                }
            }
        }
        #endregion

        #region Events

#if NETCOREAPP
        /// <summary>
        /// Raised if the CanSelect property is changed.
        /// </summary>
        public event DependencyPropertyChangedEventHandler? CanSelectChanged;
#else
        /// <summary>
        /// Raised if the CanSelect property is changed.
        /// </summary>
        public event DependencyPropertyChangedEventHandler CanSelectChanged;
#endif

        /// <summary>
        /// The backing value for the <see cref="IsSelectedChanged"/> event. See the related event for more details.
        /// </summary>
        public static readonly RoutedEvent IsSelectedChangedEvent = EventManager.RegisterRoutedEvent(
            "IsSelectedChanged", RoutingStrategy.Bubble, typeof(ItemSelectionChangedEventHandler), typeof(SelectableUserControl));

        /// <summary>
        /// Raised when the user clicks on the main button (not the menu button), via a mouse click or via the keyboard.
        /// </summary>
        public event ItemSelectionChangedEventHandler IsSelectedChanged
        {
            add { AddHandler(IsSelectedChangedEvent, value); }
            remove { RemoveHandler(IsSelectedChangedEvent, value); }
        }

        /// <summary>
        /// The backing value for the <see cref="Click"/> event. See the related event for more details.
        /// </summary>
        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent(
            "Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SelectableUserControl));

        /// <summary>
        /// Raised when the user clicks on the main button (not the menu button), via a mouse click or via the keyboard.
        /// </summary>
        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }


        /// <summary>
        /// The backing value for the <see cref="RightClick"/> event. See the related event for more details.
        /// </summary>
        public static readonly RoutedEvent RightClickEvent = EventManager.RegisterRoutedEvent(
            "RightClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SelectableUserControl));

        /// <summary>
        /// Raised when the user right-clicks on the button, via a mouse click or via the keyboard.
        /// </summary>
        public event RoutedEventHandler RightClick
        {
            add { AddHandler(RightClickEvent, value); }
            remove { RemoveHandler(RightClickEvent, value); }
        }

        #endregion

        #region Base Functions

        bool performingClick = false;
        bool highlighting = false;
        bool rightClick = false;

        void Highlight()
        {
            highlighting = true;
            if (CanSelect && SelectOnClick)
            {
                Background = HighlightBrush;
                Foreground = HighlightForeground;
            }
        }

        void Unhighlight()
        {
            highlighting = false;

            if (IsSelected)
            {
                Background = SelectedBrush;
                Foreground = SelectedForeground;
            }
            else
            {
                Background = BaseBackground;
                Foreground = BaseForeground;
            }
        }

        void InitiateClick()
        {
            performingClick = true;

            Background = ClickBrush;
            Foreground = HighlightForeground;
        }

        void PerformRightClick()
        {
            RoutedEventArgs rre = new RoutedEventArgs(RightClickEvent);
            RaiseEvent(rre);
        }

        /// <summary>
        /// Perform a click on this control programmatically. This responds the same way as if it was clicked by the user.
        /// </summary>
        public void DoClick()
        {
            OnClick();
        }

        /// <summary>
        /// Defines the actions the button performs when it is clicked.
        /// </summary>
        protected void OnClick()
        {
            if (SelectOnClick)
            {
                SetIsSelectedWithSource(true, SelectionChangeTrigger.ControlClick, this);
            }

            RoutedEventArgs rre = new RoutedEventArgs(ClickEvent);
            RaiseEvent(rre);
        }

        void PerformClick()
        {
            if (performingClick)
            {
                if (rightClick)
                {
                    PerformRightClick();
                    rightClick = false;
                }
                else
                {
                    OnClick();
                }

                performingClick = false;
            }
        }

        #endregion

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
