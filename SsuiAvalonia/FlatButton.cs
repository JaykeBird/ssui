using System;
using System.Windows.Input;
using System.Linq;
using System.ComponentModel;
using Avalonia;
using Avalonia.Input;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Media;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

namespace SolidShineUi
{
    /// <summary>
    /// A button with a custom, flat style and addiitonal functionality. Use <c>SelectOnClick</c> to have the button act like a toggle button.
    /// </summary>
    [DefaultEvent(nameof(Click))]
    public class FlatButton : ThemedContentControl, IClickSelectableControl, ICommandSource
    {

        /// <summary>
        /// Create a FlatButton.
        /// </summary>
        public FlatButton()
        {
            // applied these in the control template
            //Padding = new Thickness(5, 0, 5, 0);
            //Background = Colors.White.ToBrush();
            //BorderBrush = Colors.Black.ToBrush();
            //BorderThickness = new Thickness(1);
            //CornerRadius = new CornerRadius(0);
        }

        #region Appearance

        #region ColorScheme / TransparentBack

        /// <summary>
        /// Get or set the color scheme to apply to this button. The color scheme can quickly apply a whole visual style to your control.
        /// </summary>
        public ColorScheme ColorScheme { get => GetValue(ColorSchemeProperty); set => SetValue(ColorSchemeProperty, value); }

        /// <summary>The backing styled property for <see cref="ColorScheme"/>. See the related property for details.</summary>
        public static readonly StyledProperty<ColorScheme> ColorSchemeProperty
            = AvaloniaProperty.Register<FlatButton, ColorScheme>(nameof(ColorScheme), new ColorScheme());

        /// <summary>
        /// Get or set whether the button should have a transparent background when the button is not focused.
        /// </summary>
        public bool TransparentBack { get => GetValue(TransparentBackProperty); set => SetValue(TransparentBackProperty, value); }

        /// <summary>The backing styled property for <see cref="TransparentBack"/>. See the related property for details.</summary>
        public static readonly StyledProperty<bool> TransparentBackProperty
            = AvaloniaProperty.Register<FlatButton, bool>(nameof(TransparentBack), false);

        /// <summary>
        /// Raised when the <see cref="ColorScheme"/> property has changed.
        /// </summary>
        public event EventHandler<AvaloniaPropertyChangedEventArgs>? ColorSchemeChanged;

        #endregion

        #region Apply Color Scheme

        //bool _internalAction = true;

        /// <summary>
        /// Apply a color scheme to this control, and set some other optional appearance settings. The color scheme can quickly apply a whole visual style to the control.
        /// </summary>
        /// <param name="cs">The color scheme to apply</param>
        public void ApplyColorScheme(ColorScheme cs)
        {
            ApplyColorScheme(cs, UseAccentTheme);
        }

        /// <summary>
        /// Apply a color scheme to this control, and set some other optional appearance settings. The color scheme can quickly 
        /// apply a whole visual style to the control.
        /// </summary>
        /// <param name="cs">The color scheme to apply</param>
        /// <param name="useAccentColors">Set if accent colors should be used for this button, rather than the main color scheme colors.
        /// This can also be achieved with the <c>UseAccentTheme</c> property.
        /// </param>
        public void ApplyColorScheme(ColorScheme cs, bool useAccentColors = true)
        {
            if (ColorScheme != cs)
            {
                ColorScheme = cs;
                return;
            }

            if (UseAccentTheme != useAccentColors)
            {
                if (SsuiTheme == null) UseAccentTheme = useAccentColors;
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
                if (UseAccentTheme || useAccentColors)
                {
                    Background = cs.AccentSecondaryColor.ToBrush();
                    BorderBrush = cs.AccentBorderColor.ToBrush();
                    HighlightBrush = cs.AccentSecondHighlightColor.ToBrush();
                    DisabledBrush = cs.LightDisabledColor.ToBrush();
                    BorderDisabledBrush = cs.DarkDisabledColor.ToBrush();
                    SelectedBrush = cs.AccentThirdHighlightColor.ToBrush();
                    BorderHighlightBrush = cs.AccentHighlightColor.ToBrush();
                    BorderSelectedBrush = cs.AccentSelectionColor.ToBrush();
                    Foreground = cs.ForegroundColor.ToBrush();
                    ClickBrush = cs.AccentThirdHighlightColor.ToBrush();
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
        }



        #endregion

        #region SsuiTheme

        /// <inheritdoc/>
        protected override void OnApplySsuiTheme(SsuiTheme ssuiTheme, bool useLightBorder = false, bool useAccentTheme = false)
        {
            if (useAccentTheme && ssuiTheme is SsuiAppTheme sat && sat.AccentTheme != null)
            {
                ApplyTheme(sat.AccentTheme);
            }
            else
            {
                ApplyTheme(ssuiTheme);
            }

            void ApplyTheme(SsuiTheme theme)
            {
                ApplyThemeBinding(BackgroundProperty, SsuiTheme.ButtonBackgroundProperty, theme);
                ApplyThemeBinding(HighlightBrushProperty, SsuiTheme.HighlightBrushProperty, theme);
                ApplyThemeBinding(DisabledBrushProperty, SsuiTheme.DisabledBackgroundProperty, theme);
                ApplyThemeBinding(BorderDisabledBrushProperty, SsuiTheme.DisabledBorderBrushProperty, theme);
                ApplyThemeBinding(SelectedBrushProperty, SsuiTheme.SelectedBackgroundBrushProperty, theme);
                ApplyThemeBinding(BorderHighlightBrushProperty, SsuiTheme.HighlightBorderBrushProperty, theme);
                ApplyThemeBinding(BorderSelectedBrushProperty, SsuiTheme.SelectedBorderBrushProperty, theme);
                ApplyThemeBinding(ForegroundProperty, SsuiTheme.ForegroundProperty, theme);
                ApplyThemeBinding(HighlightForegroundProperty, SsuiTheme.HighlightForegroundProperty, theme);
                ApplyThemeBinding(SelectedForegroundProperty, SsuiTheme.SelectedForegroundProperty, theme);
                ApplyThemeBinding(ClickBrushProperty, SsuiTheme.ClickBrushProperty, theme);
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
            = AvaloniaProperty.Register<FlatButton, IBrush?>(nameof(ClickBrush), Colors.Gainsboro.ToBrush());

        /// <summary>
        /// Get or set the brush used for the background of the button while it is selected
        /// (i.e. the <c>IsSelected</c> property is true).
        /// </summary>
        [Category("Brushes")]
        public IBrush? SelectedBrush { get => GetValue(SelectedBrushProperty); set => SetValue(SelectedBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="SelectedBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> SelectedBrushProperty
            = AvaloniaProperty.Register<FlatButton, IBrush?>(nameof(SelectedBrush), Colors.WhiteSmoke.ToBrush());

        /// <summary>
        /// Get or set the brush used for the background of the control while the mouse/pointer is over it, or it has keyboard focus.
        /// </summary>
        [Category("Brushes")]
        public IBrush? HighlightBrush { get => GetValue(HighlightBrushProperty); set => SetValue(HighlightBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="HighlightBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> HighlightBrushProperty
            = AvaloniaProperty.Register<FlatButton, IBrush?>(nameof(HighlightBrush), Colors.LightGray.ToBrush());

        /// <summary>
        /// Get or set the brush used for the background of the control when it is disabled.
        /// </summary>
        [Category("Brushes")]
        public IBrush? DisabledBrush { get => GetValue(DisabledBrushProperty); set => SetValue(DisabledBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="DisabledBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> DisabledBrushProperty
            = AvaloniaProperty.Register<FlatButton, IBrush?>(nameof(DisabledBrush), Colors.LightGray.ToBrush());

        /// <summary>
        /// Get or set the brush used for the border around the control, when it is disabled.
        /// </summary>
        public IBrush? BorderDisabledBrush { get => GetValue(BorderDisabledBrushProperty); set => SetValue(BorderDisabledBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="BorderDisabledBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> BorderDisabledBrushProperty
            = AvaloniaProperty.Register<FlatButton, IBrush?>(nameof(BorderDisabledBrush), Colors.DarkGray.ToBrush());

        /// <summary>
        /// Get or set the brush used for the border while the control has the mouse/pointer over it (or it has keyboard focus).
        /// </summary>
        [Category("Brushes")]
        public IBrush? BorderHighlightBrush { get => GetValue(BorderHighlightBrushProperty); set => SetValue(BorderHighlightBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="BorderHighlightBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> BorderHighlightBrushProperty
            = AvaloniaProperty.Register<FlatButton, IBrush?>(nameof(BorderHighlightBrush), Colors.Black.ToBrush());

        /// <summary>
        /// Get or set the brush used for the border while the control is selected
        /// (i.e. the <c>IsSelected</c> property is true).
        /// </summary>
        [Category("Brushes")]
        public IBrush? BorderSelectedBrush { get => GetValue(BorderSelectedBrushProperty); set => SetValue(BorderSelectedBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="BorderSelectedBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> BorderSelectedBrushProperty
            = AvaloniaProperty.Register<FlatButton, IBrush?>(nameof(BorderSelectedBrush), Colors.DimGray.ToBrush());

        /// <summary>
        /// Get or set the brush used for the foreground while the control has the mouse over it, or it has keyboard focus.
        /// </summary>
        [Category("Brushes")]
        public IBrush? HighlightForeground { get => GetValue(HighlightForegroundProperty); set => SetValue(HighlightForegroundProperty, value); }

        /// <summary>The backing styled property for <see cref="HighlightForeground"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> HighlightForegroundProperty
            = AvaloniaProperty.Register<FlatButton, IBrush?>(nameof(HighlightForeground), Colors.Black.ToBrush());

        /// <summary>
        /// Get or set the brush used for the foreground while the control is selected (i.e., <c>IsSelected</c> is <c>true</c>).
        /// </summary>
        public IBrush? SelectedForeground { get => GetValue(SelectedForegroundProperty); set => SetValue(SelectedForegroundProperty, value); }

        /// <summary>The backing styled property for <see cref="SelectedForeground"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> SelectedForegroundProperty
            = AvaloniaProperty.Register<FlatButton, IBrush?>(nameof(SelectedForeground), Colors.Black.ToBrush());


        #endregion

        #region Border

        //public Thickness BorderThickness { get => GetValue(BorderThicknessProperty); set => SetValue(BorderThicknessProperty, value); }

        ///// <summary>The backing styled property for <see cref="BorderThickness"/>. See the related property for details.</summary>
        //public static readonly StyledProperty<Thickness> BorderThicknessProperty
        //    = AvaloniaProperty.Register<FlatButton, Thickness>(nameof(BorderThickness), new Thickness(1));

        /// <summary>
        /// Get or set the thickness of the board around the button, while the button is in a selected state (<c>IsSelected</c> is true).
        /// </summary>
        public Thickness BorderSelectionThickness { get => GetValue(BorderSelectionThicknessProperty); set => SetValue(BorderSelectionThicknessProperty, value); }

        /// <summary>The backing styled property for <see cref="BorderSelectionThickness"/>. See the related property for details.</summary>
        public static readonly StyledProperty<Thickness> BorderSelectionThicknessProperty
            = AvaloniaProperty.Register<FlatButton, Thickness>(nameof(BorderSelectionThickness), new Thickness(2));

        //public CornerRadius CornerRadius { get => GetValue(CornerRadiusProperty); set => SetValue(CornerRadiusProperty, value); }

        ///// <summary>The backing styled property for <see cref="CornerRadius"/>. See the related property for details.</summary>
        //public static readonly StyledProperty<CornerRadius> CornerRadiusProperty
        //    = AvaloniaProperty.Register<FlatButton, CornerRadius>(nameof(CornerRadius), new CornerRadius(0));

        #endregion

        #endregion

        /// <inheritdoc/>
        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            switch (change.Property.Name)
            {
                case nameof(UseAccentTheme):
                    if (SsuiTheme == null)
                    {
                        ApplyColorScheme(ColorScheme, UseAccentTheme);
                    }
                    break;
                case nameof(ColorScheme):
                    ApplyColorScheme(ColorScheme);
                    ColorSchemeChanged?.Invoke(this, change);
                    break;
                case nameof(CanSelect):
                    OnCanSelectChanged(change);
                    break;
            }
        }

        // in the future, I'll have to look into listening to Avalonia's Tapped and RightTapped events, rather than directly parsing
        // the pointer events myself. For now, though, I'm just focused on replicating the functionality in the WPF version

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
            = AvaloniaProperty.Register<FlatButton, bool>(nameof(SelectOnClick), false);

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
            = AvaloniaProperty.Register<FlatButton, bool>(nameof(CanSelect), true);

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
        public static readonly DirectProperty<FlatButton, bool> IsSelectedProperty
            = AvaloniaProperty.RegisterDirect<FlatButton, bool>(nameof(IsSelected), (fb) => fb.IsSelected, (fb, v) => fb.IsSelected = v);

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
            RoutedEvent.Register<FlatButton, ItemSelectionChangedEventArgs>(nameof(IsSelectedChanged), RoutingStrategies.Bubble);

        /// <summary>
        /// Defines the <see cref="CanSelectChanged"/> event.
        /// </summary>
        public static readonly RoutedEvent<RoutedEventArgs> CanSelectChangedEvent =
            RoutedEvent.Register<FlatButton, RoutedEventArgs>(nameof(CanSelectChanged), RoutingStrategies.Bubble);

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
            RoutedEvent.Register<FlatButton, RoutedEventArgs>(nameof(Click), RoutingStrategies.Bubble);

        /// <summary>
        /// Defines the <see cref="RightClick"/> event.
        /// </summary>
        public static readonly RoutedEvent<RoutedEventArgs> RightClickEvent =
            RoutedEvent.Register<FlatButton, RoutedEventArgs>(nameof(RightClick), RoutingStrategies.Bubble);

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
        /// The backing direct property for <see cref="IsPressedByKey"/>. See the related proeprty for details.
        /// </summary>
        public static readonly DirectProperty<FlatButton, bool> IsPressedByKeyProperty
            = AvaloniaProperty.RegisterDirect<FlatButton, bool>(nameof(IsPressedByKey), (fb) => fb.IsPressedByKey, unsetValue: false);

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

        #endregion

        #endregion

    }
}
