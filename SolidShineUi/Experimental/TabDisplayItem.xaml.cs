using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SolidShineUi.Experimental
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
#else
        public event EventHandler RequestClose;
        public event EventHandler RightClick;
        public event EventHandler Click;
#endif

        public Brush HighlightBrush { get; set; } = new SolidColorBrush(Colors.LightGray);
        public Brush BorderHighlightBrush { get; set; } = new SolidColorBrush(Colors.DimGray);
        public new Brush BorderBrush { get; set; } = new SolidColorBrush(Colors.Black);

        public TabDisplayItem()
        {
            InitializeComponent();
            TabItem = new TabItem();
            TabItem tab = TabItem;

            //if (tab.Icon != null)
            //{
            //    imgIcon.Source = tab.Icon;
            //}
            //lblTitle.Text = tab.Title;
            if (tab.IsSelected)
            {
                border.BorderThickness = new Thickness(1, 1, 1, 0);
            }
            else
            {
                border.BorderThickness = new Thickness(1, 1, 1, 1);
            }

            //btnClose.Visibility = tab.CanClose ? Visibility.Visible : Visibility.Collapsed;
            //colClose.Width = tab.CanClose ? new GridLength(18) : new GridLength(0);

            //tab.IsSelectedChanged += tab_IsSelectedChanged;
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
            if (tab.IsSelected)
            {
                border.BorderThickness = new Thickness(1, 1, 1, 0);
            }
            else
            {
                border.BorderThickness = new Thickness(1, 1, 1, 1);
            }

            //btnClose.Visibility = tab.CanClose ? Visibility.Visible : Visibility.Collapsed;
            //colClose.Width = tab.CanClose ? new GridLength(18) : new GridLength(0);

            tab.IsSelectedChanged += tab_IsSelectedChanged;
        }

#if NETCOREAPP
        private void tab_IsSelectedChanged(object? sender, EventArgs e)
#else
        private void tab_IsSelectedChanged(object sender, EventArgs e)
#endif
        {
            if (TabItem.IsSelected)
            {
                border.BorderThickness = new Thickness(1, 1, 1, 0);
            }
            else
            {
                border.BorderThickness = new Thickness(1, 1, 1, 1);
            }
        }

        #region TabItem

        private static readonly DependencyPropertyKey TabItemPropertyKey = DependencyProperty.RegisterReadOnly("TabItem", typeof(TabItem), typeof(TabDisplayItem),
            new PropertyMetadata(null));

        public static readonly DependencyProperty TabItemProperty = TabItemPropertyKey.DependencyProperty;

        public TabItem TabItem
        {
            get { return (TabItem)GetValue(TabItemProperty); }
            protected set { SetValue(TabItemPropertyKey, value); }
        }
        #endregion

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            RequestClose?.Invoke(this, e);
        }

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
    }
}
