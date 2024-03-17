using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace SolidShineUi.Ribbon
{
    /// <summary>
    /// A wrapper for displaying a control within a <see cref="RibbonGroup"/> other than any of the premade controls in this namespace.
    /// </summary>
    [ContentProperty("Content")]
    public class RibbonContentControl : Control, IRibbonItem
    {
        static RibbonContentControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonContentControl), new FrameworkPropertyMetadata(typeof(RibbonContentControl)));
        }

        /// <summary>
        /// Get or set the size to use for this control.
        /// For <see cref="RibbonContentControl"/>, <c>IconOnly</c> is not valid (and will be treated as <c>Small</c>), and <c>Content</c> will be treated as <c>Large</c>.
        /// </summary>
        /// <remarks>
        /// When the parent group is compacted, it'll request all controls within the group to use its <see cref="CompactSize"/> instead of its <see cref="StandardSize"/>.
        /// <para/>
        /// See the remarks on the <see cref="Content"/> property to determine how the sizes should impact the heights of this control's content.
        /// </remarks>
        public RibbonElementSize StandardSize { get => (RibbonElementSize)GetValue(StandardSizeProperty); set => SetValue(StandardSizeProperty, value); }

        /// <summary>The backing dependency property for <see cref="StandardSize"/>. See the related property for details.</summary>
        public static DependencyProperty StandardSizeProperty
            = DependencyProperty.Register("StandardSize", typeof(RibbonElementSize), typeof(RibbonContentControl),
            new FrameworkPropertyMetadata(RibbonElementSize.Small));

        /// <summary>
        /// Get or set the size to use for this control, when the parent group is being compacted.
        /// For <see cref="RibbonContentControl"/>, <c>IconOnly</c> is not valid (and will be treated as <c>Small</c>), and <c>Content</c> will be treated as <c>Large</c>.
        /// </summary>
        /// <remarks>
        /// When the parent group is compacted, it'll request all controls within the group to use its <see cref="CompactSize"/> instead of its <see cref="StandardSize"/>.
        /// For important and commonly used controls in a group, the <c>CompactSize</c> may still be same type as the <c>StandardSize</c>, but for less important or 
        /// more infrequently used controls, it's recommended to go down a size value for <c>CompactSize</c>.
        /// <para/>
        /// See the remarks on the <see cref="Content"/> property to determine how the sizes should impact the heights of this control's content.
        /// </remarks>
        public RibbonElementSize CompactSize { get => (RibbonElementSize)GetValue(CompactSizeProperty); set => SetValue(CompactSizeProperty, value); }

        /// <summary>The backing dependency property for <see cref="CompactSize"/>. See the related property for details.</summary>
        public static DependencyProperty CompactSizeProperty
            = DependencyProperty.Register("CompactSize", typeof(RibbonElementSize), typeof(RibbonContentControl),
            new FrameworkPropertyMetadata(RibbonElementSize.Small));

        public string AccessKey { get => (string)GetValue(AccessKeyProperty); set => SetValue(AccessKeyProperty, value); }

        /// <summary>The backing dependency property for <see cref="AccessKey"/>. See the related property for details.</summary>
        public static DependencyProperty AccessKeyProperty
            = DependencyProperty.Register("AccessKey", typeof(string), typeof(RibbonContentControl),
            new FrameworkPropertyMetadata("I"));

        /// <summary>
        /// Get or set the title to display for this control.
        /// This is displayed to the left of the control, and can be hidden via <see cref="ShowTitle"/>.
        /// </summary>
        /// <remarks>
        /// Standard convention is to end the title with a colon to indicate it's referring to the content next to it,
        /// such as "Text:" instead of "Text". The title should describe what the control is being used to modify or do.
        /// </remarks>
        public string Title { get => (string)GetValue(TitleProperty); set => SetValue(TitleProperty, value); }

        /// <summary>The backing dependency property for <see cref="Title"/>. See the related property for details.</summary>
        public static DependencyProperty TitleProperty
            = DependencyProperty.Register("Title", typeof(string), typeof(RibbonContentControl),
            new FrameworkPropertyMetadata("Item"));

        /// <summary>
        /// Get or set if the <see cref="Title"/> should be displayed next to this control.
        /// </summary>
        /// <remarks>
        /// If both <see cref="ShowTitle"/> and <see cref="ShowIcon"/> are set to <c>false</c> while this control is set to <see cref="RibbonElementSize.Large"/>,
        /// then the content will be able to take up the full height of the Ribbon without a title or icon being displayed above it.
        /// </remarks>
        public bool ShowTitle { get => (bool)GetValue(ShowTitleProperty); set => SetValue(ShowTitleProperty, value); }

        /// <summary>The backing dependency property for <see cref="ShowTitle"/>. See the related property for details.</summary>
        public static DependencyProperty ShowTitleProperty
            = DependencyProperty.Register("ShowTitle", typeof(bool), typeof(RibbonContentControl),
            new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Get or set if the <see cref="SmallIcon"/> should be displayed next to the control.
        /// </summary>
        /// <remarks>
        /// If both <see cref="ShowTitle"/> and <see cref="ShowIcon"/> are set to <c>false</c> while this control is set to <see cref="RibbonElementSize.Large"/>,
        /// then the content will be able to take up the full height of the Ribbon without a title or icon being displayed above it.
        /// </remarks>
        public bool ShowIcon { get => (bool)GetValue(ShowIconProperty); set => SetValue(ShowIconProperty, value); }

        /// <summary>The backing dependency property for <see cref="ShowIcon"/>. See the related property for details.</summary>
        public static DependencyProperty ShowIconProperty
            = DependencyProperty.Register("ShowIcon", typeof(bool), typeof(RibbonContentControl),
            new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Get or set the large icon to use with this control. For <see cref="RibbonContentControl"/> (and all controls that are based on it),
        /// this property actually does nothing, as the <c>LargeIcon</c> is never displayed on the Ribbon. Instead, only set <see cref="SmallIcon"/>.
        /// </summary>
        /// <remarks>
        /// The large icon should be 32x32 in size.
        /// </remarks>
        public ImageSource LargeIcon { get => (ImageSource)GetValue(LargeIconProperty); set => SetValue(LargeIconProperty, value); }

        /// <summary>The backing dependency property for <see cref="LargeIcon"/>. See the related property for details.</summary>
        public static DependencyProperty LargeIconProperty
            = DependencyProperty.Register("LargeIcon", typeof(ImageSource), typeof(RibbonContentControl),
            new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Get or set the small icon to use with this control. 
        /// This will be displayed to the left of the control (and to the left of the <see cref="Title"/>), and can be hidden via <see cref="ShowIcon"/>.
        /// </summary>
        /// <remarks>
        /// The small icon should be 16x16 in size.
        /// </remarks>
        public ImageSource SmallIcon { get => (ImageSource)GetValue(SmallIconProperty); set => SetValue(SmallIconProperty, value); }

        /// <summary>The backing dependency property for <see cref="SmallIcon"/>. See the related property for details.</summary>
        public static DependencyProperty SmallIconProperty
            = DependencyProperty.Register("SmallIcon", typeof(ImageSource), typeof(RibbonContentControl),
            new FrameworkPropertyMetadata(null));

        public int CompactOrder { get => (int)GetValue(CompactOrderProperty); set => SetValue(CompactOrderProperty, value); }

        /// <summary>The backing dependency property for <see cref="CompactOrder"/>. See the related property for details.</summary>
        public static DependencyProperty CompactOrderProperty
            = DependencyProperty.Register("CompactOrder", typeof(int), typeof(RibbonContentControl),
            new FrameworkPropertyMetadata(0));

        /// <summary>
        /// Get or set if this control is currently being compacted (and thus should use <see cref="CompactSize"/> rather than <see cref="StandardSize"/>).
        /// Generally, this shouldn't be set manually; instead, the parent <see cref="RibbonGroup"/> will set this for controls when the group itself is being compacted.
        /// </summary>
        public bool IsCompacted { get => (bool)GetValue(IsCompactedProperty); set => SetValue(IsCompactedProperty, value); }

        /// <summary>The backing dependency property for <see cref="IsCompacted"/>. See the related property for details.</summary>
        public static DependencyProperty IsCompactedProperty
            = DependencyProperty.Register("IsCompacted", typeof(bool), typeof(RibbonContentControl),
            new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Get or set the content to display in this <see cref="RibbonContentControl"/>.
        /// </summary>
        /// <remarks>
        /// If this RibbonContentControl is set to <see cref="RibbonElementSize.Small"/>, then the content should be at most 24 pixels in height.
        /// If it is set to <see cref="RibbonElementSize.Large"/>, then the content should be either at most 48 pixels in height, or
        /// at most 72 pixels in height if both <see cref="ShowTitle"/> and <see cref="ShowIcon"/> are set to <c>false</c>. 
        /// These are the maximum heights to use so the content fits properly on the Ribbon, without causing any layout or visual problems.
        /// <para/>
        /// For controls that are based off this, such as <see cref="RibbonComboBox"/>, this property should not be set or changed.
        /// </remarks>
        public object Content { get => GetValue(ContentProperty); set => SetValue(ContentProperty, value); }

        /// <summary>The backing dependency property for <see cref="Content"/>. See the related property for details.</summary>
        public static DependencyProperty ContentProperty
            = DependencyProperty.Register("Content", typeof(object), typeof(RibbonContentControl),
            new FrameworkPropertyMetadata(null));

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
            = DependencyProperty.Register("ColorScheme", typeof(ColorScheme), typeof(RibbonContentControl),
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

            if (d is RibbonContentControl c)
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
        }
        #endregion
    }
}
