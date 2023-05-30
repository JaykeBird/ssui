using SolidShineUi.Toolbars.Ribbon.Utils;
using SolidShineUi.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace SolidShineUi.Toolbars.Ribbon
{
    /// <summary>
    /// A toolbar that displays various commands under a series of tabs, similar to what is present in Microsoft Office or Autodesk software.
    /// </summary>
    [ContentProperty("Items")]
    public class Ribbon : Control
    {
        static Ribbon()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Ribbon), new FrameworkPropertyMetadata(typeof(Ribbon)));
        }

        /// <summary>
        /// Create a Ribbon.
        /// </summary>
        public Ribbon()
        {
            SetValue(ItemsPropertyKey, new ObservableCollection<RibbonTab>());
        }

        #region Template IO
        /// <inheritdoc/>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            LoadTemplateItems();
        }

        bool itemsLoaded = false;

        bool _internalAction = false;

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
        #endregion

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        private static readonly DependencyPropertyKey ItemsPropertyKey
            = DependencyProperty.RegisterReadOnly("Items", typeof(ObservableCollection<RibbonTab>), typeof(Ribbon),
            new FrameworkPropertyMetadata(new ObservableCollection<RibbonTab>()));

        public static readonly DependencyProperty ItemsProperty = ItemsPropertyKey.DependencyProperty;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Get or set the list of tabs in this Ribbon. This Items property can be used to add and remove items.
        /// </summary>
        [Category("Common")]
        public ObservableCollection<RibbonTab> Items
        {
            get { return (ObservableCollection<RibbonTab>)GetValue(ItemsProperty); }
            private set { SetValue(ItemsPropertyKey, value); }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly DependencyProperty TopRightElementProperty = DependencyProperty.Register(
            "TopRightElement", typeof(UIElement), typeof(Ribbon),
            new PropertyMetadata(null));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Gets or sets the UI element to place in the top-right of the control.
        /// </summary>
        public UIElement TopRightElement
        {
            get
            {
                return (UIElement)GetValue(TopRightElementProperty);
            }
            set
            {
                SetValue(TopRightElementProperty, value);
            }
        }

        public bool ShowOnlyTabs { get => (bool)GetValue(ShowOnlyTabsProperty); set => SetValue(ShowOnlyTabsProperty, value); }

        public static DependencyProperty ShowOnlyTabsProperty
            = DependencyProperty.Register("ShowOnlyTabs", typeof(bool), typeof(Ribbon),
            new FrameworkPropertyMetadata(false));

        public bool AllowControlReordering { get => (bool)GetValue(AllowControlReorderingProperty); set => SetValue(AllowControlReorderingProperty, value); }

        public static DependencyProperty AllowControlReorderingProperty
            = DependencyProperty.Register("AllowControlReordering", typeof(bool), typeof(Ribbon),
            new FrameworkPropertyMetadata(true));

        #region Color Scheme
        /// <summary>
        /// Raised when the ColorScheme property is changed.
        /// </summary>
#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? ColorSchemeChanged;
#else
        public event DependencyPropertyChangedEventHandler ColorSchemeChanged;
#endif

        /// <summary>
        /// A dependency property object backing the related ColorScheme property. See <see cref="ColorScheme"/> for more details.
        /// </summary>
        public static readonly DependencyProperty ColorSchemeProperty
            = DependencyProperty.Register("ColorScheme", typeof(ColorScheme), typeof(Ribbon),
            new FrameworkPropertyMetadata(new ColorScheme(), new PropertyChangedCallback(OnColorSchemeChanged)));

        /// <summary>
        /// Perform an action when the ColorScheme property has changed. Primarily used internally.
        /// </summary>
        /// <param name="d">The object containing the property that changed.</param>
        /// <param name="e">Event arguments about the property change.</param>
        public static void OnColorSchemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
#if NETCOREAPP
            ColorScheme cs = (e.NewValue as ColorScheme)!;
#else
            ColorScheme cs = e.NewValue as ColorScheme;
#endif

            if (d is Ribbon c)
            {
                c.ColorSchemeChanged?.Invoke(d, e);
                c.ApplyColorScheme(cs);
            }
        }

        /// <summary>
        /// Get or set the color scheme used for this control. The color scheme can quickly apply a whole visual style to your control.
        /// </summary>
        public ColorScheme ColorScheme
        {
            get => (ColorScheme)GetValue(ColorSchemeProperty);
            set => SetValue(ColorSchemeProperty, value);
        }

        /// <summary>
        /// Apply a color scheme to this control. The color scheme can quickly apply a whole visual style to the control.
        /// </summary>
        /// <param name="cs">The color scheme to apply.</param>
        public void ApplyColorScheme(ColorScheme cs)
        {
            if (cs == null)
            {
                return;
            }
            if (cs != ColorScheme)
            {
                ColorScheme = cs;
                return;
            }
        }
        #endregion

        #region Setup TabDisplayItem / Tdi Event Handlers

        /// <summary>
        /// Set up a new TabDisplayItem that was added to this TabControl.
        /// This will set up the necessary event handlers and other properties to allow the TabDisplayItem to interact with the TabControl.
        /// </summary>
        /// <param name="tdi">The TabDisplayItem to set up.</param>
        internal protected void SetupTabDisplay(RibbonTabDisplayItem tdi)
        {
            tdi.Click += tdi_Click;
            tdi.RightClick += tdi_RightClick;
            //tdi.TabItemDrop += tdi_TabItemDrop;

            //CheckScrolling();
        }

#if NETCOREAPP
        private void tdi_RightClick(object? sender, EventArgs e)
#else
        private void tdi_RightClick(object sender, EventArgs e)
#endif
        {
            if (sender is RibbonTabDisplayItem tdi)
            {
                //if (tdi.TabItem.TabContextMenu != null)
                //{
                //    ContextMenu cm = tdi.TabItem.TabContextMenu;
                //    cm.ColorScheme = ColorScheme;
                //    cm.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
                //    cm.IsOpen = true;
                //}
            }
        }

        private void tdi_TabItemDrop(object sender, RibbonTabItemDropEventArgs e)
        {
            if (e.DroppedTabItem == e.SourceTabItem) return;
            if (!Items.Contains(e.DroppedTabItem)) return;

            _internalAction = true;
//#if NETCOREAPP
//            TabItem? selItem = null;
//#else
//            TabItem selItem = null;
//#endif
            //if (Items.SelectedItems.Count != 0)
            //{
            //    selItem = Items.SelectedItems.First();
            //}
            //Items.ClearSelection();

            Items.Remove(e.DroppedTabItem);
            //if (Items.Contains(e.DroppedTabItem))
            //{
            //    Items.Remove(e.DroppedTabItem);
            //}
            //else
            //{
            //    // get source tab control and remove it from there before adding here
            //}

            int newIndex = e.PlaceBefore ? Items.IndexOf(e.SourceTabItem) : Items.IndexOf(e.SourceTabItem) + 1;

            if (newIndex == -1)
            {
                Items.Add(e.DroppedTabItem);
            }
            else
            {
                Items.Insert(newIndex, e.DroppedTabItem);
            }

            //if (selItem != null)
            //{
            //    Items.Select(selItem);
            //}

            // fix to make sure the correct tab has the IsSelected state
            //if (ic != null)
            //{
            //    for (int i = 0; i < ic.Items.Count; i++)
            //    {
            //        // I really dislike this roundabout way that I have to get the child items of an ItemsControl, but I guess this is how it is
            //        // from https://stackoverflow.com/a/1876534/2987285
            //        ContentPresenter c = (ContentPresenter)ic.ItemContainerGenerator.ContainerFromItem(ic.Items[i]);
            //        c.ApplyTemplate();
            //        //#if NETCOREAPP
            //        //#else
            //        //                    TabDisplayItem tb = c.ContentTemplate.FindName("PART_TabItem", c) as TabDisplayItem;
            //        //#endif
            //        if (c.ContentTemplate.FindName("PART_TabItem", c) is TabDisplayItem tb)
            //        {
            //            if (tb.TabItem != null && tb.TabItem == selItem)
            //            {
            //                tb.IsSelected = true;
            //            }
            //            else
            //            {
            //                tb.IsSelected = false;
            //            }
            //        }
            //    }
            //}

            _internalAction = false;
        }

#if NETCOREAPP
        private void tdi_Click(object? sender, EventArgs e)
#else
        private void tdi_Click(object sender, EventArgs e)
#endif
        {
            //if (sender != null && sender is RibbonTabDisplayItem tdi)
            //{
            //    if (tdi.TabItem != null && tdi.CanSelect)
            //    {
            //        Items.Select(tdi.TabItem);
            //    }
            //}
        }


        #endregion

        #region Scrolling

        #region ScrollButtons

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

        private static readonly DependencyPropertyKey ScrollButtonsVisiblePropertyKey = DependencyProperty.RegisterReadOnly("ScrollButtonsVisible", typeof(bool), typeof(Ribbon),
            new PropertyMetadata(false));

        public static readonly DependencyProperty ScrollButtonsVisibleProperty = ScrollButtonsVisiblePropertyKey.DependencyProperty;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Get if the scroll buttons are currently visible in the tab bar.
        /// </summary>
        public bool ScrollButtonsVisible
        {
            get { return (bool)GetValue(ScrollButtonsVisibleProperty); }
            private set { SetValue(ScrollButtonsVisiblePropertyKey, value); }
        }
        #endregion

        /// <summary>
        /// A WPF command that when executed, will scroll the tab control's tab bar to the left or right.
        /// </summary>
        public static readonly RoutedCommand TabBarScrollCommand = new RoutedCommand("TabBarScrollCommand", typeof(Ribbon));

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

        private void OnScrollCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (sv == null) return;

            double offset = sv.HorizontalOffset;

            if (e.Parameter is TabScrollCommandAction a)
            {
                switch (a)
                {
                    case TabScrollCommandAction.Left:
                        sv.ScrollToHorizontalOffset(Math.Max(offset - 20, 0));
                        break;
                    case TabScrollCommandAction.Right:
                        sv.ScrollToHorizontalOffset(Math.Min(offset + 20, sv.ScrollableWidth));
                        break;
                    case TabScrollCommandAction.Home:
                        sv.ScrollToHorizontalOffset(0);
                        break;
                    case TabScrollCommandAction.End:
                        sv.ScrollToHorizontalOffset(sv.ScrollableWidth);
                        break;
                    default:
                        break;
                }
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
}
