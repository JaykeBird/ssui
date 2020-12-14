using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SolidShineUi
{
    /// <summary>
    /// Interaction logic for SelectPanel.xaml
    /// </summary>
    [DefaultEvent(nameof(SelectionChanged))]
    public partial class SelectPanel : UserControl
    {

        public SelectPanel()
        {
            InitializeComponent();
            //UItems.CollectionChanged += UItems_CollectionChanged;
        }

        public new object Content
        {
            get
            {
                return base.Content;
            }
            set
            {

            }
        }

        #region Item Management

        #region IObservableCollection

        // the eventual goal is to move over towards using an IObservableCollection

        //private static readonly DependencyPropertyKey UItemsPropertyKey
        //    = DependencyProperty.RegisterReadOnly("UItems", typeof(ObservableCollection<SelectableUserControl>), typeof(SelectPanel),
        //    new FrameworkPropertyMetadata(new ObservableCollection<SelectableUserControl>()));

        //public static readonly DependencyProperty UItemsProperty
        //    = UItemsPropertyKey.DependencyProperty;

        //public ObservableCollection<SelectableUserControl> UItems
        //{
        //    get { return (ObservableCollection<SelectableUserControl>)GetValue(UItemsProperty); }
        //}

        //private void UItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        //{
        //    throw new NotImplementedException();
        //}

        #endregion

        [Obsolete("Try to interact with items through the AddItem(s), Items, and RemoveItem(s) methods.", false)]
        public UIElementCollection Children
        {
            get
            {
                return stkItems.Children;
            }
        }

        public int Count
        {
            get
            {
                return Items.Count();
            }
        }

        public void AddItem(SelectableUserControl item)
        {
            AddItemInternal(item);
            RaiseItemsAddedEvent(new List<SelectableUserControl>() { item });
        }

        private void AddItemInternal(SelectableUserControl item, int? index = null)
        {
            item.Background = Color.FromArgb(1, 0, 0, 0).ToBrush();
            item.HighlightBrush = HighlightBrush;
            item.ClickBrush = ClickBrush;
            item.SelectedBrush = SelectedBrush;

            item.SelectionChanged += Item_SelectionChanged;

            if (index.HasValue)
            {
                stkItems.Children.Insert(index.Value, item);
            }
            else
            {
                stkItems.Children.Add(item);
            }
            
            item.ApplyColorScheme(ColorScheme);
        }

        public void AddItems(IEnumerable<SelectableUserControl> items)
        {
            foreach (SelectableUserControl item in items)
            {
                AddItemInternal(item);
            }
            RaiseItemsAddedEvent(items.ToList());
        }

        public void InsertItem(int index, SelectableUserControl item)
        {
            AddItemInternal(item, index);
            RaiseItemsAddedEvent(new List<SelectableUserControl>() { item });
        }

        public void InsertItems(int index, IEnumerable<SelectableUserControl> items)
        {
            List<SelectableUserControl> litems = items.ToList();
            litems.Reverse();

            foreach (SelectableUserControl item in litems)
            {
                AddItemInternal(item, index);
            }

            RaiseItemsAddedEvent(litems);
        }

        public IEnumerable<SelectableUserControl> Items
        {
            get
            {
#if NETCOREAPP
                foreach (UIElement? item in stkItems.Children)
#else
                foreach (UIElement item in stkItems.Children)
#endif
                {
                    if (item is SelectableUserControl s)
                    {
                        yield return s;
                    }
                }
            }
        }

        public IEnumerable<T> GetItemsAsType<T>() where T : SelectableUserControl
        {
#if NETCOREAPP
            foreach (UIElement? item in stkItems.Children)
#else
            foreach (UIElement item in stkItems.Children)
#endif
            {
                if (item is T t)
                {
                    yield return t;
                }
            }
        }

        public void RemoveItem(SelectableUserControl item)
        {
            if (item.IsSelected)
            {
                RaiseSelectionChangedEvent(new List<SelectableUserControl>(), new List<SelectableUserControl> { item });
                SelectionCount--;
            }
            stkItems.Children.Remove(item);
            RaiseItemsRemovedEvent(new List<SelectableUserControl>() { item });
        }

        public void RemoveItems(IEnumerable<SelectableUserControl> items)
        {
            foreach (var item in items)
            {
                RemoveItem(item);
            }
        }

        public void RemoveAt(int index)
        {
            UIElement u = stkItems.Children[index];
            if (u is SelectableUserControl s)
            {
                if (s.IsSelected)
                {
                    RaiseSelectionChangedEvent(new List<SelectableUserControl>(), new List<SelectableUserControl> { s });
                    SelectionCount--;
                }

                RemoveItem(s);
            }
            else
            {
                stkItems.Children.RemoveAt(index);
            }
        }

        public void RemoveRange(int index, int count)
        {
            stkItems.Children.RemoveRange(index, count);
        }

        public int IndexOf(SelectableUserControl item)
        {
            return stkItems.Children.IndexOf(item);
        }

        public SelectableUserControl Get(int index)
        {
            if (stkItems.Children[index] is SelectableUserControl s)
            {
                return s;
            }
            else
            {
                throw new InvalidOperationException("An element not of the SelectableUserControl type is present in the SelectPanel. Please remove the item.");
            }
        }

        public SelectableUserControl this[int index]
        {
            get
            {
                return Get(index);
            }
        }

        public void Clear()
        {
            SelectionCount = 0;
            List<SelectableUserControl> items = Items.ToList();
            stkItems.Children.Clear();
            RaiseItemsRemovedEvent(items);
        }

        #endregion

        #region Item Interaction

        bool selUpdate = false;
        bool multi = true;

        public int SelectionCount { get; private set; } = 0;

        [Category("Common")]
        public bool MultiSelect
        {
            get
            {
                return multi;
            }
            set
            {
                multi = value;
                if (multi == false)
                {
                    if (SelectionCount > 1)
                    {
                        DeselectAll();
                    }
                }
            }
        }

#if NETCOREAPP
        private void Item_SelectionChanged(object? sender, EventArgs e)
#else
        private void Item_SelectionChanged(object sender, EventArgs e)
#endif
        {
            if (selUpdate)
            {
                return;
            }

            if (sender is SelectableUserControl s)
            {

                List<SelectableUserControl> addedItems = new List<SelectableUserControl>() { s };
                List<SelectableUserControl> removedItems = new List<SelectableUserControl>();

                if (s.IsSelected == false)
                {
                    if (SelectionCount > 0) SelectionCount--;
                    addedItems.Clear();
                    removedItems.Add(s);
                }
                else
                {
                    if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control) && MultiSelect)
                    {
                        // simply add to selection
                        SelectionCount++;
                    }
                    else
                    {
#if NETCOREAPP
                        foreach (UIElement? item in stkItems.Children)
#else
                        foreach (UIElement item in stkItems.Children)
#endif
                        {
                            if (item is SelectableUserControl r && item != sender)
                            {
                                selUpdate = true;
                                (r).IsSelected = false;
                                removedItems.Add(r);
                                selUpdate = false;
                            }
                        }

                        SelectionCount = 1;
                    }
                }
                RaiseSelectionChangedEvent(addedItems, removedItems);
            }
        }

        /// <summary>
        /// Get a collection of items that have been selected.
        /// </summary>
        public IEnumerable<SelectableUserControl> SelectedItems
        {
            get
            {
#if NETCOREAPP
                foreach (UIElement? item in stkItems.Children)
#else
                foreach (UIElement item in stkItems.Children)
#endif
                {
                    if (item is SelectableUserControl s)
                    {
                        if (s.IsSelected)
                        {
                            yield return s;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Get a collection of items that have been selected, returned as a certain type (that inherits from SelectableUserControl).
        /// </summary>
        /// <typeparam name="T">The type to return the selected items as. It must inherit from SelectableUserControl.</typeparam>
        /// <returns></returns>
        public IEnumerable<T> GetSelectedItemsOfType<T>() where T : SelectableUserControl
        {
#if NETCOREAPP
            foreach (UIElement? item in stkItems.Children)
#else
            foreach (UIElement item in stkItems.Children)
#endif
            {
                if (item is T t)
                {
                    if (t.IsSelected)
                    {
                        yield return t;
                    }
                }
            }
        }

        public void RemoveSelectedItems()
        {
            List<SelectableUserControl> items = SelectedItems.ToList();

            foreach (var item in items)
            {
                stkItems.Children.Remove(item);
            }

            SelectionCount = 0;
            RaiseSelectionChangedEvent(new List<SelectableUserControl>(), items);
        }

        public void SelectAll()
        {
            selUpdate = true;
            List<SelectableUserControl> addedItems = new List<SelectableUserControl>();

#if NETCOREAPP
            foreach (UIElement? item in stkItems.Children)
#else
            foreach (UIElement item in stkItems.Children)
#endif
            {
                if (item is SelectableUserControl s)
                {
                    if (s.CanSelect)
                    {
                        if (!s.IsSelected)
                        {
                            addedItems.Add(s);
                            s.IsSelected = true;
                            SelectionCount++;
                        }
                    }
                }
            }

            selUpdate = false;
            RaiseSelectionChangedEvent(addedItems, new List<SelectableUserControl>());
        }

        public void DeselectAll()
        {
#if NETCOREAPP
            foreach (UIElement? item in stkItems.Children)
#else
            foreach (UIElement item in stkItems.Children)
#endif
            {
                if (item is SelectableUserControl s)
                {
                    if (s.CanSelect)
                    {
                        s.IsSelected = false;
                    }
                }
            }
        }

        public void MoveSelectedItemsUp()
        {
            bool first = true;
            int index = -1;

            List<SelectableUserControl> imov = new List<SelectableUserControl>();

            foreach (SelectableUserControl item in SelectedItems)
            {
                if (first)
                {
                    index = IndexOf(item) - 1;
                    first = false;
                }

                imov.Add(item);
            }

            if (index < 0)
            {
                index = 0;
            }

            foreach (SelectableUserControl item in imov)
            {
                stkItems.Children.Remove(item);
            }

            imov.Reverse();

            foreach (SelectableUserControl item in imov)
            {
                stkItems.Children.Insert(index, item);
            }
        }

        public void MoveSelectedItemsDown()
        {
            int index = -1;

            List<SelectableUserControl> imov = new List<SelectableUserControl>();

            foreach (SelectableUserControl item in SelectedItems)
            {
                index = IndexOf(item) + 1;

                imov.Add(item);
            }

            if (index > (stkItems.Children.Count - 1))
            {
                index = (stkItems.Children.Count - 1);
            }

            foreach (SelectableUserControl item in imov)
            {
                stkItems.Children.Remove(item);
                stkItems.Children.Insert(index, item);
            }
        }

        public void MoveItemUp(int index)
        {
            SelectableUserControl suc = this[index];

            int moveIndex = index - 1;//IndexOf(suc) - 1;
            if (moveIndex < 0)
            {
                moveIndex = 0;
            }

            stkItems.Children.Remove(suc);
            stkItems.Children.Insert(moveIndex, suc);
        }

        public void MoveItemDown(int index)
        {
            SelectableUserControl suc = this[index];

            int moveIndex = index + 1; //IndexOf(suc) + 1;
            if (moveIndex > (stkItems.Children.Count - 1))
            {
                moveIndex = (stkItems.Children.Count - 1);
            }

            stkItems.Children.Remove(suc);
            stkItems.Children.Insert(moveIndex, suc);
        }

        #endregion

        #region Routed Events

        public static readonly RoutedEvent SelectionChangedEvent = EventManager.RegisterRoutedEvent(
            "SelectionChanged", RoutingStrategy.Bubble, typeof(SelectionChangedEventHandler), typeof(SelectPanel));

        public event SelectionChangedEventHandler SelectionChanged
        {
            add { AddHandler(SelectionChangedEvent, value); }
            remove { RemoveHandler(SelectionChangedEvent, value); }
        }

        void RaiseSelectionChangedEvent(List<SelectableUserControl> addedItems, List<SelectableUserControl> removedItems)
        {
            if (addedItems == null)
            {
                addedItems = new List<SelectableUserControl>();
            }

            if (removedItems == null)
            {
                removedItems = new List<SelectableUserControl>();
            }

            SelectionChangedEventArgs newEventArgs = new SelectionChangedEventArgs(SelectionChangedEvent, addedItems, removedItems);
            RaiseEvent(newEventArgs);
        }

        public static readonly RoutedEvent ItemsAddedEvent = EventManager.RegisterRoutedEvent(
            "ItemsAdded", RoutingStrategy.Bubble, typeof(SelectionChangedEventHandler), typeof(SelectPanel));

        public event SelectionChangedEventHandler ItemsAdded
        {
            add { AddHandler(ItemsAddedEvent, value); }
            remove { RemoveHandler(ItemsAddedEvent, value); }
        }

        void RaiseItemsAddedEvent(List<SelectableUserControl> addedItems)
        {
            if (addedItems == null)
            {
                addedItems = new List<SelectableUserControl>();
            }

            SelectionChangedEventArgs newEventArgs = new SelectionChangedEventArgs(ItemsAddedEvent, new List<SelectableUserControl>(), addedItems);
            RaiseEvent(newEventArgs);
        }

        public static readonly RoutedEvent ItemsRemovedEvent = EventManager.RegisterRoutedEvent(
            "ItemsRemoved", RoutingStrategy.Bubble, typeof(SelectionChangedEventHandler), typeof(SelectPanel));

        public event SelectionChangedEventHandler ItemsRemoved
        {
            add { AddHandler(ItemsRemovedEvent, value); }
            remove { RemoveHandler(ItemsRemovedEvent, value); }
        }

        void RaiseItemsRemovedEvent(List<SelectableUserControl> removedItems)
        {
            if (removedItems == null)
            {
                removedItems = new List<SelectableUserControl>();
            }

            SelectionChangedEventArgs newEventArgs = new SelectionChangedEventArgs(ItemsRemovedEvent, removedItems, new List<SelectableUserControl>());
            RaiseEvent(newEventArgs);
        }
        #endregion

        #region Visuals

        [Category("Layout")]
        public ScrollBarVisibility HorizontalScrollBarVisibility { get; set; } = ScrollBarVisibility.Disabled;
        [Category("Layout")]
        public ScrollBarVisibility VerticalScrollBarVisibility { get; set; } = ScrollBarVisibility.Auto;

        [Category("Appearance")]
        public KeyboardNavigationMode TabNavigation
        {
            get
            {
                return KeyboardNavigation.GetTabNavigation(stkItems);
            }
            set
            {
                KeyboardNavigation.SetTabNavigation(stkItems, value);
            }
        }

        #region Brushes

        [Category("Brushes")]
        public new Brush Background
        {
            get => (Brush)GetValue(BackgroundProperty);
            set => SetValue(BackgroundProperty, value);
        }

        [Category("Brushes")]
        public Brush ClickBrush
        {
            get => (Brush)GetValue(ClickBrushProperty);
            set
            {
                SetValue(ClickBrushProperty, value);
                UpdateChildrenAppearance();
            }
        }

        [Category("Brushes")]
        public Brush SelectedBrush
        {
            get => (Brush)GetValue(SelectedBrushProperty);
            set
            {
                SetValue(SelectedBrushProperty, value);
                UpdateChildrenAppearance();
            }
        }

        [Category("Brushes")]
        public Brush HighlightBrush
        {
            get => (Brush)GetValue(HighlightBrushProperty);
            set
            {
                SetValue(HighlightBrushProperty, value);
                UpdateChildrenAppearance();
            }
        }
        [Category("Brushes")]
        public Brush DisabledBrush
        {
            get => (Brush)GetValue(DisabledBrushProperty);
            set => SetValue(DisabledBrushProperty, value);
        }

        [Category("Brushes")]
        public Brush BorderDisabledBrush
        {
            get => (Brush)GetValue(BorderDisabledBrushProperty);
            set => SetValue(BorderDisabledBrushProperty, value);
        }

        [Category("Brushes")]
        public new Brush BorderBrush
        {
            get => (Brush)GetValue(BorderBrushProperty);
            set => SetValue(BorderBrushProperty, value);
        }

        public new static DependencyProperty BackgroundProperty = DependencyProperty.Register(
            "Background", typeof(Brush), typeof(SelectPanel),
            new PropertyMetadata(new SolidColorBrush(ColorsHelper.White)));

        public static DependencyProperty ClickBrushProperty = DependencyProperty.Register(
            "ClickBrush", typeof(Brush), typeof(SelectPanel),
            new PropertyMetadata(Colors.LightSalmon.ToBrush()));

        public static DependencyProperty SelectedBrushProperty = DependencyProperty.Register(
            "SelectedBrush", typeof(Brush), typeof(SelectPanel),
            new PropertyMetadata(Colors.MistyRose.ToBrush()));

        public static DependencyProperty HighlightBrushProperty = DependencyProperty.Register(
            "HighlightBrush", typeof(Brush), typeof(SelectPanel),
            new PropertyMetadata(Colors.Salmon.ToBrush()));

        public static DependencyProperty DisabledBrushProperty = DependencyProperty.Register(
            "DisabledBrush", typeof(Brush), typeof(SelectPanel),
            new PropertyMetadata(new SolidColorBrush(Colors.Gray)));

        public static DependencyProperty BorderDisabledBrushProperty = DependencyProperty.Register(
            "BorderDisabledBrush", typeof(Brush), typeof(SelectPanel),
            new PropertyMetadata(new SolidColorBrush(Colors.DarkGray)));

        public static new DependencyProperty BorderBrushProperty = DependencyProperty.Register(
            "BorderBrush", typeof(Brush), typeof(SelectPanel),
            new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        #endregion

        [Category("Miscellaneous")]
        public new Thickness BorderThickness
        {
            get => brdr.BorderThickness;
        }

        [Category("Appearance")]
        public Thickness PanelBorderThickness
        {
            get => brdr.BorderThickness;
            set => brdr.BorderThickness = value;
        }

        #region Color Scheme

        public static DependencyProperty ColorSchemeProperty
            = DependencyProperty.Register("ColorScheme", typeof(ColorScheme), typeof(SelectPanel),
            new FrameworkPropertyMetadata(new ColorScheme(), new PropertyChangedCallback(OnColorSchemeChanged)));

        public static void OnColorSchemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SelectPanel sp)
            {
                sp.ColorSchemeChanged?.Invoke(d, e);
#if NETCOREAPP
                sp.ApplyColorScheme((e.NewValue as ColorScheme)!, sp.UseLighterBorder);
#else
                sp.ApplyColorScheme((e.NewValue as ColorScheme), sp.UseLighterBorder);
#endif
            }
        }

#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? ColorSchemeChanged;
#else
        public event DependencyPropertyChangedEventHandler ColorSchemeChanged;
#endif

        public bool use_lbrdr = false;
        bool runApply = true;

        /// <summary>
        /// Use a lighter border color when applying color schemes. Note that this does not apply in high-contrast mode.
        /// </summary>
        public bool UseLighterBorder
        {
            get
            {
                return use_lbrdr;
            }
            set
            {
                use_lbrdr = value;
                if (runApply) ApplyColorScheme(ColorScheme, value);
            }
        }

        public ColorScheme ColorScheme
        {
            get => (ColorScheme)GetValue(ColorSchemeProperty);
            set => SetValue(ColorSchemeProperty, value);
        }

        public void ReapplyColorScheme()
        {
            if (ColorScheme == null)
            {
                ColorScheme = new ColorScheme();
            }
            ApplyColorScheme(ColorScheme);
        }

        public void ApplyColorScheme(ColorScheme cs, bool useLighterBorder = false)
        {
            runApply = false;
            if (cs == null)
            {
                runApply = true;
                return;
            }
            if (cs != ColorScheme)
            {
                ColorScheme = cs;
                UseLighterBorder = (UseLighterBorder || useLighterBorder);
                runApply = true;
                return;
            }

            if (cs.IsHighContrast)
            {
                Background = cs.BackgroundColor.ToBrush();
                BorderBrush = cs.BorderColor.ToBrush();
                HighlightBrush = cs.HighlightColor.ToBrush();
                DisabledBrush = cs.LightDisabledColor.ToBrush();
                BorderDisabledBrush = cs.BorderColor.ToBrush();
                ClickBrush = cs.HighlightColor.ToBrush();
                SelectedBrush = cs.HighlightColor.ToBrush();
                Foreground = cs.ForegroundColor.ToBrush();
            }
            else
            {
                Background = cs.LightBackgroundColor.ToBrush();
                BorderBrush = cs.BorderColor.ToBrush();
                HighlightBrush = cs.ThirdHighlightColor.ToBrush();
                DisabledBrush = cs.LightDisabledColor.ToBrush();
                BorderDisabledBrush = cs.DarkDisabledColor.ToBrush();
                ClickBrush = cs.SecondHighlightColor.ToBrush();
                SelectedBrush = cs.SecondaryColor.ToBrush();

                if (useLighterBorder || UseLighterBorder)
                {
                    BorderBrush = cs.HighlightColor.ToBrush();
                }
            }

            CheckEnabled();
            UpdateChildrenAppearance();

            runApply = true;
        }

        //public void ApplyColorScheme(HighContrastOption hco)
        //{
        //    ColorScheme cs = ColorScheme.GetHighContrastScheme(hco);

        //    Background = cs.BackgroundColor.ToBrush();
        //    BorderBrush = cs.BorderColor.ToBrush();
        //    HighlightBrush = cs.HighlightColor.ToBrush();
        //    DisabledBrush = cs.LightDisabledColor.ToBrush();
        //    BorderDisabledBrush = cs.BorderColor.ToBrush();
        //    ClickBrush = cs.HighlightColor.ToBrush();
        //    SelectedBrush = cs.HighlightColor.ToBrush();

        //    CheckEnabled();
        //    UpdateChildrenAppearance();
        //}
        #endregion

        private void control_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            CheckEnabled();
        }

        void CheckEnabled()
        {
            if (IsEnabled)
            {
                brdr.BorderBrush = BorderBrush;
                brdr.Background = Background;
            }
            else
            {
                brdr.BorderBrush = BorderBrush;
                brdr.Background = DisabledBrush;
            }
        }

        void UpdateChildrenAppearance()
        {
#if NETCOREAPP
            foreach (UIElement? item in stkItems.Children)
#else
            foreach (UIElement item in stkItems.Children)
#endif
            {
                if (item is SelectableUserControl s)
                {
                    s.HighlightBrush = HighlightBrush;
                    s.ClickBrush = ClickBrush;
                    s.SelectedBrush = SelectedBrush;
                    s.UpdateBrushes();
                    s.ApplyColorScheme(ColorScheme);
                }
            }
        }

        #endregion

        #region ScrollViewer

        public static DependencyProperty AllowParentScrollingProperty = DependencyProperty.Register(
            "AllowParentScrolling", typeof(bool), typeof(SelectPanel),
            new PropertyMetadata(true));

        /// <summary>
        /// Set whether the SelectPanel should allow its parent to scroll if the SelectPanel doesn't need to scroll. Note that enabling this may disable any child items from scrolling.
        /// </summary>
        public bool AllowParentScrolling
        {
            get => (bool)GetValue(AllowParentScrollingProperty);
            set => SetValue(AllowParentScrollingProperty, value);
        }

        // source: https://serialseb.com/blog/2007/09/03/wpf-tips-6-preventing-scrollviewer-from/
        // author: Sebastien Lambla

        private static List<MouseWheelEventArgs> _reentrantList = new List<MouseWheelEventArgs>();

        private void HandlePreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (AllowParentScrolling)
            {
                if (sender is ScrollViewer scrollControl)
                {
                    if (!e.Handled && sender != null && !_reentrantList.Contains(e))
                    {
                        var previewEventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
                        {
                            RoutedEvent = PreviewMouseWheelEvent,
                            Source = sender
                        };

                        var originalSource = (e.OriginalSource as UIElement) ?? null;
                        _reentrantList.Add(previewEventArg);
                        originalSource?.RaiseEvent(previewEventArg);
                        _reentrantList.Remove(previewEventArg);
                        // at this point if no one else handled the event in our children, we do our job

                        if (!previewEventArg.Handled && ((e.Delta > 0 && scrollControl.VerticalOffset == 0) || (e.Delta <= 0 && scrollControl.VerticalOffset >= scrollControl.ExtentHeight - scrollControl.ViewportHeight)))
                        {
                            e.Handled = true;
                            var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
                            eventArg.RoutedEvent = MouseWheelEvent;
                            eventArg.Source = sender;
                            var parent = (((Control)sender).Parent as UIElement) ?? null;
                            parent?.RaiseEvent(eventArg);
                        }
                    }
                }
            }
        }

        #endregion

    }
}
