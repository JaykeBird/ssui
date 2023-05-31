using SolidShineUi.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SolidShineUi.Toolbars.Ribbon.Utils
{
    /// <summary>
    /// A visual rendering of a <see cref="RibbonTab"/>, to display on a <see cref="Ribbon"/>.
    /// </summary>
    public partial class RibbonTabDisplayItem : UserControl
    {
        public RibbonTabDisplayItem()
        {
            InitializeComponent();

            InternalParentChanged += tdi_InternalParentChanged;
            InternalTabItemChanged += tdi_InternalTabItemChanged;

            Click += (s, e) => 
            {
                if (!IsSelected)
                {
                    RequestSelect?.Invoke(this, e);
                }
            };
        }

        public RibbonTabDisplayItem(RibbonTab tab)
        {
            InitializeComponent();

            InternalParentChanged += tdi_InternalParentChanged;
            InternalTabItemChanged += tdi_InternalTabItemChanged;

            TabItem = tab;

            Click += (s, e) =>
            {
                if (!IsSelected)
                {
                    RequestSelect?.Invoke(this, e);
                }
            };
        }


        #region Selection

        public bool IsSelected { get => (bool)GetValue(IsSelectedProperty); set => SetValue(IsSelectedProperty, value); }

        public static DependencyProperty IsSelectedProperty
            = DependencyProperty.Register("IsSelected", typeof(bool), typeof(RibbonTabDisplayItem),
            new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsSelectedChanged)));

        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RibbonTabDisplayItem r)
            {
                r.InternalIsSelectedChanged();
            }
        }

        protected void InternalIsSelectedChanged()
        {
            if (IsSelected)
            {
                border.BorderThickness = TabBorderThickSelected;
                border.Background = Background;
            }
            else
            {
                border.BorderThickness = IsMouseOver ? TabBorderThickSelected : TabBorderThickStandard;
                border.Background = (IsMouseOver || IsKeyboardFocused) ? HighlightBrush : NearTransparent;
            }
        }

        public event EventHandler RequestSelect;

        #endregion

        #region Events
#if NETCOREAPP
        /// <summary>
        /// Raised when the control is right-clicked.
        /// </summary>
        public event EventHandler? RightClick;
        /// <summary>
        /// Raised when the control is clicked.
        /// </summary>
        public event EventHandler? Click;
        /// <summary>
        /// Raised when a TabItem is dropped onto this TabDisplayItem. Used as part of the TabControl's drag-and-drop system.
        /// </summary>
        public event RibbonTabItemDropEventHandler? TabItemDrop;
#else
        /// <summary>
        /// Raised when the control is right-clicked.
        /// </summary>
        public event EventHandler RightClick;
        /// <summary>
        /// Raised when the control is clicked.
        /// </summary>
        public event EventHandler Click;
        /// <summary>
        /// Raised when a TabItem is dropped onto this TabDisplayItem. Used as part of the TabControl's drag-and-drop system.
        /// </summary>
        public event RibbonTabItemDropEventHandler TabItemDrop;
#endif

        /// <summary>
        /// A delegate to be used with events regarding dropping a TabItem into a TabControl.
        /// </summary>
        /// <param name="sender">The object where the event was raised.</param>
        /// <param name="e">The event arguments associated with this event.</param>
        public delegate void RibbonTabItemDropEventHandler(object sender, RibbonTabItemDropEventArgs e);
        #endregion

        #region TabItem

        /// <summary>
        /// A dependency property object backing the related property. See the property itself for more details.
        /// </summary>
        public static readonly DependencyProperty TabItemProperty = DependencyProperty.Register("TabItem", typeof(RibbonTab), typeof(RibbonTabDisplayItem),
            new PropertyMetadata(null, new PropertyChangedCallback(OnInternalTabItemChanged)));

        /// <summary>
        /// The TabItem that this TabDisplayItem is representing. It is not advisable to change this property after the control is loaded; instead, just create a new TabDisplayItem.
        /// </summary>
        public RibbonTab TabItem
        {
            get { return (RibbonTab)GetValue(TabItemProperty); }
            set { SetValue(TabItemProperty, value); }
        }

        /// <summary>
        /// Internal event for handling a property changed. Please view the event that is not prefixed as "Internal".
        /// </summary>
#if NETCOREAPP
        protected event DependencyPropertyChangedEventHandler? InternalTabItemChanged;
#else
        protected event DependencyPropertyChangedEventHandler InternalTabItemChanged;
#endif

        private static void OnInternalTabItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RibbonTabDisplayItem s)
            {
                s.InternalTabItemChanged?.Invoke(s, e);
            }
        }

        private void tdi_InternalTabItemChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (TabItem != null)
            {
                TabItem.InternalBringIntoViewRequested += TabItem_InternalBringIntoViewRequested;
            }
        }

#if NETCOREAPP
        private void TabItem_InternalBringIntoViewRequested(object? sender, EventArgs e)
#else
        private void TabItem_InternalBringIntoViewRequested(object sender, EventArgs e)
#endif
        {
            BringIntoView();
        }
        #endregion

        #region ParentRibbon

        /// <summary>
        /// A dependency property object backing the related property. See the property itself for more details.
        /// </summary>
        public static readonly DependencyProperty ParentRibbonProperty = DependencyProperty.Register("ParentRibbon", typeof(Ribbon), typeof(RibbonTabDisplayItem),
            new PropertyMetadata(null, OnInternalParentChanged));

        /// <summary>
        /// Get or set the parent TabControl item that holds this tab item.
        /// </summary>
        public Ribbon ParentRibbon
        {
            get { return (Ribbon)GetValue(ParentRibbonProperty); }
            set { SetValue(ParentRibbonProperty, value); }
        }

        /// <summary>
        /// Internal event for handling a property changed. Please view the event that is not prefixed as "Internal".
        /// </summary>
        protected event DependencyPropertyChangedEventHandler InternalParentChanged;

        private static void OnInternalParentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RibbonTabDisplayItem s)
            {
                s.InternalParentChanged?.Invoke(s, e);
            }
        }

        private void tdi_InternalParentChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (ParentRibbon != null)
            {
                ParentRibbon.SetupTabDisplay(this);
            }
        }

        #endregion

        #region Color Scheme

        /// <summary>
        /// A dependency property object backing the related ColorScheme property. See <see cref="ColorScheme"/> for more details.
        /// </summary>
        public static readonly DependencyProperty ColorSchemeProperty
            = DependencyProperty.Register("ColorScheme", typeof(ColorScheme), typeof(RibbonTabDisplayItem),
            new FrameworkPropertyMetadata(new ColorScheme(), new PropertyChangedCallback(OnColorSchemeChanged)));

        /// <summary>
        /// Perform an action when the ColorScheme property has changed. Primarily used internally.
        /// </summary>
        /// <param name="d">The object containing the property that changed.</param>
        /// <param name="e">Event arguments about the property change.</param>
        public static void OnColorSchemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
#if NETCOREAPP
            if (d is RibbonTabDisplayItem w)
            {
                w.ApplyColorScheme((e.NewValue as ColorScheme)!);
            }
#else
            (d as RibbonTabDisplayItem).ApplyColorScheme(e.NewValue as ColorScheme);
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

            if (cs.IsHighContrast)
            {
                Background = cs.BackgroundColor.ToBrush();
                BorderBrush = cs.BorderColor.ToBrush();
                HighlightBrush = cs.HighlightColor.ToBrush();
                BorderHighlightBrush = cs.BorderColor.ToBrush();
            }
            else
            {
                Background = cs.ThirdHighlightColor.ToBrush();
                BorderBrush = cs.BorderColor.ToBrush();
                HighlightBrush = cs.SecondHighlightColor.ToBrush();
                BorderHighlightBrush = cs.HighlightColor.ToBrush();
            }

            if (highlighting)
            {
                border.Background = HighlightBrush;
                border.BorderBrush = BorderHighlightBrush;
            }
            else
            {
                border.Background = IsSelected ? Background : NearTransparent;
                border.BorderBrush = BorderBrush;
            }
        }
        #endregion

        #region Click Handling

        bool initiatingClick = false;

        void PerformClick(bool rightClick = false)
        {
            if (initiatingClick)
            {
                if (rightClick)
                {
                    RightClick?.Invoke(this, EventArgs.Empty);
                    return;
                }

                //if (SelectOnClick)
                //{
                //    IsSelected = !sel;
                //}

                Click?.Invoke(this, EventArgs.Empty);
                initiatingClick = false;
            }
        }

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            initiatingClick = true;
        }

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            PerformClick(e.ChangedButton == MouseButton.Right);
            e.Handled = true;
        }

        private void UserControl_TouchDown(object sender, TouchEventArgs e)
        {
            initiatingClick = true;
        }

        private void UserControl_TouchUp(object sender, TouchEventArgs e)
        {
            PerformClick();
        }

        private void UserControl_StylusDown(object sender, StylusDownEventArgs e)
        {
            initiatingClick = true;
        }

        private void UserControl_StylusUp(object sender, StylusEventArgs e)
        {
            PerformClick();
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Space)
            {
                initiatingClick = true;
            }
        }

        private void UserControl_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Space)
            {
                PerformClick();
            }
            else if (e.Key == Key.Apps)
            {
                RightClick?.Invoke(this, EventArgs.Empty);
            }
        }
        #endregion

        #region Focus Events
        bool highlighting = false;

        private void UserControl_GotFocus(object sender, RoutedEventArgs e)
        {
            if (IsEnabled)
            {
                border.Background = HighlightBrush;
                border.BorderBrush = BorderHighlightBrush;
                highlighting = true;
            }
        }

        private void UserControl_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (IsEnabled)
            {
                border.Background = HighlightBrush;
                border.BorderBrush = BorderHighlightBrush;
                highlighting = true;
            }
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            if (IsEnabled)
            {
                border.Background = HighlightBrush;
                border.BorderBrush = BorderHighlightBrush;
                border.BorderThickness = TabBorderThickSelected;
                highlighting = true;
            }
        }

        private void UserControl_LostFocus(object sender, RoutedEventArgs e)
        {
            border.Background = IsSelected ? Background : NearTransparent;
            border.BorderBrush = BorderBrush;
            highlighting = false;

            initiatingClick = false;
        }

        private void UserControl_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            border.Background = IsSelected ? Background : NearTransparent;
            border.BorderBrush = BorderBrush;
            highlighting = false;

            initiatingClick = false;
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!IsKeyboardFocused)
            {
                border.Background = IsSelected ? Background : NearTransparent;
                border.BorderBrush = BorderBrush;
                border.BorderThickness = IsSelected ? TabBorderThickSelected : TabBorderThickStandard;
                highlighting = false;
            }

            initiatingClick = false;
        }

        #endregion

        #region Brushes / Border

        /// <summary>
        /// Get or set the brush for the background while this TabDisplayItem is highlighted (i.e. the mouse is over it, or it has keyboard focus).
        /// </summary>
        public Brush HighlightBrush { get; set; } = new SolidColorBrush(Colors.LightGray);

        /// <summary>
        /// Get or set the brush for the border while this TabDisplayItem is highlighted (i.e. the mouse is over it, or it had keyboard focus).
        /// </summary>
        public Brush BorderHighlightBrush { get; set; } = new SolidColorBrush(Colors.DimGray);

        /// <summary>
        /// Get or set the brush for the border of this control.
        /// </summary>
        public new Brush BorderBrush { get; set; } = new SolidColorBrush(Colors.Black);

        private static Thickness TabBorderThickSelected = new Thickness(1, 1, 1, 0);
        private static Thickness TabBorderThickStandard = new Thickness(0, 0, 0, 1);
        private static Brush NearTransparent = Color.FromArgb(1, 1, 1, 1).ToBrush();

        private void border_IsKeyboardFocusedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (border.IsKeyboardFocused)
            {
                brdr_Focus.Visibility = Visibility.Visible;
            }
            else
            {
                brdr_Focus.Visibility = Visibility.Collapsed;
            }
        }
        #endregion

        #region Drag and Drop

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly DependencyProperty AllowDragDropProperty = DependencyProperty.Register("AllowDragDrop", typeof(bool), typeof(RibbonTabDisplayItem),
            new PropertyMetadata(true, new PropertyChangedCallback(OnAllowDragDropChanged)));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Get or set if the tab can be dragged and dropped.
        /// </summary>
        public bool AllowDragDrop
        {
            get { return (bool)GetValue(AllowDragDropProperty); }
            set { SetValue(AllowDragDropProperty, value); }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly DependencyProperty AllowDataDragDropProperty = DependencyProperty.Register("AllowDataDragDrop", typeof(bool), typeof(RibbonTabDisplayItem),
            new PropertyMetadata(true, new PropertyChangedCallback(OnAllowDragDropChanged)));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Get or set if data can be dropped onto this TabDisplayItem.
        /// </summary>
        public bool AllowDataDragDrop
        {
            get { return (bool)GetValue(AllowDataDragDropProperty); }
            set { SetValue(AllowDataDragDropProperty, value); }
        }

        private static void OnAllowDragDropChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TabDisplayItem tdi)
            {
                tdi.AllowDrop = tdi.AllowDataDragDrop || tdi.AllowDragDrop;
            }
        }

        #region Event Handlers
        private void control_DragEnter(object sender, DragEventArgs e)
        {
            if (AllowDragDrop && e.Data.GetData(typeof(RibbonTab)) != null)
            {
                if (e.Data.GetData(typeof(RibbonTab)) == TabItem)
                {
                    e.Effects = DragDropEffects.None;
                }
                else
                {
                    grdDrag.Visibility = Visibility.Visible;
                    e.Effects = DragDropEffects.Move;
                }
            }
            else if (AllowDataDragDrop)
            {
                // raise TabItem.DragEnter
                TabItem.RaiseDragEvent("DragEnter", e);
            }
        }

        private void control_Drop(object sender, DragEventArgs e)
        {
            if (AllowDragDrop && e.Data.GetData(typeof(RibbonTab)) != null)
            {
                // ideally, the grdDrag should handle this
                // if not, I'll fix this in a later version
            }
            else if (AllowDataDragDrop)
            {
                // raise TabItem.Drop
                TabItem.RaiseDragEvent("Drop", e);
            }
            grdDrag.Visibility = Visibility.Collapsed;
        }

        private void control_DragLeave(object sender, DragEventArgs e)
        {
            if (AllowDragDrop && e.Data.GetData(typeof(RibbonTab)) != null)
            {

            }
            else if (AllowDataDragDrop)
            {
                // raise TabItem.DragLeave
                TabItem.RaiseDragEvent("DragLeave", e);
            }

            grdDrag.Visibility = Visibility.Collapsed;
        }
        private void control_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && AllowDragDrop)
            {
                DragDropEffects _ = DragDrop.DoDragDrop(this, this.TabItem, DragDropEffects.Move);
            }
        }

        private void control_PreviewDragEnter(object sender, DragEventArgs e)
        {
            if (AllowDragDrop && e.Data.GetData(typeof(RibbonTab)) != null)
            {
                if (e.Data.GetData(typeof(RibbonTab)) == TabItem)
                {
                    e.Effects = DragDropEffects.None;
                }
                else
                {
                    e.Effects = DragDropEffects.Move;
                }
            }
            else if (AllowDataDragDrop)
            {
                TabItem.RaiseDragEvent("PreviewDragEnter", e);
            }
        }

        private void control_PreviewDragLeave(object sender, DragEventArgs e)
        {
            if (AllowDragDrop && e.Data.GetData(typeof(RibbonTab)) != null)
            {

            }
            else if (AllowDataDragDrop)
            {
                TabItem.RaiseDragEvent("PreviewDragLeave", e);
            }
        }

        private void control_PreviewDragOver(object sender, DragEventArgs e)
        {
            if (AllowDragDrop && e.Data.GetData(typeof(RibbonTab)) != null)
            {

            }
            else if (AllowDataDragDrop)
            {
                TabItem.RaiseDragEvent("PreviewDragOver", e);
            }
        }

        private void control_PreviewDrop(object sender, DragEventArgs e)
        {
            if (AllowDragDrop && e.Data.GetData(typeof(RibbonTab)) != null)
            {

            }
            else if (AllowDataDragDrop)
            {
                TabItem.RaiseDragEvent("PreviewDrop", e);
            }
        }

        private void control_DragOver(object sender, DragEventArgs e)
        {
            if (AllowDragDrop && e.Data.GetData(typeof(RibbonTab)) != null)
            {

            }
            else if (AllowDataDragDrop)
            {
                TabItem.RaiseDragEvent("DragOver", e);
            }
        }

        private void control_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            if (AllowDataDragDrop)
            {
                TabItem.RaiseGiveFeedbackEvent(e, false);
            }
        }

        private void control_PreviewGiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            if (AllowDataDragDrop)
            {
                TabItem.RaiseGiveFeedbackEvent(e, true);
            }
        }

        private void control_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            if (AllowDataDragDrop)
            {
                TabItem.RaiseQueryContinueDragEvent(e, false);
            }
        }

        private void control_PreviewQueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            if (AllowDataDragDrop)
            {
                TabItem.RaiseQueryContinueDragEvent(e, true);
            }
        }

        #endregion

        #region Drag border event handlers
        private void brdrDragLeft_DragEnter(object sender, DragEventArgs e)
        {
            brdrDragLeft.BorderThickness = new Thickness(5, 0, 0, 0);
        }

        private void brdrDragLeft_DragLeave(object sender, DragEventArgs e)
        {
            //grdDrag.Visibility = Visibility.Collapsed;
            brdrDragLeft.BorderThickness = new Thickness(0, 0, 0, 0);
        }

        private void brdrDragRght_DragEnter(object sender, DragEventArgs e)
        {
            brdrDragRght.BorderThickness = new Thickness(0, 0, 5, 0);
        }

        private void brdrDragRght_DragLeave(object sender, DragEventArgs e)
        {
            //grdDrag.Visibility = Visibility.Collapsed;
            brdrDragRght.BorderThickness = new Thickness(0, 0, 0, 0);
        }

        private void brdrDragLeft_Drop(object sender, DragEventArgs e)
        {
            grdDrag.Visibility = Visibility.Collapsed;
            if (e.Data.GetData(typeof(RibbonTab)) != null)
            {
                TabItemDrop?.Invoke(this, new RibbonTabItemDropEventArgs(TabItem, (RibbonTab)e.Data.GetData(typeof(RibbonTab)), true));
            }
        }

        private void brdrDragRght_Drop(object sender, DragEventArgs e)
        {
            grdDrag.Visibility = Visibility.Collapsed;
            if (e.Data.GetData(typeof(RibbonTab)) != null)
            {
                TabItemDrop?.Invoke(this, new RibbonTabItemDropEventArgs(TabItem, (RibbonTab)e.Data.GetData(typeof(RibbonTab)), false));
            }
        }
        #endregion

        #endregion

    }


    /// <summary>
    /// Event arguments for a RibbonTab being dropped onto a Ribbon. Primarily used internally.
    /// </summary>
    public class RibbonTabItemDropEventArgs
    {
        /// <summary>
        /// Create a RibbonTabItemDropEventArgs.
        /// </summary>
        /// <param name="sourceTabItem">The tab item that triggered the event.</param>
        /// <param name="droppedTabItem">The tab item to be dropped.</param>
        /// <param name="before">Determine whether to drop the <paramref name="droppedTabItem"/> before or after the <paramref name="sourceTabItem"/>.</param>
        public RibbonTabItemDropEventArgs(RibbonTab sourceTabItem, RibbonTab droppedTabItem, bool before)
        {
            SourceTabItem = sourceTabItem;
            DroppedTabItem = droppedTabItem;
            PlaceBefore = before;
        }

        /// <summary>
        /// Get the TabItem that triggered the TabItemDrop event.
        /// </summary>
        public RibbonTab SourceTabItem { get; private set; }

        /// <summary>
        /// Get the TabItem that is being dropped.
        /// </summary>
        public RibbonTab DroppedTabItem { get; private set; }

        /// <summary>
        /// Get whether the dropped TabItem should be put before or after the source TabItem.
        /// </summary>
        public bool PlaceBefore { get; private set; }
    }
}
