using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace SolidShineUi
{
    /// <summary>
    /// A ListBox-like control that can be used to select and interact with multiple items, with extra functionality and a visual style that matches the rest of the Solid Shine UI controls.
    /// </summary>
    [ContentProperty("Items")]
    public class SelectPanel : Control
    {
        static SelectPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SelectPanel), new FrameworkPropertyMetadata(typeof(SelectPanel)));
        }

        /// <summary>
        /// Create a new NewSelectPanel.
        /// </summary>
        public SelectPanel()
        {
            Loaded += SelectPanel_Loaded;
            SetValue(ItemsPropertyKey, new SelectableCollection<SelectableUserControl>());
            SetValue(ItemsSourceProperty, Items);
            //Items = new SelectableCollection<SelectableUserControl>();
            //Items.ItemRemoving += Items_ItemRemoving;
            //Items.CollectionChanged += Items_CollectionChanged;
            //Items.SelectionChanged += Items_SelectionChanged;
        }

        private void SelectPanel_Loaded(object sender, RoutedEventArgs e)
        {
            LoadTemplateItems();
            if (itemsLoaded && sv != null)
            {
                sv.PreviewMouseWheel += HandlePreviewMouseWheel;
                sv.Unloaded += (s, _) => sv.PreviewMouseWheel -= HandlePreviewMouseWheel;
            }
        }

        bool itemsLoaded = false;

#if NETCOREAPP
        ItemsControl? ic = null;
        ScrollViewer? sv = null;
#else
        ItemsControl ic = null;
        ScrollViewer sv = null;
#endif

        private void LoadTemplateItems()
        {
            if (!itemsLoaded)
            {
                ic = (ItemsControl)GetTemplateChild("PART_Ic");
                sv = (ScrollViewer)GetTemplateChild("PART_Sv");

                if (ic != null && sv != null)
                {
                    itemsLoaded = true;
                }
            }
        }

        #region ItemsSource

        /// <summary>
        /// Gets or sets a collection used to generate the content of this SelectPanel.
        /// </summary>
        /// <remarks>
        /// It is recommended to set this property to an <see cref="ObservableCollection{T}"/>, <see cref="SelectableCollection{T}"/>, or <see cref="SelectableCollectionView{T}"/>.
        /// If you use other IEnumerable types that do not also implement <see cref="INotifyCollectionChanged"/>, then this control's contents will not update automatically.
        /// If you do not use a SelectableCollection or SelectableCollectionView, you may also need to implement your own code for handling the selection state of the items
        /// in your collection.
        /// </remarks>
        public IEnumerable<SelectableUserControl> ItemsSource
        {
            get { return (IEnumerable<SelectableUserControl>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable<SelectableUserControl>), typeof(SelectPanel), 
                new PropertyMetadata(new PropertyChangedCallback(OnInternalItemsSourceChanged)));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        private static void OnInternalItemsSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is SelectPanel sp)
            {
                sp.SelectPanel_InternalOnItemsSourceChanged((IEnumerable<SelectableUserControl>)e.OldValue, (IEnumerable<SelectableUserControl>)e.NewValue);
            }
        }

        private void SelectPanel_InternalOnItemsSourceChanged(IEnumerable<SelectableUserControl> oldValue, IEnumerable<SelectableUserControl> newValue)
        {
            if (ItemsSource == null)
            {
                ItemsSource = Items;
            }
            else if (ItemsSource == Items)
            {
                RemoveOldCollectionHandlers();

                Items.CollectionChanged += Items_CollectionChanged;
                Items.SelectionChanged += Items_SelectionChanged;

                AddNewCollectionHandlers();
            }
            else
            {
                RemoveOldCollectionHandlers();

                // Remove handler for oldValue.CollectionChanged
                if (oldValue is INotifyCollectionChanged oldColl)
                {
                    oldColl.CollectionChanged -= Items_CollectionChanged;
                }

                if (oldValue is ISelectableCollectionSource<SelectableUserControl> oldSc)
                {
                    oldSc.SelectionChanged -= Items_SelectionChanged;
                }
                
                // Add handler for newValue.CollectionChanged (if possible)
                if (newValue is INotifyCollectionChanged newColl)
                {
                    newColl.CollectionChanged += Items_CollectionChanged;
                }

                if (newValue is ISelectableCollectionSource<SelectableUserControl> newSc)
                {
                    newSc.SelectionChanged += Items_SelectionChanged;
                    RefreshVisualSelection();
                }

                AddNewCollectionHandlers();
            }

            void RemoveOldCollectionHandlers()
            {
                if (oldValue == null) return;
                foreach (SelectableUserControl item in oldValue)
                {
                    item.SelectionChanged -= Item_SelectionChanged;
                }
            }

            void AddNewCollectionHandlers()
            {
                foreach (SelectableUserControl item in newValue)
                {
                    AddItemInternal(item);
                    //item.SelectionChanged += Item_SelectionChanged;
                }
            }
        }

        #endregion

        #region SelectableCollection handling

        bool _internalAction = false;

        //public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register("Items", typeof(SelectableCollection<SelectableUserControl>), typeof(NewSelectPanel),
        //    new FrameworkPropertyMetadata(new SelectableCollection<SelectableUserControl>()));

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        private static readonly DependencyPropertyKey ItemsPropertyKey
            = DependencyProperty.RegisterReadOnly("Items", typeof(SelectableCollection<SelectableUserControl>), typeof(SelectPanel),
            new FrameworkPropertyMetadata(new SelectableCollection<SelectableUserControl>()));

        public static readonly DependencyProperty ItemsProperty = ItemsPropertyKey.DependencyProperty;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Get or set the list of items in this SelectPanel. This Items property can be used to add items, remove items, and also select items via the Select method.
        /// </summary>
        [Category("Common")]
        public SelectableCollection<SelectableUserControl> Items
        {
            get { return (SelectableCollection<SelectableUserControl>)GetValue(ItemsProperty); }
            private set { SetValue(ItemsPropertyKey, value); }
        }

        /// <summary>
        /// Get or set if multiple items can be selected at once. If false, then only 1 item can be selected at a time.
        /// </summary>
        [Category("Common")]
        public bool MultiSelect
        {
            get => Items.CanSelectMultiple;
            set => Items.CanSelectMultiple = value;
        }

        void RefreshVisualSelection()
        {
            _internalAction = true;

            if (ItemsSource is ISelectableCollectionSource<SelectableUserControl> isl)
            {
                foreach (SelectableUserControl item in ItemsSource)
                {
                    item.IsSelected = isl.IsSelected(item);
                }
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
                        foreach (SelectableUserControl? item in e.OldItems)
                        {
                            if (item != null)
                            {
                                item.SelectionChanged -= Item_SelectionChanged;
                            }
                        }
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
                        foreach (SelectableUserControl item in e.OldItems)
                        {
                            if (item != null)
                            {
                                item.SelectionChanged -= Item_SelectionChanged;
                            }
                        }
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

                if (ItemsSource is ISelectableCollectionSource<SelectableUserControl> isl)
                {

                    if (item.IsSelected && !isl.IsSelected(item))
                    {
                        if (isl is SelectableCollection<SelectableUserControl> isel && isel.CanSelectMultiple && Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
                        {
                            isl.AddToSelection(item);
                        }
                        else
                        {
                            isl.Select(item);
                        }

                        RaiseSelectionChangedEvent((new[] { item }).ToList(), new List<SelectableUserControl>());
                    }
                    else
                    {
                        isl.Deselect(item);

                        RaiseSelectionChangedEvent(new List<SelectableUserControl>(), (new[] { item }).ToList());
                    }
                }

                RefreshVisualSelection();

                _internalAction = false;
            }
            //throw new NotImplementedException();
        }

        private void Items_SelectionChanged(object sender, CollectionSelectionChangedEventArgs e)
        {
            //LoadTemplateItems();
            if (_internalAction) return;

            RefreshVisualSelection();

            RaiseSelectionChangedEvent(e.AddedItems.OfType<SelectableUserControl>().ToList(), e.RemovedItems.OfType<SelectableUserControl>().ToList());
        }
        #endregion

        #region Color Scheme

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly DependencyProperty ColorSchemeProperty
            = DependencyProperty.Register("ColorScheme", typeof(ColorScheme), typeof(SelectPanel),
            new FrameworkPropertyMetadata(new ColorScheme(), new PropertyChangedCallback(OnColorSchemeChanged)));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Perform an action when the ColorScheme property has changed. Primarily used internally.
        /// </summary>
        /// <param name="d">The object containing the property that changed.</param>
        /// <param name="e">Event arguments about the property change.</param>
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
        /// Raised when the ColorScheme property is changed.
        /// </summary>
#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? ColorSchemeChanged;
#else
        public event DependencyPropertyChangedEventHandler ColorSchemeChanged;
#endif

        /// <summary>
        /// This field is not meant to be public. This will be hidden in a future release. Please use the UseLighterBorder property.
        /// </summary>
        [Obsolete("This field is not meant to be public. This will be hidden in a future release. Please use the UseLighterBorder property.")]
        public bool use_lbrdr = false;
        bool runApply = true;

        /// <summary>
        /// Use a lighter border color when applying color schemes. Note that this does not apply in high-contrast mode,
        /// nor does it apply if you are not using color schemes to set this control's appearance.
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

        /// <summary>
        /// Apply a color scheme to this control. The color scheme can quickly apply a whole visual style to the control.
        /// </summary>
        /// <param name="cs">The color scheme to apply.</param>
        /// <param name="useLighterBorder">Set if a lighter border color should be used.</param>
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
            foreach (SelectableUserControl item in ItemsSource)
            {
                item.SelectedBrush = SelectedBrush;
                item.HighlightBrush = HighlightBrush;
                item.ClickBrush = ClickBrush;
                item.ApplyColorScheme(ColorScheme);
            }
        }
        #endregion

        #region Routed Events

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly RoutedEvent SelectionChangedEvent = EventManager.RegisterRoutedEvent(
            "SelectionChanged", RoutingStrategy.Bubble, typeof(SelectionChangedEventHandler), typeof(SelectPanel));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Raised when an item is selected or deselected in this list.
        /// </summary>
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

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly RoutedEvent ItemsAddedEvent = EventManager.RegisterRoutedEvent(
            "ItemsAdded", RoutingStrategy.Bubble, typeof(SelectionChangedEventHandler), typeof(SelectPanel));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Raised when an item is added to this list.
        /// </summary>
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

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly RoutedEvent ItemsRemovedEvent = EventManager.RegisterRoutedEvent(
            "ItemsRemoved", RoutingStrategy.Bubble, typeof(SelectionChangedEventHandler), typeof(SelectPanel));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Raised when an item is removed from this list.
        /// </summary>
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

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly DependencyProperty HorizontalScrollBarVisibilityProperty
            = DependencyProperty.Register("HorizontalScrollBarVisibility", typeof(ScrollBarVisibility), typeof(SelectPanel),
            new FrameworkPropertyMetadata(ScrollBarVisibility.Disabled));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Get or set the appearance of the horizontal scroll bar for this control.
        /// </summary>
        [Category("Layout")]
        public ScrollBarVisibility HorizontalScrollBarVisibility
        {
            get { return (ScrollBarVisibility)GetValue(HorizontalScrollBarVisibilityProperty); }
            set { SetValue(HorizontalScrollBarVisibilityProperty, value); }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly DependencyProperty VerticalScrollBarVisibilityProperty
            = DependencyProperty.Register("VerticalScrollBarVisibility", typeof(ScrollBarVisibility), typeof(SelectPanel),
            new FrameworkPropertyMetadata(ScrollBarVisibility.Auto));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Get or set the appearance of the vertical scroll bar for this control.
        /// </summary>
        [Category("Layout")]
        public ScrollBarVisibility VerticalScrollBarVisibility
        {
            get { return (ScrollBarVisibility)GetValue(VerticalScrollBarVisibilityProperty); }
            set { SetValue(VerticalScrollBarVisibilityProperty, value); }
        }

        #region Brushes

        /// <summary>
        /// Get or set the brush used for the background of this control.
        /// </summary>
        [Category("Brushes")]
        public new Brush Background
        {
            get => (Brush)GetValue(BackgroundProperty);
            set => SetValue(BackgroundProperty, value);
        }

        /// <summary>
        /// Get or set the brush used when an item in this control is being clicked.
        /// </summary>
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

        /// <summary>
        /// Get or set the brush used when an item in this control is selected.
        /// </summary>
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

        /// <summary>
        /// Get or set the brush used when an item in this control is highlighted (i.e. has the mouse over it or has keyboard focus).
        /// </summary>
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

        /// <summary>
        /// Get or set the brush used for the background when this control is disabled.
        /// </summary>
        [Category("Brushes")]
        public Brush DisabledBrush
        {
            get => (Brush)GetValue(DisabledBrushProperty);
            set => SetValue(DisabledBrushProperty, value);
        }

        /// <summary>
        /// Get or set the brush used for the border when this control is disabled.
        /// </summary>
        [Category("Brushes")]
        public Brush BorderDisabledBrush
        {
            get => (Brush)GetValue(BorderDisabledBrushProperty);
            set => SetValue(BorderDisabledBrushProperty, value);
        }

        /// <summary>
        /// Get or set the brush used for the border around this control.
        /// </summary>
        [Category("Brushes")]
        public new Brush BorderBrush
        {
            get => (Brush)GetValue(BorderBrushProperty);
            set => SetValue(BorderBrushProperty, value);
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public new static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register(
            "Background", typeof(Brush), typeof(SelectPanel),
            new PropertyMetadata(new SolidColorBrush(ColorsHelper.White)));

        public static readonly DependencyProperty ClickBrushProperty = DependencyProperty.Register(
            "ClickBrush", typeof(Brush), typeof(SelectPanel),
            new PropertyMetadata(Colors.LightSalmon.ToBrush()));

        public static readonly DependencyProperty SelectedBrushProperty = DependencyProperty.Register(
            "SelectedBrush", typeof(Brush), typeof(SelectPanel),
            new PropertyMetadata(Colors.MistyRose.ToBrush()));

        public static readonly DependencyProperty HighlightBrushProperty = DependencyProperty.Register(
            "HighlightBrush", typeof(Brush), typeof(SelectPanel),
            new PropertyMetadata(Colors.Salmon.ToBrush()));

        public static readonly DependencyProperty DisabledBrushProperty = DependencyProperty.Register(
            "DisabledBrush", typeof(Brush), typeof(SelectPanel),
            new PropertyMetadata(new SolidColorBrush(Colors.Gray)));

        public static readonly DependencyProperty BorderDisabledBrushProperty = DependencyProperty.Register(
            "BorderDisabledBrush", typeof(Brush), typeof(SelectPanel),
            new PropertyMetadata(new SolidColorBrush(Colors.DarkGray)));

        public static readonly new DependencyProperty BorderBrushProperty = DependencyProperty.Register(
            "BorderBrush", typeof(Brush), typeof(SelectPanel),
            new PropertyMetadata(new SolidColorBrush(Colors.Black)));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        #endregion

        #region Border

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public new static readonly DependencyProperty BorderThicknessProperty = DependencyProperty.Register(
            "BorderThickness", typeof(Thickness), typeof(SelectPanel),
            new PropertyMetadata(new Thickness(1)));

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            "CornerRadius", typeof(CornerRadius), typeof(SelectPanel),
            new PropertyMetadata(new CornerRadius(0)));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Get or set the thickness of the border around this control.
        /// </summary>
        [Category("Appearance")]
        public new Thickness BorderThickness
        {
            get => (Thickness)GetValue(BorderThicknessProperty);
            set => SetValue(BorderThicknessProperty, value);
        }

        /// <summary>
        /// Get or set the corner radius (or radii) to use for the control's border. Setting the corners to 0 means there is no rounding; square corners are used.
        /// Any corners with a higher number will be rounded.
        /// </summary>
        [Category("Appearance")]
        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        #endregion

        #endregion

        #region Convenience Methods (to remove later)

        /// <summary>
        /// Gets the number of items in this SelectPanel.
        /// Note that this function will be removed in a future version. Please instead use <c>Items.Count</c>.
        /// </summary>
        [Obsolete("This will be removed in a future version. You can instead use \"Items.Count\".", false)]
        public int Count { get => Items.Count; }

        /// <summary>
        /// Gets the number of items that are currently selected.
        /// Note that this function will be removed in a future version. Please instead use <c>Items.SelectedItems.Count</c>.
        /// </summary>
        [Obsolete("This will be removed in a future version. You can instead use \"Items.SelectedItems.Count\".", false)]
        public int SelectionCount { get => Items.SelectedItems.Count; }

        /// <summary>
        /// Get a collection of items that have been selected, returned as a certain type (that inherits from SelectableUserControl).
        /// Note that this function will be removed in a future version. Please instead use <c>Items.SelectedItems.OfType</c> (Linq).
        /// </summary>
        /// <typeparam name="T">The type to return the selected items as. It must inherit from SelectableUserControl.</typeparam>
        [Obsolete("This will be removed in a future version. You can instead use the Linq method \"Items.SelectedItems.OfType<>\".", false)]
        public IEnumerable<T> GetSelectedItemsOfType<T>() where T : SelectableUserControl
        {
            return Items.SelectedItems.OfType<T>();
        }

        /// <summary>
        /// Select all items in this SelectPanel. Note that this function will be removed in a future version. Please instead use <c>Items.SelectAll</c>.
        /// </summary>
        [Obsolete("This will be removed in a future version. You can instead use \"Items.SelectAll\".", false)]
        public void SelectAll()
        {
            Items.SelectAll();
        }

        /// <summary>
        /// Deselect all items in this SelectPanel. Note that this function will be removed in a future version. Please instead use <c>Items.ClearSelection</c>.
        /// </summary>
        [Obsolete("This will be removed in a future version. You can instead use \"Items.ClearSelection\".", false)]
        public void DeselectAll()
        {
            Items.ClearSelection();
        }

        /// <summary>
        /// Add an item to this SelectPanel. Note that this function will be removed in a future version. Please instead use <c>Items.Add</c>.
        /// </summary>
        /// <param name="item">Item to be added.</param>
        [Obsolete("This will be removed in a future version. You can instead use a for or foreach loop around \"Items.Add\".", false)]
        public void AddItem(SelectableUserControl item)
        {
            Items.Add(item);
            //RaiseItemsAddedEvent(items.ToList());
        }

        /// <summary>
        /// Add a collection of items to this SelectPanel. Note that this function will be removed in a future version. Please instead use <c>Items.Add</c>.
        /// </summary>
        /// <param name="items">The items to be added.</param>
        [Obsolete("This will be removed in a future version. You can instead use a for or foreach loop around \"Items.Add\".", false)]
        public void AddItems(IEnumerable<SelectableUserControl> items)
        {
            foreach (SelectableUserControl item in items)
            {
                Items.Add(item);
            }
            //RaiseItemsAddedEvent(items.ToList());
        }

        /// <summary>
        /// Insert an item into this SelectPanel. Note that this function will be removed in a future version. Please instead use <c>Items.Insert</c>.
        /// </summary>
        /// <param name="index">The index to insert the item at.</param>
        /// <param name="item">The item to insert.</param>
        [Obsolete("This will be removed in a future version. You can instead use \"Items.Insert\".", false)]
        public void InsertItem(int index, SelectableUserControl item)
        {
            Items.Insert(index, item);
            //RaiseItemsAddedEvent(new List<SelectableUserControl>() { item });
        }

        /// <summary>
        /// Insert a collection of items into this SelectPanel. Note that this function will be removed in a future version. Please instead use <c>Items.Insert</c>.
        /// </summary>
        /// <param name="index">The index to insert the items at.</param>
        /// <param name="items">The items to insert.</param>
        [Obsolete("This will be removed in a future version. You can instead use a for or foreach loop around \"Items.Insert\".", false)]
        public void InsertItems(int index, IEnumerable<SelectableUserControl> items)
        {
            List<SelectableUserControl> litems = items.ToList();
            litems.Reverse();

            foreach (SelectableUserControl item in litems)
            {
                Items.Insert(index, item);
            }

            //RaiseItemsAddedEvent(litems);
        }

        /// <summary>
        /// Get all items in the SelectPanel that match the specified type. Note that this function will be removed in a future version. Please instead use <c>Items.OfType</c> (Linq).
        /// </summary>
        /// <typeparam name="T">The type to filter the SelectPanel items for.</typeparam>
        /// <returns></returns>
        [Obsolete("This will be removed in a future version. You can instead use the Linq method \"Items.OfType<>\".", false)]
        public IEnumerable<T> GetItemsAsType<T>() where T : SelectableUserControl
        {
            return Items.OfType<T>();
            //foreach (SelectableUserControl item in Items)
            //{
            //    if (item is T t)
            //    {
            //        yield return t;
            //    }
            //}
        }

        /// <summary>
        /// Remove an item from this SelectPanel. Note that this function will be removed in a future version. Please instead use <c>Items.Remove</c>.
        /// </summary>
        /// <param name="item">The item to be removed.</param>
        [Obsolete("This will be removed in a future version. You can instead use \"Items.Remove\".", false)]
        public void RemoveItem(SelectableUserControl item)
        {
            Items.Remove(item);
        }

        /// <summary>
        /// Remove multiple items from this SelectPanel. Note that this function will be removed in a future version. Please instead use <c>Items.Remove</c>.
        /// </summary>
        /// <param name="items">The items to be removed.</param>
        [Obsolete("This will be removed in a future version. You can instead use a for or foreach loop around \"Items.Remove\".", false)]
        public void RemoveItems(IEnumerable<SelectableUserControl> items)
        {
            foreach (var item in items)
            {
                Items.Remove(item);
            }

            //RaiseItemsRemovedEvent(items.ToList());
        }

        /// <summary>
        /// Remove an item at a specified index from this SelectPanel. Note that this function will be removed in a future version. Please instead use <c>Items.RemoveAt</c>.
        /// </summary>
        /// <param name="index">The index of the item to be removed.</param>
        [Obsolete("This will be removed in a future version. You can instead use \"Items.RemoveAt\".", false)]
        public void RemoveAt(int index)
        {
            Items.RemoveAt(index);
        }

        /// <summary>
        /// Remove the currently selected items from the list.
        /// </summary>
        /// <remarks>
        /// Note: unlike many other functions, this one is not being removed in a near future version.
        /// </remarks>
        public void RemoveSelectedItems()
        {
            var items = new List<SelectableUserControl>(Items.SelectedItems);
            foreach (var item in items)
            {
                Items.Remove(item);
            }
        }

        /// <summary>
        /// Get the index of an item in this SelectPanel. Note that this function will be removed in a future version. Please instead use <c>Items.IndexOf</c>.
        /// </summary>
        /// <param name="item">The item to get the index of.</param>
        [Obsolete("This will be removed in a future version. You can instead use \"Items.IndexOf\".", false)]
        public int IndexOf(SelectableUserControl item)
        {
            return Items.IndexOf(item);
        }

        /// <summary>
        /// Get an item at a specified index in this SelectPanel. Note that this function will be removed in a future version. Please instead use <c>Items[]</c>.
        /// </summary>
        /// <param name="index">The index of the item to be removed.</param>
        [Obsolete("This will be removed in a future version. You can instead use \"Items[]\".", false)]
        public SelectableUserControl Get(int index)
        {
            return Items[index];
        }

        /// <summary>
        /// Get an item at a specified index in this SelectPanel. Note that this function will be removed in a future version. Please instead use <c>Items[]</c>.
        /// </summary>
        /// <param name="index">The index of the item to be removed.</param>
        [Obsolete("This will be removed in a future version. You can instead use \"Items[]\".", false)]
        public SelectableUserControl this[int index]
        {
            get
            {
                return Items[index];
            }
        }

        /// <summary>
        /// Clear all items in this SelectPanel. Note that this function will be removed in a future version. Please instead use <c>Items.Clear</c>.
        /// </summary>
        [Obsolete("This will be removed in a future version. You can instead use \"Items.Clear\".", false)]
        public void Clear()
        {
            Items.Clear();
        }

        #endregion

        #region Move Items

        private class SortByParentIndex : IComparer<SelectableUserControl>
        {
            // A class to sort a collection of SelectableUserControls by their index in the parent SelectableCollection.

#if NETCOREAPP
            public IList<SelectableUserControl>? ParentCollection { get; set; }

            public int Compare(SelectableUserControl? a, SelectableUserControl? b)
#else
            public SelectableCollection<SelectableUserControl> ParentCollection { get; set; }

            public int Compare(SelectableUserControl a, SelectableUserControl b)
#endif
            {
                // do null checks first
                // I don't think we'll run into a situation where these will actually be null, but better safe than sorry
                if (a == null)
                {
                    if (b == null) return 0;
                    else
                    {
                        return -1;
                    }
                }
                else if (b == null)
                {
                    return 1;
                }
                else
                {
                    if (ParentCollection == null)
                    {
                        // no sorting can be done
                        return 0;
                    }
                    else
                    {
                        if (ParentCollection.IndexOf(a) > ParentCollection.IndexOf(b))
                        {
                            return 1;
                        }
                        else if (ParentCollection.IndexOf(a) < ParentCollection.IndexOf(b))
                        {
                            return -1;
                        }
                        else { return 0; }
                    }
                }
            }
        }

        /// <summary>
        /// Move the currently selected items up by one in the list.
        /// </summary>
        public void MoveSelectedItemsUp()
        {
            if (Items.SelectedItems.Count == 0 || ItemsSource != Items) return;

            if (Items.SelectedItems.Count == 1)
            {
                MoveItemUp(Items.IndexOf(Items.SelectedItems[0]));
                return;
            }

            // set up variables
            int index = int.MaxValue;

            List<SelectableUserControl> imov = new List<SelectableUserControl>();

            foreach (SelectableUserControl item in Items.SelectedItems)
            {
                if (Items.IndexOf(item) < index) index = Items.IndexOf(item);
                imov.Add(item);
            }

            imov.Sort(new SortByParentIndex() { ParentCollection = Items });

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

            imov.Reverse();

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

        /// <summary>
        /// Move the currently selected items down by one in the list.
        /// </summary>
        /// <remarks>
        /// This function does not operate if the ItemsSource is set to a different collection.
        /// </remarks>
        public void MoveSelectedItemsDown()
        {
            if (Items.SelectedItems.Count == 0 || ItemsSource != Items) return;

            int index = int.MaxValue;

            List<SelectableUserControl> imov = new List<SelectableUserControl>();

            foreach (SelectableUserControl item in Items.SelectedItems)
            {
                if (Items.IndexOf(item) < index) index = Items.IndexOf(item);

                imov.Add(item);
            }

            if (index == int.MaxValue) index = Items.Count;
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

        /// <summary>
        /// Move an item in the SelectPanel up by one position in the list.
        /// </summary>
        /// <param name="index">The index of the item to move.</param>
        /// <remarks>
        /// If the index passed in is 0, then nothing is moved, as it's already at the top of the list.
        /// This function does not operate if the ItemsSource is set to a different collection.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the index passed in is beyond the number of items in the collection.</exception>
        public void MoveItemUp(int index)
        {
            if (ItemsSource != Items) return;

            SelectableUserControl suc = Items[index];

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

        /// <summary>
        /// Move an item in the SelectPanel down by one position in the list.
        /// </summary>
        /// <param name="index">The index of the item to move.</param>
        /// <remarks>
        /// If the index passed in is the last index in the collection, then nothing is moved, as it's already at the bottom of the list.
        /// This function does not operate if the ItemsSource is set to a different collection.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the index passed in is beyond the number of items in the collection.</exception>
        public void MoveItemDown(int index)
        {
            if (ItemsSource != Items) return;

            SelectableUserControl suc = Items[index];

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

        #region ScrollViewer

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly DependencyProperty AllowParentScrollingProperty = DependencyProperty.Register(
            "AllowParentScrolling", typeof(bool), typeof(SelectPanel),
            new PropertyMetadata(true));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

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
