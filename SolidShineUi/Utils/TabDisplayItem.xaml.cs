using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TabItem = SolidShineUi.TabItem;

namespace SolidShineUi.Utils
{
    /// <summary>
    /// Interaction logic for TabDisplayItem.xaml
    /// </summary>
    public partial class TabDisplayItem : UserControl
    {
#if NETCOREAPP
        public event EventHandler? RequestClose;
        public event EventHandler? RightClick;
        public event EventHandler? Click;
        public event TabItemDropEventHandler? TabItemDrop;
#else
        public event EventHandler RequestClose;
        public event EventHandler RightClick;
        public event EventHandler Click;
        public event TabItemDropEventHandler TabItemDrop;
#endif

        public delegate void TabItemDropEventHandler(object sender, TabItemDropEventArgs e);

        public Brush HighlightBrush { get; set; } = new SolidColorBrush(Colors.LightGray);
        public Brush BorderHighlightBrush { get; set; } = new SolidColorBrush(Colors.DimGray);
        public new Brush BorderBrush { get; set; } = new SolidColorBrush(Colors.Black);

        private Thickness TabBorderThickSelected = new Thickness(1, 1, 1, 0);

        public TabDisplayItem()
        {
            InitializeComponent();
            //TabItem = new TabItem();

            //if (tab.Icon != null)
            //{
            //    imgIcon.Source = tab.Icon;
            //}
            //lblTitle.Text = tab.Title;
            if (IsSelected)
            {
                border.BorderThickness = TabBorderThickSelected;
            }
            else
            {
                border.BorderThickness = new Thickness(1, 1, 1, 1);
            }

            //btnClose.Visibility = tab.CanClose ? Visibility.Visible : Visibility.Collapsed;
            //colClose.Width = tab.CanClose ? new GridLength(18) : new GridLength(0);

            //tab.IsSelectedChanged += tab_IsSelectedChanged;

            InternalParentChanged += tdi_InternalParentChanged;
            InternalIsSelectedChanged += tdi_InternalIsSelectedChanged;
            InternalShowTabsOnBottomChanged += tdi_InternalShowTabsOnBottomChanged;
            InternalTabItemChanged += tdi_InternalTabITemChanged;
        }

        public TabDisplayItem(TabItem tab)
        {
            InitializeComponent();
            TabItem = tab;

            //if (tab.Icon != null)
            //{
            //    imgIcon.Source = tab.Icon;
            //}
            //lblTitle.Text = tab.Title;
            if (IsSelected)
            {
                border.BorderThickness = TabBorderThickSelected;
            }
            else
            {
                border.BorderThickness = new Thickness(1, 1, 1, 1);
            }

            //btnClose.Visibility = tab.CanClose ? Visibility.Visible : Visibility.Collapsed;
            //colClose.Width = tab.CanClose ? new GridLength(18) : new GridLength(0);

            //tab.IsSelectedChanged += tab_IsSelectedChanged;

            InternalParentChanged += tdi_InternalParentChanged;
            InternalIsSelectedChanged += tdi_InternalIsSelectedChanged;
        }

#if NETCOREAPP
        private void tab_IsSelectedChanged(object? sender, EventArgs e)
#else
        private void tab_IsSelectedChanged(object sender, EventArgs e)
#endif
        {
            if (IsSelected)
            {
                border.BorderThickness = TabBorderThickSelected;
            }
            else
            {
                border.BorderThickness = new Thickness(1, 1, 1, 1);
            }
        }


        //#region Icon

        //public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(ImageSource), typeof(TabDisplayItem),
        //    new PropertyMetadata(null));

        //public ImageSource Icon
        //{
        //    get { return (ImageSource)GetValue(IconProperty); }
        //    set { SetValue(IconProperty, value); }
        //}

        //#endregion

        #region CanSelect

        public static readonly DependencyProperty CanSelectProperty = DependencyProperty.Register("CanSelect", typeof(bool), typeof(TabDisplayItem),
            new PropertyMetadata(true));

        public bool CanSelect
        {
            get { return (bool)GetValue(CanSelectProperty); }
            set { SetValue(CanSelectProperty, value); }
        }
        #endregion

        //#region Title

        //public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(TabDisplayItem),
        //    new PropertyMetadata("New Tab"));

        //public string Title
        //{
        //    get { return (string)GetValue(TitleProperty); }
        //    set { SetValue(TitleProperty, value); }
        //}
        //#endregion

        #region IsDirty

        public static readonly DependencyProperty IsDirtyProperty = DependencyProperty.Register("IsDirty", typeof(bool), typeof(TabDisplayItem),
            new PropertyMetadata(false));

        public bool IsDirty
        {
            get { return (bool)GetValue(IsDirtyProperty); }
            set { SetValue(IsDirtyProperty, value); }
        }
        #endregion

        //#region CanClose

        //public static readonly DependencyProperty CanCloseProperty = DependencyProperty.Register("CanClose", typeof(bool), typeof(TabDisplayItem),
        //    new PropertyMetadata(true));

        //public bool CanClose
        //{
        //    get { return (bool)GetValue(CanCloseProperty); }
        //    set { SetValue(CanCloseProperty, value); }
        //}
        //#endregion

        #region IsSelected

        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(TabDisplayItem),
            new PropertyMetadata(false, OnIsSelectedChanged));

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            internal protected set { SetValue(IsSelectedProperty, value); }
        }

        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TabDisplayItem i)
            {
                i.InternalIsSelectedChanged?.Invoke(i, e);
            }
        }

        protected event DependencyPropertyChangedEventHandler InternalIsSelectedChanged;

        private void tdi_InternalIsSelectedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsSelected)
            {
                border.BorderThickness = TabBorderThickSelected;
            }
            else
            {
                border.BorderThickness = new Thickness(1, 1, 1, 1);
            }
        }

        #endregion

        #region TabItem

        public static readonly DependencyProperty TabItemProperty = DependencyProperty.Register("TabItem", typeof(TabItem), typeof(TabDisplayItem),
            new PropertyMetadata(null, new PropertyChangedCallback(OnInternalTabItemChanged)));

        public TabItem TabItem
        {
            get { return (TabItem)GetValue(TabItemProperty); }
            set { SetValue(TabItemProperty, value); }
        }

#if NETCOREAPP
        protected event DependencyPropertyChangedEventHandler? InternalTabItemChanged;
#else
        protected event DependencyPropertyChangedEventHandler InternalTabItemChanged;
#endif

        private static void OnInternalTabItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TabDisplayItem s)
            {
                s.InternalTabItemChanged?.Invoke(s, e);
            }
        }

        private void tdi_InternalTabITemChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (TabItem != null)
            {
                TabItem.InternalTabClosing += TabItem_InternalTabClosing;
            }
        }

#if NETCOREAPP
        private void TabItem_InternalTabClosing(object? sender, EventArgs e)
#else
        private void TabItem_InternalTabClosing(object sender, EventArgs e)
#endif
        {
            RequestClose?.Invoke(this, e);
            //throw new NotImplementedException();
        }
        #endregion

        #region ShowTabsOnBottom

        public static readonly DependencyProperty ShowTabsOnBottomProperty = DependencyProperty.Register("ShowTabsOnBottom", typeof(bool), typeof(TabDisplayItem),
            new PropertyMetadata(false, new PropertyChangedCallback(OnInternalShowTabsOnBottomChanged)));

        public bool ShowTabsOnBottom
        {
            get { return (bool)GetValue(ShowTabsOnBottomProperty); }
            set { SetValue(ShowTabsOnBottomProperty, value); }
        }

#if NETCOREAPP
        protected event DependencyPropertyChangedEventHandler? InternalShowTabsOnBottomChanged;
#else
        protected event DependencyPropertyChangedEventHandler InternalShowTabsOnBottomChanged;
#endif

        private static void OnInternalShowTabsOnBottomChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TabDisplayItem s)
            {
                s.InternalShowTabsOnBottomChanged?.Invoke(s, e);
            }
        }
        private void tdi_InternalShowTabsOnBottomChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (ShowTabsOnBottom)
            {
                TabBorderThickSelected = new Thickness(1, 0, 1, 1);
            }
            else
            {
                TabBorderThickSelected = new Thickness(1, 1, 1, 0);
            }

            if (IsSelected)
            {
                border.BorderThickness = TabBorderThickSelected;
            }
            else
            {
                border.BorderThickness = new Thickness(1, 1, 1, 1);
            }
        }
        #endregion

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            RequestClose?.Invoke(this, e);
        }

        #region ParentTabControl

        public static readonly DependencyProperty ParentTabControlProperty = DependencyProperty.Register("ParentTabControl", typeof(TabControl), typeof(TabDisplayItem),
            new PropertyMetadata(null, OnInternalParentChanged));

        public TabControl ParentTabControl
        {
            get { return (TabControl)GetValue(ParentTabControlProperty); }
            set { SetValue(ParentTabControlProperty, value); }
        }

        protected event DependencyPropertyChangedEventHandler InternalParentChanged;

        private static void OnInternalParentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TabDisplayItem s)
            {
                s.InternalParentChanged?.Invoke(s, e);
            }
        }
        private void tdi_InternalParentChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (ParentTabControl != null)
            {
                ParentTabControl.SetupTabDisplay(this);
            }
        }

        #endregion

        #region Color Scheme

        public static readonly DependencyProperty ColorSchemeProperty
            = DependencyProperty.Register("ColorScheme", typeof(ColorScheme), typeof(TabDisplayItem),
            new FrameworkPropertyMetadata(new ColorScheme(), new PropertyChangedCallback(OnColorSchemeChanged)));

        public static void OnColorSchemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
#if NETCOREAPP
            if (d is TabDisplayItem w)
            {
                w.ApplyColorScheme((e.NewValue as ColorScheme)!);
            }
#else
            (d as TabDisplayItem).ApplyColorScheme(e.NewValue as ColorScheme);
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
                border.Background = Background;
                border.BorderBrush = BorderBrush;
            }
        }

        public void ApplyColorScheme(HighContrastOption hco)
        {
            ColorScheme cs = ColorScheme.GetHighContrastScheme(hco);

            ApplyColorScheme(cs);
        }
        #endregion

        #region Click Handling

        #region Variables/Properties
        bool initiatingClick = false;

        //bool sel = false;

        //public bool IsSelected
        //{
        //    get
        //    {
        //        return sel;
        //    }
        //    set
        //    {
        //        sel = value;

        //        if (sel)
        //        {
        //            border.Background = SelectionBrush;
        //        }
        //        else
        //        {
        //            border.Background = Background;
        //        }

        //        SelectionChanged?.Invoke(this, EventArgs.Empty);
        //    }
        //}

        #endregion

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
                highlighting = true;
            }
        }

        private void UserControl_LostFocus(object sender, RoutedEventArgs e)
        {
            border.Background = Background;
            border.BorderBrush = BorderBrush;
            highlighting = false;

            initiatingClick = false;
        }

        private void UserControl_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            border.Background = Background;
            border.BorderBrush = BorderBrush;
            highlighting = false;

            initiatingClick = false;
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!IsKeyboardFocused)
            {
                border.Background = Background;
                border.BorderBrush = BorderBrush;
                highlighting = false;
            }

            initiatingClick = false;
        }

        #endregion

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

        private void control_Loaded(object sender, RoutedEventArgs e)
        {
            if (ParentTabControl != null)
            {

            }
        }

        #region Drag and Drop

        public static readonly DependencyProperty AllowDragDropProperty = DependencyProperty.Register("AllowDragDrop", typeof(bool), typeof(TabDisplayItem),
            new PropertyMetadata(true));

        /// <summary>
        /// Get or set if the tab can be dragged and dropped.
        /// </summary>
        public bool AllowDragDrop
        {
            get { return (bool)GetValue(AllowDragDropProperty); }
            set { SetValue(AllowDragDropProperty, value); }
        }

        private void control_DragEnter(object sender, DragEventArgs e)
        {
            if (AllowDragDrop)
            {
                if (e.Data.GetData(typeof(TabItem)) != null)
                {
                    if (e.Data.GetData(typeof(TabItem)) == TabItem)
                    {
                        e.Effects = DragDropEffects.None;
                    }
                    else
                    {
                        grdDrag.Visibility = Visibility.Visible;
                        e.Effects = DragDropEffects.Move;
                    }
                }
                else
                {
                    // raise TabItem.DragEnter
                }
            }
        }

        private void control_Drop(object sender, DragEventArgs e)
        {
            // raise TabItem.Drop
            grdDrag.Visibility = Visibility.Collapsed;
        }

        private void control_DragLeave(object sender, DragEventArgs e)
        {
            // raise TabItem.DragLeave
            grdDrag.Visibility = Visibility.Collapsed;
        }

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
            if (e.Data.GetData(typeof(TabItem)) != null)
            {
                TabItemDrop?.Invoke(this, new TabItemDropEventArgs(TabItem, (TabItem)e.Data.GetData(typeof(TabItem)), true));
            }
        }

        private void brdrDragRght_Drop(object sender, DragEventArgs e)
        {
            grdDrag.Visibility = Visibility.Collapsed;
            if (e.Data.GetData(typeof(TabItem)) != null)
            {
                TabItemDrop?.Invoke(this, new TabItemDropEventArgs(TabItem, (TabItem)e.Data.GetData(typeof(TabItem)), false));
            }
        }
        #endregion

        private void control_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && AllowDragDrop)
            {
                DragDropEffects ee = DragDrop.DoDragDrop(this, this.TabItem, DragDropEffects.Move);
            }
        }

        private void control_StylusMove(object sender, StylusEventArgs e)
        {

        }
    }

    public class TabItemDropEventArgs
    {
        public TabItemDropEventArgs(TabItem sourceTabItem, TabItem droppedTabItem, bool before)
        {
            SourceTabItem = sourceTabItem;
            DroppedTabItem = droppedTabItem;
            Before = before;
        }

        public TabItem SourceTabItem { get; private set; }

        public TabItem DroppedTabItem { get; private set; }

        public bool Before { get; private set; }
    }
}