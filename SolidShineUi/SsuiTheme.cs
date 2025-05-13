using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SolidShineUi
{

    public class SsuiTheme : DependencyObject
    {

        public SsuiTheme()
        {

        }

        public SsuiTheme(Color baseColor)
        {

        }

        public SsuiTheme(SsuiTheme other)
        {

        }

        public SsuiTheme(ColorScheme cs)
        {

        }

        public Brush ControlBackground { get => (Brush)GetValue(ControlBackgroundProperty); set => SetValue(ControlBackgroundProperty, value); }

        /// <summary>The backing dependency property for <see cref="ControlBackground"/>. See the related property for details.</summary>
        public static DependencyProperty ControlBackgroundProperty
            = DependencyProperty.Register(nameof(ControlBackground), typeof(Brush), typeof(SsuiTheme),
            new FrameworkPropertyMetadata(Colors.Black.ToBrush()));


        public Brush PanelBackground { get => (Brush)GetValue(PanelBackgroundProperty); set => SetValue(PanelBackgroundProperty, value); }

        /// <summary>The backing dependency property for <see cref="PanelBackground"/>. See the related property for details.</summary>
        public static DependencyProperty PanelBackgroundProperty
            = DependencyProperty.Register(nameof(PanelBackground), typeof(Brush), typeof(SsuiTheme),
            new FrameworkPropertyMetadata(Colors.Black.ToBrush()));


        public Brush ControlSatBackground { get => (Brush)GetValue(ControlSatBackgroundProperty); set => SetValue(ControlSatBackgroundProperty, value); }

        /// <summary>The backing dependency property for <see cref="ControlSatBackground"/>. See the related property for details.</summary>
        public static DependencyProperty ControlSatBackgroundProperty
            = DependencyProperty.Register(nameof(ControlSatBackground), typeof(Brush), typeof(SsuiTheme),
            new FrameworkPropertyMetadata(Colors.Black.ToBrush()));


        public Brush BorderBrush { get => (Brush)GetValue(BorderBrushProperty); set => SetValue(BorderBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="BorderBrush"/>. See the related property for details.</summary>
        public static DependencyProperty BorderBrushProperty
            = DependencyProperty.Register(nameof(BorderBrush), typeof(Brush), typeof(SsuiTheme),
            new FrameworkPropertyMetadata(Colors.Black.ToBrush()));


        public Brush Foreground { get => (Brush)GetValue(ForegroundProperty); set => SetValue(ForegroundProperty, value); }

        /// <summary>The backing dependency property for <see cref="Foreground"/>. See the related property for details.</summary>
        public static DependencyProperty ForegroundProperty
            = DependencyProperty.Register(nameof(Foreground), typeof(Brush), typeof(SsuiTheme),
            new FrameworkPropertyMetadata(Colors.Black.ToBrush()));


        public Brush DisabledBackground { get => (Brush)GetValue(DisabledBackgroundProperty); set => SetValue(DisabledBackgroundProperty, value); }

        /// <summary>The backing dependency property for <see cref="DisabledBackground"/>. See the related property for details.</summary>
        public static DependencyProperty DisabledBackgroundProperty
            = DependencyProperty.Register(nameof(DisabledBackground), typeof(Brush), typeof(SsuiTheme),
            new FrameworkPropertyMetadata(Colors.Black.ToBrush()));


        public Brush DisabledBorderBrush { get => (Brush)GetValue(DisabledBorderBrushProperty); set => SetValue(DisabledBorderBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="DisabledBorderBrush"/>. See the related property for details.</summary>
        public static DependencyProperty DisabledBorderBrushProperty
            = DependencyProperty.Register(nameof(DisabledBorderBrush), typeof(Brush), typeof(SsuiTheme),
            new FrameworkPropertyMetadata(Colors.Black.ToBrush()));


        public Brush DisabledForeground { get => (Brush)GetValue(DisabledForegroundProperty); set => SetValue(DisabledForegroundProperty, value); }

        /// <summary>The backing dependency property for <see cref="DisabledForeground"/>. See the related property for details.</summary>
        public static DependencyProperty DisabledForegroundProperty
            = DependencyProperty.Register(nameof(DisabledForeground), typeof(Brush), typeof(SsuiTheme),
            new FrameworkPropertyMetadata(Colors.Black.ToBrush()));


        public Brush HighlightBrush { get => (Brush)GetValue(HighlightBrushProperty); set => SetValue(HighlightBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="HighlightBrush"/>. See the related property for details.</summary>
        public static DependencyProperty HighlightBrushProperty
            = DependencyProperty.Register(nameof(HighlightBrush), typeof(Brush), typeof(SsuiTheme),
            new FrameworkPropertyMetadata(Colors.Black.ToBrush()));


        public Brush ClickBrush { get => (Brush)GetValue(ClickBrushProperty); set => SetValue(ClickBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="ClickBrush"/>. See the related property for details.</summary>
        public static DependencyProperty ClickBrushProperty
            = DependencyProperty.Register(nameof(ClickBrush), typeof(Brush), typeof(SsuiTheme),
            new FrameworkPropertyMetadata(Colors.Black.ToBrush()));


        public Brush HighlightForeground { get => (Brush)GetValue(HighlightForegroundProperty); set => SetValue(HighlightForegroundProperty, value); }

        /// <summary>The backing dependency property for <see cref="HighlightForeground"/>. See the related property for details.</summary>
        public static DependencyProperty HighlightForegroundProperty
            = DependencyProperty.Register(nameof(HighlightForeground), typeof(Brush), typeof(SsuiTheme),
            new FrameworkPropertyMetadata(Colors.Black.ToBrush()));


        public Brush SelectedBackgroundBrush { get => (Brush)GetValue(SelectedBackgroundBrushProperty); set => SetValue(SelectedBackgroundBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="SelectedBackgroundBrush"/>. See the related property for details.</summary>
        public static DependencyProperty SelectedBackgroundBrushProperty
            = DependencyProperty.Register(nameof(SelectedBackgroundBrush), typeof(Brush), typeof(SsuiTheme),
            new FrameworkPropertyMetadata(Colors.Black.ToBrush()));


        public Brush LightBorderBrush { get => (Brush)GetValue(LightBorderBrushProperty); set => SetValue(LightBorderBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="LightBorderBrush"/>. See the related property for details.</summary>
        public static DependencyProperty LightBorderBrushProperty
            = DependencyProperty.Register(nameof(LightBorderBrush), typeof(Brush), typeof(SsuiTheme),
            new FrameworkPropertyMetadata(Colors.Black.ToBrush()));


        public Brush CheckBrush { get => (Brush)GetValue(CheckBrushProperty); set => SetValue(CheckBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="CheckBrush"/>. See the related property for details.</summary>
        public static DependencyProperty CheckBrushProperty
            = DependencyProperty.Register(nameof(CheckBrush), typeof(Brush), typeof(SsuiTheme),
            new FrameworkPropertyMetadata(Colors.Black.ToBrush()));


        public Brush ControlPopBrush { get => (Brush)GetValue(ControlPopBrushProperty); set => SetValue(ControlPopBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="ControlPopBrush"/>. See the related property for details.</summary>
        public static DependencyProperty ControlPopBrushProperty
            = DependencyProperty.Register(nameof(ControlPopBrush), typeof(Brush), typeof(SsuiTheme),
            new FrameworkPropertyMetadata(Colors.Black.ToBrush()));


        public CornerRadius CornerRadius { get => (CornerRadius)GetValue(CornerRadiusProperty); set => SetValue(CornerRadiusProperty, value); }

        /// <summary>The backing dependency property for <see cref="CornerRadius"/>. See the related property for details.</summary>
        public static DependencyProperty CornerRadiusProperty
            = DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(SsuiTheme),
            new FrameworkPropertyMetadata(new CornerRadius(0)));

    }


    public class SsuiAppTheme : SsuiTheme
    {
        public SsuiAppTheme() : base()
        {

        }

        public SsuiAppTheme(Color baseColor) : base(baseColor)
        {

        }

        public SsuiAppTheme(Color baseColor, Color accentColor) : base(baseColor)
        {

        }

        public SsuiAppTheme(Color baseColor, Color accentColor, Color subItemColor) : base(baseColor)
        {

        }

        public SsuiAppTheme(SsuiAppTheme other) : base(other)
        {

        }

        public SsuiAppTheme(ColorScheme cs) : base(cs)
        {

        }

        public Brush WindowTitleBackground { get => (Brush)GetValue(WindowTitleBackgroundProperty); set => SetValue(WindowTitleBackgroundProperty, value); }

        /// <summary>The backing dependency property for <see cref="WindowTitleBackground"/>. See the related property for details.</summary>
        public static DependencyProperty WindowTitleBackgroundProperty
            = DependencyProperty.Register(nameof(WindowTitleBackground), typeof(Brush), typeof(SsuiAppTheme),
            new FrameworkPropertyMetadata(Colors.Black.ToBrush()));


        public Brush WindowTitleForeground { get => (Brush)GetValue(WindowTitleForegroundProperty); set => SetValue(WindowTitleForegroundProperty, value); }

        /// <summary>The backing dependency property for <see cref="WindowTitleForeground"/>. See the related property for details.</summary>
        public static DependencyProperty WindowTitleForegroundProperty
            = DependencyProperty.Register(nameof(WindowTitleForeground), typeof(Brush), typeof(SsuiAppTheme),
            new FrameworkPropertyMetadata(Colors.Black.ToBrush()));


        public Brush WindowCaptionsBackground { get => (Brush)GetValue(WindowCaptionsBackgroundProperty); set => SetValue(WindowCaptionsBackgroundProperty, value); }

        /// <summary>The backing dependency property for <see cref="WindowCaptionsBackground"/>. See the related property for details.</summary>
        public static DependencyProperty WindowCaptionsBackgroundProperty
            = DependencyProperty.Register(nameof(WindowCaptionsBackground), typeof(Brush), typeof(SsuiAppTheme),
            new FrameworkPropertyMetadata(Colors.Black.ToBrush()));


        public Brush WindowCaptionsForeground { get => (Brush)GetValue(WindowCaptionsForegroundProperty); set => SetValue(WindowCaptionsForegroundProperty, value); }

        /// <summary>The backing dependency property for <see cref="WindowCaptionsForeground"/>. See the related property for details.</summary>
        public static DependencyProperty WindowCaptionsForegroundProperty
            = DependencyProperty.Register(nameof(WindowCaptionsForeground), typeof(Brush), typeof(SsuiAppTheme),
            new FrameworkPropertyMetadata(Colors.Black.ToBrush()));


        public Brush WindowCaptionsHighlight { get => (Brush)GetValue(WindowCaptionsHighlightProperty); set => SetValue(WindowCaptionsHighlightProperty, value); }

        /// <summary>The backing dependency property for <see cref="WindowCaptionsHighlight"/>. See the related property for details.</summary>
        public static DependencyProperty WindowCaptionsHighlightProperty
            = DependencyProperty.Register(nameof(WindowCaptionsHighlight), typeof(Brush), typeof(SsuiAppTheme),
            new FrameworkPropertyMetadata(Colors.Black.ToBrush()));


        public Brush WindowCaptionsHighlightForeground { get => (Brush)GetValue(WindowCaptionsHighlightForegroundProperty); set => SetValue(WindowCaptionsHighlightForegroundProperty, value); }

        /// <summary>The backing dependency property for <see cref="WindowCaptionsHighlightForeground"/>. See the related property for details.</summary>
        public static DependencyProperty WindowCaptionsHighlightForegroundProperty
            = DependencyProperty.Register(nameof(WindowCaptionsHighlightForeground), typeof(Brush), typeof(SsuiAppTheme),
            new FrameworkPropertyMetadata(Colors.Black.ToBrush()));


        public Brush WindowCaptionsClickBrush { get => (Brush)GetValue(WindowCaptionsClickBrushProperty); set => SetValue(WindowCaptionsClickBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="WindowCaptionsClickBrush"/>. See the related property for details.</summary>
        public static DependencyProperty WindowCaptionsClickBrushProperty
            = DependencyProperty.Register(nameof(WindowCaptionsClickBrush), typeof(Brush), typeof(SsuiAppTheme),
            new FrameworkPropertyMetadata(Colors.Black.ToBrush()));


        public Brush WindowBackground { get => (Brush)GetValue(WindowBackgroundProperty); set => SetValue(WindowBackgroundProperty, value); }

        /// <summary>The backing dependency property for <see cref="WindowBackground"/>. See the related property for details.</summary>
        public static DependencyProperty WindowBackgroundProperty
            = DependencyProperty.Register(nameof(WindowBackground), typeof(Brush), typeof(SsuiAppTheme),
            new FrameworkPropertyMetadata(Colors.Black.ToBrush()));


        public SsuiTheme AccentTheme { get => (SsuiTheme)GetValue(AccentThemeProperty); set => SetValue(AccentThemeProperty, value); }

        /// <summary>The backing dependency property for <see cref="AccentTheme"/>. See the related property for details.</summary>
        public static DependencyProperty AccentThemeProperty
            = DependencyProperty.Register(nameof(AccentTheme), typeof(SsuiTheme), typeof(SsuiAppTheme),
            new FrameworkPropertyMetadata(new SsuiTheme()));


        public SsuiTheme SubitemTheme { get => (SsuiTheme)GetValue(SubitemThemeProperty); set => SetValue(SubitemThemeProperty, value); }

        /// <summary>The backing dependency property for <see cref="SubitemTheme"/>. See the related property for details.</summary>
        public static DependencyProperty SubitemThemeProperty
            = DependencyProperty.Register(nameof(SubitemTheme), typeof(SsuiTheme), typeof(SsuiAppTheme),
            new FrameworkPropertyMetadata(new SsuiTheme()));


        public bool UseSubitemThemeWithMenus { get => (bool)GetValue(UseSubitemThemeWithMenusProperty); set => SetValue(UseSubitemThemeWithMenusProperty, value); }

        /// <summary>The backing dependency property for <see cref="UseSubitemThemeWithMenus"/>. See the related property for details.</summary>
        public static DependencyProperty UseSubitemThemeWithMenusProperty
            = DependencyProperty.Register(nameof(UseSubitemThemeWithMenus), typeof(bool), typeof(SsuiAppTheme),
            new FrameworkPropertyMetadata(false));


        public bool UseSubitemThemeWithPanels { get => (bool)GetValue(UseSubitemThemeWithPanelsProperty); set => SetValue(UseSubitemThemeWithPanelsProperty, value); }

        /// <summary>The backing dependency property for <see cref="UseSubitemThemeWithPanels"/>. See the related property for details.</summary>
        public static DependencyProperty UseSubitemThemeWithPanelsProperty
            = DependencyProperty.Register(nameof(UseSubitemThemeWithPanels), typeof(bool), typeof(SsuiAppTheme),
            new FrameworkPropertyMetadata(false));


        public bool UseSubitemThemeWithRibbons { get => (bool)GetValue(UseSubitemThemeWithRibbonsProperty); set => SetValue(UseSubitemThemeWithRibbonsProperty, value); }

        /// <summary>The backing dependency property for <see cref="UseSubitemThemeWithRibbons"/>. See the related property for details.</summary>
        public static DependencyProperty UseSubitemThemeWithRibbonsProperty
            = DependencyProperty.Register(nameof(UseSubitemThemeWithRibbons), typeof(bool), typeof(SsuiAppTheme),
            new FrameworkPropertyMetadata(true));

    }
}
