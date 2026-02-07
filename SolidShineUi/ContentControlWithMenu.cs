using SolidShineUi.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace SolidShineUi
{
    /// <summary>
    /// A control that can host some content, and displays a menu next to it.
    /// </summary>
    [ContentProperty(nameof(Content))]
    public class ContentControlWithMenu : MenuBase
    {
        static ContentControlWithMenu()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ContentControlWithMenu), new FrameworkPropertyMetadata(typeof(ContentControlWithMenu)));
        }

        #region ColorScheme

        /// <summary>
        /// Raised when the ColorScheme property is changed.
        /// </summary>
#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? ColorSchemeChanged;
#else
        public event DependencyPropertyChangedEventHandler ColorSchemeChanged;
#endif

        /// <summary>The backing dependency property for <see cref="ColorScheme"/>. See that related property for details.</summary>
        public static readonly DependencyProperty ColorSchemeProperty
            = DependencyProperty.Register("ColorScheme", typeof(ColorScheme), typeof(ContentControlWithMenu),
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
            if (d is ContentControlWithMenu s)
            {
                s.ColorSchemeChanged?.Invoke(d, e);
                s.ApplyColorScheme(cs);
            }
        }

        /// <summary>
        /// Get or set the color scheme used for this control. The color scheme can quickly apply a whole visual style to your control.
        /// </summary>
        [Category("Appearance")]
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
            if (cs != ColorScheme)
            {
                ColorScheme = cs;
                return;
            }

            Background = cs.MainColor.ToBrush();
            MenuBackground = cs.LightBackgroundColor.ToBrush();
            DisabledBrush = cs.DarkDisabledColor.ToBrush();
            BorderBrush = cs.BorderColor.ToBrush();
            HighlightBrush = cs.ThirdHighlightColor.ToBrush();
            HighlightSubitemBrush = cs.ThirdHighlightColor.ToBrush();
            CheckedBrush = cs.SecondaryColor.ToBrush();
            Foreground = cs.ForegroundColor.ToBrush();

            if (cs.IsHighContrast)
            {
                DisabledBrush = cs.LightDisabledColor.ToBrush();
                CheckedBrush = cs.HighlightColor.ToBrush();
            }
            else if (cs.MenusUseAccent)
            {
                HighlightSubitemBrush = cs.AccentThirdHighlightColor.ToBrush();
                CheckedBrush = cs.AccentSecondaryColor.ToBrush();
            }
        }

        #endregion

        #region Non-Brush Properties

        /// <summary>
        /// Get or set the radius to use for the corners of the control, to provide a rounded corner appearance if desired.
        /// </summary>
        public CornerRadius CornerRadius { get => (CornerRadius)GetValue(CornerRadiusProperty); set => SetValue(CornerRadiusProperty, value); }

        /// <summary>The backing dependency property for <see cref="CornerRadius"/>. See the related property for details.</summary>
        public static readonly DependencyProperty CornerRadiusProperty
            = DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(ContentControlWithMenu),
            new FrameworkPropertyMetadata(new CornerRadius(0)));


        #region Content Properties

        // see https://github.com/dotnet/wpf/blob/main/src/Microsoft.DotNet.Wpf/src/PresentationFramework/System/Windows/Controls/ContentControl.cs

        /// <summary>
        /// Get or set the content to display in the main area of the control.
        /// </summary>
        public object Content { get => GetValue(ContentProperty); set => SetValue(ContentProperty, value); }

        /// <summary>The backing dependency property for <see cref="Content"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ContentProperty = ContentControl.ContentProperty.AddOwner(typeof(ContentControlWithMenu), 
            new FrameworkPropertyMetadata(OnContentChanged));

        private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ContentControlWithMenu cc)
            {
                cc.HasContent = e.NewValue != null;
            }
        }

        /// <summary>
        /// Get if this control currently has a non-null <c>Content</c> value.
        /// </summary>
        public bool HasContent { get => (bool)GetValue(HasContentProperty); private set => SetValue(HasContentPropertyKey, value); }

        private static readonly DependencyPropertyKey HasContentPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(HasContent), typeof(bool), typeof(ContentControlWithMenu),
            new FrameworkPropertyMetadata(false));

        /// <summary>The backing dependency property for <see cref="HasContent"/>. See the related property for details.</summary>
        public static readonly DependencyProperty HasContentProperty = HasContentPropertyKey.DependencyProperty;

        ///// <summary>
        ///// Get or set the template to use for displaying the <see cref="Content"/> in this control.
        ///// </summary>
        //public DataTemplate ContentTemplate { get => (DataTemplate)GetValue(ContentTemplateProperty); set => SetValue(ContentTemplateProperty, value); }

        ///// <summary>The backing dependency property for <see cref="ContentTemplate"/>. See the related property for details.</summary>
        //public static readonly DependencyProperty ContentTemplateProperty = ContentControl.ContentTemplateProperty.AddOwner(typeof(ContentControlWithMenu));

        #endregion

        #region Menu Placement/Appearance

        /// <summary>
        /// Get or set the side of the control to place the menu items in <c>Items</c>.
        /// </summary>
        public PlacementDirection MenuPlacement { get => (PlacementDirection)GetValue(MenuPlacementProperty); set => SetValue(MenuPlacementProperty, value); }

        /// <summary>The backing dependency property for <see cref="MenuPlacement"/>. See the related property for details.</summary>
        public static readonly DependencyProperty MenuPlacementProperty
            = DependencyProperty.Register(nameof(MenuPlacement), typeof(PlacementDirection), typeof(ContentControlWithMenu),
            new FrameworkPropertyMetadata(PlacementDirection.Bottom));

        /// <summary>
        /// Get or set the padding to have around the menu items in the menu part of the control. Default value is 2.
        /// </summary>
        public Thickness MenuItemsPadding { get => (Thickness)GetValue(MenuItemsPaddingProperty); set => SetValue(MenuItemsPaddingProperty, value); }

        /// <summary>The backing dependency property for <see cref="MenuItemsPadding"/>. See the related property for details.</summary>
        public static readonly DependencyProperty MenuItemsPaddingProperty
            = DependencyProperty.Register(nameof(MenuItemsPadding), typeof(Thickness), typeof(ContentControlWithMenu),
            new FrameworkPropertyMetadata(new Thickness(2)));


        #endregion

        #endregion

        #region Brushes

        /// <summary>
        /// Get or set the brush used for the background of the menu's drop-down area.
        /// </summary>
        [Category("Brushes")]
        public Brush MenuBackground
        {
            get
            {
                return (Brush)GetValue(MenuBackgroundProperty);
            }
            set
            {
                SetValue(MenuBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Get or set the brush used for the foreground/text of a child menu item when it is disabled.
        /// </summary>
        [Category("Brushes")]
        public Brush DisabledBrush
        {
            get
            {
                return (Brush)GetValue(DisabledBrushProperty);
            }
            set
            {
                SetValue(DisabledBrushProperty, value);
            }
        }

        /// <summary>
        /// Get or set the brush used for the top-level menu items in the control itself, when the mouse is over them or they have keyboard focus.
        /// </summary>
        [Category("Brushes")]
        public Brush HighlightBrush
        {
            get
            {
                return (Brush)GetValue(HighlightBrushProperty);
            }
            set
            {
                SetValue(HighlightBrushProperty, value);
            }
        }

        /// <summary>
        /// Get or set the brush used for menu items that aren't top-level in the control, when the mouse is over them or they have keyboard focus.
        /// </summary>
        [Category("Brushes")]
        public Brush HighlightSubitemBrush
        {
            get
            {
                return (Brush)GetValue(HighlightSubitemBrushProperty);
            }
            set
            {
                SetValue(HighlightSubitemBrushProperty, value);
            }
        }

        /// <summary>
        /// Get or set the brush used for the foreground elements of the top-level menu items in the control, 
        /// when the mouse is over them or they have keyboard focus.
        /// </summary>
        [Category("Brushes")]
        public Brush HighlightForegroundBrush
        {
            get
            {
                return (Brush)GetValue(HighlightForegroundBrushProperty);
            }
            set
            {
                SetValue(HighlightForegroundBrushProperty, value);
            }
        }

        /// <summary>
        /// Get or set the brush used for the foreground elements of menu items that aren't top-level in the control, 
        /// when the mouse is over them or they have keyboard focus.
        /// </summary>
        [Category("Brushes")]
        public Brush HighlightSubitemForegroundBrush
        {
            get
            {
                return (Brush)GetValue(HighlightSubitemForegroundBrushProperty);
            }
            set
            {
                SetValue(HighlightSubitemForegroundBrushProperty, value);
            }
        }

        /// <summary>
        /// Get or set the brush to use for the check highlight for checked menu items.
        /// </summary>
        [Category("Brushes")]
        public Brush CheckedBrush
        {
            get
            {
                return (Brush)GetValue(CheckedBrushProperty);
            }
            set
            {
                SetValue(CheckedBrushProperty, value);
            }
        }

        /// <summary>
        /// Get or set the brush to use for the borders around the submenus, if any, of the menu items in this control.
        /// </summary>
        [Category("Brushes")]
        public Brush MenuBorderBrush { get => (Brush)GetValue(MenuBorderBrushProperty); set => SetValue(MenuBorderBrushProperty, value); }

        /// <summary>
        /// Get or set the brush to use for the separator between the menu and content areas of the control.
        /// </summary>
        /// <remarks>
        /// This does not affect the separators between menu items in the menu itself; you will want to use a custom style for those separators.
        /// </remarks>
        [Category("Brushes")]
        public Brush SeparatorBrush { get => (Brush)GetValue(SeparatorBrushProperty); set => SetValue(SeparatorBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="SeparatorBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty SeparatorBrushProperty
            = DependencyProperty.Register(nameof(SeparatorBrush), typeof(Brush), typeof(ContentControlWithMenu),
            new FrameworkPropertyMetadata(null));

        /// <summary>The backing dependency property for <see cref="MenuBackground"/>. See that related property for details.</summary>
        public static readonly DependencyProperty MenuBackgroundProperty = Menu.MenuBackgroundProperty.AddOwner(typeof(ContentControlWithMenu));

        /// <summary>The backing dependency property for <see cref="DisabledBrush"/>. See that related property for details.</summary>
        public static readonly DependencyProperty DisabledBrushProperty = Menu.DisabledBrushProperty.AddOwner(typeof(ContentControlWithMenu));

        /// <summary>The backing dependency property for <see cref="HighlightSubitemBrush"/>. See that related property for details.</summary>
        public static readonly DependencyProperty HighlightSubitemBrushProperty = Menu.HighlightSubitemBrushProperty.AddOwner(typeof(ContentControlWithMenu));

        /// <summary>The backing dependency property for <see cref="HighlightBrush"/>. See that related property for details.</summary>
        public static readonly DependencyProperty HighlightBrushProperty = Menu.HighlightBrushProperty.AddOwner(typeof(ContentControlWithMenu));

        /// <summary>The backing dependency property for <see cref="CheckedBrush"/>. See that related property for details.</summary>
        public static readonly DependencyProperty CheckedBrushProperty = Menu.CheckedBrushProperty.AddOwner(typeof(ContentControlWithMenu));

        /// <summary>The backing dependency property for <see cref="HighlightForegroundBrush"/>. See that related property for details.</summary>
        public static readonly DependencyProperty HighlightForegroundBrushProperty = 
            Menu.HighlightForegroundBrushProperty.AddOwner(typeof(ContentControlWithMenu));

        /// <summary>The backing dependency property for <see cref="HighlightSubitemForegroundBrush"/>. See that related property for details.</summary>
        public static readonly DependencyProperty HighlightSubitemForegroundBrushProperty =
            Menu.HighlightSubitemForegroundBrushProperty.AddOwner(typeof(ContentControlWithMenu));

        /// <summary>The backing dependency property for <see cref="MenuBorderBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty MenuBorderBrushProperty = Menu.MenuBorderBrushProperty.AddOwner(typeof(ContentControlWithMenu));

        #endregion

        #region Menu Methods

        // copied a few methods from MenuBase to help emulate some functionality
        // wanted to add more methods from Menu to emulate some other stuff, but since a lot of the relevant methods and properties are internal, I can't
        // https://github.com/dotnet/wpf/blob/main/src/Microsoft.DotNet.Wpf/src/PresentationFramework/System/Windows/Controls/Menu.cs
        // https://github.com/dotnet/wpf/blob/main/src/Microsoft.DotNet.Wpf/src/PresentationFramework/System/Windows/Controls/Primitives/MenuBase.cs

        #region Copied items I don't need

        ///// <summary>
        /////     DependencyProperty for UsesItemContainerTemplateSelector property.
        ///// </summary>
        //public static readonly DependencyProperty UsesItemContainerTemplateProperty = MenuBase.UsesItemContainerTemplateProperty.AddOwner(typeof(ContentControlWithMenu));

        ///// <summary>
        /////     UsesItemContainerTemplate property which says whether the ItemContainerTemplateSelector property is to be used.
        ///// </summary>
        //public bool UsesItemContainerTemplate
        //{
        //    get { return (bool)GetValue(UsesItemContainerTemplateProperty); }
        //    set { SetValue(UsesItemContainerTemplateProperty, value); }
        //}

        ///// <summary>
        /////     DependencyProperty for ItemContainerTemplateSelector property.
        ///// </summary>
        //public static readonly DependencyProperty ItemContainerTemplateSelectorProperty = MenuBase.ItemContainerTemplateSelectorProperty.AddOwner(typeof(ContentControlWithMenu));

        ///// <summary>
        /////     ItemContainerTemplateSelector property which provides the DataTemplate to be used to create an instance of the ItemContainer.
        ///// </summary>
        //public ItemContainerTemplateSelector ItemContainerTemplateSelector
        //{
        //    get { return (ItemContainerTemplateSelector)GetValue(ItemContainerTemplateSelectorProperty); }
        //    set { SetValue(ItemContainerTemplateSelectorProperty, value); }
        //}

        //#if NETCOREAPP
        //        private object? _currentItem;
        //#else
        //        private object _currentItem;
        //#endif

        //        /// <inheritdoc/>
        //        protected override bool IsItemItsOwnContainerOverride(object item)
        //        {
        //            bool ret = (item is MenuItem) || (item is Separator);
        //            if (!ret)
        //            {
        //                _currentItem = item;
        //            }

        //            return ret;
        //        }

        //        /// <inheritdoc/>
        //        protected override DependencyObject GetContainerForItemOverride()
        //        {
        //#if NETCOREAPP
        //            object? currentItem = _currentItem;
        //#else
        //            object currentItem = _currentItem;
        //#endif
        //            _currentItem = null;

        //            if (UsesItemContainerTemplate)
        //            {
        //                DataTemplate itemContainerTemplate = ItemContainerTemplateSelector.SelectTemplate(currentItem, this);
        //                if (itemContainerTemplate != null)
        //                {
        //                    object itemContainer = itemContainerTemplate.LoadContent();
        //                    if (itemContainer is MenuItem || itemContainer is Separator)
        //                    {
        //#if NETCOREAPP
        //                        return (itemContainer as DependencyObject)!; // MenuItem and Separator are both DependencyObjects, so this will always resolve
        //#else
        //                        return itemContainer as DependencyObject;
        //#endif
        //                    }
        //                    else
        //                    {
        //                        throw new InvalidOperationException("Item container is invalid: " + itemContainer.GetType().ToString());
        //                    }
        //                }
        //            }

        //            return new MenuItem();
        //        }

        #endregion

        #region MenuBase Properties Access

        // because MenuBase declares these as internal, I have to find these properties via Reflection

#if NETCOREAPP
        private object? BaseCurrentSelection
#else
        private object BaseCurrentSelection
#endif
        {
            get
            {
                return typeof(MenuBase)?.GetProperty("CurrentSelection", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(this);
            }
            set
            {
                typeof(MenuBase)?.GetProperty("CurrentSelection", BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(this, value);
            }
        }

#if NETCOREAPP
        private Panel? BaseItemsHost
#else
        private Panel BaseItemsHost
#endif
        {
            get
            {
                return typeof(MenuBase)?.GetProperty("ItemsHost", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(this) as Panel;
            }
            set
            {
                typeof(MenuBase)?.GetProperty("ItemsHost", BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(this, value);
            }
        }

        #endregion

        ///// <summary>
        /////     This is the method that responds to the KeyDown event.
        ///// </summary>
        ///// <param name="e">Event arguments</param>

        /// <inheritdoc/>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Handled) return;
            
            Key key = e.Key;
            switch (key)
            {
                case Key.Down:
                case Key.Up:
                    if (BaseCurrentSelection != null)
                    {
                        // Only for non vertical layout Up/Down open the submenu
#if NETCOREAPP
                        Panel? itemsHost = BaseItemsHost;
#else
                        Panel itemsHost = BaseItemsHost;
#endif
                        bool isVertical = itemsHost != null && itemsHost.HasLogicalOrientationPublic && itemsHost.LogicalOrientationPublic == Orientation.Vertical;
                        if (!isVertical)
                        {
                            if (BaseCurrentSelection is MenuItem mi && mi.HasItems)
                            {
                                mi.IsSubmenuOpen = true;
                            }
                            e.Handled = true;
                        }
                    }
                    break;
                case Key.Left:
                case Key.Right:
                    if (BaseCurrentSelection != null)
                    {
                        // Only for vertical layout Left/Right open the submenu
#if NETCOREAPP
                        Panel? itemsHost = BaseItemsHost;
#else
                        Panel itemsHost = BaseItemsHost;
#endif
                        bool isVertical = itemsHost != null && itemsHost.HasLogicalOrientationPublic && itemsHost.LogicalOrientationPublic == Orientation.Vertical;
                        if (isVertical)
                        {
                            if (BaseCurrentSelection is MenuItem mi && mi.HasItems)
                            {
                                mi.IsSubmenuOpen = true;
                            }
                            e.Handled = true;
                        }
                    }
                    break;
            }
        }

        #endregion

        #region SsuiTheme

        /// <summary>
        /// Get or set the theme to apply to the appearance of this control. This will bind the control's brushes to this theme.
        /// </summary>
        /// <remarks>
        /// This theme can be used to centrally apply a consistent appearance to all ThemedControls. Once a theme is set, the control's brushes are bound to the relevant
        /// values in this SsuiTheme, allowing you to update the control's appearance by changing the relevant values in the SsuiTheme. 
        /// This has the downside of overwriting any preset brush values set via XAML; to circumvent this, listen to the <see cref="SsuiThemeChanged"/> event and 
        /// then reapply the brushes in that event handler, or include that brush property's name in this control's <see cref="ThemeValueExclude"/>.
        /// <para/>
        /// This is set to null by default, so that brushes are not automatically bound to any SsuiTheme (allowing you to set the brushes via XAML).
        /// </remarks>
        [Category("Appearance")]
#if NETCOREAPP
        public SsuiTheme? SsuiTheme { get => (SsuiTheme)GetValue(SsuiThemeProperty); set => SetValue(SsuiThemeProperty, value); }
#else
        public SsuiTheme SsuiTheme { get => (SsuiTheme)GetValue(SsuiThemeProperty); set => SetValue(SsuiThemeProperty, value); }
#endif

        /// <summary>The backing dependency property for <see cref="SsuiTheme"/>. See the related property for details.</summary>
        public static readonly DependencyProperty SsuiThemeProperty = ThemedControl.SsuiThemeProperty.AddOwner(typeof(ContentControlWithMenu),
            new FrameworkPropertyMetadata(OnSsuiThemeChanged));

        /// <summary>
        /// The backing routed event object for <see cref="SsuiThemeChanged"/>. Please see the related event for details.
        /// </summary>
        public static readonly RoutedEvent SsuiThemeChangedEvent = ThemedControl.SsuiThemeChangedEvent.AddOwner(typeof(ContentControlWithMenu));
        //EventManager.RegisterRoutedEvent(nameof(SsuiThemeChanged), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ThemedContentControl));

        /// <summary>
        /// Raised after the <see cref="SsuiTheme"/> property has been changed and applied.
        /// </summary>
        public event RoutedEventHandler SsuiThemeChanged
        {
            add { AddHandler(SsuiThemeChangedEvent, value); }
            remove { RemoveHandler(SsuiThemeChangedEvent, value); }
        }

        private static void OnSsuiThemeChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is ContentControlWithMenu tc)
            {
                tc.ApplyAndRaiseTheme();
            }
        }

        /// <summary>
        /// Update the control's appearance by using <see cref="ApplySsuiTheme(SsuiTheme)"/> and then raise the <see cref="SsuiThemeChanged"/> event.
        /// </summary>
        protected internal void ApplyAndRaiseTheme()
        {
            ApplySsuiTheme(SsuiTheme, UseLightBorder, UseAccentTheme);

            RoutedEventArgs re = new RoutedEventArgs(SsuiThemeChangedEvent, this);
            RaiseEvent(re);
        }

        #region UseLightBorder

        /// <summary>
        /// Get or set if the <see cref="SsuiTheme.LightBorderBrush"/> should be used for the border of this control, rather than the <see cref="SsuiTheme.BorderBrush"/>.
        /// </summary>
        public bool UseLightBorder { get => (bool)GetValue(UseLightBorderProperty); set => SetValue(UseLightBorderProperty, value); }

        /// <summary>The backing dependency property for <see cref="UseLightBorder"/>. See the related property for details.</summary>
        public static readonly DependencyProperty UseLightBorderProperty = ThemedControl.UseLightBorderProperty.AddOwner(typeof(ContentControlWithMenu),
            new FrameworkPropertyMetadata(false, (d, e) => d.PerformAs<ContentControlWithMenu>((o) => o.OnUseLightBorderChange(o, e))));

        void OnUseLightBorderChange(object sender, DependencyPropertyChangedEventArgs _)
        {
            if (_skipReapply) return;
            ApplySsuiTheme(SsuiTheme, UseLightBorder, UseAccentTheme);
        }

        #endregion

        #region UseAccentColors

        /// <summary>
        /// Get or set if an accent theme should be used rather than the standard theme for this control. The accent theme, when used in moderation,
        /// can be used to help particular controls stand out in the UI.
        /// </summary>
        /// <remarks>
        /// The value of the <see cref="SsuiTheme"/> property needs to be of an <see cref="SsuiAppTheme"/> type, or otherwise this property does nothing.
        /// This should already be taken care of if you're putting this control in a <see cref="ThemedWindow"/> and inheriting from that SsuiTheme property.
        /// </remarks>
        public bool UseAccentTheme { get => (bool)GetValue(UseAccentThemeProperty); set => SetValue(UseAccentThemeProperty, value); }

        /// <summary>The backing dependency property for <see cref="UseAccentTheme"/>. See the related property for details.</summary>
        public static readonly DependencyProperty UseAccentThemeProperty = ThemedControl.UseAccentThemeProperty.AddOwner(typeof(ContentControlWithMenu),
            new FrameworkPropertyMetadata(false, (d, e) => d.PerformAs<ContentControlWithMenu>((o) => o.OnUseAccentColorsChange(o, e))));

        void OnUseAccentColorsChange(object sender, DependencyPropertyChangedEventArgs _)
        {
            if (_skipReapply) return;
            ApplySsuiTheme(SsuiTheme, UseLightBorder, UseAccentTheme);
        }

        #endregion

        #region ThemeValueExclude

        /// <summary>
        /// Get or set the properties to not apply the theme values to when applying a SsuiTheme to this control.
        /// <para/>
        /// Use a comma-separated list for multiple properties; for each property, the property's value will not be changed when the SsuiTheme is applied.
        /// </summary>
        /// <remarks>
        /// This can be used when you have certain brushes or values set for a particular control and applying a SsuiTheme ends up resetting that value.
        /// If only one property's value needs to be kept unchanged, then just set this to the name of that property. If multiple properties need this,
        /// then provide this as a comma-separated list of all the properties' names in a single string (e.g., <c>"Background,Foreground,BorderBrush"</c>).
        /// When the control is applying a SsuiTheme (such as set via a property directly, or inheriting from a parent), the control will skip over setting
        /// properties that match one of the names on this list.
        /// <para/>
        /// Note that this functionality is designed for comparing dependency properties' names (<see cref="DependencyProperty.Name"/>) 
        /// to the list provided here. This will also likely not have any effect on any property that isn't a dependency property.
        /// </remarks>
        public string ThemeValueExclude { get => (string)GetValue(ThemeValueExcludeProperty); set => SetValue(ThemeValueExcludeProperty, value); }

        /// <summary>The backing dependency property for <see cref="ThemeValueExclude"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ThemeValueExcludeProperty = ThemedControl.ThemeValueExcludeProperty.AddOwner(typeof(ContentControlWithMenu));

        #endregion

        #region ApplySsuiTheme

        bool _skipReapply = false;

        /// <summary>
        /// Check the control's properties to make sure they match these values. If not, this will update the properties and then re-call the ApplySsuiTheme function.
        /// </summary>
        /// <param name="ssuiTheme">the inputted SsuiTheme value</param>
        /// <param name="useLightBorder">the inputted UseLightBorder value</param>
        /// <param name="useAccentTheme">the inputted UseAccentTheme value</param>
#if NETCOREAPP
        bool CheckAndUpdateProperties(SsuiTheme? ssuiTheme, bool useLightBorder, bool useAccentTheme)
#else
        bool CheckAndUpdateProperties(SsuiTheme ssuiTheme, bool useLightBorder, bool useAccentTheme)
#endif
        {
            _skipReapply = true;

            if (UseLightBorder != useLightBorder) UseLightBorder = useLightBorder;
            if (UseAccentTheme != useAccentTheme) UseAccentTheme = useAccentTheme;

            _skipReapply = false;

            if (ssuiTheme != SsuiTheme)
            {
                SsuiTheme = ssuiTheme;
                return false; // ApplySsuiTheme will be called again once the property is changed
            }

            return true;
        }

        /// <summary>
        /// Apply a <see cref="SsuiTheme"/> to this control. This applies a consistent appearance to this control and child controls.
        /// </summary>
        /// <param name="ssuiTheme">the theme value to apply</param>
        public void ApplySsuiTheme(SsuiTheme ssuiTheme)
        {
            ApplySsuiTheme(ssuiTheme, UseLightBorder, UseAccentTheme);
        }

        /// <summary>
        /// Apply a <see cref="SsuiTheme"/> to this control. This applies a consistent appearance to this control and child controls.
        /// </summary>
        /// <param name="ssuiTheme">the theme value to apply</param>
        /// <param name="useLightBorder">whether a light border color should be used in place of the standard one</param>
        /// <param name="useAccentTheme">
        /// whether the accent theme should be used in place of the standard one; for this to apply, <paramref name="ssuiTheme"/>
        /// should be of type <see cref="SsuiAppTheme"/>
        /// </param>
#if NETCOREAPP
        public void ApplySsuiTheme(SsuiTheme? ssuiTheme, bool useLightBorder = false, bool useAccentTheme = false)
#else
        public void ApplySsuiTheme(SsuiTheme ssuiTheme, bool useLightBorder = false, bool useAccentTheme = false)
#endif
        {
            if (!CheckAndUpdateProperties(ssuiTheme, useLightBorder, useAccentTheme)) return;

            if (ssuiTheme != null) OnApplySsuiTheme(ssuiTheme, useLightBorder, useAccentTheme);

            RoutedEventArgs re = new RoutedEventArgs(SsuiThemeAppliedEvent, this);
            RaiseEvent(re);
        }

        /// <summary>
        /// Clear the existing SsuiTheme applied to this control and remove all bindings. This will reset the control's appearance.
        /// </summary>
        public void ClearSsuiTheme()
        {
            SsuiTheme = null;

            // maybe I should update OnApplySsuiTheme to show that it can support null values, but for now, I'm doing this lol
#if NETCOREAPP
            OnApplySsuiTheme(null!);
#else
            OnApplySsuiTheme(null);
#endif
        }

        /// <summary>
        /// Raised when the SsuiTheme value is changed, or when <see cref="ApplySsuiTheme(SsuiTheme, bool, bool)"/> is called.
        /// Inheriting controls should override this to update their brushes to reflect this new theme.
        /// </summary>
        /// <param name="ssuiTheme">the theme value to apply</param>
        /// <param name="useLightBorder">whether a light border color should be used in place of the standard one</param>
        /// <param name="useAccentTheme">
        /// whether the accent theme should be used in place of the standard one; for this to apply, <paramref name="ssuiTheme"/>
        /// should be of type <see cref="SsuiAppTheme"/>
        /// </param>
        /// <remarks>
        /// The base method will by default set the <see cref="Control.BorderBrush"/> value to match the theme; if a different
        /// border brush is needed, then update it again in your override method.
        /// </remarks>
        protected virtual void OnApplySsuiTheme(SsuiTheme ssuiTheme, bool useLightBorder = false, bool useAccentTheme = false)
        {
            if (ssuiTheme is SsuiAppTheme sat)
            {
                if (sat.UseSubitemThemeWithMenus)
                {
                    ApplyThemeBinding(HighlightSubitemBrushProperty, SsuiTheme.HighlightBrushProperty, sat.SubitemTheme);
                    ApplyThemeBinding(HighlightForegroundBrushProperty, SsuiTheme.HighlightForegroundProperty, sat.SubitemTheme);
                    ApplyThemeBinding(CheckedBrushProperty, SsuiTheme.SelectedBackgroundBrushProperty, sat.SubitemTheme);

                    ApplyTheme(useAccentTheme ? sat.AccentTheme : ssuiTheme);
                }
                else if (useAccentTheme)
                {
                    ApplyThemeBinding(HighlightSubitemBrushProperty, SsuiTheme.HighlightBrushProperty, sat.AccentTheme);
                    ApplyThemeBinding(HighlightForegroundBrushProperty, SsuiTheme.HighlightForegroundProperty, sat.AccentTheme);
                    ApplyThemeBinding(CheckedBrushProperty, SsuiTheme.SelectedBackgroundBrushProperty, sat.AccentTheme);

                    ApplyTheme(sat.AccentTheme);
                }
                else
                {
                    ApplyThemeBinding(HighlightSubitemBrushProperty, SsuiTheme.HighlightBrushProperty);
                    ApplyThemeBinding(HighlightForegroundBrushProperty, SsuiTheme.HighlightForegroundProperty);
                    ApplyThemeBinding(CheckedBrushProperty, SsuiTheme.SelectedBackgroundBrushProperty);

                    ApplyTheme(ssuiTheme);
                }
            }
            else
            {
                ApplyThemeBinding(HighlightSubitemBrushProperty, SsuiTheme.HighlightBrushProperty);
                ApplyThemeBinding(HighlightForegroundBrushProperty, SsuiTheme.HighlightForegroundProperty);
                ApplyThemeBinding(CheckedBrushProperty, SsuiTheme.SelectedBackgroundBrushProperty);

                ApplyTheme(ssuiTheme);
            }

            void ApplyTheme(SsuiTheme theme)
            {
                ApplyThemeBinding(BorderBrushProperty, useLightBorder ? SsuiTheme.LightBorderBrushProperty : SsuiTheme.BorderBrushProperty, theme);
                ApplyThemeBinding(MenuBorderBrushProperty, useLightBorder ? SsuiTheme.LightBorderBrushProperty : SsuiTheme.BorderBrushProperty, theme);
                ApplyThemeBinding(SeparatorBrushProperty, useLightBorder ? SsuiTheme.LightBorderBrushProperty : SsuiTheme.BorderBrushProperty, theme);

                ApplyThemeBinding(BackgroundProperty, SsuiTheme.PanelBackgroundProperty, theme);
                ApplyThemeBinding(MenuBackgroundProperty, SsuiTheme.PanelBackgroundProperty, theme);
                ApplyThemeBinding(HighlightBrushProperty, SsuiTheme.HighlightBrushProperty, theme);
                ApplyThemeBinding(DisabledBrushProperty, SsuiTheme.DisabledForegroundProperty, theme);
                ApplyThemeBinding(ForegroundProperty, SsuiTheme.ForegroundProperty, theme);

                ApplyThemeBinding(CornerRadiusProperty, SsuiTheme.CornerRadiusProperty, theme);
            }
        }

        /// <summary>
        /// Create and set a binding for a brush property, by binding it to a property in <see cref="SolidShineUi.SsuiTheme"/>.
        /// </summary>
        /// <param name="brushProperty">the property on this control to bind</param>
        /// <param name="ssuiThemeProperty">the property in <see cref="SolidShineUi.SsuiTheme"/> to bind this control's property to</param>
        /// <exception cref="ArgumentException">
        /// thrown if <paramref name="ssuiThemeProperty"/> is not a property in <see cref="SolidShineUi.SsuiTheme"/> or a class that inherits from SsuiTheme
        /// </exception>
        /// <remarks>
        /// If <see cref="SsuiTheme"/> is <c>null</c>, then instead the binding is cleared, and <c>null</c> is returned.
        /// </remarks>
#if NETCOREAPP
        protected BindingExpressionBase? ApplyThemeBinding(DependencyProperty brushProperty, DependencyProperty ssuiThemeProperty)
#else
        protected BindingExpressionBase ApplyThemeBinding(DependencyProperty brushProperty, DependencyProperty ssuiThemeProperty)
#endif
        {
            return ApplyThemeBinding(brushProperty, ssuiThemeProperty, SsuiTheme);
        }

        /// <summary>
        /// Create and set a binding for a brush property, by binding it to a property in <see cref="SolidShineUi.SsuiTheme"/>.
        /// </summary>
        /// <param name="brushProperty">the property on this control to bind</param>
        /// <param name="ssuiThemeProperty">the name of the property in <see cref="SolidShineUi.SsuiTheme"/> to bind this control's property to</param>
        /// <param name="source">the specific SsuiTheme object to bind to (such as <see cref="SsuiAppTheme"/>'s AccentTheme or SubitemTheme)</param>
        /// <exception cref="ArgumentException">
        /// thrown if <paramref name="ssuiThemeProperty"/> is not a property in <see cref="SolidShineUi.SsuiTheme"/> or a class that inherits from SsuiTheme
        /// </exception>
        /// <remarks>
        /// If <paramref name="source"/> is <c>null</c>, then instead the binding is cleared, and <c>null</c> is returned.
        /// </remarks>
#if NETCOREAPP
        protected BindingExpressionBase? ApplyThemeBinding(DependencyProperty brushProperty, DependencyProperty ssuiThemeProperty, SsuiTheme? source)
#else
        protected BindingExpressionBase ApplyThemeBinding(DependencyProperty brushProperty, DependencyProperty ssuiThemeProperty, SsuiTheme source)
#endif
        {
            // if ThemeValueExclude includes this property's name (as part of a comma-separated list), then we'll not do anything here
            if (!string.IsNullOrEmpty(ThemeValueExclude))
            {
                if (ThemeValueExclude.Split(',').Contains(brushProperty.Name)) return null;
            }

            // if the theme value provided is null, then we'll just undo any binding that exists (which should undo it to the previous SsuiTheme)
            if (source == null)
            {
                BindingOperations.ClearBinding(this, brushProperty);
                return null;
            }
            else
            {
                return SetBinding(brushProperty, SsuiTheme.CreateBinding(ssuiThemeProperty, source));
            }
        }

        /// <summary>
        /// The backing routed event object for <see cref="SsuiThemeChanged"/>. Please see the related event for details.
        /// </summary>
        public static readonly RoutedEvent SsuiThemeAppliedEvent = ThemedControl.SsuiThemeAppliedEvent.AddOwner(typeof(ContentControlWithMenu));
        // EventManager.RegisterRoutedEvent(nameof(SsuiThemeApplied), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ThemedContentControl));

        /// <summary>
        /// Raised after <see cref="ApplySsuiTheme(SsuiTheme, bool, bool)"/> has completed, and the theme has been applied to the control.
        /// </summary>
        /// <remarks>
        /// If creating your own control that inherits from this class, use this event or override the <see cref="OnApplySsuiTheme(SsuiTheme, bool, bool)"/>
        /// method to apply any updated brushes from the <see cref="SsuiTheme"/>.
        /// <para/>
        /// This event precedes the <see cref="SsuiThemeChanged"/> event.
        /// </remarks>
        public event RoutedEventHandler SsuiThemeApplied
        {
            add { AddHandler(SsuiThemeAppliedEvent, value); }
            remove { RemoveHandler(SsuiThemeAppliedEvent, value); }
        }

        #endregion

        #endregion
    }
}
