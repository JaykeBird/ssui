using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using System;
using System.ComponentModel;

namespace SolidShineUi
{
    /// <summary>
    /// A popup menu that can display items specific to the context it's invoked by, with a visual style that matches the rest of the Solid Shine UI controls.
    /// </summary>
    public class ContextMenu : Avalonia.Controls.ContextMenu
    {
        #region Color Scheme

        /// <summary>
        /// Get or set the color scheme used for this control. The color scheme can quickly apply a whole visual style to your control.
        /// </summary>
        public ColorScheme ColorScheme { get => GetValue(ColorSchemeProperty); set => SetValue(ColorSchemeProperty, value); }

        /// <summary>The backing styled property for <see cref="ColorScheme"/>. See the related property for details.</summary>
        public static readonly StyledProperty<ColorScheme> ColorSchemeProperty
            = AvaloniaProperty.Register<ContextMenu, ColorScheme>(nameof(ColorScheme), new ColorScheme());

        /// <inheritdoc/>
        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            if (change.Property.Name == nameof(ColorScheme))
            {
                ApplyColorScheme(change.GetNewValue<ColorScheme>());
                ColorSchemeChanged?.Invoke(this, change);
            }

            base.OnPropertyChanged(change);
        }

        /// <summary>
        /// Raised when the <see cref="ColorScheme"/> property has changed.
        /// </summary>
        public event EventHandler<AvaloniaPropertyChangedEventArgs>? ColorSchemeChanged;

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

        #region Brushes

        /// <summary>
        /// Get or set the brush used for the background of the menu's drop-down area. This is different from the Background brush, which set the background of the top menu bar.
        /// </summary>
        [Category("Brushes")]
        public IBrush? MenuBackground { get => GetValue(MenuBackgroundProperty); set => SetValue(MenuBackgroundProperty, value); }

        /// <summary>The backing styled property for <see cref="MenuBackground"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> MenuBackgroundProperty = Menu.MenuBackgroundProperty.AddOwner<ContextMenu>(
            new StyledPropertyMetadata<IBrush?>(defaultValue: new SolidColorBrush(Colors.White)));

        /// <summary>
        /// Get or set the brush used for the foreground of menu items that are disabled.
        /// </summary>
        [Category("Brushes")]
        public IBrush? DisabledBrush { get => GetValue(DisabledBrushProperty); set => SetValue(DisabledBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="DisabledBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> DisabledBrushProperty = Menu.DisabledBrushProperty.AddOwner<ContextMenu>(
            new StyledPropertyMetadata<IBrush?>(defaultValue: new SolidColorBrush(Colors.Gray)));

        /// <summary>
        /// Get or set the brush used for the border of the menu's drop-down area.
        /// </summary>
        [Category("Brushes")]
        public IBrush? PopupBorderBrush { get => GetValue(PopupBorderBrushProperty); set => SetValue(PopupBorderBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="PopupBorderBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> PopupBorderBrushProperty = Menu.PopupBorderBrushProperty.AddOwner<ContextMenu>(
            new StyledPropertyMetadata<IBrush?>(defaultValue: new SolidColorBrush(Colors.Gray)));

        /// <summary>
        /// Get or set the brush used for menu items that aren't top-level on the menu bar, when the mouse is over them or they have keyboard focus.
        /// </summary>
        [Category("Brushes")]
        public IBrush? HighlightSubitemBrush { get => GetValue(HighlightSubitemBrushProperty); set => SetValue(HighlightSubitemBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="HighlightSubitemBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> HighlightSubitemBrushProperty = Menu.HighlightSubitemBrushProperty.AddOwner<ContextMenu>(
            new StyledPropertyMetadata<IBrush?>(defaultValue: new SolidColorBrush(Colors.LightGray)));

        /// <summary>
        /// Get or set the brush used for the top-level menu items in the menu bar, when the mouse is over them or they have keyboard focus.
        /// </summary>
        [Category("Brushes")]
        public IBrush? HighlightBrush { get => GetValue(HighlightBrushProperty); set => SetValue(HighlightBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="HighlightBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> HighlightBrushProperty = Menu.HighlightBrushProperty.AddOwner<ContextMenu>(
            new StyledPropertyMetadata<IBrush?>(defaultValue: new SolidColorBrush(Colors.LightGray)));

        /// <summary>
        /// Get or set the brush to use for the check highlight for checked menu items.
        /// </summary>
        [Category("Brushes")]
        public IBrush? CheckedBrush { get => GetValue(CheckedBrushProperty); set => SetValue(CheckedBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="CheckedBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> CheckedBrushProperty = Menu.CheckedBrushProperty.AddOwner<ContextMenu>(
            new StyledPropertyMetadata<IBrush?>(defaultValue: new SolidColorBrush(Colors.LightGray)));

        /// <summary>
        /// Get or set the brush to use when clicking on a menu item.
        /// </summary>
        [Category("Brushes")]
        public IBrush? ClickBrush { get => GetValue(ClickBrushProperty); set => SetValue(ClickBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="ClickBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> ClickBrushProperty = Menu.ClickBrushProperty.AddOwner<ContextMenu>(
            new StyledPropertyMetadata<IBrush?>(defaultValue: new SolidColorBrush(Colors.Gainsboro)));

        ///// <summary>
        ///// Get or set the foreground color to use for text and other elements in the menu.
        ///// </summary>
        //public IBrush? Foreground { get => GetValue(ForegroundProperty); set => SetValue(ForegroundProperty, value); }

        ///// <summary>The backing styled property for <see cref="Foreground"/>. See the related property for details.</summary>
        //public static readonly StyledProperty<IBrush?> ForegroundProperty = TemplatedControl.ForegroundProperty.AddOwner<ContextMenu>();

        #endregion

        /// <summary>
        /// Get or set the corner radius to apply to the opened menu drop-downs.
        /// </summary>
        public CornerRadius MenuCornerRadius { get => GetValue(MenuCornerRadiusProperty); set => SetValue(MenuCornerRadiusProperty, value); }

        /// <summary>The backing styled property for <see cref="MenuCornerRadius"/>. See the related property for details.</summary>
        public static readonly StyledProperty<CornerRadius> MenuCornerRadiusProperty = Menu.MenuCornerRadiusProperty.AddOwner<ContextMenu>();

        /// <summary>
        /// Get or set if a vertical scroll bar is visible for the context menu.
        /// If the context menu is longer than the screen, the scroll bar can be used.
        /// </summary>
        public ScrollBarVisibility VerticalScrollBarVisibility { get => GetValue(VerticalScrollBarVisibilityProperty); set => SetValue(VerticalScrollBarVisibilityProperty, value); }

        /// <summary>The backing styled property for <see cref="VerticalScrollBarVisibility"/>. See the related property for details.</summary>
        public static readonly StyledProperty<ScrollBarVisibility> VerticalScrollBarVisibilityProperty
            = AvaloniaProperty.Register<ContextMenu, ScrollBarVisibility>(nameof(VerticalScrollBarVisibility), ScrollBarVisibility.Auto);

    }
}
