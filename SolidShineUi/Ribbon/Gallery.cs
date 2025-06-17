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
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
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
            SetValue(ItemsPropertyKey, new ObservableCollection<GalleryItem>());
            SetValue(DisplayedItemsPropertyKey, new List<GalleryItem>());
            Items.CollectionChanged += Items_CollectionChanged;

            SetDisplayedItems(0);
            //DisplayedItems.Filter = FilterItems;

            CommandBindings.Add(new CommandBinding(RibbonCommands.GalleryScrollUp, OnGalleryScrollUp, CanExecuteIfCanScrollUp));
            CommandBindings.Add(new CommandBinding(RibbonCommands.GalleryScrollDown, OnGalleryScrollDown, CanExecuteIfCanScrollDown));
            CommandBindings.Add(new CommandBinding(RibbonCommands.GalleryMenuExpand, OnGalleryExpand, CanExecuteIfHasItems));
        }

        #region IRibbonItem implementations

        /// <inheritdoc/>
        public bool IsCompacted { get => (bool)GetValue(IsCompactedProperty); set => SetValue(IsCompactedProperty, value); }

        /// <summary>The backing dependency property for <see cref="IsCompacted"/>. See the related property for details.</summary>
        public static DependencyProperty IsCompactedProperty
            = DependencyProperty.Register("IsCompacted", typeof(bool), typeof(Gallery),
            new FrameworkPropertyMetadata(false));

        /// <inheritdoc/>
        public RibbonElementSize StandardSize { get => (RibbonElementSize)GetValue(StandardSizeProperty); set => SetValue(StandardSizeProperty, value); }

        /// <summary>The backing dependency property for <see cref="StandardSize"/>. See the related property for details.</summary>
        public static DependencyProperty StandardSizeProperty
            = DependencyProperty.Register("StandardSize", typeof(RibbonElementSize), typeof(Gallery),
            new FrameworkPropertyMetadata(RibbonElementSize.Content));

        /// <inheritdoc/>
        public RibbonElementSize CompactSize { get => (RibbonElementSize)GetValue(CompactSizeProperty); set => SetValue(CompactSizeProperty, value); }

        /// <summary>The backing dependency property for <see cref="CompactSize"/>. See the related property for details.</summary>
        public static DependencyProperty CompactSizeProperty
            = DependencyProperty.Register("CompactSize", typeof(RibbonElementSize), typeof(Gallery),
            new FrameworkPropertyMetadata(RibbonElementSize.Small));

        /// <inheritdoc/>
        public string AccessKey { get => (string)GetValue(AccessKeyProperty); set => SetValue(AccessKeyProperty, value); }

        /// <summary>The backing dependency property for <see cref="AccessKey"/>. See the related property for details.</summary>
        public static DependencyProperty AccessKeyProperty
            = DependencyProperty.Register("AccessKey", typeof(string), typeof(Gallery),
            new FrameworkPropertyMetadata("G"));

        /// <inheritdoc/>
        public string Title { get => (string)GetValue(TitleProperty); set => SetValue(TitleProperty, value); }

        /// <summary>The backing dependency property for <see cref="Title"/>. See the related property for details.</summary>
        public static DependencyProperty TitleProperty
            = DependencyProperty.Register("Title", typeof(string), typeof(Gallery),
            new FrameworkPropertyMetadata("Gallery"));

        /// <inheritdoc/>
        public ImageSource LargeIcon { get => (ImageSource)GetValue(LargeIconProperty); set => SetValue(LargeIconProperty, value); }

        /// <summary>The backing dependency property for <see cref="LargeIcon"/>. See the related property for details.</summary>
        public static DependencyProperty LargeIconProperty
            = DependencyProperty.Register("LargeIcon", typeof(ImageSource), typeof(Gallery),
            new FrameworkPropertyMetadata(null));

        /// <inheritdoc/>
        public ImageSource SmallIcon { get => (ImageSource)GetValue(SmallIconProperty); set => SetValue(SmallIconProperty, value); }

        /// <summary>The backing dependency property for <see cref="SmallIcon"/>. See the related property for details.</summary>
        public static DependencyProperty SmallIconProperty
            = DependencyProperty.Register("SmallIcon", typeof(ImageSource), typeof(Gallery),
            new FrameworkPropertyMetadata(null));

        /// <inheritdoc/>
        public int CompactOrder { get => (int)GetValue(CompactOrderProperty); set => SetValue(CompactOrderProperty, value); }

        /// <summary>The backing dependency property for <see cref="CompactOrder"/>. See the related property for details.</summary>
        public static DependencyProperty CompactOrderProperty
            = DependencyProperty.Register("CompactOrder", typeof(int), typeof(Gallery),
            new FrameworkPropertyMetadata(0));

        #endregion

        #region Command Handling

        void OnGalleryScrollUp(object sender, ExecutedRoutedEventArgs e)
        {
            if (currentInitialValue > MaxItemsDisplayed) currentInitialValue -= MaxItemsDisplayed;
            else currentInitialValue = 0;

            SetDisplayedItems(currentInitialValue);
        }

        void OnGalleryScrollDown(object sender, ExecutedRoutedEventArgs e)
        {
            if (Items.Count - currentInitialValue > MaxItemsDisplayed) currentInitialValue += MaxItemsDisplayed;
            else currentInitialValue = Items.Count - MaxItemsDisplayed; // <--- this part might not be necessary

            SetDisplayedItems(currentInitialValue);
        }

        void OnGalleryExpand(object sender, ExecutedRoutedEventArgs e)
        {
            if (popMenu != null)
            {
                popMenu.IsOpen = true;
            }
        }

        /// <summary>
        /// The command in question is always able to be executed, regardless of the state of the object.
        /// </summary>
        private void CanExecuteAlways(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        /// <summary>
        /// The command in question is able to be executed if there are at least 1 item in <see cref="Items"/>.
        /// </summary>
        private void CanExecuteIfHasItems(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Items.Any();
        }

        /// <summary>
        /// The command in question is able to be executed if the <see cref="currentInitialValue"/> is greater than 0. 
        /// </summary>
        private void CanExecuteIfCanScrollUp(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = currentInitialValue > 0;
        }

        /// <summary>
        /// The command in question is able to be executed if the <see cref="currentInitialValue"/> can be increased by an interval of <see cref="MaxItemsDisplayed"/>.
        /// </summary>
        private void CanExecuteIfCanScrollDown(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Items.Count - currentInitialValue > MaxItemsDisplayed;
        }

        ///// <summary>
        ///// The command in question is able to execute, if at least one tab is selected.
        ///// </summary>
        //private void CanExecuteIfAnyTabSelected(object sender, CanExecuteRoutedEventArgs e)
        //{
        //    e.CanExecute = SelectedTab != null;
        //}

        #endregion

        #region Template IO

        /// <inheritdoc/>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            LoadTemplateItems();

            if (popMenu != null)
            {
                popMenu.PlacementTarget = this;

                if (popMenu.Child is Border b)
                {
                    b.SetBinding(BackgroundProperty, new Binding(nameof(Background)) { Source = this });
                    b.SetBinding(BorderBrushProperty, new Binding(nameof(BorderBrush)) { Source = this });
                    b.SetBinding(BorderThicknessProperty, new Binding(nameof(BorderThickness)) { Source = this });
                }
            }
        }

        bool itemsLoaded = false;

#if NETCOREAPP
        Popup? popMenu = null;
#else
        Popup popMenu = null;
#endif

        void LoadTemplateItems()
        {
            if (!itemsLoaded)
            {
                popMenu = (Popup)GetTemplateChild("PART_Popup");

                if (popMenu != null)
                {
                    itemsLoaded = true;
                }
            }
        }

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

        void UpdateSubitemColors()
        {
            foreach (var item in Items)
            {
                item.ColorScheme = ColorScheme;
            }
        }

        #endregion

        #region Items

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
                        gi.SetValue(GalleryItem.LayoutTypeProperty, ItemLayout);
                    }
                }
            }

            // reset the DisplayedItems property

            // first, let's check if we now have an invalid value
            if (currentInitialValue > Items.Count)
            {
                // reset it back to 0
                currentInitialValue = 0;
            }
            SetDisplayedItems(currentInitialValue);
        }

        #endregion

        #region DisplayedItems

        int currentInitialValue = 0;

        private static readonly DependencyPropertyKey DisplayedItemsPropertyKey
            = DependencyProperty.RegisterReadOnly("DisplayedItems", typeof(List<GalleryItem>), typeof(Gallery),
                new FrameworkPropertyMetadata(new List<GalleryItem>()));

        /// <summary>
        /// Get the list of the items from <see cref="Items"/> that are currently visible in the Ribbon.
        /// </summary>
        public List<GalleryItem> DisplayedItems { get => (List<GalleryItem>)GetValue(DisplayedItemsProperty); private set => SetValue(DisplayedItemsPropertyKey, value); }

        /// <summary>The backing dependency property for <see cref="DisplayedItems"/>. See the related property for details.</summary>
        public static DependencyProperty DisplayedItemsProperty = DisplayedItemsPropertyKey.DependencyProperty;

        /// <summary>
        /// Sets the <see cref="DisplayedItems"/> property, to display a subset of <see cref="Items"/>, 
        /// starting at an <paramref name="initialValue"/> and counting the next <see cref="MaxItemsDisplayed"/> number of items.
        /// </summary>
        /// <param name="initialValue">the index of the first value to display and count from</param>
        /// <remarks>
        /// If the number of items remaining between <paramref name="initialValue"/> and the max count of <see cref="Items"/> is
        /// less than <see cref="MaxItemsDisplayed"/>, then this will limit it down to display the last remaining items.
        /// </remarks>
        void SetDisplayedItems(int initialValue)
        {
            if (initialValue >= Items.Count || initialValue < 0) return; // validate initial value, don't change anything if invalid

            // we'll try to display the max number of items starting at that initial value,
            // but if the max number would go beyond the actual number of items to display, then we need
            // to clamp it to the actual number of items remaining
            int count = MaxItemsDisplayed;
            if (initialValue + count >= Items.Count)
            {
                count = Items.Count - initialValue; // clamp the count to the actual remaining number of items available
            }

            // okay, now that we've calculated that, let's set the DisplayedItems property
            DisplayedItems.Clear();
            DisplayedItems = Items.ToList().GetRange(initialValue, count);
        }

        #endregion

        #region ItemLayout

        /// <summary>
        /// Get or set the layout to use for displaying <see cref="GalleryItem"/> objects within this Gallery.
        /// </summary>
        /// <remarks>
        /// Certain layouts, such as <see cref="GalleryItemLayout.LargeIconAndText"/> will take up the full height of the Gallery, and so each <see cref="GalleryItem"/> is displayed
        /// side by side. Others, such as <see cref="GalleryItemLayout.SmallIconAndText"/> may allow multiple GalleryItems to be displayed on top of each other. Thus, the smaller layouts
        /// are more beneficial if you have a large number of options in this Gallery that you want to display, and don't need a large space to preview/display each one.
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
                item.SetValue(GalleryItem.LayoutTypeProperty, ItemLayout);
            }
        }

        #endregion

        //bool FilterItems(object item)
        //{
        //    if (item is GalleryItem i)
        //    {
        //        return true;
        //    }
        //    else return false;
        //}

        /// <summary>
        /// Get or set the max number of items that can be displayed within the Ribbon.
        /// </summary>
        public int MaxItemsDisplayed { get => (int)GetValue(MaxItemsDisplayedProperty); set => SetValue(MaxItemsDisplayedProperty, value); }

        /// <summary>The backing dependency property for <see cref="MaxItemsDisplayed"/>. See the related property for details.</summary>
        public static DependencyProperty MaxItemsDisplayedProperty
            = DependencyProperty.Register("MaxItemsDisplayed", typeof(int), typeof(Gallery),
            new FrameworkPropertyMetadata(6, (d, e) => d.PerformAs<Gallery>((o) => o.OnMaxItemsDisplayedChanged(e))));

        private void OnMaxItemsDisplayedChanged(DependencyPropertyChangedEventArgs e)
        {
            if (MaxItemsDisplayed <= 0) // need to make sure this isn't a negative value
            {
                MaxItemsDisplayed = 1;
            }
            SetDisplayedItems(currentInitialValue);
        }

        /// <summary>
        /// Get or set the maximum width of the <see cref="Items"/> in this control. This sets the <c>MaxWidth</c> property for all child items.
        /// </summary>
        public double MaxItemWidth { get => (double)GetValue(MaxItemWidthProperty); set => SetValue(MaxItemWidthProperty, value); }

        /// <summary>The backing dependency property for <see cref="MaxItemWidth"/>. See the related property for details.</summary>
        public static DependencyProperty MaxItemWidthProperty
            = DependencyProperty.Register("MaxItemWidth", typeof(double), typeof(Gallery),
            new FrameworkPropertyMetadata(150.0));

        /// <summary>
        /// Get or set the maximum width of the full menu of all the <see cref="Items"/>.
        /// </summary>
        public double MaxExpandedMenuWidth { get => (double)GetValue(MaxExpandedMenuWidthProperty); set => SetValue(MaxExpandedMenuWidthProperty, value); }

        /// <summary>The backing dependency property for <see cref="MaxExpandedMenuWidth"/>. See the related property for details.</summary>
        public static DependencyProperty MaxExpandedMenuWidthProperty
            = DependencyProperty.Register(nameof(MaxExpandedMenuWidth), typeof(double), typeof(Gallery),
            new FrameworkPropertyMetadata(300.0));

        /// <summary>
        /// Get or set the maximum height of the full menu of all the <see cref="Items"/>.
        /// </summary>
        public double MaxExpandedMenuHeight { get => (double)GetValue(MaxExpandedMenuHeightProperty); set => SetValue(MaxExpandedMenuHeightProperty, value); }

        /// <summary>The backing dependency property for <see cref="MaxExpandedMenuHeight"/>. See the related property for details.</summary>
        public static DependencyProperty MaxExpandedMenuHeightProperty
            = DependencyProperty.Register(nameof(MaxExpandedMenuHeight), typeof(double), typeof(Gallery),
            new FrameworkPropertyMetadata(192.0));



        /// <summary>
        /// Get or set if scrolling buttons should be displayed on the options on the right side. If <c>true</c>, then users can scroll through all the items in this Gallery
        /// without needing to use the expand button to display the whole menu. If <c>false</c>, then only the expand button is visible next to the gallery.
        /// </summary>
        public bool DisplayScrollButtons { get => (bool)GetValue(DisplayScrollButtonsProperty); set => SetValue(DisplayScrollButtonsProperty, value); }

        /// <summary>The backing dependency property for <see cref="DisplayScrollButtons"/>. See the related property for details.</summary>
        public static DependencyProperty DisplayScrollButtonsProperty
            = DependencyProperty.Register("DisplayScrollButtons", typeof(bool), typeof(Gallery),
            new FrameworkPropertyMetadata(true));

    }
}
