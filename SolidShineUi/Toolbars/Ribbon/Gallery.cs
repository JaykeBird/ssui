using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SolidShineUi.Toolbars.Ribbon
{
    /// <summary>
    /// A dynamic control for use on a <see cref="Ribbon"/>, designed to display a list of options with a more visual layout.
    /// </summary>
    public class Gallery : Control, IRibbonItem
    {
        static Gallery()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Gallery), new FrameworkPropertyMetadata(typeof(Gallery)));
        }

        /// <summary>
        /// Create a Gallery.
        /// </summary>
        public Gallery()
        {

        }

        public bool IsCompacted { get => (bool)GetValue(IsCompactedProperty); set => SetValue(IsCompactedProperty, value); }

        /// <summary>The backing dependency property for <see cref="IsCompacted"/>. See the related property for details.</summary>
        public static DependencyProperty IsCompactedProperty
            = DependencyProperty.Register("IsCompacted", typeof(bool), typeof(Gallery),
            new FrameworkPropertyMetadata(false));

        public RibbonElementSize StandardSize { get => (RibbonElementSize)GetValue(StandardSizeProperty); set => SetValue(StandardSizeProperty, value); }

        /// <summary>The backing dependency property for <see cref="StandardSize"/>. See the related property for details.</summary>
        public static DependencyProperty StandardSizeProperty
            = DependencyProperty.Register("StandardSize", typeof(RibbonElementSize), typeof(Gallery),
            new FrameworkPropertyMetadata(RibbonElementSize.Content));

        public RibbonElementSize CompactSize { get => (RibbonElementSize)GetValue(CompactSizeProperty); set => SetValue(CompactSizeProperty, value); }

        /// <summary>The backing dependency property for <see cref="CompactSize"/>. See the related property for details.</summary>
        public static DependencyProperty CompactSizeProperty
            = DependencyProperty.Register("CompactSize", typeof(RibbonElementSize), typeof(Gallery),
            new FrameworkPropertyMetadata(RibbonElementSize.Small));

        public string AccessKey { get => (string)GetValue(AccessKeyProperty); set => SetValue(AccessKeyProperty, value); }

        /// <summary>The backing dependency property for <see cref="AccessKey"/>. See the related property for details.</summary>
        public static DependencyProperty AccessKeyProperty
            = DependencyProperty.Register("AccessKey", typeof(string), typeof(Gallery),
            new FrameworkPropertyMetadata("G"));

        public string Title { get => (string)GetValue(TitleProperty); set => SetValue(TitleProperty, value); }

        /// <summary>The backing dependency property for <see cref="Title"/>. See the related property for details.</summary>
        public static DependencyProperty TitleProperty
            = DependencyProperty.Register("Title", typeof(string), typeof(Gallery),
            new FrameworkPropertyMetadata("Gallery"));

        public ImageSource LargeIcon { get => (ImageSource)GetValue(LargeIconProperty); set => SetValue(LargeIconProperty, value); }

        /// <summary>The backing dependency property for <see cref="LargeIcon"/>. See the related property for details.</summary>
        public static DependencyProperty LargeIconProperty
            = DependencyProperty.Register("LargeIcon", typeof(ImageSource), typeof(Gallery),
            new FrameworkPropertyMetadata(null));

        public ImageSource SmallIcon { get => (ImageSource)GetValue(SmallIconProperty); set => SetValue(SmallIconProperty, value); }

        /// <summary>The backing dependency property for <see cref="SmallIcon"/>. See the related property for details.</summary>
        public static DependencyProperty SmallIconProperty
            = DependencyProperty.Register("SmallIcon", typeof(ImageSource), typeof(Gallery),
            new FrameworkPropertyMetadata(null));

        public int CompactOrder { get => (int)GetValue(CompactOrderProperty); set => SetValue(CompactOrderProperty, value); }

        /// <summary>The backing dependency property for <see cref="CompactOrder"/>. See the related property for details.</summary>
        public static DependencyProperty CompactOrderProperty
            = DependencyProperty.Register("CompactOrder", typeof(int), typeof(Gallery),
            new FrameworkPropertyMetadata(0));

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
            = DependencyProperty.Register("ColorScheme", typeof(ColorScheme), typeof(Gallery),
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

            if (d is Gallery c)
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

        public GalleryItemLayout ItemLayout { get => (GalleryItemLayout)GetValue(ItemLayoutProperty); set => SetValue(ItemLayoutProperty, value); }

        /// <summary>The backing dependency property for <see cref="ItemLayout"/>. See the related property for details.</summary>
        public static DependencyProperty ItemLayoutProperty
            = DependencyProperty.Register("ItemLayout", typeof(GalleryItemLayout), typeof(Gallery),
            new FrameworkPropertyMetadata(GalleryItemLayout.LargeIconAndText));

        public int MaxItemsDisplayed { get => (int)GetValue(MaxItemsDisplayedProperty); set => SetValue(MaxItemsDisplayedProperty, value); }

        /// <summary>The backing dependency property for <see cref="MaxItemsDisplayed"/>. See the related property for details.</summary>
        public static DependencyProperty MaxItemsDisplayedProperty
            = DependencyProperty.Register("MaxItemsDisplayed", typeof(int), typeof(Gallery),
            new FrameworkPropertyMetadata(9));

        public double MaxItemWidth { get => (double)GetValue(MaxItemWidthProperty); set => SetValue(MaxItemWidthProperty, value); }

        /// <summary>The backing dependency property for <see cref="MaxItemWidth"/>. See the related property for details.</summary>
        public static DependencyProperty MaxItemWidthProperty
            = DependencyProperty.Register("MaxItemWidth", typeof(double), typeof(Gallery),
            new FrameworkPropertyMetadata(150.0));


    }
}
