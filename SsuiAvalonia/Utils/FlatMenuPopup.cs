using Avalonia.Media;
using Avalonia;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls.Primitives;

namespace SolidShineUi.Utils
{
    /// <summary>
    /// A helper class for <see cref="Menu"/>, to pass properties down to the menus and menu items in the menu bar.
    /// </summary>
    public class FlatMenuPopup : Popup
    {

        #region Brushes

        /// <summary>
        /// Get or set the brush used for the background of the menu's drop-down area. This is different from the Background brush, which set the background of the top menu bar.
        /// </summary>
        [Category("Brushes")]
        public IBrush? MenuBackground { get => GetValue(MenuBackgroundProperty); set => SetValue(MenuBackgroundProperty, value); }

        /// <summary>The backing styled property for <see cref="MenuBackground"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> MenuBackgroundProperty
            = AvaloniaProperty.Register<FlatMenuPopup, IBrush?>(nameof(MenuBackground), new SolidColorBrush(Colors.White));

        /// <summary>
        /// Get or set the brush used for the foreground of menu items that are disabled.
        /// </summary>
        [Category("Brushes")]
        public IBrush? DisabledBrush { get => GetValue(DisabledBrushProperty); set => SetValue(DisabledBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="DisabledBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> DisabledBrushProperty
            = AvaloniaProperty.Register<FlatMenuPopup, IBrush?>(nameof(DisabledBrush), new SolidColorBrush(Colors.Gray));

        /// <summary>
        /// Get or set the brush used for the border of the menu's drop-down area.
        /// </summary>
        [Category("Brushes")]
        public IBrush? PopupBorderBrush { get => GetValue(PopupBorderBrushProperty); set => SetValue(PopupBorderBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="PopupBorderBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> PopupBorderBrushProperty
            = AvaloniaProperty.Register<FlatMenuPopup, IBrush?>(nameof(PopupBorderBrush), new SolidColorBrush(Colors.Gray));

        /// <summary>
        /// Get or set the brush used for menu items that aren't top-level on the menu bar, when the mouse is over them or they have keyboard focus.
        /// </summary>
        [Category("Brushes")]
        public IBrush? HighlightSubitemBrush { get => GetValue(HighlightSubitemBrushProperty); set => SetValue(HighlightSubitemBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="HighlightSubitemBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> HighlightSubitemBrushProperty
            = AvaloniaProperty.Register<FlatMenuPopup, IBrush?>(nameof(HighlightSubitemBrush), new SolidColorBrush(Colors.LightGray));

        /// <summary>
        /// Get or set the brush used for the top-level menu items in the menu bar, when the mouse is over them or they have keyboard focus.
        /// </summary>
        [Category("Brushes")]
        public IBrush? HighlightBrush { get => GetValue(HighlightBrushProperty); set => SetValue(HighlightBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="HighlightBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> HighlightBrushProperty
            = AvaloniaProperty.Register<FlatMenuPopup, IBrush?>(nameof(HighlightBrush), new SolidColorBrush(Colors.LightGray));

        /// <summary>
        /// Get or set the brush to use for the check highlight for checked menu items.
        /// </summary>
        [Category("Brushes")]
        public IBrush? CheckedBrush { get => GetValue(CheckedBrushProperty); set => SetValue(CheckedBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="CheckedBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> CheckedBrushProperty
            = AvaloniaProperty.Register<FlatMenuPopup, IBrush?>(nameof(CheckedBrush), new SolidColorBrush(Colors.LightGray));

        public IBrush? Foreground { get => GetValue(ForegroundProperty); set => SetValue(ForegroundProperty, value); }

        /// <summary>The backing styled property for <see cref="Foreground"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> ForegroundProperty
            = AvaloniaProperty.Register<FlatMenuPopup, IBrush?>(nameof(Foreground), new SolidColorBrush(Colors.Black));

        #endregion

        /// <summary>
        /// Get or set the corner radius to apply to the opened menu drop-downs.
        /// </summary>
        public CornerRadius MenuCornerRadius { get => GetValue(MenuCornerRadiusProperty); set => SetValue(MenuCornerRadiusProperty, value); }

        /// <summary>The backing styled property for <see cref="MenuCornerRadius"/>. See the related property for details.</summary>
        public static readonly StyledProperty<CornerRadius> MenuCornerRadiusProperty
            = AvaloniaProperty.Register<FlatMenuPopup, CornerRadius>(nameof(MenuCornerRadius), new CornerRadius(0));

    }
}
