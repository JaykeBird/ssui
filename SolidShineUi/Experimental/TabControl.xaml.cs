using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using TabItem = SolidShineUi.TabItem;

namespace SolidShineUi.Experimental
{
    /// <summary>
    /// Interaction logic for TabControl.xaml
    /// </summary>
    [ContentProperty("Items")]
    public partial class TabControl : UserControl
    {
        public TabControl()
        {
            InitializeComponent();
            Items = new ObservableCollection<TabItem>();
            Items.CollectionChanged += Items_CollectionChanged;

#if NETCOREAPP
            foreach (TabDisplayItem? item in stkTabs.Children)
#else
            foreach (TabDisplayItem item in stkTabs.Children)
#endif
            {
                if (item != null)
                {
#pragma warning disable IDE0017 // Simplify object initialization
                    Binding csb = new Binding("ColorScheme");
                    csb.Source = this;
                    item.SetBinding(TabDisplayItem.ColorSchemeProperty, csb);
                    item.GetBindingExpression(TabDisplayItem.ColorSchemeProperty).UpdateTarget();
#pragma warning restore IDE0017 // Simplify object initialization
                }
            }
        }

#if NETCOREAPP
        private void Items_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
#else
        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
#endif
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
#if NETCOREAPP
                    foreach (TabItem? item in e.NewItems ?? new List<TabItem>())
#else
                    foreach (TabItem item in e.NewItems)
#endif
                    {
                        if (item != null)
                        {
                            AddTab(item);
                        }
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    List<TabDisplayItem> itemsToRemove = new List<TabDisplayItem>();

#if NETCOREAPP
                    foreach (TabDisplayItem? item in stkTabs.Children)
#else
                    foreach (TabDisplayItem item in stkTabs.Children)
#endif
                    {
                        if (e.OldItems != null)
                        {
                            if (item != null && e.OldItems.Contains(item.TabItem))
                            {
                                itemsToRemove.Add(item);
                            }

                            if (e.OldItems.Contains(CurrentTab))
                            {
                                CurrentTab = null;
                            }
                        }
                    }

                    foreach (TabDisplayItem item in itemsToRemove)
                    {
                        stkTabs.Children.Remove(item);
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
#if NETCOREAPP
                    foreach (TabItem? item in e.NewItems ?? new List<TabItem>())
#else
                    foreach (TabItem item in e.NewItems)
#endif
                    {
                        if (item != null)
                        {
                            AddTab(item);
                        }
                    }

                    List<TabDisplayItem> itemsToRemove2 = new List<TabDisplayItem>();

#if NETCOREAPP
                    foreach (TabDisplayItem? item in stkTabs.Children)
#else
                    foreach (TabDisplayItem item in stkTabs.Children)
#endif
                    {
                        if (e.OldItems != null)
                        {
                            if (item != null && e.OldItems.Contains(item.TabItem))
                            {
                                itemsToRemove2.Add(item);
                            }

                            if (e.OldItems.Contains(CurrentTab))
                            {
                                CurrentTab = null;
                            }
                        }
                    }

                    foreach (TabDisplayItem item in itemsToRemove2)
                    {
                        stkTabs.Children.Remove(item);
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    stkTabs.Children.Clear();

                    foreach (TabItem item in Items)
                    {
                        AddTab(item);
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    stkTabs.Children.Clear();
                    CurrentTab = null;
                    break;
                default:
                    break;
            }

            CheckScrolling();

            if (CurrentTab == null)
            {
                if (Items.Count > 0)
                {
                    try
                    {
                        TabItem ti = Items.First((tii) => tii.CanSelect);
                        LoadTab(ti);
                        TabChanged?.Invoke(this, e);
                        ti.Select();
                    }
                    catch (InvalidOperationException)
                    {
                        // none of the current tabs can be selected
                        brdrContent.Child = null;
                    }
                }
                else
                {
                    brdrContent.Child = null;
                }
            }
        }

        void AddTab(TabItem ti)
        {
#pragma warning disable IDE0017 // Simplify object initialization
            TabDisplayItem tdi = new TabDisplayItem(ti);

            // set color scheme binding
            Binding csb = new Binding("ColorScheme");

            csb.Source = this;
            tdi.SetBinding(TabDisplayItem.ColorSchemeProperty, csb);

            //tdi.ColorScheme = ColorScheme;
            tdi.RequestClose += Tdi_RequestClose;
            tdi.Click += Tdi_Click;
            // tdi.ApplyColorScheme(cs);
            stkTabs.Children.Add(tdi);
#pragma warning restore IDE0017 // Simplify object initialization
        }

#if NETCOREAPP
        public TabItem? CurrentTab { get; protected set; } = null;
        public TabItem? SelectedTab { get => CurrentTab; }

        public EventHandler? TabChanged;
        public EventHandler? TabLoaded;
        public EventHandler? TabClosed;

        private void Tdi_Click(object? sender, EventArgs e)
        {
            if (sender is TabDisplayItem tdi)
            {
                TabItem ti = tdi.TabItem;
                if (ti.CanSelect)
                {
                    LoadTab(ti);
                    TabChanged?.Invoke(this, e);
                    ti.Select();
                }
            }

            foreach (TabItem ti in Items)
            {
                if (ti != CurrentTab)
                {
                    ti.Unselect();
                }
            }
        }

        private void Tdi_RequestClose(object? sender, EventArgs e)
        {
            if (sender is TabDisplayItem tdi)
            {
                TabItem ti = tdi.TabItem;
                Items.Remove(ti);
                if (CurrentTab == ti)
                {
                    brdrContent.Child = null;
                    CurrentTab = null;
                    TabChanged?.Invoke(this, e);
                }
                TabClosed?.Invoke(this, e);
            }
        }
#else
        public TabItem CurrentTab { get; protected set; } = null;
        public TabItem SelectedTab { get => CurrentTab; }

        public EventHandler TabChanged;
        public EventHandler TabLoaded;
        public EventHandler TabClosed;

        private void Tdi_Click(object sender, EventArgs e)
        {
            if (sender is TabDisplayItem tdi)
            {
                TabItem ti = tdi.TabItem;
                if (ti.CanSelect)
                {
                    LoadTab(ti);
                    TabChanged?.Invoke(this, e);
                    ti.Select();
                }
            }

            foreach (TabItem ti in Items)
            {
                if (ti != CurrentTab)
                {
                    ti.Unselect();
                }
            }
        }

        private void Tdi_RequestClose(object sender, EventArgs e)
        {
            if (sender is TabDisplayItem tdi)
            {
                TabItem ti = tdi.TabItem;
                Items.Remove(ti);
                if (CurrentTab == ti)
                {
                    brdrContent.Child = null;
                    CurrentTab = null;
                    TabChanged?.Invoke(this, e);
                }
                TabClosed?.Invoke(this, e);
            }
        }
#endif

        public Brush ContentBorderBrush
        {
            get
            {
                return brdrContent.BorderBrush;
            }
            set
            {
                brdrContent.BorderBrush = value;
                brdrTabTop.BorderBrush = value;
                brdrScrollLeft.BorderBrush = value;
                brdrScrollRight.BorderBrush = value;
            }
        }

        public Thickness ContentBorderThickness
        {
            get
            {
                return brdrContent.BorderThickness;
            }
            set
            {
                brdrContent.BorderThickness = new Thickness(value.Left, 0, value.Right, value.Bottom);
                brdrTabTop.BorderThickness = new Thickness(0, value.Top, 0, 0);
                brdrScrollLeft.BorderThickness = new Thickness(0, value.Top, 0, 0);
                brdrScrollRight.BorderThickness = new Thickness(0, value.Top, 0, 0);
            }
        }

        private static readonly DependencyProperty ItemsProperty
            = DependencyProperty.Register("Items", typeof(ObservableCollection<TabItem>), typeof(TabControl),
            new FrameworkPropertyMetadata(new ObservableCollection<TabItem>()));

        //private static readonly DependencyPropertyKey ItemsPropertyKey
        //    = DependencyProperty.RegisterReadOnly("Items", typeof(ObservableCollection<TabItem>), typeof(TabControl),
        //    new FrameworkPropertyMetadata(new ObservableCollection<TabItem>()));

        //public static readonly DependencyProperty ItemsProperty
        //    = ItemsPropertyKey.DependencyProperty;

        public ObservableCollection<TabItem> Items
        {
            get { return (ObservableCollection<TabItem>)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        private void LoadTab(TabItem tab)
        {
            brdrContent.Child = tab.Content;
            CurrentTab = tab;
            TabLoaded?.Invoke(this, EventArgs.Empty);
        }

        #region Scrolling

        void CheckScrolling()
        {
            if (scrTabs.ViewportWidth == 0)
            {
                return;
            }

            if (stkTabs.ActualWidth > scrTabs.ViewportWidth)
            {
                colScrollLeft.Width = new GridLength(20);
                colScrollRight.Width = new GridLength(20);

                btnScrollLeft.IsEnabled = true;
                btnScrollRight.IsEnabled = true;
            }
            else
            {
                colScrollLeft.Width = new GridLength(0);
                colScrollRight.Width = new GridLength(0);

                btnScrollLeft.IsEnabled = false;
                btnScrollRight.IsEnabled = false;
            }
        }

        private void btnScrollLeft_Click(object sender, RoutedEventArgs e)
        {
            double offset = scrTabs.HorizontalOffset;

            if (offset > 0)
            {
                scrTabs.ScrollToHorizontalOffset(Math.Max(offset - 20, 0));
            }
        }

        private void btnScrollRight_Click(object sender, RoutedEventArgs e)
        {
            double offset = scrTabs.HorizontalOffset;

            if (offset < scrTabs.ScrollableWidth)
            {
                scrTabs.ScrollToHorizontalOffset(Math.Min(offset + 20, scrTabs.ScrollableWidth));
            }
        }

        private void control_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.WidthChanged)
            {
                CheckScrolling();
            }
        }

        private void scrTabs_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.ViewportWidthChange > 0)
            {
                CheckScrolling();
            }
        }

        #endregion

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

            ContentBorderBrush = cs.BorderColor.ToBrush();

            //#if NETCOREAPP
            //            foreach (TabDisplayItem? item in stkTabs.Children)
            //#else
            //            foreach (TabDisplayItem item in stkTabs.Children)
            //#endif
            //            {
            //                if (item != null)
            //                {
            //                    item.ApplyColorScheme(cs);
            //                }
            //            }
        }

        public void ApplyColorScheme(HighContrastOption hco)
        {
            ColorScheme cs = ColorScheme.GetHighContrastScheme(hco);

            ApplyColorScheme(cs);
        }
        #endregion
    }
}
