using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TabItem = SolidShineUi.TabItem;

namespace SolidShineUi.Utils
{
    /// <summary>
    /// A visual rendering of a <see cref="SolidShineUi.TabItem" />, to display in a <see cref="TabControl"/>.
    /// </summary>
    public partial class TabDisplayItem : UserControl
    {

        #region Constructors
        /// <summary>
        /// Create a TabDisplayItem.
        /// </summary>
        public TabDisplayItem()
        {
            InitializeComponent();
            //TabItem = new TabItem();

            //if (tab.Icon != null)
            //{
            //    imgIcon.Source = tab.Icon;
            //}
            //lblTitle.Text = tab.Title;
            border.BorderThickness = IsSelected ? TabBorderThickSelected : new Thickness(1, 1, 1, 1);
            //border.Background = IsSelected ? SelectedTabBackground : Background;

            //btnClose.Visibility = tab.CanClose ? Visibility.Visible : Visibility.Collapsed;
            //colClose.Width = tab.CanClose ? new GridLength(18) : new GridLength(0);

            //tab.IsSelectedChanged += tab_IsSelectedChanged;

            InternalParentChanged += tdi_InternalParentChanged;
            InternalIsSelectedChanged += tdi_InternalIsSelectedChanged;
            InternalShowTabsOnBottomChanged += tdi_InternalShowTabsOnBottomChanged;
            InternalTabItemChanged += tdi_InternalTabItemChanged;
            InternalTabBackgroundChanged += tdi_InternalTabBackgroundChanged;
        }

        /// <summary>
        /// Create a TabDisplayItem.
        /// </summary>
        /// <param name="tab">The base TabItem that this TabDisplayItem represents.</param>
        public TabDisplayItem(TabItem tab)
        {
            InitializeComponent();
            TabItem = tab;

            //if (tab.Icon != null)
            //{
            //    imgIcon.Source = tab.Icon;
            //}
            //lblTitle.Text = tab.Title;
            border.BorderThickness = IsSelected ? TabBorderThickSelected : new Thickness(1, 1, 1, 1);
            //border.Background = IsSelected ? SelectedTabBackground : Background;

            //btnClose.Visibility = tab.CanClose ? Visibility.Visible : Visibility.Collapsed;
            //colClose.Width = tab.CanClose ? new GridLength(18) : new GridLength(0);

            //tab.IsSelectedChanged += tab_IsSelectedChanged;

            InternalParentChanged += tdi_InternalParentChanged;
            InternalIsSelectedChanged += tdi_InternalIsSelectedChanged;
            InternalShowTabsOnBottomChanged += tdi_InternalShowTabsOnBottomChanged;
            InternalTabItemChanged += tdi_InternalTabItemChanged;
            InternalTabBackgroundChanged += tdi_InternalTabBackgroundChanged;
        }

        private void control_Loaded(object sender, RoutedEventArgs e)
        {
            if (ParentTabControl != null)
            {

            }
        }
        #endregion

        #region Events
#if NETCOREAPP
        /// <summary>
        /// Raised when the Close button is clicked, and this tab wants to be closed.
        /// </summary>
        public event EventHandler? RequestClose;
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
        public event TabItemDropEventHandler? TabItemDrop;
#else
        /// <summary>
        /// Raised when the Close button is clicked, and this tab wants to be closed.
        /// </summary>
        public event EventHandler RequestClose;
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
        public event TabItemDropEventHandler TabItemDrop;
#endif

        /// <summary>
        /// A delegate to be used with events regarding dropping a TabItem into a TabControl.
        /// </summary>
        /// <param name="sender">The object where the event was raised.</param>
        /// <param name="e">The event arguments associated with this event.</param>
        public delegate void TabItemDropEventHandler(object sender, TabItemDropEventArgs e);
        #endregion

        #region Brushes / Border

        /// <summary>
        /// Get or set the brush for the background while this TabDisplayItem is highlighted (i.e. the mouse is over it, or it has keyboard focus).
        /// </summary>
        public Brush HighlightBrush { get => (Brush)GetValue(HighlightBrushProperty); set => SetValue(HighlightBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="HighlightBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty HighlightBrushProperty
            = DependencyProperty.Register(nameof(HighlightBrush), typeof(Brush), typeof(TabDisplayItem),
            new FrameworkPropertyMetadata(Colors.LightGray.ToBrush()));

        /// <summary>
        /// Get or set the brush for the border while this TabDisplayItem is highlighted (i.e. the mouse is over it, or it had keyboard focus).
        /// </summary>
        public Brush BorderHighlightBrush { get => (Brush)GetValue(BorderHighlightBrushProperty); set => SetValue(BorderHighlightBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="BorderHighlightBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty BorderHighlightBrushProperty
            = DependencyProperty.Register(nameof(BorderHighlightBrush), typeof(Brush), typeof(TabDisplayItem),
            new FrameworkPropertyMetadata(Colors.DimGray.ToBrush()));

        /// <summary>
        /// Get or set the brush for the border of this control.
        /// </summary>
        public Brush TabBorderBrush { get => (Brush)GetValue(TabBorderBrushProperty); set => SetValue(TabBorderBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="TabBorderBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty TabBorderBrushProperty
            = DependencyProperty.Register(nameof(TabBorderBrush), typeof(Brush), typeof(TabDisplayItem),
            new FrameworkPropertyMetadata(Colors.Black.ToBrush(), OnInternalTabBorderBrushChanged));

        private static void OnInternalTabBorderBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TabDisplayItem s)
            {
                s.InternalTabBorderBrushChanged();
            }
        }

        private void InternalTabBorderBrushChanged()
        {
            //if (highlighting)
            //{
            //    //border.Background = HighlightBrush;
            //    border.BorderBrush = BorderHighlightBrush;
            //}
            //else
            //{
            //    //border.Background = IsSelected ? SelectedTabBackground : Background;
            //    border.BorderBrush = TabBorderBrush;
            //}
        }

        /// <summary>
        /// Get or set the brush used for the close glyph in this control.
        /// </summary>
        public Brush CloseBrush { get => (Brush)GetValue(CloseBrushProperty); set => SetValue(CloseBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="CloseBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty CloseBrushProperty
            = DependencyProperty.Register(nameof(CloseBrush), typeof(Brush), typeof(TabDisplayItem),
            new FrameworkPropertyMetadata(Colors.Black.ToBrush()));

        /// <summary>
        /// Get or set the background for the tab while it is selected (<see cref="IsSelected"/> is <c>true</c>).
        /// </summary>
        public Brush SelectedTabBackground { get => (Brush)GetValue(SelectedTabBackgroundProperty); set => SetValue(SelectedTabBackgroundProperty, value); }

        /// <summary>The backing dependency property for <see cref="SelectedTabBackground"/>. See the related property for details.</summary>
        public static readonly DependencyProperty SelectedTabBackgroundProperty
            = DependencyProperty.Register(nameof(SelectedTabBackground), typeof(Brush), typeof(TabDisplayItem),
            new FrameworkPropertyMetadata(Colors.White.ToBrush()));

        /// <summary>
        /// Get or set the brush used for the close button, when it is highlighted (i.e. mouse over).
        /// </summary>
        public Brush ButtonHighlightBackground { get => (Brush)GetValue(ButtonHighlightBackgroundProperty); set => SetValue(ButtonHighlightBackgroundProperty, value); }

        /// <summary>The backing dependency property for <see cref="ButtonHighlightBackground"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ButtonHighlightBackgroundProperty
            = DependencyProperty.Register(nameof(ButtonHighlightBackground), typeof(Brush), typeof(TabDisplayItem),
            new FrameworkPropertyMetadata(Colors.Silver.ToBrush()));

        /// <summary>
        /// Get or set the brush used for the close button, when it is highlighted (i.e. mouse over).
        /// </summary>
        public Brush ButtonHighlightBorderBrush { get => (Brush)GetValue(ButtonHighlightBorderBrushProperty); set => SetValue(ButtonHighlightBorderBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="ButtonHighlightBorderBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ButtonHighlightBorderBrushProperty
            = DependencyProperty.Register(nameof(ButtonHighlightBorderBrush), typeof(Brush), typeof(TabDisplayItem),
            new FrameworkPropertyMetadata(Colors.DimGray.ToBrush()));

        /// <summary>
        /// Get or set the brush used for the close button, when it is being clicked (i.e. mouse down, key down).
        /// </summary>
        public Brush ButtonClickBrush { get => (Brush)GetValue(ButtonClickBrushProperty); set => SetValue(ButtonClickBrushProperty, value); }

        /// <summary>The backing dependency property for <see cref="ButtonClickBrush"/>. See the related property for details.</summary>
        public static readonly DependencyProperty ButtonClickBrushProperty
            = DependencyProperty.Register(nameof(ButtonClickBrush), typeof(Brush), typeof(TabDisplayItem),
            new FrameworkPropertyMetadata(Colors.LightGray.ToBrush()));

        //private Brush _innerColor = new SolidColorBrush(Colors.Transparent);

        #region TabBackground
        /// <summary>
        /// A dependency property object backing the related property. See the property itself for more details.
        /// </summary>
        public static readonly DependencyProperty TabBackgroundProperty = DependencyProperty.Register(
            "TabBackground", typeof(Brush), typeof(TabDisplayItem),
            new PropertyMetadata(new SolidColorBrush(Colors.Transparent), new PropertyChangedCallback(OnInternalTabBackgroundChanged)));

        /// <summary>
        /// Get or set the brush used for the custom background of this tab. Taken from <see cref="TabItem.TabBackground"/>.
        /// </summary>
        public Brush TabBackground { get => (Brush)GetValue(TabBackgroundProperty); set => SetValue(TabBackgroundProperty, value); }

        private static void OnInternalTabBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TabDisplayItem s)
            {
                s.InternalTabBackgroundChanged?.Invoke(s, e);
            }
        }
        private void tdi_InternalTabBackgroundChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            
        }

        /// <summary>
        /// Internal event for handling a property changed. Please view the event that is not prefixed as "Internal".
        /// </summary>
        protected event DependencyPropertyChangedEventHandler InternalTabBackgroundChanged;
        #endregion

        private Thickness TabBorderThickSelected = new Thickness(1, 1, 1, 0);

        #endregion

        #region CanSelect

        /// <summary>
        /// A dependency property object backing the related property. See the property itself for more details.
        /// </summary>
        public static readonly DependencyProperty CanSelectProperty = DependencyProperty.Register("CanSelect", typeof(bool), typeof(TabDisplayItem),
            new PropertyMetadata(true));

        /// <summary>
        /// Get or set if the tab can be selected.
        /// </summary>
        public bool CanSelect
        {
            get { return (bool)GetValue(CanSelectProperty); }
            set { SetValue(CanSelectProperty, value); }
        }
        #endregion

        #region IsDirty

        /// <summary>
        /// A dependency property object backing the related property. See the property itself for more details.
        /// </summary>
        public static readonly DependencyProperty IsDirtyProperty = DependencyProperty.Register("IsDirty", typeof(bool), typeof(TabDisplayItem),
            new PropertyMetadata(false));

        /// <summary>
        /// Get or set if this tab is dirty. This can be used to visually indicate, for example, unsaved changes in the tab's contents. 
        /// </summary>
        public bool IsDirty
        {
            get { return (bool)GetValue(IsDirtyProperty); }
            set { SetValue(IsDirtyProperty, value); }
        }

        #endregion

        #region Commented Out Code

        //#region Icon

        //public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(ImageSource), typeof(TabDisplayItem),
        //    new PropertyMetadata(null));

        //public ImageSource Icon
        //{
        //    get { return (ImageSource)GetValue(IconProperty); }
        //    set { SetValue(IconProperty, value); }
        //}

        //#endregion


        //#region Title

        //public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(TabDisplayItem),
        //    new PropertyMetadata("New Tab"));

        //public string Title
        //{
        //    get { return (string)GetValue(TitleProperty); }
        //    set { SetValue(TitleProperty, value); }
        //}
        //#endregion

        //#region CanClose

        //public static readonly DependencyProperty CanCloseProperty = DependencyProperty.Register("CanClose", typeof(bool), typeof(TabDisplayItem),
        //    new PropertyMetadata(true));

        //public bool CanClose
        //{
        //    get { return (bool)GetValue(CanCloseProperty); }
        //    set { SetValue(CanCloseProperty, value); }
        //}
        //#endregion

        #endregion

        #region IsSelected

        /// <summary>
        /// A dependency property object backing the related property. See the property itself for more details.
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(TabDisplayItem),
            new PropertyMetadata(false, OnIsSelectedChanged));

        /// <summary>
        /// Get or set if this tab is selected. A selected tab will have visual differences to show that it is selected.
        /// </summary>
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

        /// <summary>
        /// Internal event for handling a property changed. Please view the event that is not prefixed as "Internal".
        /// </summary>
        protected event DependencyPropertyChangedEventHandler InternalIsSelectedChanged;

        private void tdi_InternalIsSelectedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            border.BorderThickness = IsSelected ? TabBorderThickSelected : new Thickness(1, 1, 1, 1);
            //border.Background = IsSelected ? SelectedTabBackground : Background;
        }

#if NETCOREAPP
        private void tab_IsSelectedChanged(object? sender, EventArgs e)
#else
        private void tab_IsSelectedChanged(object sender, EventArgs e)
#endif
        {
            border.BorderThickness = IsSelected ? TabBorderThickSelected : new Thickness(1, 1, 1, 1);
            //border.Background = IsSelected ? SelectedTabBackground : Background;
        }

        #endregion

        #region TabItem


        /// <summary>
        /// A dependency property object backing the related property. See the property itself for more details.
        /// </summary>
        public static readonly DependencyProperty TabItemProperty = DependencyProperty.Register("TabItem", typeof(TabItem), typeof(TabDisplayItem),
            new PropertyMetadata(null, new PropertyChangedCallback(OnInternalTabItemChanged)));

        /// <summary>
        /// The TabItem that this TabDisplayItem is representing. It is not advisable to change this property after the control is loaded; instead, just create a new TabDisplayItem.
        /// </summary>
        public TabItem TabItem
        {
            get { return (TabItem)GetValue(TabItemProperty); }
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
            if (d is TabDisplayItem s)
            {
                s.InternalTabItemChanged?.Invoke(s, e);
            }
        }

        private void tdi_InternalTabItemChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (TabItem != null)
            {
                TabItem.RequestTabClosing += TabItem_InternalTabClosing;
                TabItem.BringIntoViewRequested += TabItem_InternalBringIntoViewRequested;
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

        /// <summary>
        /// A dependency property object backing the related property. See the property itself for more details.
        /// </summary>
        public static readonly DependencyProperty ShowTabsOnBottomProperty = DependencyProperty.Register("ShowTabsOnBottom", typeof(bool), typeof(TabDisplayItem),
            new PropertyMetadata(false, new PropertyChangedCallback(OnInternalShowTabsOnBottomChanged)));

        /// <summary>
        /// Get or set if the parent tab control has its ShowTabsOnBottom property set.
        /// </summary>
        /// <remarks>
        /// This setting is used as there are some visual differences, depending upon if the tab list is on the top or bottom of the tab control.
        /// </remarks>
        public bool ShowTabsOnBottom
        {
            get { return (bool)GetValue(ShowTabsOnBottomProperty); }
            set { SetValue(ShowTabsOnBottomProperty, value); }
        }

        /// <summary>
        /// Internal event for handling a property changed. Please view the event that is not prefixed as "Internal".
        /// </summary>
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

        #region ParentTabControl

        /// <summary>
        /// A dependency property object backing the related property. See the property itself for more details.
        /// </summary>
        public static readonly DependencyProperty ParentTabControlProperty = DependencyProperty.Register("ParentTabControl", typeof(TabControl), typeof(TabDisplayItem),
            new PropertyMetadata(null, OnInternalParentChanged));

        /// <summary>
        /// Get or set the parent TabControl item that holds this tab item.
        /// </summary>
        public TabControl ParentTabControl
        {
            get { return (TabControl)GetValue(ParentTabControlProperty); }
            set { SetValue(ParentTabControlProperty, value); }
        }

        /// <summary>
        /// Internal event for handling a property changed. Please view the event that is not prefixed as "Internal".
        /// </summary>
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
            ParentTabControl?.SetupTabDisplay(this);
        }

        #endregion

        #region Color Scheme

        /// <summary>
        /// A dependency property object backing the related ColorScheme property. See <see cref="ColorScheme"/> for more details.
        /// </summary>
        public static readonly DependencyProperty ColorSchemeProperty
            = DependencyProperty.Register("ColorScheme", typeof(ColorScheme), typeof(TabDisplayItem),
            new FrameworkPropertyMetadata(new ColorScheme(), new PropertyChangedCallback(OnColorSchemeChanged)));

        /// <summary>
        /// Perform an action when the ColorScheme property has changed. Primarily used internally.
        /// </summary>
        /// <param name="d">The object containing the property that changed.</param>
        /// <param name="e">Event arguments about the property change.</param>
        public static void OnColorSchemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is ColorScheme cs)
            {
                if (d is TabDisplayItem tdi)
                {
                    tdi.ApplyColorScheme(cs);
                }
            }
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
                TabBorderBrush = cs.BorderColor.ToBrush();
                HighlightBrush = cs.HighlightColor.ToBrush();
                BorderHighlightBrush = cs.BorderColor.ToBrush();
                CloseBrush = cs.BorderColor.ToBrush();
            }
            else
            {
                Background = cs.ThirdHighlightColor.ToBrush();
                TabBorderBrush = cs.BorderColor.ToBrush();
                HighlightBrush = cs.SecondHighlightColor.ToBrush();
                BorderHighlightBrush = cs.HighlightColor.ToBrush();
                CloseBrush = cs.ForegroundColor.ToBrush();
            }

            //if (highlighting)
            //{
            //    border.Background = HighlightBrush;
            //    border.BorderBrush = BorderHighlightBrush;
            //}
            //else
            //{
            //    border.Background = IsSelected ? SelectedTabBackground : Background;
            //    border.BorderBrush = TabBorderBrush;
            //}
        }
        #endregion

        #region Click Handling

        #region Variables/Properties

        bool initiatingClick = false;

        /// <summary>
        /// Get if this TabDisplayItem is currently highlighted (i.e. has focus or mouse over).
        /// </summary>
        public bool IsHighlighted { get => (bool)GetValue(IsHighlightedProperty); private set => SetValue(IsHighlightedPropertyKey, value); }

        private static readonly DependencyPropertyKey IsHighlightedPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(IsHighlighted), typeof(bool), typeof(TabDisplayItem),
            new FrameworkPropertyMetadata(false));

        /// <summary>The backing dependency property for <see cref="IsHighlighted"/>. See the related property for details.</summary>
        public static readonly DependencyProperty IsHighlightedProperty = IsHighlightedPropertyKey.DependencyProperty;


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

        // bool highlighting = false;

        private void UserControl_GotFocus(object sender, RoutedEventArgs e)
        {
            if (IsEnabled && CanSelect)
            {
                IsHighlighted = true;

                //border.Background = HighlightBrush;
                //border.BorderBrush = BorderHighlightBrush;
                //highlighting = true;
            }
        }

        private void UserControl_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (IsEnabled && CanSelect)
            {
                IsHighlighted = true;

                //border.Background = HighlightBrush;
                //border.BorderBrush = BorderHighlightBrush;
                //highlighting = true;
            }
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            if (IsEnabled && CanSelect)
            {
                IsHighlighted = true;

                //border.Background = HighlightBrush;
                //border.BorderBrush = BorderHighlightBrush;
                //highlighting = true;
            }
        }

        private void UserControl_LostFocus(object sender, RoutedEventArgs e)
        {

            //border.Background = IsSelected ? SelectedTabBackground : Background;
            //border.BorderBrush = TabBorderBrush;
            //highlighting = false;

            IsHighlighted = false;
            initiatingClick = false;
        }

        private void UserControl_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            //border.Background = IsSelected ? SelectedTabBackground : Background;
            //border.BorderBrush = TabBorderBrush;
            //highlighting = false;

            IsHighlighted = false;
            initiatingClick = false;
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!IsKeyboardFocused)
            {
            //    border.Background = IsSelected ? SelectedTabBackground : Background;
            //    border.BorderBrush = TabBorderBrush;
            //    highlighting = false;
                IsHighlighted = false;
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

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            RequestClose?.Invoke(this, e);
        }

        #region Drag and Drop

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly DependencyProperty AllowDragDropProperty = DependencyProperty.Register("AllowDragDrop", typeof(bool), typeof(TabDisplayItem),
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
        public static readonly DependencyProperty AllowDataDragDropProperty = DependencyProperty.Register("AllowDataDragDrop", typeof(bool), typeof(TabDisplayItem),
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
            if (AllowDragDrop && e.Data.GetData(typeof(TabItem)) != null)
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
            else if (AllowDataDragDrop)
            {
                // raise TabItem.DragEnter
                TabItem.RaiseDragEvent("DragEnter", e);
            }
        }

        private void control_Drop(object sender, DragEventArgs e)
        {
            if (AllowDragDrop && e.Data.GetData(typeof(TabItem)) != null)
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
            if (AllowDragDrop && e.Data.GetData(typeof(TabItem)) != null)
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
            if (AllowDragDrop && e.Data.GetData(typeof(TabItem)) != null)
            {
                if (e.Data.GetData(typeof(TabItem)) == TabItem)
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
            if (AllowDragDrop && e.Data.GetData(typeof(TabItem)) != null)
            {

            }
            else if (AllowDataDragDrop)
            {
                TabItem.RaiseDragEvent("PreviewDragLeave", e);
            }
        }

        private void control_PreviewDragOver(object sender, DragEventArgs e)
        {
            if (AllowDragDrop && e.Data.GetData(typeof(TabItem)) != null)
            {

            }
            else if (AllowDataDragDrop)
            {
                TabItem.RaiseDragEvent("PreviewDragOver", e);
            }
        }

        private void control_PreviewDrop(object sender, DragEventArgs e)
        {
            if (AllowDragDrop && e.Data.GetData(typeof(TabItem)) != null)
            {

            }
            else if (AllowDataDragDrop)
            {
                TabItem.RaiseDragEvent("PreviewDrop", e);
            }
        }

        private void control_DragOver(object sender, DragEventArgs e)
        {
            if (AllowDragDrop && e.Data.GetData(typeof(TabItem)) != null)
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

        #endregion
    }

    /// <summary>
    /// Event arguments for a TabItem being dropped onto a TabControl. Primarily used internally.
    /// </summary>
    public class TabItemDropEventArgs
    {
        /// <summary>
        /// Create a TabItemDropEventArgs.
        /// </summary>
        /// <param name="sourceTabItem">The tab item that triggered the event.</param>
        /// <param name="droppedTabItem">The tab item to be dropped.</param>
        /// <param name="before">Determine whether to drop the <paramref name="droppedTabItem"/> before or after the <paramref name="sourceTabItem"/>.</param>
        public TabItemDropEventArgs(TabItem sourceTabItem, TabItem droppedTabItem, bool before)
        {
            SourceTabItem = sourceTabItem;
            DroppedTabItem = droppedTabItem;
#pragma warning disable CS0618 // Type or member is obsolete
            Before = before;
#pragma warning restore CS0618 // Type or member is obsolete
            PlaceBefore = before;
        }

        /// <summary>
        /// Get the TabItem that triggered the TabItemDrop event.
        /// </summary>
        public TabItem SourceTabItem { get; private set; }

        /// <summary>
        /// Get the TabItem that is being dropped.
        /// </summary>
        public TabItem DroppedTabItem { get; private set; }

        /// <summary>
        /// Get whether the dropped TabItem should be put before or after the source TabItem. 
        /// (Please use <see cref="PlaceBefore"/> instead, as this property will be removed in the future.)
        /// </summary>
        [Obsolete("Please use the PlaceBefore property instead going forward. This property will be removed in a future version.", false)]
        public bool Before { get; private set; }

        /// <summary>
        /// Get whether the dropped TabItem should be put before or after the source TabItem.
        /// </summary>
        public bool PlaceBefore { get; private set; }
    }
}