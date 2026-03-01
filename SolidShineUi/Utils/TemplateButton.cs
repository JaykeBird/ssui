using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace SolidShineUi.Utils
{
    /// <summary>
    /// A button similar to <see cref="TemplateButton"/>, for usage in control templates.
    /// </summary>
    public class TemplateButton : ButtonBase, ISsuiButton
    {
        static TemplateButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TemplateButton), new FrameworkPropertyMetadata(typeof(TemplateButton)));
        }

        /// <summary>
        /// Create a new TemplateButton.
        /// </summary>
        public TemplateButton()
        {
            MouseDown += UserControl_MouseDown;
            MouseUp += UserControl_MouseUp;
            MouseLeave += UserControl_MouseLeave;

            PreviewMouseDown += UserControl_PreviewMouseDown;
            PreviewMouseUp += UserControl_PreviewMouseUp;

            LostFocus += UserControl_LostFocus;
            LostKeyboardFocus += UserControl_LostKeyboardFocus;

            KeyDown += UserControl_KeyDown;
            KeyUp += UserControl_KeyUp;

            //KeyboardNavigation.SetIsTabStop(this, true);

            SetupTimer();
            Click += TemplateButton_Click;
        }

        #region Brushes

        /// <summary>
        /// Get or set the brush used for the background of the control while the mouse is clicking it.
        /// </summary>
        [Category("Brushes")]
        public Brush ClickBrush
        {
            get => (Brush)GetValue(ClickBrushProperty);
            set => SetValue(ClickBrushProperty, value);
        }

        /// <summary>
        /// Get or set the brush used for the background of this button while it is selected
        /// (i.e. the <c>IsSelected</c> property is true).
        /// </summary>
        [Category("Brushes")]
        public Brush SelectedBrush
        {
            get => (Brush)GetValue(SelectedBrushProperty);
            set => SetValue(SelectedBrushProperty, value);
        }

        /// <summary>
        /// Get or set the brush used for the background of the control while the mouse is over it, or it has keyboard focus.
        /// </summary>
        [Category("Brushes")]
        public Brush HighlightBrush
        {
            get => (Brush)GetValue(HighlightBrushProperty);
            set => SetValue(HighlightBrushProperty, value);
        }

        /// <summary>
        /// Get or set the brush used for the foreground while the control has the mouse over it, or it has keyboard focus.
        /// </summary>
        [Category("Brushes")]
        public Brush ForegroundHighlightBrush
        {
            get => (Brush)GetValue(BorderHighlightBrushProperty);
            set => SetValue(BorderHighlightBrushProperty, value);
        }

        /// <summary>
        /// Get or set the brush used for the background of the control when the control is disabled.
        /// </summary>
        [Category("Brushes")]
        public Brush DisabledBrush
        {
            get => (Brush)GetValue(DisabledBrushProperty);
            set => SetValue(DisabledBrushProperty, value);
        }

        /// <summary>
        /// Get or set the brush used for the border of the control when the control is disabled.
        /// </summary>
        [Category("Brushes")]
        public Brush BorderDisabledBrush
        {
            get => (Brush)GetValue(BorderDisabledBrushProperty);
            set => SetValue(BorderDisabledBrushProperty, value);
        }

        /// <summary>
        /// Get or set the brush used for the border while the control has the mouse over it, or it has keyboard focus.
        /// </summary>
        [Category("Brushes")]
        public Brush BorderHighlightBrush
        {
            get => (Brush)GetValue(BorderHighlightBrushProperty);
            set => SetValue(BorderHighlightBrushProperty, value);
        }

        /// <summary>
        /// Get or set the brush used for the border while the control is selected
        /// (i.e. the <c>IsSelected</c> property is true).
        /// </summary>
        [Category("Brushes")]
        public Brush BorderSelectedBrush
        {
            get => (Brush)GetValue(BorderSelectedBrushProperty);
            set => SetValue(BorderSelectedBrushProperty, value);
        }

        /// <summary>The backing dependency property for <see cref="ClickBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ClickBrushProperty = FlatButton.ClickBrushProperty.AddOwner(typeof(TemplateButton));

        /// <summary>The backing dependency property for <see cref="SelectedBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty SelectedBrushProperty = FlatButton.SelectedBrushProperty.AddOwner(typeof(TemplateButton));

        /// <summary>The backing dependency property for <see cref="HighlightBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty HighlightBrushProperty = FlatButton.HighlightBrushProperty.AddOwner(typeof(TemplateButton));

        /// <summary>The backing dependency property for <see cref="ForegroundHighlightBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ForegroundHighlightBrushProperty = FlatButton.ForegroundHighlightBrushProperty.AddOwner(typeof(TemplateButton));

        /// <summary>The backing dependency property for <see cref="DisabledBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty DisabledBrushProperty = FlatButton.DisabledBrushProperty.AddOwner(typeof(TemplateButton));

        /// <summary>The backing dependency property for <see cref="BorderDisabledBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty BorderDisabledBrushProperty = FlatButton.BorderDisabledBrushProperty.AddOwner(typeof(TemplateButton));

        /// <summary>The backing dependency property for <see cref="BorderHighlightBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty BorderHighlightBrushProperty = FlatButton.BorderHighlightBrushProperty.AddOwner(typeof(TemplateButton));

        /// <summary>The backing dependency property for <see cref="BorderSelectedBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty BorderSelectedBrushProperty = FlatButton.BorderSelectedBrushProperty.AddOwner(typeof(TemplateButton));

        #endregion

        /// <summary>
        /// Apply a color scheme to this control, and set some other optional appearance settings. The color scheme can quickly apply a whole visual style to the control.
        /// </summary>
        /// <param name="cs">The color scheme to apply</param>
        public void ApplyColorScheme(ColorScheme cs)
        {
            if (cs == null)
            {
                return;
            }

            if (cs.IsHighContrast)
            {
                Background = cs.BackgroundColor.ToBrush();
                HighlightBrush = cs.HighlightColor.ToBrush();
                SelectedBrush = cs.HighlightColor.ToBrush();
                BorderHighlightBrush = cs.BorderColor.ToBrush();
                BorderSelectedBrush = cs.BorderColor.ToBrush();
                BorderDisabledBrush = cs.DarkDisabledColor.ToBrush();
                DisabledBrush = cs.BackgroundColor.ToBrush();
                Foreground = cs.ForegroundColor.ToBrush();
                ClickBrush = cs.ThirdHighlightColor.ToBrush();
            }
            else
            {
                Background = cs.SecondaryColor.ToBrush();
                BorderBrush = cs.BorderColor.ToBrush();
                HighlightBrush = cs.SecondHighlightColor.ToBrush();
                DisabledBrush = cs.LightDisabledColor.ToBrush();
                BorderDisabledBrush = cs.DarkDisabledColor.ToBrush();
                SelectedBrush = cs.ThirdHighlightColor.ToBrush();
                BorderHighlightBrush = cs.HighlightColor.ToBrush();
                BorderSelectedBrush = cs.SelectionColor.ToBrush();
                Foreground = cs.ForegroundColor.ToBrush();
                ClickBrush = cs.ThirdHighlightColor.ToBrush();
            }
        }


#if NETCOREAPP
        SsuiTheme? IClickSelectableControl.SsuiTheme
#else
        SsuiTheme IClickSelectableControl.SsuiTheme
#endif
        {
            get => new SsuiTheme();
            set
            {
                if (value == null) return;

                Background = value.ButtonBackground;
                HighlightBrush = value.HighlightBrush;
                DisabledBrush = value.DisabledBackground;
                BorderDisabledBrush = value.DisabledBorderBrush;
                SelectedBrush = value.SelectedBackgroundBrush;
                BorderHighlightBrush = value.HighlightBorderBrush;
                BorderSelectedBrush = value.SelectedBorderBrush;
                Foreground = value.Foreground;
                ForegroundHighlightBrush = value.HighlightForeground;
                ClickBrush = value.ClickBrush;
                BorderBrush = value.BorderBrush;

                CornerRadius = value.CornerRadius;
            }
        }

        #region TransparentBack

        /// <summary>
        /// The backing dependency property for <see cref="TransparentBack"/>. See the related property for details.
        /// </summary>
        public static readonly DependencyProperty TransparentBackProperty = FlatButton.TransparentBackProperty.AddOwner(typeof(TemplateButton));

        /// <summary>
        /// Get or set whether the button should have a transparent background when the button is not focused or selected.
        /// </summary>
        [Category("Common")]
        [Description("Get or set whether the button should have a transparent background when the button is not focused or selected.")]
        public bool TransparentBack
        {
            get => (bool)GetValue(TransparentBackProperty);
            set => SetValue(TransparentBackProperty, value);
        }

        #endregion

        #region Border

        /// <summary>The backing dependency property for <see cref="BorderSelectionThickness"/>. See the related property for details.</summary>
        public static readonly DependencyProperty BorderSelectionThicknessProperty = FlatButton.BorderSelectionThicknessProperty.AddOwner(typeof(TemplateButton));

        /// <summary>The backing dependency property for <see cref="CornerRadius"/>. See the related property for details.</summary>
        public static readonly DependencyProperty CornerRadiusProperty = FlatButton.CornerRadiusProperty.AddOwner(typeof(TemplateButton));

        /// <summary>
        /// Get or set the thickness of the border around the button, while the button is in a selected (<c>IsSelected</c>) state.
        /// </summary>
        [Category("Appearance")]
        [Description("Get or set the thickness of the border around the button, while the button is in a selected state.")]
        public Thickness BorderSelectionThickness
        {
            get => (Thickness)GetValue(BorderSelectionThicknessProperty);
            set => SetValue(BorderSelectionThicknessProperty, value);
        }

        /// <summary>
        /// Get or set the corner radius (or radii) to use for the button and its border. Can be used to create a rounded button.
        /// </summary>
        [Category("Appearance")]
        [Description("Get or set the corner radius (or radii) to use for the button and its border. Can be used to create a rounded button.")]
        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        #endregion

        #region HighlightOnKeyboardFocus

        /// <summary>
        /// Get or set if the button should be highlighted (using the <see cref="HighlightBrush"/> and <see cref="BorderHighlightBrush"/>)
        /// when it has keyboard focus. If <c>false</c>, only the keyboard focus outline appears, and highlighting only occurs on mouse/stylus over.
        /// </summary>
        [Category("Appearance")]
        [Description("Get or set if the button should be highlighted when it has keyboard focus.")]
        public bool HighlightOnKeyboardFocus { get => (bool)GetValue(HighlightOnKeyboardFocusProperty); set => SetValue(HighlightOnKeyboardFocusProperty, value); }

        /// <summary>The backing dependency property for <see cref="HighlightOnKeyboardFocus"/>. See the related property for details.</summary>
        public static readonly DependencyProperty HighlightOnKeyboardFocusProperty = FlatButton.HighlightOnKeyboardFocusProperty.AddOwner(typeof(TemplateButton));

        #endregion

        #region Click / Selection Handling

        #region Routed Events

        /// <summary>
        /// The backing value for the <see cref="RightClick"/> event. See the related event for more details.
        /// </summary>
        public static readonly RoutedEvent RightClickEvent = EventManager.RegisterRoutedEvent(
            nameof(RightClick), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TemplateButton));

        /// <summary>
        /// Raised when the user right-clicks on the button, via a mouse click or via the keyboard.
        /// </summary>
        public event RoutedEventHandler RightClick
        {
            add { AddHandler(RightClickEvent, value); }
            remove { RemoveHandler(RightClickEvent, value); }
        }

        #endregion

        #region IsMouseDown

        // from https://stackoverflow.com/questions/10667545/why-ismouseover-is-recognized-and-mousedown-isnt-wpf-style-trigger

        /// <summary>
        /// The internal dependency property for <see cref="IsMouseDown"/>. See that property for more details.
        /// </summary>
        protected static readonly DependencyPropertyKey IsMouseDownPropertyKey = DependencyProperty.RegisterAttachedReadOnly(nameof(IsMouseDown),
            typeof(bool), typeof(TemplateButton), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Get if there is a mouse button currently being pressed, while the mouse cursor is over this control.
        /// </summary>
        public static readonly DependencyProperty IsMouseDownProperty = IsMouseDownPropertyKey.DependencyProperty;

        /// <summary>
        /// Set the IsMouseDown property for a TemplateButton.
        /// </summary>
        /// <param name="obj">The TemplateButton to apply the property change to.</param>
        /// <param name="value">The new value to set for the property.</param>
        protected static void SetIsMouseDown(DependencyObject obj, bool value)
        {
            obj.SetValue(IsMouseDownPropertyKey, value);
        }

        /// <summary>
        /// Get the IsMouseDown property for a TemplateButton.
        /// </summary>
        /// <param name="obj">The TemplateButton to get the property value from.</param>
        public static bool GetIsMouseDown(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsMouseDownProperty);
        }

        /// <summary>
        /// Get if a mouse button is currently being pressed while the cursor is over this TemplateButton.
        /// </summary>
        [ReadOnly(true)]
        public bool IsMouseDown
        {
            get => (bool)GetValue(IsMouseDownProperty);
            protected set => SetValue(IsMouseDownPropertyKey, value);
        }

        #endregion

        #region Variables/Properties

        bool initiatingClick = false;

        #region IsSelected / IsSelectedChanged

        bool _runSelChangeEvent = true;

        /// <summary>
        /// The backing dependency property for <see cref="IsSelected"/>. See the related property for details.
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty = FlatButton.IsSelectedProperty.AddOwner(typeof(TemplateButton),
            new FrameworkPropertyMetadata(OnIsSelectedChanged));

        /// <summary>
        /// Perform an action when a property of an object has changed. Primarily used internally.
        /// </summary>
        /// <param name="d">The object containing the property that changed.</param>
        /// <param name="e">Event arguments about the property change.</param>
        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is bool se)
            {
                bool old = (e.OldValue is bool oval) ? oval : false;

                if (d is TemplateButton f)
                {
                    if (f._runSelChangeEvent)
                    {
                        ItemSelectionChangedEventArgs re = new ItemSelectionChangedEventArgs(IsSelectedChangedEvent, old, se, IsSelectedProperty, SelectionChangeTrigger.CodeUnknown, null);
                        f.RaiseEvent(re);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets whether this button is selected. This property (combined with <see cref="SelectOnClick"/>) allows the button to function like a ToggleButton.
        /// </summary>
        /// <remarks>
        /// A selected button will have a slightly different visual appearance to differentiate it as being selected. This will include, by default, the border being a bit thicker.
        /// This can be changed via the <see cref="BorderSelectionThickness"/> property. You can also directly edit the brushes used via the <see cref="SelectedBrush"/> and
        /// <see cref="BorderSelectedBrush"/> properties.
        /// <para />
        /// To listen to changes to this property, use <see cref="IsSelectedChanged"/>, rather than listening to the <c>Click</c> event, as other actions could change this 
        /// property rather than just clicking it.
        /// </remarks>
        [Category("Common")]
        [Description("Gets or sets whether this button is selected.")]
        public bool IsSelected
        {
            get
            {
                return (bool)GetValue(IsSelectedProperty);
            }
            set
            {
                SetValue(IsSelectedProperty, value);
            }
        }

        /// <summary>
        /// The backing value for the <see cref="IsSelectedChanged"/> event. See the related event for more details.
        /// </summary>
        public static readonly RoutedEvent IsSelectedChangedEvent = EventManager.RegisterRoutedEvent(
            nameof(IsSelectedChanged), RoutingStrategy.Bubble, typeof(ItemSelectionChangedEventHandler), typeof(TemplateButton));

        /// <summary>
        /// Raised when the user clicks on the main button (not the menu button), via a mouse click or via the keyboard.
        /// </summary>
        public event ItemSelectionChangedEventHandler IsSelectedChanged
        {
            add { AddHandler(IsSelectedChangedEvent, value); }
            remove { RemoveHandler(IsSelectedChangedEvent, value); }
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
            bool old = IsSelected;

            _runSelChangeEvent = false;
            IsSelected = value;
            _runSelChangeEvent = true;

            ItemSelectionChangedEventArgs re = new ItemSelectionChangedEventArgs(IsSelectedChangedEvent, old, value, IsSelectedProperty, trigger, triggerSource);
            RaiseEvent(re);
        }
        #endregion

        /// <summary>
        /// The backing dependency property object for the <see cref="SelectOnClick"/> property. See the related property for more details.
        /// </summary>
        public static readonly DependencyProperty SelectOnClickProperty = FlatButton.SelectOnClickProperty.AddOwner(typeof(TemplateButton));

        /// <summary>
        /// Gets or sets whether the button should change its IsSelected property when a click is performed. With this enabled, this allows the button to take on the functionality of a ToggleButton.
        /// </summary>
        /// <remarks>
        /// While SelectOnClick is true, the button will toggle between <see cref="IsSelected"/> being true and false (similar to a ToggleButton).<para/>
        /// The button's Click event will still be raised while this property is set to <c>true</c>, but the event occurs after the
        /// IsSelected property has already changed. Do not use Click event to check when the button's IsSelected property is changed, but instead the IsSelectedChanged event,
        /// in case of situations where IsSelected is changed via methods other than clicking, such as programmatically or via WPF binding.
        /// </remarks>
        [Category("Common")]
        [Description("Gets or sets whether the button should change its IsSelected property when a click is performed. With this enabled, this allows the button to take on the functionality of a ToggleButton.")]
        public bool SelectOnClick
        {
            get => (bool)GetValue(SelectOnClickProperty);
            set => SetValue(SelectOnClickProperty, value);
        }

        #endregion

        #region Base Click Functions

        // Sets up the button to be clicked. This must be run before PerformClick.
        void PressRightClick()
        {
            initiatingClick = true;
        }

        // If the button is prepared by PerformPress, perform the Click actions, including raising the Click event.
        void PerformRightClick()
        {
            if (initiatingClick)
            {
                RoutedEventArgs rre = new RoutedEventArgs(RightClickEvent);
                RaiseEvent(rre);
            }
        }

        /// <summary>
        /// Perform a click programmatically. The button responds the same way as if it was clicked by the user.
        /// </summary>
        public void DoClick()
        {
            OnClick();
        }

        /// <summary>
        /// Defines the actions the button performs when it is clicked.
        /// </summary>
        protected override void OnClick()
        {
            if (SelectOnClick)
            {
                SetIsSelectedWithSource(!IsSelected, SelectionChangeTrigger.ControlClick, this);
            }

            base.OnClick();
        }

        #endregion

        #region Event handlers

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //if (e.ChangedButton == MouseButton.Right)
            //{
            //    PressRightClick();
            //}
        }

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //if (e.ChangedButton == MouseButton.Right)
            //{
            //    PerformRightClick();
            //}
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            //if (ClickMode == ClickMode.Press && e.Key == Key.Apps)
            //{
            //    PressRightClick();
            //    PerformRightClick();
            //}
        }

        private void UserControl_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Apps)
            {
                PressRightClick();
                PerformRightClick();
            }
        }

        private void UserControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
            {
                PressRightClick();
            }
            SetIsMouseDown(this, true);
        }

        private void UserControl_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
            {
                PerformRightClick();
            }
            SetIsMouseDown(this, false);
        }


        private void UserControl_LostFocus(object sender, RoutedEventArgs e)
        {
            initiatingClick = false;
        }

        private void UserControl_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            initiatingClick = false;
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            SetIsMouseDown(this, false);
            initiatingClick = false;
        }


        #endregion

        #endregion

        /// <summary>
        /// Get or set if the execute timer should be set up for this button, to provide RepeatButton-like functionality.
        /// </summary>
        public bool UseExecuteTimer { get => (bool)GetValue(UseExecuteTimerProperty); set => SetValue(UseExecuteTimerProperty, value); }

        /// <summary>The backing dependency property for <see cref="UseExecuteTimer"/>. See the related property for details.</summary>
        public static readonly DependencyProperty UseExecuteTimerProperty
            = DependencyProperty.Register(nameof(UseExecuteTimer), typeof(bool), typeof(TemplateButton),
            new FrameworkPropertyMetadata(false));


        #region Events / Command Properties

        #region Press Begins

        ///// <summary>
        ///// The backing value for the <see cref="PressBegins"/> event. See the related event for more details.
        ///// </summary>
        //public static readonly RoutedEvent PressBeginsEvent = EventManager.RegisterRoutedEvent(
        //    nameof(PressBegins), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TemplateButton));

        ///// <summary>
        ///// Raised when the user starts clicking/pressing this button.
        ///// </summary>
        ///// <remarks>
        ///// This will execute whenever <see cref="System.Windows.Controls.Primitives.ButtonBase.IsPressed"/> is changed to <c>true</c>. This occurs when the user clicks down on
        ///// the button, or presses the Space or Enter key on the button, or presses on the button via touch, pen, or other pointer.
        ///// </remarks>
        //public event RoutedEventHandler PressBegins
        //{
        //    add { AddHandler(PressBeginsEvent, value); }
        //    remove { RemoveHandler(PressBeginsEvent, value); }
        //}

        ///// <summary>
        ///// Get or set the command to execute when this button begins being pressed.
        ///// </summary>
        ///// <remarks>
        ///// This will execute whenever <see cref="System.Windows.Controls.Primitives.ButtonBase.IsPressed"/> is changed to <c>true</c>. This occurs when the user clicks down on
        ///// the button, or presses the Space or Enter key on the button, or presses on the button via touch, pen, or other pointer.
        ///// </remarks>
        //[Category("Common")]
        //[Description("Get or set the command to execute when this button begins being pressed.")]
        //public ICommand PressBeginsCommand { get => (ICommand)GetValue(PressBeginsCommandProperty); set => SetValue(PressBeginsCommandProperty, value); }

        ///// <summary>The backing dependency property for <see cref="PressBeginsCommand"/>. See the related property for details.</summary>
        //public static readonly DependencyProperty PressBeginsCommandProperty
        //    = DependencyProperty.Register(nameof(PressBeginsCommand), typeof(ICommand), typeof(TemplateButton),
        //    new FrameworkPropertyMetadata(null));

        ///// <summary>
        ///// Get or set the parameter to pass with <see cref="PressBeginsCommand"/> when it is executed. Default value is <c>null</c>.
        ///// </summary>
        //[Category("Common")]
        //[Description("Get or set the parameter to pass with PressBeginsCommand when it is executed.")]
        //public object PressBeginsCommandParameter { get => GetValue(PressBeginsCommandParameterProperty); set => SetValue(PressBeginsCommandParameterProperty, value); }

        ///// <summary>The backing dependency property for <see cref="PressBeginsCommandParameter"/>. See the related property for details.</summary>
        //public static readonly DependencyProperty PressBeginsCommandParameterProperty
        //    = DependencyProperty.Register(nameof(PressBeginsCommandParameter), typeof(object), typeof(TemplateButton),
        //    new FrameworkPropertyMetadata(null));

        ///// <summary>
        ///// Get or set the target element that will receive <see cref="PressBeginsCommand"/> when it is executed. Default value is <c>null</c>.
        ///// </summary>
        ///// <remarks>
        ///// WPF's <see cref="RoutedCommand"/> supports indicating a command target, but other <see cref="ICommand"/> implementations may not. In those cases, this property
        ///// will not do anything.
        ///// </remarks>
        //[Description("Get or set the target element that will receive PressBeginsCommand when it is executed.")]
        //public IInputElement PressBeginsCommandTarget { get => (IInputElement)GetValue(PressBeginsCommandTargetProperty); set => SetValue(PressBeginsCommandTargetProperty, value); }

        ///// <summary>The backing dependency property for <see cref="PressBeginsCommandTarget"/>. See the related property for details.</summary>
        //public static readonly DependencyProperty PressBeginsCommandTargetProperty
        //    = DependencyProperty.Register(nameof(PressBeginsCommandTarget), typeof(IInputElement), typeof(TemplateButton),
        //    new FrameworkPropertyMetadata(null));

        #endregion

        #region Press Ends

        ///// <summary>
        ///// The backing value for the <see cref="PressEnds"/> event. See the related event for more details.
        ///// </summary>
        //public static readonly RoutedEvent PressEndsEvent = EventManager.RegisterRoutedEvent(
        //    nameof(PressEnds), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TemplateButton));

        ///// <summary>
        ///// Raised when the user stops clicking/pressing this button.
        ///// </summary>
        ///// <remarks>
        ///// This will execute whenever <see cref="System.Windows.Controls.Primitives.ButtonBase.IsPressed"/> is changed to <c>false</c>. This occurs when the user releases a
        ///// mouse button while over this button, or releases the Space or Enter key, or moves the touch, pen, or other pointer away from the button. This also occurs if the user
        ///// moves the mouse cursor away from the button while still holding down a mouse button (and bringing the mouse cursor back to trigger the press begins actions again).
        ///// </remarks>
        //public event RoutedEventHandler PressEnds
        //{
        //    add { AddHandler(PressEndsEvent, value); }
        //    remove { RemoveHandler(PressEndsEvent, value); }
        //}

        ///// <summary>
        ///// Get or set the command to execute when this button is no longer being pressed.
        ///// </summary>
        ///// <remarks>
        ///// This will execute whenever <see cref="System.Windows.Controls.Primitives.ButtonBase.IsPressed"/> is changed to <c>false</c>. This occurs when the user releases a
        ///// mouse button while over this button, or releases the Space or Enter key, or moves the touch, pen, or other pointer away from the button. This also occurs if the user
        ///// moves the mouse cursor away from the button while still holding down a mouse button (and bringing the mouse cursor back to trigger the press begins actions again).
        ///// </remarks>
        //[Category("Common")]
        //[Description("Get or set the command to execute when this button is no longer being pressed.")]
        //public ICommand PressEndsCommand { get => (ICommand)GetValue(PressEndsCommandProperty); set => SetValue(PressEndsCommandProperty, value); }

        ///// <summary>The backing dependency property for <see cref="PressEndsCommand"/>. See the related property for details.</summary>
        //public static readonly DependencyProperty PressEndsCommandProperty
        //    = DependencyProperty.Register(nameof(PressEndsCommand), typeof(ICommand), typeof(TemplateButton),
        //    new FrameworkPropertyMetadata(null));

        ///// <summary>
        ///// Get or set the parameter to pass with <see cref="PressEndsCommand"/> when it is executed. Default value is <c>null</c>.
        ///// </summary>
        //[Category("Common")]
        //[Description("Get or set the parameter to pass with PressEndsCommand when it is executed.")]
        //public object PressEndsCommandParameter { get => GetValue(PressEndsCommandParameterProperty); set => SetValue(PressEndsCommandParameterProperty, value); }

        ///// <summary>The backing dependency property for <see cref="PressEndsCommandParameter"/>. See the related property for details.</summary>
        //public static readonly DependencyProperty PressEndsCommandParameterProperty
        //    = DependencyProperty.Register(nameof(PressEndsCommandParameter), typeof(object), typeof(TemplateButton),
        //    new FrameworkPropertyMetadata(null));

        ///// <summary>
        ///// Get or set the target element that will receive <see cref="PressEndsCommand"/> when it is executed. Default value is <c>null</c>.
        ///// </summary>
        ///// <remarks>
        ///// WPF's <see cref="RoutedCommand"/> supports indicating a command target, but other <see cref="ICommand"/> implementations may not. In those cases, this property
        ///// will not do anything.
        ///// </remarks>
        //[Description("Get or set the target element that will receive PressEndsCommand when it is executed.")]
        //public IInputElement PressEndsCommandTarget { get => (IInputElement)GetValue(PressEndsCommandTargetProperty); set => SetValue(PressEndsCommandTargetProperty, value); }

        ///// <summary>The backing dependency property for <see cref="PressEndsCommandTarget"/>. See the related property for details.</summary>
        //public static readonly DependencyProperty PressEndsCommandTargetProperty
        //    = DependencyProperty.Register(nameof(PressEndsCommandTarget), typeof(IInputElement), typeof(TemplateButton),
        //    new FrameworkPropertyMetadata(null));

        #endregion

        #region Execute (repeatedly)

        /// <summary>
        /// The backing value for the <see cref="Execute"/> event. See the related event for more details.
        /// </summary>
        public static readonly RoutedEvent ExecuteEvent = EventManager.RegisterRoutedEvent(
            nameof(Execute), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TemplateButton));

        /// <summary>
        /// Raised while this button is being pressed. This continues to be raised repeatedly until the button is no longer pressed.
        /// </summary>
        /// <remarks>
        /// This will begin whenever <see cref="System.Windows.Controls.Primitives.ButtonBase.IsPressed"/> is changed to <c>true</c>, and will stop when it changes back to <c>false</c>. 
        /// This includes when the user clicks and holds a mouse button over this button, pressing down on the Space or Enter key, or pressing on the butotn with touch, a pen, or other
        /// pointer.
        /// <para/>
        /// You can use <see cref="Delay"/> to set up how long the initial delay is until this command begins being executed, and <see cref="Interval"/> to set how soon each 
        /// successive execution should occur after the previous one.
        /// You can also use <see cref="ExecuteOnFirstClick"/> to disable this raising when the button is just clicked, without holding it down beyond <see cref="Delay"/>.
        /// </remarks>
        public event RoutedEventHandler Execute
        {
            add { AddHandler(ExecuteEvent, value); }
            remove { RemoveHandler(ExecuteEvent, value); }
        }

        /// <summary>
        /// Get or set the command to execute while this button is being pressed. This continues to be executed repeatedly until the button is no longer pressed.
        /// </summary>
        /// <remarks>
        /// This will begin whenever <see cref="System.Windows.Controls.Primitives.ButtonBase.IsPressed"/> is changed to <c>true</c>, and will stop when it changes back to <c>false</c>. 
        /// This includes when the user clicks and holds a mouse button over this button, pressing down on the Space or Enter key, or pressing on the butotn with touch, a pen, or other
        /// pointer.
        /// <para/>
        /// You can use <see cref="Delay"/> to set up how long the initial delay is until this command begins being executed, and <see cref="Interval"/> to set how soon each 
        /// successive execution should occur after the previous one.
        /// You can also use <see cref="ExecuteOnFirstClick"/> to disable this raising when the button is just clicked, without holding it down beyond <see cref="Delay"/>.
        /// </remarks>
        [Category("Common")]
        [Description("Get or set the command to execute while this button is being pressed. This continues to be executed repeatedly until the button is no longer pressed.")]
        public ICommand ExecuteCommand { get => (ICommand)GetValue(ExecuteCommandProperty); set => SetValue(ExecuteCommandProperty, value); }

        /// <summary>The backing dependency property for <see cref="ExecuteCommand"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ExecuteCommandProperty = FlatRepeatButton.ExecuteCommandProperty.AddOwner(typeof(TemplateButton));

        /// <summary>
        /// Get or set the parameter to pass with <see cref="ExecuteCommand"/> when it is executed. Default value is <c>null</c>.
        /// </summary>
        [Category("Common")]
        [Description("Get or set the parameter to pass with ExecuteCommand when it is executed.")]
        public object ExecuteCommandParameter { get => GetValue(ExecuteCommandParameterProperty); set => SetValue(ExecuteCommandParameterProperty, value); }

        /// <summary>The backing dependency property for <see cref="ExecuteCommandParameter"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ExecuteCommandParameterProperty = FlatRepeatButton.ExecuteCommandParameterProperty.AddOwner(typeof(TemplateButton));


        /// <summary>
        /// Get or set the target element that will receive <see cref="ExecuteCommand"/> when it is executed. Default value is <c>null</c>.
        /// </summary>
        /// <remarks>
        /// WPF's <see cref="RoutedCommand"/> supports indicating a command target, but other <see cref="ICommand"/> implementations may not. In those cases, this property
        /// will not do anything.
        /// </remarks>
        /// 
        [Description("Get or set the target element that will receive ExecuteCommand when it is executed.")]
        public IInputElement ExecuteCommandTarget { get => (IInputElement)GetValue(ExecuteCommandTargetProperty); set => SetValue(ExecuteCommandTargetProperty, value); }

        /// <summary>The backing dependency property for <see cref="ExecuteCommandTarget"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ExecuteCommandTargetProperty = FlatRepeatButton.ExecuteCommandTargetProperty.AddOwner(typeof(TemplateButton));

        #endregion

        #endregion

        #region Timer / ExecuteOnFirstClick

        #region Setup / Logic

        void SetupTimer()
        {
            ExecuteTimer.Tick += ExecuteTimer_Elapsed;
            ResetTimer();
        }

        void ResetTimer()
        {
            ExecuteTimer.Stop();
            ExecuteTimer.Interval = new TimeSpan(0, 0, 0, 0, Delay);
            //executeTimer.AutoReset = false;
        }

#if NETCOREAPP
        private void ExecuteTimer_Elapsed(object? sender, EventArgs e)
#else
        private void ExecuteTimer_Elapsed(object sender, EventArgs e)
#endif
        {
            if (firstRun)
            {
                ExecuteTimer.IsEnabled = false;
                ExecuteTimer.Interval = new TimeSpan(0, 0, 0, 0, Interval);
                ExecuteTimer.Start();

                firstRun = false;
                timerRan = true;

                ExecuteTimer.Start();
            }

            DoExecute();
        }

        /// <summary>
        /// The timer to set how long to wait before calling the Execute event.
        /// </summary>
        protected DispatcherTimer ExecuteTimer { get; } = new DispatcherTimer();

        bool firstRun = false;
        bool timerRan = false;

        #endregion

        #region Properties

        /// <summary>
        /// Get or set how long of a pause there should be between <see cref="Execute"/> firing, while the button is being pressed. Measured in milliseconds.
        /// </summary>
        [Category("Common")]
        [Description("Get or set how long of a pause there should be between Execute firing, while the button is being pressed. Measured in milliseconds.")]
        public int Interval { get => (int)GetValue(IntervalProperty); set => SetValue(IntervalProperty, value); }

        /// <summary>The backing dependency property for <see cref="Interval"/>. See the related property for details.</summary>
        public static readonly DependencyProperty IntervalProperty = FlatRepeatButton.IntervalProperty.AddOwner(typeof(TemplateButton));

        /// <summary>
        /// Get or set how long the delay should be after the button is initially pressed, before starting to raise <see cref="Execute"/>. Measured in milliseconds.
        /// </summary>
        [Category("Common")]
        [Description("Get or set how long the delay should be after the button is initially pressed, before starting to raise Execute. Measured in milliseconds.")]
        public int Delay { get => (int)GetValue(DelayProperty); set => SetValue(DelayProperty, value); }

        /// <summary>The backing dependency property for <see cref="Delay"/>. See the related property for details.</summary>
        public static readonly DependencyProperty DelayProperty = FlatRepeatButton.DelayProperty.AddOwner(typeof(TemplateButton));

        /// <summary>
        /// Get or set if the Execute event should be activated when the button is initially clicked, even if the <see cref="Delay"/> time hasn't been reached.
        /// </summary>
        /// <remarks>
        /// When this is <c>true</c>, this guarantees that Execute is ran at least once when button is clicked, regardless of if the button was pressed for long enough
        /// to wait past <see cref="Delay"/> and trigger running the Execute event repeatedly. This is the default and expected behaviour with a RepeatButton,
        /// but if you need to have more fine control over when Execute runs, this can be set to false.
        /// </remarks>
        [Category("Common")]
        [Description("Get or set if the Execute event should be activated when the button is initially clicked, even if the Delay time hasn't been reached.")]
        public bool ExecuteOnFirstClick { get => (bool)GetValue(ExecuteOnFirstClickProperty); set => SetValue(ExecuteOnFirstClickProperty, value); }

        /// <summary>The backing dependency property for <see cref="ExecuteOnFirstClick"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ExecuteOnFirstClickProperty = FlatRepeatButton.ExecuteOnFirstClickProperty.AddOwner(typeof(TemplateButton));

        #endregion

        #endregion

        #region Pressed Handling / Core Logic

        /// <inheritdoc/>
        protected override void OnIsPressedChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnIsPressedChanged(e);

            if (IsPressed)
            {
                // started pressing
                PressBegan();
            }
            else
            {
                // stopped pressing (or cursor/pointer left the button)
                PressEnded();
            }
        }

        void PressBegan()
        {
            //if (PressBeginsCommand != null)
            //{
            //    ActivateCommand(PressBeginsCommand, PressBeginsCommandParameter, PressBeginsCommandTarget);
            //}
            //RoutedEventArgs re = new RoutedEventArgs(PressBeginsEvent, this);
            //RaiseEvent(re);

            firstRun = true;
            if (UseExecuteTimer) ExecuteTimer.Start();
        }

        void PressEnded()
        {
            //if (PressEndsCommand != null)
            //{
            //    ActivateCommand(PressEndsCommand, PressEndsCommandParameter, PressEndsCommandTarget);
            //}
            //RoutedEventArgs re = new RoutedEventArgs(PressEndsEvent, this);
            //RaiseEvent(re);

            ResetTimer();
        }

        private void TemplateButton_Click(object sender, RoutedEventArgs e)
        {
            if (ExecuteOnFirstClick && !timerRan)
            {
                DoExecute();
            }

            timerRan = false;
        }

        /// <summary>
        /// Raise the <see cref="Execute"/> event, and activate the <see cref="ExecuteCommand"/>.
        /// </summary>
        public void DoExecute()
        {
            if (ExecuteCommand != null)
            {
                ActivateCommand(ExecuteCommand, ExecuteCommandParameter, ExecuteCommandTarget);
            }
            RoutedEventArgs re = new RoutedEventArgs(ExecuteEvent, this);
            RaiseEvent(re);
        }

        /// <summary>
        /// Execute a particular command, with an optional parameter and target.
        /// </summary>
        /// <param name="command">The command to execute</param>
        /// <param name="parameter">The parameter to pass with the command</param>
        /// <param name="target">The target element that will be receiving this command</param>
#if NETCOREAPP
        static void ActivateCommand(ICommand command, object? parameter, IInputElement? target)
#else
        static void ActivateCommand(ICommand command, object parameter, IInputElement target)
#endif
        {
            if (command is RoutedCommand rc)
            {
                if (rc.CanExecute(parameter, target))
                {
                    rc.Execute(parameter, target);
                }
            }
            else
            {
                if (command.CanExecute(parameter))
                {
                    command.Execute(parameter);
                }
            }
        }

        #endregion
    }
}
