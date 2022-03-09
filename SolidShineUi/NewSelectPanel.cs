using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SolidShineUi
{
    public class NewSelectPanel : Control
    {
        static NewSelectPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NewSelectPanel), new FrameworkPropertyMetadata(typeof(NewSelectPanel)));
        }

        public NewSelectPanel()
        {
            SetValue(ItemsProperty, new SelectableCollection<SelectableUserControl>());
            //Items = new SelectableCollection<SelectableUserControl>();
            Items.ItemRemoving += Items_ItemRemoving;
            Items.CollectionChanged += Items_CollectionChanged;
            Items.SelectionChanged += Items_SelectionChanged;
        }

        bool itemsLoaded = false;

#if NETCOREAPP
        ItemsControl? ic = null;
#else
        ItemsControl ic = null;
#endif

        private void LoadTemplateItems()
        {
            // TODO: if needed, locate and load in any needed elements from the control template
            if (!itemsLoaded)
            {
                ic = (ItemsControl)GetTemplateChild("PART_Ic");

                if (ic != null)
                {
                    itemsLoaded = true;
                }
            }
        }

        #region SelectableCollection handling

        bool _internalAction = false;

        //private static readonly DependencyProperty ItemsProperty
        //    = DependencyProperty.Register("Items", typeof(ObservableCollection<TabItem>), typeof(TabControl),
        //    new FrameworkPropertyMetadata(new ObservableCollection<TabItem>()));

        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register("Items", typeof(SelectableCollection<SelectableUserControl>), typeof(NewSelectPanel),
            new FrameworkPropertyMetadata(new SelectableCollection<SelectableUserControl>()));

        //public static readonly DependencyProperty ItemsProperty = ItemsPropertyKey.DependencyProperty;

        /// <summary>
        /// Get or set the list of items in this SelectPanel. This Items property can be used to add items, remove items, and also select items via the Select method.
        /// </summary>
        [Category("Common")]
        public SelectableCollection<SelectableUserControl> Items
        {
            get { return (SelectableCollection<SelectableUserControl>)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        [Category("Common")]
        public bool MultiSelect
        {
            get => Items.CanSelectMultiple;
            set => Items.CanSelectMultiple = value;
        }

        void RefreshVisualSelection()
        {
            _internalAction = true;

            foreach (SelectableUserControl item in Items)
            {
                item.IsSelected = Items.IsSelected(item);
            }

            _internalAction = false;
        }

#if NETCOREAPP
        private void Items_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems != null)
                    {
                        foreach (SelectableUserControl? item in e.NewItems)
                        {
                            if (item != null)
                            {
                                AddItemInternal(item);
                            }
                        }
                        RaiseItemsAddedEvent(e.NewItems.OfType<SelectableUserControl>().ToList());
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems != null)
                    {
                        RaiseItemsRemovedEvent(e.OldItems.OfType<SelectableUserControl>().ToList());
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (e.OldItems != null)
                    {
                        RaiseItemsRemovedEvent(e.OldItems.OfType<SelectableUserControl>().ToList());
                    }
                    if (e.NewItems != null)
                    {
                        foreach (SelectableUserControl? item in e.NewItems)
                        {
                            if (item != null)
                            {
                                AddItemInternal(item);
                            }
                        }
                        RaiseItemsAddedEvent(e.NewItems.OfType<SelectableUserControl>().ToList());
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    if (e.OldItems != null)
                    {
                        RaiseItemsRemovedEvent(e.OldItems.OfType<SelectableUserControl>().ToList());
                    }
                    //TabsCleared?.Invoke(this, EventArgs.Empty);
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
                    if (e.NewItems != null)
                    {
                        foreach (SelectableUserControl item in e.NewItems)
                        {
                            if (item != null)
                            {
                                AddItemInternal(item);
                            }
                        }
                        RaiseItemsAddedEvent(e.NewItems.OfType<SelectableUserControl>().ToList());
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems != null)
                    {
                        RaiseItemsRemovedEvent(e.OldItems.OfType<SelectableUserControl>().ToList());
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (e.OldItems != null)
                    {
                        RaiseItemsRemovedEvent(e.OldItems.OfType<SelectableUserControl>().ToList());
                    }
                    if (e.NewItems != null)
                    {
                        foreach (SelectableUserControl item in e.NewItems)
                        {
                            if (item != null)
                            {
                                AddItemInternal(item);
                            }
                        }
                        RaiseItemsAddedEvent(e.NewItems.OfType<SelectableUserControl>().ToList());
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    if (e.OldItems != null)
                    {
                        RaiseItemsRemovedEvent(e.OldItems.OfType<SelectableUserControl>().ToList());
                    }
                    //TabsCleared?.Invoke(this, EventArgs.Empty);
                    break;
                default:
                    break;
            }
#endif

            //LoadTemplateItems();
        }

        void AddItemInternal(SelectableUserControl item)
        {
            item.SelectedBrush = SelectedBrush;
            item.HighlightBrush = HighlightBrush;
            item.ClickBrush = ClickBrush;
            item.SelectionChanged += Item_SelectionChanged;
            item.ApplyColorScheme(ColorScheme);
        }

#if NETCOREAPP
        private void Item_SelectionChanged(object? sender, EventArgs e)
#else
        private void Item_SelectionChanged(object sender, EventArgs e)
#endif
        {
            if (_internalAction) return;

            if (sender is SelectableUserControl item)
            {
                _internalAction = true;

                if (item.IsSelected && !Items.IsSelected(item))
                {
                    if (Items.CanSelectMultiple && Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
                    {
                        Items.AddToSelection(item);
                    }
                    else
                    {
                        Items.Select(item);
                    }

                    RaiseSelectionChangedEvent((new[] { item }).ToList(), new List<SelectableUserControl>());
                }
                else
                {
                    Items.Deselect(item);

                    RaiseSelectionChangedEvent(new List<SelectableUserControl>(), (new[] { item }).ToList());
                }

                RefreshVisualSelection();

                _internalAction = false;
            }
            //throw new NotImplementedException();
        }

        private void Items_SelectionChanged(object sender, SelectionChangedEventArgs<SelectableUserControl> e)
        {
            //LoadTemplateItems();
            if (_internalAction) return;

            RefreshVisualSelection();

            RaiseSelectionChangedEvent(e.AddedItems.OfType<SelectableUserControl>().ToList(), e.RemovedItems.OfType<SelectableUserControl>().ToList());
        }

        private void Items_ItemRemoving(object sender, ItemRemovingEventArgs<SelectableUserControl> e)
        {
            // item hasn't been removed yet
            e.Item.SelectionChanged -= Item_SelectionChanged;
        }
        #endregion

        #region Color Scheme

        public static readonly DependencyProperty ColorSchemeProperty
            = DependencyProperty.Register("ColorScheme", typeof(ColorScheme), typeof(NewSelectPanel),
            new FrameworkPropertyMetadata(new ColorScheme(), new PropertyChangedCallback(OnColorSchemeChanged)));

        public static void OnColorSchemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NewSelectPanel sp)
            {
                sp.ColorSchemeChanged?.Invoke(d, e);
#if NETCOREAPP
                sp.ApplyColorScheme((e.NewValue as ColorScheme)!, sp.UseLighterBorder);
#else
                sp.ApplyColorScheme((e.NewValue as ColorScheme), sp.UseLighterBorder);
#endif
            }
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

            UpdateChildrenAppearance();

            runApply = true;
        }

        private void UpdateChildrenAppearance()
        {
            foreach (SelectableUserControl item in Items)
            {
                item.ApplyColorScheme(ColorScheme);
            }
        }
        #endregion

        #region Routed Events

        public static readonly RoutedEvent SelectionChangedEvent = EventManager.RegisterRoutedEvent(
            "SelectionChanged", RoutingStrategy.Bubble, typeof(SelectionChangedEventHandler), typeof(NewSelectPanel));

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
            "ItemsAdded", RoutingStrategy.Bubble, typeof(SelectionChangedEventHandler), typeof(NewSelectPanel));

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
            "ItemsRemoved", RoutingStrategy.Bubble, typeof(SelectionChangedEventHandler), typeof(NewSelectPanel));

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

        #region Visual Properties

        public static readonly DependencyProperty HorizontalScrollBarVisibilityProperty
            = DependencyProperty.Register("HorizontalScrollBarVisibility", typeof(ScrollBarVisibility), typeof(NewSelectPanel),
            new FrameworkPropertyMetadata(ScrollBarVisibility.Disabled));

        [Category("Layout")]
        public ScrollBarVisibility HorizontalScrollBarVisibility
        {
            get { return (ScrollBarVisibility)GetValue(HorizontalScrollBarVisibilityProperty); }
            set { SetValue(HorizontalScrollBarVisibilityProperty, value); }
        }

        public static readonly DependencyProperty VerticalScrollBarVisibilityProperty
            = DependencyProperty.Register("VerticalScrollBarVisibility", typeof(ScrollBarVisibility), typeof(NewSelectPanel),
            new FrameworkPropertyMetadata(ScrollBarVisibility.Auto));

        [Category("Layout")]
        public ScrollBarVisibility VerticalScrollBarVisibility
        {
            get { return (ScrollBarVisibility)GetValue(VerticalScrollBarVisibilityProperty); }
            set { SetValue(VerticalScrollBarVisibilityProperty, value); }
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

        public new static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register(
            "Background", typeof(Brush), typeof(NewSelectPanel),
            new PropertyMetadata(new SolidColorBrush(ColorsHelper.White)));

        public static readonly DependencyProperty ClickBrushProperty = DependencyProperty.Register(
            "ClickBrush", typeof(Brush), typeof(NewSelectPanel),
            new PropertyMetadata(Colors.LightSalmon.ToBrush()));

        public static readonly DependencyProperty SelectedBrushProperty = DependencyProperty.Register(
            "SelectedBrush", typeof(Brush), typeof(NewSelectPanel),
            new PropertyMetadata(Colors.MistyRose.ToBrush()));

        public static readonly DependencyProperty HighlightBrushProperty = DependencyProperty.Register(
            "HighlightBrush", typeof(Brush), typeof(NewSelectPanel),
            new PropertyMetadata(Colors.Salmon.ToBrush()));

        public static readonly DependencyProperty DisabledBrushProperty = DependencyProperty.Register(
            "DisabledBrush", typeof(Brush), typeof(NewSelectPanel),
            new PropertyMetadata(new SolidColorBrush(Colors.Gray)));

        public static readonly DependencyProperty BorderDisabledBrushProperty = DependencyProperty.Register(
            "BorderDisabledBrush", typeof(Brush), typeof(NewSelectPanel),
            new PropertyMetadata(new SolidColorBrush(Colors.DarkGray)));

        public static readonly new DependencyProperty BorderBrushProperty = DependencyProperty.Register(
            "BorderBrush", typeof(Brush), typeof(NewSelectPanel),
            new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        #endregion

        #region Border

        public new static readonly DependencyProperty BorderThicknessProperty = DependencyProperty.Register(
            "BorderThickness", typeof(Thickness), typeof(NewSelectPanel),
            new PropertyMetadata(new Thickness(1)));

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            "CornerRadius", typeof(CornerRadius), typeof(NewSelectPanel),
            new PropertyMetadata(new CornerRadius(0)));

        /// <summary>
        /// Get or set the thickness of the border around the button.
        /// </summary>
        [Category("Appearance")]
        public new Thickness BorderThickness
        {
            get => (Thickness)GetValue(BorderThicknessProperty);
            set => SetValue(BorderThicknessProperty, value);
        }

        /// <summary>
        /// Get or set the corner radius (or radii) to use for the button and its border. Can be used to create a rounded button.
        /// </summary>
        [Category("Appearance")]
        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        #endregion

        #endregion

        #region Convenience Methods

        /// <summary>
        /// Gets the number of items in this NewSelectPanel.
        /// </summary>
        public int Count { get => Items.Count; }

        /// <summary>
        /// Gets the number of items that are currently selected.
        /// </summary>
        public int SelectionCount { get => Items.SelectedItems.Count; }

        /// <summary>
        /// Get a collection of items that have been selected, returned as a certain type (that inherits from SelectableUserControl).
        /// </summary>
        /// <typeparam name="T">The type to return the selected items as. It must inherit from SelectableUserControl.</typeparam>
        /// <returns></returns>
        public IEnumerable<T> GetSelectedItemsOfType<T>() where T : SelectableUserControl
        {
            return Items.SelectedItems.OfType<T>();
        }

        /// <summary>
        /// Select all items in this NewSelectPanel.
        /// </summary>
        public void SelectAll()
        {
            Items.SelectAll();
        }

        /// <summary>
        /// Deselect all items in this NewSelectPanel.
        /// </summary>
        public void DeselectAll()
        {
            Items.ClearSelection();
        }

        public void AddItems(IEnumerable<SelectableUserControl> items)
        {
            foreach (SelectableUserControl item in items)
            {
                // TODO: add internal variable to suppress events
                Items.Add(item);
            }
            RaiseItemsAddedEvent(items.ToList());
        }

        public void InsertItem(int index, SelectableUserControl item)
        {
            Items.Insert(index, item);
            //RaiseItemsAddedEvent(new List<SelectableUserControl>() { item });
        }

        public void InsertItems(int index, IEnumerable<SelectableUserControl> items)
        {
            List<SelectableUserControl> litems = items.ToList();
            litems.Reverse();

            foreach (SelectableUserControl item in litems)
            {
                // TODO: add internal variable to suppress events
                Items.Insert(index, item);
            }

            RaiseItemsAddedEvent(litems);
        }

        public IEnumerable<T> GetItemsAsType<T>() where T : SelectableUserControl
        {
            foreach (SelectableUserControl item in Items)
            {
                if (item is T t)
                {
                    yield return t;
                }
            }
        }

        public void RemoveItem(SelectableUserControl item)
        {
            Items.Remove(item);
        }

        public void RemoveItems(IEnumerable<SelectableUserControl> items)
        {
            foreach (var item in items)
            {
                // TODO: add internal variable to suppress events
                RemoveItem(item);
            }

            RaiseItemsRemovedEvent(items.ToList());
        }

        public void RemoveAt(int index)
        {
            Items.RemoveAt(index);
        }

        public void RemoveSelectedItems()
        {
            var items = new List<SelectableUserControl>(Items.SelectedItems);
            foreach (var item in items)
            {
                // TODO: add internal variable to suppress events
                Items.Remove(item);
            }

            RaiseItemsRemovedEvent(items.ToList());
        }

        public int IndexOf(SelectableUserControl item)
        {
            return Items.IndexOf(item);
        }

        public SelectableUserControl Get(int index)
        {
            return Items[index];
        }

        public SelectableUserControl this[int index]
        {
            get
            {
                return Items[index];
            }
        }

        public void Clear()
        {
            Items.Clear();
        }

        #endregion

        #region Move Items

        public void MoveSelectedItemsUp()
        {
            // set up variables
            int index = int.MaxValue;

            List<SelectableUserControl> imov = new List<SelectableUserControl>();

            foreach (SelectableUserControl item in Items.SelectedItems)
            {
                if (IndexOf(item) < index) index = IndexOf(item);
                imov.Add(item);
            }

            index--;

            if (index < 0 || index == int.MaxValue - 1)
            {
                index = 0;
            }

            // remove the items out of the control, so they can be re-inserted at the new location
            foreach (SelectableUserControl item in imov)
            {
                Items.Remove(item);
            }

            // sometimes the list needs to be reversed, sometimes it doesn't???
            //imov.Reverse();

            Items.ClearSelection();

            // re-insert and re-select all the items
            foreach (SelectableUserControl item in imov)
            {
                Items.Insert(index, item);
                //Items.AddToSelection(item);
            }

            foreach (SelectableUserControl item in imov)
            {
                Items.AddToSelection(item);
            }

            RefreshVisualSelection();
        }

        public void MoveSelectedItemsDown()
        {
            int index = int.MaxValue;

            List<SelectableUserControl> imov = new List<SelectableUserControl>();

            foreach (SelectableUserControl item in Items.SelectedItems)
            {
                if (IndexOf(item) < index) index = IndexOf(item);

                imov.Add(item);
            }

            index++;

            if (index > (Items.Count - 1) || index == int.MinValue + 1)
            {
                index = (Items.Count - 1);
            }

            // remove the controls
            foreach (SelectableUserControl item in imov)
            {
                Items.Remove(item);
            }

            if (index > (Items.Count - 1))
            {
                index = Items.Count;
            }

            Items.ClearSelection();

            // re-insert and re-select the controls at the new location
            foreach (SelectableUserControl item in imov)
            {
                Items.Insert(index, item);
                //Items.AddToSelection(item);
            }

            foreach (SelectableUserControl item in imov)
            {
                Items.AddToSelection(item);
            }

            RefreshVisualSelection();
        }

        public void MoveItemUp(int index)
        {
            SelectableUserControl suc = this[index];

            int moveIndex = index - 1;//IndexOf(suc) - 1;
            if (moveIndex < 0)
            {
                moveIndex = 0;
            }

            bool resel = Items.IsSelected(suc);

            Items.Remove(suc);
            Items.Insert(moveIndex, suc);
            if (resel) Items.AddToOrReplaceSelection(suc);

            RefreshVisualSelection();
        }

        public void MoveItemDown(int index)
        {
            SelectableUserControl suc = this[index];

            int moveIndex = index + 1; //IndexOf(suc) + 1;
            if (moveIndex > (Items.Count - 1))
            {
                moveIndex = (Items.Count - 1);
            }

            bool resel = Items.IsSelected(suc);

            Items.Remove(suc);
            Items.Insert(moveIndex, suc);
            if (resel) Items.AddToOrReplaceSelection(suc);

            RefreshVisualSelection();
        }

        #endregion

    }
}
