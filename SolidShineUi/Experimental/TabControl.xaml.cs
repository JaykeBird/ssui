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
                    Binding csb = new Binding("ColorScheme");
                    csb.Source = this;
                    item.SetBinding(TabDisplayItem.ColorSchemeProperty, csb);
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
                    break;
                default:
                    break;
            }
        }

        void AddTab(TabItem ti)
        {
            TabDisplayItem tdi = new TabDisplayItem(ti);

            // set color scheme binding
            Binding csb = new Binding("ColorScheme");
            csb.Source = this;
            tdi.SetBinding(TabDisplayItem.ColorSchemeProperty, csb);

            tdi.ColorScheme = ColorScheme;
            tdi.RequestClose += Tdi_RequestClose;
            tdi.Click += Tdi_Click;
            // tdi.ApplyColorScheme(cs);
            stkTabs.Children.Add(tdi);
        }

#if NETCOREAPP
        public TabItem? CurrentTab { get; internal set; } = null;

        public EventHandler? TabChanged;
        public EventHandler? TabLoaded;
        public EventHandler? TabClosed;

        private void Tdi_Click(object? sender, EventArgs e)
        {
            if (sender is TabDisplayItem tdi)
            {
                TabItem ti = tdi.TabItem;
                LoadTab(ti);
                TabChanged?.Invoke(this, e);
            }

            // TODO: set all other TabDisplayItems to be "not selected"
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
        public TabItem CurrentTab { get; internal set; } = null;

        public EventHandler TabChanged;
        public EventHandler TabLoaded;
        public EventHandler TabClosed;

        private void Tdi_Click(object sender, EventArgs e)
        {
            if (sender is TabDisplayItem tdi)
            {
                TabItem ti = tdi.TabItem;
                LoadTab(ti);
                TabChanged?.Invoke(this, e);
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
                brdrContent.BorderThickness = value;
            }
        }

        private static DependencyProperty ItemsProperty
            = DependencyProperty.Register("Items", typeof(ObservableCollection<TabItem>), typeof(TabControl),
            new FrameworkPropertyMetadata(new ObservableCollection<TabItem>()));

        //private static DependencyPropertyKey ItemsPropertyKey
        //    = DependencyProperty.RegisterReadOnly("Items", typeof(ObservableCollection<TabItem>), typeof(TabControl),
        //    new FrameworkPropertyMetadata(new ObservableCollection<TabItem>()));

        //public static DependencyProperty ItemsProperty
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


#region Color Scheme

        // TODO: add different color for inactive window caption (especially for High Contrast Mode)

        public static DependencyProperty ColorSchemeProperty
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
