using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using System.ComponentModel;

namespace SolidShineUi.Utils.MenuUtils
{

    /// <summary>
    /// A modified version of <see cref="ScrollViewer"/> with additional visual properties, used internally with <see cref="Menu"/>.
    /// </summary>
    public class FlatMenuScrollViewer : ScrollViewer
    {

        #region Brushes

        /// <summary>
        /// Get or set the brush used for the background of the menu's drop-down area. This is different from the Background brush, which set the background of the top menu bar.
        /// </summary>
        [Category("Brushes")]
        public IBrush? MenuBackground { get => GetValue(MenuBackgroundProperty); set => SetValue(MenuBackgroundProperty, value); }

        /// <summary>The backing styled property for <see cref="MenuBackground"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> MenuBackgroundProperty = Menu.MenuBackgroundProperty.AddOwner<FlatMenuScrollViewer>();

        /// <summary>
        /// Get or set the brush used for the foreground of menu items that are disabled.
        /// </summary>
        [Category("Brushes")]
        public IBrush? DisabledBrush { get => GetValue(DisabledBrushProperty); set => SetValue(DisabledBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="DisabledBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> DisabledBrushProperty = Menu.DisabledBrushProperty.AddOwner<FlatMenuScrollViewer>();

        /// <summary>
        /// Get or set the brush used for the border of the menu's drop-down area.
        /// </summary>
        [Category("Brushes")]
        public IBrush? PopupBorderBrush { get => GetValue(PopupBorderBrushProperty); set => SetValue(PopupBorderBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="PopupBorderBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> PopupBorderBrushProperty = Menu.PopupBorderBrushProperty.AddOwner<FlatMenuScrollViewer>();

        /// <summary>
        /// Get or set the brush used for menu items that aren't top-level on the menu bar, when the mouse is over them or they have keyboard focus.
        /// </summary>
        [Category("Brushes")]
        public IBrush? HighlightSubitemBrush { get => GetValue(HighlightSubitemBrushProperty); set => SetValue(HighlightSubitemBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="HighlightSubitemBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> HighlightSubitemBrushProperty = Menu.HighlightSubitemBrushProperty.AddOwner<FlatMenuScrollViewer>();

        /// <summary>
        /// Get or set the brush used for the top-level menu items in the menu bar, when the mouse is over them or they have keyboard focus.
        /// </summary>
        [Category("Brushes")]
        public IBrush? HighlightBrush { get => GetValue(HighlightBrushProperty); set => SetValue(HighlightBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="HighlightBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> HighlightBrushProperty = Menu.HighlightBrushProperty.AddOwner<FlatMenuScrollViewer>();

        /// <summary>
        /// Get or set the brush to use for the check highlight for checked menu items.
        /// </summary>
        [Category("Brushes")]
        public IBrush? CheckedBrush { get => GetValue(CheckedBrushProperty); set => SetValue(CheckedBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="CheckedBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> CheckedBrushProperty = Menu.CheckedBrushProperty.AddOwner<FlatMenuScrollViewer>();

        /// <summary>
        /// Get or set the brush to use when clicking on a menu item.
        /// </summary>
        [Category("Brushes")]
        public IBrush? ClickBrush { get => GetValue(ClickBrushProperty); set => SetValue(ClickBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="ClickBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> ClickBrushProperty = Menu.ClickBrushProperty.AddOwner<FlatMenuScrollViewer>();

        ///// <summary>
        ///// Get or set the foreground color to use for text and other elements in the menu.
        ///// </summary>
        //public IBrush? Foreground { get => GetValue(ForegroundProperty); set => SetValue(ForegroundProperty, value); }

        ///// <summary>The backing styled property for <see cref="Foreground"/>. See the related property for details.</summary>
        //public static readonly StyledProperty<IBrush?> ForegroundProperty = TemplatedControl.ForegroundProperty.AddOwner<FlatMenuScrollViewer>();

        /// <summary>
        /// Get or set the brush used for the foreground elements of the top-level menu items in the menu bar, 
        /// when the mouse is over them or they have keyboard focus.
        /// </summary>
        [Category("Brushes")]
        public IBrush? HighlightForegroundBrush { get => GetValue(HighlightForegroundBrushProperty); set => SetValue(HighlightForegroundBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="HighlightForegroundBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> HighlightForegroundBrushProperty
            = Menu.HighlightForegroundBrushProperty.AddOwner<FlatMenuScrollViewer>();

        /// <summary>
        /// Get or set the brush used for the foreground elements of menu items that aren't top-level on the menu bar, 
        /// when the mouse is over them or they have keyboard focus.
        /// </summary>
        [Category("Brushes")]
        public IBrush? HighlightSubitemForegroundBrush { get => GetValue(HighlightSubitemForegroundBrushProperty); set => SetValue(HighlightSubitemForegroundBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="HighlightSubitemForegroundBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> HighlightSubitemForegroundBrushProperty
            = Menu.HighlightSubitemForegroundBrushProperty.AddOwner<FlatMenuScrollViewer>();

        /// <summary>
        /// Get or set the brush used for the border around the top-level menu items in the menu bar, 
        /// when the mouse is over them or they have keyboard focus.
        /// </summary>
        [Category("Brushes")]
        public IBrush? HighlightBorderBrush { get => GetValue(HighlightBorderBrushProperty); set => SetValue(HighlightBorderBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="HighlightBorderBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> HighlightBorderBrushProperty
            = Menu.HighlightBorderBrushProperty.AddOwner<FlatMenuScrollViewer>();

        /// <summary>
        /// Get or set the brush used for the border around the menu items that aren't top-level on the menu bar, 
        /// when the mouse is over them or they have keyboard focus.
        /// </summary>
        [Category("Brushes")]
        public IBrush? HighlightSubitemBorderBrush { get => GetValue(HighlightSubitemBorderBrushProperty); set => SetValue(HighlightSubitemBorderBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="HighlightSubitemBorderBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> HighlightSubitemBorderBrushProperty
            = Menu.HighlightSubitemBorderBrushProperty.AddOwner<FlatMenuScrollViewer>();

        /// <summary>
        /// Get or set the brush used for the border of the menu's drop-down area.
        /// </summary>
        [Category("Brushes")]
        public IBrush? MenuBorderBrush { get => GetValue(MenuBorderBrushProperty); set => SetValue(MenuBorderBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="MenuBorderBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> MenuBorderBrushProperty
            = Menu.MenuBorderBrushProperty.AddOwner<FlatMenuScrollViewer>();

        #endregion

        /// <summary>
        /// Get or set the corner radius to apply to the opened menu drop-downs.
        /// </summary>
        public CornerRadius MenuCornerRadius { get => GetValue(MenuCornerRadiusProperty); set => SetValue(MenuCornerRadiusProperty, value); }

        /// <summary>The backing styled property for <see cref="MenuCornerRadius"/>. See the related property for details.</summary>
        public static readonly StyledProperty<CornerRadius> MenuCornerRadiusProperty = Menu.MenuCornerRadiusProperty.AddOwner<FlatMenuScrollViewer>();
    }
}
