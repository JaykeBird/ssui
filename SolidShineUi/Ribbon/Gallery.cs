using SolidShineUi.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace SolidShineUi.Ribbon
{
    /// <summary>
    /// A dynamic control for use on a <see cref="Ribbon"/>, designed to display a list of options with a more visual layout.
    /// </summary>
    [ContentProperty("Items")]
    [Localizability(LocalizationCategory.ListBox)]
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
            Padding = new Thickness(1);
            Items.CollectionChanged += Items_CollectionChanged;

            DisplayedItems = new ListCollectionView(Items);
            DisplayedItems.Filter = FilterItems;
        }

        #region IRibbonItem implementations

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

        #endregion

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

            BorderBrush = cs.BorderColor.ToBrush();

            UpdateSubitemColors();

        }
        #endregion

        private static readonly DependencyPropertyKey ItemsPropertyKey
            = DependencyProperty.RegisterReadOnly("Items", typeof(ObservableCollection<GalleryItem>), typeof(Gallery),
            new FrameworkPropertyMetadata(new ObservableCollection<GalleryItem>()));

        /// <summary>
        /// The backing dependency property for <see cref="Items"/>. See the related property for details.
        /// </summary>
        public static readonly DependencyProperty ItemsProperty = ItemsPropertyKey.DependencyProperty;

        /// <summary>
        /// Get the list of items in this Gallery. This Items property can be used to add and remove items.
        /// </summary>
        [Category("Common")]
        public ObservableCollection<GalleryItem> Items
        {
            get { return (ObservableCollection<GalleryItem>)GetValue(ItemsProperty); }
            private set { SetValue(ItemsPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey DisplayedItemsPropertyKey
            = DependencyProperty.RegisterReadOnly("DisplayedItems", typeof(ListCollectionView), typeof(Gallery), new FrameworkPropertyMetadata());

        /// <summary>
        /// Get the handler that displays the items from <see cref="Items"/> that are currently visible in the Ribbon.
        /// </summary>
        public ListCollectionView DisplayedItems { get => (ListCollectionView)GetValue(DisplayedItemsProperty); private set => SetValue(DisplayedItemsPropertyKey, value); }

        /// <summary>The backing dependency property for <see cref="DisplayedItems"/>. See the related property for details.</summary>
        public static DependencyProperty DisplayedItemsProperty = DisplayedItemsPropertyKey.DependencyProperty;

#if NETCOREAPP
        private void Items_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
#else
        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
#endif
        {
            //throw new NotImplementedException();
            if (e.NewItems == null) return;
            if (e.NewItems.Count > 0)
            {
                foreach (var item in e.NewItems)
                {
                    if (item is GalleryItem gi)
                    {
                        gi.ColorScheme = ColorScheme;
                        gi.LayoutType = ItemLayout;
                    }
                }
            }
        }

        /// <summary>
        /// Get or set the layout to use for displaying <see cref="GalleryItem"/> objects within this Gallery.
        /// </summary>
        /// <remarks>
        /// Certain layouts, such as <see cref="GalleryItemLayout.LargeIconAndText"/> will take up the full height of the Gallery, and so each <see cref="GalleryItem"/> is displayed
        /// side by side. Others, such as <see cref="GalleryItemLayout.SmallIconAndText"/> may allow multiple GalleryItems to be displayed on top of each other. Thus, the smaller layouts
        /// are more beneficial if you have a large number of options in this Gallery that you want to display.
        /// <para/>
        /// Please be mindful about the type of content you want to display in this Gallery, and also the height of the Ribbon that this Gallery will be in, to set which layout option
        /// you want to use. Some layout options, such as <see cref="GalleryItemLayout.SmallContentOnly"/> or <see cref="GalleryItemLayout.LargeContentOnly"/> allow you to set some
        /// custom content to be displayed in each <see cref="GalleryItem"/>. However, other layouts won't display this custom content, or they may or may not display an icon or some text.
        /// </remarks>
        public GalleryItemLayout ItemLayout { get => (GalleryItemLayout)GetValue(ItemLayoutProperty); set => SetValue(ItemLayoutProperty, value); }

        /// <summary>The backing dependency property for <see cref="ItemLayout"/>. See the related property for details.</summary>
        public static DependencyProperty ItemLayoutProperty
            = DependencyProperty.Register("ItemLayout", typeof(GalleryItemLayout), typeof(Gallery),
            new FrameworkPropertyMetadata(GalleryItemLayout.LargeIconAndText, (d, e) => d.PerformAs<Gallery>(g => g.UpdateSubitemLayouts())));

        void UpdateSubitemLayouts()
        {
            foreach (var item in Items)
            {
                item.LayoutType = ItemLayout;
            }
        }

        void UpdateSubitemColors()
        {
            foreach (var item in Items)
            {
                item.ColorScheme = ColorScheme;
            }
        }

        bool FilterItems(object item)
        {
            if (item is GalleryItem i)
            {
                return true;
            }
            else return false;
        }

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

        /// <summary>
        /// Get or set if scrolling buttons should be displayed on the options on the right side. If <c>true</c>, then users can scroll through all the items in this Gallery
        /// without needing to use the expand button to display the whole menu.
        /// </summary>
        public bool DisplayScrollButtons { get => (bool)GetValue(DisplayScrollButtonsProperty); set => SetValue(DisplayScrollButtonsProperty, value); }

        /// <summary>The backing dependency property for <see cref="DisplayScrollButtons"/>. See the related property for details.</summary>
        public static DependencyProperty DisplayScrollButtonsProperty
            = DependencyProperty.Register("DisplayScrollButtons", typeof(bool), typeof(Gallery),
            new FrameworkPropertyMetadata(true));

    }
}
