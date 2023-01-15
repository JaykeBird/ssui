using System.Windows;
using System.Windows.Media;
using static SolidShineUi.ChromeButtons;
using System.Windows.Shell;
//using Microsoft.Windows.Shell;
using System.ComponentModel;
using System;
using System.Windows.Input;
using System.Windows.Interop;
using System.Diagnostics;

namespace SolidShineUi
{
    /// <summary>
    /// A WPF window that has a custom, flat style and additional functionality.
    /// </summary>
    public class FlatWindow : Window
    {
        static FlatWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FlatWindow), new FrameworkPropertyMetadata(typeof(FlatWindow)));
        }

        /// <summary>
        /// Create a new FlatWindow.
        /// </summary>
        public FlatWindow()
        {
            //InternalCornerRadiusChanged += flatWindow_InternalCornerRadiusChanged;
            CommandBindings.Add(new CommandBinding(FlatWindowCommands.CloseWindow, OnCloseWindow));
            CommandBindings.Add(new CommandBinding(FlatWindowCommands.Minimize, OnMinimizeWindow));
            CommandBindings.Add(new CommandBinding(FlatWindowCommands.Maximize, OnMaximizeWindow));
            CommandBindings.Add(new CommandBinding(FlatWindowCommands.Restore, OnRestoreWindow));
            CommandBindings.Add(new CommandBinding(FlatWindowCommands.DisplaySystemMenu, OnShowSystemMenu));
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            HwndSource hwnd = (HwndSource)PresentationSource.FromVisual(this);
            hwnd.AddHook(WndProc);
            base.OnSourceInitialized(e);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            //Window window = (Window)HwndSource.FromHwnd(hwnd).RootVisual;

            // https://learn.microsoft.com/en-us/windows/apps/desktop/modernize/apply-snap-layout-menu#how-do-i-fix-it
            if (msg == 0x0084) // NCHITTEST (asking what this point is in relation to the window)
            {
                
                Debug.Print("ASKING NCHITTEST");
                // lParam stores the coordinates the message is asking about, so let's get the coordinates out
                // https://learn.microsoft.com/en-us/windows/win32/inputdev/wm-nchittest
                // https://stackoverflow.com/questions/49288552/how-do-i-read-the-win32-wm-move-lparam-x-y-coordinates-in-c
                uint lparam32 = (uint)lParam.ToInt64(); // We want the bottom unsigned 32-bits
                Point p = PointFromScreen(new Point(lparam32 & 0xffff, (lparam32 >> 16) & 0xffff));

                if (TestIfPointIsMaximizeButton(p))
                {
                    handled = true;
                    return new IntPtr(9);
                }
            }

            return IntPtr.Zero;
        }

        public bool TestIfPointIsMaximizeButton(Point p)
        {
            if (p.Y > CaptionHeight + 3) return false;

            double borderTop = BorderThickness.Top;
            double borderRight = BorderThickness.Right;
            // next, let's get the width of a button
            int captionFontSize = 16; // the caption font size is hardset in the theme, in the future maybe I'll open it to be changed
            double btnWidth = captionFontSize + CaptionButtonPadding.Left + CaptionButtonPadding.Right;
            double btnHeight = captionFontSize + CaptionButtonPadding.Top + CaptionButtonPadding.Bottom;

            double maxButtonRightBound = ActualWidth - borderRight - btnWidth; // this is the right edge of the maximize/restore button
            double maxButtonLeftBound = maxButtonRightBound - btnWidth; // this is the left edge of the maximize/restore button
            double maxButtonTopBound = borderTop;
            double maxButtonBottomBound = borderTop + btnHeight;

            if (p.X > maxButtonLeftBound && p.X < maxButtonRightBound && p.Y > maxButtonTopBound && p.Y < maxButtonBottomBound)
            {
                Debug.Print("MAX BUTTON");
                return true; // MAXIMIZE BUTTON
            }
            else
            {
                return false; // let the main function handle this
            }
        }

        #region Color Scheme
        /// <summary>
        /// Get if the window's color scheme is a high contrast theme (and thus, the window is displaying high-contrast colors).
        /// </summary>
        public bool HighContrastMode { get; private set; } = false;

        // TODO: add different color for inactive window caption (especially for High Contrast Mode)

        /// <summary>
        /// Raised when the ColorScheme property is changed.
        /// </summary>
#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? ColorSchemeChanged;
#else
        public event DependencyPropertyChangedEventHandler ColorSchemeChanged;
#endif

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly DependencyProperty ColorSchemeProperty
            = DependencyProperty.Register("ColorScheme", typeof(ColorScheme), typeof(FlatWindow),
            new FrameworkPropertyMetadata(new ColorScheme(), new PropertyChangedCallback(OnColorSchemeChanged)));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Perform an action when the ColorScheme property has changed. Primarily used internally.
        /// </summary>
        /// <param name="d">The object containing the property that changed.</param>
        /// <param name="e">Event arguments about the property change.</param>
        public static void OnColorSchemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is ColorScheme cs)
            {
                if (d is FlatWindow w)
                {
                    w.ApplyColorScheme(cs);
                    w.ColorSchemeChanged?.Invoke(d, e);
                }
            }

            //#if NETCOREAPP
            //            if (d is FlatWindow w)
            //            {
            //                w.ApplyColorScheme((e.NewValue as ColorScheme));
            //                w.ColorSchemeChanged?.Invoke(w, e);
            //            }
            //#else
            //            (d as FlatWindow).ApplyColorScheme(e.NewValue as ColorScheme);
            //            (d as FlatWindow).ColorSchemeChanged?.Invoke(
            //#endif
        }

        /// <summary>
        /// Get or set the color scheme to apply to the window. The color scheme can quickly apply a whole visual style to your control.
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
                HighContrastMode = true;
                InactiveTextBrush = cs.WindowTitleBarTextColor.ToBrush();
            }
            else
            {
                InactiveTextBrush = ColorsHelper.CreateFromHex("#505050").ToBrush();
            }

            Background = cs.WindowTitleBarColor.ToBrush();
            InactiveBackground = cs.WindowInactiveColor.ToBrush();
            CaptionButtonsBrush = cs.WindowTitleBarTextColor.ToBrush();
            CaptionTextBrush = cs.WindowTitleBarTextColor.ToBrush();
            BorderBrush = cs.BorderColor.ToBrush();
            HighlightBrush = cs.HighlightColor.ToBrush();
            SelectionBrush = cs.SelectionColor.ToBrush();
            Foreground = cs.ForegroundColor.ToBrush();
            ContentBackground = cs.BackgroundColor.ToBrush();
        }

        /// <summary>
        /// Apply a color scheme to this control. The color scheme can quickly apply a whole visual style to the control.
        /// </summary>
        /// <param name="hco">The high-contrast color scheme to apply.</param>
        [Obsolete("This overload of the ApplyColorScheme method will be removed in the future. Please use the other ApplyColorScheme method, " +
            "and use ColorScheme.GetHighContrastScheme to get the desired high-contrast scheme.", false)]
        public void ApplyColorScheme(HighContrastOption hco)
        {
            ColorScheme cs = ColorScheme.GetHighContrastScheme(hco);

            ApplyColorScheme(cs);
        }
        #endregion

        #region Layout/Appearance Properties

        int captionHeight = 29;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly DependencyProperty CaptionDisplayTypeProperty = DependencyProperty.Register(
            "CaptionDisplayType", typeof(CaptionType), typeof(FlatWindow),
            new PropertyMetadata(CaptionType.Full));

        public static readonly DependencyProperty CaptionButtonPaddingProperty = DependencyProperty.Register(
            "CaptionButtonPadding", typeof(Thickness), typeof(FlatWindow),
            new PropertyMetadata(new Thickness(9, 7, 9, 7)));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Gets or sets the visibility of the caption buttons (close, maximize, minimize). The actions may still be available via other methods even if the buttons are hidden.
        /// </summary>
        /// <remarks>
        /// Use the <c>DisableMinimizeAction</c> and <c>DisableMaximizeAction</c> methods to further prevent the user from being able to minimize or maximize the window.
        /// The standard <c>ResizeMode</c> property works as well.
        /// </remarks>
        public CaptionType CaptionDisplayType
        {
            get
            {
                return (CaptionType)GetValue(CaptionDisplayTypeProperty);
            }
            set
            {
                SetValue(CaptionDisplayTypeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the amount of padding to use with each of the caption buttons. A higher padding will make the buttons larger.
        /// </summary>
        /// <remarks>The default value is (9,7,9,7).</remarks>
        public Thickness CaptionButtonPadding
        {
            get
            {
                return (Thickness)GetValue(CaptionButtonPaddingProperty);
            }
            set
            {
                SetValue(CaptionButtonPaddingProperty, value);
            }
        }

        /// <summary>
        /// Get or set the height of the caption (title bar) area of the window. Default is 29.
        /// </summary>
        public int CaptionHeight
        {
            get
            {
                return captionHeight;
            }
            set
            {
                captionHeight = value;
                ResizeCaptionHeight(value);
            }
        }

        void ResizeCaptionHeight(int height)
        {
            // note to self: if updating this method, make sure this matches the WindowChrome settings in the Generic.xaml file.
            WindowChrome wc = new WindowChrome
            {
                CaptionHeight = height,
                CornerRadius = new CornerRadius(0),
                ResizeBorderThickness = new Thickness(4),
                GlassFrameThickness = new Thickness(0, 0, 0, 1)
            };

            WindowChrome.SetWindowChrome(this, wc);
        }

        //void ChangeChromeCornerRadius(CornerRadius radius)
        //{
        //    // note to self: if updating this method, make sure this matches the WindowChrome settings in the Generic.xaml file.
        //    WindowChrome wc = new WindowChrome
        //    {
        //        CaptionHeight = captionHeight,
        //        CornerRadius = radius,
        //        ResizeBorderThickness = new Thickness(4),
        //        GlassFrameThickness = new Thickness(0, 0, 0, 1)
        //    };

        //    WindowChrome.SetWindowChrome(this, wc);
        //}


        //public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
        //    "CornerRadius", typeof(CornerRadius), typeof(FlatWindow),
        //    new PropertyMetadata(new CornerRadius(0), new PropertyChangedCallback(OnInternalCornerRadiusChanged)));

        ///// <summary>
        ///// Get or set the corner radius (or radii) to use for the window and its border. Can be used to created rounded corners for windows (this should apply regardless of what version of Windows is being used).
        ///// </summary>
        //[Category("Appearance")]
        //public CornerRadius CornerRadius
        //{
        //    get => (CornerRadius)GetValue(CornerRadiusProperty);
        //    set => SetValue(CornerRadiusProperty, value);
        //}

        //protected event DependencyPropertyChangedEventHandler InternalCornerRadiusChanged;

        //private void flatWindow_InternalCornerRadiusChanged(object sender, DependencyPropertyChangedEventArgs e)
        //{
        //    ChangeChromeCornerRadius(CornerRadius);
        //}

        //private static void OnInternalCornerRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    if (d is FlatWindow f)
        //    {
        //        f.InternalCornerRadiusChanged?.Invoke(f, e);
        //    }
        //}

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly DependencyProperty TopRightElementProperty = DependencyProperty.Register(
            "TopRightElement", typeof(UIElement), typeof(FlatWindow),
            new PropertyMetadata(null));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Gets or sets the UI element to place in the top-right of the window, to the left of the caption buttons.
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

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly DependencyProperty TopLeftElementProperty = DependencyProperty.Register(
            "TopLeftElement", typeof(UIElement), typeof(FlatWindow),
            new PropertyMetadata(null));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Gets or sets the UI element to place in the top-left corner of the window.
        /// </summary>
        public UIElement TopLeftElement
        {
            get
            {
                return (UIElement)GetValue(TopLeftElementProperty);
            }
            set
            {
                SetValue(TopLeftElementProperty, value);
            }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly DependencyProperty ShowTitleProperty = DependencyProperty.Register(
            "ShowTitle", typeof(bool), typeof(FlatWindow),
            new PropertyMetadata(true));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Get or set if the <see cref="Window.Title"/> should be displayed at the top of the window.
        /// </summary>
        [Category("Appearance")]
        public bool ShowTitle
        {
            get
            {
                return (bool)GetValue(ShowTitleProperty);
            }
            set
            {
                SetValue(ShowTitleProperty, value);
            }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly DependencyProperty ShowIconProperty = DependencyProperty.Register(
            "ShowIcon", typeof(bool), typeof(FlatWindow),
            new PropertyMetadata(false));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Get or set if the <see cref="Window.Icon"/> should be displayed at the top-left of the window.
        /// </summary>
        [Category("Appearance")]
        public bool ShowIcon
        {
            get
            {
                return (bool)GetValue(ShowIconProperty);
            }
            set
            {
                SetValue(ShowIconProperty, value);
            }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly DependencyProperty ExcludeTopLeftElementFromChromeProperty = DependencyProperty.Register(
            "ExcludeTopLeftElementFromChrome", typeof(bool), typeof(FlatWindow),
            new PropertyMetadata(true));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Get or set if the top-left element should be considered part of the window chrome or not. Set to <c>true</c> to exclude it from the window chrome.
        /// </summary>
        /// <remarks>
        /// This changes the IsHitTestVisibleInChrome property for the top-left element. If set to true, the element can be interacted with.
        /// If set to false, the element cannot be interacted with, and clicking, dragging, etc. acts as if you're clicking on the window's title bar.
        /// </remarks>
        public bool ExcludeTopLeftElementFromChrome
        {
            get
            {
                return (bool)GetValue(ExcludeTopLeftElementFromChromeProperty);
            }
            set
            {
                SetValue(ExcludeTopLeftElementFromChromeProperty, value);
            }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly DependencyProperty ExcludeTopRightElementFromChromeProperty = DependencyProperty.Register(
            "ExcludeTopRightElementFromChrome", typeof(bool), typeof(FlatWindow),
            new PropertyMetadata(true));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Get or set if the top-right element should be considered part of the window chrome or not. Set to <c>true</c> to exclude it from the window chrome.
        /// </summary>
        /// <remarks>
        /// This changes the IsHitTestVisibleInChrome property for the top-right element. If set to true, the element can be interacted with.
        /// If set to false, the element cannot be interacted with, and clicking, dragging, etc. acts as if you're clicking on the window's title bar.
        /// </remarks>
        public bool ExcludeTopRightElementFromChrome
        {
            get
            {
                return (bool)GetValue(ExcludeTopRightElementFromChromeProperty);
            }
            set
            {
                SetValue(ExcludeTopRightElementFromChromeProperty, value);
            }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly new DependencyProperty BorderThicknessProperty = DependencyProperty.Register(
            "BorderThickness", typeof(Thickness), typeof(FlatWindow),
            new PropertyMetadata(new Thickness(1)));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Get or set the thickness of the border of the window. Setting the thickness to 0 means the border is not visible.
        /// </summary>
        [Category("Appearance")]
        public new Thickness BorderThickness
        {
            get
            {
                return (Thickness)GetValue(BorderThicknessProperty);
            }
            set
            {
                SetValue(BorderThicknessProperty, value);
            }
        }

        #endregion

        #region Brushes

        /// <summary>
        /// Get or set the brush used when one of the caption buttons (in the top-right on non-RTL systems) is being clicked.
        /// </summary>
        public Brush SelectionBrush
        {
            get
            {
                return (Brush)GetValue(SelectionBrushProperty);
            }
            set
            {
                SetValue(SelectionBrushProperty, value);
            }
        }

        /// <summary>
        /// Get or set the brush used when one of the caption buttons (in the top-right on non-RTL systems) has focus or has the mouse over it.
        /// </summary>
        public Brush HighlightBrush
        {
            get
            {
                return (Brush)GetValue(HighlightBrushProperty);
            }
            set
            {
                SetValue(HighlightBrushProperty, value);
            }
        }

        /// <summary>
        /// Get or set the background brush to use for the caption area (title bar). Use <c>ContentBackground</c> for the content area of the window.
        /// </summary>
        public new Brush Background
        {
            get
            {
                return (Brush)GetValue(BackgroundProperty);
            }
            set
            {
                SetValue(BackgroundProperty, value);
            }
        }

        /// <summary>
        /// Get or set the brush used for the icons of the caption buttons (in the top-right in non-RTL systems).
        /// </summary>
        public Brush CaptionButtonsBrush
        {
            get
            {
                return (Brush)GetValue(CaptionButtonsBrushProperty);
            }
            set
            {
                SetValue(CaptionButtonsBrushProperty, value);
            }
        }

        /// <summary>
        /// Get or set the brush used for the text in the caption area (title bar). This has no effect if <c>ShowTitle</c> is false.
        /// </summary>
        public Brush CaptionTextBrush
        {
            get
            {
                return (Brush)GetValue(CaptionTextBrushProperty);
            }
            set
            {
                SetValue(CaptionTextBrushProperty, value);
            }
        }

        /// <summary>
        /// Get or set the brush used for the border around the window.
        /// </summary>
        public new Brush BorderBrush
        {
            get
            {
                return (Brush)GetValue(BorderBrushProperty);
            }
            set
            {
                SetValue(BorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Get or set the brush used for the text in the caption area (title bar) when the window isn't focused/active. This has no effect if <c>ShowTitle</c> is false.
        /// </summary>
        public Brush InactiveTextBrush
        {
            get
            {
                return (Brush)GetValue(InactiveTextBrushProperty);
            }
            set
            {
                SetValue(InactiveTextBrushProperty, value);
            }
        }

        /// <summary>
        /// Get or set the brush used for the caption area (title bar) when the window isn't focused/active. This does not change the color of the content area of the window.
        /// </summary>
        public Brush InactiveBackground
        {
            get
            {
                return (Brush)GetValue(InactiveBackgroundProperty);
            }
            set
            {
                SetValue(InactiveBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Get or set the brush used for the background of the content area of the window (where most content and controls are displayed).
        /// </summary>
        public Brush ContentBackground
        {
            get
            {
                return (Brush)GetValue(ContentBackgroundProperty);
            }
            set
            {
                SetValue(ContentBackgroundProperty, value);
            }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly DependencyProperty SelectionBrushProperty = DependencyProperty.Register(
            "SelectionBrush", typeof(Brush), typeof(FlatWindow),
            new PropertyMetadata(new SolidColorBrush(Colors.Gray)));

        public static readonly DependencyProperty HighlightBrushProperty = DependencyProperty.Register(
            "HighlightBrush", typeof(Brush), typeof(FlatWindow),
            new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));

        public static readonly new DependencyProperty BackgroundProperty = DependencyProperty.Register(
            "Background", typeof(Brush), typeof(FlatWindow),
            new PropertyMetadata(new SolidColorBrush(ColorsHelper.White)));

        public static readonly DependencyProperty CaptionButtonsBrushProperty = DependencyProperty.Register(
            "CaptionButtonsBrush", typeof(Brush), typeof(FlatWindow),
            new PropertyMetadata(new SolidColorBrush(ColorsHelper.Black)));

        public static readonly DependencyProperty CaptionTextBrushProperty = DependencyProperty.Register(
            "CaptionTextBrush", typeof(Brush), typeof(FlatWindow),
            new PropertyMetadata(new SolidColorBrush(ColorsHelper.Black)));

        public static readonly new DependencyProperty BorderBrushProperty = DependencyProperty.Register(
            "BorderBrush", typeof(Brush), typeof(FlatWindow),
            new PropertyMetadata(new SolidColorBrush(ColorsHelper.Black)));

        public static readonly DependencyProperty InactiveTextBrushProperty = DependencyProperty.Register(
            "InactiveTextBrush", typeof(Brush), typeof(FlatWindow),
            new PropertyMetadata(new SolidColorBrush(Colors.DimGray)));

        public static readonly DependencyProperty InactiveBackgroundProperty = DependencyProperty.Register(
            "InactiveBackground", typeof(Brush), typeof(FlatWindow),
            new PropertyMetadata(new SolidColorBrush(Colors.DimGray)));

        public static readonly DependencyProperty ContentBackgroundProperty = DependencyProperty.Register(
            "ContentBackground", typeof(Brush), typeof(FlatWindow),
            new PropertyMetadata(new SolidColorBrush(Colors.DimGray)));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        #endregion

        #region Command Handling

        private void OnCloseWindow(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        private void OnMinimizeWindow(object sender, ExecutedRoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void OnMaximizeWindow(object sender, ExecutedRoutedEventArgs e)
        {
            WindowState = WindowState.Maximized;
        }

        private void OnRestoreWindow(object sender, ExecutedRoutedEventArgs e)
        {
            WindowState = WindowState.Normal;
        }

        private void OnShowSystemMenu(object sender, ExecutedRoutedEventArgs e)
        {
            if (ShowIcon)
            {
                Point p = PointToScreen(new Point(6, 26));
                DisplaySystemMenu(p);
            }
            else
            {
                Point p = PointToScreen(new Point(8, 26));
                DisplaySystemMenu(p);
            }
        }

        #endregion

        #region Native Methods interop
        /// <summary>
        /// Calls the native Windows method to disable the Maximize action (via the right-click menu and the like). For <see cref="FlatWindow"/>, 
        /// you'll want to update the CaptionDisplayType to remove the physical button.
        /// </summary>
        public void DisableMaximizeAction()
        {
            NativeMethods.DisableMaximizeAction(this);
        }

        /// <summary>
        /// Calls the native Windows method to disable the Minimize action (via the right-click menu and the like). For <see cref="FlatWindow"/>, 
        /// you'll want to update the CaptionDisplayType to remove the physical button.
        /// </summary>
        public void DisableMinimizeAction()
        {
            NativeMethods.DisableMinimizeAction(this);
        }

        /// <summary>
        /// Calls the native Windows method to disable the Minimize and Maximize actions (via the right-click menu and the like). For <see cref="FlatWindow"/>, 
        /// you'll want to update the CaptionDisplayType to remove the physical buttons.
        /// </summary>
        public void DisableMaximizeAndMinimizeActions()
        {
            NativeMethods.DisableMinimizeAndMaximizeActions(this);
        }

        /// <summary>
        /// Display the system menu for this window (i.e. the menu containing values like Move, Size, Minimize, and Close), at a specified point on the screen.
        /// </summary>
        /// <param name="p">The coordinates of the pixel at which to display this menu (relative to the screen).</param>
        public void DisplaySystemMenu(Point p)
        {
            // from https://stackoverflow.com/a/21830822/2987285
            IntPtr hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            IntPtr hmnu = NativeMethods.GetSystemMenu(hwnd, false);
            int cmd = NativeMethods.TrackPopupMenu(hmnu, 0x100, (int)p.X, (int)p.Y, 0, hwnd, IntPtr.Zero);
            if (cmd > 0) NativeMethods.SendMessage(hwnd, 0x112, (IntPtr)cmd, IntPtr.Zero);
        }
        #endregion
    }
}