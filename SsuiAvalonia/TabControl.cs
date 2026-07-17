using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media.TextFormatting;
using Avalonia.Metadata;

namespace SolidShineUi
{
    /// <summary>
    /// A control that can contain multiple tabs, each with its own content.
    /// </summary>
    public class TabControl : TemplatedControl
    {
        /// <summary>
        /// Create a TabControl.
        /// </summary>
        public TabControl()
        {
            SetValue(ItemsProperty, new SelectableCollection<TabItem>());

            Loaded += TabControl_Loaded;

            Items.CollectionChanged += Items_CollectionChanged;
            Items.SelectionChanged += Items_SelectionChanged;
        }


        /// <summary>
        /// Get or set if the first tab should be selected right away when the control is loaded. This property has no effect after the control is loaded.
        /// </summary>
        public bool SelectFirstTabOnLoad { get; set; } = true;

        private void TabControl_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (SelectFirstTabOnLoad)
            {

            }
        }

        #region SelectableCollection handling

        #region ItemsProperty

        private SelectableCollection<TabItem> _items = new SelectableCollection<TabItem>();

        /// <summary>
        /// Get or set the list of items in this SelectPanel. This Items property can be used to add items, remove items, and also select items via the Select method.
        /// </summary>
        [Content]
        public SelectableCollection<TabItem> Items
        {
            get => _items;
            set
            {
                // have to do this weird roundabout way because Avalonia's system doesn't like having a content property that can't be set
                // however, for item controls like this, we don't want others to be able to set this, so instead I mask the changes
                // and simply just add the relevant items into the actual Items structure instead
                SelectableCollection<TabItem> newItems = new SelectableCollection<TabItem>(_items);
                foreach (TabItem item in value)
                {
                    _items.Add(item);
                }
                SetAndRaise(ItemsProperty, ref newItems, value);

            }
        }

        /// <summary>The backing direct property for <see cref="Items"/>. See the related property for details.</summary>
        public static readonly DirectProperty<TabControl, SelectableCollection<TabItem>> ItemsProperty
            = AvaloniaProperty.RegisterDirect<TabControl, SelectableCollection<TabItem>>(nameof(Items), (s) => s.Items, (s, e) => s.Items = e,
                unsetValue: new SelectableCollection<TabItem>());

        #endregion

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

            // CheckScrolling();
        }

        private void Items_SelectionChanged(object sender, CollectionSelectionChangedEventArgs e)
        {
            // find a way to access the visual items to be added into the TabControl's template,
            // so that I can mark the right TabDisplayItem as selected and the others as not selected
        }

        #endregion

        #region Internal Base Functions

        int closedTabIndex = -1;

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

        void SetupCurrentTab(TabItem? tab)
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

        private TabItem? _selectedTab = null;

        /// <summary>
        /// Get the tab currently selected. Use <c>Items.Select()</c> to select another tab.
        /// </summary>
        [Category("Common")]
        public TabItem? SelectedTab { get => _selectedTab; private set => SetAndRaise(SelectedTabProperty, ref _selectedTab, value); }

        /// <summary>The backing direct property for <see cref="SelectedTab"/>. See the related property for details.</summary>
        public static readonly DirectProperty<TabControl, TabItem?> SelectedTabProperty
            = AvaloniaProperty.RegisterDirect<TabControl, TabItem?>(nameof(SelectedTab), (s) => s.SelectedTab, unsetValue: null);

        private Control? _selectedTabContent = null;

        /// <summary>
        /// Get the currently shown content in the main part of the control. This is the content of the currently selected tab.
        /// </summary>
        public Control? SelectedTabContent { get => _selectedTabContent; private set => SetAndRaise(SelectedTabContentProperty, ref _selectedTabContent, value); }

        /// <summary>The backing direct property for <see cref="SelectedTabContent"/>. See the related property for details.</summary>
        public static readonly DirectProperty<TabControl, Control?> SelectedTabContentProperty
            = AvaloniaProperty.RegisterDirect<TabControl, Control?>(nameof(SelectedTabContent), (s) => s.SelectedTabContent, unsetValue: null);

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

        #region Properties

        /// <summary>
        /// Get or set if the tab bar should be shown at the bottom of the control, rather than the top.
        /// </summary>
        public bool ShowTabsOnBottom { get => GetValue(ShowTabsOnBottomProperty); set => SetValue(ShowTabsOnBottomProperty, value); }

        /// <summary>The backing styled property for <see cref="ShowTabsOnBottom"/>. See the related property for details.</summary>
        public static readonly StyledProperty<bool> ShowTabsOnBottomProperty
            = AvaloniaProperty.Register<TabControl, bool>(nameof(ShowTabsOnBottom), false);


        /// <summary>
        /// Get or set the height of the horizontal tab bar. The default value is 24.
        /// </summary>
        public double HorizontalTabBarHeight { get => GetValue(HorizontalTabBarHeightProperty); set => SetValue(HorizontalTabBarHeightProperty, value); }

        /// <summary>The backing styled property for <see cref="HorizontalTabBarHeight"/>. See the related property for details.</summary>
        public static readonly StyledProperty<double> HorizontalTabBarHeightProperty
            = AvaloniaProperty.Register<TabControl, double>(nameof(HorizontalTabBarHeight), 24.0);


        /// <summary>
        /// Get or set the element to display on the left side of the tab bar.
        /// </summary>
        public Control? LeftTabBarElement { get => GetValue(LeftTabBarElementProperty); set => SetValue(LeftTabBarElementProperty, value); }

        /// <summary>The backing styled property for <see cref="LeftTabBarElement"/>. See the related property for details.</summary>
        public static readonly StyledProperty<Control?> LeftTabBarElementProperty
            = AvaloniaProperty.Register<TabControl, Control?>(nameof(LeftTabBarElement), null);


        /// <summary>
        /// Get or set the element to display on the right side of the tab bar (to the left of the Tab List Menu).
        /// </summary>
        public Control? RightTabBarElement { get => GetValue(RightTabBarElementProperty); set => SetValue(RightTabBarElementProperty, value); }

        /// <summary>The backing styled property for <see cref="RightTabBarElement"/>. See the related property for details.</summary>
        public static readonly StyledProperty<Control?> RightTabBarElementProperty
            = AvaloniaProperty.Register<TabControl, Control?>(nameof(RightTabBarElement), null);


        /// <summary>
        /// Get or set if a tab list menu should be shown on the far-right edge of the control's tab bar, listing all the open tabs. 
        /// This mimics a similar menu found in Visual Studio.
        /// </summary>
        public bool ShowTabListMenu { get => GetValue(ShowTabListMenuProperty); set => SetValue(ShowTabListMenuProperty, value); }

        /// <summary>The backing styled property for <see cref="ShowTabListMenu"/>. See the related property for details.</summary>
        public static readonly StyledProperty<bool> ShowTabListMenuProperty
            = AvaloniaProperty.Register<TabControl, bool>(nameof(ShowTabListMenu), true);


        /// <summary>
        /// Get or set the minimum width a tab should have in the tab bar. While tabs may be wider than this width, they will never be shorter than it.
        /// </summary>
        public double TabMinWidth { get => GetValue(TabMinWidthProperty); set => SetValue(TabMinWidthProperty, value); }

        /// <summary>The backing styled property for <see cref="TabMinWidth"/>. See the related property for details.</summary>
        public static readonly StyledProperty<double> TabMinWidthProperty
            = AvaloniaProperty.Register<TabControl, double>(nameof(TabMinWidth), 120.0d);


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
        public SelectedTabCloseAction SelectedTabClosedAction { get => GetValue(SelectedTabClosedActionProperty); set => SetValue(SelectedTabClosedActionProperty, value); }

        /// <summary>The backing styled property for <see cref="SelectedTabClosedAction"/>. See the related property for details.</summary>
        public static readonly StyledProperty<SelectedTabCloseAction> SelectedTabClosedActionProperty
            = AvaloniaProperty.Register<TabControl, SelectedTabCloseAction>(nameof(SelectedTabClosedAction), SelectedTabCloseAction.SelectTabToLeft);


        /// <summary>
        /// Get or set if tabs can be dragged and dropped. If <c>true</c>, users can drag tabs around to rearrange them in the control's tab bar.
        /// </summary>
        /// <remarks>
        /// Note that dragging and dropping tabs between TabControls is not currently supported.
        /// </remarks>
        public bool AllowTabDragDrop { get => GetValue(AllowTabDragDropProperty); set => SetValue(AllowTabDragDropProperty, value); }

        /// <summary>The backing styled property for <see cref="AllowTabDragDrop"/>. See the related property for details.</summary>
        public static readonly StyledProperty<bool> AllowTabDragDropProperty
            = AvaloniaProperty.Register<TabControl, bool>(nameof(AllowTabDragDrop), true);

        #endregion

    }
}
