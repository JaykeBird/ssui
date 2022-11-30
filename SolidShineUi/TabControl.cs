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
using System.Windows.Markup;

namespace SolidShineUi
{
    /// <summary>
    /// A control that can house multiple controls under a number of tabs. Each tab has a title, icon, and close button (see <see cref="TabItem"/>).
    /// </summary>
    [ContentProperty("Items")]
    public class TabControl : Control
    {

        static TabControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TabControl), new FrameworkPropertyMetadata(typeof(TabControl)));
        }


        /// <summary>
        /// Create a TabControl.
        /// </summary>
        public TabControl()
        {
            Loaded += TabControl_Loaded;
            SizeChanged += control_SizeChanged;

            SetValue(ItemsPropertyKey, new SelectableCollection<TabItem>());
            Items.CanSelectMultiple = false;
            Items.CollectionChanged += Items_CollectionChanged;
            Items.SelectionChanged += items_SelectionChanged;
            Items.ItemRemoving += Items_ItemRemoving;

            InternalShowTabsOnBottomChanged += tabControl_InternalShowTabsOnBottomChanged;
            InternalShowTabListMenuChanged += tabControl_InternalShowTabListMenuChanged;
            InternalTabMinWidthChanged += tabControl_InternalTabMinWidthChanged;
            InternalAllowTabDragDropChanged += tabControl_InternalAllowDragDropChanged;
            InternalHorizontalTabBarHeightChanged += tabControl_InternalHorizontalTabBarHeightChanged;

            CommandBindings.Add(new CommandBinding(TabBarScrollCommand, OnScrollCommand, (s, e) => { e.CanExecute = ScrollButtonsVisible; }));

            CommandBindings.Add(new CommandBinding(CloseCurrentTab, DoCloseCurrentTab, CanExecuteIfAnyTabSelected));
            CommandBindings.Add(new CommandBinding(CloseSpecificTab, DoCloseSpecificTab, CanExecuteIfTabPresent));
            CommandBindings.Add(new CommandBinding(SwitchToTab, DoSwitchToTab, CanExecuteIfTabPresent));
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

        #region SelectableCollection handling

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        private static readonly DependencyPropertyKey ItemsPropertyKey = DependencyProperty.RegisterReadOnly("Items", typeof(SelectableCollection<TabItem>), typeof(TabControl),
            new FrameworkPropertyMetadata(new SelectableCollection<TabItem>()));

        public static readonly DependencyProperty ItemsProperty = ItemsPropertyKey.DependencyProperty;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Get or set the list of tabs in this TabControl. This Items property can be used to add tabs, remove tabs, and also select tabs via the Select method.
        /// </summary>
        [Category("Common")]
        public SelectableCollection<TabItem> Items
        {
            get { return (SelectableCollection<TabItem>)GetValue(ItemsProperty); }
            private set { SetValue(ItemsPropertyKey, value); }
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

        private void items_SelectionChanged(object sender, CollectionSelectionChangedEventArgs e)
        {
            LoadTemplateItems();
            if (_internalAction) return;

            if (((SelectionChangedEventArgs<TabItem>)e).AddedItems.Count > 0)
            {
                // selection has changed
                TabItem newItem = ((SelectionChangedEventArgs<TabItem>)e).AddedItems[0];

                if (ic != null)
                {
                    for (int i = 0; i < ic.Items.Count; i++)
                    {
                        // I really dislike this roundabout way that I have to get the child items of an ItemsControl, but I guess this is how it is
                        // from https://stackoverflow.com/a/1876534/2987285
                        ContentPresenter c = (ContentPresenter)ic.ItemContainerGenerator.ContainerFromItem(ic.Items[i]);
                        c.ApplyTemplate();
                        //#if NETCOREAPP
                        //                        TabDisplayItem? tb = c.ContentTemplate.FindName("PART_TabItem", c) as TabDisplayItem;
                        //#else
                        //#endif
                        if (c.ContentTemplate.FindName("PART_TabItem", c) is TabDisplayItem tb)
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

                SetupCurrentTab(newItem);

                TabChanged?.Invoke(this, new TabItemChangeEventArgs(newItem));
            }
            else
            {
                if (Items.SelectedItems.Count == 0)
                {
                    SetupCurrentTab(null);

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
            if (_internalAction) return;
            if (e.Item != null)
            {
                if (PrepareCloseTab(e.Item))
                {
                    // good to go, event can continue with closing tab
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }
        #endregion

        #region Internal Base Functions

        /// <summary>
        /// Set up the process to close a tab. This function does not actually close the tab.
        /// </summary>
        /// <param name="tab">The tab to close.</param>
        /// <returns><c>true</c> if the tab is good to be closed (wasn't cancelled); <c>false</c> if it was cancelled.</returns>
        bool PrepareCloseTab(TabItem tab)
        {
            // raise the closing event (to give implementers the chance to cancel)
            TabItemClosingEventArgs ee = new TabItemClosingEventArgs(tab);
            TabClosing?.Invoke(this, ee);

            if (ee.Cancel)
            {
                // don't remove or close anything, just exit
                closedTabIndex = -1;
                return false;
            }

            // if SelectedTabClosedAction is set to SelectTabToLeft or SelectTabToRight, then let's store the current tab's index before it's closed, so that we can use it later
            if (Items.SelectedItems.Contains(tab) && (SelectedTabClosedAction == SelectedTabCloseAction.SelectTabToLeft || SelectedTabClosedAction == SelectedTabCloseAction.SelectTabToRight))
            {
                closedTabIndex = Items.IndexOf(tab);
            }
            else
            {
                closedTabIndex = -1;
            }
            return true;
        }

#if NETCOREAPP
        void SetupCurrentTab(TabItem? tab)
#else
        void SetupCurrentTab(TabItem tab)
#endif
        {
            //if (ch != null)
            //{
            //    ch.Child = tab?.Content;
            //}

            SelectedTab = tab;
            SelectedTabContent = tab?.Content;
        }

        #endregion


        #region Basic Properties / Events

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
            = DependencyProperty.RegisterReadOnly("CurrentTab", typeof(TabItem), typeof(TabControl), new FrameworkPropertyMetadata(null));

        private static readonly DependencyPropertyKey SelectedTabContentPropertyKey
            = DependencyProperty.RegisterReadOnly("Content", typeof(UIElement), typeof(TabControl), new FrameworkPropertyMetadata(null));

#if NETCOREAPP
        /// <summary>
        /// Get the tab currently selected. Use <c>Items.Select()</c> to select another tab.
        /// </summary>
        [Category("Common")]
        public TabItem? SelectedTab { get => (TabItem)GetValue(SelectedTabProperty); private set => SetValue(SelectedTabPropertyKey, value); }

        /// <summary>
        /// Get the currently shown content in the main part of the control. This is the content of the currently selected tab.
        /// </summary>
        public UIElement? SelectedTabContent { get => (UIElement)GetValue(SelectedTabContentProperty); private set => SetValue(SelectedTabContentPropertyKey, value); }

        /// <summary>
        /// A dependency property object backing a related property. See the related property for more details.
        /// </summary>
        public static readonly DependencyProperty SelectedTabProperty = SelectedTabPropertyKey!.DependencyProperty;

        /// <summary>
        /// A dependency property object backing a related property. See the related property for more details.
        /// </summary>
        public static readonly DependencyProperty SelectedTabContentProperty = SelectedTabContentPropertyKey!.DependencyProperty;

        /// <summary>
        /// Get the tab currently selected. Use <c>Items.Select()</c> to select another tab.
        /// </summary>
        public TabItem? CurrentTab { get => SelectedTab; }

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
        public TabItem SelectedTab { get => (TabItem)GetValue(SelectedTabProperty); private set => SetValue(SelectedTabPropertyKey, value); }

        /// <summary>
        /// Get the currently shown content in the main part of the control. This is the content of the currently selected tab.
        /// </summary>
        public UIElement SelectedTabContent { get => (UIElement)GetValue(SelectedTabContentProperty); private set => SetValue(SelectedTabContentPropertyKey, value); }
        
        /// <summary>
        /// A dependency property object backing a related property. See the related property for more details.
        /// </summary>
        public static readonly DependencyProperty SelectedTabProperty = SelectedTabPropertyKey.DependencyProperty;

        /// <summary>
        /// A dependency property object backing a related property. See the related property for more details.
        /// </summary>
        public static readonly DependencyProperty SelectedTabContentProperty = SelectedTabContentPropertyKey.DependencyProperty;

        /// <summary>
        /// Get the tab currently selected. Use <c>Items.Select()</c> to select another tab.
        /// </summary>
        public TabItem CurrentTab { get => SelectedTab; }

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

        #endregion

        #region Base Functions

        /// <summary>
        /// Close a specific tab in this TabControl.
        /// </summary>
        /// <param name="tab">The tab to close.</param>
        /// <remarks>
        /// If <paramref name="tab"/> is not in this TabControl, or if the tab closing is cancelled via the TabClosing event, then nothing will happen.
        /// This does not take the <paramref name="tab"/>'s <c>CanClose</c> property into account; the tab will be closed regardless.
        /// </remarks>
        public void CloseTab(TabItem tab)
        {
            if (Items.Contains(tab)) // check if that tab is in this TabControl
            {
                if (PrepareCloseTab(tab))
                {
                    Items.Remove(tab);
                }
            }
        }

        #endregion

        #region Commands

        /// <summary>A WPF command that when executed, will close the currently open tab.</summary>
        public static readonly RoutedCommand CloseCurrentTab = new RoutedCommand();
        /// <summary>A WPF command that when executed, will close a specific tab on this TabControl.</summary>
        public static readonly RoutedCommand CloseSpecificTab = new RoutedCommand();
        /// <summary>A WPF command that when executed, will select and display a specific tab on this TabControl.</summary>
        public static readonly RoutedCommand SwitchToTab = new RoutedCommand();

        private void DoCloseCurrentTab(object sender, ExecutedRoutedEventArgs e)
        {
            var ti = SelectedTab;
            if (ti != null)
            {
                CloseTab(ti);
            }
        }

        private void DoCloseSpecificTab(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter != null)
            {
                if (e.Parameter is TabItem tab)
                {
                    CloseTab(tab);
                }
            }
        }

        private void DoSwitchToTab(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter != null)
            {
                if (e.Parameter is TabItem tab)
                {
                    if (Items.Contains(tab))
                    {
                        Items.Select(tab);
                        tab.BringIntoView();
                    }
                }
            }
        }

        private void CanExecuteAlways(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CanExecuteIfAnyTabSelected(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SelectedTab != null;
        }

        private void CanExecuteIfTabPresent(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter != null)
            {
                if (e.Parameter is TabItem tab)
                {
                    e.CanExecute = Items.Contains(tab);
                    return;
                }
            }

            e.CanExecute = false;
        }

        #endregion

        #region ShowTabsOnBottom

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly DependencyProperty ShowTabsOnBottomProperty = DependencyProperty.Register("ShowTabsOnBottom", typeof(bool), typeof(TabControl),
            new PropertyMetadata(false, new PropertyChangedCallback(OnInternalShowTabsOnBottomChanged)));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Get or set if the tab bar should be shown at the bottom of the control, rather than the top.
        /// </summary>
        [Category("Common")]
        public bool ShowTabsOnBottom
        {
            get { return (bool)GetValue(ShowTabsOnBottomProperty); }
            set { SetValue(ShowTabsOnBottomProperty, value); }
        }

        /// <summary>
        /// Internal event for handling a property changed. Please view the event that is not prefixed as "Internal".
        /// </summary>
        protected event DependencyPropertyChangedEventHandler InternalShowTabsOnBottomChanged;

        /// <summary>
        /// Raised when the ShowTabsOnBottom property is changed.
        /// </summary>
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

        #region HorizontalTabBarHeight

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly DependencyProperty HorizontalTabBarHeightProperty = DependencyProperty.Register("HorizontalTabBarHeight", typeof(double), typeof(TabControl),
            new FrameworkPropertyMetadata(24.0, new PropertyChangedCallback(OnInternalHorizontalTabBarHeightChanged)));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Get or set the height of the horizontal tab bar. The default value is 24.
        /// </summary>
        [Category("Common")]
        public double HorizontalTabBarHeight
        {
            get { return (double)GetValue(HorizontalTabBarHeightProperty); }
            set { SetValue(HorizontalTabBarHeightProperty, value); }
        }

        /// <summary>
        /// Internal event for handling a property changed. Please view the event that is not prefixed as "Internal".
        /// </summary>
        protected event DependencyPropertyChangedEventHandler InternalHorizontalTabBarHeightChanged;

        /// <summary>
        /// Raised when the HorizontalTabBarHeight property is changed.
        /// </summary>
#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? HorizontalTabBarHeightChanged;
#else
        public event DependencyPropertyChangedEventHandler HorizontalTabBarHeightChanged;
#endif

        private static void OnInternalHorizontalTabBarHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TabControl s)
            {
                s.InternalHorizontalTabBarHeightChanged?.Invoke(s, e);
            }
        }
        private void tabControl_InternalHorizontalTabBarHeightChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            HorizontalTabBarHeightChanged?.Invoke(this, e);
        }
        #endregion

        #region LeftTabBarElement and RightTabBarElement

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly DependencyProperty LeftTabBarElementProperty = DependencyProperty.Register("LeftTabBarElement", typeof(UIElement), typeof(TabControl),
            new FrameworkPropertyMetadata(null));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Get or set the element to display on the left side of the tab bar.
        /// </summary>
        public UIElement LeftTabBarElement
        {
            get { return (UIElement)GetValue(LeftTabBarElementProperty); }
            set { SetValue(LeftTabBarElementProperty, value); }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly DependencyProperty RightTabBarElementProperty = DependencyProperty.Register("RightTabBarElement", typeof(UIElement), typeof(TabControl),
            new FrameworkPropertyMetadata(null));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Get or set the element to display on the right side of the tab bar (to the left of the Tab List Menu).
        /// </summary>
        public UIElement RightTabBarElement
        {
            get { return (UIElement)GetValue(RightTabBarElementProperty); }
            set { SetValue(RightTabBarElementProperty, value); }
        }

        #endregion

        #region ShowTabListMenu / TabListMenuItemClick command

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly DependencyProperty ShowTabListMenuProperty = DependencyProperty.Register("ShowTabListMenu", typeof(bool), typeof(TabControl),
            new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnInternalShowTabListMenuChanged)));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Get or set if a tab list menu should be shown on the far-right edge of the control's tab bar, listing all the open tabs. This mimics a similar menu found in Visual Studio.
        /// </summary>
        [Category("Common")]
        public bool ShowTabListMenu
        {
            get { return (bool)GetValue(ShowTabListMenuProperty); }
            set { SetValue(ShowTabListMenuProperty, value); }
        }

        /// <summary>
        /// Internal event for handling a property changed. Please view the event that is not prefixed as "Internal".
        /// </summary>
        protected event DependencyPropertyChangedEventHandler InternalShowTabListMenuChanged;

        /// <summary>
        /// Raised when the ShowTabListMenu property is changed.
        /// </summary>
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

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly DependencyProperty TabMinWidthProperty = DependencyProperty.Register("TabMinWidth", typeof(double), typeof(TabControl),
            new FrameworkPropertyMetadata(120.0d, new PropertyChangedCallback(OnInternalTabMinWidthChanged)));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Get or set the minimum width a tab should have in the tab bar. While tabs may be wider than this width, they will never be shorter than it.
        /// </summary>
        [Category("Layout")]
        public double TabMinWidth
        {
            get { return (double)GetValue(TabMinWidthProperty); }
            set { SetValue(TabMinWidthProperty, value); }
        }

        /// <summary>
        /// Internal event for handling a property changed. Please view the event that is not prefixed as "Internal".
        /// </summary>
        protected event DependencyPropertyChangedEventHandler InternalTabMinWidthChanged;

        /// <summary>
        /// Raised when the TabMinWidth property is changed.
        /// </summary>
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

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly DependencyProperty SelectedTabClosedActionProperty = DependencyProperty.Register("SelectedTabClosedAction", typeof(SelectedTabCloseAction), typeof(TabControl),
            new PropertyMetadata(SelectedTabCloseAction.SelectTabToLeft));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Get or set the action to take when the currently selected tab is closed.
        /// </summary>
        /// <remarks>
        /// Clearing the selection (via <c>Items.ClearSelection</c> or other methods) will trigger this action as well, meaning the TabControl will attempt to select another tab if
        /// this action is not <c>SelectNothing</c>. If you need to clear the selection, you'll need to set this property to <c>SelectNothing</c> first, clear the selection, and then reset
        /// this to whatever value you had previously (by default, the value is <c>SelectTabToLeft</c>).<para/>
        /// 
        /// When a tab that isn't currently the selected tab is closed, that is not affected with this property.
        /// </remarks>
        public SelectedTabCloseAction SelectedTabClosedAction
        {
            get { return (SelectedTabCloseAction)GetValue(SelectedTabClosedActionProperty); }
            set { SetValue(SelectedTabClosedActionProperty, value); }
        }
        #endregion

        #region AllowDragDrop

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly DependencyProperty AllowTabDragDropProperty = DependencyProperty.Register("AllowTabDragDrop", typeof(bool), typeof(TabControl),
            new PropertyMetadata(true, new PropertyChangedCallback(OnAllowTabDragDropChanged)));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Get or set if tabs can be dragged and dropped.
        /// </summary>
        /// <remarks>
        /// Note that dragging and dropping tabs between TabControls is not currently supported.
        /// </remarks>
        [Category("Common")]
        public bool AllowTabDragDrop
        {
            get { return (bool)GetValue(AllowTabDragDropProperty); }
            set { SetValue(AllowTabDragDropProperty, value); }
        }

        /// <summary>
        /// Internal event for handling a property changed. Please view the event that is not prefixed as "Internal".
        /// </summary>
        protected event DependencyPropertyChangedEventHandler InternalAllowTabDragDropChanged;

        /// <summary>
        /// Raised when the AllowTabDragDrop property is changed.
        /// </summary>
#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? AllowTabDragDropChanged;
#else
        public event DependencyPropertyChangedEventHandler AllowTabDragDropChanged;
#endif

        private static void OnAllowTabDragDropChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TabControl s)
            {
                s.InternalAllowTabDragDropChanged?.Invoke(s, e);
            }
        }
        private void tabControl_InternalAllowDragDropChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            AllowTabDragDropChanged?.Invoke(this, e);
        }
        #endregion

        #region Color Scheme

        /// <summary>
        /// A dependency property object backing the related ColorScheme property. See <see cref="ColorScheme"/> for more details.
        /// </summary>
        public static readonly DependencyProperty ColorSchemeProperty
            = DependencyProperty.Register("ColorScheme", typeof(ColorScheme), typeof(TabControl),
            new FrameworkPropertyMetadata(new ColorScheme(), new PropertyChangedCallback(OnColorSchemeChanged)));

        /// <summary>
        /// Perform an action when the ColorScheme property has changed. Primarily used internally.
        /// </summary>
        /// <param name="d">The object containing the property that changed.</param>
        /// <param name="e">Event arguments about the property change.</param>
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
        /// Get or set the color scheme to apply to the control. The color scheme can quickly apply a whole visual style to your control.
        /// </summary>
        [Category("Appearance")]
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
            if (cs != ColorScheme)
            {
                ColorScheme = cs;
                return;
            }
        }
        #endregion

        #region Setup TabDisplayItem / Tdi Event Handlers

        /// <summary>
        /// Set up a new TabDisplayItem that was added to this TabControl.
        /// This will set up the necessary event handlers and other properties to allow the TabDisplayItem to interact with the TabControl.
        /// </summary>
        /// <param name="tdi">The TabDisplayItem to set up.</param>
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

        private void tdi_TabItemDrop(object sender, TabItemDropEventArgs e)
        {
            if (e.DroppedTabItem == e.SourceTabItem) return;
            if (!Items.Contains(e.DroppedTabItem)) return;

            _internalAction = true;
#if NETCOREAPP
            TabItem? selItem = null;
#else
            TabItem selItem = null;
#endif
            if (Items.SelectedItems.Count != 0)
            {
                selItem = Items.SelectedItems.First();
            }
            Items.ClearSelection();

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
                Items.Select(selItem);
            }

            // fix to make sure the correct tab has the IsSelected state
            if (ic != null)
            {
                for (int i = 0; i < ic.Items.Count; i++)
                {
                    // I really dislike this roundabout way that I have to get the child items of an ItemsControl, but I guess this is how it is
                    // from https://stackoverflow.com/a/1876534/2987285
                    ContentPresenter c = (ContentPresenter)ic.ItemContainerGenerator.ContainerFromItem(ic.Items[i]);
                    c.ApplyTemplate();
                    //#if NETCOREAPP
                    //#else
                    //                    TabDisplayItem tb = c.ContentTemplate.FindName("PART_TabItem", c) as TabDisplayItem;
                    //#endif
                    if (c.ContentTemplate.FindName("PART_TabItem", c) is TabDisplayItem tb)
                    {
                        if (tb.TabItem != null && tb.TabItem == selItem)
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

            _internalAction = false;
        }

#if NETCOREAPP
        private void tdi_Click(object? sender, EventArgs e)
#else
        private void tdi_Click(object sender, EventArgs e)
#endif
        {
            if (sender != null && sender is TabDisplayItem tdi)
            {
                if (tdi.TabItem != null && tdi.CanSelect)
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

        #endregion

        #region Scrolling

        #region ScrollButtons

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

        private static readonly DependencyPropertyKey ScrollButtonsVisiblePropertyKey = DependencyProperty.RegisterReadOnly("ScrollButtonsVisible", typeof(bool), typeof(TabControl),
            new PropertyMetadata(false));

        public static readonly DependencyProperty ScrollButtonsVisibleProperty = ScrollButtonsVisiblePropertyKey.DependencyProperty;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Get if the scroll buttons are currently visible in the tab bar.
        /// </summary>
        public bool ScrollButtonsVisible
        {
            get { return (bool)GetValue(ScrollButtonsVisibleProperty); }
            private set { SetValue(ScrollButtonsVisiblePropertyKey, value); }
        }
        #endregion

        /// <summary>
        /// A WPF command that when executed, will scroll the tab control's tab bar to the left or right.
        /// </summary>
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

    /// <summary>
    /// Event arguments for the TabClosing event in TabControl.
    /// </summary>
    public class TabItemClosingEventArgs
    {
        /// <summary>
        /// Create a TabItemClosingEventArgs.
        /// </summary>
        /// <param name="t">The tab item being closed.</param>
        public TabItemClosingEventArgs(TabItem t)
        {
            TabItem = t;
        }

        /// <summary>
        /// The TabItem being closed (and removed from the TabControl).
        /// </summary>
        public TabItem TabItem { get; private set; }

        /// <summary>
        /// Get or set if this operation should be cancelled. If <c>true</c>, this tab will not be closed and removed from the TabControl.
        /// </summary>
        public bool Cancel { get; set; } = false;
    }

    /// <summary>
    /// Event arguments for the TabChanged event in TabControl.
    /// </summary>
    public class TabItemChangeEventArgs
    {
        /// <summary>
        /// Create a TabItemChangeEventArgs.
        /// </summary>
        /// <param name="t">The TabItem being changed to.</param>
        public TabItemChangeEventArgs(TabItem t)
        {
            TabItem = t;
        }

        /// <summary>
        /// The TabItem being changed to. This is the new TabItem being displayed.
        /// </summary>
        public TabItem TabItem { get; private set; }
    }

    /// <summary>
    /// References the action to take when the currently-selected tab is closed.
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

    //public enum TabBarDisplay
    //{
    //    /// <summary>
    //    /// Do not display the tab bar at all.
    //    /// </summary>
    //    Hidden = 0,
    //    /// <summary>
    //    /// Display the tab bar horizontally at the top of the control.
    //    /// </summary>
    //    Top = 1,
    //    /// <summary>
    //    /// Display the tab bar horizontally at the bottom of the control.
    //    /// </summary>
    //    Bottom = 2,
    //    /// <summary>
    //    /// Display the tab bar vertically at the left of the control.
    //    /// </summary>
    //    Left = 3,
    //    /// <summary>
    //    /// Display the tab bar vertically at the right of the control.
    //    /// </summary>
    //    Right = 4,
    //}
}
