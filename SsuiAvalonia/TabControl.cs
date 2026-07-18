using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Metadata;
using SolidShineUi.Utils;

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
            SizeChanged += control_SizeChanged;

            Items.CollectionChanged += Items_CollectionChanged;
            Items.SelectionChanged += Items_SelectionChanged;
        }

        bool _internalAction = false;

        /// <summary>
        /// Get or set if the first tab should be selected right away when the control is loaded. This property has no effect after the control is loaded.
        /// </summary>
        public bool SelectFirstTabOnLoad { get; set; } = true;

        private void TabControl_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (SelectFirstTabOnLoad && Items.Count > 0)
            {
                Items.Select(Items[0]);
            }
        }

        #region Template IO

        /// <inheritdoc/>
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            // get items
            LoadTemplateItems(e);
        }

        ItemsControl? ic = null;
        ScrollViewer? sv = null;

        MenuButton? btnTabList = null;
        FlatButton? btnScrollLeft = null;
        FlatButton? btnScrollRight = null;

        bool itemsLoaded = false;

        void LoadTemplateItems(TemplateAppliedEventArgs e)
        {
            if (!itemsLoaded)
            {
                ic = e.NameScope.Find<ItemsControl>("PART_TabBar");
                sv = e.NameScope.Find<ScrollViewer>("PART_TabScroll");
                btnTabList = e.NameScope.Find<MenuButton>("PART_TabMenu");
                btnScrollLeft = e.NameScope.Find<FlatButton>("btnScrollLeft");
                btnScrollRight = e.NameScope.Find<FlatButton>("btnScrollRight");

                if (ic != null && sv != null)
                {
                    sv.ScrollChanged += sv_ScrollChanged;
                    ic.SizeChanged += control_SizeChanged;
                    itemsLoaded = true;
                }

                if (btnTabList != null)
                {
                    btnTabList.HighlightBrush = ButtonHighlightBackground;
                    btnTabList.BorderHighlightBrush = ButtonBorderHighlightBrush;
                    btnTabList.ClickBrush = ButtonClickBrush;

                    // TODO: add Click action
                }

                if (btnScrollLeft != null)
                {
                    btnScrollLeft.HighlightBrush = ButtonHighlightBackground;
                    btnScrollLeft.BorderHighlightBrush = ButtonBorderHighlightBrush;
                    btnScrollLeft.ClickBrush = ButtonClickBrush;

                    btnScrollLeft.Click += (s, e) =>
                    {
                        DoScroll(TabScrollCommandAction.Left);
                    };
                }

                if (btnScrollRight != null)
                {
                    btnScrollRight.HighlightBrush = ButtonHighlightBackground;
                    btnScrollRight.BorderHighlightBrush = ButtonBorderHighlightBrush;
                    btnScrollRight.ClickBrush = ButtonClickBrush;

                    btnScrollRight.Click += (s, e) =>
                    {
                        DoScroll(TabScrollCommandAction.Left);
                    };
                }
            }
        }

        #endregion

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

            CheckScrolling();
        }

        private void Items_SelectionChanged(object sender, CollectionSelectionChangedEventArgs e)
        {
            if (_internalAction) return;

            if (e.AddedItems.Count > 0)
            {
                // selection has changed
                TabItem newItem = ((SelectionChangedEventArgs<TabItem>)e).AddedItems[0];

                UpdateSelectedState(newItem);
                SetupCurrentTab(newItem);

                TabChanged?.Invoke(this, new TabItemChangeEventArgs(newItem));
            }
            else
            {
                // nothing selected now
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
            SelectedTab = tab;
            SelectedTabContent = tab?.Content;
        }

        void UpdateSelectedState(TabItem? selectedTab)
        {
            // fix to make sure the correct tab has the IsSelected state
            if (ic != null)
            {
                for (int i = 0; i < ic.Items.Count; i++)
                {
                    if (ic.Items[i] != null)
                    {
                        Control? c = ic.ContainerFromItem(ic.Items[i]!);
                        if (c is ContentPresenter cp)
                        {
                            cp.ApplyTemplate();
                            if (cp.Child is TabDisplayItem tb)
                            {
                                if (tb.TabItem != null && tb.TabItem == selectedTab)
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
                }
            }
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

        /// <summary>
        /// Select and bring into view a particular tab on this TabControl.
        /// </summary>
        /// <param name="tab">The tab to close.</param>
        /// <remarks>
        /// If <paramref name="tab"/> is not in this TabControl, then nothing will happen.
        /// </remarks>
        public void SwitchToTab(TabItem tab)
        {
            if (Items.Contains(tab))
            {
                Items.Select(tab);
                tab.BringIntoView();
            }
        }

        #endregion

        /// <inheritdoc/>
        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            switch (change.Property.Name)
            {
                case nameof(ColorScheme):
                    OnColorSchemeChange(change);
                    break;
            }
        }

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

        #region Color Scheme

        /// <summary>
        /// Get or set the color scheme to apply to the control. The color scheme can quickly apply a whole visual style to your control.
        /// </summary>
        [Category("Appearance")]
        public ColorScheme ColorScheme { get => GetValue(ColorSchemeProperty); set => SetValue(ColorSchemeProperty, value); }

        /// <summary>The backing styled property for <see cref="ColorScheme"/>. See the related property for details.</summary>
        public static readonly StyledProperty<ColorScheme> ColorSchemeProperty
            = AvaloniaProperty.Register<CheckBox, ColorScheme>(nameof(ColorScheme), new ColorScheme());

        void OnColorSchemeChange(AvaloniaPropertyChangedEventArgs e)
        {
            ApplyColorScheme(e.GetNewValue<ColorScheme>());
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
                ButtonBorderHighlightBrush = cs.BorderColor.ToBrush();
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
                ButtonBorderHighlightBrush = cs.HighlightColor.ToBrush();
            }
        }
        #endregion

        #region Brushes

        /// <summary>
        /// Get or set the background used for the content area of the TabControl.
        /// </summary>
        public IBrush ContentAreaBackground { get => GetValue(ContentAreaBackgroundProperty); set => SetValue(ContentAreaBackgroundProperty, value); }

        /// <summary>The backing styled property for <see cref="ContentAreaBackground"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush> ContentAreaBackgroundProperty
            = AvaloniaProperty.Register<TabControl, IBrush>(nameof(ContentAreaBackground), Colors.White.ToBrush());


        /// <summary>
        /// Get or set the brush used for the background of a tab while it is highlighted (i.e. mouse over, keyboard focus).
        /// </summary>
        public IBrush TabHighlightBrush { get => GetValue(TabHighlightBrushProperty); set => SetValue(TabHighlightBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="TabHighlightBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush> TabHighlightBrushProperty
            = AvaloniaProperty.Register<TabControl, IBrush>(nameof(TabHighlightBrush), Colors.Gainsboro.ToBrush());


        /// <summary>
        /// Get or set the brush used for the borders of a tab while it is highlighted (i.e. mouse over, keyboard focus).
        /// </summary>
        public IBrush TabBorderHighlightBrush { get => GetValue(TabBorderHighlightBrushProperty); set => SetValue(TabBorderHighlightBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="TabBorderHighlightBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush> TabBorderHighlightBrushProperty
            = AvaloniaProperty.Register<TabControl, IBrush>(nameof(TabBorderHighlightBrush), Colors.DimGray.ToBrush());


        /// <summary>
        /// Get or set the brush used for the borders of tabs. This is different from the <see cref="TemplatedControl.BorderBrush"/> 
        /// used for the rest of the TabControl.
        /// </summary>
        public IBrush TabBorderBrush { get => GetValue(TabBorderBrushProperty); set => SetValue(TabBorderBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="TabBorderBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush> TabBorderBrushProperty
            = AvaloniaProperty.Register<TabControl, IBrush>(nameof(TabBorderBrush), Colors.Black.ToBrush());


        /// <summary>
        /// Get or set the brush used for the close glyph used in the tabs (where <see cref="TabItem.CanClose"/> is set to <c>true</c>).
        /// </summary>
        public IBrush TabCloseBrush { get => GetValue(TabCloseBrushProperty); set => SetValue(TabCloseBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="TabCloseBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush> TabCloseBrushProperty
            = AvaloniaProperty.Register<TabControl, IBrush>(nameof(TabCloseBrush), Colors.Black.ToBrush());


        /// <summary>
        /// Get or set the brush used for the background of a tab. Individual tabs can overwrite their backgrounds by changing <see cref="TabItem.TabBackground"/>.
        /// </summary>
        public IBrush TabBackground { get => GetValue(TabBackgroundProperty); set => SetValue(TabBackgroundProperty, value); }

        /// <summary>The backing styled property for <see cref="TabBackground"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush> TabBackgroundProperty
            = AvaloniaProperty.Register<TabControl, IBrush>(nameof(TabBackground), Colors.LightGray.ToBrush());


        /// <summary>
        /// Get or set the brush used for the background of a selected tab.
        /// </summary>
        public IBrush SelectedTabBackground { get => GetValue(SelectedTabBackgroundProperty); set => SetValue(SelectedTabBackgroundProperty, value); }

        /// <summary>The backing styled property for <see cref="SelectedTabBackground"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush> SelectedTabBackgroundProperty
            = AvaloniaProperty.Register<TabControl, IBrush>(nameof(SelectedTabBackground), Colors.White.ToBrush());


        /// <summary>
        /// Get or set the brush used for buttons in the TabControl, when they are highlighted (i.e. mouse over).
        /// </summary>
        public IBrush ButtonHighlightBackground { get => GetValue(ButtonHighlightBackgroundProperty); set => SetValue(ButtonHighlightBackgroundProperty, value); }

        /// <summary>The backing styled property for <see cref="ButtonHighlightBackground"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush> ButtonHighlightBackgroundProperty
            = AvaloniaProperty.Register<TabControl, IBrush>(nameof(ButtonHighlightBackground), Colors.Silver.ToBrush());


        /// <summary>
        /// Get or set the brush used for the borders of buttons in the TabControl, when they are highlighted (i.e. mouse over).
        /// </summary>
        public IBrush ButtonBorderHighlightBrush { get => GetValue(ButtonBorderHighlightBrushProperty); set => SetValue(ButtonBorderHighlightBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="ButtonBorderHighlightBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush> ButtonBorderHighlightBrushProperty
            = AvaloniaProperty.Register<TabControl, IBrush>(nameof(ButtonBorderHighlightBrush), Colors.DimGray.ToBrush());


        /// <summary>
        /// Get or set the brush used for buttons in the TabControl, when they are being clicked (i.e. mouse down, key down).
        /// </summary>
        public IBrush ButtonClickBrush { get => GetValue(ButtonClickBrushProperty); set => SetValue(ButtonClickBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="ButtonClickBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush> ButtonClickBrushProperty
            = AvaloniaProperty.Register<TabControl, IBrush>(nameof(ButtonClickBrush), Colors.LightGray.ToBrush());

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
                    //cm.ColorScheme = ColorScheme;
                    cm.Placement = PlacementMode.Pointer;
                    cm.Open();
                }
            }
        }

        private void tdi_TabItemDrop(object sender, TabItemDropEventArgs e)
        {
            if (e.DroppedTabItem == e.SourceTabItem) return;
            if (!Items.Contains(e.DroppedTabItem)) return;

            _internalAction = true;
            TabItem? selItem = null;

            if (Items.SelectedItems.Count != 0)
            {
                selItem = Items.SelectedItems.First();
            }
            Items.ClearSelection();

            Items.Remove(e.DroppedTabItem);

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
            UpdateSelectedState(selItem);

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

        private bool _scrollButtonsVisible = false;

        /// <summary>
        /// Get if the scroll buttons are currently visible in the tab bar.
        /// </summary>
        public bool ScrollButtonsVisible { get => _scrollButtonsVisible; private set => SetAndRaise(ScrollButtonsVisibleProperty, ref _scrollButtonsVisible, value); }

        /// <summary>The backing direct property for <see cref="ScrollButtonsVisible"/>. See the related property for details.</summary>
        public static readonly DirectProperty<TabControl, bool> ScrollButtonsVisibleProperty
            = AvaloniaProperty.RegisterDirect<TabControl, bool>(nameof(ScrollButtonsVisible), (s) => s.ScrollButtonsVisible, unsetValue: false);

        void CheckScrolling()
        {
            if (sv == null || ic == null) return;

            if (sv.Viewport.Width == 0)
            {
                return;
            }

            if (ic.Width > sv.Viewport.Width)
            {
                ScrollButtonsVisible = true;
            }
            else
            {
                ScrollButtonsVisible = false;
            }
        }

        /// <summary>
        /// Perform a scroll action on the TabControl's tab bar.
        /// </summary>
        /// <param name="a">The scroll action to take</param>
        public void DoScroll(TabScrollCommandAction a)
        {
            if (sv == null) return;

            // double offset = sv.Offset.X;

            switch (a)
            {
                case TabScrollCommandAction.Left:
                    sv.LineLeft(); //.ScrollToHorizontalOffset(Math.Max(offset - 20, 0));
                    break;
                case TabScrollCommandAction.Right:
                    sv.LineRight(); //.ScrollToHorizontalOffset(Math.Min(offset + 20, sv.ScrollableWidth));
                    break;
                case TabScrollCommandAction.Home:
                    sv.ScrollToHome(); //.ScrollToHorizontalOffset(0);
                    break;
                case TabScrollCommandAction.End:
                    sv.ScrollToEnd(); //.ScrollToHorizontalOffset(sv.ScrollableWidth);
                    break;
                default:
                    break;
            }
        }

        private void control_SizeChanged(object? sender, SizeChangedEventArgs e)
        {
            // note that this both handles the TabControl changes and the internal ItemControl changes
            if (e.WidthChanged)
            {
                CheckScrolling();
            }
        }

        private void sv_ScrollChanged(object? sender, ScrollChangedEventArgs e)
        {
            if (e.ViewportDelta.X != 0)
            {
                CheckScrolling();
            }
        }

        #endregion
    }
}
