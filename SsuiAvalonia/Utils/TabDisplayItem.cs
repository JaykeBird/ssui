using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using SolidShineUi;
using TabItem = SolidShineUi.TabItem;

namespace SolidShineUi.Utils
{

    public class TabDisplayItem : TemplatedControl
    {

        public TabItem TabItem { get => GetValue(TabItemProperty); set => SetValue(TabItemProperty, value); }

        /// <summary>The backing styled property for <see cref="TabItem"/>. See the related property for details.</summary>
        public static readonly StyledProperty<TabItem> TabItemProperty
            = AvaloniaProperty.Register<TabDisplayItem, TabItem>(nameof(TabItem), new TabItem());

        public TabControl? ParentTabControl { get => GetValue(ParentTabControlProperty); set => SetValue(ParentTabControlProperty, value); }

        /// <summary>The backing styled property for <see cref="ParentTabControl"/>. See the related property for details.</summary>
        public static readonly StyledProperty<TabControl?> ParentTabControlProperty
            = AvaloniaProperty.Register<TabDisplayItem, TabControl?>(nameof(ParentTabControl), null);

        public bool ShowTabOnBottom { get => GetValue(ShowTabOnBottomProperty); set => SetValue(ShowTabOnBottomProperty, value); }

        /// <summary>The backing styled property for <see cref="ShowTabOnBottom"/>. See the related property for details.</summary>
        public static readonly StyledProperty<bool> ShowTabOnBottomProperty
            = AvaloniaProperty.Register<TabDisplayItem, bool>(nameof(ShowTabOnBottom), false);

        #region State

        public bool IsSelected { get => GetValue(IsSelectedProperty); set => SetValue(IsSelectedProperty, value); }

        /// <summary>The backing styled property for <see cref="IsSelected"/>. See the related property for details.</summary>
        public static readonly StyledProperty<bool> IsSelectedProperty
            = AvaloniaProperty.Register<TabDisplayItem, bool>(nameof(IsSelected), false);


        private bool _isHighlighting = false;

        public bool IsHighlighting { get => _isHighlighting; private set => SetAndRaise(IsHighlightingProperty, ref _isHighlighting, value); }

        /// <summary>The backing direct property for <see cref="IsHighlighting"/>. See the related property for details.</summary>
        public static readonly DirectProperty<TabDisplayItem, bool> IsHighlightingProperty
            = AvaloniaProperty.RegisterDirect<TabDisplayItem, bool>(nameof(IsHighlighting), (s) => s.IsHighlighting, unsetValue: false);

        #endregion

        #region DirtyState

        public bool IsDirty { get => GetValue(IsDirtyProperty); set => SetValue(IsDirtyProperty, value); }

        /// <summary>The backing styled property for <see cref="IsDirty"/>. See the related property for details.</summary>
        public static readonly StyledProperty<bool> IsDirtyProperty
            = AvaloniaProperty.Register<TabDisplayItem, bool>(nameof(IsDirty), false);

        public bool DisplayDirtyState { get => GetValue(DisplayDirtyStateProperty); set => SetValue(DisplayDirtyStateProperty, value); }

        /// <summary>The backing styled property for <see cref="DisplayDirtyState"/>. See the related property for details.</summary>
        public static readonly StyledProperty<bool> DisplayDirtyStateProperty
            = AvaloniaProperty.Register<TabDisplayItem, bool>(nameof(DisplayDirtyState), false);


        #endregion

        #region Brushes

        public IBrush TabBorderBrush { get => GetValue(TabBorderBrushProperty); set => SetValue(TabBorderBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="TabBorderBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush> TabBorderBrushProperty
            = AvaloniaProperty.Register<TabDisplayItem, IBrush>(nameof(TabBorderBrush), Colors.Black.ToBrush());

        public IBrush CloseBrush { get => GetValue(CloseBrushProperty); set => SetValue(CloseBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="CloseBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush> CloseBrushProperty
            = AvaloniaProperty.Register<TabDisplayItem, IBrush>(nameof(CloseBrush), Colors.Black.ToBrush());

        public IBrush? SelectedTabBackground { get => GetValue(SelectedTabBackgroundProperty); set => SetValue(SelectedTabBackgroundProperty, value); }

        /// <summary>The backing styled property for <see cref="SelectedTabBackground"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> SelectedTabBackgroundProperty
            = AvaloniaProperty.Register<TabDisplayItem, IBrush?>(nameof(SelectedTabBackground), Colors.White.ToBrush());

        public IBrush? BorderHighlightBrush { get => GetValue(BorderHighlightBrushProperty); set => SetValue(BorderHighlightBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="BorderHighlightBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> BorderHighlightBrushProperty
            = AvaloniaProperty.Register<TabDisplayItem, IBrush?>(nameof(BorderHighlightBrush), Colors.Black.ToBrush());

        public IBrush? HighlightBrush { get => GetValue(HighlightBrushProperty); set => SetValue(HighlightBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="HighlightBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> HighlightBrushProperty
            = AvaloniaProperty.Register<TabDisplayItem, IBrush?>(nameof(HighlightBrush), Colors.White.ToBrush());

        public IBrush? ButtonClickBrush { get => GetValue(ButtonClickBrushProperty); set => SetValue(ButtonClickBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="ButtonClickBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> ButtonClickBrushProperty
            = AvaloniaProperty.Register<TabDisplayItem, IBrush?>(nameof(ButtonClickBrush), Colors.LightGray.ToBrush());

        public IBrush? ButtonHighlightBrush { get => GetValue(ButtonHighlightBrushProperty); set => SetValue(ButtonHighlightBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="ButtonHighlightBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> ButtonHighlightBrushProperty
            = AvaloniaProperty.Register<TabDisplayItem, IBrush?>(nameof(ButtonHighlightBrush), Colors.Gainsboro.ToBrush());

        public IBrush? ButtonBorderHighlightBrush { get => GetValue(ButtonBorderHighlightBrushProperty); set => SetValue(ButtonBorderHighlightBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="ButtonBorderHighlightBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush?> ButtonBorderHighlightBrushProperty
            = AvaloniaProperty.Register<TabDisplayItem, IBrush?>(nameof(ButtonBorderHighlightBrush), Colors.DimGray.ToBrush());


        #endregion
    }
}
