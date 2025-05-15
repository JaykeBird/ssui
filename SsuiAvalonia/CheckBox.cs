using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.VisualTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidShineUi
{
    // Usage of a CheckBox is good to indicate selection (such as, checking specific items from a list of checkboxes to select those items).
    // The IsChecked value is just a bool, so this can be useful for situations where you need to present an option that's stored as a bool.

    /// <summary>
    /// A control that can display content alongside a large, touch-friendly checkbox, which users are able to check or uncheck. Provides more customization than the standard WPF CheckBox.
    /// </summary>
    /// <remarks>
    /// Unlike the standard WPF CheckBox, a nullable bool (<c>bool?</c>) is not used for <see cref="IsChecked"/>; instead, <c>IsChecked</c> will return true if there is any mark,
    /// including either a full checkmark or an indeterminate mark (a square), and false only if there is no mark at all.
    /// Instead, use <see cref="IsIndeterminate"/> to check if the mark is an indeterminate mark, or use <see cref="CheckState"/> to get the current state as an enum.
    /// </remarks>
    public class CheckBox : ContentControl
    {

        /// <summary>
        /// Create a new CheckBox.
        /// </summary>
        public CheckBox()
        {

        }

        /// <inheritdoc/>
        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            switch (change.Property.Name)
            {
                case nameof(IsChecked):
                    OnCheckedChange(change);
                    break;
                case nameof(IsIndeterminate):
                    OnIndeterminateChange(change);
                    break;
                case nameof(CheckState):
                    OnCheckStateChange(change);
                    break;
                case nameof(ColorScheme):
                    OnColorSchemeChange(change);
                    break;
            }
        }

        #region CheckState

        bool _updatingValues = false;

        /// <summary>
        /// Get or set if the check box is checked. (Note: if in the Indeterminate state, it will still return true as checked.)
        /// </summary>
        public bool IsChecked { get => GetValue(IsCheckedProperty); set => SetValue(IsCheckedProperty, value); }

        /// <summary>The backing styled property for <see cref="IsChecked"/>. See the related property for details.</summary>
        public static readonly StyledProperty<bool> IsCheckedProperty
            = AvaloniaProperty.Register<CheckBox, bool>(nameof(IsChecked), false);

        /// <summary>
        /// Get or set if the check box is in the Indeterminate state. 
        /// </summary>
        public bool IsIndeterminate { get => GetValue(IsIndeterminateProperty); set => SetValue(IsIndeterminateProperty, value); }

        /// <summary>The backing styled property for <see cref="IsIndeterminate"/>. See the related property for details.</summary>
        public static readonly StyledProperty<bool> IsIndeterminateProperty
            = AvaloniaProperty.Register<CheckBox, bool>(nameof(IsIndeterminate), false);

        /// <summary>
        /// Get or set the state of the checkbox, via a CheckState enum. Can be set via this property or via the IsChecked and IsIndeterminate properties.
        /// </summary>
        public CheckState CheckState { get => GetValue(CheckStateProperty); set => SetValue(CheckStateProperty, value); }

        /// <summary>The backing styled property for <see cref="CheckState"/>. See the related property for details.</summary>
        public static readonly StyledProperty<CheckState> CheckStateProperty
            = AvaloniaProperty.Register<CheckBox, CheckState>(nameof(CheckState), CheckState.Unchecked);

        /// <summary>
        /// Raised when the CheckState property is changed, either to Checked, Indeterminate, or Unchecked.
        /// </summary>
        public event EventHandler<RoutedEventArgs> CheckChanged
        {
            add { AddHandler(CheckChangedEvent, value); }
            remove { RemoveHandler(CheckChangedEvent, value); }
        }

        /// <summary>
        /// The backing routed event for <see cref="CheckChanged"/>. See the related event for more info.
        /// </summary>
        public RoutedEvent<RoutedEventArgs> CheckChangedEvent = RoutedEvent.Register<RoutedEventArgs>
            (nameof(CheckChanged), RoutingStrategies.Bubble, typeof(CheckBox));

        /// <summary>
        /// Raised when the CheckState property is changed to Checked.
        /// </summary>
        public event EventHandler<RoutedEventArgs> Checked
        {
            add { AddHandler(CheckedEvent, value); }
            remove { RemoveHandler(CheckedEvent, value); }
        }

        /// <summary>
        /// The backing routed event for <see cref="Checked"/>. See the related event for more info.
        /// </summary>
        public RoutedEvent<RoutedEventArgs> CheckedEvent = RoutedEvent.Register<RoutedEventArgs>
            (nameof(Checked), RoutingStrategies.Bubble, typeof(CheckBox));

        /// <summary>
        /// Raised when the CheckState property is changed to Unchecked.
        /// </summary>
        public event EventHandler<RoutedEventArgs> Unchecked
        {
            add { AddHandler(UncheckedEvent, value); }
            remove { RemoveHandler(UncheckedEvent, value); }
        }

        /// <summary>
        /// The backing routed event for <see cref="Unchecked"/>. See the related event for more info.
        /// </summary>
        public RoutedEvent<RoutedEventArgs> UncheckedEvent = RoutedEvent.Register<RoutedEventArgs>
            (nameof(Unchecked), RoutingStrategies.Bubble, typeof(CheckBox));

        /// <summary>
        /// Raised when the CheckState property is changed to Indeterminate.
        /// </summary>
        public event EventHandler<RoutedEventArgs> Indeterminate
        {
            add { AddHandler(IndeterminateEvent, value); }
            remove { RemoveHandler(IndeterminateEvent, value); }
        }

        /// <summary>
        /// The backing routed event for <see cref="Indeterminate"/>. See the related event for more info.
        /// </summary>
        public RoutedEvent<RoutedEventArgs> IndeterminateEvent = RoutedEvent.Register<RoutedEventArgs>
            (nameof(Indeterminate), RoutingStrategies.Bubble, typeof(CheckBox));

        void OnCheckedChange(AvaloniaPropertyChangedEventArgs e)
        {
            // IsChecked: bool
            if (!_updatingValues)
            {
                _updatingValues = true;

                bool value = e.GetNewValue<bool>();
                if (value == false)
                {
                    IsIndeterminate = false;
                    CheckState = CheckState.Unchecked;
                }
                else
                {
                    if (IsIndeterminate)
                    {
                        CheckState = CheckState.Indeterminate;
                    }
                    else
                    {
                        CheckState = CheckState.Checked;
                    }
                }

                _updatingValues = false;
                FinishCheckValueChange();
            }
        }

        void OnIndeterminateChange(AvaloniaPropertyChangedEventArgs e)
        {
            // IsIndeterminate: bool
            if (!_updatingValues)
            {
                _updatingValues = true;

                bool value = e.GetNewValue<bool>();
                if (value == true)
                {
                    IsChecked = true;
                    CheckState = CheckState.Indeterminate;
                }
                else
                {
                    if (IsChecked)
                    {
                        CheckState = CheckState.Checked;
                    }
                    else
                    {
                        CheckState = CheckState.Unchecked;
                    }
                }

                _updatingValues = false;
                FinishCheckValueChange();
            }
        }

        void OnCheckStateChange(AvaloniaPropertyChangedEventArgs e)
        {
            // CheckState: CheckState
            if (!_updatingValues)
            {
                _updatingValues = true;

                CheckState value = e.GetNewValue<CheckState>();

                switch (value)
                {
                    case CheckState.Unchecked:
                        IsChecked = false;
                        IsIndeterminate = false;
                        break;
                    case CheckState.Checked:
                        IsChecked = true;
                        IsIndeterminate = false;
                        break;
                    case CheckState.Indeterminate:
                        IsChecked = true;
                        IsIndeterminate = true;
                        break;
                    default:
                        IsChecked = false;
                        IsIndeterminate = false;
                        break;
                }

                _updatingValues = false;
                FinishCheckValueChange();
            }
        }

        /// <summary>
        /// Raise needed change events, and also apply pseudo-classes.
        /// </summary>
        void FinishCheckValueChange()
        {
            RoutedEventArgs re = new RoutedEventArgs(CheckChangedEvent, this);
            RaiseEvent(re);

            switch (CheckState)
            {
                case CheckState.Unchecked:
                    PseudoClasses.Remove(":checked");
                    PseudoClasses.Remove(":indeterminate");
                    RoutedEventArgs ce1 = new RoutedEventArgs(UncheckedEvent, this);
                    RaiseEvent(ce1);
                    break;
                case CheckState.Checked:
                    PseudoClasses.Add(":checked");
                    PseudoClasses.Remove(":indeterminate");
                    RoutedEventArgs ce2 = new RoutedEventArgs(CheckedEvent, this);
                    RaiseEvent(ce2);
                    break;
                case CheckState.Indeterminate:
                    PseudoClasses.Add(":checked");
                    PseudoClasses.Add(":indeterminate");
                    RoutedEventArgs ce3 = new RoutedEventArgs(IndeterminateEvent, this);
                    RaiseEvent(ce3);
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region Color Scheme

        /// <summary>
        /// Get or set the color scheme used for this checkbox. For easier color scheme management, bind this to the window or larger control you're using.
        /// </summary>
        public ColorScheme ColorScheme { get => GetValue(ColorSchemeProperty); set => SetValue(ColorSchemeProperty, value); }

        /// <summary>The backing styled property for <see cref="ColorScheme"/>. See the related property for details.</summary>
        public static readonly StyledProperty<ColorScheme> ColorSchemeProperty
            = AvaloniaProperty.Register<CheckBox, ColorScheme>(nameof(ColorScheme), new ColorScheme());

        void OnColorSchemeChange(AvaloniaPropertyChangedEventArgs e)
        {
            ApplyColorScheme(e.GetNewValue<ColorScheme>());
        }

        /// <summary>
        /// Apply a color scheme to this control. The color scheme can quickly apply a whole visual style to the control.
        /// </summary>
        /// <param name="cs">The color scheme to apply.</param>
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
                //Background = cs.BackgroundColor.ToBrush();
                BackgroundDisabledBrush = cs.BackgroundColor.ToBrush();
                CheckForeground = cs.ForegroundColor.ToBrush();
            }
            else
            {
                //Background = Colors.White.ToBrush();
                BackgroundDisabledBrush = cs.LightDisabledColor.ToBrush();
                CheckForeground = Colors.Black.ToBrush();
            }

            //Background = cs.SecondaryColor.ToBrush();
            BorderBrush = cs.BorderColor.ToBrush();
            //HighlightBrush = cs.SecondHighlightColor.ToBrush();
            BackgroundDisabledBrush = cs.LightDisabledColor.ToBrush();
            BorderDisabledBrush = cs.DarkDisabledColor.ToBrush();
            CheckDisabledBrush = cs.DarkDisabledColor.ToBrush();
            //SelectedBrush = cs.ThirdHighlightColor.ToBrush();
            BorderHighlightBrush = cs.HighlightColor.ToBrush();
            //BorderSelectedBrush = cs.SelectionColor.ToBrush();
            Foreground = cs.ForegroundColor.ToBrush();
        }

        #endregion

        #region Brushes

        /// <summary>
        /// Get or set the brush used for the background of the checkbox's box. This is not set via a color scheme.
        /// </summary>
        public IBrush? CheckBackground { get => GetValue(CheckBackgroundProperty); set => SetValue(CheckBackgroundProperty, value); }

        /// <summary>The backing styled property for <see cref="CheckBackground"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> CheckBackgroundProperty
            = AvaloniaProperty.Register<CheckBox, IBrush?>(nameof(CheckBackground), Colors.White.ToBrush());

        /// <summary>
        /// Get or set the brush used for the check mark in the checkbox's box.
        /// </summary>
        public IBrush? CheckForeground { get => GetValue(CheckForegroundProperty); set => SetValue(CheckForegroundProperty, value); }

        /// <summary>The backing styled property for <see cref="CheckForeground"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> CheckForegroundProperty
            = AvaloniaProperty.Register<CheckBox, IBrush?>(nameof(CheckForeground), Colors.Black.ToBrush());

        public IBrush? HighlightBrush { get => GetValue(HighlightBrushProperty); set => SetValue(HighlightBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="HighlightBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> HighlightBrushProperty
            = AvaloniaProperty.Register<CheckBox, IBrush?>(nameof(HighlightBrush), ColorsHelper.DarkerGray.ToBrush());

        public IBrush? BackgroundHighlightBrush { get => GetValue(BackgroundHighlightBrushProperty); set => SetValue(BackgroundHighlightBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="BackgroundHighlightBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> BackgroundHighlightBrushProperty
            = AvaloniaProperty.Register<CheckBox, IBrush?>(nameof(BackgroundHighlightBrush), Color.FromArgb(16, 255, 255, 255).ToBrush());

        /// <summary>
        /// Get or set the brush to use for the background of the checkbox's box when it is disabled.
        /// </summary>
        public IBrush? BackgroundDisabledBrush { get => GetValue(BackgroundDisabledBrushProperty); set => SetValue(BackgroundDisabledBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="BackgroundDisabledBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> BackgroundDisabledBrushProperty
            = AvaloniaProperty.Register<CheckBox, IBrush?>(nameof(BackgroundDisabledBrush), ColorsHelper.CreateFromHex("c4d9d9d9").ToBrush());

        /// <summary>
        /// Get or set the brush used for the border of the checkbox's box when it is disabled.
        /// </summary>
        public IBrush? BorderDisabledBrush { get => GetValue(BorderDisabledBrushProperty); set => SetValue(BorderDisabledBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="BorderDisabledBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> BorderDisabledBrushProperty
            = AvaloniaProperty.Register<CheckBox, IBrush?>(nameof(BorderDisabledBrush), Colors.DarkGray.ToBrush());

        /// <summary>
        /// Get or set the brush used for the check mark when the control is disabled.
        /// </summary>
        public IBrush? CheckDisabledBrush { get => GetValue(CheckDisabledBrushProperty); set => SetValue(CheckDisabledBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="CheckDisabledBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> CheckDisabledBrushProperty
            = AvaloniaProperty.Register<CheckBox, IBrush?>(nameof(CheckDisabledBrush), Colors.DarkGray.ToBrush());

        /// <summary>
        /// Get or set the brush used for the border of the checkbox's box, while the mouse is over the control or it has keyboard focus.
        /// </summary>
        public IBrush? BorderHighlightBrush { get => GetValue(BorderHighlightBrushProperty); set => SetValue(BorderHighlightBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="BorderHighlightBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> BorderHighlightBrushProperty
            = AvaloniaProperty.Register<CheckBox, IBrush?>(nameof(BorderHighlightBrush), ColorsHelper.DarkerGray.ToBrush());

        public IBrush? CheckBorderBrush { get => GetValue(CheckBorderBrushProperty); set => SetValue(CheckBorderBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="CheckBorderBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> CheckBorderBrushProperty
            = AvaloniaProperty.Register<CheckBox, IBrush?>(nameof(CheckBorderBrush), Colors.Black.ToBrush());

        #endregion

        #region Border / Content

        /// <summary>
        /// Get or set the thickness of the border of the check box.
        /// </summary>
        public Thickness CheckBorderThickness { get => GetValue(CheckBorderThicknessProperty); set => SetValue(CheckBorderThicknessProperty, value); }

        /// <summary>The backing styled property for <see cref="CheckBorderThickness"/>. See the related property for details.</summary>
        public static readonly StyledProperty<Thickness> CheckBorderThicknessProperty
            = AvaloniaProperty.Register<CheckBox, Thickness>(nameof(CheckBorderThickness), new Thickness(1));

        /// <summary>
        /// Get or set the thickness of the border of the check box, while the check box's IsChecked property is true.
        /// </summary>
        public Thickness CheckBorderSelectionThickness { get => GetValue(CheckBorderSelectionThicknessProperty); set => SetValue(CheckBorderSelectionThicknessProperty, value); }

        /// <summary>The backing styled property for <see cref="CheckBorderSelectionThickness"/>. See the related property for details.</summary>
        public static readonly StyledProperty<Thickness> CheckBorderSelectionThicknessProperty
            = AvaloniaProperty.Register<CheckBox, Thickness>(nameof(CheckBorderSelectionThickness), new Thickness(1));

        /// <summary>
        /// Get or set the corner radius of the check box.
        /// </summary>
        public CornerRadius CheckBorderCornerRadius { get => GetValue(CheckBorderCornerRadiusProperty); set => SetValue(CheckBorderCornerRadiusProperty, value); }

        /// <summary>The backing styled property for <see cref="CheckBorderCornerRadius"/>. See the related property for details.</summary>
        public static readonly StyledProperty<CornerRadius> CheckBorderCornerRadiusProperty
            = AvaloniaProperty.Register<CheckBox, CornerRadius>(nameof(CheckBorderCornerRadius), new CornerRadius(0));

        /// <summary>
        /// Get or set if the content of this checkbox should be dimmed (opacity lowered) when the checkbox is disabled.
        /// </summary>
        public bool DimContentWhenDisabled { get => GetValue(DimContentWhenDisabledProperty); set => SetValue(DimContentWhenDisabledProperty, value); }

        /// <summary>The backing styled property for <see cref="DimContentWhenDisabled"/>. See the related property for details.</summary>
        public static readonly StyledProperty<bool> DimContentWhenDisabledProperty
            = AvaloniaProperty.Register<CheckBox, bool>(nameof(DimContentWhenDisabled), true);


        #endregion

        #region Click / Selection Handling

        #region Base Variables

        private bool _isPressed = false;
        private bool _isRightPressed = false;

        #endregion

        #region Click Events

        /// <summary>
        /// Defines the <see cref="Click"/> event.
        /// </summary>
        public static readonly RoutedEvent<RoutedEventArgs> ClickEvent =
            RoutedEvent.Register<CheckBox, RoutedEventArgs>(nameof(Click), RoutingStrategies.Bubble);

        /// <summary>
        /// Defines the <see cref="RightClick"/> event.
        /// </summary>
        public static readonly RoutedEvent<RoutedEventArgs> RightClickEvent =
            RoutedEvent.Register<CheckBox, RoutedEventArgs>(nameof(RightClick), RoutingStrategies.Bubble);

        /// <summary>
        /// Raised when the user clicks the checkbox.
        /// </summary>
        public event EventHandler<RoutedEventArgs> Click
        {
            add => AddHandler(ClickEvent, value);
            remove => RemoveHandler(ClickEvent, value);
        }

        /// <summary>
        /// Raised when the user clicks the checkbox with the right mouse.
        /// </summary>
        public event EventHandler<RoutedEventArgs> RightClick
        {
            add => AddHandler(RightClickEvent, value);
            remove => RemoveHandler(RightClickEvent, value);
        }

        #endregion

        #region Click Properties

        /// <summary>
        /// The backing styled property for <see cref="ClickMode"/>. See the related property for details.
        /// </summary>
        public static readonly StyledProperty<ClickMode> ClickModeProperty
            = AvaloniaProperty.Register<CheckBox, ClickMode>(nameof(ClickMode), ClickMode.Release);

        /// <summary>
        /// The backing direct property for <see cref="IsPressed"/>. See the related property for details.
        /// </summary>
        public static readonly DirectProperty<CheckBox, bool> IsPressedProperty
            = AvaloniaProperty.RegisterDirect<CheckBox, bool>(nameof(IsPressed), (fb) => fb.IsPressed, unsetValue: false);

        /// <summary>
        /// The backing direct property for <see cref="IsRightPressed"/>. See the related property for details.
        /// </summary>
        public static readonly DirectProperty<CheckBox, bool> IsRightPressedProperty
            = AvaloniaProperty.RegisterDirect<CheckBox, bool>(nameof(IsRightPressed), (fb) => fb.IsRightPressed, unsetValue: false);

        /// <summary>
        /// Gets or sets a value indicating how this checkbox should react to clicks.
        /// </summary>
        public ClickMode ClickMode
        {
            get => GetValue(ClickModeProperty);
            set => SetValue(ClickModeProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this checkbox is currently being pressed down via the primary input.
        /// </summary>
        /// <remarks>
        /// Primary input includes the left mouse checkbox, or a touch occurring with a pen on a tablet or a finger/stylus on a touchpad.
        /// </remarks>
        public bool IsPressed
        {
            get => _isPressed;
            private set => SetAndRaise(IsPressedProperty, ref _isPressed, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this checkbox is currently being pressed down via the secondary input.
        /// </summary>
        /// <remarks>
        /// Secondary input primarily refers to the right mouse checkbox, but other inputs could apply (such as holding a pen immediately above a tablet).
        /// </remarks>
        public bool IsRightPressed
        {
            get => _isRightPressed;
            private set => SetAndRaise(IsRightPressedProperty, ref _isRightPressed, value);
        }

        #endregion

        #region CheckBox Properties

        /// <summary>
        /// Gets or sets whether the checkbox should cycle through three states (rather than two) when clicked. 
        /// The third state is the "Indeterminate" state, which can be checked via <see cref="IsIndeterminate"/> or <see cref="CheckState"/>.
        /// </summary>
        public bool TriStateClick { get => GetValue(TriStateClickProperty); set => SetValue(TriStateClickProperty, value); }

        /// <summary>The backing styled property for <see cref="TriStateClick"/>. See the related property for details.</summary>
        public static readonly StyledProperty<bool> TriStateClickProperty
            = AvaloniaProperty.Register<CheckBox, bool>(nameof(TriStateClick), false);

        #endregion

        #region Base Click Functions

        /// <summary>
        /// Perform a click programmatically. This control responds the same way as if it was clicked by the user.
        /// </summary>
        public void DoClick()
        {
            OnClick();
        }

        /// <summary>
        /// Perform a click on this checkbox, raising the <see cref="Click"/> event and causing any automatic actions like changing the CheckState.
        /// </summary>
        protected void OnClick()
        {
            if (TriStateClick)
            {
                if (IsChecked && IsIndeterminate)
                {
                    IsChecked = false;
                }
                else if (IsChecked && !IsIndeterminate)
                {
                    IsIndeterminate = true;
                }
                else
                {
                    IsChecked = true;
                    IsIndeterminate = false;
                }
            }
            else
            {
                IsChecked = !IsChecked;
            }

            RoutedEventArgs rre = new RoutedEventArgs(ClickEvent);
            RaiseEvent(rre);
        }

        /// <summary>
        /// Perform a right click on this checkbox, raising the <see cref="RightClick"/> event.
        /// </summary>
        protected void OnRightClick()
        {
            RoutedEventArgs rre = new RoutedEventArgs(RightClickEvent);
            RaiseEvent(rre);
        }

        #endregion

        #region Event Handlers

        /// <inheritdoc/>
        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);

            var pointerProperties = e.GetCurrentPoint(this).Properties;

            if (pointerProperties.IsRightButtonPressed)
            {
                RegisterRightPress();
                // e.Handled = true; // don't want to override base right-clicking functionality
            }
            else if (pointerProperties.IsLeftButtonPressed)
            {
                RegisterPress();
                e.Handled = true;
            }
        }

        void RegisterPress()
        {
            IsPressed = true;
            if (ClickMode == ClickMode.Press)
            {
                OnClick();
            }
        }

        void RegisterRightPress()
        {
            IsRightPressed = true;
            if (ClickMode == ClickMode.Press)
            {
                OnRightClick();
            }
        }

        /// <inheritdoc/>
        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            base.OnPointerReleased(e);

            bool isInThis = this.GetVisualsAt(e.GetPosition(this)).Any(c => this == c || this.IsVisualAncestorOf(c));


            if (IsPressed && e.InitialPressMouseButton == MouseButton.Left)
            {
                IsPressed = false;

                if (ClickMode != ClickMode.Press && isInThis)
                {
                    e.Handled = true;
                    OnClick();
                }
            }
            else if (IsRightPressed && e.InitialPressMouseButton == MouseButton.Right)
            {
                IsRightPressed = false;

                if (ClickMode != ClickMode.Press && isInThis)
                {
                    //e.Handled = true;
                    OnRightClick();
                }
            }
        }

        /// <inheritdoc/>
        protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e)
        {
            base.OnPointerCaptureLost(e);

            IsPressed = false;
        }

        /// <inheritdoc/>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);

            IsPressed = false;
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// Represents the current state of a <see cref="CheckBox"/>.
    /// </summary>
    public enum CheckState
    {
        /// <summary>
        /// The CheckBox is unchecked. Nothing is displayed in the box.
        /// </summary>
        Unchecked = 0,
        /// <summary>
        /// The CheckBox is checked. A check mark is displayed in the box.
        /// </summary>
        Checked = 1,
        /// <summary>
        /// The CheckBox is indeterminate. A square is displayed in the box.
        /// </summary>
        Indeterminate = 2,
    }
}
