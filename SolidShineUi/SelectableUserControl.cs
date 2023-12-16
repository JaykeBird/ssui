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
    public class SelectableUserControl : System.Windows.Controls.UserControl, IClickSelectableControl
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

            base.Background = Background;
        }

        #region Brushes
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

        #endregion

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
        public static DependencyProperty SelectOnClickProperty
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
        public static DependencyProperty CanSelectProperty
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

                        ItemSelectionChangedEventArgs re = new ItemSelectionChangedEventArgs(IsSelectedChangedEvent, true, false, SelectionChangeTrigger.DisableSelecting, s);
                        s.RaiseEvent(re);
                    }
                    s.ChangeBaseBackground(s.Background);
                }

                s.CanSelectChanged?.Invoke(s, e);
            });
        }

        private void ChangeBaseBackground(Brush newValue)
        {
            base.Background = newValue;
        }

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
                    base.Background = SelectedBrush;
                }
                else
                {
                    base.Background = Background;
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
                base.Background = HighlightBrush;
            }
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
