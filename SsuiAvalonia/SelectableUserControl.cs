using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace SolidShineUi
{
    /// <summary>
    /// The basic control that can be added into a <see cref="SelectPanel"/>. Extend this class to create your own UI elements to use with the SelectPanel.
    /// </summary>
    public class SelectableUserControl : UserControl, ICommandSource, IClickSelectableControl
    {

        /// <summary>
        /// Create a SelectableUserControl.
        /// </summary>
        public SelectableUserControl()
        {

        }

        #region Properties

        #region Color Scheme

        /// <summary>
        /// Get or set the color scheme to apply to this button. The color scheme can quickly apply a whole visual style to your control.
        /// </summary>
        public ColorScheme ColorScheme { get => GetValue(ColorSchemeProperty); set => SetValue(ColorSchemeProperty, value); }

        /// <summary>The backing styled property for <see cref="ColorScheme"/>. See the related property for details.</summary>
        public static readonly StyledProperty<ColorScheme> ColorSchemeProperty
            = AvaloniaProperty.Register<SelectableUserControl, ColorScheme>(nameof(ColorScheme), new ColorScheme());

        /// <summary>
        /// Raised when the <see cref="ColorScheme"/> property has changed.
        /// </summary>
        public event EventHandler<AvaloniaPropertyChangedEventArgs>? ColorSchemeChanged;


        /// <summary>
        /// Apply a color scheme to this control, and set some other optional appearance settings. The color scheme can quickly apply a whole visual style to the control.
        /// </summary>
        /// <param name="cs">The color scheme to apply</param>
        public void ApplyColorScheme(ColorScheme cs)
        {
            if (ColorScheme != cs)
            {
                ColorScheme = cs;
                return;
            }

            //_internalAction = false;

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

        #endregion

        #region Brushes

        /// <summary>
        /// Get or set the brush used for the background of the control while the mouse/pointer is clicking it.
        /// </summary>
        [Category("Brushes")]
        public IBrush? ClickBrush { get => GetValue(ClickBrushProperty); set => SetValue(ClickBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="ClickBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> ClickBrushProperty
            = AvaloniaProperty.Register<SelectableUserControl, IBrush?>(nameof(ClickBrush), Colors.LightGray.ToBrush());


        /// <summary>
        /// Get or set the brush used for the background of the button while it is selected
        /// (i.e. the <c>IsSelected</c> property is true).
        /// </summary>
        [Category("Brushes")]
        public IBrush? SelectedBrush { get => GetValue(SelectedBrushProperty); set => SetValue(SelectedBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="SelectedBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> SelectedBrushProperty
            = AvaloniaProperty.Register<SelectableUserControl, IBrush?>(nameof(SelectedBrush), Colors.WhiteSmoke.ToBrush());


        /// <summary>
        /// Get or set the brush used for the background of the control while the mouse/pointer is over it, or it has keyboard focus.
        /// </summary>
        [Category("Brushes")]
        public IBrush? HighlightBrush { get => GetValue(HighlightBrushProperty); set => SetValue(HighlightBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="HighlightBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> HighlightBrushProperty
            = AvaloniaProperty.Register<SelectableUserControl, IBrush?>(nameof(HighlightBrush), Colors.Gainsboro.ToBrush());


        /// <summary>
        /// Get or set the brush used for the background of the control when it is disabled.
        /// </summary>
        [Category("Brushes")]
        public IBrush? DisabledBrush { get => GetValue(DisabledBrushProperty); set => SetValue(DisabledBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="DisabledBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> DisabledBrushProperty
            = AvaloniaProperty.Register<SelectableUserControl, IBrush?>(nameof(DisabledBrush), Colors.LightGray.ToBrush());


        /// <summary>
        /// Get or set the brush used for the border around the control, when it is disabled.
        /// </summary>
        [Category("Brushes")]
        public IBrush? BorderDisabledBrush { get => GetValue(BorderDisabledBrushProperty); set => SetValue(BorderDisabledBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="BorderDisabledBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> BorderDisabledBrushProperty
            = AvaloniaProperty.Register<SelectableUserControl, IBrush?>(nameof(BorderDisabledBrush), Colors.Gray.ToBrush());


        /// <summary>
        /// Get or set the brush used for the border while the control has the mouse/pointer over it (or it has keyboard focus).
        /// </summary>
        [Category("Brushes")]
        public IBrush? BorderHighlightBrush { get => GetValue(BorderHighlightBrushProperty); set => SetValue(BorderHighlightBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="BorderHighlightBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> BorderHighlightBrushProperty
            = AvaloniaProperty.Register<SelectableUserControl, IBrush?>(nameof(BorderHighlightBrush), Colors.Black.ToBrush());


        /// <summary>
        /// Get or set the brush used for the border while the control is selected
        /// (i.e. the <c>IsSelected</c> property is true).
        /// </summary>
        [Category("Brushes")]
        public IBrush? BorderSelectedBrush { get => GetValue(BorderSelectedBrushProperty); set => SetValue(BorderSelectedBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="BorderSelectedBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> BorderSelectedBrushProperty
            = AvaloniaProperty.Register<SelectableUserControl, IBrush?>(nameof(BorderSelectedBrush), Colors.DimGray.ToBrush());


        /// <summary>
        /// Get or set the brush to use for the background of this control when in its default state (e.g., when not selected or highlighted).
        /// </summary>
        /// <remarks>
        /// Setting <c>Background</c> will only affect its background for the current state and time; once the state changes,
        /// the background will be overwritten with either this brush or one of the relevant other brushes.
        /// Instead, this brush should be set to control what the background should be when falling back to a default, base state.
        /// </remarks>
        [Category("Brushes")]
        public IBrush? BaseBackground { get => GetValue(BaseBackgroundProperty); set => SetValue(BaseBackgroundProperty, value); }

        /// <summary>The backing styled property for <see cref="BaseBackground"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> BaseBackgroundProperty
            = AvaloniaProperty.Register<SelectableUserControl, IBrush?>(nameof(BaseBackground), Colors.White.ToBrush());


        /// <summary>
        /// Get or set the brush to use for the foreground of this control when in its default state (e.g., when not selected or highlighted).
        /// </summary>
        /// <remarks>
        /// Setting <c>Foreground</c> will only affect its foreground for the current state and time; once the state changes,
        /// the foreground will be overwritten with either this brush or one of the relevant other brushes.
        /// Instead, this brush should be set to control what the foreground should be when falling back to a default, base state.
        /// </remarks>
        [Category("Brushes")]
        public IBrush? BaseForeground { get => GetValue(BaseForegroundProperty); set => SetValue(BaseForegroundProperty, value); }

        /// <summary>The backing styled property for <see cref="BaseForeground"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> BaseForegroundProperty
            = AvaloniaProperty.Register<SelectableUserControl, IBrush?>(nameof(BaseForeground), Colors.Black.ToBrush());


        /// <summary>
        /// Get or set the brush to use for the background of this contol while it is highlighted (i.e. has a mouse over it, or has keyboard focus).
        /// </summary>
        [Category("Brushes")]
        public IBrush? HighlightForeground { get => GetValue(HighlightForegroundProperty); set => SetValue(HighlightForegroundProperty, value); }

        /// <summary>The backing styled property for <see cref="HighlightForeground"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> HighlightForegroundProperty
            = AvaloniaProperty.Register<SelectableUserControl, IBrush?>(nameof(HighlightForeground), Colors.Black.ToBrush());


        /// <summary>
        /// Get or set the brush to use for the background of this control while it is being clicked.
        /// </summary>
        [Category("Brushes")]
        public IBrush? DisabledForeground { get => GetValue(DisabledForegroundProperty); set => SetValue(DisabledForegroundProperty, value); }

        /// <summary>The backing styled property for <see cref="DisabledForeground"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> DisabledForegroundProperty
            = AvaloniaProperty.Register<SelectableUserControl, IBrush?>(nameof(DisabledForeground), Colors.DimGray.ToBrush());


        /// <summary>
        /// Get or set the brush to use for the background of this control while it is selected.
        /// </summary>
        [Category("Brushes")]
        public IBrush? SelectedForeground { get => GetValue(SelectedForegroundProperty); set => SetValue(SelectedForegroundProperty, value); }

        /// <summary>The backing styled property for <see cref="SelectedForeground"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> SelectedForegroundProperty
            = AvaloniaProperty.Register<SelectableUserControl, IBrush?>(nameof(SelectedForeground), Colors.Black.ToBrush());



        #endregion

        #region Property Changes

        /// <inheritdoc/>
        protected override void OnLoaded(RoutedEventArgs e)
        {
            base.OnLoaded(e);

            // set up bindings to BaseBackground and BaseForeground
            // I can do this in Avalonia and not WPF because Avalonia lets me set the binding priority level
            Bind(BackgroundProperty, new Binding(nameof(BaseBackground)) 
                { RelativeSource = new RelativeSource(RelativeSourceMode.Self), Priority = BindingPriority.Style });
            Bind(ForegroundProperty, new Binding(nameof(BaseForeground))
                { RelativeSource = new RelativeSource(RelativeSourceMode.Self), Priority = BindingPriority.Style });
        }

        /// <inheritdoc/>
        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            switch (change.Property.Name)
            {
                case nameof(ColorScheme):
                    ApplyColorScheme(ColorScheme);
                    ColorSchemeChanged?.Invoke(this, change);
                    break;
                case nameof(CanSelect):
                    OnCanSelectChanged(change);
                    break;
                case nameof(IsEnabled):
                    OnIsEnabledChanged(change);
                    break;
            }
        }

        void OnIsEnabledChanged(AvaloniaPropertyChangedEventArgs e)
        {
            if (e.GetNewValue<bool>() == false)
            {
                SetValue(BackgroundProperty, DisabledBrush);
                SetValue(ForegroundProperty, DisabledForeground);
            }
            else
            {
                if (Highlighting)
                {
                    SetValue(BackgroundProperty, HighlightBrush);
                    SetValue(ForegroundProperty, HighlightForeground);
                    // SetValue(BorderBrushProperty, BorderHighlightBrush);
                }
                else if (IsSelected)
                {
                    SetValue(BackgroundProperty, SelectedBrush);
                    SetValue(ForegroundProperty, SelectedForeground);
                }
                else
                {
                    ClearValue(BackgroundProperty);
                    ClearValue(ForegroundProperty);
                }
            }
        }

        #endregion

        #endregion

        #region Click / Selection Handling

        #region Base Variables

        private bool _isPressed = false;
        private bool _isRightPressed = false;
        private bool _isPressedByKey = false;

        #endregion

        #region Selection Properties

        /// <summary>
        /// Get or set if this control should change its <see cref="IsSelected"/> value when you click on the control.
        /// </summary>
        /// <remarks>
        /// This allows more fine-tuned control over when and how this control can be selected. If this is <c>false</c>, 
        /// then the user can only use other methods to select this control, such as a checkbox, rather than just clicking 
        /// on it. You can use <see cref="CanSelect"/> to globally disable selecting this control entirely.
        /// </remarks>
        public bool SelectOnClick { get => GetValue(SelectOnClickProperty); set => SetValue(SelectOnClickProperty, value); }

        /// <summary>The backing dependency property for <see cref="SelectOnClick"/>. See the related property for details.</summary>
        public static readonly StyledProperty<bool> SelectOnClickProperty
            = AvaloniaProperty.Register<SelectableUserControl, bool>(nameof(SelectOnClick), false);

        /// <summary>
        /// Get or set if this control can be selected.
        /// </summary>
        /// <remarks>
        /// If this is set to <c>false</c>, then this control cannot be selected via any method - even by setting <see cref="IsSelected"/>
        /// directly. Setting this to <c>false</c> will also deselect this control, if currently selected. For more fine-tuned control, 
        /// you can use <see cref="SelectOnClick"/> to limit how the user can select this control, while still being able to change the 
        /// selection status via <see cref="IsSelected"/>.
        /// </remarks>
        public bool CanSelect { get => GetValue(CanSelectProperty); set => SetValue(CanSelectProperty, value); }

        /// <summary>The backing dependency property for <see cref="CanSelect"/>. See the related property for details.</summary>
        public static readonly StyledProperty<bool> CanSelectProperty
            = AvaloniaProperty.Register<SelectableUserControl, bool>(nameof(CanSelect), true);

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
        public static readonly DirectProperty<SelectableUserControl, bool> IsSelectedProperty
            = AvaloniaProperty.RegisterDirect<SelectableUserControl, bool>(nameof(IsSelected), (fb) => fb.IsSelected, (fb, v) => fb.IsSelected = v);

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

        #region SetIsSelectedWithSource
        /// <summary>
        /// Set the <see cref="IsSelected"/> value of this control, while also defining how the selection was changed.
        /// </summary>
        /// <param name="value">The value to set <see cref="IsSelected"/> to.</param>
        /// <param name="triggerMethod">The source or method used to trigger the change in selection.</param>
        /// <param name="triggerSource">The object that triggered the change.</param>
        /// <remarks>If <see cref="CanSelect"/> is set to <c>false</c>, then nothing will occur (silent fail).</remarks>
        public void SetIsSelectedWithSource(bool value, SelectionChangeTrigger triggerMethod, object? triggerSource = null)
        {
            if (CanSelect)
            {
                bool curVal = sel;
                SetAndRaise(IsSelectedProperty, ref sel, value);
                //PseudoClasses.Set(pcSel, value);

                if (curVal != sel)
                {
                    ItemSelectionChangedEventArgs e = new ItemSelectionChangedEventArgs(IsSelectedChangedEvent, curVal, sel, triggerMethod, triggerSource);
                    RaiseEvent(e);
                }
            }
        }
        #endregion

        #endregion

        #region Selection Events

        /// <summary>
        /// Defines the <see cref="IsSelectedChanged"/> event.
        /// </summary>
        public static readonly RoutedEvent<ItemSelectionChangedEventArgs> IsSelectedChangedEvent =
            RoutedEvent.Register<SelectableUserControl, ItemSelectionChangedEventArgs>(nameof(IsSelectedChanged), RoutingStrategies.Bubble);

        /// <summary>
        /// Defines the <see cref="CanSelectChanged"/> event.
        /// </summary>
        public static readonly RoutedEvent<RoutedEventArgs> CanSelectChangedEvent =
            RoutedEvent.Register<SelectableUserControl, RoutedEventArgs>(nameof(CanSelectChanged), RoutingStrategies.Bubble);

        /// <summary>
        /// Raised when the user clicks the button with the right mouse.
        /// </summary>
        public event EventHandler<ItemSelectionChangedEventArgs> IsSelectedChanged
        {
            add => AddHandler(IsSelectedChangedEvent, value);
            remove => RemoveHandler(IsSelectedChangedEvent, value);
        }

        /// <summary>
        /// Raised when the <see cref="CanSelect"/> property has changed.
        /// </summary>
        public event EventHandler<RoutedEventArgs> CanSelectChanged
        {
            add => AddHandler(CanSelectChangedEvent, value);
            remove => RemoveHandler(CanSelectChangedEvent, value);
        }

        #endregion

        #region Click Events

        /// <summary>
        /// Defines the <see cref="Click"/> event.
        /// </summary>
        public static readonly RoutedEvent<RoutedEventArgs> ClickEvent =
            RoutedEvent.Register<SelectableUserControl, RoutedEventArgs>(nameof(Click), RoutingStrategies.Bubble);

        /// <summary>
        /// Defines the <see cref="RightClick"/> event.
        /// </summary>
        public static readonly RoutedEvent<RoutedEventArgs> RightClickEvent =
            RoutedEvent.Register<SelectableUserControl, RoutedEventArgs>(nameof(RightClick), RoutingStrategies.Bubble);

        /// <summary>
        /// Raised when the user clicks the button.
        /// </summary>
        public event EventHandler<RoutedEventArgs> Click
        {
            add => AddHandler(ClickEvent, value);
            remove => RemoveHandler(ClickEvent, value);
        }

        /// <summary>
        /// Raised when the user clicks the button with the right mouse.
        /// </summary>
        public event EventHandler<RoutedEventArgs> RightClick
        {
            add => AddHandler(RightClickEvent, value);
            remove => RemoveHandler(RightClickEvent, value);
        }

        #endregion

        #region ICommandSource implementations

        private bool _commandCanExecute = true;

        /// <summary>
        /// The backing styled property for <see cref="Command"/>. See the related property for details.
        /// </summary>
        public static readonly StyledProperty<ICommand?> CommandProperty
            = AvaloniaProperty.Register<SelectableUserControl, ICommand?>(nameof(Command), null);

        /// <summary>
        /// The backing styled property for <see cref="CommandParameter"/>. See the related property for details.
        /// </summary>
        public static readonly StyledProperty<object?> CommandParameterProperty
            = AvaloniaProperty.Register<SelectableUserControl, object?>(nameof(CommandParameter), null);

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

        /// <inheritdoc/>
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
            = AvaloniaProperty.Register<SelectableUserControl, ClickMode>(nameof(ClickMode), ClickMode.Release);

        /// <summary>
        /// The backing direct property for <see cref="IsPressed"/>. See the related property for details.
        /// </summary>
        public static readonly DirectProperty<SelectableUserControl, bool> IsPressedProperty
            = AvaloniaProperty.RegisterDirect<SelectableUserControl, bool>(nameof(IsPressed), (fb) => fb.IsPressed, unsetValue: false);

        /// <summary>
        /// The backing direct property for <see cref="IsRightPressed"/>. See the related property for details.
        /// </summary>
        public static readonly DirectProperty<SelectableUserControl, bool> IsRightPressedProperty
            = AvaloniaProperty.RegisterDirect<SelectableUserControl, bool>(nameof(IsRightPressed), (fb) => fb.IsRightPressed, unsetValue: false);

        /// <summary>
        /// The backing direct property for <see cref="IsPressedByKey"/>. See the related proeprty for details.
        /// </summary>
        public static readonly DirectProperty<SelectableUserControl, bool> IsPressedByKeyProperty
            = AvaloniaProperty.RegisterDirect<SelectableUserControl, bool>(nameof(IsPressedByKey), (fb) => fb.IsPressedByKey, unsetValue: false);

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
        /// For being pressed by a key press, refer to <see cref="IsPressedByKey"/>.
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

        /// <summary>
        /// Gets or sets a value indicating whether this button is currently being pressed down via the Enter or Space keys being held down.
        /// For being pressed by other inputs (e.g., mouse, touch, pen), see <see cref="IsPressed"/>.
        /// </summary>
        public bool IsPressedByKey
        {
            get => _isPressedByKey;
            private set => SetAndRaise(IsPressedProperty, ref _isPressedByKey, value);
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

            if (Command != null && Command.CanExecute(CommandParameter))
            {
                Command.Execute(CommandParameter);
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

        #region Highlighting

        private bool _highlighting = false;

        /// <summary>
        /// Get if this control is currently in a "highlight" state.
        /// <para/>
        /// Use <see cref="Highlight"/> and <see cref="Unhighlight"/> to change states.
        /// </summary>
        public bool Highlighting { get => _highlighting; private set => SetAndRaise(HighlightingProperty, ref _highlighting, value); }

        /// <summary>The backing direct property for <see cref="Highlighting"/>. See the related property for details.</summary>
        public static readonly DirectProperty<SelectableUserControl, bool> HighlightingProperty
            = AvaloniaProperty.RegisterDirect<SelectableUserControl, bool>(nameof(Highlighting), (s) => s.Highlighting, unsetValue: false);


        /// <summary>
        /// Change the control to a highlighted state, while it has focus or has the mouse or stylus over it.
        /// </summary>
        protected virtual void Highlight()
        {
            Highlighting = true;
            if (CanSelect && SelectOnClick)
            {
                // in the future, I may want to look into using the IDisposible object returned by these methods
                SetValue(BackgroundProperty, HighlightBrush);
                SetValue(ForegroundProperty, HighlightForeground);
            }
        }

        /// <summary>
        /// Change the control to an unhighlighted state, when it no longer has focus or has the mouse or stylus over it.
        /// </summary>
        protected virtual void Unhighlight()
        {
            Highlighting = false;

            if (IsSelected)
            {
                SetValue(BackgroundProperty, SelectedBrush);
                SetValue(ForegroundProperty, SelectedForeground);
            }
            else
            {
                ClearValue(BackgroundProperty);
                ClearValue(ForegroundProperty);
            }
        }

        #endregion

        #region Event Handlers

        /// <inheritdoc/>
        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);

            var pointerProperties = e.Properties; // e.GetCurrentPoint(this).Properties;

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

            // set brush
            if (CanSelect)
            {
                SetValue(BackgroundProperty, ClickBrush);
                SetValue(ForegroundProperty, HighlightForeground);
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

            // reset brushes
            if (Highlighting)
            {
                SetValue(BackgroundProperty, HighlightBrush);
                SetValue(ForegroundProperty, HighlightForeground);
            }
            else if (IsSelected)
            {
                SetValue(BackgroundProperty, SelectedBrush);
                SetValue(ForegroundProperty, SelectedForeground);
            }
            else
            {
                ClearValue(BackgroundProperty);
                ClearValue(ForegroundProperty);
            }
        }

        /// <inheritdoc/>
        protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e)
        {
            base.OnPointerCaptureLost(e);

            IsPressed = false;
        }

        /// <inheritdoc/>
        protected override void OnLostFocus(FocusChangedEventArgs e)
        {
            base.OnLostFocus(e);

            IsPressed = false;
        }



        /// <inheritdoc/>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Key == Key.Enter || e.Key == Key.Space)
            {
                _isPressedByKey = true;

                if (ClickMode == ClickMode.Press)
                {
                    OnClick();
                }
            }
        }

        /// <inheritdoc/>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            if (_isPressedByKey && (e.Key == Key.Enter || e.Key == Key.Space))
            {
                if (ClickMode != ClickMode.Press)
                {
                    OnClick();
                }
            }
            else if (e.Key == Key.Apps)
            {
                OnRightClick();
            }
        }

        /// <inheritdoc/>
        protected override void OnPointerEntered(PointerEventArgs e)
        {
            base.OnPointerEntered(e);
            Highlight();
        }

        /// <inheritdoc/>
        protected override void OnPointerExited(PointerEventArgs e)
        {
            base.OnPointerExited(e);
            Unhighlight();
        }

        #endregion

        #endregion
    }
}
