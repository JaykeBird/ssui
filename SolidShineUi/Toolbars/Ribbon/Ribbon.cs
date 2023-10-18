using SolidShineUi.Toolbars.Ribbon.Utils;
using SolidShineUi.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace SolidShineUi.Toolbars.Ribbon
{
    /// <summary>
    /// A toolbar that displays various commands under a series of tabs, similar to what is present in Microsoft Office or Autodesk software.
    /// </summary>
    [ContentProperty("Items")]
    public class Ribbon : Control
    {
        static Ribbon()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Ribbon), new FrameworkPropertyMetadata(typeof(Ribbon)));
        }

        /// <summary>
        /// Create a Ribbon.
        /// </summary>
        public Ribbon()
        {
            SetValue(ItemsPropertyKey, new ObservableCollection<RibbonTab>());

            //InternalShowTabsOnBottomChanged += tabControl_InternalShowTabsOnBottomChanged;

            Loaded += Ribbon_Loaded;

            Items.CollectionChanged += Items_CollectionChanged;
        }

        private void Ribbon_Loaded(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();

            if (!loadedFirstTab)
            {
                if (SelectTabInternal(Items[0])) { loadedFirstTab = true; }
            }
        }

        bool loadedFirstTab = false;

        #region Template IO
        /// <inheritdoc/>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            LoadTemplateItems();

            if (Items.Count > 0)
            {
                if (!loadedFirstTab)
                {
                    if (SelectTabInternal(Items[0])) { loadedFirstTab = true; }
                }
            }
        }

        bool itemsLoaded = false;

        bool _internalAction = false;

#if NETCOREAPP
        ItemsControl? ic = null;
        ScrollViewer? sv = null;
        Border? ch = null;
#else
        ItemsControl ic = null;
        ScrollViewer sv = null;
        Border ch = null;
#endif

        void LoadTemplateItems()
        {
            if (!itemsLoaded)
            {
                ic = (ItemsControl)GetTemplateChild("PART_TabBar");
                sv = (ScrollViewer)GetTemplateChild("PART_TabScroll");
                ch = (Border)GetTemplateChild("PART_Content");

                if (ic != null && sv != null && ch != null)
                {
                    sv.ScrollChanged += sv_ScrollChanged;
                    ic.SizeChanged += control_SizeChanged;
                    itemsLoaded = true;
                }
            }
        }
        #endregion

        #region Tabs

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        private static readonly DependencyPropertyKey ItemsPropertyKey
            = DependencyProperty.RegisterReadOnly("Items", typeof(ObservableCollection<RibbonTab>), typeof(Ribbon),
            new FrameworkPropertyMetadata(new ObservableCollection<RibbonTab>()));

        public static readonly DependencyProperty ItemsProperty = ItemsPropertyKey.DependencyProperty;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Get or set the list of tabs in this Ribbon. This Items property can be used to add and remove items.
        /// </summary>
        [Category("Common")]
        public ObservableCollection<RibbonTab> Items
        {
            get { return (ObservableCollection<RibbonTab>)GetValue(ItemsProperty); }
            private set { SetValue(ItemsPropertyKey, value); }
        }

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // TODO: does e.Action == Reset list all removed items in e.OldItems? if not, how can we get them???
            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                {
                    if (item is RibbonTab tab)
                    {
                        tab.InternalBringIntoViewRequested += tab_InternalBringIntoViewRequested;
                        tab.VisibilityChanged += tab_VisibilityChanged;

                        foreach (RibbonGroup group in tab.Items)
                        {
                            group.ColorScheme = ColorScheme;

                            foreach (IRibbonItem control in group.Items)
                            {
                                control.ColorScheme = ColorScheme;
                            }
                        }
                    }
                }
            }

            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                    if (item is RibbonTab tab)
                    {
                        tab.VisibilityChanged -= tab_VisibilityChanged;
                        tab.InternalBringIntoViewRequested -= tab_InternalBringIntoViewRequested;
                    }
                }
            }
        }

        private void tab_InternalBringIntoViewRequested(object sender, EventArgs e)
        {
            if (sender is RibbonTab tab)
            {
                if (Items.Contains(tab) && tab.Visibility == Visibility.Visible)
                {
                    SelectedTab = tab;

                    for (int i = 0; i < ic.Items.Count; i++)
                    {
                        // I really dislike this roundabout way that I have to get the child items of an ItemsControl, but I guess this is how it is
                        // from https://stackoverflow.com/a/1876534/2987285
                        ContentPresenter c = (ContentPresenter)ic.ItemContainerGenerator.ContainerFromItem(ic.Items[i]);
                        if (c == null) return;
                        c.ApplyTemplate();

                        if (c.ContentTemplate.FindName("PART_TabItem", c) is RibbonTabDisplayItem tb)
                        {
                            if (tb.TabItem != null && tb.TabItem == tab)
                            {
                                tb.BringIntoView();
                            }
                        }
                    }
                }
            }
        }

        private void tab_VisibilityChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is RibbonTab tab)
            {
                if (SelectedTab == tab && tab.Visibility == Visibility.Collapsed)
                {
                    Dispatcher.Invoke(() => { SelectFirstVisibleTab(); });
                    // move to first visible tab
                }
            }
        }

        #region Selection

        bool _internalIndexSet = false;

        private bool SelectTabInternal(RibbonTab tab)
        {
            if (ic == null) return false;

            if (Items.Contains(tab) && tab.Visibility == Visibility.Visible)
            {
                SelectedTab = tab;

                for (int i = 0; i < ic.Items.Count; i++)
                {
                    // I really dislike this roundabout way that I have to get the child items of an ItemsControl, but I guess this is how it is
                    // from https://stackoverflow.com/a/1876534/2987285
                    ContentPresenter c = (ContentPresenter)ic.ItemContainerGenerator.ContainerFromItem(ic.Items[i]);
                    if (c == null) return false;
                    c.ApplyTemplate();
                    //#if NETCOREAPP
                    //#else
                    //                    TabDisplayItem tb = c.ContentTemplate.FindName("PART_TabItem", c) as TabDisplayItem;
                    //#endif
                    if (c.ContentTemplate.FindName("PART_TabItem", c) is RibbonTabDisplayItem tb)
                    {
                        if (tb.TabItem != null && tb.TabItem == tab)
                        {
                            tb.IsSelected = true;
                        }
                        else
                        {
                            tb.IsSelected = false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }

            if (!_internalIndexSet)
            {
                SelectedIndex = Items.IndexOf(tab);
            }

            return true;
        }

        /// <summary>
        /// Select a specific tab on the Ribbon. This tab needs to be visible (i.e. <see cref="RibbonTab.Visibility"/> is set to <c>Visible</c>) before it can be selected.
        /// </summary>
        /// <param name="tab"></param>
        /// <returns></returns>
        public bool SelectTab(RibbonTab tab)
        {
            return SelectTabInternal(tab);
        }

        void SelectFirstVisibleTab()
        {
            if (Items.Count == 0) return;
            for (int i = 0; Items.Count > 0; i++)
            {
                RibbonTab tab = Items[i];
                if (tab.Visibility == Visibility.Visible)
                {
                    SelectTab(tab);
                    return;
                }
            }
        }

        private void DeselectAllTabs()
        {
            SelectedTab = null;
            if (ic == null) return;
            for (int i = 0; i < ic.Items.Count; i++)
            {
                // I really dislike this roundabout way that I have to get the child items of an ItemsControl, but I guess this is how it is
                // from https://stackoverflow.com/a/1876534/2987285
                ContentPresenter c = (ContentPresenter)ic.ItemContainerGenerator.ContainerFromItem(ic.Items[i]);
                if (c == null) continue;
                c.ApplyTemplate();
                if (c.ContentTemplate.FindName("PART_TabItem", c) is RibbonTabDisplayItem tb)
                {
                    if (tb.TabItem != null)
                    {
                        tb.IsSelected = false;
                    }
                }
                else
                {
                    continue;
                }
            }
        }

        public int SelectedIndex { get => (int)GetValue(SelectedIndexProperty); set => SetValue(SelectedIndexProperty, value); }

        public static DependencyProperty SelectedIndexProperty
            = DependencyProperty.Register("SelectedIndex", typeof(int), typeof(Ribbon),
            new FrameworkPropertyMetadata(-1, new PropertyChangedCallback((d, e) =>
            {
                if (d is Ribbon r)
                {
                    r.OnSelectedIndexChange();
                }
            }
            )));

        protected void OnSelectedIndexChange()
        {
            _internalIndexSet = true;

            if (SelectedIndex == -1) // nothing should be selected
            {
                // deselect all tabs
                DeselectAllTabs();
            }
            else if (SelectedIndex >= Items.Count) // make sure the new value is within the valid number of indexes
            {
                SelectedIndex = -1;
                // should deselect all tabs automatically by running this function again
            }
            else
            {
                // select the specific tab that has this index
                try
                {
                    SelectTabInternal(Items[SelectedIndex]);
                }
                catch (IndexOutOfRangeException)
                {
                    // entered an index that isn't valid
                    SelectedIndex = -1;
                    // should deselect all tabs automatically by running this function again
                }
            }

            _internalIndexSet = false;
        }

        /// <summary>
        /// A delegate to be used with events regarding the selected TabItem changing in a TabControl.
        /// </summary>
        /// <param name="sender">The object where the event was raised.</param>
        /// <param name="e">The event arguments associated with this event.</param>
        public delegate void TabItemChangeEventHandler(object sender, TabItemChangeEventArgs e);

        /// <summary>
        /// A delegate to be used with events regarding a TabItem being closed in a TabControl.
        /// </summary>
        /// <param name="sender">The object where the event was raised.</param>
        /// <param name="e">The event arguments associated with this event.</param>
        public delegate void TabItemClosingEventHandler(object sender, TabItemClosingEventArgs e);

        private static readonly DependencyPropertyKey SelectedTabPropertyKey
            = DependencyProperty.RegisterReadOnly("SelectedTab", typeof(RibbonTab), typeof(Ribbon), new FrameworkPropertyMetadata(null));

#if NETCOREAPP
        /// <summary>
        /// Get the tab currently selected. Use <c>Items.Select()</c> to select another tab.
        /// </summary>
        [Category("Common")]
        public RibbonTab? SelectedTab { get => (RibbonTab)GetValue(SelectedTabProperty); private set => SetValue(SelectedTabPropertyKey, value); }

        /// <summary>
        /// A dependency property object backing a related property. See the related property for more details.
        /// </summary>
        public static readonly DependencyProperty SelectedTabProperty = SelectedTabPropertyKey!.DependencyProperty;

        /// <summary>
        /// Raised when the tab currently selected is changed.
        /// </summary>
        public event TabItemChangeEventHandler? SelectedTabChanged;
        /// <summary>
        /// Raised when all tabs are closed at once (via <c>Items.Clear()</c>).
        /// </summary>
        public event EventHandler? TabsCleared;
#else
        /// <summary>
        /// Get the tab currently selected. Use <c>Items.Select()</c> to select another tab.
        /// </summary>
        [Category("Common")]
        public RibbonTab SelectedTab { get => (RibbonTab)GetValue(SelectedTabProperty); private set => SetValue(SelectedTabPropertyKey, value); }

        /// <summary>
        /// A dependency property object backing a related property. See the related property for more details.
        /// </summary>
        public static readonly DependencyProperty SelectedTabProperty = SelectedTabPropertyKey.DependencyProperty;

        /// <summary>
        /// Raised when the tab currently selected is changed.
        /// </summary>
        public event TabItemChangeEventHandler SelectedTabChanged;
        /// <summary>
        /// Raised when all tabs are closed at once (via <c>Items.Clear()</c>).
        /// </summary>
        public event EventHandler TabsCleared;
#endif

        #endregion

        #endregion

        #region Main Bar

        public double MainBarHeight { get => (double)GetValue(MainBarHeightProperty); set => SetValue(MainBarHeightProperty, value); }

        public static DependencyProperty MainBarHeightProperty
            = DependencyProperty.Register("MainBarHeight", typeof(double), typeof(Ribbon),
            new FrameworkPropertyMetadata(96.0));

        #endregion

        #region Tab Bar

//        #region ShowTabsOnBottom

//#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
//        public static readonly DependencyProperty ShowTabsOnBottomProperty = DependencyProperty.Register("ShowTabsOnBottom", typeof(bool), typeof(Ribbon),
//            new PropertyMetadata(false, new PropertyChangedCallback(OnInternalShowTabsOnBottomChanged)));
//#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

//        /// <summary>
//        /// Get or set if the tab bar should be shown at the bottom of the control, rather than the top.
//        /// </summary>
//        [Category("Common")]
//        public bool ShowTabsOnBottom
//        {
//            get { return (bool)GetValue(ShowTabsOnBottomProperty); }
//            set { SetValue(ShowTabsOnBottomProperty, value); }
//        }

//        /// <summary>
//        /// Internal event for handling a property changed. Please view the event that is not prefixed as "Internal".
//        /// </summary>
//        protected event DependencyPropertyChangedEventHandler InternalShowTabsOnBottomChanged;

//        /// <summary>
//        /// Raised when the ShowTabsOnBottom property is changed.
//        /// </summary>
//#if NETCOREAPP
//        public event DependencyPropertyChangedEventHandler? ShowTabsOnBottomChanged;
//#else
//        public event DependencyPropertyChangedEventHandler ShowTabsOnBottomChanged;
//#endif

//        private static void OnInternalShowTabsOnBottomChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
//        {
//            if (d is Ribbon s)
//            {
//                s.InternalShowTabsOnBottomChanged?.Invoke(s, e);
//            }
//        }
//        private void tabControl_InternalShowTabsOnBottomChanged(object sender, DependencyPropertyChangedEventArgs e)
//        {
//            ShowTabsOnBottomChanged?.Invoke(this, e);
//        }
//        #endregion

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly DependencyProperty FileMenuProperty = DependencyProperty.Register(
            "FileMenu", typeof(UIElement), typeof(Ribbon),
            new PropertyMetadata(null));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Gets or sets the UI element to place in the top-left of the control, where the File menu usually is. Recommended to use a <see cref="RibbonFileMenu"/>.
        /// </summary>
        public UIElement FileMenu
        {
            get
            {
                return (UIElement)GetValue(FileMenuProperty);
            }
            set
            {
                SetValue(FileMenuProperty, value);
            }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly DependencyProperty TopRightElementProperty = DependencyProperty.Register(
            "TopRightElement", typeof(UIElement), typeof(Ribbon),
            new PropertyMetadata(null));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Gets or sets the UI element to place in the top-right of the control.
        /// </summary>
        public UIElement TopRightElement
        {
            get
            {
                return (UIElement)GetValue(TopRightElementProperty);
            }
            set
            {
                SetValue(TopRightElementProperty, value);
            }
        }

        public bool ShowOnlyTabs { get => (bool)GetValue(ShowOnlyTabsProperty); set => SetValue(ShowOnlyTabsProperty, value); }

        public static DependencyProperty ShowOnlyTabsProperty
            = DependencyProperty.Register("ShowOnlyTabs", typeof(bool), typeof(Ribbon),
            new FrameworkPropertyMetadata(false));

        #endregion

        public bool AllowControlReordering { get => (bool)GetValue(AllowControlReorderingProperty); set => SetValue(AllowControlReorderingProperty, value); }

        /// <summary>The backing dependency property for <see cref="AllowControlReordering"/>. See the related property for details.</summary>
        public static DependencyProperty AllowControlReorderingProperty
            = DependencyProperty.Register("AllowControlReordering", typeof(bool), typeof(Ribbon),
            new FrameworkPropertyMetadata(false));

        #region Brushes

        public Brush MainBarBackground { get => (Brush)GetValue(MainBarBackgroundProperty); set => SetValue(MainBarBackgroundProperty, value); }

        /// <summary>The backing dependency property for <see cref="MainBarBackground"/>. See the related property for details.</summary>
        public static DependencyProperty MainBarBackgroundProperty
            = DependencyProperty.Register("MainBarBackground", typeof(Brush), typeof(Ribbon),
            new FrameworkPropertyMetadata(Colors.White.ToBrush()));


        public Brush TabBarBackground { get => (Brush)GetValue(TabBarBackgroundProperty); set => SetValue(TabBarBackgroundProperty, value); }

        /// <summary>The backing dependency property for <see cref="TabBarBackground"/>. See the related property for details.</summary>
        public static DependencyProperty TabBarBackgroundProperty
            = DependencyProperty.Register("TabBarBackground", typeof(Brush), typeof(Ribbon),
            new FrameworkPropertyMetadata(Colors.White.ToBrush()));

        public new Brush BorderBrush { get => (Brush)GetValue(BorderBrushProperty); set => SetValue(BorderBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="BorderBrush"/>. See the related property for details.</summary>
        public new static DependencyProperty BorderBrushProperty
            = DependencyProperty.Register("BorderBrush", typeof(Brush), typeof(Ribbon),
            new FrameworkPropertyMetadata(Colors.DarkGray.ToBrush()));


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
            = DependencyProperty.Register("ColorScheme", typeof(ColorScheme), typeof(Ribbon),
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

            if (d is Ribbon c)
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

            MainBarBackground = cs.LightBackgroundColor.ToBrush();
            BorderBrush = cs.BorderColor.ToBrush();
            TabBarBackground = cs.BackgroundColor.ToBrush();

            foreach (RibbonTab tab in Items)
            {
                foreach (RibbonGroup group in tab.Items)
                {
                    group.ColorScheme = ColorScheme;

                    foreach (IRibbonItem control in group.Items)
                    {
                        control.ColorScheme = ColorScheme;
                    }
                }
            }
        }
        #endregion

        #region Setup TabDisplayItem / Tdi Event Handlers

        /// <summary>
        /// Set up a new TabDisplayItem that was added to this TabControl.
        /// This will set up the necessary event handlers and other properties to allow the TabDisplayItem to interact with the TabControl.
        /// </summary>
        /// <param name="tdi">The TabDisplayItem to set up.</param>
        internal protected void SetupTabDisplay(RibbonTabDisplayItem tdi)
        {
            tdi.Click += tdi_Click;
            tdi.RightClick += tdi_RightClick;
            tdi.RequestSelect += tdi_RequestSelect;
            tdi.TabItemDrop += tdi_TabItemDrop;

            //CheckScrolling();
        }

        private void tdi_RequestSelect(object sender, EventArgs e)
        {
            if (sender is RibbonTabDisplayItem tdi)
            {
                SelectTab(tdi.TabItem);
            }
        }

#if NETCOREAPP
        private void tdi_RightClick(object? sender, EventArgs e)
#else
        private void tdi_RightClick(object sender, EventArgs e)
#endif
        {
            if (sender is RibbonTabDisplayItem tdi)
            {
                //if (tdi.TabItem.TabContextMenu != null)
                //{
                //    ContextMenu cm = tdi.TabItem.TabContextMenu;
                //    cm.ColorScheme = ColorScheme;
                //    cm.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
                //    cm.IsOpen = true;
                //}
            }
        }

        private void tdi_TabItemDrop(object sender, RibbonTabItemDropEventArgs e)
        {
            if (e.DroppedTabItem == e.SourceTabItem) return;
            if (!Items.Contains(e.DroppedTabItem)) return;

            _internalAction = true;
#if NETCOREAPP
            RibbonTab? selItem = SelectedTab;
#else
            RibbonTab selItem = SelectedTab;
#endif
            if (e.DroppedTabItem == selItem)
            {
                DeselectAllTabs();
            }

            Items.Remove(e.DroppedTabItem);
            //if (Items.Contains(e.DroppedTabItem))
            //{
            //    Items.Remove(e.DroppedTabItem);
            //}
            //else
            //{
            //    // get source tab control and remove it from there before adding here
            //}

            int newIndex = e.PlaceBefore ? Items.IndexOf(e.SourceTabItem) : Items.IndexOf(e.SourceTabItem) + 1;

            if (newIndex == -1)
            {
                Items.Add(e.DroppedTabItem);
            }
            else
            {
                Items.Insert(newIndex, e.DroppedTabItem);
            }

            if (selItem != null)
            {
                SelectTab(selItem);
            }

            // fix to make sure the correct tab has the IsSelected state
            //if (ic != null)
            //{
            //    for (int i = 0; i < ic.Items.Count; i++)
            //    {
            //        // I really dislike this roundabout way that I have to get the child items of an ItemsControl, but I guess this is how it is
            //        // from https://stackoverflow.com/a/1876534/2987285
            //        ContentPresenter c = (ContentPresenter)ic.ItemContainerGenerator.ContainerFromItem(ic.Items[i]);
            //        c.ApplyTemplate();
            //        //#if NETCOREAPP
            //        //#else
            //        //                    TabDisplayItem tb = c.ContentTemplate.FindName("PART_TabItem", c) as TabDisplayItem;
            //        //#endif
            //        if (c.ContentTemplate.FindName("PART_TabItem", c) is TabDisplayItem tb)
            //        {
            //            if (tb.TabItem != null && tb.TabItem == selItem)
            //            {
            //                tb.IsSelected = true;
            //            }
            //            else
            //            {
            //                tb.IsSelected = false;
            //            }
            //        }
            //    }
            //}

            _internalAction = false;
        }

#if NETCOREAPP
        private void tdi_Click(object? sender, EventArgs e)
#else
        private void tdi_Click(object sender, EventArgs e)
#endif
        {
            //if (sender != null && sender is RibbonTabDisplayItem tdi)
            //{
            //    if (tdi.TabItem != null && tdi.CanSelect)
            //    {
            //        Items.Select(tdi.TabItem);
            //    }
            //}
        }


        #endregion

        #region Scrolling

        #region ScrollButtons

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

        private static readonly DependencyPropertyKey TabScrollButtonsVisiblePropertyKey = DependencyProperty.RegisterReadOnly("TabScrollButtonsVisible", typeof(bool), typeof(Ribbon),
            new PropertyMetadata(false));

        public static readonly DependencyProperty TabScrollButtonsVisibleProperty = TabScrollButtonsVisiblePropertyKey.DependencyProperty;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Get if the scroll buttons are currently visible in the tab bar.
        /// </summary>
        public bool TabScrollButtonsVisible
        {
            get { return (bool)GetValue(TabScrollButtonsVisibleProperty); }
            private set { SetValue(TabScrollButtonsVisiblePropertyKey, value); }
        }
        #endregion

        /// <summary>
        /// A WPF command that when executed, will scroll the tab control's tab bar to the left or right.
        /// </summary>
        public static readonly RoutedCommand TabBarScrollCommand = new RoutedCommand("TabBarScrollCommand", typeof(Ribbon));

        void CheckScrolling()
        {
            if (sv == null || ic == null) return;

            if (sv.ViewportWidth == 0)
            {
                return;
            }

            if (ic.ActualWidth > sv.ViewportWidth)
            {
                TabScrollButtonsVisible = true;
            }
            else
            {
                TabScrollButtonsVisible = false;
            }
        }

        private void OnScrollCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (sv == null) return;

            double offset = sv.HorizontalOffset;

            if (e.Parameter is TabScrollCommandAction a)
            {
                switch (a)
                {
                    case TabScrollCommandAction.Left:
                        sv.ScrollToHorizontalOffset(Math.Max(offset - 20, 0));
                        break;
                    case TabScrollCommandAction.Right:
                        sv.ScrollToHorizontalOffset(Math.Min(offset + 20, sv.ScrollableWidth));
                        break;
                    case TabScrollCommandAction.Home:
                        sv.ScrollToHorizontalOffset(0);
                        break;
                    case TabScrollCommandAction.End:
                        sv.ScrollToHorizontalOffset(sv.ScrollableWidth);
                        break;
                    default:
                        break;
                }
            }
        }

        private void control_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // note that this both handles the TabControl changes and the internal ItemControl changes
            if (e.WidthChanged)
            {
                CheckScrolling();
            }
        }

        private void sv_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.ViewportWidthChange > 0)
            {
                CheckScrolling();
            }
        }

        #endregion
    }
}
