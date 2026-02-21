using SolidShineUi.Utils;
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
    [DefaultEvent(nameof(SelectionChanged))]
    [Localizability(LocalizationCategory.ListBox)]
    public class SelectPanel : ThemedControl
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
            SetValue(ItemsPropertyKey, new SelectableCollection<IClickSelectableControl>());
            SetValue(ItemsSourceProperty, Items);
        }

        #region Template Handling
        /// <inheritdoc/>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            LoadTemplateItems();
            if (itemsLoaded && sv != null)
            {
                sv.PreviewMouseWheel += HandlePreviewMouseWheel;
                sv.Unloaded += (s, _) => sv.PreviewMouseWheel -= HandlePreviewMouseWheel;
            }
        }

        bool itemsLoaded = false;

#if NETCOREAPP
        ScrollViewer? sv = null;
#else
        ScrollViewer sv = null;
#endif

        private void LoadTemplateItems()
        {
            if (!itemsLoaded)
            {
                sv = (ScrollViewer)GetTemplateChild("PART_Sv");

                if (sv != null)
                {
                    itemsLoaded = true;
                }
            }
        }
        #endregion

        #region ItemsSource

        /// <summary>
        /// Gets or sets a collection used to generate the content of this SelectPanel.
        /// </summary>
        /// <remarks>
        /// It is recommended to set this property to an <see cref="ObservableCollection{IClickSelectableControl}"/>, <see cref="SelectableCollection{IClickSelectableControl}"/>, 
        /// or <see cref="SelectableCollectionView{IClickSelectableControl}"/>.
        /// If you use other IEnumerable types that do not implement <see cref="INotifyCollectionChanged"/>, then this control's contents will not update automatically.
        /// If your collection doesn't support <see cref="ISelectableCollection"/>, you may need to implement your own code for handling the selection state of the items
        /// in your collection (and certain functions in this control, such as <see cref="MultiSelect"/> or <see cref="RemoveSelectedItems"/>, may not work).
        /// </remarks>
        public IEnumerable<IClickSelectableControl> ItemsSource
        {
            get { return (IEnumerable<IClickSelectableControl>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        /// <summary>
        /// A dependency property object backing the related property. See the property itself for more details.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable<IClickSelectableControl>), typeof(SelectPanel),
                new PropertyMetadata((s, e) => s.PerformAs<SelectPanel>((sp) =>
                sp.OnItemsSourceChanged((IEnumerable<IClickSelectableControl>)e.OldValue, (IEnumerable<IClickSelectableControl>)e.NewValue))));

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

        #region SelectableCollection handling

        #region ItemsProperty

        bool _internalAction = false;
        
        private static readonly DependencyPropertyKey ItemsPropertyKey
            = DependencyProperty.RegisterReadOnly("Items", typeof(SelectableCollection<IClickSelectableControl>), typeof(SelectPanel),
            new FrameworkPropertyMetadata(new SelectableCollection<IClickSelectableControl>()));

        /// <summary>
        /// The dependency property object backing the <see cref="Items"/> property. See the related property for details.
        /// </summary>
        public static readonly DependencyProperty ItemsProperty = ItemsPropertyKey.DependencyProperty;

        /// <summary>
        /// Get or set the list of items in this SelectPanel. This Items property can be used to add items, remove items, and also select items via the Select method.
        /// </summary>
        /// <remarks>
        /// If you're using <see cref="ItemsSource"/> to set the items in this control, you should instead modify/manage your items through that items source, rather than using this property.
        /// </remarks>
        [Category("Common")]
        [Description("Get or set the list of items in this SelectPanel.")]
        public SelectableCollection<IClickSelectableControl> Items
        {
            get { return (SelectableCollection<IClickSelectableControl>)GetValue(ItemsProperty); }
            private set { SetValue(ItemsPropertyKey, value); }
        }

        #endregion

        #region MultiSelect

        /// <summary>
        /// Get or set if multiple items can be selected at once. If false, then only 1 item can be selected at a time.
        /// </summary>
        /// <remarks>
        /// If you're using <see cref="ItemsSource"/> to manage this control's items, this property will not function if <c>ItemsSource</c> is not an <see cref="ISelectableCollection"/>.
        /// This warning does not apply if you're using the <see cref="Items"/> property, which is itself an <see cref="ISelectableCollection"/>.
        /// </remarks>
        /// <exception cref="NotSupportedException">Thrown if the underlying <see cref="ISelectableCollection"/> in <see cref="ItemsSource"/> doesn't allow changing this value</exception>
        [Category("Common")]
        [Description("Get or set if multiple items can be selected at once. If false, then only 1 item can be selected at a time.")]
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
                    // not really a selectable collection, so I don't really have any way of knowing - to be on the safe side, I'll return false
                    return false;
                }
            }
            set
            {
                if (ItemsSource is ISelectableCollection isl)
                {
                    isl.CanSelectMultiple = value;
                    // some ISelectionCollection types may not allow this property to be changed. at that point, an exception is raised
                }
            }
        }

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

        #endregion

        #region CollectionChanged / CollectionSelectionChanged
#if NETCOREAPP
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
#else
        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // update .NET Core section first, copy into here
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems != null)
                    {
                        foreach (IClickSelectableControl item in e.NewItems)
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
                        foreach (IClickSelectableControl item in e.OldItems)
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
                        foreach (IClickSelectableControl item in e.NewItems)
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
                    break;
                default:
                    break;
            }
#endif

        }
        private void Items_SelectionChanged(object sender, CollectionSelectionChangedEventArgs e)
        {
            if (_internalAction) return;

            RefreshVisualSelection();

            RaiseSelectionChangedEvent(e.AddedItems.OfType<IClickSelectableControl>().ToList(), e.RemovedItems.OfType<IClickSelectableControl>().ToList());
        }

        #endregion

        #region Item Selection Changed

#if NETCOREAPP
        private void Item_SelectionChanged(object? sender, ItemSelectionChangedEventArgs e)
#else
        private void Item_SelectionChanged(object sender, ItemSelectionChangedEventArgs e)
#endif
        {
            if (_internalAction) return;

            if (sender is IClickSelectableControl item)
            {
                _internalAction = true;

                if (ItemsSource is ISelectableCollection isl)
                {

                    if (item.IsSelected && !isl.IsSelected(item))
                    {
                        if (isl.CanSelectMultiple && (Keyboard.Modifiers.HasFlag(ModifierKeys.Control) || e.TriggerMethod == SelectionChangeTrigger.CheckBox))
                        {
                            isl.AddToSelection(item);
                        }
                        else
                        {
                            isl.SelectItem(item);
                        }

                        RaiseSelectionChangedEvent((new[] { item }).ToList(), null);
                    }
                    else
                    {
                        isl.Deselect(item);

                        RaiseSelectionChangedEvent(null, (new[] { item }).ToList());
                    }
                }

                RefreshVisualSelection();

                _internalAction = false;
            }
        }

        #endregion

        #endregion

        #region Color Scheme

        /// <summary>
        /// A dependency property object backing the related property. See the property itself for more details.
        /// </summary>
        public static readonly DependencyProperty ColorSchemeProperty
            = DependencyProperty.Register("ColorScheme", typeof(ColorScheme), typeof(SelectPanel),
            new FrameworkPropertyMetadata(new ColorScheme(), OnColorSchemeChanged));

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

        private bool use_lbrdr = false;
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

            ItemsTheme = new SsuiTheme(cs);

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

            //UpdateChildrenColorScheme();

            runApply = true;
        }

        //private void UpdateChildrenColorScheme()
        //{
        //    foreach (IClickSelectableControl item in ItemsSource)
        //    {
        //        item.SelectedBrush = SelectedBrush;
        //        item.HighlightBrush = HighlightBrush;
        //        item.ClickBrush = ClickBrush;
        //        item.ApplyColorScheme(ColorScheme);
        //    }
        //}
        #endregion

        #region SsuiTheme / child SsuiTheme

        /// <inheritdoc/>
        protected override void OnApplySsuiTheme(SsuiTheme ssuiTheme, bool useLightBorder = false, bool useAccentTheme = false)
        {
            base.OnApplySsuiTheme(ssuiTheme, useLightBorder, useAccentTheme);

            SsuiTheme subitemTheme = ssuiTheme;
            SsuiTheme mainTheme = ssuiTheme;

            if (ssuiTheme is SsuiAppTheme sat)
            {
                if (useAccentTheme)
                {
                    mainTheme = sat.AccentTheme;
                }

                if (sat.UseSubitemThemeWithPanels)
                {
                    // subitems should use the subitem theme
                    if (!ThemeValueExclude.Split(',').Contains(nameof(ItemsTheme)))
                    {
                        SetBinding(ItemsThemeProperty, new System.Windows.Data.Binding(nameof(SsuiAppTheme.SubitemTheme)) { Source = this.SsuiTheme });
                    }
                    subitemTheme = sat.SubitemTheme;
                }
                else if (useAccentTheme)
                {
                    if (!ThemeValueExclude.Split(',').Contains(nameof(ItemsTheme)))
                    {
                        SetBinding(ItemsThemeProperty, new System.Windows.Data.Binding(nameof(SsuiAppTheme.AccentTheme)) { Source = this.SsuiTheme });
                    }
                    subitemTheme = sat.AccentTheme;
                }
                else
                {
                    if (!ThemeValueExclude.Split(',').Contains(nameof(ItemsTheme)))
                    {
                        SetBinding(ItemsThemeProperty, new System.Windows.Data.Binding(nameof(SsuiTheme)) { Source = this });
                    }
                }
            }
            else
            {
                if (!ThemeValueExclude.Split(',').Contains(nameof(ItemsTheme)))
                {
                    SetBinding(ItemsThemeProperty, new System.Windows.Data.Binding(nameof(SsuiTheme)) { Source = this });
                }
            }

            // apply brushes using mainTheme
            ApplyThemeBinding(BackgroundProperty, SsuiTheme.PanelBackgroundProperty, mainTheme);
            ApplyThemeBinding(DisabledBrushProperty, SsuiTheme.DisabledBackgroundProperty, mainTheme);
            ApplyThemeBinding(BorderDisabledBrushProperty, SsuiTheme.DisabledBorderBrushProperty, mainTheme);
            ApplyThemeBinding(CornerRadiusProperty, SsuiTheme.CornerRadiusProperty, mainTheme);

            // these brushes are applied directly from ItemsTheme
            // however, we'll directly set these properties so that people can see what they are

            _internalThemeAction = true;

            HighlightBrush = subitemTheme.HighlightBrush;
            ClickBrush = subitemTheme.ClickBrush;
            SelectedBrush = subitemTheme.SelectedBackgroundBrush;

            _internalThemeAction = false;

            // UpdateChildrenAppearance();
        }

        bool _internalThemeAction = false;

        /// <summary>
        /// Get or set the theme and brushes this panel will use for its child items.
        /// <para/>
        /// This is set automatically when the <c>SsuiTheme</c> or <c>ColorScheme</c> property is updated.
        /// </summary>
        public SsuiTheme ItemsTheme { get => (SsuiTheme)GetValue(ItemsThemeProperty); set => SetValue(ItemsThemeProperty, value); }

        /// <summary>The backing dependency property for <see cref="ItemsTheme"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ItemsThemeProperty
            = DependencyProperty.Register(nameof(ItemsTheme), typeof(SsuiTheme), typeof(SelectPanel),
            new FrameworkPropertyMetadata(new SsuiTheme(), (d, e) => d.PerformAs<SelectPanel>((o) => o.OnItemsThemeChange(e))));

        //public static readonly DependencyProperty ItemsThemeProperty = ItemsThemePropertyKey.DependencyProperty;

        private void OnItemsThemeChange(DependencyPropertyChangedEventArgs e)
        {
            foreach (IClickSelectableControl item in ItemsSource)
            {
                if (item is FrameworkElement dobj)
                {
                    // most (if not all) IClickSelectableControl objects will be at least FrameworkElements, if not Controls
                    // so we can pretty reliably set this property here. we will need to inform in the documentation that
                    // implementers of this property will need to AddOwner this property so that it'll actually work

                    // we want to do binding in case the ItemsTheme is itself bound to a single or central SsuiTheme somewhere
                    // that way, the end user or developer can just change that SsuiTheme and it'll propogate down to here
                    dobj.SetBinding(SsuiThemeProperty, new System.Windows.Data.Binding(nameof(ItemsTheme)) { Source = this });
                }
                else
                {
                    // on the rare off-chance it's not a framework element, then we'll just do plain-old property assignment
                    item.SsuiTheme = SsuiTheme;
                }
            }
        }

        #endregion

        #region Routed Events

        /// <summary>
        /// A routed event object backing the related event. See the event itself for more details.
        /// </summary>
        public static readonly RoutedEvent SelectionChangedEvent = EventManager.RegisterRoutedEvent(
            "SelectionChanged", RoutingStrategy.Bubble, typeof(RoutedSelectionChangedEventHandler<IClickSelectableControl>), typeof(SelectPanel));

        /// <summary>
        /// Raised when an item is selected or deselected in this list.
        /// </summary>
        public event RoutedSelectionChangedEventHandler<IClickSelectableControl> SelectionChanged
        {
            add { AddHandler(SelectionChangedEvent, value); }
            remove { RemoveHandler(SelectionChangedEvent, value); }
        }

#if NETCOREAPP
        void RaiseSelectionChangedEvent(List<IClickSelectableControl>? addedItems, List<IClickSelectableControl>? removedItems)
        {
            addedItems ??= new List<IClickSelectableControl>();
            removedItems ??= new List<IClickSelectableControl>();
#else
        void RaiseSelectionChangedEvent(List<IClickSelectableControl> addedItems, List<IClickSelectableControl> removedItems)
        {
            if (addedItems == null)
            {
                addedItems = new List<IClickSelectableControl>();
            }

            if (removedItems == null)
            {
                removedItems = new List<IClickSelectableControl>();
            }
#endif

            RoutedSelectionChangedEventArgs<IClickSelectableControl> newEventArgs = 
                new RoutedSelectionChangedEventArgs<IClickSelectableControl>(SelectionChangedEvent, addedItems, removedItems);
            RaiseEvent(newEventArgs);
        }

        /// <summary>
        /// A routed event object backing the related event. See the event itself for more details.
        /// </summary>
        public static readonly RoutedEvent ItemsAddedEvent = EventManager.RegisterRoutedEvent(
            "ItemsAdded", RoutingStrategy.Bubble, typeof(RoutedSelectionChangedEventHandler<IClickSelectableControl>), typeof(SelectPanel));

        /// <summary>
        /// Raised when an item is added to the SelectPanel's items.
        /// </summary>
        public event RoutedSelectionChangedEventHandler<IClickSelectableControl> ItemsAdded
        {
            add { AddHandler(ItemsAddedEvent, value); }
            remove { RemoveHandler(ItemsAddedEvent, value); }
        }

        void RaiseItemsAddedEvent(List<IClickSelectableControl> addedItems)
        {
#if NETCOREAPP
            addedItems ??= new List<IClickSelectableControl>();
#else
            if (addedItems == null)
            {
                addedItems = new List<IClickSelectableControl>();
            }
#endif

            RoutedSelectionChangedEventArgs<IClickSelectableControl> newEventArgs = 
                new RoutedSelectionChangedEventArgs<IClickSelectableControl>(ItemsAddedEvent, new List<IClickSelectableControl>(), addedItems);
            RaiseEvent(newEventArgs);
        }

        /// <summary>
        /// A routed event object backing the related event. See the event itself for more details.
        /// </summary>
        public static readonly RoutedEvent ItemsRemovedEvent = EventManager.RegisterRoutedEvent(
            "ItemsRemoved", RoutingStrategy.Bubble, typeof(RoutedSelectionChangedEventHandler<IClickSelectableControl>), typeof(SelectPanel));

        /// <summary>
        /// Raised when an item is removed from the SelectPanel's items.
        /// </summary>
        public event RoutedSelectionChangedEventHandler<IClickSelectableControl> ItemsRemoved
        {
            add { AddHandler(ItemsRemovedEvent, value); }
            remove { RemoveHandler(ItemsRemovedEvent, value); }
        }

        void RaiseItemsRemovedEvent(List<IClickSelectableControl> removedItems)
        {
#if NETCOREAPP
            removedItems ??= new List<IClickSelectableControl>();
#else
            if (removedItems == null)
            {
                removedItems = new List<IClickSelectableControl>();
            }
#endif

            RoutedSelectionChangedEventArgs<IClickSelectableControl> newEventArgs = 
                new RoutedSelectionChangedEventArgs<IClickSelectableControl>(ItemsRemovedEvent, removedItems, new List<IClickSelectableControl>());
            RaiseEvent(newEventArgs);
        }
        #endregion

        #region Visual Properties

        #region Scrollbars

        /// <summary>
        /// A dependency property object backing the related property. See the property itself for more details.
        /// </summary>
        public static readonly DependencyProperty HorizontalScrollBarVisibilityProperty
            = DependencyProperty.Register(nameof(HorizontalScrollBarVisibility), typeof(ScrollBarVisibility), typeof(SelectPanel),
            new FrameworkPropertyMetadata(ScrollBarVisibility.Disabled));

        /// <summary>
        /// Get or set the appearance of the horizontal scroll bar for this control.
        /// </summary>
        [Category("Layout")]
        [Description("Get or set the appearance of the horizontal scroll bar for this control.")]
        public ScrollBarVisibility HorizontalScrollBarVisibility
        {
            get { return (ScrollBarVisibility)GetValue(HorizontalScrollBarVisibilityProperty); }
            set { SetValue(HorizontalScrollBarVisibilityProperty, value); }
        }

        /// <summary>
        /// A dependency property object backing the related property. See the property itself for more details.
        /// </summary>
        public static readonly DependencyProperty VerticalScrollBarVisibilityProperty
            = DependencyProperty.Register(nameof(VerticalScrollBarVisibility), typeof(ScrollBarVisibility), typeof(SelectPanel),
            new FrameworkPropertyMetadata(ScrollBarVisibility.Auto));

        /// <summary>
        /// Get or set the appearance of the vertical scroll bar for this control.
        /// </summary>
        [Category("Layout")]
        [Description("Get or set the appearance of the vertical scroll bar for this control.")]
        public ScrollBarVisibility VerticalScrollBarVisibility
        {
            get { return (ScrollBarVisibility)GetValue(VerticalScrollBarVisibilityProperty); }
            set { SetValue(VerticalScrollBarVisibilityProperty, value); }
        }

        #endregion

        #region Brushes

        /// <summary>
        /// Get or set the brush used when an item in this control is being clicked.
        /// </summary>
        [Category("Brushes")]
        public Brush ClickBrush
        {
            get => (Brush)GetValue(ClickBrushProperty);
            set => SetValue(ClickBrushProperty, value);
        }

        /// <summary>
        /// Get or set the brush used when an item in this control is selected.
        /// </summary>
        [Category("Brushes")]
        public Brush SelectedBrush
        {
            get => (Brush)GetValue(SelectedBrushProperty);
            set => SetValue(SelectedBrushProperty, value);
        }

        /// <summary>
        /// Get or set the brush used when an item in this control is highlighted (e.g., has the mouse over it or has keyboard focus).
        /// </summary>
        [Category("Brushes")]
        public Brush HighlightBrush
        {
            get => (Brush)GetValue(HighlightBrushProperty);
            set => SetValue(HighlightBrushProperty, value);
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
        /// A dependency property object backing the related property. See the property itself for more details.
        /// </summary>
        public static readonly DependencyProperty ClickBrushProperty = DependencyProperty.Register(
            nameof(ClickBrush), typeof(Brush), typeof(SelectPanel),
            new PropertyMetadata(Colors.LightSalmon.ToBrush(), ChildBrushChanged));

        /// <summary>
        /// A dependency property object backing the related property. See the property itself for more details.
        /// </summary>
        public static readonly DependencyProperty SelectedBrushProperty = DependencyProperty.Register(
            nameof(SelectedBrush), typeof(Brush), typeof(SelectPanel),
            new PropertyMetadata(Colors.MistyRose.ToBrush(), ChildBrushChanged));

        /// <summary>
        /// A dependency property object backing the related property. See the property itself for more details.
        /// </summary>
        public static readonly DependencyProperty HighlightBrushProperty = DependencyProperty.Register(
            nameof(HighlightBrush), typeof(Brush), typeof(SelectPanel),
            new PropertyMetadata(Colors.Salmon.ToBrush(), ChildBrushChanged));

        /// <summary>
        /// A dependency property object backing the related property. See the property itself for more details.
        /// </summary>
        public static readonly DependencyProperty DisabledBrushProperty = DependencyProperty.Register(
            nameof(DisabledBrush), typeof(Brush), typeof(SelectPanel),
            new PropertyMetadata(new SolidColorBrush(Colors.Gray)));

        /// <summary>
        /// A dependency property object backing the related property. See the property itself for more details.
        /// </summary>
        public static readonly DependencyProperty BorderDisabledBrushProperty = DependencyProperty.Register(
            nameof(BorderDisabledBrush), typeof(Brush), typeof(SelectPanel),
            new PropertyMetadata(new SolidColorBrush(Colors.DarkGray)));

        private static void ChildBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SelectPanel sp && e.NewValue is Brush b)
            {
                if (sp._internalThemeAction) return;

                if (sp.ItemsTheme.IsFrozen) // if it's frozen, we'll want to unfreeze it to make sure we can edit it
                {
                    sp.ItemsTheme = sp.ItemsTheme.CloneCurrentValue();
                }

                switch (e.Property.Name)
                {
                    case nameof(HighlightBrush):
                        sp.ItemsTheme.HighlightBrush = b;
                        break;
                    case nameof(ClickBrush):
                        sp.ItemsTheme.ClickBrush = b;
                        break;
                    case nameof(SelectedBrush):
                        sp.ItemsTheme.SelectedBackgroundBrush = b;
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion

        #region CornerRadius

        /// <summary>
        /// A dependency property object backing the related property. See the property itself for more details.
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            nameof(CornerRadius), typeof(CornerRadius), typeof(SelectPanel),
            new PropertyMetadata(new CornerRadius(0)));

        /// <summary>
        /// Get or set the corner radius (or radii) to use for the control's border. Setting the corners to 0 means there is no rounding; square corners are used.
        /// </summary>
        [Category("Appearance")]
        [Description("Get or set the corner radius (or radii) to use for the control's border.")]
        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        #endregion

        #endregion

        #region Convenience Methods (to remove later)

        ///// <summary>
        ///// Gets the number of items in this SelectPanel.
        ///// Note that this function will be removed in a future version. Please instead use <c>Items.Count</c>.
        ///// </summary>
        //[Obsolete("This will be removed in a future version. You can instead use \"Items.Count\".", false)]
        //public int Count { get => Items.Count; }

        ///// <summary>
        ///// Gets the number of items that are currently selected.
        ///// Note that this function will be removed in a future version. Please instead use <c>Items.SelectedItems.Count</c>.
        ///// </summary>
        //[Obsolete("This will be removed in a future version. You can instead use \"Items.SelectedItems.Count\".", false)]
        //public int SelectionCount { get => Items.SelectedItems.Count; }

        ///// <summary>
        ///// Get a collection of items that have been selected, returned as a certain type (that inherits from IClickSelectableControl).
        ///// Note that this function will be removed in a future version. Please instead use <c>Items.SelectedItems.OfType</c> (Linq).
        ///// </summary>
        ///// <typeparam name="T">The type to return the selected items as. It must inherit from IClickSelectableControl.</typeparam>
        //[Obsolete("This will be removed in a future version. You can instead use the Linq method \"Items.SelectedItems.OfType<>\".", false)]
        //public IEnumerable<T> GetSelectedItemsOfType<T>() where T : IClickSelectableControl
        //{
        //    return Items.SelectedItems.OfType<T>();
        //}

        ///// <summary>
        ///// Select all items in this SelectPanel. Note that this function will be removed in a future version. Please instead use <c>Items.SelectAll</c>.
        ///// </summary>
        //[Obsolete("This will be removed in a future version. You can instead use \"Items.SelectAll\".", false)]
        //public void SelectAll()
        //{
        //    Items.SelectAll();
        //}

        ///// <summary>
        ///// Deselect all items in this SelectPanel. Note that this function will be removed in a future version. Please instead use <c>Items.ClearSelection</c>.
        ///// </summary>
        //[Obsolete("This will be removed in a future version. You can instead use \"Items.ClearSelection\".", false)]
        //public void DeselectAll()
        //{
        //    Items.ClearSelection();
        //}

        ///// <summary>
        ///// Add an item to this SelectPanel. Note that this function will be removed in a future version. Please instead use <c>Items.Add</c>.
        ///// </summary>
        ///// <param name="item">Item to be added.</param>
        //[Obsolete("This will be removed in a future version. You can instead use \"Items.Add\".", false)]
        //public void AddItem(IClickSelectableControl item)
        //{
        //    Items.Add(item);
        //    //RaiseItemsAddedEvent(items.ToList());
        //}

        ///// <summary>
        ///// Add a collection of items to this SelectPanel. Note that this function will be removed in a future version. Please instead use <c>Items.Add</c>.
        ///// </summary>
        ///// <param name="items">The items to be added.</param>
        //[Obsolete("This will be removed in a future version. You can instead use a for or foreach loop around \"Items.Add\".", false)]
        //public void AddItems(IEnumerable<IClickSelectableControl> items)
        //{
        //    foreach (IClickSelectableControl item in items)
        //    {
        //        Items.Add(item);
        //    }
        //    //RaiseItemsAddedEvent(items.ToList());
        //}

        ///// <summary>
        ///// Insert an item into this SelectPanel. Note that this function will be removed in a future version. Please instead use <c>Items.Insert</c>.
        ///// </summary>
        ///// <param name="index">The index to insert the item at.</param>
        ///// <param name="item">The item to insert.</param>
        //[Obsolete("This will be removed in a future version. You can instead use \"Items.Insert\".", false)]
        //public void InsertItem(int index, IClickSelectableControl item)
        //{
        //    Items.Insert(index, item);
        //    //RaiseItemsAddedEvent(new List<IClickSelectableControl>() { item });
        //}

        ///// <summary>
        ///// Insert a collection of items into this SelectPanel. Note that this function will be removed in a future version. Please instead use <c>Items.Insert</c>.
        ///// </summary>
        ///// <param name="index">The index to insert the items at.</param>
        ///// <param name="items">The items to insert.</param>
        //[Obsolete("This will be removed in a future version. You can instead use a for or foreach loop around \"Items.Insert\".", false)]
        //public void InsertItems(int index, IEnumerable<IClickSelectableControl> items)
        //{
        //    List<IClickSelectableControl> litems = items.ToList();
        //    litems.Reverse();

        //    foreach (IClickSelectableControl item in litems)
        //    {
        //        Items.Insert(index, item);
        //    }

        //    //RaiseItemsAddedEvent(litems);
        //}

        ///// <summary>
        ///// Get all items in the SelectPanel that match the specified type. Note that this function will be removed in a future version. Please instead use <c>Items.OfType</c> (Linq).
        ///// </summary>
        ///// <typeparam name="T">The type to filter the SelectPanel items for.</typeparam>
        ///// <returns></returns>
        //[Obsolete("This will be removed in a future version. You can instead use the Linq method \"Items.OfType<>\".", false)]
        //public IEnumerable<T> GetItemsAsType<T>() where T : IClickSelectableControl
        //{
        //    return Items.OfType<T>();
        //    //foreach (IClickSelectableControl item in Items)
        //    //{
        //    //    if (item is T t)
        //    //    {
        //    //        yield return t;
        //    //    }
        //    //}
        //}

        ///// <summary>
        ///// Remove an item from this SelectPanel. Note that this function will be removed in a future version. Please instead use <c>Items.Remove</c>.
        ///// </summary>
        ///// <param name="item">The item to be removed.</param>
        //[Obsolete("This will be removed in a future version. You can instead use \"Items.Remove\".", false)]
        //public void RemoveItem(IClickSelectableControl item)
        //{
        //    Items.Remove(item);
        //}

        ///// <summary>
        ///// Remove multiple items from this SelectPanel. Note that this function will be removed in a future version. Please instead use <c>Items.Remove</c>.
        ///// </summary>
        ///// <param name="items">The items to be removed.</param>
        //[Obsolete("This will be removed in a future version. You can instead use a for or foreach loop around \"Items.Remove\".", false)]
        //public void RemoveItems(IEnumerable<IClickSelectableControl> items)
        //{
        //    foreach (var item in items)
        //    {
        //        Items.Remove(item);
        //    }

        //    //RaiseItemsRemovedEvent(items.ToList());
        //}

        ///// <summary>
        ///// Remove an item at a specified index from this SelectPanel. Note that this function will be removed in a future version. Please instead use <c>Items.RemoveAt</c>.
        ///// </summary>
        ///// <param name="index">The index of the item to be removed.</param>
        //[Obsolete("This will be removed in a future version. You can instead use \"Items.RemoveAt\".", false)]
        //public void RemoveAt(int index)
        //{
        //    Items.RemoveAt(index);
        //}

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

        ///// <summary>
        ///// Get the index of an item in this SelectPanel. Note that this function will be removed in a future version. Please instead use <c>Items.IndexOf</c>.
        ///// </summary>
        ///// <param name="item">The item to get the index of.</param>
        //[Obsolete("This will be removed in a future version. You can instead use \"Items.IndexOf\".", false)]
        //public int IndexOf(IClickSelectableControl item)
        //{
        //    return Items.IndexOf(item);
        //}

        ///// <summary>
        ///// Get an item at a specified index in this SelectPanel. Note that this function will be removed in a future version. Please instead use <c>Items[]</c>.
        ///// </summary>
        ///// <param name="index">The index of the item to be removed.</param>
        //[Obsolete("This will be removed in a future version. You can instead use \"Items[]\".", false)]
        //public IClickSelectableControl Get(int index)
        //{
        //    return Items[index];
        //}

        ///// <summary>
        ///// Get an item at a specified index in this SelectPanel. Note that this function will be removed in a future version. Please instead use <c>Items[]</c>.
        ///// </summary>
        ///// <param name="index">The index of the item to be removed.</param>
        //[Obsolete("This will be removed in a future version. You can instead use \"Items[]\".", false)]
        //public IClickSelectableControl this[int index]
        //{
        //    get
        //    {
        //        return Items[index];
        //    }
        //}

        ///// <summary>
        ///// Clear all items in this SelectPanel. Note that this function will be removed in a future version. Please instead use <c>Items.Clear</c>.
        ///// </summary>
        //[Obsolete("This will be removed in a future version. You can instead use \"Items.Clear\".", false)]
        //public void Clear()
        //{
        //    Items.Clear();
        //}

        #endregion

        #region Move Items

        private sealed class SortByParentIndex : IComparer<IClickSelectableControl>
        {
            // A class to sort a collection of IClickSelectableControls by their index in the parent SelectableCollection.

#pragma warning disable CA1859 // for code readability purposes and future proofing, this will stay as it is
#if NETCOREAPP
            public IList<IClickSelectableControl>? ParentCollection { get; set; }

            public int Compare(IClickSelectableControl? a, IClickSelectableControl? b)
#else
            public IList<IClickSelectableControl> ParentCollection { get; set; }

            public int Compare(IClickSelectableControl a, IClickSelectableControl b)
#endif
#pragma warning restore CA1859
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
                        if (resel) isc.SelectItem(suc);
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
                        if (resel) isc.SelectItem(suc);
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

        /// <summary>
        /// A dependency property object backing the related property. See the property itself for more details.
        /// </summary>
        public static readonly DependencyProperty AllowParentScrollingProperty = DependencyProperty.Register(
            "AllowParentScrolling", typeof(bool), typeof(SelectPanel),
            new PropertyMetadata(true));

        /// <summary>
        /// Set whether the SelectPanel should allow its parent to scroll if the SelectPanel doesn't need to scroll. Note that enabling this may prevent any child items from scrolling.
        /// </summary>
        [Category("Common")]
        [Description("Set whether the SelectPanel should allow its parent to scroll if the SelectPanel doesn't need to scroll.")]
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

                        if (!previewEventArg.Handled && ((e.Delta > 0 && scrollControl.VerticalOffset == 0) || 
                            (e.Delta <= 0 && scrollControl.VerticalOffset >= scrollControl.ExtentHeight - scrollControl.ViewportHeight)))
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
