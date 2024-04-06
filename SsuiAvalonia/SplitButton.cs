using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace SolidShineUi
{
    /// <summary>
    /// A control that displays two buttons: a main button on the left which activates like a normal button, 
    /// and a secondary button on the right that displays a menu of additional options.
    /// </summary>
    public class SplitButton : ContentControl, IClickSelectableControl
    {
        /// <summary>
        /// Create a SplitButton.
        /// </summary>
        public SplitButton()
        {

        }

        #region Appearance

        #region Color Scheme

        /// <summary>
        /// Get or set the color scheme to apply to this button. The color scheme can quickly apply a whole visual style to your control.
        /// </summary>
        public ColorScheme ColorScheme { get => GetValue(ColorSchemeProperty); set => SetValue(ColorSchemeProperty, value); }

        /// <summary>The backing styled property for <see cref="ColorScheme"/>. See the related property for details.</summary>
        public static readonly StyledProperty<ColorScheme> ColorSchemeProperty = FlatButton.ColorSchemeProperty.AddOwner<SplitButton>();

        private void OnColorSchemeChanged(AvaloniaPropertyChangedEventArgs change)
        {
            Menu?.ApplyColorScheme(change.GetNewValue<ColorScheme>());
            ColorSchemeChanged?.Invoke(this, change);
        }

        private void MenuButton_Click(object? sender, RoutedEventArgs e)
        {
            if (Menu != null)
            {
                Menu.Placement = MenuPlacement;
                Menu.PlacementTarget = MenuPlacementTarget ?? this;
                Menu.PlacementRect = MenuPlacementRectangle;
                Menu.HorizontalOffset = MenuHorizontalOffset;
                Menu.VerticalOffset = MenuVerticalOffset;
                Menu.Open(this);
                Menu.Closing += Menu_Closing;
                Menu.Closed += Menu_Closed;
            }
        }

        /// <inheritdoc/>
        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            switch (change.Property.Name)
            {
                case nameof(UseAccentColors):
                    ApplyColorScheme(ColorScheme, UseAccentColors);
                    break;
                case nameof(ColorScheme):
                    ApplyColorScheme(ColorScheme);
                    OnColorSchemeChanged(change);
                    break;
            }

            base.OnPropertyChanged(change);
        }
        /// <summary>
        /// Raised when the <see cref="ColorScheme"/> property has changed.
        /// </summary>
        public event EventHandler<AvaloniaPropertyChangedEventArgs>? ColorSchemeChanged;

        #region Apply Color Scheme

        bool _internalAction = true;

        /// <summary>
        /// Apply a color scheme to this control, and set some other optional appearance settings. The color scheme can quickly apply a whole visual style to the control.
        /// </summary>
        /// <param name="cs">The color scheme to apply</param>
        public void ApplyColorScheme(ColorScheme cs)
        {
            ApplyColorScheme(cs, UseAccentColors);
        }

        /// <summary>
        /// Apply a color scheme to this control, and set some other optional appearance settings. The color scheme can quickly apply a whole visual style to the control.
        /// </summary>
        /// <param name="cs">The color scheme to apply</param>
        /// <param name="useAccentColors">Set if accent colors should be used for this button, rather than the main color scheme colors.
        /// This can also be achieved with the <c>UseAccentColors</c> property.
        /// </param>
        public void ApplyColorScheme(ColorScheme cs, bool useAccentColors = true)
        {
            if (ColorScheme != cs)
            {
                ColorScheme = cs;
                return;
            }

            if (UseAccentColors != useAccentColors)
            {
                UseAccentColors = useAccentColors;
                return;
            }

            _internalAction = false;

            if (cs.IsHighContrast)
            {
                Background = cs.BackgroundColor.ToBrush();
                HighlightBrush = cs.HighlightColor.ToBrush();
                SelectedBrush = cs.HighlightColor.ToBrush();
                BorderHighlightBrush = cs.BorderColor.ToBrush();
                BorderSelectedBrush = cs.BorderColor.ToBrush();
                BorderDisabledBrush = cs.DarkDisabledColor.ToBrush();
                SeparatorBrush = cs.BorderColor.ToBrush();
                DisabledBrush = cs.BackgroundColor.ToBrush();
                Foreground = cs.ForegroundColor.ToBrush();
                ClickBrush = cs.ThirdHighlightColor.ToBrush();
            }
            else
            {
                if (UseAccentColors)
                {
                    Background = cs.AccentSecondaryColor.ToBrush();
                    BorderBrush = cs.AccentBorderColor.ToBrush();
                    HighlightBrush = cs.AccentSecondHighlightColor.ToBrush();
                    DisabledBrush = cs.LightDisabledColor.ToBrush();
                    BorderDisabledBrush = cs.DarkDisabledColor.ToBrush();
                    SelectedBrush = cs.AccentThirdHighlightColor.ToBrush();
                    BorderHighlightBrush = cs.AccentHighlightColor.ToBrush();
                    BorderSelectedBrush = cs.AccentSelectionColor.ToBrush();
                    SeparatorBrush = cs.AccentBorderColor.ToBrush();
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
                    SeparatorBrush = cs.BorderColor.ToBrush();
                    Foreground = cs.ForegroundColor.ToBrush();
                    ClickBrush = cs.ThirdHighlightColor.ToBrush();
                }
            }
        }

        #endregion

        #endregion

        /// <summary>
        /// Get or set whether the button should have a transparent background when the button is not focused.
        /// </summary>
        public bool TransparentBack { get => GetValue(TransparentBackProperty); set => SetValue(TransparentBackProperty, value); }

        /// <summary>The backing styled property for <see cref="TransparentBack"/>. See the related property for details.</summary>
        public static readonly StyledProperty<bool> TransparentBackProperty = FlatButton.TransparentBackProperty.AddOwner<SplitButton>();

        /// <summary>
        /// Get or set if the button should use the accent brushes of the color scheme, rather than the standard brushes.
        /// </summary>
        public bool UseAccentColors { get => GetValue(UseAccentColorsProperty); set => SetValue(UseAccentColorsProperty, value); }

        /// <summary>The backing styled property for <see cref="UseAccentColors"/>. See the related property for details.</summary>
        public static readonly StyledProperty<bool> UseAccentColorsProperty = FlatButton.UseAccentColorsProperty.AddOwner<SplitButton>();

        #region Brushes

        /// <summary>
        /// Get or set the brush used for the background of the control while the mouse/pointer is clicking it.
        /// </summary>
        [Category("Brushes")]
        public IBrush? ClickBrush { get => GetValue(ClickBrushProperty); set => SetValue(ClickBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="ClickBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> ClickBrushProperty = FlatButton.ClickBrushProperty.AddOwner<SplitButton>();

        /// <summary>
        /// Get or set the brush used for the background of the button while it is selected
        /// (i.e. the <c>IsSelected</c> property is true).
        /// </summary>
        [Category("Brushes")]
        public IBrush? SelectedBrush { get => GetValue(SelectedBrushProperty); set => SetValue(SelectedBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="SelectedBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> SelectedBrushProperty = FlatButton.SelectedBrushProperty.AddOwner<SplitButton>();

        /// <summary>
        /// Get or set the brush used for the background of the control while the mouse/pointer is over it, or it has keyboard focus.
        /// </summary>
        [Category("Brushes")]
        public IBrush? HighlightBrush { get => GetValue(HighlightBrushProperty); set => SetValue(HighlightBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="HighlightBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> HighlightBrushProperty = FlatButton.HighlightBrushProperty.AddOwner<SplitButton>();

        /// <summary>
        /// Get or set the brush used for the background of the control when it is disabled.
        /// </summary>
        [Category("Brushes")]
        public IBrush? DisabledBrush { get => GetValue(DisabledBrushProperty); set => SetValue(DisabledBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="DisabledBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> DisabledBrushProperty = FlatButton.DisabledBrushProperty.AddOwner<SplitButton>();

        /// <summary>
        /// Get or set the brush used for the border around the control, when it is disabled.
        /// </summary>
        public IBrush? BorderDisabledBrush { get => GetValue(BorderDisabledBrushProperty); set => SetValue(BorderDisabledBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="BorderDisabledBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> BorderDisabledBrushProperty = FlatButton.BorderDisabledBrushProperty.AddOwner<SplitButton>();

        /// <summary>
        /// Get or set the brush used for the border while the control has the mouse/pointer over it (or it has keyboard focus).
        /// </summary>
        [Category("Brushes")]
        public IBrush? BorderHighlightBrush { get => GetValue(BorderHighlightBrushProperty); set => SetValue(BorderHighlightBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="BorderHighlightBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> BorderHighlightBrushProperty = FlatButton.BorderHighlightBrushProperty.AddOwner<SplitButton>();

        /// <summary>
        /// Get or set the brush used for the border while the control is selected
        /// (i.e. the <c>IsSelected</c> property is true).
        /// </summary>
        [Category("Brushes")]
        public IBrush? BorderSelectedBrush { get => GetValue(BorderSelectedBrushProperty); set => SetValue(BorderSelectedBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="BorderSelectedBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> BorderSelectedBrushProperty = FlatButton.BorderSelectedBrushProperty.AddOwner<SplitButton>();

        #endregion

        #region Border

        /// <summary>
        /// Get or set the thickness of the board around the button, while the button is in a selected state (<c>IsSelected</c> is true).
        /// </summary>
        public Thickness BorderSelectionThickness { get => GetValue(BorderSelectionThicknessProperty); set => SetValue(BorderSelectionThicknessProperty, value); }

        /// <summary>The backing styled property for <see cref="BorderSelectionThickness"/>. See the related property for details.</summary>
        public static readonly StyledProperty<Thickness> BorderSelectionThicknessProperty = FlatButton.BorderSelectionThicknessProperty.AddOwner<SplitButton>();

        #region Separator Border

        /// <summary>
        /// Get or set if a separator bar should be shown between the main and menu buttons. Without the separator bar, the buttons look more connected, 
        /// but they can be hard to discern as two separate clickable buttons without mousing over them.
        /// </summary>
        public bool ShowSeparator { get => GetValue(ShowSeparatorProperty); set => SetValue(ShowSeparatorProperty, value); }

        /// <summary>The backing styled property for <see cref="ShowSeparator"/>. See the related property for details.</summary>
        public static readonly StyledProperty<bool> ShowSeparatorProperty
            = AvaloniaProperty.Register<SplitButton, bool>(nameof(ShowSeparator), true);

        /// <summary>
        /// Get or set the brush to use for the separator bar between the main and menu buttons.
        /// </summary>
        public IBrush? SeparatorBrush { get => GetValue(SeparatorBrushProperty); set => SetValue(SeparatorBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="SeparatorBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> SeparatorBrushProperty
            = AvaloniaProperty.Register<SplitButton, IBrush?>(nameof(SeparatorBrush), Colors.Gray.ToBrush());

        /// <summary>
        /// Get or set how distant the separator bar should be from the edges of the button. The higher the number, the bigger the space between
        /// the edges of the button and the ends of the separator bar. A value of 0 will bring the separator bar right up to the edges.
        /// </summary>
        public double SeparatorEdgeMargin { get => GetValue(SeparatorEdgeMarginProperty); set => SetValue(SeparatorEdgeMarginProperty, value); }

        /// <summary>The backing styled property for <see cref="SeparatorEdgeMargin"/>. See the related property for details.</summary>
        public static readonly StyledProperty<double> SeparatorEdgeMarginProperty
            = AvaloniaProperty.Register<SplitButton, double>(nameof(SeparatorEdgeMargin), 4.0);

        /// <summary>
        /// Get or set how wide the separator bar should be. A value of 0 will make it invisible.
        /// </summary>
        public double SeparatorThickness { get => GetValue(SeparatorThicknessProperty); set => SetValue(SeparatorThicknessProperty, value); }

        /// <summary>The backing styled property for <see cref="SeparatorThickness"/>. See the related property for details.</summary>
        public static readonly StyledProperty<double> SeparatorThicknessProperty
            = AvaloniaProperty.Register<SplitButton, double>(nameof(SeparatorThickness), 1.0);

        #endregion

        #endregion

        #endregion

        #region Menu

        /// <summary>
        /// Get or set the menu that appears when the button is clicked.
        /// </summary>
        public ContextMenu? Menu { get => GetValue(MenuProperty); set => SetValue(MenuProperty, value); }

        /// <summary>The backing styled property for <see cref="Menu"/>. See the related property for details.</summary>
        public static readonly StyledProperty<ContextMenu?> MenuProperty
            = AvaloniaProperty.Register<SplitButton, ContextMenu?>(nameof(Menu), null);

        /// <summary>
        /// This event is raised when this MenuButtons's menu is closed.
        /// </summary>
        public EventHandler? MenuClosed;

        private void Menu_Closing(object? sender, CancelEventArgs e)
        {
            if (_staysOpen) { e.Cancel = true; }
        }

        private void Menu_Closed(object? sender, RoutedEventArgs e)
        {
            MenuClosed?.Invoke(this, e);
        }

        private bool _staysOpen = false;

        /// <summary>
        /// Get or set if the menu should close automatically. If <c>true</c>, the menu will continue to remain open until this is changed back to <c>false</c>.
        /// </summary>
        /// <remarks>
        /// When this is set to <c>false</c>, the menu will close when a menu item is selected, or when the user clicks outside of the menu or moves focus.
        /// Due to the differences between how Avalonia and WPF handle their context menus, this functions a bit differently in the two versions when set to <c>true</c>.
        /// <para/>
        /// In the WPF version of Solid Shine UI, <c>StaysOpen</c> will keep the context menu open until a menu item is clicked (unless the menu item also has <c>StaysOpenOnClick</c>
        /// set to true), but other methods to close the menu don't generally work (such as clicking outside of the menu). So the best ways to close the menu is to have a "Close" menu
        /// item that doesn't have <c>StaysOpenOnClick</c> applied, or some other code or function that directly sets the menu's <c>IsOpen</c> property to <c>false</c>. You do not
        /// need to explicitly change <c>StaysOpen</c> to <c>false</c> in order to allow the menu to close, so this value can remain unchanged as long as you want this behavior.
        /// <para/>
        /// In the Avalonia version of Solid Shine UI, <c>StaysOpen</c> will set the context menu to refuse to close; when the context menu is about to close, the MenuButton cancels 
        /// that action. This also means that clicking any menu items will not close the menu, nor will setting the menu's <c>IsOpen</c> property to <c>false</c>.
        /// Instead, to close the context menu, you will need to change <c>StaysOpen</c> back to <c>false</c> before you change <c>IsOpen</c> to <c>false</c> or click out of the menu,
        /// and then you will need to re-set <c>StaysOpen</c> back to <c>true</c> whenever you want this behavior to occur again.
        /// </remarks>
        public bool StaysOpen { get => _staysOpen; set => SetAndRaise(StaysOpenProperty, ref _staysOpen, value); }

        /// <summary>The backing direct property for <see cref="StaysOpen"/>. See the related property for details.</summary>
        public static readonly DirectProperty<SplitButton, bool> StaysOpenProperty
            = AvaloniaProperty.RegisterDirect<SplitButton, bool>(nameof(StaysOpen), (s) => s.StaysOpen, (s, v) => s.StaysOpen = v, unsetValue: false);

        #region Placement

        /// <summary>
        /// Get or set the placement mode for the MenuButton's menu.
        /// </summary>
        public PlacementMode MenuPlacement { get => GetValue(MenuPlacementProperty); set => SetValue(MenuPlacementProperty, value); }

        /// <summary>The backing styled property for <see cref="MenuPlacement"/>. See the related property for details.</summary>
        public static readonly StyledProperty<PlacementMode> MenuPlacementProperty = MenuButton.MenuPlacementProperty.AddOwner<SplitButton>();

        /// <summary>
        /// Get or set the placement target for the MenuButton's menu. Set to <c>null</c> to set the target to this MenuButton.
        /// </summary>
        public Control? MenuPlacementTarget { get => GetValue(MenuPlacementTargetProperty); set => SetValue(MenuPlacementTargetProperty, value); }

        /// <summary>The backing styled property for <see cref="MenuPlacementTarget"/>. See the related property for details.</summary>
        public static readonly StyledProperty<Control?> MenuPlacementTargetProperty = MenuButton.MenuPlacementTargetProperty.AddOwner<SplitButton>();

        /// <summary>
        /// Get or set the placement rectangle for the MenuButton's menu. This sets the area relative to the button that the menu is positioned.
        /// </summary>
        public Rect? MenuPlacementRectangle { get => GetValue(MenuPlacementRectangleProperty); set => SetValue(MenuPlacementRectangleProperty, value); }

        /// <summary>The backing styled property for <see cref="MenuPlacementRectangle"/>. See the related property for details.</summary>
        public static readonly StyledProperty<Rect?> MenuPlacementRectangleProperty = MenuButton.MenuPlacementRectangleProperty.AddOwner<SplitButton>();

        /// <summary>
        /// Get or set how far offset the menu is horizontally (left or right) from its placement target/rectangle when it's opened.
        /// </summary>
        public double MenuHorizontalOffset { get => GetValue(MenuHorizontalOffsetProperty); set => SetValue(MenuHorizontalOffsetProperty, value); }

        /// <summary>The backing styled property for <see cref="MenuHorizontalOffset"/>. See the related property for details.</summary>
        public static readonly StyledProperty<double> MenuHorizontalOffsetProperty = MenuButton.MenuHorizontalOffsetProperty.AddOwner<SplitButton>();


        /// <summary>
        /// Get or set how far offset the menu is vertically (up or down) from its placement target/rectangle when it's opened.
        /// </summary>
        public double MenuVerticalOffset { get => GetValue(MenuVerticalOffsetProperty); set => SetValue(MenuVerticalOffsetProperty, value); }

        /// <summary>The backing styled property for <see cref="MenuVerticalOffset"/>. See the related property for details.</summary>
        public static readonly StyledProperty<double> MenuVerticalOffsetProperty = MenuButton.MenuVerticalOffsetProperty.AddOwner<SplitButton>();


        #endregion

        #endregion

        #region Button Handling

        private void btnMain_Click(object? sender, RoutedEventArgs e)
        {
            OnClick();
        }

        private void btnMenu_Click(object? sender, RoutedEventArgs e)
        {
            if (Menu != null)
            {
                Menu.Placement = MenuPlacement;
                Menu.PlacementTarget = MenuPlacementTarget ?? this;
                Menu.PlacementRect = MenuPlacementRectangle;
                Menu.HorizontalOffset = 0;
                Menu.VerticalOffset = -1;
                Menu.Open();
                Menu.Closed += Menu_Closed;
            }

            RoutedEventArgs rre = new RoutedEventArgs(MenuClickEvent);
            RaiseEvent(rre);
        }

        private void btnMain_RightClick(object? sender, RoutedEventArgs e)
        {
            PerformRightClick();
        }

        private void btnMenu_RightClick(object? sender, RoutedEventArgs e)
        {
            PerformRightClick();
        }

        #region Menu Button Layout

        /// <summary>
        /// Get or set where the menu button should be placed in relation to the main button. Default is <c>Right</c>.
        /// </summary>
        /// <remarks>
        /// If you want to set this value to <c>Top</c> or <c>Bottom</c>, also consider increasing the height of the button or decreasing the value of <see cref="MenuButtonSize"/>.
        /// If you set this value to <c>Hidden</c>, the menu button is entirely hidden (and no longer accessible via the mouse).
        /// </remarks>
        public PlacementDirection MenuButtonPlacement { get => GetValue(MenuButtonPlacementProperty); set => SetValue(MenuButtonPlacementProperty, value); }

        /// <summary>The backing styled property for <see cref="MenuButtonPlacement"/>. See the related property for details.</summary>
        public static readonly StyledProperty<PlacementDirection> MenuButtonPlacementProperty
            = AvaloniaProperty.Register<SplitButton, PlacementDirection>(nameof(MenuButtonPlacement), PlacementDirection.Right);

        /// <summary>
        /// Get or set the width or height of the menu button. Larger values make the button wider or taller, and takes up a larger portion of the overall control. Default is <c>20</c>.
        /// </summary>
        /// <remarks>
        /// When <see cref="MenuButtonPlacement"/> is set to <c>Left</c> or <c>Right</c>, this affects the width of the menu button.
        /// When it is set to <c>Top</c> or <c>Bottom</c>, this affects the height of the menu button. When it is set to <c>Hidden</c>, this has no effect.
        /// </remarks>
        public double MenuButtonSize { get => GetValue(MenuButtonSizeProperty); set => SetValue(MenuButtonSizeProperty, value); }

        /// <summary>The backing styled property for <see cref="MenuButtonSize"/>. See the related property for details.</summary>
        public static readonly StyledProperty<double> MenuButtonSizeProperty
            = AvaloniaProperty.Register<SplitButton, double>(nameof(MenuButtonSize), 20.0);

        #endregion
        
        #endregion

        #region Click / Selection Handling

        #region Routed Events

        /// <summary>
        /// The backing value for the <see cref="Click"/> event. See the related event for more details.
        /// </summary>
        public static readonly RoutedEvent ClickEvent = RoutedEvent.Register<RoutedEventArgs>(
            "Click", RoutingStrategies.Bubble, typeof(SplitButton));

        /// <summary>
        /// Raised when the user clicks on the main button (not the menu button), via a mouse click or via the keyboard.
        /// </summary>
        public event EventHandler<RoutedEventArgs> Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }

        /// <summary>
        /// The backing value for the <see cref="MenuClick"/> event. See the related event for more details.
        /// </summary>
        public static readonly RoutedEvent MenuClickEvent = RoutedEvent.Register<RoutedEventArgs>(
            "MenuClick", RoutingStrategies.Bubble, typeof(SplitButton));

        /// <summary>
        /// Raised when the user clicks on the menu button (not the main button), via a mouse click or via the keyboard.
        /// </summary>
        public event EventHandler<RoutedEventArgs> MenuClick
        {
            add { AddHandler(MenuClickEvent, value); }
            remove { RemoveHandler(MenuClickEvent, value); }
        }

        /// <summary>
        /// The backing value for the <see cref="RightClick"/> event. See the related event for more details.
        /// </summary>
        public static readonly RoutedEvent RightClickEvent = RoutedEvent.Register<RoutedEventArgs>(
            "RightClick", RoutingStrategies.Bubble, typeof(SplitButton));

        /// <summary>
        /// Raised when the user right-clicks on the button, via a mouse click or via the keyboard.
        /// </summary>
        public event EventHandler<RoutedEventArgs> RightClick
        {
            add { AddHandler(RightClickEvent, value); }
            remove { RemoveHandler(RightClickEvent, value); }
        }

        /// <summary>
        /// Defines the <see cref="IsSelectedChanged"/> event.
        /// </summary>
        public static readonly RoutedEvent<ItemSelectionChangedEventArgs> IsSelectedChangedEvent =
            RoutedEvent.Register<SplitButton, ItemSelectionChangedEventArgs>(nameof(IsSelectedChanged), RoutingStrategies.Bubble);

        /// <summary>
        /// Raised when the user clicks the button with the right mouse.
        /// </summary>
        public event EventHandler<ItemSelectionChangedEventArgs> IsSelectedChanged
        {
            add { AddHandler(IsSelectedChangedEvent, value); }
            remove { RemoveHandler(IsSelectedChangedEvent, value); }
        }

        /// <summary>
        /// Defines the <see cref="CanSelectChanged"/> event.
        /// </summary>
        public static readonly RoutedEvent<RoutedEventArgs> CanSelectChangedEvent =
            RoutedEvent.Register<SplitButton, RoutedEventArgs>(nameof(CanSelectChanged), RoutingStrategies.Bubble);

        /// <summary>
        /// Raised when the user right-clicks on the button, via a mouse click or via the keyboard.
        /// </summary>
        public event EventHandler<RoutedEventArgs> CanSelectChanged
        {
            add { AddHandler(CanSelectChangedEvent, value); }
            remove { RemoveHandler(CanSelectChangedEvent, value); }
        }

        #endregion

        #region Selection Properties

        bool sel = false;

        /// <summary>
        /// Gets or sets whether this button is selected. This property (combined with <c>SelectOnClick</c>) allows the button to function like a ToggleButton.
        /// </summary>
        /// <remarks>
        /// A selected button will have a slightly different visual appearance to differentiate it as being selected. This will include, by default, the border being a bit thicker.
        /// This can be changed via the <see cref="BorderSelectionThickness"/> property. You can also directly edit the brushes used via the <see cref="SelectedBrush"/> and
        /// <see cref="BorderSelectedBrush"/> properties.
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

        /// <summary>The backing direct property for <see cref="IsSelected"/>. See the related property for details.</summary>
        public static readonly DirectProperty<SplitButton, bool> IsSelectedProperty
            = FlatButton.IsSelectedProperty.AddOwner<SplitButton>((s) => s.IsSelected, (s, v) => s.IsSelected = v, unsetValue: false);


        /// <summary>
        /// Gets or sets whether the button should change its IsSelected property when a click is performed. With this enabled, this allows the button to take on the functionality of a ToggleButton.
        /// </summary>
        /// <remarks>
        /// While SelectOnClick is true, the button will toggle between <see cref="IsSelected"/> being true and false (similar to a ToggleButton). A selected button will, by default, have some visual
        /// differences to help make it look distinct from unselected buttons. The button's Click event will still be raised while this property is set to <c>true</c>, but the event occurs after the
        /// IsSelected property has already changed. While you could use the Click event to check when the button's IsSelected property is changed, it is better to use the IsSelectedChanged event,
        /// in case of situations where IsSelected is changed via methods other than clicking, such as programmatically or via WPF binding.
        /// </remarks>
        public bool SelectOnClick { get => GetValue(SelectOnClickProperty); set => SetValue(SelectOnClickProperty, value); }

        /// <summary>The backing styled property for <see cref="SelectOnClick"/>. See the related property for details.</summary>
        public static readonly StyledProperty<bool> SelectOnClickProperty
            = FlatButton.SelectOnClickProperty.AddOwner<SplitButton>();

        /// <summary>
        /// Get or set if this control can be selected.
        /// </summary>
        /// <remarks>
        /// If this is set to <c>false</c>, then this control cannot be selected via any method - even programmatically. Setting this to <c>false</c> will also deselect this control, 
        /// if currently selected. For more fine-tuned control, you can use <see cref="SelectOnClick"/> to limit how the user can select this control, 
        /// while still being able to change the selection status via <see cref="IsSelected"/>.
        /// </remarks>
        public bool CanSelect { get => GetValue(CanSelectProperty); set => SetValue(CanSelectProperty, value); }

        /// <summary>The backing styled property for <see cref="CanSelect"/>. See the related property for details.</summary>
        public static readonly StyledProperty<bool> CanSelectProperty
            = FlatButton.CanSelectProperty.AddOwner<SplitButton>();


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

        #endregion


        // If the button is prepared by PerformPress, perform the Click actions, including raising the Click event.
        void PerformRightClick()
        {
            RoutedEventArgs rre = new RoutedEventArgs(RightClickEvent);
            RaiseEvent(rre);
        }

        /// <summary>
        /// Perform a click on the main button programmatically. The button responds the same way as if it was clicked by the user.
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
                SetIsSelectedWithSource(!IsSelected, SelectionChangeTrigger.ControlClick, this);
            }

            RoutedEventArgs rre = new RoutedEventArgs(ClickEvent);
            RaiseEvent(rre);
        }

        #endregion

        #region Template IO

        /// <inheritdoc/>
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            LoadTemplateItems(e);

            if (itemsLoaded)
            {
                btnMain!.Click += btnMain_Click;
                btnMenu!.Click += btnMenu_Click;

                btnMain.RightClick += btnMain_RightClick;
                btnMenu.RightClick += btnMenu_RightClick;
            }
        }

        bool itemsLoaded = false;

        FlatButton? btnMain = null;
        FlatButton? btnMenu = null;

        void LoadTemplateItems(TemplateAppliedEventArgs e)
        {
            if (!itemsLoaded)
            {
                btnMain = e.NameScope.Find<FlatButton>("PART_Main");
                btnMenu = e.NameScope.Find<FlatButton>("PART_Menu");

                if (btnMain != null && btnMenu != null)
                {
                    itemsLoaded = true;
                }
            }
        }

        #endregion

    }
}
