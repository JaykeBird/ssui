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
using System.Windows.Media;

namespace SolidShineUi
{
    /// <summary>
    /// A control that can house multiple controls under a number of tabs. Each tab has a title, icon, and close button (see <see cref="TabItem"/>).
    /// </summary>
    [ContentProperty("Items")]
    [DefaultEvent(nameof(TabChanged))]
    [Localizability(LocalizationCategory.None)]
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

            CommandBindings.Add(new CommandBinding(TabBarScrollCommand, OnScrollCommand, (s, e) => { e.CanExecute = ScrollButtonsVisible; }));

            CommandBindings.Add(new CommandBinding(CloseCurrentTab, DoCloseCurrentTab, CanExecuteIfAnyTabSelected));
            CommandBindings.Add(new CommandBinding(CloseSpecificTab, DoCloseSpecificTab, CanExecuteIfTabPresent));
            CommandBindings.Add(new CommandBinding(SwitchToTab, DoSwitchToTab, CanExecuteIfTabPresent));

            // add support for WPF generic commands
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, DoCloseCurrentTab, CanExecuteIfAnyTabSelected));
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

        #region Template IO
        /// <inheritdoc/>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            LoadTemplateItems();
        }

        bool itemsLoaded = false;

        bool _internalAction = false;

#if NETCOREAPP
        ItemsControl? ic = null;
        ScrollViewer? sv = null;

        MenuButton? tlm = null;
        FlatButton? bsl = null;
        FlatButton? bsr = null;
#else
        ItemsControl ic = null;
        ScrollViewer sv = null;

        MenuButton tlm = null;
        FlatButton bsl = null;
        FlatButton bsr = null;
#endif

        void LoadTemplateItems()
        {
            if (!itemsLoaded)
            {
                ic = (ItemsControl)GetTemplateChild("PART_TabBar");
                sv = (ScrollViewer)GetTemplateChild("PART_TabScroll");
                tlm = (MenuButton)GetTemplateChild("PART_TabMenu");
                bsl = (FlatButton)GetTemplateChild("btnScrollLeft");
                bsr = (FlatButton)GetTemplateChild("btnScrollRight");

                if (ic != null && sv != null)
                {
                    sv.ScrollChanged += sv_ScrollChanged;
                    ic.SizeChanged += control_SizeChanged;
                    itemsLoaded = true;
                }

                if (tlm != null)
                {
                    tlm.HighlightBrush = ButtonHighlightBackground;
                    tlm.BorderHighlightBrush = ButtonHighlightBorderBrush;
                    tlm.ClickBrush = ButtonClickBrush;
                }

                if (bsl != null)
                {
                    bsl.HighlightBrush = ButtonHighlightBackground;
                    bsl.BorderHighlightBrush = ButtonHighlightBorderBrush;
                    bsl.ClickBrush = ButtonClickBrush;
                }

                if (bsr != null)
                {
                    bsr.HighlightBrush = ButtonHighlightBackground;
                    bsr.BorderHighlightBrush = ButtonHighlightBorderBrush;
                    bsr.ClickBrush = ButtonClickBrush;
                }
            }
        }
        #endregion

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

            //LoadTemplateItems();
            CheckScrolling();
        }

        private void items_SelectionChanged(object sender, CollectionSelectionChangedEventArgs e)
        {
            //LoadTemplateItems();
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

        private void Items_ItemRemoving(object sender, CancelableItemEventArgs<TabItem> e)
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
        /// A dependency property object backing the <see cref="SelectedTab"/> property. See the related property for more details.
        /// </summary>
        public static readonly DependencyProperty SelectedTabProperty = SelectedTabPropertyKey!.DependencyProperty;

        /// <summary>
        /// A dependency property object backing the <see cref="SelectedTabContent"/> property. See the related property for more details.
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
        /// A dependency property object backing the <see cref="SelectedTab"/> property. See the related property for more details.
        /// </summary>
        public static readonly DependencyProperty SelectedTabProperty = SelectedTabPropertyKey.DependencyProperty;

        /// <summary>
        /// A dependency property object backing the <see cref="SelectedTabContent"/> property. See the related property for more details.
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
        /// Close a specific tab in this TabControl. This will close the tab even if <c>CanClose</c> is false.
        /// This triggers the <see cref="TabClosing"/> event, which can cancel closing this tab.
        /// </summary>
        /// <param name="tab">The tab to close.</param>
        /// <remarks>
        /// If <paramref name="tab"/> is not in this TabControl, or if the tab closing is cancelled via the TabClosing event, then nothing will happen.
        /// <para/>
        /// This does not take the <paramref name="tab"/>'s <see cref="TabItem.CanClose"/> property into account; if you want to ensure you don't close a tab where <c>CanClose</c> is false,
        /// make sure you check that value before running this function. This can be used to circumvent <c>CanClose</c>, in situations where you need to close a tab
        /// which an end user would normally not be able to do.
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
        public static readonly RoutedCommand CloseCurrentTab = new RoutedCommand("CloseCurrentTab", typeof(TabControl));
        /// <summary>A WPF command that when executed, will close a specific tab on this TabControl.</summary>
        public static readonly RoutedCommand CloseSpecificTab = new RoutedCommand("CloseSpecificTab", typeof(TabControl));
        /// <summary>A WPF command that when executed, will select and display a specific tab on this TabControl.</summary>
        public static readonly RoutedCommand SwitchToTab = new RoutedCommand("SwitchToTab", typeof(TabControl));

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

        /// <summary>
        /// The command in question is always able to be executed, regardless of the state of the object.
        /// </summary>
        private void CanExecuteAlways(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        /// <summary>
        /// The command in question is able to execute, if at least one tab is selected.
        /// </summary>
        private void CanExecuteIfAnyTabSelected(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SelectedTab != null;
        }

        /// <summary>
        /// The command in question is able to execute, if the tab specified in the command parameter is contained in this control.
        /// </summary>
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

        #region Properties

        #region ShowTabsOnBottom

        /// <summary>
        /// The dependency property object for the <see cref="ShowTabsOnBottom"/> property. See the related property for details.
        /// </summary>
        public static readonly DependencyProperty ShowTabsOnBottomProperty = DependencyProperty.Register("ShowTabsOnBottom", typeof(bool), typeof(TabControl),
            new PropertyMetadata(false, new PropertyChangedCallback((d, e) => d.PerformAs<TabControl>((t) => t.ShowTabsOnBottomChanged?.Invoke(t, e)))));

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
        /// Raised when the ShowTabsOnBottom property is changed.
        /// </summary>
#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? ShowTabsOnBottomChanged;
#else
        public event DependencyPropertyChangedEventHandler ShowTabsOnBottomChanged;
#endif
        #endregion

        #region HorizontalTabBarHeight

        /// <summary>
        /// The dependency property object for the <see cref="HorizontalTabBarHeight"/> property. See the related property for details.
        /// </summary>
        public static readonly DependencyProperty HorizontalTabBarHeightProperty = DependencyProperty.Register("HorizontalTabBarHeight", typeof(double), typeof(TabControl),
            new FrameworkPropertyMetadata(24.0, new PropertyChangedCallback((d, e) => d.PerformAs<TabControl>((t) => t.HorizontalTabBarHeightChanged?.Invoke(t, e)))));

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
        /// Raised when the HorizontalTabBarHeight property is changed.
        /// </summary>
#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? HorizontalTabBarHeightChanged;
#else
        public event DependencyPropertyChangedEventHandler HorizontalTabBarHeightChanged;
#endif
        #endregion

        #region LeftTabBarElement and RightTabBarElement

        /// <summary>The backing dependency property object for the related property. See <see cref="LeftTabBarElement"/> for details.</summary>
        public static readonly DependencyProperty LeftTabBarElementProperty = DependencyProperty.Register("LeftTabBarElement", typeof(UIElement), typeof(TabControl),
            new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Get or set the element to display on the left side of the tab bar.
        /// </summary>
        public UIElement LeftTabBarElement
        {
            get { return (UIElement)GetValue(LeftTabBarElementProperty); }
            set { SetValue(LeftTabBarElementProperty, value); }
        }

        /// <summary>The backing dependency property object for the related property. See <see cref="RightTabBarElement"/> for details.</summary>
        public static readonly DependencyProperty RightTabBarElementProperty = DependencyProperty.Register("RightTabBarElement", typeof(UIElement), typeof(TabControl),
            new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Get or set the element to display on the right side of the tab bar (to the left of the Tab List Menu).
        /// </summary>
        public UIElement RightTabBarElement
        {
            get { return (UIElement)GetValue(RightTabBarElementProperty); }
            set { SetValue(RightTabBarElementProperty, value); }
        }

        #endregion

        #region ShowTabListMenu

        /// <summary>
        /// The dependency property object for the <see cref="ShowTabListMenu"/> property. See the related property for details.
        /// </summary>
        public static readonly DependencyProperty ShowTabListMenuProperty = DependencyProperty.Register("ShowTabListMenu", typeof(bool), typeof(TabControl),
            new FrameworkPropertyMetadata(true, new PropertyChangedCallback((d, e) => d.PerformAs<TabControl>((t) => t.ShowTabListMenuChanged?.Invoke(t, e)))));

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
        /// Raised when the ShowTabListMenu property is changed.
        /// </summary>
#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? ShowTabListMenuChanged;
#else
        public event DependencyPropertyChangedEventHandler ShowTabListMenuChanged;
#endif

        #endregion

        #region TabMinWidth

        /// <summary>
        /// The dependency property object for the <see cref="TabMinWidth"/> property. See the related property for details.
        /// </summary>
        public static readonly DependencyProperty TabMinWidthProperty = DependencyProperty.Register("TabMinWidth", typeof(double), typeof(TabControl),
            new FrameworkPropertyMetadata(120.0d, new PropertyChangedCallback((d, e) => d.PerformAs<TabControl>((t) => t.OnTabMinWidthChanged(e)))));

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
        /// Raised when the TabMinWidth property is changed.
        /// </summary>
#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? TabMinWidthChanged;
#else
        public event DependencyPropertyChangedEventHandler TabMinWidthChanged;
#endif

        private void OnTabMinWidthChanged(DependencyPropertyChangedEventArgs e)
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

        /// <summary>
        /// The dependency property object for the <see cref="SelectedTabClosedAction"/> property. See the related property for details.
        /// </summary>
        public static readonly DependencyProperty SelectedTabClosedActionProperty = DependencyProperty.Register("SelectedTabClosedAction", typeof(SelectedTabCloseAction), typeof(TabControl),
            new PropertyMetadata(SelectedTabCloseAction.SelectTabToLeft));

        /// <summary>
        /// Get or set the action to take when the currently selected tab is closed.
        /// </summary>
        /// <remarks>
        /// When the currently selected tab is closed, or the selection is cleared via methods like <c>Items.ClearSelection</c>, the TabControl will attempt to select another tab based
        /// upon what action is set here. If this is set to <c>SelectNothing</c>, then that means no tab will be selected and the content area will be blank.
        /// <para/>
        /// If you need to deselect all tabs and display nothing, you'll need to set this property to <c>SelectNothing</c> first, clear the selection, and then reset
        /// this to whatever value you had previously (by default, the value is <c>SelectTabToLeft</c>).<para/>
        /// 
        /// This property is only for situations affecting a change in the selected tab. If a different (not selected) tab is closed, no extra action is needed
        /// and this property has no effect or relation to that.
        /// </remarks>
        public SelectedTabCloseAction SelectedTabClosedAction
        {
            get { return (SelectedTabCloseAction)GetValue(SelectedTabClosedActionProperty); }
            set { SetValue(SelectedTabClosedActionProperty, value); }
        }
        #endregion

        #region AllowDragDrop

        /// <summary>
        /// The dependency property object for the <see cref="AllowTabDragDrop"/> property. See the related property for details.
        /// </summary>
        public static readonly DependencyProperty AllowTabDragDropProperty = DependencyProperty.Register("AllowTabDragDrop", typeof(bool), typeof(TabControl),
            new PropertyMetadata(true, new PropertyChangedCallback((d, e) => d.PerformAs<TabControl>((t) => t.AllowTabDragDropChanged?.Invoke(t, e)))));

        /// <summary>
        /// Get or set if tabs can be dragged and dropped. If <c>true</c>, users can drag tabs around to rearrange them in the control's tab bar.
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
        /// Raised when the AllowTabDragDrop property is changed.
        /// </summary>
#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? AllowTabDragDropChanged;
#else
        public event DependencyPropertyChangedEventHandler AllowTabDragDropChanged;
#endif

        #endregion
        
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
            = DependencyProperty.Register("ColorScheme", typeof(ColorScheme), typeof(TabControl),
            new FrameworkPropertyMetadata(new ColorScheme(), new PropertyChangedCallback(OnColorSchemeChanged)));

        /// <summary>
        /// Perform an action when the ColorScheme property has changed. Primarily used internally.
        /// </summary>
        /// <param name="d">The object containing the property that changed.</param>
        /// <param name="e">Event arguments about the property change.</param>
        public static void OnColorSchemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is ColorScheme cs)
            {
                if (d is TabControl t)
                {
                    t.ColorSchemeChanged?.Invoke(t, e);
                    t.ApplyColorScheme(cs);
                }
            }
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

            BorderBrush = cs.BorderColor.ToBrush();
            Foreground = cs.ForegroundColor.ToBrush();
            ContentAreaBackground = cs.BackgroundColor.ToBrush();
            // Background = cs.BackgroundColor.ToBrush();

            //Debug.WriteLine(cs.ThirdHighlightColor.ToString());
            //Debug.WriteLine(cs.SecondHighlightColor.ToString());
            //Debug.WriteLine(cs.HighlightColor.ToString());
            //Debug.WriteLine("========================");

            ButtonClickBrush = cs.ThirdHighlightColor.ToBrush();

            if (cs.IsHighContrast)
            {
                TabBackground = cs.BackgroundColor.ToBrush();
                TabBorderBrush = cs.BorderColor.ToBrush();
                TabHighlightBrush = cs.HighlightColor.ToBrush();
                TabBorderHighlightBrush = cs.BorderColor.ToBrush();
                SelectedTabBackground = cs.BackgroundColor.ToBrush();
                TabCloseBrush = cs.BorderColor.ToBrush();

                ButtonHighlightBackground = cs.HighlightColor.ToBrush();
                ButtonHighlightBorderBrush = cs.BorderColor.ToBrush();
            }
            else
            {
                TabBackground = cs.ThirdHighlightColor.ToBrush();
                TabBorderBrush = cs.BorderColor.ToBrush();
                TabHighlightBrush = cs.SecondHighlightColor.ToBrush();
                TabBorderHighlightBrush = cs.HighlightColor.ToBrush();
                SelectedTabBackground = cs.BackgroundColor.ToBrush();
                TabCloseBrush = cs.ForegroundColor.ToBrush();

                ButtonHighlightBackground = cs.SecondHighlightColor.ToBrush();
                ButtonHighlightBorderBrush = cs.HighlightColor.ToBrush();
            }
        }
        #endregion

        #region Brushes

        /// <summary>
        /// Get or set the background used for the content area of the TabControl.
        /// </summary>
        [Category("Brushes")]
        public Brush ContentAreaBackground { get => (Brush)GetValue(ContentAreaBackgroundProperty); set => SetValue(ContentAreaBackgroundProperty, value); }

        /// <summary>The backing dependency property for <see cref="ContentAreaBackground"/>. See the related property for details.</summary>
        public static DependencyProperty ContentAreaBackgroundProperty
            = DependencyProperty.Register(nameof(ContentAreaBackground), typeof(Brush), typeof(TabControl),
            new FrameworkPropertyMetadata(Colors.White.ToBrush(), FrameworkPropertyMetadataOptions.AffectsRender, OnContentAreaBackgroundUpdate));

        /// <summary>
        /// Get or set the brush used for the background of a tab while it is highlighted (i.e. mouse over, keyboard focus).
        /// </summary>
        [Category("Brushes")]
        public Brush TabHighlightBrush { get => (Brush)GetValue(TabHighlightBrushProperty); set => SetValue(TabHighlightBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="TabHighlightBrush"/>. See the related property for details.</summary>
        public static DependencyProperty TabHighlightBrushProperty
            = DependencyProperty.Register(nameof(TabHighlightBrush), typeof(Brush), typeof(TabControl),
            new FrameworkPropertyMetadata(Colors.Gainsboro.ToBrush(), FrameworkPropertyMetadataOptions.AffectsRender, UpdateChildBrushes));

        /// <summary>
        /// Get or set the brush used for the borders of a tab while it is highlighted (i.e. mouse over, keyboard focus).
        /// </summary>
        [Category("Brushes")]
        public Brush TabBorderHighlightBrush { get => (Brush)GetValue(TabBorderHighlightBrushProperty); set => SetValue(TabBorderHighlightBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="TabBorderHighlightBrush"/>. See the related property for details.</summary>
        public static DependencyProperty TabBorderHighlightBrushProperty
            = DependencyProperty.Register(nameof(TabBorderHighlightBrush), typeof(Brush), typeof(TabControl),
            new FrameworkPropertyMetadata(Colors.DimGray.ToBrush(), FrameworkPropertyMetadataOptions.AffectsRender, UpdateChildBrushes));

        /// <summary>
        /// Get or set the brush used for the borders of tabs. This is different from the <see cref="Control.BorderBrush"/> used for the rest of the TabControl.
        /// </summary>
        [Category("Brushes")]
        public Brush TabBorderBrush { get => (Brush)GetValue(TabBorderBrushProperty); set => SetValue(TabBorderBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="TabBorderBrush"/>. See the related property for details.</summary>
        public static DependencyProperty TabBorderBrushProperty
            = DependencyProperty.Register(nameof(TabBorderBrush), typeof(Brush), typeof(TabControl),
            new FrameworkPropertyMetadata(Colors.Black.ToBrush(), FrameworkPropertyMetadataOptions.AffectsRender, UpdateChildBrushes));

        /// <summary>
        /// Get or set the brush used for the close glyph used in the tabs (where <see cref="TabItem.CanClose"/> is set to <c>true</c>).
        /// </summary>
        [Category("Brushes")]
        public Brush TabCloseBrush { get => (Brush)GetValue(TabCloseBrushProperty); set => SetValue(TabCloseBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="TabCloseBrush"/>. See the related property for details.</summary>
        public static DependencyProperty TabCloseBrushProperty
            = DependencyProperty.Register(nameof(TabCloseBrush), typeof(Brush), typeof(TabControl),
            new FrameworkPropertyMetadata(Colors.Black.ToBrush(), FrameworkPropertyMetadataOptions.AffectsRender, UpdateChildBrushes));

        /// <summary>
        /// Get or set the brush used for the background of a tab. Individual tabs can overwrite their backgrounds by changing <see cref="TabItem.TabBackground"/>.
        /// </summary>
        [Category("Brushes")]
        public Brush TabBackground { get => (Brush)GetValue(TabBackgroundProperty); set => SetValue(TabBackgroundProperty, value); }

        /// <summary>The backing dependency property for <see cref="TabBackground"/>. See the related property for details.</summary>
        public static DependencyProperty TabBackgroundProperty
            = DependencyProperty.Register(nameof(TabBackground), typeof(Brush), typeof(TabControl),
            new FrameworkPropertyMetadata(Colors.LightGray.ToBrush(), FrameworkPropertyMetadataOptions.AffectsRender, UpdateChildBrushes));

        /// <summary>
        /// Get or set the brush used for the background of a selected tab.
        /// </summary>
        [Category("Brushes")]
        public Brush SelectedTabBackground { get => (Brush)GetValue(SelectedTabBackgroundProperty); set => SetValue(SelectedTabBackgroundProperty, value); }

        /// <summary>The backing dependency property for <see cref="SelectedTabBackground"/>. See the related property for details.</summary>
        public static DependencyProperty SelectedTabBackgroundProperty
            = DependencyProperty.Register(nameof(SelectedTabBackground), typeof(Brush), typeof(TabControl),
            new FrameworkPropertyMetadata(Colors.White.ToBrush(), FrameworkPropertyMetadataOptions.AffectsRender, UpdateChildBrushes));

        /// <summary>
        /// Get or set the brush used for buttons in the TabControl, when they are highlighted (i.e. mouse over).
        /// </summary>
        [Category("Brushes")]
        public Brush ButtonHighlightBackground { get => (Brush)GetValue(ButtonHighlightBackgroundProperty); set => SetValue(ButtonHighlightBackgroundProperty, value); }

        /// <summary>The backing dependency property for <see cref="ButtonHighlightBackground"/>. See the related property for details.</summary>
        public static DependencyProperty ButtonHighlightBackgroundProperty
            = DependencyProperty.Register(nameof(ButtonHighlightBackground), typeof(Brush), typeof(TabControl),
            new FrameworkPropertyMetadata(Colors.Silver.ToBrush(), FrameworkPropertyMetadataOptions.AffectsRender, OnHighlightBrushUpdate));

        /// <summary>
        /// Get or set the brush used for the borders of buttons in the TabControl, when they are highlighted (i.e. mouse over).
        /// </summary>
        [Category("Brushes")]
        public Brush ButtonHighlightBorderBrush { get => (Brush)GetValue(ButtonHighlightBorderBrushProperty); set => SetValue(ButtonHighlightBorderBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="ButtonHighlightBorderBrush"/>. See the related property for details.</summary>
        public static DependencyProperty ButtonHighlightBorderBrushProperty
            = DependencyProperty.Register(nameof(ButtonHighlightBorderBrush), typeof(Brush), typeof(TabControl),
            new FrameworkPropertyMetadata(Colors.DimGray.ToBrush(), FrameworkPropertyMetadataOptions.AffectsRender, OnHighlightBorderBrushUpdate));

        /// <summary>
        /// Get or set the brush used for buttons in the TabControl, when they are being clicked (i.e. mouse down, key down).
        /// </summary>
        [Category("Brushes")]
        public Brush ButtonClickBrush { get => (Brush)GetValue(ButtonClickBrushProperty); set => SetValue(ButtonClickBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="ButtonClickBrush"/>. See the related property for details.</summary>
        public static DependencyProperty ButtonClickBrushProperty
            = DependencyProperty.Register(nameof(ButtonClickBrush), typeof(Brush), typeof(TabControl),
            new FrameworkPropertyMetadata(Colors.LightGray.ToBrush(), FrameworkPropertyMetadataOptions.AffectsRender, OnClickBrushUpdate));

        private static void UpdateChildBrushes(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TabControl tc)
            {
                tc.InternalUpdateChildBrushes(e);
            }
        }

        /// <inheritdoc/>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == BorderBrushProperty)
            {
                if (tlm != null)
                {
                    if (tlm.Menu != null)
                    {
                        // tlm.Menu.Background = ContentAreaBackground;
                        tlm.Menu.BorderBrush = BorderBrush;
                    }
                }

                InternalUpdateChildBrushes(e);
            }
            else if (e.Property == BackgroundProperty)
            {
                InternalUpdateChildBrushes(e);
            }
            else if (e.Property == ForegroundProperty)
            {
                InternalUpdateChildBrushes(e);
            }
        }

        /// <summary>
        /// Called when a brush property is updated in the TabControl.
        /// </summary>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> related to the property change.</param>
        protected virtual void InternalUpdateChildBrushes(DependencyPropertyChangedEventArgs e)
        {

        }

        private static void OnContentAreaBackgroundUpdate(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TabControl tc)
            {
                if (tc.tlm != null)
                {
                    if (tc.tlm.Menu != null)
                    {
                        tc.tlm.Menu.MenuBackground = tc.ContentAreaBackground;
                        // tc.tlm.Menu.BorderBrush = BorderBrush;
                    }
                }

                tc.InternalUpdateChildBrushes(e);
            }
        }

        private static void OnClickBrushUpdate(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TabControl tc && e.NewValue is Brush b && tc.tlm != null)
            {
                if (tc.tlm != null) tc.tlm.ClickBrush = b;
                if (tc.bsr != null) tc.bsr.ClickBrush = b;
                if (tc.bsl != null) tc.bsl.ClickBrush = b;
            }

            UpdateChildBrushes(d, e);
        }

        private static void OnHighlightBrushUpdate(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TabControl tc && e.NewValue is Brush b)
            {
                if (tc.tlm != null) tc.tlm.HighlightBrush = b;
                if (tc.bsr != null) tc.bsr.HighlightBrush = b;
                if (tc.bsl != null) tc.bsl.HighlightBrush = b;
            }

            UpdateChildBrushes(d, e);
        }

        private static void OnHighlightBorderBrushUpdate(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TabControl tc && e.NewValue is Brush b)
            {
                if (tc.tlm != null) tc.tlm.BorderHighlightBrush = b;
                if (tc.bsr != null) tc.bsr.BorderHighlightBrush = b;
                if (tc.bsl != null) tc.bsl.BorderHighlightBrush = b;
            }

            UpdateChildBrushes(d, e);
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

        private static readonly DependencyPropertyKey ScrollButtonsVisiblePropertyKey = DependencyProperty.RegisterReadOnly("ScrollButtonsVisible", typeof(bool), typeof(TabControl),
            new PropertyMetadata(false));

        /// <summary>
        /// The read-only dependency property for the <see cref="ScrollButtonsVisible"/> property. See that related property for more details.
        /// </summary>
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
    public class TabItemClosingEventArgs : CancelEventArgs
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
    }

    /// <summary>
    /// Event arguments for the TabChanged event in TabControl.
    /// </summary>
    public class TabItemChangeEventArgs : EventArgs
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
    /// A delegate to be used with events regarding the selected <see cref="TabItem"/> changing in a <see cref="TabControl"/>, such as <see cref="TabControl.TabChanged"/>.
    /// </summary>
    /// <param name="sender">The object where the event was raised.</param>
    /// <param name="e">The event arguments associated with this event.</param>
    public delegate void TabItemChangeEventHandler(object sender, TabItemChangeEventArgs e);

    /// <summary>
    /// A delegate to be used with the <see cref="TabControl.TabClosing"/> event, which is raised immediately before a tab is closed (to provide an ability to cancel closing the tab).
    /// </summary>
    /// <param name="sender">The object where the event was raised.</param>
    /// <param name="e">The event arguments associated with this event.</param>
    public delegate void TabItemClosingEventHandler(object sender, TabItemClosingEventArgs e);


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

}
