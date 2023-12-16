using Avalonia;
using Avalonia.Input;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Metadata;
using Avalonia.Interactivity;
using System;
using System.Windows.Input;
using Avalonia.VisualTree;
using System.Linq;
using Avalonia.Media;

namespace SolidShineUi
{
    [PseudoClasses(pcPressed, pcTb, pcSel)]
    public class FlatButton : ContentControl, IClickSelectableControl, ICommandSource
    {

        static FlatButton()
        {

        }

        public FlatButton()
        {

        }

        #region Appearance

        private const string pcTb = ":tb";

        #endregion

        #region Click / Selection Handling

        #region Base Variables

        private bool _isPressed = false;
        private bool _isRightPressed = false;

        private const string pcPressed = ":pressed";
        private const string pcSel = ":sel";

        #endregion

        #region Selection Properties
        /// <summary>
        /// Get or set if this control should change its <see cref="IsSelected"/> value when you click on the control.
        /// </summary>
        /// <remarks>
        /// This allows more fine-tuned control over when and how this control can be selected. If this is <c>false</c>, then the user can only use the checkbox to directly 
        /// select or deselect this control. You can use <see cref="CanSelect"/> to globally disable selecting this control via any method.
        /// </remarks>
        public bool SelectOnClick { get => GetValue(SelectOnClickProperty); set => SetValue(SelectOnClickProperty, value); }

        /// <summary>The backing dependency property for <see cref="SelectOnClick"/>. See the related property for details.</summary>
        public static StyledProperty<bool> SelectOnClickProperty
            = AvaloniaProperty.Register<FlatButton, bool>("SelectOnClick", false);

        /// <summary>
        /// Get or set if this control can be selected.
        /// </summary>
        /// <remarks>
        /// If this is set to <c>false</c>, then this control cannot be selected via any method - even programmatically. Setting this to <c>false</c> will also deselect this control, 
        /// if currently selected. For more fine-tuned control, you can use <see cref="SelectOnClick"/> to limit how the user can select this control, 
        /// while still being able to change the selection status via <see cref="IsSelected"/>.
        /// </remarks>
        public bool CanSelect { get => GetValue(CanSelectProperty); set => SetValue(CanSelectProperty, value); }

        /// <summary>The backing dependency property for <see cref="CanSelect"/>. See the related property for details.</summary>
        public static StyledProperty<bool> CanSelectProperty
            = AvaloniaProperty.Register<FlatButton, bool>("CanSelect", true);
        // OnCanSelectChanged

        private void OnCanSelectChanged(AvaloniaPropertyChangedEventArgs e)
        {
            if (((bool?)e.NewValue ?? true) == false)
            {
                if (IsSelected)
                {
                    sel = false;

                    ItemSelectionChangedEventArgs re = new ItemSelectionChangedEventArgs(IsSelectedChangedEvent, true, false, SelectionChangeTrigger.DisableSelecting, e.Sender);
                    RaiseEvent(re);
                }
            }

            RoutedEventArgs cre = new RoutedEventArgs(CanSelectChangedEvent, this);
            RaiseEvent(cre);
        }

        bool sel = false;

        /// <summary>
        /// The backing direct property for <see cref="IsSelected"/>. See the related property for details.
        /// </summary>
        public static DirectProperty<FlatButton, bool> IsSelectedProperty
            = AvaloniaProperty.RegisterDirect<FlatButton, bool>("IsSelected", (fb) => fb.IsSelected, (fb, v) => fb.IsSelected = v);

        /// <summary>
        /// Get or set if this control is currently selected.
        /// </summary>
        /// <remarks>
        /// If <see cref="CanSelect"/> is set to <c>false</c>, then this value will not be changed (silent fail).
        /// Use <see cref="SelectOnClick"/> to offer control over whether this can be selected via the user interacting with this, without disabling 
        /// the ability to set this state programmatically.
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
        /// <remarks>If <see cref="CanSelect"/> is set to <c>false</c>, then nothing will occur (silent fail).</remarks>
#if NETCOREAPP
        public void SetIsSelectedWithSource(bool value, SelectionChangeTrigger triggerMethod, object? triggerSource = null)
#else
        public void SetIsSelectedWithSource(bool value, SelectionChangeTrigger triggerMethod, object triggerSource = null)
#endif
        {
            if (CanSelect)
            {
                bool curVal = sel;
                SetAndRaise(IsSelectedProperty, ref sel, value);
                PseudoClasses.Set(pcSel, value);

                if (curVal != sel)
                {
                    ItemSelectionChangedEventArgs e = new ItemSelectionChangedEventArgs(IsSelectedChangedEvent, curVal, sel, triggerMethod, triggerSource);
                    RaiseEvent(e);
                }
            }
        }
        #endregion

        #region Events

        /// <summary>
        /// Defines the <see cref="Click"/> event.
        /// </summary>
        public static readonly RoutedEvent<RoutedEventArgs> ClickEvent =
            RoutedEvent.Register<FlatButton, RoutedEventArgs>(nameof(Click), RoutingStrategies.Bubble);

        /// <summary>
        /// Defines the <see cref="RightClick"/> event.
        /// </summary>
        public static readonly RoutedEvent<RoutedEventArgs> RightClickEvent =
            RoutedEvent.Register<FlatButton, RoutedEventArgs>(nameof(RightClick), RoutingStrategies.Bubble);

        /// <summary>
        /// Defines the <see cref="IsSelectedChanged"/> event.
        /// </summary>
        public static readonly RoutedEvent<ItemSelectionChangedEventArgs> IsSelectedChangedEvent =
            RoutedEvent.Register<FlatButton, ItemSelectionChangedEventArgs>(nameof(IsSelectedChanged), RoutingStrategies.Bubble);

        /// <summary>
        /// Defines the <see cref="CanSelectChanged"/> event.
        /// </summary>
        public static readonly RoutedEvent<RoutedEventArgs> CanSelectChangedEvent =
            RoutedEvent.Register<FlatButton, RoutedEventArgs>(nameof(CanSelectChanged), RoutingStrategies.Bubble);

        /// <summary>
        /// Raised when the user clicks the button.
        /// </summary>
        public event EventHandler<RoutedEventArgs>? Click
        {
            add => AddHandler(ClickEvent, value);
            remove => RemoveHandler(ClickEvent, value);
        }

        /// <summary>
        /// Raised when the user clicks the button with the right mouse.
        /// </summary>
        public event EventHandler<RoutedEventArgs>? RightClick
        {
            add => AddHandler(RightClickEvent, value);
            remove => RemoveHandler(RightClickEvent, value);
        }

        /// <summary>
        /// Raised when the user clicks the button with the right mouse.
        /// </summary>
        public event EventHandler<ItemSelectionChangedEventArgs>? IsSelectedChanged
        {
            add => AddHandler(IsSelectedChangedEvent, value);
            remove => RemoveHandler(IsSelectedChangedEvent, value);
        }

        /// <summary>
        /// Raised when the <see cref="CanSelect"/> property has changed.
        /// </summary>
        public event EventHandler<RoutedEventArgs>? CanSelectChanged
        {
            add => AddHandler(CanSelectChangedEvent, value);
            remove => RemoveHandler(CanSelectChangedEvent, value);
        }

        #endregion

        #region ICommandSource implementations

        private bool _commandCanExecute = true;

        /// <summary>
        /// The backing styled property for <see cref="Command"/>. See the related property for details.
        /// </summary>
        public static readonly StyledProperty<ICommand?> CommandProperty
            = AvaloniaProperty.Register<FlatButton, ICommand?>(nameof(Command), null);

        /// <summary>
        /// The backing styled property for <see cref="CommandParameter"/>. See the related property for details.
        /// </summary>
        public static readonly StyledProperty<object?> CommandParameterProperty
            = AvaloniaProperty.Register<FlatButton, object?>(nameof(CommandParameter), null);

        /// <summary>
        /// Gets or sets an <see cref="ICommand"/> to be invoked when the button is clicked.
        /// </summary>
        public ICommand? Command
        {
            get => GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        /// <summary>
        /// Gets or sets a parameter to be passed to the <see cref="Command"/>.
        /// </summary>
        public object? CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public void CanExecuteChanged(object sender, EventArgs e)
        {
            // TODO: change if this is enabled??

            //throw new NotImplementedException();
        }

        /// <inheritdoc/>
        protected override bool IsEnabledCore => base.IsEnabledCore && _commandCanExecute;

        #endregion

        #region Click Properties

        /// <summary>
        /// The backing styled property for <see cref="ClickMode"/>. See the related property for details.
        /// </summary>
        public static readonly StyledProperty<ClickMode> ClickModeProperty
            = AvaloniaProperty.Register<FlatButton, ClickMode>(nameof(ClickMode), ClickMode.Release);

        /// <summary>
        /// The backing direct property for <see cref="IsPressed"/>. See the related property for details.
        /// </summary>
        public static readonly DirectProperty<FlatButton, bool> IsPressedProperty
            = AvaloniaProperty.RegisterDirect<FlatButton, bool>(nameof(IsPressed), (fb) => fb.IsPressed, unsetValue: false);

        /// <summary>
        /// The backing direct property for <see cref="IsRightPressed"/>. See the related property for details.
        /// </summary>
        public static readonly DirectProperty<FlatButton, bool> IsRightPressedProperty
            = AvaloniaProperty.RegisterDirect<FlatButton, bool>(nameof(IsRightPressed), (fb) => fb.IsRightPressed, unsetValue: false);

        /// <summary>
        /// Gets or sets a value indicating how this button should react to clicks.
        /// </summary>
        public ClickMode ClickMode
        {
            get => GetValue(ClickModeProperty);
            set => SetValue(ClickModeProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this button is currently being pressed down via the primary input.
        /// </summary>
        /// <remarks>
        /// Primary input includes the left mouse button, or a touch occurring with a pen on a tablet or a finger/stylus on a touchpad.
        /// </remarks>
        public bool IsPressed
        {
            get => _isPressed;
            private set => SetAndRaise(IsPressedProperty, ref _isPressed, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this button is currently being pressed down via the secondary input.
        /// </summary>
        /// <remarks>
        /// Secondary input primarily refers to the right mouse button, but other inputs could apply (such as holding a pen immediately above a tablet).
        /// </remarks>
        public bool IsRightPressed
        {
            get => _isRightPressed;
            private set => SetAndRaise(IsRightPressedProperty, ref _isRightPressed, value);
        }

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
        /// Perform a click on this button, raising the <see cref="Click"/> event and causing any automatic actions like <see cref="SelectOnClick"/> or executing <see cref="Command"/>.
        /// </summary>
        protected void OnClick()
        {
            if (SelectOnClick)
            {
                SetIsSelectedWithSource(!IsSelected, SelectionChangeTrigger.ControlClick, this);
            }

            RoutedEventArgs rre = new RoutedEventArgs(ClickEvent);
            RaiseEvent(rre);
        }

        /// <summary>
        /// Perform a right click on this button, raising the <see cref="RightClick"/> event.
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

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);

            IsPressed = false;
        }

        #endregion

        #endregion

        private Button btn;
    }
}
