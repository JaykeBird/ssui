using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using SolidShineUi.Utils;
using System.Windows;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;
using System.ComponentModel;

namespace SolidShineUi
{
    /// <summary>
    /// A control that can house multiple controls under a number of tabs. Each tab has a title, icon, and close button (see <see cref="TabItem"/>).
    /// </summary>
    public class TabControl : Control
    {

        static TabControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TabControl), new FrameworkPropertyMetadata(typeof(TabControl)));
        }

        public TabControl()
        {
            Loaded += TabControl_Loaded;
            SizeChanged += control_SizeChanged;

            Items = new SelectableCollection<TabItem>();
            Items.CanSelectMultiple = false;
            Items.CollectionChanged += Items_CollectionChanged;
            Items.SelectionChanged += items_SelectionChanged;
            Items.ItemRemoving += Items_ItemRemoving;

            InternalShowTabsOnBottomChanged += tabControl_InternalShowTabsOnBottomChanged;
            InternalShowTabListMenuChanged += tabControl_InternalShowTabListMenuChanged;
            InternalTabMinWidthChanged += tabControl_InternalTabMinWidthChanged;

            CommandBindings.Add(new CommandBinding(TabListMenuItemClick, OnTabListMenuItemClick));
            CommandBindings.Add(new CommandBinding(TabBarScrollCommand, OnScrollCommand, (s, e) => { e.CanExecute = ScrollButtonsVisible; }));
        }

        /// <summary>
        /// Get or set if the first tab should be selected right away when the control is loaded. This property has no effect after the control is loaded.
        /// </summary>
        public bool SelectFirstTabOnLoad { get; set; } = true;

        private void TabControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (Items.Count > 0 && SelectFirstTabOnLoad)
            {
                Items.Select(Items[0]);
            }
        }

        bool itemsLoaded = false;

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

#if NETCOREAPP
        private void Items_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems != null)
                    {
                        foreach (TabItem? ti in e.OldItems)
                        {
                            if (ti != null)
                            {
                                TabClosed?.Invoke(this, new TabItemChangeEventArgs(ti));
                            }
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (e.OldItems != null)
                    {
                        foreach (TabItem? ti in e.OldItems)
                        {
                            if (ti != null)
                            {
                                TabClosed?.Invoke(this, new TabItemChangeEventArgs(ti));
                            }
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    TabsCleared?.Invoke(this, EventArgs.Empty);
                    break;
                default:
                    break;
            }
#else
        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // update .NET Core section first, copy into here
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems != null)
                    {
                        foreach (TabItem ti in e.OldItems)
                        {
                            if (ti != null)
                            {
                                TabClosed?.Invoke(this, new TabItemChangeEventArgs(ti));
                            }
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (e.OldItems != null)
                    {
                        foreach (TabItem ti in e.OldItems)
                        {
                            if (ti != null)
                            {
                                TabClosed?.Invoke(this, new TabItemChangeEventArgs(ti));
                            }
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    TabsCleared?.Invoke(this, EventArgs.Empty);
                    break;
                default:
                    break;
            }
#endif

            LoadTemplateItems();
            CheckScrolling();
        }

        private void items_SelectionChanged(object sender, SelectionChangedEventArgs<TabItem> e)
        {
            LoadTemplateItems();

            if (e.AddedItems.Count > 0)
            {
                // selection has changed
                TabItem newItem = e.AddedItems[0];

                if (ic != null)
                {
                    for (int i = 0; i < ic.Items.Count; i++)
                    {
                        // I really dislike this roundabout way that I have to get the child items of an ItemsControl, but I guess this is how it is
                        // from https://stackoverflow.com/a/1876534/2987285
                        ContentPresenter c = (ContentPresenter)ic.ItemContainerGenerator.ContainerFromItem(ic.Items[i]);
                        c.ApplyTemplate();
#if NETCOREAPP
                        TabDisplayItem? tb = c.ContentTemplate.FindName("PART_TabItem", c) as TabDisplayItem;
#else
                        TabDisplayItem tb = c.ContentTemplate.FindName("PART_TabItem", c) as TabDisplayItem;
#endif
                        if (tb != null)
                        {
                            if (tb.TabItem != null && tb.TabItem == newItem)
                            {
                                tb.IsSelected = true;
                            }
                            else
                            {
                                tb.IsSelected = false;
                            }
                        }
                    }
                }

                if (ch != null)
                {
                    ch.Child = newItem.Content;
                }
                TabChanged?.Invoke(this, new TabItemChangeEventArgs(newItem));
            }
            else
            {
                if (Items.SelectedItems.Count == 0)
                {
                    if (ch != null)
                    {
                        ch.Child = null;
                    }

                    if (Items.Count > 0)
                    {
                        switch (SelectedTabClosedAction)
                        {
                            case SelectedTabCloseAction.SelectNothing:
                                // nothing to do
                                break;
                            case SelectedTabCloseAction.SelectFirstTab:
                                Items.Select(Items[0]);
                                break;
                            case SelectedTabCloseAction.SelectLastTab:
                                Items.Select(Items[Items.Count - 1]);
                                break;
                            case SelectedTabCloseAction.SelectTabToLeft:
                                if (closedTabIndex == -1)
                                {
                                    // most likely closed via Items.Remove command
                                    Items.Select(Items[0]);
                                }
                                else if (closedTabIndex == 0)
                                {
                                    // left most tab closed
                                    Items.Select(Items[0]);
                                }
                                else
                                {
                                    Items.Select(Items[closedTabIndex - 1]);
                                }
                                break;
                            case SelectedTabCloseAction.SelectTabToRight:
                                if (closedTabIndex == -1)
                                {
                                    // most likely closed via Items.Remove command
                                    Items.Select(Items[Items.Count - 1]);
                                }
                                else
                                {
                                    Items.Select(Items[closedTabIndex]);
                                }
                                break;
                            default:
                                // treat as if SelectNothing
                                break;
                        }
                    }
                }
            }
        }

        private void Items_ItemRemoving(object sender, ItemRemovingEventArgs<TabItem> e)
        {
            if (e.Item != null)
            {
                TabItemClosingEventArgs ee = new TabItemClosingEventArgs(e.Item);
                TabClosing?.Invoke(this, ee);

                if (ee.Cancel)
                {
                    // don't remove or close anything, just exit
                    closedTabIndex = -1;
                    e.Cancel = true;
                    return;
                }

                if (Items.SelectedItems.Contains(e.Item) && (SelectedTabClosedAction == SelectedTabCloseAction.SelectTabToLeft || SelectedTabClosedAction == SelectedTabCloseAction.SelectTabToRight))
                {
                    closedTabIndex = Items.IndexOf(e.Item);
                }
                else
                {
                    closedTabIndex = -1;
                }
            }
        }

        //private static readonly DependencyProperty ItemsProperty
        //    = DependencyProperty.Register("Items", typeof(ObservableCollection<TabItem>), typeof(TabControl),
        //    new FrameworkPropertyMetadata(new ObservableCollection<TabItem>()));

        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register("Items", typeof(SelectableCollection<TabItem>), typeof(TabControl),
            new FrameworkPropertyMetadata(new SelectableCollection<TabItem>()));

        //public static readonly DependencyProperty ItemsProperty = ItemsPropertyKey.DependencyProperty;

        /// <summary>
        /// Get or set the list of tabs in this TabControl. This Items property can be used to add tabs, remove tabs, and also select tabs via the Select method.
        /// </summary>
        [Category("Common")]
        public SelectableCollection<TabItem> Items
        {
            get { return (SelectableCollection<TabItem>)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        public delegate void TabItemChangeEventHandler(object sender, TabItemChangeEventArgs e);
        public delegate void TabItemClosingEventHandler(object sender, TabItemClosingEventArgs e);

#if NETCOREAPP
        /// <summary>
        /// Get the tab currently selected. Use <c>Items.Select()</c> to select another tab.
        /// </summary>
        [Category("Common")]
        public TabItem? CurrentTab { get => Items.SelectedItems.FirstOrDefault(); }

        /// <summary>
        /// Get the tab currently selected. Use <c>Items.Select()</c> to select another tab.
        /// </summary>
        public TabItem? SelectedTab { get => CurrentTab; }

        /// <summary>
        /// Raised when the tab currently selected is changed.
        /// </summary>
        public event TabItemChangeEventHandler? TabChanged;
        /// <summary>
        /// Raised when a tab is about to be closed, with the ability to cancel the action.
        /// </summary>
        public event TabItemClosingEventHandler? TabClosing;
        /// <summary>
        /// Raised when a tab is closed.
        /// </summary>
        public event TabItemChangeEventHandler? TabClosed;
        /// <summary>
        /// Raised when all tabs are closed at once (via <c>Items.Clear()</c>).
        /// </summary>
        public event EventHandler? TabsCleared;
#else
        /// <summary>
        /// Get the tab currently selected. Use <c>Items.Select()</c> to select another tab.
        /// </summary>
        [Category("Common")]
        public TabItem CurrentTab { get; protected set; } = null;
        /// <summary>
        /// Get the tab currently selected. Use <c>Items.Select()</c> to select another tab.
        /// </summary>
        public TabItem SelectedTab { get => CurrentTab; }

        /// <summary>
        /// Raised when the tab currently selected is changed.
        /// </summary>
        public event TabItemChangeEventHandler TabChanged;
        /// <summary>
        /// Raised when a tab is about to be closed, with the ability to cancel the action.
        /// </summary>
        public event TabItemClosingEventHandler TabClosing;
        /// <summary>
        /// Raised when a tab is closed.
        /// </summary>
        public event TabItemChangeEventHandler TabClosed;
        /// <summary>
        /// Raised when all tabs are closed at once (via <c>Items.Clear()</c>).
        /// </summary>
        public event EventHandler TabsCleared;
#endif

        #region ShowTabsOnBottom

        public static readonly DependencyProperty ShowTabsOnBottomProperty = DependencyProperty.Register("ShowTabsOnBottom", typeof(bool), typeof(TabControl),
            new PropertyMetadata(false, new PropertyChangedCallback(OnInternalShowTabsOnBottomChanged)));

        /// <summary>
        /// Get or set if the tab bar should be shown at the bottom of the control, rather than the top.
        /// </summary>
        [Category("Common")]
        public bool ShowTabsOnBottom
        {
            get { return (bool)GetValue(ShowTabsOnBottomProperty); }
            set { SetValue(ShowTabsOnBottomProperty, value); }
        }

        protected event DependencyPropertyChangedEventHandler InternalShowTabsOnBottomChanged;

#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? ShowTabsOnBottomChanged;
#else
        public event DependencyPropertyChangedEventHandler ShowTabsOnBottomChanged;
#endif

        private static void OnInternalShowTabsOnBottomChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TabControl s)
            {
                s.InternalShowTabsOnBottomChanged?.Invoke(s, e);
            }
        }
        private void tabControl_InternalShowTabsOnBottomChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ShowTabsOnBottomChanged?.Invoke(this, e);
        }
        #endregion

        #region ShowTabListMenu

        public static readonly DependencyProperty ShowTabListMenuProperty = DependencyProperty.Register("ShowTabListMenu", typeof(bool), typeof(TabControl),
            new PropertyMetadata(true, new PropertyChangedCallback(OnInternalShowTabListMenuChanged)));

        /// <summary>
        /// Get or set if a tab list menu should be shown on the far-right edge of the control, listing all the open tabs. This mimics a similar menu in Visual Studio.
        /// </summary>
        [Category("Common")]
        public bool ShowTabListMenu
        {
            get { return (bool)GetValue(ShowTabListMenuProperty); }
            set { SetValue(ShowTabListMenuProperty, value); }
        }

        protected event DependencyPropertyChangedEventHandler InternalShowTabListMenuChanged;

#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? ShowTabListMenuChanged;
#else
        public event DependencyPropertyChangedEventHandler ShowTabListMenuChanged;
#endif

        private static void OnInternalShowTabListMenuChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TabControl s)
            {
                s.InternalShowTabListMenuChanged?.Invoke(s, e);
            }
        }
        private void tabControl_InternalShowTabListMenuChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ShowTabListMenuChanged?.Invoke(this, e);
        }
        #endregion

        #region TabMinWidth

        public static readonly DependencyProperty TabMinWidthProperty = DependencyProperty.Register("TabMinWidth", typeof(double), typeof(TabControl),
            new PropertyMetadata(120.0d, new PropertyChangedCallback(OnInternalTabMinWidthChanged)));

        /// <summary>
        /// Get or set the minimum width a tab should have in the tab bar. While tabs may be wider than this width, they will never be shorter than it.
        /// </summary>
        [Category("Layout")]
        public double TabMinWidth
        {
            get { return (double)GetValue(TabMinWidthProperty); }
            set { SetValue(TabMinWidthProperty, value); }
        }

        protected event DependencyPropertyChangedEventHandler InternalTabMinWidthChanged;

#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? TabMinWidthChanged;
#else
        public event DependencyPropertyChangedEventHandler TabMinWidthChanged;
#endif

        private static void OnInternalTabMinWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TabControl s)
            {
                s.InternalTabMinWidthChanged?.Invoke(s, e);
            }
        }
        private void tabControl_InternalTabMinWidthChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            TabMinWidthChanged?.Invoke(this, e);

            if (ic != null)
            {
                for (int i = 0; i < ic.Items.Count; i++)
                {
                    // I really dislike this roundabout way that I have to get the child items of an ItemsControl, but I guess this is how it is
                    // from https://stackoverflow.com/a/1876534/2987285
                    ContentPresenter c = (ContentPresenter)ic.ItemContainerGenerator.ContainerFromItem(ic.Items[i]);
                    c.ApplyTemplate();

                    if (c.ContentTemplate.FindName("PART_TabItem", c) is TabDisplayItem tb)
                    {
                        tb.MinWidth = (double)e.NewValue;
                    }
                }
            }
        }
        #endregion

        #region SelectedTabClosedAction

        public static readonly DependencyProperty SelectedTabClosedActionProperty = DependencyProperty.Register("SelectedTabClosedAction", typeof(SelectedTabCloseAction), typeof(TabControl),
            new PropertyMetadata(SelectedTabCloseAction.SelectTabToLeft));

        /// <summary>
        /// Get or set the action to take when the currently selected tab is closed.
        /// </summary>
        public SelectedTabCloseAction SelectedTabClosedAction
        {
            get { return (SelectedTabCloseAction)GetValue(SelectedTabClosedActionProperty); }
            set { SetValue(SelectedTabClosedActionProperty, value); }
        }
        #endregion

        public static readonly RoutedCommand TabListMenuItemClick = new RoutedCommand();

        private void OnTabListMenuItemClick(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter is TabItem ti)
            {
                Items.Select(ti);
            }
        }

        #region Color Scheme

        public static readonly DependencyProperty ColorSchemeProperty
            = DependencyProperty.Register("ColorScheme", typeof(ColorScheme), typeof(TabControl),
            new FrameworkPropertyMetadata(new ColorScheme(), new PropertyChangedCallback(OnColorSchemeChanged)));

        public static void OnColorSchemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
#if NETCOREAPP
            if (d is TabControl w)
            {
                w.ApplyColorScheme((e.NewValue as ColorScheme)!);
            }
#else
            (d as TabControl).ApplyColorScheme(e.NewValue as ColorScheme);
#endif
        }

        /// <summary>
        /// Get or set the color scheme to apply to the control.
        /// </summary>
        [Category("Appearance")]
        public ColorScheme ColorScheme
        {
            get => (ColorScheme)GetValue(ColorSchemeProperty);
            set => SetValue(ColorSchemeProperty, value);
        }

        public void ApplyColorScheme(ColorScheme cs)
        {
            if (cs != ColorScheme)
            {
                ColorScheme = cs;
                return;
            }
        }
        #endregion

        internal protected void SetupTabDisplay(TabDisplayItem tdi)
        {
            tdi.RequestClose += tdi_RequestClose;
            tdi.Click += tdi_Click;
            tdi.RightClick += tdi_RightClick;
            tdi.TabItemDrop += tdi_TabItemDrop;
            tdi.MinWidth = TabMinWidth;

            CheckScrolling();
        }

#if NETCOREAPP
        private void tdi_RightClick(object? sender, EventArgs e)
#else
        private void tdi_RightClick(object sender, EventArgs e)
#endif
        {
            if (sender is TabDisplayItem tdi)
            {
                if (tdi.TabItem.TabContextMenu != null)
                {
                    ContextMenu cm = tdi.TabItem.TabContextMenu;
                    cm.ColorScheme = ColorScheme;
                    cm.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
                    cm.IsOpen = true;
                }
            }
        }

        private void tdi_TabItemDrop(object sender, TabItemChangeEventArgs e)
        {
            //throw new NotImplementedException();
        }

#if NETCOREAPP
        private void tdi_Click(object? sender, EventArgs e)
#else
        private void tdi_Click(object sender, EventArgs e)
#endif
        {
            if (sender != null && sender is TabDisplayItem tdi)
            {
                if (tdi.TabItem != null)
                {
                    Items.Select(tdi.TabItem);
                }
            }
        }

        int closedTabIndex = -1;

#if NETCOREAPP
        private void tdi_RequestClose(object? sender, EventArgs e)
#else
        private void tdi_RequestClose(object sender, EventArgs e)
#endif
        {
            if (sender != null && sender is TabDisplayItem tdi)
            {
                if (tdi.TabItem != null)
                {
                    TabItemClosingEventArgs ee = new TabItemClosingEventArgs(tdi.TabItem);
                    TabClosing?.Invoke(this, ee);

                    if (ee.Cancel)
                    {
                        // don't remove or close anything, just exit
                        closedTabIndex = -1;
                        return;
                    }

                    if (tdi.IsSelected && (SelectedTabClosedAction == SelectedTabCloseAction.SelectTabToLeft || SelectedTabClosedAction == SelectedTabCloseAction.SelectTabToRight))
                    {
                        closedTabIndex = Items.IndexOf(tdi.TabItem);
                    }
                    else
                    {
                        closedTabIndex = -1;
                    }
                    Items.Remove(tdi.TabItem);
                }
            }
        }

#region Scrolling

#region ScrollButtons

        private static readonly DependencyPropertyKey ScrollButtonsVisiblePropertyKey = DependencyProperty.RegisterReadOnly("ScrollButtonsVisible", typeof(bool), typeof(TabControl),
            new PropertyMetadata(false));

        public static readonly DependencyProperty ScrollButtonsVisibleProperty = ScrollButtonsVisiblePropertyKey.DependencyProperty;

        /// <summary>
        /// Get if the scroll buttons are currently visible in the tab bar.
        /// </summary>
        public bool ScrollButtonsVisible
        {
            get { return (bool)GetValue(ScrollButtonsVisibleProperty); }
            private set { SetValue(ScrollButtonsVisiblePropertyKey, value); }
        }
#endregion

        public static readonly RoutedCommand TabBarScrollCommand = new RoutedCommand("TabBarScrollCommand", typeof(TabControl));

        void CheckScrolling()
        {
            if (sv == null || ic == null) return;

            if (sv.ViewportWidth == 0)
            {
                return;
            }

            if (ic.ActualWidth > sv.ViewportWidth)
            {
                ScrollButtonsVisible = true;
            }
            else
            {
                ScrollButtonsVisible = false;
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

    public class TabItemClosingEventArgs
    {
        public TabItemClosingEventArgs(TabItem t)
        {
            TabItem = t;
        }

        public TabItem TabItem { get; private set; }

        public bool Cancel { get; set; } = false;
    }

    public class TabItemChangeEventArgs
    {
        public TabItemChangeEventArgs(TabItem t)
        {
            TabItem = t;
        }

        public TabItem TabItem { get; private set; }
    }

    /// <summary>
    /// References the action to take when a currently-selected tab is closed.
    /// </summary>
    public enum SelectedTabCloseAction
    {
        /// <summary>Do not select anything; all tabs are deselected and nothing is shown.</summary>
        SelectNothing = 0,
        /// <summary>Select the first (leftmost) tab on the TabControl.</summary>
        SelectFirstTab = 1,
        /// <summary>Select the last (rightmost) tab on the TabControl.</summary>
        SelectLastTab = 2,
        /// <summary>Select the tab to the left of the one being closed.</summary>
        SelectTabToLeft = 3,
        /// <summary>Select the tab to the right of the one being closed.</summary>
        SelectTabToRight = 4,
    }
}
