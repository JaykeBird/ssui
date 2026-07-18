using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace SolidShineUi
{

    /// <summary>
    /// A popup context menu, with a large section also available for displaying custom content.
    /// </summary>
    public class ContentContextMenu : ContextMenu
    {
        static ContentContextMenu()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ContentContextMenu), new FrameworkPropertyMetadata(typeof(ContentContextMenu)));
        }


        #region Content Properties

        // see https://github.com/dotnet/wpf/blob/main/src/Microsoft.DotNet.Wpf/src/PresentationFramework/System/Windows/Controls/ContentControl.cs

        /// <summary>
        /// Get or set the content to display in the content area of the control.
        /// </summary>
        [Category("Common")]
        public object Content { get => GetValue(ContentProperty); set => SetValue(ContentProperty, value); }

        /// <summary>The backing dependency property for <see cref="Content"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ContentProperty = ContentControl.ContentProperty.AddOwner(typeof(ContentContextMenu),
            new FrameworkPropertyMetadata(OnContentChanged));

        private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ContentContextMenu cc)
            {
                cc.HasContent = e.NewValue != null;
            }
        }

        /// <summary>
        /// Get if this control currently has a non-null <c>Content</c> value.
        /// </summary>
        public bool HasContent { get => (bool)GetValue(HasContentProperty); private set => SetValue(HasContentPropertyKey, value); }

        private static readonly DependencyPropertyKey HasContentPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(HasContent), typeof(bool), typeof(ContentContextMenu),
            new FrameworkPropertyMetadata(false));

        /// <summary>The backing dependency property for <see cref="HasContent"/>. See the related property for details.</summary>
        public static readonly DependencyProperty HasContentProperty = HasContentPropertyKey.DependencyProperty;

        /// <summary>
        /// Get or set the template to use for displaying the <see cref="Content"/> in this control.
        /// </summary>
        public DataTemplate ContentTemplate { get => (DataTemplate)GetValue(ContentTemplateProperty); set => SetValue(ContentTemplateProperty, value); }

        /// <summary>The backing dependency property for <see cref="ContentTemplate"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ContentTemplateProperty = ContentControl.ContentTemplateProperty.AddOwner(typeof(ContentContextMenu));

        /// <summary>
        /// Get or set a composite string that specifies how to format the <see cref="Content"/> property if it is displayed as a string.
        /// </summary>
        public string ContentStringFormat { get => (string)GetValue(ContentStringFormatProperty); set => SetValue(ContentStringFormatProperty, value); }

        /// <summary>The backing dependency property for <see cref="ContentStringFormat"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ContentStringFormatProperty = ContentControl.ContentStringFormatProperty.AddOwner(typeof(ContentContextMenu));

        /// <summary>
        /// Get or set a template selector that can provide custom template-selection logic.
        /// </summary>
        public DataTemplateSelector ContentTemplateSelector { get => (DataTemplateSelector)GetValue(ContentTemplateSelectorProperty); set => SetValue(ContentTemplateSelectorProperty, value); }

        /// <summary>The backing dependency property for <see cref="ContentTemplateSelector"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ContentTemplateSelectorProperty = ContentControl.ContentTemplateSelectorProperty.AddOwner(typeof(ContentContextMenu));

        #endregion

        #region Menu Placement/Appearance

        /// <summary>
        /// Get or set the side of the control to place the menu items in <c>Items</c>.
        /// </summary>
        [Category("Common")]
        public PlacementDirection MenuPlacement { get => (PlacementDirection)GetValue(MenuPlacementProperty); set => SetValue(MenuPlacementProperty, value); }

        /// <summary>The backing dependency property for <see cref="MenuPlacement"/>. See the related property for details.</summary>
        public static readonly DependencyProperty MenuPlacementProperty
            = DependencyProperty.Register(nameof(MenuPlacement), typeof(PlacementDirection), typeof(ContentContextMenu),
            new FrameworkPropertyMetadata(PlacementDirection.Bottom));

        /// <summary>
        /// Get or set the padding to have around the menu items in the menu part of the control. Default value is 2.
        /// </summary>
        [Category("Appearance")]
        public Thickness MenuItemsPadding { get => (Thickness)GetValue(MenuItemsPaddingProperty); set => SetValue(MenuItemsPaddingProperty, value); }

        /// <summary>The backing dependency property for <see cref="MenuItemsPadding"/>. See the related property for details.</summary>
        public static readonly DependencyProperty MenuItemsPaddingProperty
            = DependencyProperty.Register(nameof(MenuItemsPadding), typeof(Thickness), typeof(ContentContextMenu),
            new FrameworkPropertyMetadata(new Thickness(1)));

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
            = DependencyProperty.Register(nameof(SeparatorBrush), typeof(Brush), typeof(ContentContextMenu),
            new FrameworkPropertyMetadata(null));

        #endregion

        #region ColorScheme / SsuiTheme

        /// <inheritdoc/>
        protected override void OnApplySsuiTheme(SsuiTheme ssuiTheme, bool useLightBorder = false, bool useAccentTheme = false)
        {
            base.OnApplySsuiTheme(ssuiTheme, useLightBorder, useAccentTheme);

            if (ssuiTheme is SsuiAppTheme sat)
            {
                if (sat.UseSubitemThemeWithMenus)
                {
                    ApplyTheme(useAccentTheme ? sat.AccentTheme : ssuiTheme);
                }
                else if (useAccentTheme)
                {
                    ApplyTheme(sat.AccentTheme);
                }
                else
                {
                    ApplyTheme(ssuiTheme);
                }
            }
            else
            {
                ApplyTheme(ssuiTheme);
            }

            void ApplyTheme(SsuiTheme theme)
            {
                // ApplyThemeBinding(BorderBrushProperty, useLightBorder ? SsuiTheme.LightBorderBrushProperty : SsuiTheme.BorderBrushProperty, theme);
                // ApplyThemeBinding(MenuBorderBrushProperty, useLightBorder ? SsuiTheme.LightBorderBrushProperty : SsuiTheme.BorderBrushProperty, theme);

                ApplyThemeBinding(SeparatorBrushProperty, useLightBorder ? SsuiTheme.LightBorderBrushProperty : SsuiTheme.BorderBrushProperty, theme);

                ApplyThemeBinding(BackgroundProperty, SsuiTheme.PanelBackgroundProperty, theme);
            }
        }

        #endregion

    }
}
