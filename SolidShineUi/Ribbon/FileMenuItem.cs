using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using SolidShineUi.Utils;

namespace SolidShineUi.Ribbon
{
    /// <summary>
    /// An item that can be added to a <see cref="RibbonFileMenu"/>.
    /// </summary>
    public class FileMenuItem : ThemedControl, IClickSelectableControl, IRibbonItem
    {
        static FileMenuItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FileMenuItem), new FrameworkPropertyMetadata(typeof(FileMenuItem)));
        }

        /// <summary>
        /// Create a FileMenuItem.
        /// </summary>
        public FileMenuItem()
        {

        }

        #region Color Scheme

        /// <summary>
        /// Raised when the ColorScheme property is changed.
        /// </summary>
#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? ColorSchemeChanged;
#else
        public event DependencyPropertyChangedEventHandler ColorSchemeChanged;
#endif

        /// <summary>
        /// A dependency property object backing the related ColorScheme property. See <see cref="ColorScheme"/> for more details.
        /// </summary>
        public static readonly DependencyProperty ColorSchemeProperty
            = DependencyProperty.Register("ColorScheme", typeof(ColorScheme), typeof(FileMenuItem),
            new FrameworkPropertyMetadata(new ColorScheme(), new PropertyChangedCallback(OnColorSchemeChanged)));

        /// <summary>
        /// Perform an action when the ColorScheme property has changed. Primarily used internally.
        /// </summary>
        /// <param name="d">The object containing the property that changed.</param>
        /// <param name="e">Event arguments about the property change.</param>
        public static void OnColorSchemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
#if NETCOREAPP
            ColorScheme cs = (e.NewValue as ColorScheme)!;
#else
            ColorScheme cs = e.NewValue as ColorScheme;
#endif

            if (d is FileMenuItem c)
            {
                c.ColorSchemeChanged?.Invoke(d, e);
                c.ApplyColorScheme(cs);
            }
        }

        /// <summary>
        /// Get or set the color scheme used for this control. The color scheme can quickly apply a whole visual style to your control.
        /// </summary>
        public ColorScheme ColorScheme
        {
            get => (ColorScheme)GetValue(ColorSchemeProperty);
            set => SetValue(ColorSchemeProperty, value);
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

            Background = Color.FromArgb(1, 0, 0, 0).ToBrush();
            BorderBrush = Color.FromArgb(1, 0, 0, 0).ToBrush();

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

                BorderBrush = Color.FromArgb(1, 0, 0, 0).ToBrush();
            }
            else
            {
                Background = Color.FromArgb(1, 0, 0, 0).ToBrush();
                BorderBrush = Color.FromArgb(1, 0, 0, 0).ToBrush();

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

        #region SsuiTheme

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

            void ApplyTheme(SsuiTheme theme)
            {
                ApplyThemeBinding(HighlightBrushProperty, SsuiTheme.HighlightBrushProperty, theme);
                ApplyThemeBinding(ClickBrushProperty, SsuiTheme.ClickBrushProperty, theme);
                ApplyThemeBinding(BorderHighlightBrushProperty, SsuiTheme.HighlightBorderBrushProperty, theme);
                ApplyThemeBinding(BorderDisabledBrushProperty, SsuiTheme.DisabledBorderBrushProperty, theme);
                ApplyThemeBinding(BorderSelectedBrushProperty, SsuiTheme.SelectedBorderBrushProperty, theme);
                ApplyThemeBinding(SelectedBrushProperty, SsuiTheme.SelectedBackgroundBrushProperty, theme);
                ApplyThemeBinding(DisabledBrushProperty, SsuiTheme.DisabledBackgroundProperty, theme);
            }
        }

        #endregion

        #region Buttons

        // TODO: add in buttons to template and then here

        private static void ApplyPropertyUpdate(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FileMenuItem fmi)
            {
                fmi.ApplyValueToButtons(e.Property, e.NewValue);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", 
            Justification = "yet to implement")]
        private void ApplyValueToButtons(DependencyProperty property, object value)
        {
            //btnMain?.SetValue(property, value);
            //btnMenu?.SetValue(property, value);
        }

        #endregion

        #region Appearance

        /// <summary>
        /// Get or set if this FileMenuItem should be a split item; if <c>true</c>,
        /// then there will be a separate main button and menu button.
        /// </summary>
        public bool IsSplit { get => (bool)GetValue(IsSplitProperty); set => SetValue(IsSplitProperty, value); }

        /// <summary>The backing dependency property for <see cref="IsSplit"/>. See the related property for details.</summary>
        public static readonly DependencyProperty IsSplitProperty
            = DependencyProperty.Register(nameof(IsSplit), typeof(bool), typeof(FileMenuItem),
            new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Get or set the corner radius for the border around the item.
        /// </summary>
        public CornerRadius CornerRadius { get => (CornerRadius)GetValue(CornerRadiusProperty); set => SetValue(CornerRadiusProperty, value); }

        /// <summary>The backing dependency property for <see cref="CornerRadius"/>. See the related property for details.</summary>
        public static DependencyProperty CornerRadiusProperty
            = DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(FileMenuItem),
            new FrameworkPropertyMetadata(new CornerRadius(0)));

        #endregion

        #region Menu

        /// <summary>The backing dependency property for <see cref="Menu"/>. See the related property for details.</summary>
        public static readonly DependencyProperty MenuProperty
            = MenuButton.MenuProperty.AddOwner(typeof(FileMenuItem),
            new FrameworkPropertyMetadata(null, (d, e) => d.PerformAs<FileMenuItem>((o) => o.OnMenuChanged(e))));

        /// <summary>
        /// Raised when the <see cref="Menu"/> property is changed.
        /// </summary>
#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? MenuChanged;
#else
        public event DependencyPropertyChangedEventHandler MenuChanged;
#endif

        private void OnMenuChanged(DependencyPropertyChangedEventArgs e)
        {
            MenuChanged?.Invoke(this, e);
            HasMenu = Menu != null;
        }


#if NETCOREAPP
        /// <summary>
        /// Get or set the menu that appears when the button is clicked. Set to null to not have a menu.
        /// </summary>
        [Category("Common")]
        [Description("Get or set the menu that appears when the button is clicked. Set to null to not have a menu.")]
        public ContextMenu? Menu
        {
            get { return (ContextMenu)GetValue(MenuProperty); }
            set { SetValue(MenuProperty, value); }
        }

        /// <summary>
        /// This event is raised when this MenuButton's menu is about to open.
        /// </summary>
        public event CancelEventHandler? MenuOpening;

        /// <summary>
        /// This event is raised when this MenuButton's menu has been opened.
        /// </summary>
        public event EventHandler? MenuOpened;

        /// <summary>
        /// This event is raised when this MenuButton's menu has been closed.
        /// </summary>
        public event EventHandler? MenuClosed;

#else
        /// <summary>
        /// Get or set the menu that appears when the button is clicked. Set to null to not have a menu.
        /// </summary>
        [Category("Common")]
        [Description("Get or set the menu that appears when the button is clicked. Set to null to not have a menu.")]
        public ContextMenu Menu
        {
            get { return (ContextMenu)GetValue(MenuProperty); }
            set { SetValue(MenuProperty, value); }
        }

        /// <summary>
        /// This event is raised when this MenuButton's menu is about to open.
        /// </summary>
        public event CancelEventHandler MenuOpening;

        /// <summary>
        /// This event is raised when this MenuButton's menu has been opened.
        /// </summary>
        public event EventHandler MenuOpened;

        /// <summary>
        /// This event is raised when this MenuButton's menu has been closed.
        /// </summary>
        public event EventHandler MenuClosed;
#endif

        /// <summary>
        /// Get or set if this RibbonFileItem has a value set for its <c>Menu</c> property.
        /// </summary>
        public bool HasMenu { get => (bool)GetValue(HasMenuProperty); private set => SetValue(HasMenuPropertyKey, value); }

        private static readonly DependencyPropertyKey HasMenuPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(HasMenu), typeof(bool), typeof(FileMenuItem),
            new FrameworkPropertyMetadata(false));

        /// <summary>The backing dependency property for <see cref="HasMenu"/>. See the related property for details.</summary>
        public static readonly DependencyProperty HasMenuProperty = HasMenuPropertyKey.DependencyProperty;

        /// <summary>
        /// Get or set if the menu should close automatically upon selecting an item. Remember to set the <c>StaysOpenOnClick</c> property for child menu items as well.
        /// </summary>
        /// <remarks>
        /// When this is set to <c>false</c>, the menu will close when a menu item is selected, or when the user clicks outside of the menu or moves focus.
        /// Due to the differences between how Avalonia and WPF handle their context menus, this functions a bit differently in the two versions when set to <c>true</c>.
        /// <para/>
        /// In the WPF version of Solid Shine UI, <c>StaysOpen</c> will keep the context menu open until a menu item is clicked (unless the menu item also has <c>StaysOpenOnClick</c>
        /// set to true), but other methods to close the menu don't generally work (such as clicking outside of the menu). So the best ways to close the menu is to have a "Close" menu
        /// item that doesn't have <c>StaysOpenOnClick</c> applied, or some other code or function that directly sets the menu's <c>IsOpen</c> property to <c>false</c>. You do not
        /// need to explicitly change <c>StaysOpen</c> to <c>false</c> in order to allow the menu to close via explicity setting the property, so this value can remain unchanged as 
        /// long as you want this behavior.
        /// </remarks>
        [Category("Common")]
        [Description("Get or set if the menu should close automatically upon selecting an item.")]
        public bool StaysOpen
        {
            get
            {
                if (Menu != null) return Menu.StaysOpen;
                else return false;
            }
            set
            {
                if (Menu != null) Menu.StaysOpen = value;
            }
        }

        // /// <para/>
        // /// In the Avalonia version of Solid Shine UI, <c>StaysOpen</c> will set the context menu to refuse to close; when the context menu is about to close, the MenuButton cancels 
        // /// that action. This also means that clicking any menu items will not close the menu, nor will setting the menu's <c>IsOpen</c> property to <c>false</c>.
        // /// Instead, to close the context menu, you will need to change <c>StaysOpen</c> back to <c>false</c> before you change <c>IsOpen</c> to <c>false</c> or click out of the menu,
        // /// and then you will need to re-set <c>StaysOpen</c> back to <c>true</c> whenever you want this behavior to occur again.

        private void Menu_Closed(object sender, RoutedEventArgs e)
        {
            MenuClosed?.Invoke(this, EventArgs.Empty);
        }

        #region Placement

        /// <summary>
        /// Get or set the placement mode for the MenuButton's menu.
        /// </summary>
        [Category("Appearance")]
        [Description("Get or set the placement mode for the MenuButton's menu.")]
        public PlacementMode MenuPlacement { get => (PlacementMode)GetValue(MenuPlacementProperty); set => SetValue(MenuPlacementProperty, value); }

        /// <summary>The backing dependency property for <see cref="MenuPlacement"/>. See the related property for details.</summary>
        public static readonly DependencyProperty MenuPlacementProperty
            = MenuButton.MenuPlacementProperty.AddOwner(typeof(FileMenuItem),
            new FrameworkPropertyMetadata(PlacementMode.Bottom));


        /// <summary>
        /// Get or set the placement target for the MenuButton's menu. Set to <c>null</c> to set the target to this MenuButton.
        /// </summary>
        [Category("Appearance")]
        [Description("Get or set the placement target for the MenuButton's menu. Set to null to set the target to this MenuButton.")]
#if NETCOREAPP
        public UIElement? MenuPlacementTarget { get => (UIElement)GetValue(MenuPlacementTargetProperty); set => SetValue(MenuPlacementTargetProperty, value); }
#else
        public UIElement MenuPlacementTarget { get => (UIElement)GetValue(MenuPlacementTargetProperty); set => SetValue(MenuPlacementTargetProperty, value); }
#endif

        /// <summary>The backing dependency property for <see cref="MenuPlacementTarget"/>. See the related property for details.</summary>
        public static readonly DependencyProperty MenuPlacementTargetProperty
            = MenuButton.MenuPlacementTargetProperty.AddOwner(typeof(FileMenuItem),
            new FrameworkPropertyMetadata(null));


        /// <summary>
        /// Get or set the placement rectangle for the MenuButton's menu. This sets the area relative to the button that the menu is positioned.
        /// </summary>
        [Category("Appearance")]
        [Description("Get or set the placement rectangle for the MenuButton's menu.")]
        public Rect MenuPlacementRectangle { get => (Rect)GetValue(MenuPlacementRectangleProperty); set => SetValue(MenuPlacementRectangleProperty, value); }

        /// <summary>The backing dependency property for <see cref="MenuPlacementRectangle"/>. See the related property for details.</summary>
        public static readonly DependencyProperty MenuPlacementRectangleProperty
            = MenuButton.MenuPlacementRectangleProperty.AddOwner(typeof(FileMenuItem),
            new FrameworkPropertyMetadata(Rect.Empty));

        /// <summary>
        /// Get or set how far offset the menu is horizontally (left or right) from its placement target/rectangle when it's opened.
        /// </summary>
        [Category("Appearance")]
        [Description("Get or set how far offset the menu is horizontally from its placement target/rectangle when it's opened.")]
        public double MenuHorizontalOffset { get => (double)GetValue(MenuHorizontalOffsetProperty); set => SetValue(MenuHorizontalOffsetProperty, value); }

        /// <summary>The backing dependency property for <see cref="MenuHorizontalOffset"/>. See the related property for details.</summary>
        public static readonly DependencyProperty MenuHorizontalOffsetProperty
            = MenuButton.MenuHorizontalOffsetProperty.AddOwner(typeof(FileMenuItem),
            new FrameworkPropertyMetadata(0.0));

        /// <summary>
        /// Get or set how far offset the menu is vertically (up or down) from its placement target/rectangle when it's opened.
        /// </summary>
        [Category("Appearance")]
        [Description("Get or set how far offset the menu is vertically from its placement target/rectangle when it's opened.")]
        public double MenuVerticalOffset { get => (double)GetValue(MenuVerticalOffsetProperty); set => SetValue(MenuVerticalOffsetProperty, value); }

        /// <summary>The backing dependency property for <see cref="MenuVerticalOffset"/>. See the related property for details.</summary>
        public static readonly DependencyProperty MenuVerticalOffsetProperty
            = MenuButton.MenuVerticalOffsetProperty.AddOwner(typeof(FileMenuItem),
                new FrameworkPropertyMetadata(-1.0));


        #endregion

        #endregion

        #region Brushes

        // TODO: add owner from brush properties of FlatButton

        /// <summary>
        /// Get or set the brush used for the background of the control.
        /// </summary>
        [Category("Brushes")]
        public new Brush Background
        {
            get => (Brush)GetValue(BackgroundProperty);
            set => SetValue(BackgroundProperty, value);
        }

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
        /// Get or set the brush used for the border around the edges of the control.
        /// </summary>
        [Category("Brushes")]
        public new Brush BorderBrush
        {
            get => (Brush)GetValue(BorderBrushProperty);
            set => SetValue(BorderBrushProperty, value);
        }

        /// <summary>
        /// Get or set the brush used for the border while the control has the mouse over it (or it has keyboard focus).
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

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public new static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register(
            "Background", typeof(Brush), typeof(FileMenuItem),
            new PropertyMetadata(Colors.White.ToBrush()));

        public static readonly DependencyProperty ClickBrushProperty = DependencyProperty.Register(
            "ClickBrush", typeof(Brush), typeof(FileMenuItem),
            new PropertyMetadata(Colors.Gainsboro.ToBrush()));

        public static readonly DependencyProperty SelectedBrushProperty = DependencyProperty.Register(
            "SelectedBrush", typeof(Brush), typeof(FileMenuItem),
            new PropertyMetadata(Colors.WhiteSmoke.ToBrush()));

        public static readonly DependencyProperty HighlightBrushProperty = DependencyProperty.Register(
            "HighlightBrush", typeof(Brush), typeof(FileMenuItem),
            new PropertyMetadata(Colors.LightGray.ToBrush()));

        public static readonly DependencyProperty DisabledBrushProperty = DependencyProperty.Register(
            "DisabledBrush", typeof(Brush), typeof(FileMenuItem),
            new PropertyMetadata(Colors.Gray.ToBrush()));

        public static readonly DependencyProperty BorderDisabledBrushProperty = DependencyProperty.Register(
            "BorderDisabledBrush", typeof(Brush), typeof(FileMenuItem),
            new PropertyMetadata(Colors.DarkGray.ToBrush()));

        public static readonly new DependencyProperty BorderBrushProperty = DependencyProperty.Register(
            "BorderBrush", typeof(Brush), typeof(FileMenuItem),
            new PropertyMetadata(Colors.Black.ToBrush()));

        public static readonly DependencyProperty BorderHighlightBrushProperty = DependencyProperty.Register(
            "BorderHighlightBrush", typeof(Brush), typeof(FileMenuItem),
            new PropertyMetadata(Colors.Black.ToBrush()));

        public static readonly DependencyProperty BorderSelectedBrushProperty = DependencyProperty.Register(
            "BorderSelectedBrush", typeof(Brush), typeof(FileMenuItem),
            new PropertyMetadata(Colors.DimGray.ToBrush()));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        #endregion

        #region Click Handling

        #region Routed Events

        /// <summary>
        /// The backing value for the <see cref="Click"/> event. See the related event for more details.
        /// </summary>
        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent(
            "Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(FileMenuItem));

        /// <summary>
        /// Raised when the user clicks on the main button (not the menu button), via a mouse click or via the keyboard.
        /// </summary>
        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }

        /// <summary>
        /// The backing value for the <see cref="MenuClick"/> event. See the related event for more details.
        /// </summary>
        public static readonly RoutedEvent MenuClickEvent = EventManager.RegisterRoutedEvent(
            "MenuClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(FileMenuItem));

        /// <summary>
        /// Raised when the user clicks on the menu button (not the main button), via a mouse click or via the keyboard.
        /// </summary>
        public event RoutedEventHandler MenuClick
        {
            add { AddHandler(MenuClickEvent, value); }
            remove { RemoveHandler(MenuClickEvent, value); }
        }

        /// <summary>
        /// The backing value for the <see cref="RightClick"/> event. See the related event for more details.
        /// </summary>
        public static readonly RoutedEvent RightClickEvent = EventManager.RegisterRoutedEvent(
            "RightClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(FileMenuItem));

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

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        protected static readonly DependencyPropertyKey IsMouseDownPropertyKey = DependencyProperty.RegisterAttachedReadOnly("IsMouseDown",
            typeof(bool), typeof(FileMenuItem), new FrameworkPropertyMetadata(false));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Get if there is a mouse button currently being pressed, while the mouse cursor is over this control.
        /// </summary>
        public static readonly DependencyProperty IsMouseDownProperty = IsMouseDownPropertyKey.DependencyProperty;

        /// <summary>
        /// Set the IsMouseDown property for a FileMenuItem.
        /// </summary>
        /// <param name="obj">The FileMenuItem to apply the property change to.</param>
        /// <param name="value">The new value to set for the property.</param>
        protected static void SetIsMouseDown(DependencyObject obj, bool value)
        {
            obj.SetValue(IsMouseDownPropertyKey, value);
        }

        /// <summary>
        /// Get the IsMouseDown property for a FileMenuItem.
        /// </summary>
        /// <param name="obj">The FileMenuItem to get the property value from.</param>
        public static bool GetIsMouseDown(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsMouseDownProperty);
        }

        #endregion

        #region Variables/Properties

        #region IsSelected / IsSelectedChanged

        bool _runSelChangeEvent = true;

        /// <summary>
        /// The backing dependency property for <see cref="IsSelected"/>. See the related property for details.
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
            "IsSelected", typeof(bool), typeof(FileMenuItem),
            new PropertyMetadata(false, new PropertyChangedCallback(OnIsSelectedChanged)));

        /// <summary>
        /// Gets or sets whether this FileMenuItem is selected. If <see cref="SelectOnClick"/> is <c>true</c>, this state will toggle when this item is clicked.
        /// </summary>
        /// <remarks>
        /// A selected FileMenuItem will have a constant highlighted background (set via <see cref="SelectedBrush"/>) to show that it is selected.
        /// This can be used in situations where a Gallery is being used to select one or more options, with the currently active option having <c>IsSelected</c> set to <c>true</c>.
        /// </remarks>
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

        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is bool se)
            {
                bool old = (e.OldValue is bool oval) ? oval : false;

                if (d is FileMenuItem f)
                {
                    if (f._runSelChangeEvent)
                    {
                        ItemSelectionChangedEventArgs re = new ItemSelectionChangedEventArgs(IsSelectedChangedEvent, old, se, SelectionChangeTrigger.CodeUnknown, null);
                        f.RaiseEvent(re);
                    }
                }
            }
        }

        /// <summary>
        /// The backing value for the <see cref="IsSelectedChanged"/> event. See the related event for more details.
        /// </summary>
        public static readonly RoutedEvent IsSelectedChangedEvent = EventManager.RegisterRoutedEvent(
            "IsSelectedChanged", RoutingStrategy.Bubble, typeof(ItemSelectionChangedEventHandler), typeof(FileMenuItem));

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

            ItemSelectionChangedEventArgs re = new ItemSelectionChangedEventArgs(IsSelectedChangedEvent, old, value, trigger, triggerSource);
            RaiseEvent(re);
        }
        #endregion

        /// <summary>
        /// The backing dependency property for <see cref="SelectOnClick"/>. See the related property for details.
        /// </summary>
        public static readonly DependencyProperty SelectOnClickProperty = DependencyProperty.Register(
            "SelectOnClick", typeof(bool), typeof(FileMenuItem),
            new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets whether the button should change its IsSelected property when a click is performed. With this enabled, this allows the button to take on the functionality of a ToggleButton.
        /// </summary>
        /// <remarks>
        /// While SelectOnClick is true, the button will toggle between <see cref="IsSelected"/> being true and false (similar to a ToggleButton). A selected button will, by default, have some visual
        /// differences to help make it look distinct from unselected buttons. The button's Click event will still be raised while this property is set to <c>true</c>, but the event occurs after the
        /// IsSelected property has already changed. While you could use the Click event to check when the button's IsSelected property is changed, it is better to use the IsSelectedChanged event,
        /// in case of situations where IsSelected is changed via methods other than clicking, such as programmatically or via WPF binding.
        /// </remarks>
        [Category("Common")]
        public bool SelectOnClick
        {
            get => (bool)GetValue(SelectOnClickProperty);
            set => SetValue(SelectOnClickProperty, value);
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

        #region HighlightOnKeyboardFocus

        /// <summary>
        /// Get or set if the button should be highlighted (using the <see cref="HighlightBrush"/> and <see cref="BorderHighlightBrush"/>)
        /// when it has keyboard focus. If <c>false</c>, only the keyboard focus outline appears, and highlighting only occurs on mouse/stylus over.
        /// </summary>
        [Category("Appearance")]
        [Description("Get or set if the button should be highlighted when it has keyboard focus.")]
        public bool HighlightOnKeyboardFocus { get => (bool)GetValue(HighlightOnKeyboardFocusProperty); set => SetValue(HighlightOnKeyboardFocusProperty, value); }

        /// <summary>The backing dependency property for <see cref="HighlightOnKeyboardFocus"/>. See the related property for details.</summary>
        public static readonly DependencyProperty HighlightOnKeyboardFocusProperty = FlatButton.HighlightOnKeyboardFocusProperty.AddOwner(typeof(FileMenuItem),
            new PropertyMetadata(ApplyPropertyUpdate));

        #endregion

        #region IRibbonItem implementations

        /// <summary>
        /// Get or set the size to use for this control. 
        /// For <see cref="FileMenuItem"/>, the only valid values are <c>Small</c> and <c>IconOnly</c>; all other values will be treated as <c>Small</c>.
        /// </summary>
        /// <remarks>
        /// When the parent group is compacted, it'll request all controls within the group to use its <see cref="CompactSize"/> instead of its <see cref="StandardSize"/>.
        /// </remarks>
        public RibbonElementSize StandardSize { get => (RibbonElementSize)GetValue(StandardSizeProperty); set => SetValue(StandardSizeProperty, value); }

        /// <summary>The backing dependency property for <see cref="StandardSize"/>. See the related property for details.</summary>
        public static readonly DependencyProperty StandardSizeProperty
            = DependencyProperty.Register(nameof(StandardSize), typeof(RibbonElementSize), typeof(FileMenuItem),
            new FrameworkPropertyMetadata(RibbonElementSize.Large));


        /// <summary>
        /// Get or set the size to use for this control, when the parent group is being compacted.
        /// For <see cref="FileMenuItem"/>, the only valid values are <c>Small</c> and <c>IconOnly</c>; all other values will be treated as <c>Small</c>.
        /// </summary>
        /// <remarks>
        /// When the parent group is compacted, it'll request all controls within the group to use its <see cref="CompactSize"/> instead of its <see cref="StandardSize"/>.
        /// For important and commonly used controls in a group, the <c>CompactSize</c> may still be same type as the <c>StandardSize</c>, but for less important or 
        /// more infrequently used controls, it's recommended to go down a size value for <c>CompactSize</c>.
        /// </remarks>
        public RibbonElementSize CompactSize { get => (RibbonElementSize)GetValue(CompactSizeProperty); set => SetValue(CompactSizeProperty, value); }

        /// <summary>The backing dependency property for <see cref="CompactSize"/>. See the related property for details.</summary>
        public static readonly DependencyProperty CompactSizeProperty
            = DependencyProperty.Register(nameof(CompactSize), typeof(RibbonElementSize), typeof(FileMenuItem),
            new FrameworkPropertyMetadata(RibbonElementSize.Small));

        /// <inheritdoc/>
        public string AccessKey { get => (string)GetValue(AccessKeyProperty); set => SetValue(AccessKeyProperty, value); }

        /// <summary>The backing dependency property for <see cref="AccessKey"/>. See the related property for details.</summary>
        public static readonly DependencyProperty AccessKeyProperty
            = DependencyProperty.Register(nameof(AccessKey), typeof(string), typeof(FileMenuItem),
            new FrameworkPropertyMetadata("C"));

        /// <summary>
        /// Get or set the large icon to use, when using a layout that allows large icons.
        /// </summary>
        public ImageSource LargeIcon { get => (ImageSource)GetValue(LargeIconProperty); set => SetValue(LargeIconProperty, value); }

        /// <summary>The backing dependency property for <see cref="LargeIcon"/>. See the related property for details.</summary>
        public static readonly DependencyProperty LargeIconProperty
            = DependencyProperty.Register(nameof(LargeIcon), typeof(ImageSource), typeof(FileMenuItem),
            new FrameworkPropertyMetadata(defaultValue: null));

        /// <summary>
        /// Get or set the small icon to use, when using a layout that allows small icons.
        /// </summary>
        public ImageSource SmallIcon { get => (ImageSource)GetValue(SmallIconProperty); set => SetValue(SmallIconProperty, value); }

        /// <summary>The backing dependency property for <see cref="SmallIcon"/>. See the related property for details.</summary>
        public static readonly DependencyProperty SmallIconProperty
            = DependencyProperty.Register(nameof(SmallIcon), typeof(ImageSource), typeof(FileMenuItem),
            new FrameworkPropertyMetadata(defaultValue: null));

        /// <summary>
        /// Get or set the title or label of this item.
        /// </summary>
        public string Title { get => (string)GetValue(TitleProperty); set => SetValue(TitleProperty, value); }

        /// <summary>The backing dependency property for <see cref="Title"/>. See the related property for details.</summary>
        public static readonly DependencyProperty TitleProperty
            = DependencyProperty.Register(nameof(Title), typeof(string), typeof(FileMenuItem),
            new FrameworkPropertyMetadata("Item"));

        /// <inheritdoc/>
        public bool IsCompacted { get => (bool)GetValue(IsCompactedProperty); set => SetValue(IsCompactedProperty, value); }

        /// <summary>The backing dependency property for <see cref="IsCompacted"/>. See the related property for details.</summary>
        public static readonly DependencyProperty IsCompactedProperty
            = DependencyProperty.Register(nameof(IsCompacted), typeof(bool), typeof(FileMenuItem),
            new FrameworkPropertyMetadata(false));

        /// <inheritdoc/>
        public int CompactOrder { get => (int)GetValue(CompactOrderProperty); set => SetValue(CompactOrderProperty, value); }

        /// <summary>The backing dependency property for <see cref="CompactOrder"/>. See the related property for details.</summary>
        public static readonly DependencyProperty CompactOrderProperty
            = DependencyProperty.Register(nameof(CompactOrder), typeof(int), typeof(FileMenuItem),
            new FrameworkPropertyMetadata(0));

        #endregion

        /// <summary>
        /// Display this menu button's menu programmatically. This will open the menu at the set placement target and location.
        /// </summary>
        public void OpenMenu()
        {
            if (Menu != null)
            {
                // first, raise MenuOpening event
                CancelEventArgs ce = new CancelEventArgs(false);
                MenuOpening?.Invoke(this, ce);
                if (ce.Cancel) return;

                // then, set up the full menu and show it
                Menu.Placement = MenuPlacement;
                Menu.PlacementTarget = MenuPlacementTarget ?? this;
                Menu.PlacementRectangle = MenuPlacementRectangle;
                Menu.HorizontalOffset = MenuHorizontalOffset;
                Menu.VerticalOffset = MenuVerticalOffset;
                Menu.IsOpen = true;
                Menu.Closed += Menu_Closed;

                // finally, we can raise the MenuOpened event
                MenuOpened?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
