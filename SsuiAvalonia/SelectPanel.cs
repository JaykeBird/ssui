using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Metadata;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidShineUi
{
    /// <summary>
    /// A ListBox-like control that can be used to select and interact with multiple items, with extra functionality and a visual style that matches the rest of the Solid Shine UI controls.
    /// </summary>
    public class SelectPanel : TemplatedControl, IAddChild
    {
        // [TemplatePart(true, "PART_Sv", typeof(ScrollViewer))]

        public SelectPanel()
        {
            SetValue(ItemsProperty, new SelectableCollection<IClickSelectableControl>());
            SetValue(ItemsSourceProperty, Items);
        }

        bool _internalAction = false;
        bool runApply = true;
        private bool use_lbrdr = false;

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property.Name == nameof(ItemsSource))
            {
                OnItemsSourceChanged(change.GetOldValue<IEnumerable<IClickSelectableControl>>(), change.GetNewValue<IEnumerable<IClickSelectableControl>>());
            }
            else if (change.Property.Name == nameof(ColorSchemeProperty))
            {
                OnColorSchemeChanged(change);
            }
            else if (change.Property.Name == nameof(UseLighterBorderProperty))
            {
                ApplyColorScheme(ColorScheme, UseLighterBorder);
            }
        }

        #region Template IO

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            LoadTemplateItems(e);
        }

        bool itemsLoaded = false;

        ScrollViewer? sv = null;

        private void LoadTemplateItems(TemplateAppliedEventArgs e)
        {
            if (!itemsLoaded)
            {
                sv = (ScrollViewer?)e.NameScope.Find("PART_Sv");

                if (sv != null)
                {
                    itemsLoaded = true;

                    sv.AddHandler(ScrollViewer.PointerWheelChangedEvent, HandlePreviewMouseWheel, Avalonia.Interactivity.RoutingStrategies.Tunnel, true);
                    sv.Unloaded += (s, _) => sv.RemoveHandler(ScrollViewer.PointerWheelChangedEvent, HandlePreviewMouseWheel);
                }
            }
        }

        #endregion

        #region ItemsSource

        /// <summary>
        /// Gets or sets a collection used to generate the content of this SelectPanel.
        /// </summary>
        /// <remarks>
        /// It is recommended to set this property to an <see cref="ObservableCollection{IClickSelectableControl}"/> or <see cref="SelectableCollection{IClickSelectableControl}"/>.
        /// If you use other IEnumerable types that do not implement <see cref="INotifyCollectionChanged"/>, then this control's contents will not update automatically.
        /// If your collection doesn't support <see cref="ISelectableCollection"/>, you may need to implement your own code for handling the selection state of the items
        /// in your collection (and certain functions in this control, such as <see cref="MultiSelect"/> or <see cref="RemoveSelectedItems"/>, may not work).
        /// </remarks>
        public IEnumerable<IClickSelectableControl> ItemsSource { get => GetValue(ItemsSourceProperty); set => SetValue(ItemsSourceProperty, value); }

        /// <summary>The backing styled property for <see cref="ItemsSource"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IEnumerable<IClickSelectableControl>> ItemsSourceProperty
            = AvaloniaProperty.Register<SelectPanel, IEnumerable<IClickSelectableControl>>(nameof(ItemsSource));

        private void OnItemsSourceChanged(IEnumerable<IClickSelectableControl> oldValue, IEnumerable<IClickSelectableControl> newValue)
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

                if (oldValue is ISelectableCollection oldSc)
                {
                    oldSc.SelectionChanged -= Items_SelectionChanged;
                }

                // Add handler for newValue.CollectionChanged (if possible)
                if (newValue is INotifyCollectionChanged newColl)
                {
                    newColl.CollectionChanged += Items_CollectionChanged;
                }

                if (newValue is ISelectableCollection newSc)
                {
                    newSc.SelectionChanged += Items_SelectionChanged;
                    RefreshVisualSelection();
                }

                AddNewCollectionHandlers();
            }

            void RemoveOldCollectionHandlers()
            {
                if (oldValue == null) return;
                foreach (IClickSelectableControl item in oldValue)
                {
                    item.IsSelectedChanged -= Item_SelectionChanged;
                }
            }

            void AddNewCollectionHandlers()
            {
                foreach (IClickSelectableControl item in newValue)
                {
                    AddItemInternal(item);
                    //item.SelectionChanged += Item_SelectionChanged;
                }
            }
        }

        #endregion

        #region Collection Properties / Methods

        #region ItemsProperty

        private SelectableCollection<IClickSelectableControl> _items = new SelectableCollection<IClickSelectableControl>();

        [Content]
        public SelectableCollection<IClickSelectableControl> Items { get => _items; private set => SetAndRaise(ItemsProperty, ref _items, value); }

        /// <summary>The backing direct property for <see cref="Items"/>. See the related property for details.</summary>
        public static readonly DirectProperty<SelectPanel, SelectableCollection<IClickSelectableControl>> ItemsProperty
            = AvaloniaProperty.RegisterDirect<SelectPanel, SelectableCollection<IClickSelectableControl>>(nameof(Items), (s) => s.Items, unsetValue: new SelectableCollection<IClickSelectableControl>());

        #endregion

        #region MultiSelect

        public bool MultiSelect
        { 
            get
            {
                if (ItemsSource is ISelectableCollection isl)
                {
                    return isl.CanSelectMultiple;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                if (ItemsSource is ISelectableCollection isl)
                {
                    //isl.CanSelectMultiple = value;
                    bool _multiple = false;
                    SetAndRaise(MultiSelectProperty, ref _multiple, value);
                    isl.CanSelectMultiple = _multiple;
                }
            }
        }

        /// <summary>The backing direct property for <see cref="MultiSelect"/>. See the related property for details.</summary>
        public static readonly DirectProperty<SelectPanel, bool> MultiSelectProperty
            = AvaloniaProperty.RegisterDirect<SelectPanel, bool>(nameof(MultiSelect), (s) => s.MultiSelect, (s, v) => s.MultiSelect = v, unsetValue: false);

        #endregion

        #region Helper Methods

        void RefreshVisualSelection()
        {
            _internalAction = true;

            if (ItemsSource is ISelectableCollection isl)
            {
                foreach (var item in ItemsSource)
                {
                    item?.SetIsSelectedWithSource(isl.IsSelected(item), SelectionChangeTrigger.Parent, this);
                }
            }

            _internalAction = false;
        }

        void AddItemInternal(IClickSelectableControl item)
        {
            item.SelectedBrush = SelectedBrush;
            item.HighlightBrush = HighlightBrush;
            item.ClickBrush = ClickBrush;
            item.IsSelectedChanged += Item_SelectionChanged;
            item.ApplyColorScheme(ColorScheme);
        }

        /// <inheritdoc/>
        public void AddChild(object child)
        {
            if (child is IClickSelectableControl icl)
            {
                Items.Add(icl);
            }
        }

        #endregion

        #region CollectionChanged / CollectionSelectionChanged

        private void Items_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems != null)
                    {
                        foreach (IClickSelectableControl? item in e.NewItems)
                        {
                            if (item != null)
                            {
                                AddItemInternal(item);
                            }
                        }
                        RaiseItemsAddedEvent(e.NewItems.OfType<IClickSelectableControl>().ToList());
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems != null)
                    {
                        foreach (IClickSelectableControl? item in e.OldItems)
                        {
                            if (item != null)
                            {
                                item.IsSelectedChanged -= Item_SelectionChanged;
                            }
                        }
                        RaiseItemsRemovedEvent(e.OldItems.OfType<IClickSelectableControl>().ToList());
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (e.OldItems != null)
                    {
                        RaiseItemsRemovedEvent(e.OldItems.OfType<IClickSelectableControl>().ToList());
                    }
                    if (e.NewItems != null)
                    {
                        foreach (IClickSelectableControl? item in e.NewItems)
                        {
                            if (item != null)
                            {
                                AddItemInternal(item);
                            }
                        }
                        RaiseItemsAddedEvent(e.NewItems.OfType<IClickSelectableControl>().ToList());
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    if (e.OldItems != null)
                    {
                        RaiseItemsRemovedEvent(e.OldItems.OfType<IClickSelectableControl>().ToList());
                    }
                    //TabsCleared?.Invoke(this, EventArgs.Empty);
                    break;
                default:
                    break;
            }
        }

        private void Items_SelectionChanged(object sender, CollectionSelectionChangedEventArgs e)
        {
            if (_internalAction) return;

            RefreshVisualSelection();

            RaiseSelectionChangedEvent(e.AddedItems.OfType<IClickSelectableControl>().ToList(), e.RemovedItems.OfType<IClickSelectableControl>().ToList());
        }

        #endregion

        #region Item Selection Changed

        private void Item_SelectionChanged(object? sender, ItemSelectionChangedEventArgs e)
        {
            if (_internalAction) return;

            if (sender is IClickSelectableControl item)
            {
                _internalAction = true;

                if (ItemsSource is ISelectableCollection isl)
                {

                    if (item.IsSelected && !isl.IsSelected(item))
                    {
                        //if (isl.CanSelectMultiple && (Keyboard.Modifiers.HasFlag(ModifierKeys.Control) || e.TriggerMethod == SelectionChangeTrigger.CheckBox))
                        if (isl.CanSelectMultiple && e.TriggerMethod == SelectionChangeTrigger.CheckBox)
                        {
                            isl.AddToSelection(item);
                        }
                        else
                        {
                            isl.Select(item);
                        }

                        RaiseSelectionChangedEvent((new[] { item }).ToList(), new List<IClickSelectableControl>());
                    }
                    else
                    {
                        isl.Deselect(item);

                        RaiseSelectionChangedEvent(new List<IClickSelectableControl>(), (new[] { item }).ToList());
                    }
                }

                RefreshVisualSelection();

                _internalAction = false;
            }
        }

        #endregion

        #endregion

        #region Routed Events

        public static readonly RoutedEvent<RoutedSelectionChangedEventArgs<IClickSelectableControl>> SelectionChangedEvent =
            RoutedEvent.Register<RoutedSelectionChangedEventArgs<IClickSelectableControl>>(nameof(SelectionChanged), RoutingStrategies.Bubble, typeof(SelectPanel));

        public event RoutedSelectionChangedEventHandler<IClickSelectableControl> SelectionChanged
        {
            add { AddHandler(SelectionChangedEvent, value); }
            remove { RemoveHandler(SelectionChangedEvent, value); }
        }

        void RaiseSelectionChangedEvent(List<IClickSelectableControl> addedItems, List<IClickSelectableControl> removedItems)
        {
            addedItems ??= new List<IClickSelectableControl>();
            removedItems ??= new List<IClickSelectableControl>();

            RoutedSelectionChangedEventArgs<IClickSelectableControl> newEventArgs =
                new RoutedSelectionChangedEventArgs<IClickSelectableControl>(SelectionChangedEvent, addedItems, removedItems);
            RaiseEvent(newEventArgs);
        }

        public static readonly RoutedEvent<RoutedSelectionChangedEventArgs<IClickSelectableControl>> ItemsAddedEvent =
            RoutedEvent.Register<RoutedSelectionChangedEventArgs<IClickSelectableControl>>(nameof(ItemsAdded), RoutingStrategies.Bubble, typeof(SelectPanel));

        public event RoutedSelectionChangedEventHandler<IClickSelectableControl> ItemsAdded
        {
            add { AddHandler(ItemsAddedEvent, value); }
            remove { RemoveHandler(ItemsAddedEvent, value); }
        }

        void RaiseItemsAddedEvent(List<IClickSelectableControl> addedItems)
        {
            addedItems ??= new List<IClickSelectableControl>();

            RoutedSelectionChangedEventArgs<IClickSelectableControl> newEventArgs =
                new RoutedSelectionChangedEventArgs<IClickSelectableControl>(ItemsAddedEvent, new List<IClickSelectableControl>(), addedItems);
            RaiseEvent(newEventArgs);
        }

        public static readonly RoutedEvent<RoutedSelectionChangedEventArgs<IClickSelectableControl>> ItemsRemovedEvent =
            RoutedEvent.Register<RoutedSelectionChangedEventArgs<IClickSelectableControl>>(nameof(ItemsRemoved), RoutingStrategies.Bubble, typeof(SelectPanel));

        public event RoutedSelectionChangedEventHandler<IClickSelectableControl> ItemsRemoved
        {
            add { AddHandler(ItemsRemovedEvent, value); }
            remove { RemoveHandler(ItemsRemovedEvent, value); }
        }

        void RaiseItemsRemovedEvent(List<IClickSelectableControl> removedItems)
        {
            removedItems ??= new List<IClickSelectableControl>();

            RoutedSelectionChangedEventArgs<IClickSelectableControl> newEventArgs =
                new RoutedSelectionChangedEventArgs<IClickSelectableControl>(ItemsRemovedEvent, removedItems, new List<IClickSelectableControl>());
            RaiseEvent(newEventArgs);
        }

        #endregion

        #region Visual Properties

        #region ColorScheme

        public ColorScheme ColorScheme { get => GetValue(ColorSchemeProperty); set => SetValue(ColorSchemeProperty, value); }

        /// <summary>The backing styled property for <see cref="ColorScheme"/>. See the related property for details.</summary>
        public static readonly StyledProperty<ColorScheme> ColorSchemeProperty
            = AvaloniaProperty.Register<SelectPanel, ColorScheme>(nameof(ColorScheme), new ColorScheme());

        private void OnColorSchemeChanged(AvaloniaPropertyChangedEventArgs e)
        {
            ApplyColorScheme(e.GetNewValue<ColorScheme>());
            ColorSchemeChanged?.Invoke(this, e);
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
            foreach (IClickSelectableControl item in ItemsSource)
            {
                item.SelectedBrush = SelectedBrush;
                item.HighlightBrush = HighlightBrush;
                item.ClickBrush = ClickBrush;
                item.ApplyColorScheme(ColorScheme);
            }
        }

        /// <summary>
        /// Raised when the ColorScheme property is changed.
        /// </summary>
        public event EventHandler<AvaloniaPropertyChangedEventArgs>? ColorSchemeChanged;

        #endregion

        #region ScrollBars

        public ScrollBarVisibility HorizontalScrollBarVisibility { get => GetValue(HorizontalScrollBarVisibilityProperty); set => SetValue(HorizontalScrollBarVisibilityProperty, value); }

        /// <summary>The backing styled property for <see cref="HorizontalScrollBarVisibility"/>. See the related property for details.</summary>
        public static readonly StyledProperty<ScrollBarVisibility> HorizontalScrollBarVisibilityProperty
            = AvaloniaProperty.Register<SelectPanel, ScrollBarVisibility>(nameof(HorizontalScrollBarVisibility), ScrollBarVisibility.Disabled);


        public ScrollBarVisibility VerticalScrollBarVisibility { get => GetValue(VerticalScrollBarVisibilityProperty); set => SetValue(VerticalScrollBarVisibilityProperty, value); }

        /// <summary>The backing styled property for <see cref="VerticalScrollBarVisibility"/>. See the related property for details.</summary>
        public static readonly StyledProperty<ScrollBarVisibility> VerticalScrollBarVisibilityProperty
            = AvaloniaProperty.Register<SelectPanel, ScrollBarVisibility>(nameof(VerticalScrollBarVisibility), ScrollBarVisibility.Disabled);

        #endregion

        #region Brushes

        public IBrush ClickBrush { get => GetValue(ClickBrushProperty); set => SetValue(ClickBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="ClickBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush> ClickBrushProperty
            = AvaloniaProperty.Register<SelectPanel, IBrush>(nameof(ClickBrush), Colors.LightSalmon.ToBrush());


        public IBrush SelectedBrush { get => GetValue(SelectedBrushProperty); set => SetValue(SelectedBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="SelectedBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush> SelectedBrushProperty
            = AvaloniaProperty.Register<SelectPanel, IBrush>(nameof(SelectedBrush), Colors.MistyRose.ToBrush());


        public IBrush HighlightBrush { get => GetValue(HighlightBrushProperty); set => SetValue(HighlightBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="HighlightBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush> HighlightBrushProperty
            = AvaloniaProperty.Register<SelectPanel, IBrush>(nameof(HighlightBrush), Colors.Salmon.ToBrush());


        public IBrush DisabledBrush { get => GetValue(DisabledBrushProperty); set => SetValue(DisabledBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="DisabledBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush> DisabledBrushProperty
            = AvaloniaProperty.Register<SelectPanel, IBrush>(nameof(DisabledBrush), Colors.Gray.ToBrush());


        public IBrush BorderDisabledBrush { get => GetValue(BorderDisabledBrushProperty); set => SetValue(BorderDisabledBrushProperty, value); }

        /// <summary>The backing styled property for <see cref="BorderDisabledBrush"/>. See the related property for details.</summary>
        public static readonly StyledProperty<IBrush> BorderDisabledBrushProperty
            = AvaloniaProperty.Register<SelectPanel, IBrush>(nameof(BorderDisabledBrush), Colors.DarkGray.ToBrush());

        #endregion

        public CornerRadius CornerRadius { get => GetValue(CornerRadiusProperty); set => SetValue(CornerRadiusProperty, value); }

        /// <summary>The backing styled property for <see cref="CornerRadius"/>. See the related property for details.</summary>
        public static readonly StyledProperty<CornerRadius> CornerRadiusProperty
            = AvaloniaProperty.Register<SelectPanel, CornerRadius>(nameof(CornerRadius), new CornerRadius(0));

        public bool UseLighterBorder { get => GetValue(UseLighterBorderProperty); set => SetValue(UseLighterBorderProperty, value); }

        /// <summary>The backing styled property for <see cref="UseLighterBorder"/>. See the related property for details.</summary>
        public static readonly StyledProperty<bool> UseLighterBorderProperty
            = AvaloniaProperty.Register<SelectPanel, bool>(nameof(UseLighterBorder), false);

        #endregion

        #region Convenience Methods / Move Items

        /// <summary>
        /// Remove the currently selected items from the list.
        /// </summary>
        /// <remarks>
        /// If you are using <see cref="ItemsSource"/> to set this control's items, then this function will only work if the ItemsSource is a
        /// <see cref="ISelectableCollection{IClickSelectableControl}"/>.
        /// </remarks>
        public void RemoveSelectedItems()
        {
            if (ItemsSource != Items)
            {
                if (ItemsSource is ISelectableCollection<IClickSelectableControl> isc)
                {
                    var items = new List<IClickSelectableControl>(isc.SelectedItems);
                    foreach (var item in items)
                    {
                        isc.Remove(item);
                    }
                }
            }
            else
            {
                var items = new List<IClickSelectableControl>(Items.SelectedItems);
                foreach (var item in items)
                {
                    Items.Remove(item);
                }
            }
        }

        private class SortByParentIndex : IComparer<IClickSelectableControl>
        {
            // A class to sort a collection of IClickSelectableControls by their index in the parent SelectableCollection.

            public IList<IClickSelectableControl>? ParentCollection { get; set; }

            public int Compare(IClickSelectableControl? a, IClickSelectableControl? b)
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
        /// Move all of the currently selected items up by one in the list.
        /// </summary>
        /// <remarks>
        /// This will not function if you're using <see cref="ItemsSource"/> to manage the items in this control, rather than <see cref="Items"/>.
        /// If you're using <c>ItemsSource</c>, please use <see cref="MoveItemUp(int)"/>.
        /// </remarks>
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

            List<IClickSelectableControl> imov = new List<IClickSelectableControl>();

            foreach (IClickSelectableControl item in Items.SelectedItems)
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
            foreach (IClickSelectableControl item in imov)
            {
                Items.Remove(item);
            }

            imov.Reverse();

            Items.ClearSelection();

            // re-insert and re-select all the items
            foreach (IClickSelectableControl item in imov)
            {
                Items.Insert(index, item);
                //Items.AddToSelection(item);
            }

            foreach (IClickSelectableControl item in imov)
            {
                Items.AddToSelection(item);
            }

            RefreshVisualSelection();
        }

        /// <summary>
        /// Move all of the currently selected items down by one in the list.
        /// </summary>
        /// <remarks>
        /// This will not function if you're using <see cref="ItemsSource"/> to manage the items in this control, rather than <see cref="Items"/>.
        /// If you're using <c>ItemsSource</c>, please use <see cref="MoveItemDown(int)"/>.
        /// </remarks>
        public void MoveSelectedItemsDown()
        {
            if (Items.SelectedItems.Count == 0 || ItemsSource != Items) return;

            int index = int.MaxValue;

            List<IClickSelectableControl> imov = new List<IClickSelectableControl>();

            foreach (IClickSelectableControl item in Items.SelectedItems)
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
            foreach (IClickSelectableControl item in imov)
            {
                Items.Remove(item);
            }

            if (index > (Items.Count - 1))
            {
                index = Items.Count;
            }

            Items.ClearSelection();

            // re-insert and re-select the controls at the new location
            foreach (IClickSelectableControl item in imov)
            {
                Items.Insert(index, item);
                //Items.AddToSelection(item);
            }

            foreach (IClickSelectableControl item in imov)
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
        /// This function does not work if ItemsSource is not an <see cref="IList"/>.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the index passed in is beyond the number of items in the collection.</exception>
        public void MoveItemUp(int index)
        {
            if (ItemsSource != Items)
            {
                if (ItemsSource is IList isl)
                {
                    var suc = isl[index];
                    if (suc == null) return;

                    int moveIndex = index - 1;//IndexOf(suc) - 1;
                    if (moveIndex < 0)
                    {
                        moveIndex = 0;
                    }

                    if (ItemsSource is ISelectableCollection isc) // try to retain selection state
                    {
                        bool resel = isc.IsSelected(suc);
                        isl.Remove(suc);
                        isl.Insert(moveIndex, suc);
                        if (resel) isc.Select(suc);
                    }
                    else
                    {
                        isl.Remove(suc);
                        isl.Insert(moveIndex, suc);
                    }
                }
                else return;
            }
            else
            {
                IClickSelectableControl suc = Items[index];

                int moveIndex = index - 1;//IndexOf(suc) - 1;
                if (moveIndex < 0)
                {
                    moveIndex = 0;
                }

                bool resel = Items.IsSelected(suc);

                Items.Remove(suc);
                Items.Insert(moveIndex, suc);
                if (resel) Items.AddToOrReplaceSelection(suc);
            }

            RefreshVisualSelection();
        }

        /// <summary>
        /// Move an item in the SelectPanel down by one position in the list.
        /// </summary>
        /// <param name="index">The index of the item to move.</param>
        /// <remarks>
        /// If the index passed in is the last index in the collection, then nothing is moved, as it's already at the bottom of the list.
        /// This function does not work if ItemsSource is not an <see cref="IList"/>.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the index passed in is beyond the number of items in the collection.</exception>
        public void MoveItemDown(int index)
        {
            if (ItemsSource != Items)
            {
                if (ItemsSource is IList isl)
                {
                    var suc = isl[index];
                    if (suc == null) return;

                    int moveIndex = index + 1; //IndexOf(suc) + 1;
                    if (moveIndex > (isl.Count - 1))
                    {
                        moveIndex = (isl.Count - 1);
                    }

                    if (ItemsSource is ISelectableCollection isc) // try to retain selection state
                    {
                        bool resel = isc.IsSelected(suc);
                        isl.Remove(suc);
                        isl.Insert(moveIndex, suc);
                        if (resel) isc.Select(suc);
                    }
                    else
                    {
                        isl.Remove(suc);
                        isl.Insert(moveIndex, suc);
                    }
                }
                else return;
            }
            else
            {
                IClickSelectableControl suc = Items[index];

                int moveIndex = index + 1; //IndexOf(suc) + 1;
                if (moveIndex > (Items.Count - 1))
                {
                    moveIndex = (Items.Count - 1);
                }

                bool resel = Items.IsSelected(suc);

                Items.Remove(suc);
                Items.Insert(moveIndex, suc);
                if (resel) Items.AddToOrReplaceSelection(suc);
            }

            RefreshVisualSelection();
        }

        #endregion

        #region AllowParentScrolling

        public bool AllowParentScrolling { get => GetValue(AllowParentScrollingProperty); set => SetValue(AllowParentScrollingProperty, value); }

        /// <summary>The backing styled property for <see cref="AllowParentScrolling"/>. See the related property for details.</summary>
        public static readonly StyledProperty<bool> AllowParentScrollingProperty
            = AvaloniaProperty.Register<SelectPanel, bool>(nameof(AllowParentScrolling), true);

        // source: https://serialseb.com/blog/2007/09/03/wpf-tips-6-preventing-scrollviewer-from/
        // author: Sebastien Lambla

        private static List<PointerWheelEventArgs> _reentrantList = new List<PointerWheelEventArgs>();

        // lol not sure if this works on Avalonia, we'll see what happens

        private void HandlePreviewMouseWheel(object sender, PointerWheelEventArgs e)
        {
            if (AllowParentScrolling)
            {
                if (sender is ScrollViewer scrollControl)
                {
                    if (!e.Handled && sender != null && !_reentrantList.Contains(e))
                    {
                        var previewEventArg = new PointerWheelEventArgs(sender, e.Pointer, this, e.GetCurrentPoint(this).Position, 
                            e.Timestamp, new PointerPointProperties(), e.KeyModifiers, e.Delta)
                        {
                            RoutedEvent = InputElement.PointerWheelChangedEvent,
                            Source = sender
                        };

                        var originalSource = (e.Source as Control) ?? null;
                        _reentrantList.Add(previewEventArg);
                        originalSource?.RaiseEvent(previewEventArg);
                        _reentrantList.Remove(previewEventArg);

                        // at this point if no one else handled the event in our children, we do our job

                        if (!previewEventArg.Handled && ((e.Delta.Length > 0 && scrollControl.Offset.Y == 0) ||
                            (e.Delta.Length <= 0 && scrollControl.Offset.Y >= scrollControl.Extent.Height - scrollControl.Viewport.Height)))
                        {
                            e.Handled = true;
                            var eventArg = new PointerWheelEventArgs(sender, e.Pointer, this, e.GetCurrentPoint(this).Position, 
                                e.Timestamp, new PointerPointProperties(), e.KeyModifiers, e.Delta);
                            eventArg.RoutedEvent = InputElement.PointerWheelChangedEvent;
                            eventArg.Source = sender;
                            var parent = (((Control)sender).Parent as Control) ?? null;
                            parent?.RaiseEvent(eventArg);
                        }
                    }
                }
            }
        }

        #endregion
    }
}
