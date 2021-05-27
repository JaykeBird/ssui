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

namespace SolidShineUi
{
    public class TabControl : Control
    {

        static TabControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TabControl), new FrameworkPropertyMetadata(typeof(TabControl)));
        }

        public TabControl()
        {
            SizeChanged += control_SizeChanged;

            Items = new SelectableCollection<TabItem>();
            Items.CanSelectMultiple = false;
            Items.CollectionChanged += Items_CollectionChanged;
            Items.SelectionChanged += items_SelectionChanged;

            InternalShowTabsOnBottomChanged += tabControl_InternalShowTabsOnBottomChanged;
            InternalShowTabListMenuChanged += tabControl_InternalShowTabListMenuChanged;

            CommandBindings.Add(new CommandBinding(TabListMenuItemClick, OnTabListMenuItemClick));

            if (Items.Count > 0)
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

        }

        //private static readonly DependencyProperty ItemsProperty
        //    = DependencyProperty.Register("Items", typeof(ObservableCollection<TabItem>), typeof(TabControl),
        //    new FrameworkPropertyMetadata(new ObservableCollection<TabItem>()));

        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register("Items", typeof(SelectableCollection<TabItem>), typeof(TabControl),
            new FrameworkPropertyMetadata(new SelectableCollection<TabItem>()));

        //public static readonly DependencyProperty ItemsProperty = ItemsPropertyKey.DependencyProperty;

        public SelectableCollection<TabItem> Items
        {
            get { return (SelectableCollection<TabItem>)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        public delegate void TabItemChangeEventHandler(object sender, TabItemChangeEventArgs e);

#if NETCOREAPP
        public TabItem? CurrentTab { get => Items.SelectedItems.FirstOrDefault(); }
        public TabItem? SelectedTab { get => CurrentTab; }

        public event TabItemChangeEventHandler? TabChanged;
        public event TabItemChangeEventHandler? TabClosed;
        public event EventHandler? TabsCleared;
#else
        public TabItem CurrentTab { get; protected set; } = null;
        public TabItem SelectedTab { get => CurrentTab; }

        public event TabItemChangeEventHandler TabChanged;
        public event TabItemChangeEventHandler TabClosed;
        public event EventHandler TabsCleared;
#endif

        #region ShowTabsOnBottom

        public static readonly DependencyProperty ShowTabsOnBottomProperty = DependencyProperty.Register("ShowTabsOnBottom", typeof(bool), typeof(TabControl),
            new PropertyMetadata(false, new PropertyChangedCallback(OnInternalShowTabsOnBottomChanged)));

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
        /// Get or set the color scheme to apply to the window.
        /// </summary>
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

            CheckScrolling();
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
                    Items.Remove(tdi.TabItem);
                }
            }

            CheckScrolling();
        }

        #region Scrolling

        #region ScrollButtons

        private static readonly DependencyPropertyKey ScrollButtonsVisiblePropertyKey = DependencyProperty.RegisterReadOnly("ScrollButtonsVisible", typeof(bool), typeof(TabControl),
            new PropertyMetadata(false));

        public static readonly DependencyProperty ScrollButtonsVisibleProperty = ScrollButtonsVisiblePropertyKey.DependencyProperty;

        public bool ScrollButtonsVisible
        {
            get { return (bool)GetValue(ScrollButtonsVisibleProperty); }
            private set { SetValue(ScrollButtonsVisiblePropertyKey, value); }
        }
        #endregion

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

        private void btnScrollLeft_Click(object sender, RoutedEventArgs e)
        {
            if (sv == null) return;

            double offset = sv.HorizontalOffset;

            if (offset > 0)
            {
                sv.ScrollToHorizontalOffset(Math.Max(offset - 20, 0));
            }
        }

        private void btnScrollRight_Click(object sender, RoutedEventArgs e)
        {
            double offset = sv.HorizontalOffset;

            if (offset < sv.ScrollableWidth)
            {
                sv.ScrollToHorizontalOffset(Math.Min(offset + 20, sv.ScrollableWidth));
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

    public class TabItemChangeEventArgs
    {
        public TabItemChangeEventArgs(TabItem t)
        {
            TabItem = t;
        }

        public TabItem TabItem { get; private set; }
    }
}
