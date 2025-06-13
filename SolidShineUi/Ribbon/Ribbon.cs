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
using SolidShineUi.Ribbon.Utils;
using SolidShineUi.Utils;

namespace SolidShineUi.Ribbon
{
    /// <summary>
    /// A toolbar that displays various commands under a series of tabs, similar to what is present in Microsoft Office or Autodesk software.
    /// </summary>
    [ContentProperty("Items")]
    public class Ribbon : Control
    {
        #region Constructors / Loaded

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

            CommandBindings.Add(new CommandBinding(TabControl.TabBarScrollCommand, OnScrollCommand, CanExecuteAlways));
            CommandBindings.Add(new CommandBinding(RibbonCommands.MainBarScrollCommand, OnMainScrollCommand, CanExecuteAlways));

            Loaded += Ribbon_Loaded;
            SizeChanged += Ribbon_SizeChanged;
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


        /// <summary>
        /// The command in question is always able to be executed, regardless of the state of the object.
        /// </summary>
        private void CanExecuteAlways(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        #endregion

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
        ItemsControl? tabContainer = null;
        ScrollViewer? tabScrollContainer = null;
        ItemsControl? mainContainer = null;
        ScrollViewer? mainScrollContainer = null;
        Border? mainBar = null;
#else
        ItemsControl tabContainer = null;
        ScrollViewer tabScrollContainer = null;
        ItemsControl mainContainer = null;
        ScrollViewer mainScrollContainer = null;
        Border mainBar = null;
#endif

        void LoadTemplateItems()
        {
            if (!itemsLoaded)
            {
                tabContainer = (ItemsControl)GetTemplateChild("PART_TabBar");
                tabScrollContainer = (ScrollViewer)GetTemplateChild("PART_TabScroll");
                mainContainer = (ItemsControl)GetTemplateChild("PART_MainContent");
                mainScrollContainer = (ScrollViewer)GetTemplateChild("PART_MainScroll");
                mainBar = (Border)GetTemplateChild("PART_MainBar");

                if (tabContainer != null && tabScrollContainer != null && mainContainer != null &&
                    mainScrollContainer != null && mainBar != null)
                {
                    itemsLoaded = true;
                }

                if (tabContainer != null && tabScrollContainer != null)
                {
                    tabScrollContainer.ScrollChanged += sv_ScrollChanged;
                    tabContainer.SizeChanged += tabBar_SizeChanged;
                }

                if (mainContainer != null && mainScrollContainer != null)
                {
                    mainScrollContainer.ScrollChanged += mv_ScrollChanged;
                    mainContainer.SizeChanged += mc_SizeChanged;
                }
            }
        }
        #endregion

        #region Tabs

        private static readonly DependencyPropertyKey ItemsPropertyKey
            = DependencyProperty.RegisterReadOnly("Items", typeof(ObservableCollection<RibbonTab>), typeof(Ribbon),
            new FrameworkPropertyMetadata(new ObservableCollection<RibbonTab>()));

        /// <summary>The backing dependency property for <see cref="Items"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ItemsProperty = ItemsPropertyKey.DependencyProperty;

        /// <summary>
        /// Get or set the list of tabs in this Ribbon. This Items property can be used to add and remove items.
        /// </summary>
        [Category("Common")]
        public ObservableCollection<RibbonTab> Items
        {
            get { return (ObservableCollection<RibbonTab>)GetValue(ItemsProperty); }
            private set { SetValue(ItemsPropertyKey, value); }
        }

#if NETCOREAPP
        private void Items_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
#else
        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
#endif
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

            CheckScrolling();
        }

#if NETCOREAPP
        private void tab_InternalBringIntoViewRequested(object? sender, EventArgs e)
#else
        private void tab_InternalBringIntoViewRequested(object sender, EventArgs e)
#endif
        {
            if (sender is RibbonTab tab)
            {
                if (Items.Contains(tab) && tab.Visibility == Visibility.Visible)
                {
                    SelectedTab = tab;

                    if (tabContainer == null) return;
                    for (int i = 0; i < tabContainer.Items.Count; i++)
                    {
                        // I really dislike this roundabout way that I have to get the child items of an ItemsControl, but I guess this is how it is
                        // from https://stackoverflow.com/a/1876534/2987285
                        ContentPresenter c = (ContentPresenter)tabContainer.ItemContainerGenerator.ContainerFromItem(tabContainer.Items[i]);
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
            if (tabContainer == null) return false;

            if (Items.Contains(tab) && tab.Visibility == Visibility.Visible)
            {
                SelectedTab = tab;

                for (int i = 0; i < tabContainer.Items.Count; i++)
                {
                    // I really dislike this roundabout way that I have to get the child items of an ItemsControl, but I guess this is how it is
                    // from https://stackoverflow.com/a/1876534/2987285
                    ContentPresenter c = (ContentPresenter)tabContainer.ItemContainerGenerator.ContainerFromItem(tabContainer.Items[i]);
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

            // make sure the groups in the tabs are compacted and fit within the window screen
            CheckGroupSizes();

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
            if (tabContainer == null) return;
            for (int i = 0; i < tabContainer.Items.Count; i++)
            {
                // I really dislike this roundabout way that I have to get the child items of an ItemsControl, but I guess this is how it is
                // from https://stackoverflow.com/a/1876534/2987285
                ContentPresenter c = (ContentPresenter)tabContainer.ItemContainerGenerator.ContainerFromItem(tabContainer.Items[i]);
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

        /// <summary>
        /// Get or set the index of the tab that is currently selected in this <see cref="Ribbon"/>.
        /// </summary>
        /// <remarks>
        /// Use <see cref="Items"/> for a list of the tabs in this Ribbon. Tabs can also be selected by using <c>Items.Select(<i>tabItemName</i>)</c>.
        /// </remarks>
        public int SelectedIndex { get => (int)GetValue(SelectedIndexProperty); set => SetValue(SelectedIndexProperty, value); }

        /// <summary>The backing dependency property for <see cref="SelectedIndex"/>. See the related property for details.</summary>
        public static DependencyProperty SelectedIndexProperty
            = DependencyProperty.Register("SelectedIndex", typeof(int), typeof(Ribbon),
            new FrameworkPropertyMetadata(-1, (d, e) => d.PerformAs<Ribbon>((r) => r.OnSelectedIndexChange())));

        /// <summary>
        /// Raised when <see cref="SelectedIndex"/> is changed.
        /// </summary>
        void OnSelectedIndexChange()
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

        /// <summary>
        /// Get or set the height of the main bar (where the groups and commands are). The value should be between 24 and 96 pixels - use 24 for a single-line Ribbon, and 96 for a full-size Ribbon.
        /// </summary>
        public double MainBarHeight { get => (double)GetValue(MainBarHeightProperty); set => SetValue(MainBarHeightProperty, value); }

        /// <summary>The backing dependency property for <see cref="MainBarHeight"/>. See the related property for details.</summary>
        public static DependencyProperty MainBarHeightProperty
            = DependencyProperty.Register("MainBarHeight", typeof(double), typeof(Ribbon),
            new FrameworkPropertyMetadata(96.0));

        #region Main Bar Scrolling / Resizing

        #region Scroll Buttons

        /// <summary>
        /// Get if the scroll buttons are currently visible in the main (command area) bar of the Ribbon.
        /// </summary>
        /// <remarks>
        /// You may encounter this being true when you shrink the size of the window or Ribbon, if the current tab has compacting disabled
        /// (<see cref="RibbonTab.FitContentsToWidth"/> set to <c>false</c>), or if the current compacting isn't enough to get everything to fit on screen.
        /// </remarks>
        public bool MainScrollButtonsVisible { get => (bool)GetValue(MainScrollButtonsVisibleProperty); private set => SetValue(MainScrollButtonsVisiblePropertyKey, value); }

        private static readonly DependencyPropertyKey MainScrollButtonsVisiblePropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(MainScrollButtonsVisible), typeof(bool), typeof(Ribbon),
            new FrameworkPropertyMetadata(false));

        /// <summary>The backing dependency property for <see cref="MainScrollButtonsVisible"/>. See the related property for details.</summary>
        public static readonly DependencyProperty MainScrollButtonsVisibleProperty = MainScrollButtonsVisiblePropertyKey.DependencyProperty;

        #endregion

        private void Ribbon_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (mainContainer == null) return;

            if (e.WidthChanged)
            {
                // CheckGroupSizes(); // seems to give undesired results as it is simultaneously trying to compact and also uncompact

                if (e.NewSize.Width < e.PreviousSize.Width) // getting smaller
                {
                    if (ActualWidth < mainContainer.ActualWidth + 2)
                    {
                        CompactGroups();
                        CheckMainScrolling();
                    }
                }
                else // getting bigger
                {
                    if (ActualWidth > mainContainer.ActualWidth + 2)
                    {
                        CheckMainScrolling();
                        UncompactGroups();
                        CheckMainScrolling();
                    }
                }

                //CheckMainScrolling();
            }

            CheckScrolling();
        }

        void CheckGroupSizes()
        {
            if (mainContainer == null) return;
            if (ActualWidth < mainContainer.ActualWidth + 2)
            {
                CompactGroups();
            }
            else
            {
                UncompactGroups();
            }

            CheckMainScrolling();
        }

        void CheckMainScrolling()
        {
            if (mainContainer == null || mainScrollContainer == null || mainBar == null) return;

            if (mainContainer.ActualWidth < mainBar.ActualWidth + 32)
            {
                MainScrollButtonsVisible = false;
            }
            else if (mainContainer.ActualWidth > mainScrollContainer.ViewportWidth)
            {
                MainScrollButtonsVisible = true;
            }
            else
            {
                MainScrollButtonsVisible = false;
            }
        }

        /// <summary>
        /// Force the Ribbon to remeasure the groups on the currently selected tab, and compact or uncompact groups as necessary to fit the Ribbon's width.
        /// </summary>
        /// <remarks>
        /// The measuring and compacting occurs automatically when the Ribbon is resized, this would be a manually-forced remeasure.
        /// The measuring includes a few calls to <see cref="UIElement.InvalidateMeasure"/>, which can cause CPU or GPU usage to increase
        /// and potentially cause UI delays or stuttering, if this function is called too frequently.
        /// </remarks>
        public void MeasureAndCompactGroups()
        {
            CheckGroupSizes();
        }

        void CompactGroups()
        {
            // some logic at first
            if (mainContainer == null) return;
            if (SelectedTab == null) return;
            if (SelectedTab.FitContentsToWidth == false) return;
            if (SelectedTab.Items.Count == 0) return;

            // get the list of groups, but sorted with the largest CompactOrder first
            var groups = SelectedTab.Items.OrderByDescending(g => g.CompactOrder).ToList();
            int selectedGroup = 0;
            bool iconCompacting = false;
            while (ActualWidth < mainContainer.ActualWidth + 2)
            {
                if (selectedGroup >= SelectedTab.Items.Count)
                {
                    // we've gone through all the groups, including getting them all down to an icon
                    if (iconCompacting) break;
                    else
                    {
                        // start again, but this time we'll actually compact groups down to just an icon
                        iconCompacting = true;
                        selectedGroup = 0;
                    }
                }

                if (iconCompacting)
                {
                    // don't bother compacting groups with just 1 (or no) items down to an icon - this just makes for extra clicks for the user
                    if (groups[selectedGroup].Items.Count < 2)
                    {
                        selectedGroup++;
                        continue;
                    }
                    groups[selectedGroup].CompactSize = GroupSizeMode.IconOnly;
                }
                else
                {
                    groups[selectedGroup].CompactSize = GroupSizeMode.Compact;
                }

                // forcing an UpdateLayout run may cause a small performance hit (especially if looping over multiple groups in a row), but at least it works - maybe there's a better way?
                groups[selectedGroup].InvalidateMeasure();
                groups[selectedGroup].UpdateLayout();

                selectedGroup++;
                // value put into breakpoint (set to log as message):
                // changed {groups[selectedGroup].Title} to {groups[selectedGroup].CompactSize}, now {(ActualWidth < mainContainer.ActualWidth + 2) ? "this is still not good" : "now we are good" }
            }
        }

        void UncompactGroups()
        {
            // some logic at first
            if (mainContainer == null) return;
            if (SelectedTab == null) return;
            if (SelectedTab.FitContentsToWidth == false) return;
            if (SelectedTab.Items.Count == 0) return;

            // get the list of groups, but sorted with the smallest CompactOrder first
            var groups = SelectedTab.Items.OrderBy(g => g.CompactOrder).ToList();
            int selectedGroup = 0;
            bool fullUncompacting = false;

            while (ActualWidth > mainContainer.ActualWidth + 2)
            {
                if (selectedGroup >= SelectedTab.Items.Count)
                {
                    // we've gone through all the groups, including getting them all down to an icon
                    if (fullUncompacting) break;
                    else
                    {
                        // start again, but this time we'll actually compact groups down to just an icon
                        fullUncompacting = true;
                        selectedGroup = 0;
                    }
                }

                if (fullUncompacting)
                {
                    // don't bother compacting groups with just 1 (or no) items down to an icon - this just makes for extra clicks for the user
                    groups[selectedGroup].CompactSize = GroupSizeMode.Standard;
                }
                else
                {
                    groups[selectedGroup].CompactSize = GroupSizeMode.Compact;
                }

                // forcing an UpdateLayout run may cause a small performance hit (especially if looping over multiple groups in a row), but at least it works - maybe there's a better way?
                groups[selectedGroup].InvalidateMeasure();
                groups[selectedGroup].UpdateLayout();

                if (ActualWidth < mainContainer.ActualWidth + 2)
                {
                    // we've gone too far! go back!
                    if (fullUncompacting)
                    {
                        groups[selectedGroup].CompactSize = GroupSizeMode.Compact;
                    }
                    else
                    {
                        if (groups[selectedGroup].Items.Count < 2)
                        {
                            break;
                        }
                        groups[selectedGroup].CompactSize = GroupSizeMode.IconOnly;
                    }
                    break;
                }

                selectedGroup++;
            }
        }

        private void OnMainScrollCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (mainScrollContainer == null) return;

            double offset = mainScrollContainer.HorizontalOffset;

            if (e.Parameter is TabScrollCommandAction a)
            {
                switch (a)
                {
                    case TabScrollCommandAction.Left:
                        mainScrollContainer.ScrollToHorizontalOffset(Math.Max(offset - 40, 0));
                        break;
                    case TabScrollCommandAction.Right:
                        mainScrollContainer.ScrollToHorizontalOffset(Math.Min(offset + 40, mainScrollContainer.ScrollableWidth));
                        break;
                    case TabScrollCommandAction.Home:
                        mainScrollContainer.ScrollToHorizontalOffset(0);
                        break;
                    case TabScrollCommandAction.End:
                        mainScrollContainer.ScrollToHorizontalOffset(mainScrollContainer.ScrollableWidth);
                        break;
                    default:
                        break;
                }
            }
        }

        private void mc_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.WidthChanged)
            {
                // update resizing algorithm
                // and also see if we can hide the scroll buttons
                //CheckMainScrolling();
            }
        }

        private void mv_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            // update scroll buttons as needed (if they are visible)
            if (e.ViewportWidthChange > 0)
            {
                CheckMainScrolling();
            }
        }

        #endregion

        #endregion

        #region Tab Bar Elements / Properties

        #region ShowTabsOnBottom

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
        #endregion

        #region Main Properties

        /// <summary>The backing dependency property for <see cref="FileMenu"/>. See the related property for details.</summary>
        public static readonly DependencyProperty FileMenuProperty = DependencyProperty.Register(
            "FileMenu", typeof(UIElement), typeof(Ribbon),
            new PropertyMetadata(null));

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

        /// <summary>The backing dependency property for <see cref="TopRightElement"/>. See the related property for details.</summary>
        public static readonly DependencyProperty TopRightElementProperty = DependencyProperty.Register(
            "TopRightElement", typeof(UIElement), typeof(Ribbon),
            new PropertyMetadata(null));

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

        /// <summary>
        /// Get or set if only the tab bar should be visible. In this state, clicking on a tab will display its groups temporarily until a command is selected.
        /// </summary>
        public bool ShowOnlyTabs { get => (bool)GetValue(ShowOnlyTabsProperty); set => SetValue(ShowOnlyTabsProperty, value); }

        /// <summary>The backing dependency property for <see cref="ShowOnlyTabs"/>. See the related property for details.</summary>
        public static DependencyProperty ShowOnlyTabsProperty
            = DependencyProperty.Register("ShowOnlyTabs", typeof(bool), typeof(Ribbon),
            new FrameworkPropertyMetadata(false));

        #endregion

        #region Setup RibbonTabDisplayItem / Tdi Event Handlers

        /// <summary>
        /// Set up a new RibbonTabDisplayItem that was added to this Ribbon.
        /// This will set up the necessary event handlers and other properties to allow the RibbonTabDisplayItem to interact with the Ribbon.
        /// </summary>
        /// <param name="tdi">The RibbonTabDisplayItem to set up.</param>
        internal protected void SetupTabDisplay(RibbonTabDisplayItem tdi)
        {
            //tdi.Click += tdi_Click;
            tdi.RightClick += tdi_RightClick;
            tdi.RequestSelect += tdi_RequestSelect;
            tdi.TabItemDrop += tdi_TabItemDrop;

            CheckScrolling();
        }

#if NETCOREAPP
        private void tdi_RequestSelect(object? sender, EventArgs e)
#else
        private void tdi_RequestSelect(object sender, EventArgs e)
#endif
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
                // TODO: add on Ribbon context menu

                //if (tdi.TabItem.TabContextMenu != null)
                //{
                //    ContextMenu cm = tdi.TabItem.TabContextMenu;
                //    cm.ColorScheme = ColorScheme;
                //    cm.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
                //    cm.IsOpen = true;
                //}
            }
        }

        #endregion

        #region Tab Bar Scrolling

        #region ScrollButtons

        private static readonly DependencyPropertyKey TabScrollButtonsVisiblePropertyKey = DependencyProperty.RegisterReadOnly("TabScrollButtonsVisible", typeof(bool), typeof(Ribbon),
            new PropertyMetadata(false));

        /// <summary>The backing dependency property for <see cref="TabScrollButtonsVisible"/>. See the related property for details.</summary>
        public static readonly DependencyProperty TabScrollButtonsVisibleProperty = TabScrollButtonsVisiblePropertyKey.DependencyProperty;

        /// <summary>
        /// Get if the scroll buttons are currently visible in the tab bar.
        /// </summary>
        public bool TabScrollButtonsVisible
        {
            get { return (bool)GetValue(TabScrollButtonsVisibleProperty); }
            private set { SetValue(TabScrollButtonsVisiblePropertyKey, value); }
        }
        #endregion

        void CheckScrolling()
        {
            if (tabScrollContainer == null || tabContainer == null) return;

            if (tabScrollContainer.ViewportWidth == 0)
            {
                return;
            }

            if (tabContainer.ActualWidth > tabScrollContainer.ViewportWidth)
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
            if (tabScrollContainer == null) return;

            double offset = tabScrollContainer.HorizontalOffset;

            if (e.Parameter is TabScrollCommandAction a)
            {
                switch (a)
                {
                    case TabScrollCommandAction.Left:
                        tabScrollContainer.ScrollToHorizontalOffset(Math.Max(offset - 20, 0));
                        break;
                    case TabScrollCommandAction.Right:
                        tabScrollContainer.ScrollToHorizontalOffset(Math.Min(offset + 20, tabScrollContainer.ScrollableWidth));
                        break;
                    case TabScrollCommandAction.Home:
                        tabScrollContainer.ScrollToHorizontalOffset(0);
                        break;
                    case TabScrollCommandAction.End:
                        tabScrollContainer.ScrollToHorizontalOffset(tabScrollContainer.ScrollableWidth);
                        break;
                    default:
                        break;
                }
            }
        }

        private void tabBar_SizeChanged(object sender, SizeChangedEventArgs e)
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

        #endregion

        #region Reordering

        /// <summary>
        /// Get or set if tabs, groups, and controls are able to be re-arranged on this Ribbon. (Note that for 2.0, this will not be supported, and will be iterated upon further in later releases.)
        /// </summary>
        private bool AllowControlReordering { get => (bool)GetValue(AllowControlReorderingProperty); set => SetValue(AllowControlReorderingProperty, value); }

        /// <summary>The backing dependency property for <see cref="AllowControlReordering"/>. See the related property for details.</summary>
        private static DependencyProperty AllowControlReorderingProperty
            = DependencyProperty.Register("AllowControlReordering", typeof(bool), typeof(Ribbon),
            new FrameworkPropertyMetadata(false));

        #region Tdi Drag & Drop

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

        #endregion

        #endregion

        #region Brushes

        /// <summary>
        /// Get or set the brush to use for the background of the main command bar area of the Ribbon.
        /// </summary>
        public Brush MainBarBackground { get => (Brush)GetValue(MainBarBackgroundProperty); set => SetValue(MainBarBackgroundProperty, value); }

        /// <summary>The backing dependency property for <see cref="MainBarBackground"/>. See the related property for details.</summary>
        public static DependencyProperty MainBarBackgroundProperty
            = DependencyProperty.Register("MainBarBackground", typeof(Brush), typeof(Ribbon),
            new FrameworkPropertyMetadata(Colors.White.ToBrush()));

        /// <summary>
        /// Get or set the brush to use for the background of the tab area of the Ribbon (where the tab labels are).
        /// </summary>
        public Brush TabBarBackground { get => (Brush)GetValue(TabBarBackgroundProperty); set => SetValue(TabBarBackgroundProperty, value); }

        /// <summary>The backing dependency property for <see cref="TabBarBackground"/>. See the related property for details.</summary>
        public static DependencyProperty TabBarBackgroundProperty
            = DependencyProperty.Register("TabBarBackground", typeof(Brush), typeof(Ribbon),
            new FrameworkPropertyMetadata(Colors.White.ToBrush()));

        /// <summary>
        /// Get or set the brush to use for borders in the Ribbon - such as the border around the main command bar area and around the tab headers.
        /// </summary>
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
        private static void OnColorSchemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
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

            // TODO: replace applying the ColorScheme directly below with applying these yet-to-be-made properties
            // to achieve this, I think I'll need to build for myself a small function that goes into each RibbonItem and each group,
            // allowing me to change values as needed (in case users change the brush properties and don't use ColorScheme)

            /*
            SubitemBackground
            SubitemHighlight
            SubitemBorderBrush
            SubitemClickBrush
            SubitemBorderHighlight
            SubitemSelectedBrush
            SubitemBorderSelectedBrush
             */

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

            // TODO: replace check for RibbonFileMenu with a check for a control that has a ColorScheme/SsuiTheme
            if (FileMenu is RibbonFileMenu rfm)
            {
                rfm.ColorScheme = cs;
            }
        }
        #endregion


#if DEBUG

        /// <summary>
        /// Get a status string indicating the size and status of elements inside this Ribbon. Used for debugging.
        /// </summary>
        public string GetSizeStatuses()
        {
            return "main container: " + (mainContainer?.ActualWidth.ToString() ?? "(not defined)") + "\n"
                + "main scroll actual: " + (mainScrollContainer?.ActualWidth.ToString() ?? "(not defined)") + "\n"
                + "main scroll viewport: " + (mainScrollContainer?.ViewportWidth.ToString() ?? "(not defined)") + "\n"
                + "main bar: " + (mainBar?.ActualWidth.ToString() ?? "(not defined)") + "\n"
                + "show scroll buttons: " + MainScrollButtonsVisible.ToString();
        }

#endif

    }
}
