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
    [Localizability(LocalizationCategory.ListBox)]
    [DefaultEvent(nameof(IsSelectedChanged))]
    public class SelectableUserControl : System.Windows.Controls.UserControl, IClickSelectableControl //, IEquatable<SelectableUserControl>
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

        #region Selection Handling

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
                SetIsSelectedWithSource(value, SelectionChangeTrigger.CodeUnknown);
            }
        }

        /// <summary>
        /// Set the <see cref="IsSelected"/> value of this control, while also defining how the selection was changed.
        /// </summary>
        /// <param name="value">The value to set <see cref="IsSelected"/> to.</param>
        /// <param name="trigger">The source or method used to trigger the change in selection.</param>
        /// <param name="triggerSource">The object that triggered the change.</param>
#if NETCOREAPP
        public void SetIsSelectedWithSource(bool value, SelectionChangeTrigger trigger, object? triggerSource = null)
#else
        public void SetIsSelectedWithSource(bool value, SelectionChangeTrigger trigger, object triggerSource = null)
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
                    IsSelectedChanged?.Invoke(this, new ItemSelectionChangedEventArgs(curVal, sel, trigger, triggerSource));
                    SelectionChanged?.Invoke(this, new ItemSelectionChangedEventArgs(curVal, sel, trigger, triggerSource));
                }
            }
        }

        bool performingClick = false;
        bool highlighting = false;
        bool rightClick = false;

#if NETCOREAPP
        /// <summary>
        /// Raised if the IsSelected property is changed.
        /// </summary>
        public event SolidShineUi.ItemSelectionChangedEventHandler? IsSelectedChanged;        
        /// <summary>
        /// Raised if the IsSelected property is changed.
        /// </summary>
        /// <remarks>
        /// This event will be renamed/replaced with <see cref="IsSelectedChanged"/> in version 2.0, so please adjust your code to use that event now.
        /// </remarks>
        [Obsolete("This event is being replaced with " + nameof(IsSelectedChanged) + " in future versions. Please transition to using that event.")]
        public event ItemSelectionChangedEventHandler? SelectionChanged;
        /// <summary>
        /// Raised if the CanSelect property is changed.
        /// </summary>
        public event EventHandler? CanSelectChanged;
#else
        /// <summary>
        /// Raised if the IsSelected property is changed.
        /// </summary>
        public event SolidShineUi.ItemSelectionChangedEventHandler IsSelectedChanged;
        /// <summary>
        /// Raised if the IsSelected property is changed.
        /// </summary>
        /// <remarks>
        /// This event will be renamed/replaced with <see cref="IsSelectedChanged"/> in version 2.0, so please adjust your code to use that event now.
        /// </remarks>
        [Obsolete("This event is being replaced with " + nameof(IsSelectedChanged) + " in future versions. Please transition to using that event.")]
        public event ItemSelectionChangedEventHandler SelectionChanged;
        /// <summary>
        /// Raised if the CanSelect property is changed.
        /// </summary>
        public event EventHandler CanSelectChanged;
#endif

        /// <summary>
        /// Represents a handler for the <see cref="IsSelectedChanged"/> event.
        /// </summary>
        /// <param name="sender">The source object of the event.</param>
        /// <param name="e">The event arguments, containing information on the new IsSelected value and how the selection changed.</param>
        public delegate void ItemSelectionChangedEventHandler(object sender, ItemSelectionChangedEventArgs e);

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

        #region Routed Events

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

        #endregion

    }

    /// <summary>
    /// The event arguments for the IsSelectedChanged event of the SelectableUserControl.
    /// </summary>
    public class ItemSelectionChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Create a new ItemSelectionChangedEventArgs.
        /// </summary>
        /// <param name="oldValue">The old IsSelected value.</param>
        /// <param name="newValue">The new IsSelected value.</param>
        /// <param name="trigger">The trigger method that caused the value to be updated.</param>
        /// <param name="triggerSource">The source object that updated the value (if available).</param>
#if NETCOREAPP
        public ItemSelectionChangedEventArgs(bool oldValue, bool newValue, SelectionChangeTrigger trigger, object? triggerSource = null)
#else
        public ItemSelectionChangedEventArgs(bool oldValue, bool newValue, SelectionChangeTrigger trigger, object triggerSource = null)
#endif
        {
            OldValue = oldValue;
            NewValue = newValue;
            TriggerMethod = trigger;
            TriggerSource = triggerSource;
        }

        /// <summary>
        /// Create a new ItemSelectionChangedEventArgs, with a related DependencyProperty.
        /// </summary>
        /// <param name="oldValue">The old IsSelected value.</param>
        /// <param name="newValue">The new IsSelected value.</param>
        /// <param name="trigger">The trigger method that caused the value to be updated.</param>
        /// <param name="triggerSource">The source object that updated the value (if available).</param>
        /// <param name="sourceProperty">The dependency property related to this event change (if available).</param>
#if NETCOREAPP
        public ItemSelectionChangedEventArgs(bool oldValue, bool newValue, DependencyProperty? sourceProperty, SelectionChangeTrigger trigger, object? triggerSource = null)
#else
        public ItemSelectionChangedEventArgs(bool oldValue, bool newValue, DependencyProperty sourceProperty, SelectionChangeTrigger trigger, object triggerSource = null)
#endif
        {
            OldValue = oldValue;
            NewValue = newValue;
            TriggerMethod = trigger;
            TriggerSource = triggerSource;
            Property = sourceProperty;
        }

        /// <summary>
        /// The old value of the IsSelected property.
        /// </summary>
        public bool OldValue { get; private set; }

        /// <summary>
        /// The new value of the IsSelected property.
        /// </summary>
        public bool NewValue { get; private set; }

        /// <summary>
        /// The method that was used to update the value.
        /// </summary>
        public SelectionChangeTrigger TriggerMethod { get; private set; }

        /// <summary>
        /// The dependency property related to this event, if applicable.
        /// </summary>
#if NETCOREAPP
        public DependencyProperty? Property { get; private set; } = null;
#else
        public DependencyProperty Property { get; private set; } = null;
#endif

        /// <summary>
        /// The object that caused the update to occur, if available.
        /// </summary>
#if NETCOREAPP
        public object? TriggerSource { get; private set; }
#else
        public object TriggerSource { get; private set; }
#endif
    }
}
